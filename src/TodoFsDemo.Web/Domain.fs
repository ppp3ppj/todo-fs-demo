module Domain

open System

type TodoId = TodoId of int

type TodoStatus =
    | Pending
    | InProgress of startedAt: DateTime
    | Done of completedAt: DateTime
    | Cancelled of reason: string

type Todo =
    { Id: TodoId
      Title: string
      Due: DateTime
      Status: TodoStatus }

let create id title due =
    if String.IsNullOrWhiteSpace title then Error "Title is required"
    else Ok { Id = id; Title = title.Trim(); Due = due; Status = Pending }

let start now todo =
    match todo.Status with
    | Pending -> Ok { todo with Status = InProgress now }
    | InProgress _ -> Error "Already started"
    | Done _ -> Error "Already done"
    | Cancelled _ -> Error "Cannot start a cancelled task"

let complete now todo =
    match todo.Status with
    | InProgress _ -> Ok { todo with Status = Done now }
    | Pending -> Error "Start the task first"
    | Done _ -> Error "Already done"
    | Cancelled _ -> Error "Cannot complete a cancelled task"

let cancel reason todo =
    if String.IsNullOrWhiteSpace reason then Error "Reason is required"
    else
        match todo.Status with
        | Done _ -> Error "Cannot cancel a finished task"
        | Cancelled _ -> Error "Already cancelled"
        | Pending | InProgress _ -> Ok { todo with Status = Cancelled reason }