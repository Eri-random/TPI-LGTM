-- Insertar roles predefinidos en la tabla 'rol'
INSERT INTO rol (nombre) VALUES 
('organizacion'), 
('usuario');

-- Insertar tres usuarios nuevos en la tabla 'usuario'
INSERT INTO usuario (nombre, apellido, email, contrasena, telefono, direccion, localidad, provincia, rol_id)
VALUES 
('Nombre1', 'Apellido1', 'test1@test.com', crypt('12345678', gen_salt('bf')), '1234567890', 'Calle Uno 123', 'Ciudad Uno', 'Provincia Uno', 2),
('Nombre2', 'Apellido2', 'test2@test.com', crypt('12345678', gen_salt('bf')), '2345678901', 'Calle Dos 234', 'Ciudad Dos', 'Provincia Dos', 2),
('Nombre3', 'Apellido3', 'test3@test.com', crypt('12345678', gen_salt('bf')), '3456789012', 'Calle Tres 345', 'Ciudad Tres', 'Provincia Tres', 2);

INSERT INTO public.necesidad(nombre, icono)
VALUES ('Ropa y vestimenta', 'assets/images/needs-icons/Jumper.svg'),
('Ropa de cama y baño', 'assets/images/needs-icons/Bed.svg'),
('Juguetes y Productos para Niños', 'assets/images/needs-icons/Carousel.svg'),
('Productos para Mascotas', 'assets/images/needs-icons/Dog.svg'),
('Productos para el Hogar', 'assets/images/needs-icons/Home.svg'),
('Accesorios', 'assets/images/needs-icons/Bag.svg');


INSERT INTO public.subcategoria(nombre, necesidad_id)
VALUES ('Camisetas', 1),
('Pantalones', 1),
('Chaquetas',1),
('Sábanas',2),
('Fundas de Almohada',2),
('Toallas',2),
('Mantitas',3),
('Baberos',3),
('Peluches',3),
('Ropa',4),
('Juguetes de tela',4),
('Camas',4),
('Fundas para Almohadones',5),
('Alfombras',5),
('Manteles',5),
('Mochilas',6),
('Bufandas',6),
('Guantes',6);

