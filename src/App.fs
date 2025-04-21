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

let init () =
    let todoId = Guid.NewGuid()

    let todo =
        { Id = todoId
          Description = "Learn F#"
          Completed = false
          BeingEdited = false
          EditDescription = "Learn F#" }

    { TodoList = Map [ todoId, todo ]
      NewTodo = ""
      SelectedFilter = All }

let update msg state =
    match msg with

    | AddNewTodo when state.NewTodo = "" -> state

    | AddNewTodo ->
        let todoId = Guid.NewGuid()

        let todo =
            { Id = todoId
              Description = state.NewTodo
              Completed = false
              BeingEdited = false
              EditDescription = state.NewTodo }

        { state with
            NewTodo = ""
            TodoList = state.TodoList |> Map.add todoId todo }

    | ApplyEdit todoId ->
        let todo = state.TodoList[todoId]

        let editedTodo =
            { todo with
                Description = todo.EditDescription }

        { state with
            TodoList = state.TodoList |> Map.add todoId editedTodo }

    | CancelEdit todoId ->
        let todo = state.TodoList[todoId]

        { state with
            TodoList = state.TodoList |> Map.add todoId { todo with BeingEdited = false } }

    | DeleteTodo todoId ->
        { state with
            TodoList = state.TodoList |> Map.remove todoId }

    | SetEditedDescription(todoId, newText) ->
        let todo = state.TodoList[todoId]

        { state with
            TodoList = state.TodoList |> Map.add todoId { todo with EditDescription = newText } }

    | SetNewTodo desc -> { state with NewTodo = desc }

    | ShowAll -> { state with SelectedFilter = All }

    | ShowCompleted ->
        { state with
            SelectedFilter = Completed }

    | ShowIncomplete ->
        { state with
            SelectedFilter = Incomplete }

    | StartEditingTodo todoId ->
        let todo = state.TodoList[todoId]

        let editingTodo =
            { todo with
                BeingEdited = true
                EditDescription = todo.Description }

        { state with
            TodoList = state.TodoList |> Map.add todoId editingTodo }

    | ToggleCompleted todoId ->
        let todo = state.TodoList[todoId]

        let toggleCompleted todo =
            { todo with
                Completed = not todo.Completed }

        { state with
            TodoList = state.TodoList |> Map.add todoId (toggleCompleted todo) }

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

let filterTodosBy filter todoList =
    match filter with
    | All -> todoList
    | Completed -> Map.filter (fun id todo -> todo.Completed) todoList
    | Incomplete -> Map.filter (fun id todo -> not todo.Completed) todoList

let todoList (state: State) (dispatch: Msg -> unit) =
    Html.ul
        [ prop.children
              [ for id, todo in state.TodoList |> filterTodosBy state.SelectedFilter |> Map.toSeq ->
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
