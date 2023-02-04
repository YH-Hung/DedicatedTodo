module DedicatedTodo.Server.Domain.TodoIt

open DedicatedTodo.Server.Domain

let private markDone (todo: TodoItem) =
    match todo with
    | InCompleteTodo content -> CompletedTodo content
    | CompletedTodo _ -> todo

let private revertDone (todo: TodoItem) =
    match todo with
    | InCompleteTodo _ -> todo
    | CompletedTodo content -> InCompleteTodo content

let changeStatus cmd =
    match cmd with
    | MarkDone -> markDone
    | RevertDone -> revertDone

let private changeContentTitle newTitle (content: TodoContent) =
    { content with Title = newTitle }

let private changeContentPriority nextPriority (content: TodoContent) =
    { content with SelectedPriority = nextPriority }

let private changeProperty cmd nextVal (todo: TodoItem) =
    let action = cmd nextVal

    match todo with
    | InCompleteTodo content -> content |> action |> InCompleteTodo
    | CompletedTodo content -> content |> action  |> CompletedTodo

let changePriority = changeProperty changeContentPriority

let changeTitle = changeProperty changeContentTitle
