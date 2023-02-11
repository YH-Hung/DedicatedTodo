import {createContext, useCallback, useState} from "react";
import axios from "axios";
import HttpCodeDesc from "../Util/HttpCodeDesc";

const defaultValue = {};

const TodoContext = createContext(defaultValue);

function Provider({children}) {
    const [todos, setTodos] = useState([]);
    const [byComplete, setByComplete] = useState(undefined);
    const [byPriority, setByPriority] = useState(undefined);
    const [errorObj, setErrorObj] = useState(undefined);


    const baseUrl = process.env.REACT_APP_API_URL;
    const genSuccessObj = (code) => ({
        title: HttpCodeDesc(code),
        status: code,
        errors: {Healthy: ["Without Error"]}
    });

    /*
    Success -> update local state and show success message.
     */
    const successFlow = (code, data) => {
        setTodos(data);
        setErrorObj(genSuccessObj(code));
    }

    /*
    Error -> Send HTTP request error to error boundary
     */
    const errorHandler = (err) => {
        const response = err.response
        if (response) {
            setErrorObj(response.data);
        } else {
            setErrorObj({title: "Service Unavailable", status: 503, errors: {BackEnd: ["Service unhealthy"]}})
        }
    }

    /*
    Fetch list from backend. Use Callback to avoid pointer varying
     */
    const fetchTodos = useCallback(async () => {
        try {
            const response = await axios.get(baseUrl);
            if (response.status === 204) {
                successFlow(204, []);
            } else {
                successFlow(response.status, response.data);
            }
        } catch (err) {
            errorHandler(err);
        }
    }, []);

    /*
    Filter list by conditions. NEVER use state as arguments due to delayed state update.
     */
    const filterTodos = async (isComplete, priority) => {
        try {
            const params = { byComplete: isComplete, byPriority: priority };
            const response = await axios.get(`${baseUrl}/filter`, {params});
            if (response.status === 204) {
                successFlow(204, []);
            } else {
                successFlow(response.status, response.data);
            }
        } catch (err) {
            errorHandler(err);
        }
    }

    /*
    Shared query single function.
     */
    async function querySingle(id) {
        const querySingleResponse = await axios.get(`${baseUrl}/${id}`);
        return querySingleResponse.data;
    }

    /*
    Post -> Get new id -> Query by id -> Add new item into local list.
     */
    const createNewTodo = async (todo) => {
        try {
            const postResponse = await axios.post(baseUrl,
                {postTitle: todo.title, postPriority: todo.priority});

            const newId = postResponse.data;
            const newTodo = await querySingle(newId);
            successFlow(postResponse.status, [...todos, newTodo]);
        } catch (err) {
            errorHandler(err);
        }
    };

    /*
    Patch -> Accepted -> Query by id -> Update local list.
     */
    const editTodoById = async (id, content) => {
        try {
            const patchResponse = await axios.patch(`${baseUrl}/${id}`,
                {
                    patchTitle: content.title,
                    patchComplete: content.isComplete,
                    patchPriority: content.priority
                });

            const renewedTodo = await querySingle(id);
            const updatedTodos = todos.map(t => {
                if (t.id === id) {
                    return {...t, ...renewedTodo};
                } else {
                    return t;
                }
            });

            successFlow(patchResponse.status, updatedTodos);
        } catch (err) {
            errorHandler(err);
        }
    };

    /*
    Delete -> Accepted -> Update local list
     */
    const deleteTodoById = async (id) => {
      try {
          const deleteResponse = await axios.delete(`${baseUrl}/${id}`)

          const updatedTodos = todos.filter(t => t.id!==id);
          successFlow(deleteResponse.status, updatedTodos);
      }  catch (err) {
          errorHandler(err);
      }
    };

    const valueToShare = {
        todos,
        fetchTodos,
        filterTodos,
        createNewTodo,
        editTodoById,
        deleteTodoById,
        byComplete,
        setByComplete,
        byPriority,
        setByPriority,
        errorObj
    };

    return (<TodoContext.Provider value={valueToShare}>
        {children}
    </TodoContext.Provider>);
}

export {Provider};
export default TodoContext;
