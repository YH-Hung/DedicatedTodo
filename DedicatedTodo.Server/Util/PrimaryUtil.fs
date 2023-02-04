namespace DedicatedTodo.Server.Util.PrimaryUtil

open System
open Microsoft.FSharp.Core
open DedicatedTodo.Server.Util

/// Ensure string length from 0 to 50
type String50 = private String50 of string

module String50 =
    /// Constructor with validation
    let ctor str =
        let strLen = String.length str
        let isLess50 = strLen <= 50
        let isNotBlank = str |> String.IsNullOrWhiteSpace |> not

        if isLess50 && isNotBlank then
            Ok (String50 str)
        else
            Error [(TitleStr, "Invalid string length;")]

    /// Simple Getter because destructor is disabled.
    let value (String50 str) = str

    /// For unit test arrangement
    let getSample (sampleText: string) =
        if sampleText.Length > 50 || (String.IsNullOrWhiteSpace sampleText) then
            invalidArg (nameof sampleText) "Invalid sample text"

        String50 sampleText
