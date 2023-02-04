namespace EvaluateTodo.Server.Dto.Validation

open System
open EvaluateTodo.Server.DbMapping
open EvaluateTodo.Server.Domain
open EvaluateTodo.Server.Util.PrimaryUtil
open EvaluateTodo.Server.Util
open EvaluateTodo.Server.Dto.Rest

/// Validated Post inputs
type ValidPost = {
    ValidTitle: String50
    ValidPriority: Priority
}

/// Validated Patch inputs
type ValidPatch = {
    ValidNewTitle: String50 option
    ValidNewStatus: ChangeStatusCommand option
    ValidNewPriority: Priority option
}

/// Validated Filter inputs
type ValidFilter = {
    ValidByStatus: bool option
    ValidByPriority: Priority option
}

/// Validation functions
module Validate =

    /// Validate priority string in model from post / patch endpoints
    let priorityStr (pStr : string) =
        if pStr |> String.IsNullOrWhiteSpace then NotAssigned |> Ok
        else
            match pStr.Trim().ToUpper() with
            | "HIGH" -> High |> Ok
            | "MEDIUM" -> Medium |> Ok
            | "LOW" -> Low |> Ok
            | "NOTASSIGNED" -> NotAssigned |> Ok
            | _ -> Error [(PriorityStr, "Invalid priority string")]

    /// Validate priority id from DB
    let priorityInt (pInt: Nullable<int>) =
        pInt |> Option.ofNullable
        |> fun pOp ->
            match pOp with
            | None -> NotAssigned |> Ok
            | Some 1 -> High |> Ok
            | Some 2 -> Medium |> Ok
            | Some 3 -> Low |> Ok
            | _ -> Error [(PriorityId, "Invalid priority id;")]

    /// Validate post endpoint model
    let post postDto =
        let newTitle = postDto.PostTitle |> String50.ctor
        let newPriority = postDto.PostPriority |> priorityStr

        match newTitle, newPriority with
        | Ok title50, Ok priority -> Ok { ValidTitle = title50; ValidPriority = priority }
        | rt, rp -> rt +? rp

    /// Combine null handling with validation
    let private consumeNullable ctor nVal =
        match nVal with
        | null -> Ok None
        | _ ->
            nVal
            |> ctor
            |> fun validResult ->
                match validResult with
                | Ok r -> r |> Some |> Ok
                | Error msg -> Error msg

    /// Validate patch endpoint model
    let patch patchDto =
        let consumedTitle = patchDto.PatchTitle |> consumeNullable String50.ctor
        let consumeComplete =
            patchDto.PatchComplete
            |> Option.ofNullable
            |> Option.map (fun c -> if c then MarkDone else RevertDone)
            |> Ok

        let consumedPriority = patchDto.PatchPriority |> consumeNullable priorityStr

        match consumedTitle, consumeComplete, consumedPriority with
        | Ok patchTitle, Ok patchComplete, Ok patchPriority ->
            { ValidNewTitle = patchTitle
              ValidNewStatus = patchComplete
              ValidNewPriority = patchPriority }
            |> Ok
        | rt, rc, rp -> (rt +? rc +? rp)


    /// Validate select todoItem from DB
    let thePatchedItem (qDto: TodoEntity) =
        let consumedTitle = qDto.Title |> String50.ctor
        let consumedPriority = qDto.PriorityId |> priorityInt

        match consumedTitle, consumedPriority with
        | Ok titleInDb, Ok priorityInDb ->
            { Id = TodoIdentity qDto.Id
              Title = titleInDb
              SelectedPriority = priorityInDb }
            |> fun content ->
                if qDto.IsComplete then CompletedTodo content
                else InCompleteTodo content
            |> Ok
        | rt, rp -> rt +? rp

    /// Validate filter endpoint model
    let filter filterDto =
        let completeOp = filterDto.ByComplete |> Option.ofNullable
        let priorityCond = filterDto.ByPriority |> consumeNullable priorityStr

        match priorityCond with
        | Ok priorityOp -> Ok { ValidByStatus = completeOp; ValidByPriority = priorityOp }
        | Error rp -> Error rp
