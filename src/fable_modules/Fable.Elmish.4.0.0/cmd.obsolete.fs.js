import { singleton } from "../fable-library.4.9.0/List.js";
import { singleton as singleton_1 } from "../fable-library.4.9.0/AsyncBuilder.js";

/**
 * Command to issue a specific message
 */
export function OfFunc_result(msg) {
    return singleton((dispatch) => {
        dispatch(msg);
    });
}

/**
 * Command that will evaluate an async block to the message
 */
export function OfAsyncWith_result(start, task) {
    const bind = (dispatch) => singleton_1.Delay(() => singleton_1.Bind(task, (_arg) => {
        const r = _arg;
        dispatch(r);
        return singleton_1.Zero();
    }));
    return singleton((arg) => {
        start(bind(arg));
    });
}

/**
 * Command to dispatch the `promise` result
 */
export function OfPromise_result(task) {
    const bind = (dispatch) => {
        task.then(dispatch);
    };
    return singleton(bind);
}

