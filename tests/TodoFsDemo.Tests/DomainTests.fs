module DomainTests

open System
open Xunit
open Swensen.Unquote
open FsCheck.Xunit
open Domain

let now = DateTime(2026, 11, 26, 9, 0, 0)
let due = DateTime(2026, 11, 30)

let pending =
    { Id = TodoId 1; Title = "Write slides"; Due = due; Status = Pending }

[<Fact>]
let ``create trims the title and starts as pending`` () =
    test <@ create (TodoId 1) "  Write slides  " due = Ok pending @>

[<Fact>]
let ``create rejects an empty title`` () =
    test <@ create (TodoId 1) "   " due = Error "Title is required" @>

[<Fact>]
let ``start moves pending to in progress`` () =
    test <@ start now pending = Ok { pending with Status = InProgress now } @>

[<Fact>]
let ``cannot complete a task that was never started`` () =
    test <@ complete now pending = Error "Start the task first" @>

[<Fact>]
let ``cancel requires a reason`` () =
    test <@ cancel "  " pending = Error "Reason is required" @>

[<Property>]
let ``only pending tasks can be started`` (todo: Todo) =
    match todo.Status, start now todo with
    | Pending, Ok _ -> true
    | Pending, Error _ -> false
    | _, Ok _ -> false
    | _, Error _ -> true

[<Property>]
let ``a pending task can always be started and then completed``
        (todo: Todo) (startedAt: DateTime) (completedAt: DateTime) =
    let p = { todo with Status = Pending }
    let result = p |> start startedAt |> Result.bind (complete completedAt)
    result = Ok { p with Status = Done completedAt }