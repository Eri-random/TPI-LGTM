-- This script was generated by the ERD tool in pgAdmin 4.
-- Please log an issue at https://github.com/pgadmin-org/pgadmin4/issues/new/choose if you find any bugs, including reproduction steps.
BEGIN;

CREATE EXTENSION IF NOT EXISTS pgcrypto;

CREATE TABLE IF NOT EXISTS public."__EFMigrationsHistory"
(
    "MigrationId" character varying(150) COLLATE pg_catalog."default" NOT NULL,
    "ProductVersion" character varying(32) COLLATE pg_catalog."default" NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

CREATE TABLE IF NOT EXISTS public.donacion
(
    id integer NOT NULL GENERATED BY DEFAULT AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ),
    producto text COLLATE pg_catalog."default" NOT NULL,
    cantidad integer NOT NULL,
    usuario_id integer NOT NULL,
    organizacion_id integer NOT NULL,
    CONSTRAINT donacion_pk PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS public.idea
(
    id integer NOT NULL GENERATED BY DEFAULT AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ),
    titulo text COLLATE pg_catalog."default" NOT NULL,
    usuario_id integer NOT NULL,
    dificultad text COLLATE pg_catalog."default",
    CONSTRAINT idea_pkey PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS public.info_organizacion
(
    id integer NOT NULL GENERATED BY DEFAULT AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ),
    organizacion character varying(30) COLLATE pg_catalog."default" NOT NULL,
    descripcion_breve character varying(150) COLLATE pg_catalog."default" NOT NULL,
    descripcion_completa character varying(4000) COLLATE pg_catalog."default" NOT NULL,
    img text COLLATE pg_catalog."default" NOT NULL,
    organizacion_id integer NOT NULL,
    CONSTRAINT id PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS public.organizacion
(
    id integer NOT NULL GENERATED BY DEFAULT AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ),
    nombre text COLLATE pg_catalog."default" NOT NULL,
    cuit text COLLATE pg_catalog."default" NOT NULL,
    telefono text COLLATE pg_catalog."default" NOT NULL,
    direccion text COLLATE pg_catalog."default" NOT NULL,
    localidad text COLLATE pg_catalog."default" NOT NULL,
    provincia text COLLATE pg_catalog."default" NOT NULL,
    usuario_id integer NOT NULL,
    latitud double precision NOT NULL,
    longitud double precision NOT NULL,
    CONSTRAINT organizacion_pkey PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS public.paso
(
    id integer NOT NULL GENERATED BY DEFAULT AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ),
    paso_num integer NOT NULL,
    descripcion text COLLATE pg_catalog."default" NOT NULL,
    idea_id integer NOT NULL,
    CONSTRAINT paso_pk PRIMARY KEY (id)
        INCLUDE(id)
);

CREATE TABLE IF NOT EXISTS public.rol
(
    rol_id integer NOT NULL GENERATED BY DEFAULT AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ),
    nombre character varying(50) COLLATE pg_catalog."default" NOT NULL,
    CONSTRAINT rol_pkey PRIMARY KEY (rol_id)
);

CREATE TABLE IF NOT EXISTS public.sede
(
    id integer NOT NULL GENERATED BY DEFAULT AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ),
    direccion text COLLATE pg_catalog."default" NOT NULL,
    localidad text COLLATE pg_catalog."default" NOT NULL,
    provincia text COLLATE pg_catalog."default" NOT NULL,
    organizacion_id integer NOT NULL,
    nombre text COLLATE pg_catalog."default" NOT NULL,
    telefono text COLLATE pg_catalog."default" NOT NULL,
    latitud double precision NOT NULL,
    longitud double precision NOT NULL,
    CONSTRAINT sede_pkey PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS public.usuario
(
    id integer NOT NULL GENERATED BY DEFAULT AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ),
    nombre character varying(100) COLLATE pg_catalog."default",
    apellido character varying(100) COLLATE pg_catalog."default",
    email character varying(255) COLLATE pg_catalog."default",
    contrasena character varying(255) COLLATE pg_catalog."default",
    telefono character varying(20) COLLATE pg_catalog."default",
    direccion text COLLATE pg_catalog."default",
    localidad character varying(100) COLLATE pg_catalog."default",
    provincia character varying(100) COLLATE pg_catalog."default",
    rol_id integer NOT NULL,
    CONSTRAINT usuario_pkey PRIMARY KEY (id)
);

