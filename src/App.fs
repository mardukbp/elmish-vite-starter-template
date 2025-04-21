module App

open Elmish
open Elmish.React
open Feliz
open System
open Zanaptak.TypedCssClasses

type Bulma = CssClasses<"https://cdnjs.cloudflare.com/ajax/libs/bulma/0.7.4/css/bulma.min.css", Naming.PascalCase>

type FA =
    CssClasses<"https://stackpath.bootstrapcdn.com/font-awesome/4.7.0/css/font-awesome.min.css", Naming.PascalCase>

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
    { TodoList: Todo list
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

let init () =
    { TodoList =
        [ { Description = "Learn F#"
            Completed = false
            Id = Guid.NewGuid()
            BeingEdited = false
            EditDescription = "Learn F#" } ]
      NewTodo = ""
      SelectedFilter = All }

let update msg state =
    match msg with

    | SetNewTodo desc -> { state with NewTodo = desc }

    | DeleteTodo todoId ->
        let todoList = state.TodoList |> List.filter (fun todo -> todo.Id <> todoId)

        { state with TodoList = todoList }

    | ToggleCompleted todoId ->
        let todoList =
            state.TodoList
            |> List.map (fun todo ->
                if todo.Id = todoId then
                    { todo with
                        Completed = not todo.Completed }
                else
                    todo)

        { state with TodoList = todoList }

    | AddNewTodo when state.NewTodo = "" -> state

    | AddNewTodo ->
        { state with
            NewTodo = ""
            TodoList =
                { Description = state.NewTodo
                  Completed = false
                  Id = Guid.NewGuid()
                  BeingEdited = false
                  EditDescription = state.NewTodo }
                :: state.TodoList }

    | StartEditingTodo todoId ->
        let editingTodos =
            List.map
                (fun todo ->
                    if todo.Id = todoId then
                        { todo with
                            BeingEdited = true
                            EditDescription = todo.Description }
                    else
                        todo)

                state.TodoList

        { state with TodoList = editingTodos }

    | CancelEdit todoId ->
        let editingTodos =
            List.map
                (fun todo ->
                    if todo.Id = todoId then
                        { todo with BeingEdited = false }
                    else
                        todo)

                state.TodoList

        { state with TodoList = editingTodos }

    | ApplyEdit todoId ->
        let editingTodos =
            List.map
                (fun todo ->
                    if todo.Id = todoId then
                        { todo with
                            Description = todo.EditDescription }
                    else
                        todo)

                state.TodoList

        { state with TodoList = editingTodos }

    | SetEditedDescription(todoId, newText) ->
        let editingTodos =
            List.map
                (fun todo ->
                    if todo.Id = todoId then
                        { todo with EditDescription = newText }
                    else
                        todo)

                state.TodoList

        { state with TodoList = editingTodos }

    | ShowAll -> { state with SelectedFilter = All }

    | ShowCompleted ->
        { state with
            SelectedFilter = Completed }

    | ShowIncomplete ->
        { state with
            SelectedFilter = Incomplete }

let appTitle = Html.p [ prop.className Bulma.Title; prop.text "Elmish To-Do list" ]

let inputField (state: State) (dispatch: Msg -> unit) =
    Html.div
        [ prop.classes [ Bulma.Field; Bulma.HasAddons ]
          prop.children
              [ Html.div
                    [ prop.classes [ Bulma.Control; Bulma.IsExpanded ]
                      prop.children
                          [ Html.input
                                [ prop.classes [ Bulma.Input; Bulma.IsMedium ]
                                  prop.valueOrDefault state.NewTodo
                                  prop.onChange (SetNewTodo >> dispatch) ] ] ]

                Html.div
                    [ prop.className Bulma.Control
                      prop.children
                          [ Html.button
                                [ prop.classes [ Bulma.Button; Bulma.IsPrimary; Bulma.IsMedium ]
                                  prop.onClick (fun _ -> dispatch AddNewTodo)
                                  prop.children [ Html.i [ prop.classes [ FA.Fa; FA.FaPlus ] ] ] ] ] ] ] ]

/// Helper function to easily construct div with only classes and children
let div (classes: string list) (children: Fable.React.ReactElement list) =
    Html.div [ prop.classes classes; prop.children children ]

let renderTodo (todo: Todo) (dispatch: Msg -> unit) =
    div
        [ Bulma.Box ]
        [ div
              [ Bulma.Columns; Bulma.IsMobile; Bulma.IsVcentered ]
              [ div [ Bulma.Column ] [ Html.p [ prop.className Bulma.Subtitle; prop.text todo.Description ] ]

                div
                    [ Bulma.Column; Bulma.IsNarrow ]
                    [ div
                          [ Bulma.Buttons ]
                          [ Html.button
                                [ prop.classes
                                      [ Bulma.Button
                                        if todo.Completed then
                                            Bulma.IsSuccess ]
                                  prop.onClick (fun _ -> dispatch (ToggleCompleted todo.Id))
                                  prop.children [ Html.i [ prop.classes [ FA.Fa; FA.FaCheck ] ] ] ]

                            Html.button
                                [ prop.classes [ Bulma.Button; Bulma.IsPrimary ]
                                  prop.onClick (fun _ -> dispatch (StartEditingTodo todo.Id))
                                  prop.children [ Html.i [ prop.classes [ FA.Fa; FA.FaEdit ] ] ] ]

                            Html.button
                                [ prop.classes [ Bulma.Button; Bulma.IsDanger ]
                                  prop.onClick (fun _ -> dispatch (DeleteTodo todo.Id))
                                  prop.children [ Html.i [ prop.classes [ FA.Fa; FA.FaTimes ] ] ] ] ] ] ] ]


let renderEditForm (todo: Todo) (dispatch: Msg -> unit) =
    div
        [ Bulma.Box ]
        [ div
              [ Bulma.Field; Bulma.IsGrouped ]
              [ div
                    [ Bulma.Control; Bulma.IsExpanded ]
                    [ Html.input
                          [ prop.classes [ Bulma.Input; Bulma.IsMedium ]
                            prop.defaultValue todo.Description
                            prop.value todo.EditDescription
                            prop.onTextChange ((fun text -> SetEditedDescription(todo.Id, text)) >> dispatch) ] ]

                div
                    [ Bulma.Control; Bulma.Buttons ]
                    [ Html.button
                          [ prop.disabled (todo.EditDescription = "")
                            prop.classes
                                [ Bulma.Button
                                  if todo.EditDescription = todo.Description then
                                      Bulma.IsOutlined
                                  else
                                      Bulma.IsPrimary ]
                            prop.onClick (fun _ -> dispatch (ApplyEdit todo.Id))
                            prop.children [ Html.i [ prop.classes [ FA.Fa; FA.FaSave ] ] ] ]

                      Html.button
                          [ prop.classes [ Bulma.Button; Bulma.IsPrimary; Bulma.IsMedium ]
                            prop.onClick (fun _ -> dispatch (CancelEdit todo.Id))
                            prop.children [ Html.i [ prop.classes [ FA.Fa; FA.FaArrowRight ] ] ] ] ] ] ]

let filterTodos filter todoList =
    match filter with
    | All -> todoList
    | Completed -> List.filter _.Completed todoList
    | Incomplete -> List.filter (fun todo -> not todo.Completed) todoList

let todoList (state: State) (dispatch: Msg -> unit) =
    Html.ul
        [ prop.children
              [ for todo in filterTodos state.SelectedFilter state.TodoList ->
                    if todo.BeingEdited then
                        renderEditForm todo dispatch
                    else
                        renderTodo todo dispatch ] ]

let renderFilterTabs (state: State) (dispatch: Msg -> unit) =
    div
        [ Bulma.Tabs; Bulma.IsToggle; Bulma.IsFullwidth ]
        [ Html.ul
              [ Html.li
                    [ if state.SelectedFilter = All then
                          prop.className Bulma.IsActive
                      prop.children [ Html.a [ prop.onClick (fun _ -> dispatch ShowAll); prop.text "All" ] ] ]

                Html.li
                    [ if state.SelectedFilter = Completed then
                          prop.className Bulma.IsActive
                      prop.children [ Html.a [ prop.onClick (fun _ -> dispatch ShowCompleted); prop.text "Completed" ] ] ]

                Html.li
                    [ if state.SelectedFilter = Incomplete then
                          prop.className Bulma.IsActive
                      prop.children
                          [ Html.a [ prop.onClick (fun _ -> dispatch ShowIncomplete); prop.text "Incomplete" ] ] ] ] ]

let render state dispatch =
    Html.div
        [ prop.style [ style.padding 20 ]
          prop.children
              [ appTitle
                inputField state dispatch
                renderFilterTabs state dispatch
                todoList state dispatch ] ]

Program.mkSimple init update render
|> Program.withReactSynchronous "root"
|> Program.run
