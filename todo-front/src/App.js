import TodoCreate from "./components/TodoCreate";
import TodoList from "./components/TodoList";
import {useContext, useEffect} from "react";
import TodoContext from "./context/todos";
import ErrorDisplay from "./components/ErrorDisplay";

function App() {
    const {fetchTodos, errorObj} = useContext(TodoContext);
    useEffect(() => fetchTodos(), [fetchTodos]);

  return (
      <div>
          {errorObj ? <ErrorDisplay {...errorObj} />
              : <h3>Initializing...</h3>}
          <TodoCreate />
          <TodoList />
      </div>
  );
}

export default App;