ALTER TABLE IF EXISTS public.donacion
    ADD CONSTRAINT donacion_organizacion_fk FOREIGN KEY (organizacion_id)
    REFERENCES public.organizacion (id) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION;
CREATE INDEX IF NOT EXISTS "IX_donacion_organizacion_id"
    ON public.donacion(organizacion_id);


ALTER TABLE IF EXISTS public.donacion
    ADD CONSTRAINT donacion_usuario_fk FOREIGN KEY (usuario_id)
    REFERENCES public.usuario (id) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION;
CREATE INDEX IF NOT EXISTS "IX_donacion_usuario_id"
    ON public.donacion(usuario_id);


ALTER TABLE IF EXISTS public.idea
    ADD CONSTRAINT idea_usuario_fk FOREIGN KEY (usuario_id)
    REFERENCES public.usuario (id) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION
    NOT VALID;


ALTER TABLE IF EXISTS public.info_organizacion
    ADD CONSTRAINT organizacion_fkey FOREIGN KEY (organizacion_id)
    REFERENCES public.organizacion (id) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION;
CREATE INDEX IF NOT EXISTS organizacion_id
    ON public.info_organizacion(organizacion_id);


ALTER TABLE IF EXISTS public.organizacion
    ADD CONSTRAINT usuario_fkey FOREIGN KEY (usuario_id)
    REFERENCES public.usuario (id) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION;
CREATE INDEX IF NOT EXISTS usuario_id_key
    ON public.organizacion(usuario_id);


ALTER TABLE IF EXISTS public.paso
    ADD CONSTRAINT paso_idea_fk FOREIGN KEY (idea_id)
    REFERENCES public.idea (id) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION
    NOT VALID;


ALTER TABLE IF EXISTS public.sede
    ADD CONSTRAINT organizacion_fkey FOREIGN KEY (organizacion_id)
    REFERENCES public.organizacion (id) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION;


ALTER TABLE IF EXISTS public.usuario
    ADD CONSTRAINT usuario_rol_id_fkey FOREIGN KEY (rol_id)
    REFERENCES public.rol (rol_id) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION;
CREATE INDEX IF NOT EXISTS "IX_usuario_rol_id"
    ON public.usuario(rol_id);

CREATE TABLE IF NOT EXISTS public.necesidad
(
    id integer NOT NULL GENERATED BY DEFAULT AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ),
    nombre text COLLATE pg_catalog."default" NOT NULL,
    icono text COLLATE pg_catalog."default" NOT NULL,
    CONSTRAINT necesidad_pkey PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS public.subcategoria
(
    id integer NOT NULL GENERATED BY DEFAULT AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ),
    nombre text COLLATE pg_catalog."default" NOT NULL,
    necesidad_id integer NOT NULL,
    CONSTRAINT subcategorias_pkey PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS public.organizacion_necesidad
(
    organizacion_id integer NOT NULL,
    subcategoria_id integer NOT NULL,
    CONSTRAINT organizacion_necesidad_pk PRIMARY KEY (organizacion_id, subcategoria_id)
);

ALTER TABLE IF EXISTS public.organizacion_necesidad
    ADD CONSTRAINT organizacion_necesidad_fk FOREIGN KEY (organizacion_id)
    REFERENCES public.organizacion (id) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION
    NOT VALID;


ALTER TABLE IF EXISTS public.organizacion_necesidad
    ADD CONSTRAINT organizacion_subcategoria_fk FOREIGN KEY (subcategoria_id)
    REFERENCES public.subcategoria (id) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION
    NOT VALID;


ALTER TABLE IF EXISTS public.subcategoria
    ADD CONSTRAINT subcategoria_necesidad_fk FOREIGN KEY (necesidad_id)
    REFERENCES public.necesidad (id) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION
    NOT VALID;

END;