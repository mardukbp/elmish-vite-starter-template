module App

open Elmish
open Elmish.Debug
open Elmish.React
open Feliz
open System
open Zanaptak.TypedCssClasses
open type Html
open type prop

type Bulma = CssClasses<"https://cdnjs.cloudflare.com/ajax/libs/bulma/0.7.4/css/bulma.min.css", Naming.PascalCase>

type FA = CssClasses<"https://cdn.jsdelivr.net/npm/fontawesome5-fullcss@1.1.0/css/all.min.css", Naming.PascalCase>

type Todo =
    { Id: Guid
      Description: string
      Completed: bool
      BeingEdited: bool
      EditDescription: string }

type Filter =
    | All
    | Completed
    | Incomplete

type State =
    { TodoList: Map<Guid, Todo>
      NewTodo: string
      SelectedFilter: Filter }

type Msg =
    | SetNewTodo of string
    | AddNewTodo
    | DeleteTodo of Guid
    | ToggleCompleted of Guid
    | CancelEdit of Guid
    | ApplyEdit of Guid
    | StartEditingTodo of Guid
    | SetEditedDescription of Guid * string
    | ShowAll
    | ShowCompleted
    | ShowIncomplete

let filterTodosBy filter todoList =
    match filter with
    | All -> todoList
    | Completed -> Map.filter (fun id todo -> todo.Completed) todoList
    | Incomplete -> Map.filter (fun id todo -> not todo.Completed) todoList

let newTodo todoId description =
    { Id = todoId
      Description = description
      Completed = false
      BeingEdited = false
      EditDescription = description }

let init () =
    let todoId = Guid.NewGuid()

    let todo = newTodo todoId "Learn F#"

    { TodoList = Map [ todoId, todo ]
      NewTodo = ""
      SelectedFilter = All }

let updateTodoList state todoId updateTodo =
    let todo = state.TodoList[todoId]

    { state with
        TodoList = state.TodoList |> Map.add todoId (updateTodo todo) }

let update msg state =
    match msg with

    | AddNewTodo when state.NewTodo = "" -> state

    | AddNewTodo ->
        let todoId = Guid.NewGuid()

        let todo = newTodo todoId state.NewTodo

        { state with
            NewTodo = ""
            TodoList = state.TodoList |> Map.add todoId todo }

    | ApplyEdit todoId ->
        updateTodoList state todoId (fun todo ->
            { todo with
                Description = todo.EditDescription })

    | CancelEdit todoId -> updateTodoList state todoId (fun todo -> { todo with BeingEdited = false })

    | DeleteTodo todoId ->
        { state with
            TodoList = state.TodoList |> Map.remove todoId }

    | SetEditedDescription(todoId, newText) ->
        updateTodoList state todoId (fun todo -> { todo with EditDescription = newText })

    | SetNewTodo desc -> { state with NewTodo = desc }

    | ShowAll -> { state with SelectedFilter = All }

    | ShowCompleted ->
        { state with
            SelectedFilter = Completed }

    | ShowIncomplete ->
        { state with
            SelectedFilter = Incomplete }

    | StartEditingTodo todoId ->
        updateTodoList state todoId (fun todo ->
            { todo with
                BeingEdited = true
                EditDescription = todo.Description })

    | ToggleCompleted todoId ->
        updateTodoList state todoId (fun todo ->
            { todo with
                Completed = not todo.Completed })

/// Helper function to easily construct div with only classes and children
let div (classes: string list) (children: Fable.React.ReactElement list) =
    Html.div [ prop.classes classes; prop.children children ]

let appTitle = p [ className Bulma.Title; text "Elmish To-Do list" ]

let inputField (state: State) (dispatch: Msg -> unit) =
    div
        [ Bulma.Field; Bulma.HasAddons ]
        [ div
              [ Bulma.Control; Bulma.IsExpanded ]
              [ input
                    [ classes [ Bulma.Input; Bulma.IsMedium ]
                      valueOrDefault state.NewTodo
                      onChange (SetNewTodo >> dispatch) ] ]

          div
              [ Bulma.Control ]
              [ button
                    [ classes [ Bulma.Button; Bulma.IsPrimary; Bulma.IsMedium ]
                      onClick (fun _ -> dispatch AddNewTodo)
                      children [ i [ classes [ FA.Fa; FA.FaPlus ] ] ] ] ] ]

