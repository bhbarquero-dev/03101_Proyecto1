/*
    04_Comentarios.sql
    Comentarios asociados a un ticket.
    Requiere: 02_Usuarios.sql, 03_Tickets.sql
*/

USE HelpDesk;
GO

CREATE TABLE dbo.Comentarios
(
    Id        INT             NOT NULL IDENTITY(1, 1),
    IdTicket  INT             NOT NULL,
    IdUsuario INT             NOT NULL,
    Texto     NVARCHAR(1000)  NOT NULL,
    Fecha     DATETIME2(0)    NOT NULL CONSTRAINT DF_Comentarios_Fecha DEFAULT (SYSDATETIME()),

    CONSTRAINT PK_Comentarios PRIMARY KEY (Id),
    -- Unica relacion con borrado en cascada: al eliminar un ticket se eliminan sus comentarios
    CONSTRAINT FK_Comentarios_Tickets  FOREIGN KEY (IdTicket)  REFERENCES dbo.Tickets (Id) ON DELETE CASCADE,
    CONSTRAINT FK_Comentarios_Usuarios FOREIGN KEY (IdUsuario) REFERENCES dbo.Usuarios (Id)
);
GO

CREATE INDEX IX_Comentarios_IdTicket ON dbo.Comentarios (IdTicket);
GO
