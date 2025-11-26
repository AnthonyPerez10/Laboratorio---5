CREATE DATABASE DBAlumnos;  
USE DBAlumnos;
GO


CREATE TABLE Alumnos (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,
    Apellido NVARCHAR(100) NOT NULL,
    Cedula NVARCHAR(20) NOT NULL,
    Carrera NVARCHAR(50) NOT NULL,
    Semestre NVARCHAR(20) NOT NULL,
    Jornada NVARCHAR(20) NOT NULL,
    Usuario NVARCHAR(50) NOT NULL,
    Contrasena NVARCHAR(100) NOT NULL,
    RecibirNotificaciones BIT NOT NULL DEFAULT 0,
    FechaRegistro DATETIME NOT NULL DEFAULT GETDATE()
);

select * from Alumnos;