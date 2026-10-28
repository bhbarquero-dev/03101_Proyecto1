/*
    01_EstadosTicket.sql
    Catalogo de estados de un ticket y sus valores fijos.
    Requiere: 00_crear_bd.sql
*/

USE HelpDesk;
GO

CREATE TABLE dbo.EstadosTicket
(
    Id      INT           NOT NULL,
    Nombre  NVARCHAR(30)  NOT NULL,

    CONSTRAINT PK_EstadosTicket PRIMARY KEY (Id),
    CONSTRAINT UQ_EstadosTicket_Nombre UNIQUE (Nombre)
);
GO

-- Los Id son fijos porque la aplicacion los referencia (1 = Abierto es el estado inicial)
INSERT INTO dbo.EstadosTicket (Id, Nombre)
VALUES
    (1, N'Abierto'),
    (2, N'En Proceso'),
    (3, N'Resuelto'),
    (4, N'Cerrado');
GO
