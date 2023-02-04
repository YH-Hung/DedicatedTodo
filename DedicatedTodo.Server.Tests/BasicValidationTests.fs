module EvaluatedTodo.Server.Tests.BasicValidationTests

open System
open EvaluateTodo.Server.Util.PrimaryUtil
open EvaluateTodo.Server.Dto.Validation
open Xunit
open FsUnit.Xunit
open FsUnit.CustomMatchers

let validPriorityStr =
    [ "High"; "Medium"; "Low"; "NotAssigned"
      "high"; "medium"; "low"; "notassigned"
      "HIGH"; "MEDIUM"; "LOW"; "NOTASSIGNED" ]
    |> Generate.singleArgumentFromList

let invalidPriorityStr = [ "Special"; "H1gh"; "L0W" ] |> Generate.singleArgumentFromList

let validPriorityId =
    seq { box null; box 1; box 2; box 3 }
    |> Seq.map (fun o -> [| o |])

let invalidPriorityId = [4; 5; 6] |> Generate.singleArgumentFromList

let emptyStr = [null; ""; " "] |> Generate.singleArgumentFromList

let validPriorityStrOk = Validate.priorityStr >> Result.isOk
let validPriorityIdOk = Validate.priorityInt >> Result.isOk

let validString50Ok = String50.ctor >> Result.isOk

[<Theory>]
[<MemberData(nameof validPriorityStr)>]
[<MemberData(nameof emptyStr)>]
let ``Valid priority string return Ok`` (priorityStr: string) =
    priorityStr
    |> validPriorityStrOk
    |> should be True

[<Theory>]
[<MemberData(nameof invalidPriorityStr)>]
let ``Invalid priority string return Error`` (priorityStr: string) =
    priorityStr
    |> validPriorityStrOk
    |> should not' (contain true)

[<Theory>]
[<MemberData(nameof validPriorityId)>]
let ``Valid priority id return Ok`` (priorityId: Nullable<int>) =
    priorityId
    |> validPriorityIdOk
    |> should not' (contain false)

[<Theory>]
[<MemberData(nameof invalidPriorityId)>]
let ``Invalid priority id return Error`` (priorityId: Nullable<int>) =
    priorityId
    |> validPriorityIdOk
    |> should not' (contain true)

[<Fact>]
let ``Valid title length return Ok`` () =
    String('a', 49)
    |> validString50Ok
    |> should be True

[<Fact>]
let ``Too long title length return Error`` () =
    String('a', 60)
    |> validString50Ok
    |> should not' (be True)

[<Theory>]
[<MemberData(nameof emptyStr)>]
let ``Empty and null title return Error`` (title: string) =
    title
    |> validString50Ok
    |> should not' (contain true)
