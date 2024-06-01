-- Insertar roles predefinidos en la tabla 'rol'
INSERT INTO rol (nombre) VALUES 
('organizacion'), 
('usuario');

-- Insertar tres usuarios nuevos en la tabla 'usuario'
INSERT INTO usuario (nombre, apellido, email, contrasena, telefono, direccion, localidad, provincia,cuit, rol_id)
VALUES 
('Nombre1', 'Apellido1', 'test1@test.com', crypt('12345678', gen_salt('bf')), '1234567890', 'Calle Uno 123', 'Ciudad Uno', 'Provincia Uno', 2),
('Nombre2', 'Apellido2', 'test2@test.com', crypt('12345678', gen_salt('bf')), '2345678901', 'Calle Dos 234', 'Ciudad Dos', 'Provincia Dos', 2),
('Nombre3', 'Apellido3', 'test3@test.com', crypt('12345678', gen_salt('bf')), '3456789012', 'Calle Tres 345', 'Ciudad Tres', 'Provincia Tres', 2);

INSERT INTO public.necesidad(nombre, icono)
VALUES ('Ropa y vestimenta', 'http://localhost:5203/iconos/Jumper.png'),
('Ropa de cama y baño', 'http://localhost:5203/iconos/Single-Bed.png');

INSERT INTO public.subcategoria(nombre, necesidad_id)
VALUES ('Camisetas', 1),
('Pantalones', 1),
('Chaquetas',1),
('Sábanas',2),
('Fundas de Almohada',2),
('Toallas',2);