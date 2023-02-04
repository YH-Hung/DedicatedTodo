namespace EvaluateTodo.Server.Controllers

open EvaluateTodo.Server.DAL
open EvaluateTodo.Server.Flow
open EvaluateTodo.Server.Dto.Rest
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Mvc

[<ApiController>]   // 1. Must explicit specify route 2. Handle validation automatically 3. Infer binding attribute
[<Route("[controller]")>]
[<Produces("application/json")>]
type TodoController(repo: ITodoRepository) =
    inherit ControllerBase()

    /// Fetch all items
    [<HttpGet>]
    [<ProducesResponseType(typeof<ResizeArray<TodoViewModel>>, StatusCodes.Status200OK)>]
    [<ProducesResponseType(StatusCodes.Status204NoContent)>]
    member _.GetAll() = Railway.queryAll repo

    /// Fetch all satisfied items
    [<HttpGet("filter")>]
    [<ProducesResponseType(typeof<ResizeArray<TodoViewModel>>, StatusCodes.Status200OK)>]
    [<ProducesResponseType(StatusCodes.Status204NoContent)>]
    [<ProducesResponseType(typeof<ErrorViewModel>, StatusCodes.Status400BadRequest)>]
     member _.GetFilteredAll([<FromQuery>] filterModel: FilterTodo) = Railway.filterBy repo filterModel


    /// Fetch specified item by id
    [<HttpGet("{id}")>]
    [<ProducesResponseType(typeof<TodoViewModel>, StatusCodes.Status200OK)>]
    [<ProducesResponseType(typeof<ErrorViewModel>, StatusCodes.Status404NotFound)>]
    member _.GetById(id: int) = Railway.queryById repo id

    /// Add new item
    [<HttpPost>]
    [<ProducesResponseType(typeof<int>, StatusCodes.Status201Created)>]
    [<ProducesResponseType(typeof<ErrorViewModel>, StatusCodes.Status400BadRequest)>]
    member _.Post(postModel: PostTodo) = Railway.insertSingleTodo repo postModel

    /// Delete specified item by id
    [<HttpDelete("{id}")>]
    [<ProducesResponseType(StatusCodes.Status202Accepted)>]
    [<ProducesResponseType(typeof<ErrorViewModel>, StatusCodes.Status404NotFound)>]
    member _.Delete(id: int) = Railway.deleteById repo id

    /// Update specified item by id, only non-null fields would take effect.
    [<HttpPatch("{id}")>]
    [<ProducesResponseType(StatusCodes.Status202Accepted)>]
    [<ProducesResponseType(typeof<ErrorViewModel>, StatusCodes.Status400BadRequest)>]
    [<ProducesResponseType(typeof<ErrorViewModel>, StatusCodes.Status404NotFound)>]
    member _.Patch(id: int, patchModel: PatchTodo) = Railway.patchById repo id patchModel
