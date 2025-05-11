import { Union, Record } from "./fable_modules/fable-library.4.9.0/Types.js";
import { union_type, record_type, bool_type, string_type, class_type } from "./fable_modules/fable-library.4.9.0/Reflection.js";
import { values, remove, add, FSharpMap__get_Item, ofSeq, filter as filter_1 } from "./fable_modules/fable-library.4.9.0/Map.js";
import { newGuid } from "./fable_modules/fable-library.4.9.0/Guid.js";
import { uncurry2, equals, createObj, comparePrimitives } from "./fable_modules/fable-library.4.9.0/Util.js";
import { createElement } from "react";
import { join } from "./fable_modules/fable-library.4.9.0/String.js";
import { reactApi } from "./fable_modules/Feliz.2.9.0/./Interop.fs.js";
import { singleton, ofArray } from "./fable_modules/fable-library.4.9.0/List.js";
import { map, empty, singleton as singleton_1, append, delay, toList } from "./fable_modules/fable-library.4.9.0/Seq.js";
import { ProgramModule_mkSimple, ProgramModule_run } from "./fable_modules/Fable.Elmish.4.3.0/program.fs.js";
import { Program_withReactSynchronous } from "./fable_modules/Fable.Elmish.React.4.0.0/react.fs.js";
import { Program_withDebuggerUsing, Debugger_getCase, Debugger_showWarning, Debugger_showError } from "./fable_modules/Fable.Elmish.Debugger.4.2.1/./debugger.fs.js";
import { Auto_generateBoxedEncoder_437914C6, uint64, int64, decimal } from "./fable_modules/Thoth.Json.6.0.0/./Encode.fs.js";
import { Auto_generateBoxedDecoder_Z6670B51, uint64 as uint64_1, int64 as int64_1, decimal as decimal_1 } from "./fable_modules/Thoth.Json.6.0.0/./Decode.fs.js";
import { empty as empty_1 } from "./fable_modules/Fable.Elmish.Debugger.4.2.1/../Thoth.Json.6.0.0/Extra.fs.js";
import { ExtraCoders } from "./fable_modules/Thoth.Json.6.0.0/Types.fs.js";
import { fromValue } from "./fable_modules/Fable.Elmish.Debugger.4.2.1/../Thoth.Json.6.0.0/Decode.fs.js";

export class Todo extends Record {
    constructor(Id, Description, Completed, BeingEdited, EditDescription) {
        super();
        this.Id = Id;
        this.Description = Description;
        this.Completed = Completed;
        this.BeingEdited = BeingEdited;
        this.EditDescription = EditDescription;
    }
}

export function Todo_$reflection() {
    return record_type("App.Todo", [], Todo, () => [["Id", class_type("System.Guid")], ["Description", string_type], ["Completed", bool_type], ["BeingEdited", bool_type], ["EditDescription", string_type]]);
}

export class Filter extends Union {
    constructor(tag, fields) {
        super();
        this.tag = tag;
        this.fields = fields;
    }
    cases() {
        return ["All", "Completed", "Incomplete"];
    }
}

export function Filter_$reflection() {
    return union_type("App.Filter", [], Filter, () => [[], [], []]);
}

export class State extends Record {
    constructor(TodoList, NewTodo, SelectedFilter) {
        super();
        this.TodoList = TodoList;
        this.NewTodo = NewTodo;
        this.SelectedFilter = SelectedFilter;
    }
}

export function State_$reflection() {
    return record_type("App.State", [], State, () => [["TodoList", class_type("Microsoft.FSharp.Collections.FSharpMap`2", [class_type("System.Guid"), Todo_$reflection()])], ["NewTodo", string_type], ["SelectedFilter", Filter_$reflection()]]);
}

export class Msg extends Union {
    constructor(tag, fields) {
        super();
        this.tag = tag;
        this.fields = fields;
    }
    cases() {
        return ["SetNewTodo", "AddNewTodo", "DeleteTodo", "ToggleCompleted", "CancelEdit", "ApplyEdit", "StartEditingTodo", "SetEditedDescription", "ShowAll", "ShowCompleted", "ShowIncomplete"];
    }
}

