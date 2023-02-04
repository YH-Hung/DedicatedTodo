namespace DedicatedTodo.Server.DbMapping

open System

/// Projection of SELECT query
type TodoEntity = {
    Id: int
    Title: string
    IsComplete: bool
    PriorityId: Nullable<int>
}

/// Unified SQL where condition
type WhereCondition =
    | ById of {| TodoId: int |}
    | ByComplete of {| IsComplete: bool |}
    | ByPriority of {| PriorityId: Nullable<int> |}
    | ByCompleteAndPriority of {| IsComplete: bool; PriorityId: Nullable<int> |}

/// Select Clause part of SQL string
type SelectClause = SelectClause of string

/// Select Clause + Where Clause
type QueryPair = {
    SelectStatement: SelectClause
    Condition: WhereCondition
}

/// Parameters for INSERT INTO VALUES
type InsertTodo = {
    InsertTitle: string
    WithPriority: Nullable<int>
}

/// Parameters for UPDATE SET
type UpdateTodoContent = {
    TargetId: int
    UpdateTitle: string
    UpdatePriorityId: Nullable<int>
}

/// Unified db return type
type DbReturnCondition<'T> =
    | Success of 'T
    | TransientFailure of int
    | InvalidOperation of int
