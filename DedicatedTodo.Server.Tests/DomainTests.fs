module DedicatedTodo.Server.Tests.DomainTests

open DedicatedTodo.Server.Domain
open DedicatedTodo.Server.Util.PrimaryUtil
open Xunit
open FsUnit.Xunit
open FsUnit.CustomMatchers

let issueTodoContent (iid, title, priority) =
    { Id = TodoIdentity iid; Title = String50.getSample title; SelectedPriority = priority }

let commonArgs = (1, "sample title", High)
let commonSample = issueTodoContent commonArgs

let status = [| InCompleteTodo; CompletedTodo |]
let nS = status.Length
let priorities = [| High; Medium; Low; NotAssigned |]
let nP = priorities.Length

let randomTitleStatusPriority =
    Generate.tripleArgument
        (fun i -> Generate.randomString i ()) (fun i -> status[i % nS]) (fun i -> priorities[i % nP]) 10

[<Fact>]
let ``Mark a Incomplete Todo Done return a Complete Todo`` () =
    commonSample
    |> InCompleteTodo
    |> TodoIt.changeStatus MarkDone
    |> should equal (CompletedTodo commonSample)

[<Fact>]
let ``Revert a Complete Todo return an Incomplete Todo`` () =
    commonSample
    |> CompletedTodo
    |> TodoIt.changeStatus RevertDone
    |> should equal (InCompleteTodo commonSample)

[<Fact>]
let ``Mark a Complete Todo Done again still return a Complete Todo`` () =
    commonSample
    |> CompletedTodo
    |> TodoIt.changeStatus MarkDone
    |> should equal (CompletedTodo commonSample)

[<Fact>]
let ``Revert an Incomplete Todo again still return an Incomplete Todo`` () =
    commonSample
    |> InCompleteTodo
    |> TodoIt.changeStatus RevertDone
    |> should equal (InCompleteTodo commonSample)


[<Theory>]
[<MemberData(nameof randomTitleStatusPriority)>]
let ``Change title of a Todo return a Todo with new title`` (newTitle: string, statusCtor: TodoContent -> TodoItem, priority: Priority) =
    let iid = Generate.randomInt 10 ()
    issueTodoContent (iid, "Sample title", priority)
    |> statusCtor
    |> TodoIt.changeTitle (String50.getSample newTitle)
    |> should equal (statusCtor (issueTodoContent (iid, newTitle, priority)))

[<Theory>]
[<MemberData(nameof randomTitleStatusPriority)>]
let ``Change priority of a Todo return a Todo with new priority`` (title: string, statusCtor: TodoContent -> TodoItem, newPriority: Priority) =
    let iid = Generate.randomInt 10 ()
    issueTodoContent (iid, title, High)
    |> statusCtor
    |> TodoIt.changePriority newPriority
    |> should equal (statusCtor (issueTodoContent (iid, title, newPriority)))
