/// Deliver View Model DTO for Frontend
module EvaluateTodo.Server.Dto.ToViewModel

open EvaluateTodo.Server.DbMapping
open EvaluateTodo.Server.Domain
open EvaluateTodo.Server.Dto.Rest
open EvaluateTodo.Server.Dto.Validation
open EvaluateTodo.Server.Util
open Microsoft.AspNetCore

/// Convert Priority id to value in view
let private priorityToStr =
    Validate.priorityInt
    >> (Result.defaultValue NotAssigned)    // ignore validation temporarily
    >> fun priority ->
        match priority with
        | NotAssigned -> "NotAssigned"
        | High -> "HIGH"
        | Medium -> "MEDIUM"
        | Low -> "LOW"

/// Map DB entity to view model
let entity2ViewModel (e: TodoEntity) : TodoViewModel =
    { Id = e.Id
      Title = e.Title
      IsComplete = e.IsComplete
      Priority = e.PriorityId |> priorityToStr }

let private returnError (code: HttpCode) items =
    { ErrorTitle = "Error during server processing"
      ErrorCode = code
      ErrorItems = items }

/// Assemble error view model then pass to typed bad request
let badRequest: ErrorCollection -> Http.IResult =
    (returnError HttpCode.BadRequestCode) >> Http.Results.BadRequest<ErrorViewModel>

/// Assemble error view model then pass to typed not found
let notFound: ErrorCollection -> Http.IResult =
    (returnError HttpCode.NotFoundCode) >> Http.Results.NotFound<ErrorViewModel>
