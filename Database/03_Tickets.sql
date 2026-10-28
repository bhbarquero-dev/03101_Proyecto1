/*
    03_Tickets.sql
    Solicitudes de soporte tecnico.
    Requiere: 01_EstadosTicket.sql, 02_Usuarios.sql
*/

USE HelpDesk;
GO

CREATE TABLE dbo.Tickets
(
    Id                    INT            NOT NULL IDENTITY(1, 1),
    Titulo                NVARCHAR(150)  NOT NULL,
    Descripcion           NVARCHAR(MAX)  NOT NULL,
    IdUsuarioSolicitante  INT            NOT NULL,
    IdUsuarioAsignado     INT            NULL,       -- NULL mientras no se asigna un responsable
    IdEstado              INT            NOT NULL CONSTRAINT DF_Tickets_IdEstado DEFAULT (1),    -- 1 = Abierto
    FechaCreacion         DATETIME2(0)   NOT NULL CONSTRAINT DF_Tickets_FechaCreacion DEFAULT (SYSDATETIME()),
    FechaActualizacion    DATETIME2(0)   NOT NULL CONSTRAINT DF_Tickets_FechaActualizacion DEFAULT (SYSDATETIME()),
    Categoria             NVARCHAR(50)   NOT NULL,

    CONSTRAINT PK_Tickets PRIMARY KEY (Id),
    CONSTRAINT FK_Tickets_Usuarios_Solicitante FOREIGN KEY (IdUsuarioSolicitante) REFERENCES dbo.Usuarios (Id),
    CONSTRAINT FK_Tickets_Usuarios_Asignado    FOREIGN KEY (IdUsuarioAsignado)    REFERENCES dbo.Usuarios (Id),
    CONSTRAINT FK_Tickets_EstadosTicket        FOREIGN KEY (IdEstado)             REFERENCES dbo.EstadosTicket (Id)
);
GO

-- SQL Server no indexa las claves foraneas automaticamente
CREATE INDEX IX_Tickets_IdUsuarioSolicitante ON dbo.Tickets (IdUsuarioSolicitante);
CREATE INDEX IX_Tickets_IdUsuarioAsignado    ON dbo.Tickets (IdUsuarioAsignado);
CREATE INDEX IX_Tickets_IdEstado             ON dbo.Tickets (IdEstado);
GO
