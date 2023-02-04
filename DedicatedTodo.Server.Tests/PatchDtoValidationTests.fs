module DedicatedTodo.Server.Tests.PatchDtoValidationTests

open System
open DedicatedTodo.Server.Dto.Rest
open DedicatedTodo.Server.Dto.Validation
open Xunit
open FsUnit.Xunit
open FsUnit.CustomMatchers

let validPriorityStr = [| "HIGH"; "MEDIUM"; "LOW"; "NOTASSIGNED"; null; ""; " " |]
let vpLength = validPriorityStr.Length

let emptyTitle = [| ""; "  " |]
let allIsComplete = [| Nullable(); true |> Nullable; false |> Nullable |]

let invalidPriority = [| "Cool3d"; "FindJob"; "Sleep" |]

let validTitleAndPriority =
    Generate.tripleArgument
        (fun i -> Generate.randomString i ()) (fun i -> allIsComplete[i % 3]) (fun i -> validPriorityStr[i % vpLength]) 50

let ivpLength = invalidPriority.Length

let invalidTitleWithValidPriority =
    Generate.tripleArgument
        (fun i -> Generate.randomString (i + 50) ()) (fun i -> allIsComplete[i % 3]) (fun i -> validPriorityStr[i % vpLength]) 50

let validTitleWithInvalidPriority =
    Generate.tripleArgument
        (fun i -> Generate.randomString i ()) (fun i -> allIsComplete[i % 3]) (fun i -> invalidPriority[i % ivpLength]) 50

let validPatchOk = Validate.patch >> Result.isOk
let validPatchError = Validate.patch >> Result.isError

[<Theory>]
[<InlineData(null, null, null)>]    // Record ctor not allow null string
let ``Patch fields allow null`` (title: string, isComplete: Nullable<bool>, priority: string) =
    { PatchTitle = title; PatchComplete = isComplete; PatchPriority = priority }
    |> validPatchOk
    |> should be True

[<Theory>]
[<MemberData(nameof validTitleAndPriority)>]
let ``Patch title has value and does not too long with valid priority pass validation`` (title: string, isComplete: Nullable<bool>, priority: string) =
    { PatchTitle = title; PatchComplete = isComplete; PatchPriority = priority }
    |> validPatchOk
    |> should be True

[<Theory>]
[<MemberData(nameof invalidTitleWithValidPriority)>]
let ``Patch title is too long do not pass`` (title: string, isComplete: Nullable<bool>, priority: string) =
    { PatchTitle = title; PatchComplete = isComplete; PatchPriority = priority }
    |> validPatchError
    |> should be True

[<Theory>]
[<MemberData(nameof validTitleWithInvalidPriority)>]
let ``Patch title has value and does not too long with invalid priority do not pass validation`` (title: string, isComplete: Nullable<bool>, priority: string) =
    { PatchTitle = title; PatchComplete = isComplete; PatchPriority = priority }
    |> validPatchError
    |> should be True
