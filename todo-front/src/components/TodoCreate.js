import {useContext, useState} from "react";
import nextId from "react-id-generator";
import TodoContext from "../context/todos";

/*
Create new item and add into list.
 */
function TodoCreate() {
    const [title, setTitle] = useState("");
    const [priority, setPriority] = useState("NotAssigned");
    const {createNewTodo} = useContext(TodoContext);

    function handleTitleChange(event) {
        setTitle(event.target.value);
    }

    function handlePriorityChange(event) {
        setPriority(event.target.value);
    }

    function handleSubmit(event) {
        event.preventDefault();
        let newId = nextId();
        createNewTodo({id: newId, title, isComplete: false, priority});
        setTitle("");
    }

    return (
        <div className="section">
            <h3 className="title">Create new item</h3>
            <form onSubmit={handleSubmit}>
                <div className="field">
                    <label className="label">Title</label>
                    <div className="control">
                        <input className="input" type="text" value={title} onChange={handleTitleChange}/>
                    </div>
                </div>
                <div className="field">
                    <label className="label">Priority</label>
                    <div className="control select">
                        <select value={priority} onChange={handlePriorityChange}>
                            <option value="NotAssigned"></option>
                            <option value="HIGH">High</option>
                            <option value="MEDIUM">Medium</option>
                            <option value="LOW">Low</option>
                        </select>
                    </div>
                </div>
                <div className="field">
                    <div className="control">
                        <button className="button is-link" type="submit">Create</button>
                    </div>
                </div>
            </form>
        </div>
    );
}

export default TodoCreate;
