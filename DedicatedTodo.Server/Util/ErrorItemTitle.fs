namespace DedicatedTodo.Server.Util

open System.Collections.Generic

/// Error source category
type ErrorItemTitle = DbExec | PriorityStr | PriorityId | TitleStr | TargetId

/// Represent ApiController validation error format
type ErrorCollection = IDictionary<string, string array>

/// Transform and collect error message
module ErrorItem =
    let toTitleStr eit =
        match eit with
        | DbExec -> "DbExecution"
        | PriorityStr -> "PriorityString"
        | PriorityId -> "PriorityId"
        | TitleStr -> "TitleString"
        | TargetId -> "Id"

    let private folder (state: Map<ErrorItemTitle, string list>) (itemTitle, itemMsg) =
        let currentMsg = Map.tryFind itemTitle state
        let updateMsg =
            match currentMsg with
            | Some msgList -> itemMsg :: msgList
            | None -> [ itemMsg ]

        Map.add itemTitle updateMsg state

    /// Collect error messages into Dictionary
    let consolidate: (ErrorItemTitle * string) list -> ErrorCollection =
        List.fold folder Map.empty<ErrorItemTitle, string list>
        >> Map.map (fun k sl -> k |> toTitleStr, sl |> List.toArray)
        >> Map.values
        >> dict
