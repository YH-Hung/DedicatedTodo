/// Data access related procedures
module EvaluateTodo.Server.TodoNpgSql

open System
open EvaluateTodo.Server.DbMapping
open Dapper
open Npgsql
open Polly

/// Null from DB -> None
let private castDbQueryReturn recordFromDb =
    if (box recordFromDb) = null then None
    else Some recordFromDb

let private policy =
    Policy
        .Handle<NpgsqlException>(fun pgEx -> pgEx.IsTransient)
        .WaitAndRetry([ for i in 1..3 -> TimeSpan.FromSeconds i ])

let private safeDbOperation operation parameters =
    try
        fun () -> operation parameters
        |> policy.Execute
        |> Success
    with
    | :? NpgsqlException as ex when ex.IsTransient -> TransientFailure ex.ErrorCode
    | :? NpgsqlException as ex -> InvalidOperation ex.ErrorCode

let private baseQuerySqlStr =
    """SELECT id AS Id, title AS Title, is_complete AS IsComplete, priority_id AS PriorityId
       FROM todo.todos"""
    |> SelectClause

let private generateCmd (pair: QueryPair) =
    let { SelectStatement = SelectClause selectSql; Condition  = cond } = pair

    match cond with
    | ById para -> CommandDefinition($"{selectSql} WHERE id = @TodoId", para)
    | ByComplete para -> CommandDefinition($"{selectSql} WHERE is_complete = @IsComplete", para)
    | ByPriority para ->
        if para.PriorityId.HasValue then CommandDefinition($"{selectSql} WHERE priority_id = @PriorityId", para)
        else CommandDefinition($"{selectSql} WHERE priority_id IS NULL")
    | ByCompleteAndPriority para ->
        if para.PriorityId.HasValue then CommandDefinition($"{selectSql} WHERE is_complete = @IsComplete AND priority_id = @PriorityId", para)
        else CommandDefinition($"{selectSql} WHERE is_complete = @IsComplete AND priority_id IS NULL", para)

let queryAll (cn: NpgsqlConnection) =
    baseQuerySqlStr
    |> (fun (SelectClause sqlStr) -> sqlStr)
    |> safeDbOperation (cn.Query<TodoEntity> >> ResizeArray)

let queryAllBy (cn: NpgsqlConnection) (cond: WhereCondition) =
    { SelectStatement = baseQuerySqlStr
      Condition = cond }
    |> generateCmd
    |> safeDbOperation (cn.Query<TodoEntity> >> ResizeArray)

let queryById (cn: NpgsqlConnection) (id : int) =
    { SelectStatement = baseQuerySqlStr
      Condition = ById {| TodoId = id |} }
    |> generateCmd
    |> safeDbOperation (cn.QuerySingleOrDefault<TodoEntity> >> castDbQueryReturn)

let insertSingle (cn: NpgsqlConnection) (insertDto : InsertTodo) =
    let sql = """INSERT INTO todo.todos (title, is_complete, priority_id)
                 VALUES (@InsertTitle, false, @WithPriority)
                 RETURNING id"""

    CommandDefinition(sql, insertDto)
    |> safeDbOperation cn.QuerySingle<int>  // Use query to get return id

let deleteById (cn: NpgsqlConnection) (id : int) =
    let sql = """DELETE FROM todo.todos
                 WHERE id = @TodoId"""

    CommandDefinition(sql, {| TodoId = id |})
    |> safeDbOperation cn.Execute

let updateEntity (cn: NpgsqlConnection) (isComplete: bool, content) =
    let sql = """UPDATE todo.todos
                 SET title = @UpdateTitle, is_complete = @IsComplete, priority_id = @UpdatePriority
                 WHERE id = @TargetId"""

    let para = {| TargetId = content.TargetId
                  UpdateTitle = content.UpdateTitle
                  IsComplete = isComplete
                  UpdatePriority = content.UpdatePriorityId |}

    CommandDefinition(sql, para)
    |> safeDbOperation cn.Execute
