/// Process valid endpoint DTO to domain model
module EvaluateTodo.Server.Dto.ToDomain

open EvaluateTodo.Server.Domain
open EvaluateTodo.Server.Dto.Validation

/// Combine patch requests into single operation
let consolidatePatchCmd (validPatch: ValidPatch) =
    // Some -> create update command, None -> do nothing
    let evaluateToCmd tCmd opVal =
        match opVal with
        | Some value -> tCmd value
        | None -> id

    let { ValidNewTitle = patchTitle
          ValidNewStatus = patchComplete
          ValidNewPriority = patchPriority } = validPatch

    evaluateToCmd TodoIt.changeTitle patchTitle
    >> evaluateToCmd TodoIt.changeStatus patchComplete
    >> evaluateToCmd TodoIt.changePriority patchPriority