export function Msg_$reflection() {
    return union_type("App.Msg", [], Msg, () => [[["Item", string_type]], [], [["Item", class_type("System.Guid")]], [["Item", class_type("System.Guid")]], [["Item", class_type("System.Guid")]], [["Item", class_type("System.Guid")]], [["Item", class_type("System.Guid")]], [["Item1", class_type("System.Guid")], ["Item2", string_type]], [], [], []]);
}

export function filterTodosBy(filter, todoList_1) {
    switch (filter.tag) {
        case 1:
            return filter_1((id, todo) => todo.Completed, todoList_1);
        case 2:
            return filter_1((id_1, todo_1) => !todo_1.Completed, todoList_1);
        default:
            return todoList_1;
    }
}

export function newTodo(todoId, description) {
    return new Todo(todoId, description, false, false, description);
}

export function init() {
    const todoId = newGuid();
    const todo = newTodo(todoId, "Learn F#");
    return new State(ofSeq([[todoId, todo]], {
        Compare: comparePrimitives,
    }), "", new Filter(0, []));
}

export function updateTodoList(state, todoId, updateTodo) {
    const todo = FSharpMap__get_Item(state.TodoList, todoId);
    return new State(add(todoId, updateTodo(todo), state.TodoList), state.NewTodo, state.SelectedFilter);
}

export function update(msg, state) {
    switch (msg.tag) {
        case 5: {
            const todoId_1 = msg.fields[0];
            return updateTodoList(state, todoId_1, (todo_1) => (new Todo(todo_1.Id, todo_1.EditDescription, todo_1.Completed, false, todo_1.EditDescription)));
        }
        case 4: {
            const todoId_2 = msg.fields[0];
            return updateTodoList(state, todoId_2, (todo_2) => (new Todo(todo_2.Id, todo_2.Description, todo_2.Completed, false, todo_2.EditDescription)));
        }
        case 2: {
            const todoId_3 = msg.fields[0];
            return new State(remove(todoId_3, state.TodoList), state.NewTodo, state.SelectedFilter);
        }
        case 7: {
            const todoId_4 = msg.fields[0];
            const newText = msg.fields[1];
            return updateTodoList(state, todoId_4, (todo_3) => (new Todo(todo_3.Id, todo_3.Description, todo_3.Completed, todo_3.BeingEdited, newText)));
        }
        case 0: {
            const desc = msg.fields[0];
            return new State(state.TodoList, desc, state.SelectedFilter);
        }
        case 8:
            return new State(state.TodoList, state.NewTodo, new Filter(0, []));
        case 9:
            return new State(state.TodoList, state.NewTodo, new Filter(1, []));
        case 10:
            return new State(state.TodoList, state.NewTodo, new Filter(2, []));
        case 6: {
            const todoId_5 = msg.fields[0];
            return updateTodoList(state, todoId_5, (todo_4) => (new Todo(todo_4.Id, todo_4.Description, todo_4.Completed, true, todo_4.Description)));
        }
        case 3: {
            const todoId_6 = msg.fields[0];
            return updateTodoList(state, todoId_6, (todo_5) => (new Todo(todo_5.Id, todo_5.Description, !todo_5.Completed, todo_5.BeingEdited, todo_5.EditDescription)));
        }
        default:
            if (state.NewTodo === "") {
                return state;
            }
            else {
                const todoId = newGuid();
                const todo = newTodo(todoId, state.NewTodo);
                return new State(add(todoId, todo, state.TodoList), "", state.SelectedFilter);
            }
    }
}

/**
 * Helper function to easily construct div with only classes and children
 */
export function div(classes, children) {
    return createElement("div", {
        className: join(" ", classes),
        children: reactApi.Children.toArray(Array.from(children)),
    });
}

export const appTitle = createElement("p", {
    className: "title",
    children: "Elmish To-Do list",
});