let renderTodo (todo: Todo) (dispatch: Msg -> unit) =
    div
        [ Bulma.Box ]
        [ div
              [ Bulma.Columns; Bulma.IsMobile; Bulma.IsVcentered ]
              [ div [ Bulma.Column ] [ p [ className Bulma.Subtitle; text todo.Description ] ]

                div
                    [ Bulma.Column; Bulma.IsNarrow ]
                    [ div
                          [ Bulma.Buttons ]
                          [ button
                                [ classes
                                      [ Bulma.Button
                                        if todo.Completed then
                                            Bulma.IsSuccess ]
                                  onClick (fun _ -> dispatch (ToggleCompleted todo.Id))
                                  children [ i [ classes [ FA.Fa; FA.FaCheck ] ] ] ]

                            button
                                [ classes [ Bulma.Button; Bulma.IsPrimary ]
                                  onClick (fun _ -> dispatch (StartEditingTodo todo.Id))
                                  children [ i [ classes [ FA.Fa; FA.FaEdit ] ] ] ]

                            button
                                [ classes [ Bulma.Button; Bulma.IsDanger ]
                                  onClick (fun _ -> dispatch (DeleteTodo todo.Id))
                                  children [ i [ classes [ FA.Fa; FA.FaTimes ] ] ] ] ] ] ] ]

let renderEditForm (todo: Todo) (dispatch: Msg -> unit) =
    div
        [ Bulma.Box ]
        [ div
              [ Bulma.Field; Bulma.IsGrouped ]
              [ div
                    [ Bulma.Control; Bulma.IsExpanded ]
                    [ input
                          [ classes [ Bulma.Input; Bulma.IsMedium ]
                            defaultValue todo.Description
                            value todo.EditDescription
                            onTextChange ((fun text -> SetEditedDescription(todo.Id, text)) >> dispatch) ] ]

                div
                    [ Bulma.Control; Bulma.Buttons ]
                    [ button
                          [ disabled (todo.EditDescription = "")
                            classes
                                [ Bulma.Button
                                  if todo.EditDescription = todo.Description then
                                      Bulma.IsOutlined
                                  else
                                      Bulma.IsPrimary ]
                            onClick (fun _ -> dispatch (ApplyEdit todo.Id))
                            children [ i [ classes [ FA.Fa; FA.FaSave ] ] ] ]

                      button
                          [ classes [ Bulma.Button; Bulma.IsPrimary ]
                            onClick (fun _ -> dispatch (CancelEdit todo.Id))
                            children [ i [ classes [ FA.Fa; FA.FaTimes ] ] ] ] ] ] ]

let todoList (state: State) (dispatch: Msg -> unit) =
    ul
        [ children
              [ for _id, todo in state.TodoList |> filterTodosBy state.SelectedFilter |> Map.toSeq ->
                    if todo.BeingEdited then
                        renderEditForm todo dispatch
                    else
                        renderTodo todo dispatch ] ]

let renderFilterTabs (state: State) (dispatch: Msg -> unit) =
    div
        [ Bulma.Tabs; Bulma.IsToggle; Bulma.IsFullwidth ]
        [ ul
              [ li
                    [ if state.SelectedFilter = All then
                          className Bulma.IsActive
                      children [ a [ onClick (fun _ -> dispatch ShowAll); text "All" ] ] ]

                li
                    [ if state.SelectedFilter = Completed then
                          className Bulma.IsActive
                      children [ a [ onClick (fun _ -> dispatch ShowCompleted); text "Completed" ] ] ]

                li
                    [ if state.SelectedFilter = Incomplete then
                          className Bulma.IsActive
                      children [ a [ onClick (fun _ -> dispatch ShowIncomplete); text "Incomplete" ] ] ] ] ]

let render state dispatch =
    Html.div
        [ style [ style.padding 20 ]
          children
              [ appTitle
                inputField state dispatch
                renderFilterTabs state dispatch
                todoList state dispatch ] ]

Program.mkSimple init update render
|> Program.withReactSynchronous "root"
|> Program.withDebugger
|> Program.run
