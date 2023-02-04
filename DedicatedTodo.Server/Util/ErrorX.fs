[<Microsoft.FSharp.Core.AutoOpen>]
module EvaluateTodo.Server.Util.ErrorX

/// OK -> empty string / Error -> combine error message
let inline (+?) r1 r2 =
    match r1, r2 with
    | Error eml1, Error eml2 -> Error (eml1 @ eml2)
    | Error eml1, Ok _ -> Error eml1
    | Ok _, Error eml2 -> Error eml2
    | Ok _, Ok _ -> Error []    // Don't return Ok to make Ok type generic
