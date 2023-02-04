module EvaluatedTodo.Server.Tests.PostDtoValidationTests

open EvaluateTodo.Server.Dto.Rest
open EvaluateTodo.Server.Dto.Validation
open Xunit
open FsUnit.Xunit
open FsUnit.CustomMatchers

let validPriorityStr = [| "HIGH"; "MEDIUM"; "LOW"; "NOTASSIGNED"; null; ""; " " |]
let vpLength = validPriorityStr.Length

let emptyTitle = [| null; ""; "  " |]

let invalidPriority = [| "Cool3d"; "FindJob"; "Sleep" |]
let ivpLength = invalidPriority.Length

let validTitleAndPriority =
    Generate.doubleArgument
        (fun i -> Generate.randomString i ()) (fun i -> validPriorityStr[i % vpLength]) 50

let tooLongTitleWithValidPriority =
    Generate.doubleArgument
        (fun i -> Generate.randomString (i + 50) ()) (fun i -> validPriorityStr[i % vpLength]) 5

let emptyTitleWithValidPriority =
    Generate.doubleArgument
        (fun i -> emptyTitle[i % 3]) (fun i -> validPriorityStr[i % vpLength]) 5

let validTitleWithInvalidPriority =
    Generate.doubleArgument
        (fun i -> Generate.randomString i ()) (fun i -> invalidPriority[i % ivpLength]) 50

let validPostOk = Validate.post >> Result.isOk
let validPostError = Validate.post >> Result.isError

[<Theory>]
[<MemberData(nameof validTitleAndPriority)>]
let ``Post Title has value and does Not too long With Valid Priority pass validation`` (title: string, priority: string) =
    { PostTitle = title; PostPriority = priority }
    |> validPostOk
    |> should be True

[<Theory>]
[<MemberData(nameof tooLongTitleWithValidPriority)>]
[<MemberData(nameof emptyTitleWithValidPriority)>]
let ``Post Title is empty or too long do not pass validation`` (title: string, priority: string) =
    { PostTitle = title; PostPriority = priority }
    |> validPostError
    |> should be True

[<Theory>]
[<MemberData(nameof validTitleWithInvalidPriority)>]
let ``Post Title is Required and Not too long with invalid Priority do not pass validation`` (title: string, priority: string) =
    { PostTitle = title; PostPriority = priority }
    |> validPostError
    |> should be True
