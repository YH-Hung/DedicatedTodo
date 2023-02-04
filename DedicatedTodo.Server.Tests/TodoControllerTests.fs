module TodoControllerTests

open System
open EvaluateTodo.Server.Controllers
open EvaluateTodo.Server.DAL
open EvaluateTodo.Server.DbMapping
open EvaluateTodo.Server.Dto.Rest
open Microsoft.AspNetCore
open Moq
open Xunit
open FsUnit.Xunit

let mockRepo = Mock<ITodoRepository>()
let controller = TodoController(mockRepo.Object)

[<Fact>]
let ``Query all has results return Ok`` () =
    mockRepo
        .Setup(fun r -> r.QueryAll())
        .Returns([{Id = 1; Title = "First Todo"; IsComplete = false; PriorityId = Nullable 1}]
                 |> ResizeArray<TodoEntity> |> Success)
        .Verifiable()


    controller.GetAll()
    |> should be instanceOfType<Http.HttpResults.Ok<ResizeArray<TodoViewModel>>>
    mockRepo.Verify()

[<Fact>]
let ``Query all get empty result return No Content`` () =
    mockRepo
        .Setup(fun r -> r.QueryAll())
        .Returns([] |> ResizeArray<TodoEntity> |> Success)
        .Verifiable()

    controller.GetAll()
    |> should be instanceOfType<Http.HttpResults.NoContent>
    mockRepo.Verify()

[<Fact>]
let ``Query an existed id return Ok`` () =
    let queryId = 1
    mockRepo
        .Setup(fun r -> r.QueryById queryId)
        .Returns({Id = queryId; Title = "First Todo"; IsComplete = false; PriorityId = Nullable 1}
                 |> Some |> Success)
        .Verifiable()

    controller.GetById queryId
    |> should be instanceOfType<Http.HttpResults.Ok<TodoViewModel>>
    mockRepo.Verify()

[<Fact>]
let ``Query an absent id return Not Found`` () =
    let queryId = 1
    mockRepo
        .Setup(fun r -> r.QueryById queryId)
        .Returns(None |> Success)
        .Verifiable()

    controller.GetById queryId
    |> should be instanceOfType<Http.HttpResults.NotFound<ErrorViewModel>>
    mockRepo.Verify()

[<Fact>]
let ``Filter has result return Ok`` () =
    mockRepo
        .Setup(fun r -> r.QueryFilteredAll (It.IsAny<WhereCondition>()))
        .Returns([{Id = 1; Title = "First Todo"; IsComplete = false; PriorityId = Nullable 1}]
                 |> ResizeArray<TodoEntity> |> Success)
        .Verifiable()

    { ByComplete = Nullable false; ByPriority = "HIGH" }
    |> controller.GetFilteredAll
    |> should be instanceOfType<Http.HttpResults.Ok<ResizeArray<TodoViewModel>>>
    mockRepo.Verify()

[<Fact>]
let ``Filter get empty result return NoContent`` () =
    mockRepo
        .Setup(fun r -> r.QueryFilteredAll (It.IsAny<WhereCondition>()))
        .Returns([] |> ResizeArray<TodoEntity> |> Success)
        .Verifiable()

    { ByComplete = Nullable false; ByPriority = "HIGH" }
    |> controller.GetFilteredAll
    |> should be instanceOfType<Http.HttpResults.NoContent>
    mockRepo.Verify()

[<Fact>]
let ``Make an invalid filter return BadRequest`` () =
    { ByComplete = Nullable false; ByPriority = "Not a High" }
    |> controller.GetFilteredAll
    |> should be instanceOfType<Http.HttpResults.BadRequest<ErrorViewModel>>
    mockRepo.Verify()

[<Fact>]
let ``Insert a valid todo return Created`` () =
    mockRepo
        .Setup(fun r -> r.InsertSingle (It.IsAny<InsertTodo>()))
        .Returns(Success 1)
        .Verifiable()

    { PostTitle = "Sample Title"; PostPriority = "Low" }
    |> controller.Post
    |> should be instanceOfType<Http.HttpResults.Created<int>>
    mockRepo.Verify()

[<Fact>]
let ``Insert an invalid todo return Bad Request`` () =
    mockRepo
        .Setup(fun r -> r.InsertSingle (It.IsAny<InsertTodo>()))
        .Returns(InvalidOperation 2000)
    |> ignore   // Would not invoke due to blocked by verification

    { PostTitle = "GOOD Title"; PostPriority = "Special" }
    |> controller.Post
    |> should be instanceOfType<Http.HttpResults.BadRequest<ErrorViewModel>>

[<Fact>]
let ``Delete an existed id return Accepted`` () =
    mockRepo
        .Setup(fun r -> r.DeleteById(It.IsAny<int>()))
        .Returns(Success 1)
        .Verifiable()

    controller.Delete 1
    |> should be instanceOfType<Http.HttpResults.Accepted>
    mockRepo.Verify()

[<Fact>]
let ``Delete an absent id return Not Found`` () =
    mockRepo
        .Setup(fun r -> r.DeleteById(It.IsAny<int>()))
        .Returns(Success -1)
        .Verifiable()

    controller.Delete 1
    |> should be instanceOfType<Http.HttpResults.NotFound<ErrorViewModel>>
    mockRepo.Verify()

[<Fact>]
let ``Do a valid patch return Accepted`` () =
    mockRepo
        .Setup(fun r -> r.QueryById(It.IsAny<int>()))
        .Returns({Id = 1; Title = "First Todo"; IsComplete = false; PriorityId = Nullable 1}
                 |> Some |> Success)
        .Verifiable()
    mockRepo
        .Setup(fun r -> r.UpdateEntity (It.IsAny<bool>(), It.IsAny<UpdateTodoContent>()))
        .Returns(Success 1)
        .Verifiable()

    (1, { PatchTitle = "New Title"; PatchComplete = Nullable true; PatchPriority = "MEDIUM" })
    |> controller.Patch
    |> should be instanceOfType<Http.HttpResults.Accepted>
    mockRepo.Verify()

[<Fact>]
let ``Patch an absent id return Not Found`` () =
    mockRepo
        .Setup(fun r -> r.QueryById(It.IsAny<int>()))
        .Returns(None |> Success)
        .Verifiable()
    mockRepo
        .Setup(fun r -> r.UpdateEntity (It.IsAny<bool>(), It.IsAny<UpdateTodoContent>()))
        .Returns(Success -1)
    |> ignore

    (1, { PatchTitle = "New Title"; PatchComplete = Nullable true; PatchPriority = "MEDIUM" })
    |> controller.Patch
    |> should be instanceOfType<Http.HttpResults.NotFound<ErrorViewModel>>
    mockRepo.Verify()

[<Fact>]
let ``Do an invalid Patch return Bad Request`` () =
    mockRepo
        .Setup(fun r -> r.QueryById(It.IsAny<int>()))
        .Returns({Id = 1; Title = "First Todo"; IsComplete = false; PriorityId = Nullable 1}
                 |> Some |> Success)
        .Verifiable()
    mockRepo
        .Setup(fun r -> r.UpdateEntity (It.IsAny<bool>(), It.IsAny<UpdateTodoContent>()))
        .Returns(Success 1)
    |> ignore

    (1, { PatchTitle = "New Title"; PatchComplete = Nullable true; PatchPriority = "Special" })
    |> controller.Patch
    |> should be instanceOfType<Http.HttpResults.BadRequest<ErrorViewModel>>
    mockRepo.Verify()