export function inputField(state, dispatch) {
    let value_1, elems;
    return div(ofArray(["field", "has-addons"]), ofArray([div(ofArray(["control", "is-expanded"]), singleton(createElement("input", createObj(ofArray([["className", join(" ", ["input", "is-medium"])], (value_1 = state.NewTodo, ["ref", (e) => {
        if (!(e == null) && !equals(e.value, value_1)) {
            e.value = value_1;
        }
    }]), ["onChange", (ev) => {
        dispatch(new Msg(0, [ev.target.value]));
    }]]))))), div(singleton("control"), singleton(createElement("button", createObj(ofArray([["disabled", state.NewTodo === ""], ["className", join(" ", ["button", "is-primary", "is-medium"])], ["onClick", (_arg) => {
        dispatch(new Msg(1, []));
    }], (elems = [createElement("i", {
        className: join(" ", ["fa", "fa-plus"]),
    })], ["children", reactApi.Children.toArray(Array.from(elems))])])))))]));
}

export function renderTodo(todo, dispatch) {
    let elems, elems_1, elems_2;
    return div(singleton("box"), singleton(div(ofArray(["columns", "is-mobile", "is-vcentered"]), ofArray([div(singleton("column"), singleton(createElement("p", {
        className: "subtitle",
        children: todo.Description,
    }))), div(ofArray(["column", "is-narrow"]), singleton(div(singleton("buttons"), ofArray([createElement("button", createObj(ofArray([["className", join(" ", toList(delay(() => append(singleton_1("button"), delay(() => (todo.Completed ? singleton_1("is-success") : empty()))))))], ["onClick", (_arg) => {
        dispatch(new Msg(3, [todo.Id]));
    }], (elems = [createElement("i", {
        className: join(" ", ["fa", "fa-check"]),
    })], ["children", reactApi.Children.toArray(Array.from(elems))])]))), createElement("button", createObj(ofArray([["className", join(" ", ["button", "is-primary"])], ["onClick", (_arg_1) => {
        dispatch(new Msg(6, [todo.Id]));
    }], (elems_1 = [createElement("i", {
        className: join(" ", ["fa", "fa-edit"]),
    })], ["children", reactApi.Children.toArray(Array.from(elems_1))])]))), createElement("button", createObj(ofArray([["className", join(" ", ["button", "is-danger"])], ["onClick", (_arg_2) => {
        dispatch(new Msg(2, [todo.Id]));
    }], (elems_2 = [createElement("i", {
        className: join(" ", ["fa", "fa-times"]),
    })], ["children", reactApi.Children.toArray(Array.from(elems_2))])])))]))))]))));
}

export function renderEditForm(todo, dispatch) {
    let elems, elems_1;
    return div(singleton("box"), singleton(div(ofArray(["field", "is-grouped"]), ofArray([div(ofArray(["control", "is-expanded"]), singleton(createElement("input", {
        className: join(" ", ["input", "is-medium"]),
        defaultValue: todo.Description,
        value: todo.EditDescription,
        onChange: (ev) => {
            dispatch(new Msg(7, [todo.Id, ev.target.value]));
        },
    }))), div(ofArray(["control", "buttons"]), ofArray([createElement("button", createObj(ofArray([["disabled", todo.EditDescription === todo.Description], ["className", join(" ", toList(delay(() => append(singleton_1("button"), delay(() => ((todo.EditDescription === todo.Description) ? singleton_1("is-outlined") : singleton_1("is-primary")))))))], ["onClick", (_arg) => {
        dispatch(new Msg(5, [todo.Id]));
    }], (elems = [createElement("i", {
        className: join(" ", ["fa", "fa-save"]),
    })], ["children", reactApi.Children.toArray(Array.from(elems))])]))), createElement("button", createObj(ofArray([["className", join(" ", ["button", "is-primary"])], ["onClick", (_arg_1) => {
        dispatch(new Msg(4, [todo.Id]));
    }], (elems_1 = [createElement("i", {
        className: join(" ", ["fa", "fa-times"]),
    })], ["children", reactApi.Children.toArray(Array.from(elems_1))])])))]))]))));
}

export function todoList(state, dispatch) {
    let elems;
    return createElement("ul", createObj(singleton((elems = toList(delay(() => map((todo) => (todo.BeingEdited ? renderEditForm(todo, dispatch) : renderTodo(todo, dispatch)), values(filterTodosBy(state.SelectedFilter, state.TodoList))))), ["children", reactApi.Children.toArray(Array.from(elems))]))));
}

