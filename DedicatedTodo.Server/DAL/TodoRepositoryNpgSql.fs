namespace EvaluateTodo.Server.DAL

open EvaluateTodo.Server
open Npgsql

/// Delegate data access logic to NpgSql + Dapper implementation
type TodoRepositoryNpgSql(cn: NpgsqlConnection) =
    interface ITodoRepository with
        member this.DeleteById(id) = TodoNpgSql.deleteById cn id
        member this.InsertSingle(insertDto) = TodoNpgSql.insertSingle cn insertDto
        member this.QueryAll() = TodoNpgSql.queryAll cn
        member this.QueryById(id) = TodoNpgSql.queryById cn id
        member this.UpdateEntity(isComplete, content) = TodoNpgSql.updateEntity cn (isComplete, content)
        member this.QueryFilteredAll(cond) = TodoNpgSql.queryAllBy cn cond
