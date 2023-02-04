/// Consolidate data flow via Railway oriented programming
module EvaluateTodo.Server.Flow.Railway

open EvaluateTodo.Server.DAL
open EvaluateTodo.Server.Util
open EvaluateTodo.Server.Dto
open EvaluateTodo.Server.Dto.Rest
open EvaluateTodo.Server.Dto.Validation
open Microsoft.AspNetCore

/// In case of not found is an error.
let private resultFromOption errMsg opVal =
    match opVal with
    | None -> Error errMsg
    | Some value -> Ok value

/// Map Result to Http Code
let private resultToHttp normalCtor errCtor result =
    match result with
    | Ok okVal -> okVal |> normalCtor
    | Error errorValue -> errorValue |> errCtor

/// Wrap query return with status
let private queryAllTemplate queryReturn =
    let normalFlow rList =
        if Seq.isEmpty rList then Http.Results.NoContent()
        else
            rList
            |> Seq.map ToViewModel.entity2ViewModel
            |> ResizeArray
            |> Http.Results.Ok<ResizeArray<TodoViewModel>>

    queryReturn
    |> ToDb.dbReturnConditionToResult
    |> resultToHttp normalFlow (fun _ -> Http.Results.StatusCode(500))

/// List all todos in DB
let queryAll (repo: ITodoRepository) = queryAllTemplate (repo.QueryAll())

/// List all todos in DB with specified filtering conditions
let filterBy (repo: ITodoRepository) filterModel =
    let successFlow =
        ToDb.filterDtoToWhereClause
        >> Option.map repo.QueryFilteredAll
        >> Option.defaultValue (repo.QueryAll())
        >> queryAllTemplate

    filterModel
    |> Validate.filter
    |> resultToHttp successFlow (ErrorItem.consolidate >> ToViewModel.badRequest)

/// Try to fetch the selected todoItem
let queryById (repo: ITodoRepository) id =
    repo.QueryById id
    |> ToDb.dbReturnConditionToResult
    |> Result.bind (resultFromOption [(TargetId, $"Todo with id {id} is not found")])
    |> resultToHttp (ToViewModel.entity2ViewModel >> Http.Results.Ok<TodoViewModel>) (ErrorItem.consolidate >> ToViewModel.notFound)

/// Try to insert a todoItem based on provided field values
let insertSingleTodo (repo: ITodoRepository) postModel =
    postModel
    |> Validate.post
    |> Result.map ToDb.postDtoToInsertParameter
    |> Result.bind (repo.InsertSingle >> ToDb.dbReturnConditionToResult)
    |> resultToHttp (fun newId -> Http.Results.Created<int>($"/{newId}", newId)) (ErrorItem.consolidate >> ToViewModel.badRequest)

/// Try to delete the selected todoItem
let deleteById (repo: ITodoRepository) id =
    repo.DeleteById id
    |> ToDb.dbReturnConditionToResult
    |> resultToHttp (fun _ -> Http.Results.Accepted()) (ErrorItem.consolidate >> ToViewModel.notFound)

/// Try to update the selected todoItem with provided field values
let patchById (repo: ITodoRepository) id patchModel =
    let normalFindProcessFlow opTodo =
        match opTodo with
        | Some qDto -> qDto |> Validate.thePatchedItem
        | None -> Error [(TargetId, $"Todo with id {id} is not found")]

    let toBePatchedTodo =
        repo.QueryById id
        |> ToDb.dbReturnConditionToResult
        |> Result.bind normalFindProcessFlow

    let validPatch = patchModel |> Validate.patch

    match toBePatchedTodo with
    | Error msg -> msg |> ErrorItem.consolidate |> ToViewModel.notFound
    | Ok todo ->
        match validPatch with
        | Error msg -> msg |> ErrorItem.consolidate |> ToViewModel.badRequest
        | Ok validPatch ->
            todo
            |> ToDomain.consolidatePatchCmd validPatch
            |> ToDb.todoItemExtractUpdateParameter
            |> repo.UpdateEntity
            |> ToDb.dbReturnConditionToResult
            |> resultToHttp (fun _ -> Http.Results.Accepted()) (ErrorItem.consolidate >> ToViewModel.badRequest)
