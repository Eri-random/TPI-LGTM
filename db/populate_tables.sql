-- Insertar un usuario administrador en la tabla 'usuario'
INSERT INTO usuario (nombre, apellido, email, contrasena, telefono, direccion, localidad, provincia, rol_id)
VALUES ('NombreOrg', 'ApellidoOrg', 'test@test.com', crypt('12345678', gen_salt('bf')), '1234567890', 'Calle Falsa 123', 'Ciudad', 'Provincia', 1);

-- Insertar tres usuarios nuevos en la tabla 'usuario'
INSERT INTO usuario (nombre, apellido, email, contrasena, telefono, direccion, localidad, provincia, rol_id)
VALUES 
('Nombre1', 'Apellido1', 'test1@test.com', crypt('12345678', gen_salt('bf')), '1234567890', 'Calle Uno 123', 'Ciudad Uno', 'Provincia Uno', 2),
('Nombre2', 'Apellido2', 'test2@test.com', crypt('12345678', gen_salt('bf')), '2345678901', 'Calle Dos 234', 'Ciudad Dos', 'Provincia Dos', 2),
('Nombre3', 'Apellido3', 'test3@test.com', crypt('12345678', gen_salt('bf')), '3456789012', 'Calle Tres 345', 'Ciudad Tres', 'Provincia Tres', 2);
