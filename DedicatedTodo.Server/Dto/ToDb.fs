/// Map Domain and Valid endpoint Dto to DB related DTO
module DedicatedTodo.Server.Dto.ToDb

open System
open DedicatedTodo.Server.Domain
open DedicatedTodo.Server.Dto.Validation
open DedicatedTodo.Server.DbMapping
open DedicatedTodo.Server.Util
open DedicatedTodo.Server.Util.PrimaryUtil

/// Convert Priority domain type to value in DB
let private priorityToId priority =
    match priority with
    | High -> Nullable 1
    | Medium -> Nullable 2
    | Low -> Nullable 3
    | NotAssigned -> Nullable()

/// Convert valid filter model to SQL
let filterDtoToWhereClause (validFilter: ValidFilter) =
    let { ValidByStatus = completeOp; ValidByPriority = priorityOp } = validFilter
    match completeOp, priorityOp with
    | Some isComplete, Some priority ->
        {| IsComplete = isComplete; PriorityId = priority |> priorityToId |}
        |> ByCompleteAndPriority |> Some
    | Some isComplete, None -> {| IsComplete = isComplete |} |> ByComplete |> Some
    | None, Some priority -> {| PriorityId = priority |> priorityToId |} |> ByPriority |> Some
    | None, None -> None

/// Convert Valid post model to SQL parameter
let postDtoToInsertParameter (validPost: ValidPost) =
    { InsertTitle = validPost.ValidTitle |> String50.value
      WithPriority = validPost.ValidPriority |> priorityToId }

/// Map DB query / exec return condition to Result
let dbReturnConditionToResult dbExecCond =
    match dbExecCond with
    | Success result ->
        match (box result) with
        | :? int as rowNo -> if (rowNo > 0) then Ok result else Error [(DbExec, "No row affected")]
        | _ -> Ok result
    | TransientFailure code -> Error [(DbExec, $"Transient DB error code {code}")]
    | InvalidOperation code -> Error [(DbExec, $"DB exception error code {code}")]

let private extractId (TodoIdentity tId) = tId

/// Map Domain model to DB command parameters
let private extractTodoContent (content: TodoContent) =
    { TargetId = extractId content.Id
      UpdateTitle = content.Title |> String50.value
      UpdatePriorityId = content.SelectedPriority |> priorityToId }

/// Map Domain model to DB command parameters
let todoItemExtractUpdateParameter (todo: TodoItem) =
    match todo with
    | InCompleteTodo content -> false, (content |> extractTodoContent)
    | CompletedTodo content -> true, (content |> extractTodoContent)
