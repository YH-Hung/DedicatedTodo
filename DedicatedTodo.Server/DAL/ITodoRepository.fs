namespace EvaluateTodo.Server.DAL

open EvaluateTodo.Server.DbMapping
open Microsoft.FSharp.Collections

/// Decoupling Data access logic from implementation
type ITodoRepository =
    abstract member QueryAll: unit -> DbReturnCondition<ResizeArray<TodoEntity>>
    abstract member QueryFilteredAll: WhereCondition -> DbReturnCondition<ResizeArray<TodoEntity>>
    abstract member QueryById: int -> DbReturnCondition<TodoEntity option>
    abstract member InsertSingle: InsertTodo -> DbReturnCondition<int>
    abstract member DeleteById: int -> DbReturnCondition<int>
    abstract member UpdateEntity: bool*UpdateTodoContent -> DbReturnCondition<int>
