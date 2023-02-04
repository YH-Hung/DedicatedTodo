/// XUnit Theory only consume IEnumerable<Object[]>
module EvaluatedTodo.Server.Tests.Generate

open System.Security.Cryptography
open System.Text

let singleArgumentFromList<'T> =
    List.map (fun i -> [| box<'T> i |])
    >> List.toSeq

let singleArgument ctor n =
    seq { for i in 1..n -> [| i |> ctor |> box |] }

let doubleArgument ctor1 ctor2 n =
    seq { for i in 1..n -> [| i |> ctor1 |> box ; i |> ctor2 |> box |] }

let tripleArgument ctor1 ctor2 ctor3 n =
    seq { for i in 1..n ->
            [| i |> ctor1 |> box
               i |> ctor2 |> box
               i |> ctor3 |> box |] }

let randomInt upLimit () =
    RandomNumberGenerator.GetInt32(0, upLimit)

let private charSet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"
let private charLength = charSet.Length

let randomString length () =    // Must have a unit argument to ensure invoking every time.
    let sb = StringBuilder()
    for i in 1..length do
        let ind = RandomNumberGenerator.GetInt32(0, charLength)
        sb.Append charSet[ind] |> ignore

    sb.ToString()
