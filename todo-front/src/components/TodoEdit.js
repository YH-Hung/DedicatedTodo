import {useContext, useState} from "react";
import TodoContext from "../context/todos";

/*
Item edit widget
 */
function TodoEdit({todoItem, toggleSetter}) {
    const [title, setTitle] = useState(todoItem.title)
    const [priority, setPriority] = useState(todoItem.priority);
    const { editTodoById } = useContext(TodoContext);

    /*
    Update local title state.
     */
    function handleTitleChange(event) {
        setTitle(event.target.value);
    }

    /*
    Update local priority state.
     */
    function handlePriorityChange(event) {
        setPriority(event.target.value);
    }

    /*
    Use current title and priority state update item in list.
     */
    function handleTitleAndPrioritySubmit(event) {
        event.preventDefault();
        toggleSetter(true);
        setTitle(title);

        let newTodo = {
            id: todoItem.id,
            title,
            priority
        };
        editTodoById(todoItem.id, newTodo);
    }

    return (<div className="column">
        <form onSubmit={handleTitleAndPrioritySubmit}>
            <div className="field has-addons">
                <div className="control">
                    <input className="input" type="text" value={title} onChange={handleTitleChange}/>
                </div>
                <div className="control select">
                    <select value={priority} onChange={handlePriorityChange}>
                        <option value="NotAssigned"></option>
                        <option value="HIGH">High</option>
                        <option value="MEDIUM">Medium</option>
                        <option value="LOW">Low</option>
                    </select>
                </div>
                <div className="control">
                    <button className="button" type="submit">Update</button>
                </div>
            </div>
        </form>
    </div>);
}

export default TodoEdit;
