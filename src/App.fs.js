import { Union, Record } from "./fable_modules/fable-library.4.9.0/Types.js";
import { union_type, record_type, int32_type } from "./fable_modules/fable-library.4.9.0/Reflection.js";
import { createElement } from "react";
import { ofArray } from "./fable_modules/fable-library.4.9.0/List.js";
import { reactApi } from "./fable_modules/Feliz.2.9.0/./Interop.fs.js";
import { ProgramModule_mkSimple, ProgramModule_run } from "./fable_modules/Fable.Elmish.4.0.0/program.fs.js";
import { Program_withReactSynchronous } from "./fable_modules/Fable.Elmish.React.4.0.0/react.fs.js";

export class State extends Record {
    constructor(Count) {
        super();
        this.Count = (Count | 0);
    }
}

export function State_$reflection() {
    return record_type("App.State", [], State, () => [["Count", int32_type]]);
}

export class Msg extends Union {
    constructor(tag, fields) {
        super();
        this.tag = tag;
        this.fields = fields;
    }
    cases() {
        return ["Increment", "Decrement"];
    }
}

export function Msg_$reflection() {
    return union_type("App.Msg", [], Msg, () => [[], []]);
}

export function init() {
    return new State(0);
}

export function update(msg, state) {
    if (msg.tag === 1) {
        return new State(state.Count - 1);
    }
    else {
        return new State(state.Count + 1);
    }
}

export function render(state, dispatch) {
    const children = ofArray([createElement("button", {
        onClick: (_arg) => {
            dispatch(new Msg(0, []));
        },
        children: "Increment",
    }), createElement("button", {
        onClick: (_arg_1) => {
            dispatch(new Msg(1, []));
        },
        children: "Decrement",
    }), createElement("h1", {
        children: [state.Count],
    })]);
    return createElement("div", {
        children: reactApi.Children.toArray(Array.from(children)),
    });
}

ProgramModule_run(Program_withReactSynchronous("root", ProgramModule_mkSimple(init, update, render)));

