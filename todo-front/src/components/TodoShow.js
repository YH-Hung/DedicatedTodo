import {useContext, useState} from "react";
import TodoContext from "../context/todos";
import TodoEdit from "./TodoEdit";

/*
Display and edit single item.
 */
function TodoShow({todoItem}) {

    const [checked, setChecked] = useState(todoItem.isComplete)
    const [toggled, setToggled] = useState(true)
    const { editTodoById, deleteTodoById } = useContext(TodoContext);


    /*
    When check change, update local state and item in list.
     */
    function handleCheckChange() {

        let completeStatus = !checked;
        setChecked(completeStatus);
        let newTodo = {
            id: todoItem.id,
            isComplete: completeStatus,
        };
        editTodoById(todoItem.id, newTodo);
    }


    /*
    Display title and priority edit controls.
     */
    function showEdit() {
        if (!toggled) {
            return <TodoEdit todoItem={todoItem} toggleSetter={setToggled} />;
        }
        return <div className="column"></div>;
    }

    return (
        <div className="panel-block columns">
            <div className="column is-1">
                <form>
                    <input type="checkbox" checked={checked} onChange={handleCheckChange}/>
                </form>
            </div>
            <p className="column">{todoItem.title}</p>
            <span className={`column ${todoItem.priority === "NotAssigned" ? "" : "tag is-info"}`}>{todoItem.priority === "NotAssigned" ? "" : todoItem.priority}</span>
            {showEdit()}
            <div className="column">
                <button className="is-pulled-right button" onClick={() => setToggled(!toggled)}>Edit</button>
                <button className="is-pulled-right button" onClick={() => deleteTodoById(todoItem.id)}>Delete</button>
            </div>
        </div>
    );
}

export default TodoShow;
