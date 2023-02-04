#r "bin/Debug/net7.0/EvaluateTodo.Server.dll"

open System
open EvaluateTodo.Server.Dto
open EvaluateTodo.Server.Dto.Rest
open EvaluateTodo.Server.Dto.Validation

{ ByComplete = false |> Nullable; ByPriority = "NotAssigned" }
|> Validate.filter
|> Result.map ToDb.filterDtoToWhereClause
