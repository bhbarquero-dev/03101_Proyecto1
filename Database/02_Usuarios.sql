/*
    02_Usuarios.sql
    Usuarios del sistema con rol Usuario (empleado) o Administrador.
    Requiere: 00_crear_bd.sql
*/

USE HelpDesk;
GO

CREATE TABLE dbo.Usuarios
(
    Id              INT            NOT NULL IDENTITY(1, 1),
    Nombre          NVARCHAR(100)  NOT NULL,
    Email           NVARCHAR(150)  NOT NULL,
    ContrasenaHash  NVARCHAR(255)  NOT NULL,    -- nunca se guarda la contrasena en texto plano
    Rol             NVARCHAR(20)   NOT NULL,
    FechaRegistro   DATETIME2(0)   NOT NULL CONSTRAINT DF_Usuarios_FechaRegistro DEFAULT (SYSDATETIME()),
    Estado          BIT            NOT NULL CONSTRAINT DF_Usuarios_Estado DEFAULT (1),    -- 1 = activo, 0 = baja logica

    CONSTRAINT PK_Usuarios PRIMARY KEY (Id),
    CONSTRAINT UQ_Usuarios_Email UNIQUE (Email),
    CONSTRAINT CK_Usuarios_Rol CHECK (Rol IN (N'Usuario', N'Administrador'))
);
GO
