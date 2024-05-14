-- Habilitar la extensión pgcrypto para encriptaciar la password
CREATE EXTENSION IF NOT EXISTS pgcrypto;

-- Crear la tabla 'rol' para almacenar los roles de los usuarios
CREATE TABLE rol (
    rol_id SERIAL PRIMARY KEY,
    nombre VARCHAR(50) UNIQUE NOT NULL
);

-- Insertar roles predefinidos en la tabla 'rol'
INSERT INTO rol (nombre) VALUES 
('organizacion'), 
('usuario');

-- Crear la tabla 'usuario' para almacenar información del usuario
CREATE TABLE usuario (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(100),
    apellido VARCHAR(100),
    email VARCHAR(255) UNIQUE,
    contrasena VARCHAR(255),
    telefono VARCHAR(20),
    direccion TEXT,
    localidad VARCHAR(100),
    provincia VARCHAR(100),
    cuit VARCHAR(100),
    rol_id INT,
    FOREIGN KEY (rol_id) REFERENCES rol (rol_id)
);
