/// Map DB return to Domain
module DedicatedTodo.Server.Dto.FromDb

open DedicatedTodo.Server.DbMapping
open DedicatedTodo.Server.Util

/// Map DB query / exec return condition to Result
let returnConditionToResult dbExecCond =
    match dbExecCond with
    | Success result ->
        match (box result) with // Have to box first to do casting match.
        | :? int as rowNo -> if (rowNo > 0) then Ok result else Error [(DbExec, "No row affected")]
        | _ -> Ok result
    | TransientFailure code -> Error [(DbExec, $"Transient DB error code {code}")]
    | InvalidOperation code -> Error [(DbExec, $"DB exception error code {code}")]
    | CircuitOnBreak -> Error [(DbExec, "Circuit on break due to intensive failure.")]