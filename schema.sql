CREATE TABLE todo.priorities (
  id INTEGER PRIMARY KEY ,
  priority_level    VARCHAR
);

CREATE TABLE todo.todos (
    id SERIAL PRIMARY KEY ,
    title VARCHAR,
    is_complete BOOLEAN DEFAULT false,
    priority_id INTEGER REFERENCES todo.priorities(id)
);