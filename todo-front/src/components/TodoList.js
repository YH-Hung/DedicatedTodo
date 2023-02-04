import TodoShow from "./TodoShow";
import {useContext} from "react";
import TodoContext from "../context/todos";

/*
Transfer list of item into list of display element.
 */
function TodoList() {
    const {todos, byComplete ,setByComplete, byPriority, setByPriority, filterTodos } = useContext(TodoContext);
    const renderTodos = todos.map(t => <TodoShow key={t.id} todoItem={t} />);

    const completedNo = () => todos.filter(t => t.isComplete).length;

    const setAllFilters = (event, completeStatus, priority) => {
        event.preventDefault();
        filterTodos(completeStatus, priority);
        setByComplete(completeStatus);
        setByPriority(priority);
    };

    const setCompleteFilter = (event, completeStatus) => {
        event.preventDefault();
        filterTodos(completeStatus, byPriority);
        setByComplete(completeStatus);
    };

    const setPriorityFilter = (event, priority) => {
        event.preventDefault();
        filterTodos(byComplete, priority);
        setByPriority(priority);
    }

    return <div className="section">
        <div className="panel">
            <div className="panel-heading">Completed items: {completedNo()}</div>
            <p className="panel-tabs">
                <a className={byComplete === undefined && byPriority === undefined ? "is-active" : ""} onClick={event => setAllFilters(event, undefined, undefined)}>All</a>
                <a className={byComplete === true ? "is-active" : ""} onClick={event => setCompleteFilter(event, true)}>Complete</a>
                <a className={byComplete === false ? "is-active" : ""} onClick={event => setCompleteFilter(event, false)}>Incomplete</a>
                <a className={byPriority === "HIGH" ? "is-active" : ""} onClick={event => setPriorityFilter(event, "HIGH")}>High</a>
                <a className={byPriority === "MEDIUM" ? "is-active" : ""} onClick={event => setPriorityFilter(event, "MEDIUM")}>Medium</a>
                <a className={byPriority === "LOW" ? "is-active" : ""} onClick={event => setPriorityFilter(event, "LOW")}>Low</a>
            </p>
            {renderTodos}
        </div>
    </div>;
}

export default TodoList;