export function renderFilterTabs(state, dispatch) {
    let children;
    return div(ofArray(["tabs", "is-toggle", "is-fullwidth"]), singleton((children = ofArray([createElement("li", createObj(toList(delay(() => append(equals(state.SelectedFilter, new Filter(0, [])) ? singleton_1(["className", "is-active"]) : empty(), delay(() => {
        let elems;
        return singleton_1((elems = [createElement("a", {
            onClick: (_arg) => {
                dispatch(new Msg(8, []));
            },
            children: "All",
        })], ["children", reactApi.Children.toArray(Array.from(elems))]));
    })))))), createElement("li", createObj(toList(delay(() => append(equals(state.SelectedFilter, new Filter(1, [])) ? singleton_1(["className", "is-active"]) : empty(), delay(() => {
        let elems_1;
        return singleton_1((elems_1 = [createElement("a", {
            onClick: (_arg_1) => {
                dispatch(new Msg(9, []));
            },
            children: "Completed",
        })], ["children", reactApi.Children.toArray(Array.from(elems_1))]));
    })))))), createElement("li", createObj(toList(delay(() => append(equals(state.SelectedFilter, new Filter(2, [])) ? singleton_1(["className", "is-active"]) : empty(), delay(() => {
        let elems_2;
        return singleton_1((elems_2 = [createElement("a", {
            onClick: (_arg_2) => {
                dispatch(new Msg(10, []));
            },
            children: "Incomplete",
        })], ["children", reactApi.Children.toArray(Array.from(elems_2))]));
    }))))))]), createElement("ul", {
        children: reactApi.Children.toArray(Array.from(children)),
    }))));
}

export function render(state, dispatch) {
    let elems;
    return createElement("div", createObj(ofArray([["style", {
        padding: 20,
    }], (elems = [appTitle, inputField(state, dispatch), renderFilterTabs(state, dispatch), todoList(state, dispatch)], ["children", reactApi.Children.toArray(Array.from(elems))])])));
}

ProgramModule_run((() => {
    let copyOfStruct, copyOfStruct_1, copyOfStruct_2;
    const program_1 = Program_withReactSynchronous("root", ProgramModule_mkSimple(init, update, render));
    const options = {};
    const program_3 = program_1;
    try {
        let patternInput;
        try {
            let coders;
            let extra_2_1;
            const extra_1_1 = new ExtraCoders((copyOfStruct = newGuid(), copyOfStruct), add("System.Decimal", [decimal, (path) => ((value_1) => decimal_1(path, value_1))], empty_1.Coders));
            extra_2_1 = (new ExtraCoders((copyOfStruct_1 = newGuid(), copyOfStruct_1), add("System.Int64", [int64, int64_1], extra_1_1.Coders)));
            coders = (new ExtraCoders((copyOfStruct_2 = newGuid(), copyOfStruct_2), add("System.UInt64", [uint64, uint64_1], extra_2_1.Coders)));
            const encoder_3 = Auto_generateBoxedEncoder_437914C6(State_$reflection(), void 0, coders, void 0);
            const decoder_3 = Auto_generateBoxedDecoder_Z6670B51(State_$reflection(), void 0, coders);
            const deflate = (x) => {
                try {
                    return encoder_3(x);
                }
                catch (er) {
                    Debugger_showWarning(singleton(er.message));
                    return x;
                }
            };
            const inflate = (x_1) => {
                const matchValue = fromValue("$", uncurry2(decoder_3), x_1);
                if (matchValue.tag === 1) {
                    const er_1 = matchValue.fields[0];
                    throw new Error(er_1);
                }
                else {
                    const x_2 = matchValue.fields[0];
                    return x_2;
                }
            };
            patternInput = [deflate, inflate];
        }
        catch (er_2) {
            Debugger_showWarning(singleton(er_2.message));
            patternInput = [(value_7) => value_7, (_arg) => {
                throw new Error("Cannot inflate model");
            }];
        }
        const inflater = patternInput[1];
        const deflater = patternInput[0];
        let connection;
        const options_1 = options;
        options_1.getActionType = (Debugger_getCase);
        connection = (window.__REDUX_DEVTOOLS_EXTENSION__.connect(options_1));
        return Program_withDebuggerUsing(deflater, inflater, connection, program_3);
    }
    catch (ex) {
        Debugger_showError(ofArray(["Unable to connect to the monitor, continuing w/o debugger", ex.message]));
        return program_3;
    }
})());

