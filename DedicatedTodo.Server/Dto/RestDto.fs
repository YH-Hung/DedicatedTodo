namespace EvaluateTodo.Server.Dto.Rest

open System
open System.ComponentModel.DataAnnotations
open System.Text.Json.Serialization
open EvaluateTodo.Server.Util

/// Filter endpoint model (From Uri)
[<CLIMutable>]
type FilterTodo = {
    ByComplete: Nullable<bool>
    ByPriority: string
}

/// Post endpoint model
[<CLIMutable>]
type PostTodo = {
    [<Required>] [<StringLength(50)>] PostTitle: string
    PostPriority: string    // null, empty, whitespace -> NotAssigned
}

/// Patch endpoint model
[<CLIMutable>]
type PatchTodo = {
    [<StringLength(50)>] PatchTitle: string
    PatchComplete: Nullable<bool>
    PatchPriority: string   // null -> not patched, empty, whitespace -> NotAssign
}

/// Query response content
type TodoViewModel = {
    Id: int
    Title: string
    IsComplete: bool
    Priority: string
}

/// Map to HTTP status code
type HttpCode =
    | NoContentCode = 204
    | BadRequestCode = 400
    | NotFoundCode = 404
    | ServerErrorCode = 500


/// Unified error return, align with APIController model invalid format.
type ErrorViewModel = {
    [<JsonPropertyName("title")>] ErrorTitle: string
    [<JsonPropertyName("status")>] ErrorCode: HttpCode
    [<JsonPropertyName("errors")>] ErrorItems: ErrorCollection
}
