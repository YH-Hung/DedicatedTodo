namespace DedicatedTodo.Server.DAL

open DedicatedTodo.Server.DbMapping
open Microsoft.FSharp.Collections

/// Decoupling Data access logic from implementation for mock
type ITodoRepository =
    abstract member QueryAll: unit -> DbReturnCondition<ResizeArray<TodoEntity>>
    abstract member QueryFilteredAll: WhereCondition -> DbReturnCondition<ResizeArray<TodoEntity>>
    abstract member QueryById: int -> DbReturnCondition<TodoEntity option>
    abstract member InsertSingle: InsertTodo -> DbReturnCondition<int>
    abstract member DeleteById: int -> DbReturnCondition<int>
    abstract member UpdateEntity: bool * UpdateTodoContent -> DbReturnCondition<int>
