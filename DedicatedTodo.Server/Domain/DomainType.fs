namespace EvaluateTodo.Server.Domain

open EvaluateTodo.Server.Util.PrimaryUtil

type Priority = High | Medium | Low | NotAssigned

type TodoIdentity = TodoIdentity of int

type TodoContent = {
    Id: TodoIdentity
    Title: String50
    SelectedPriority: Priority
}

type TodoItem =
    | InCompleteTodo of TodoContent
    | CompletedTodo of TodoContent

type ChangeStatusCommand = MarkDone | RevertDone
