insert into todo.priorities (id, priority_level)
values
    (1, 'HIGH'),
    (2, 'MEDIUM'),
    (3, 'LOW');

insert into todo.todos (title, is_complete, priority_id)
values
    ('todo app', false, 1),
    ('Learn Quarkus', false, 2),
    ('Push to Github', false, null);
