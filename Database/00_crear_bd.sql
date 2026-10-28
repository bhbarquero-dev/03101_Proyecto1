/*
    00_crear_bd.sql
    Crea la base de datos HelpDesk.

    ATENCION: si la base de datos ya existe, la elimina junto con todos sus datos.
    Uso exclusivo para desarrollo (SQL Server Express LocalDB).

    Orden de ejecucion:
        00_crear_bd.sql
        01_EstadosTicket.sql
        02_Usuarios.sql
        03_Tickets.sql
        04_Comentarios.sql
        05_datos_semilla.sql
*/

USE master;
GO

IF DB_ID(N'HelpDesk') IS NOT NULL
BEGIN
    -- Cierra las conexiones abiertas para poder eliminarla
    ALTER DATABASE HelpDesk SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE HelpDesk;
END
GO

CREATE DATABASE HelpDesk;
GO
