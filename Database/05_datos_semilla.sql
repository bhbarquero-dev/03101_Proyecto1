/*
    05_datos_semilla.sql
    Datos de prueba: usuarios, tickets y comentarios.
    Requiere: 01_EstadosTicket.sql a 04_Comentarios.sql

    Cuentas de prueba (solo desarrollo):
        Correo                         Contrasena      Rol             Estado
        admin@helpdesk.test            Admin#2026      Administrador   Activo
        tecnico@helpdesk.test          Tecnico#2026    Administrador   Activo
        ana.jimenez@helpdesk.test      Usuario#2026    Usuario         Activo
        carlos.vargas@helpdesk.test    Usuario#2026    Usuario         Activo
        laura.solis@helpdesk.test      Usuario#2026    Usuario         Inactivo (no debe poder iniciar sesion)

    Los tecnicos de soporte son usuarios con rol Administrador.

    Formato de ContrasenaHash:
        PBKDF2-SHA256$<iteraciones>$<sal en Base64>$<hash en Base64>
        Sal de 16 bytes, hash de 32 bytes, 600 000 iteraciones.
        Equivale en .NET Framework a:
            new Rfc2898DeriveBytes(contrasena, sal, iteraciones, HashAlgorithmName.SHA256).GetBytes(32)

    Las fechas son relativas al momento de ejecucion para que los reportes por periodo
    siempre tengan datos recientes.
*/

USE HelpDesk;
GO

-- ---------------------------------------------------------------------------
-- Usuarios
-- ---------------------------------------------------------------------------
DECLARE @Ahora DATETIME2(0) = SYSDATETIME();

SET IDENTITY_INSERT dbo.Usuarios ON;

INSERT INTO dbo.Usuarios (Id, Nombre, Email, ContrasenaHash, Rol, FechaRegistro, Estado)
VALUES
    (1, N'Sofía Rojas',   N'admin@helpdesk.test',
        N'PBKDF2-SHA256$600000$WMqYlf6UVWO6Y1xubz58LQ==$A8OcDy7fvgkDnEWLUNem0hV4WOjujXg6aAFVxk/ZyBM=',
        N'Administrador', DATEADD(DAY, -90, @Ahora), 1),
    (2, N'Diego Mora',    N'tecnico@helpdesk.test',
        N'PBKDF2-SHA256$600000$tE5740JGuwQrGDnsmSr+Rw==$eioEys1GgCye50FIWyzR/gkvNW08zv5+3+tm0VkVYrY=',
        N'Administrador', DATEADD(DAY, -90, @Ahora), 1),
    (3, N'Ana Jiménez',   N'ana.jimenez@helpdesk.test',
        N'PBKDF2-SHA256$600000$EBsKn0azsCkbJD08ym3zNw==$0fqwQRujqsSBZANzwytahbp5zGxmdn7mgLer+11D1dE=',
        N'Usuario',       DATEADD(DAY, -80, @Ahora), 1),
    (4, N'Carlos Vargas', N'carlos.vargas@helpdesk.test',
        N'PBKDF2-SHA256$600000$O4YcHK0YR+8WqlyT2bpNCA==$5wf2z32e7hyKIfXhtnnyITTUYLJKPRHLKvDtQYWEXQg=',
        N'Usuario',       DATEADD(DAY, -75, @Ahora), 1),
    (5, N'Laura Solís',   N'laura.solis@helpdesk.test',
        N'PBKDF2-SHA256$600000$mDq7PHi+y3IYlIisaF4ANA==$TuClIZrt2oVtJBH7o1jSS0re3Fb+bBYvYzwjxrRzksI=',
        N'Usuario',       DATEADD(DAY, -70, @Ahora), 0);

SET IDENTITY_INSERT dbo.Usuarios OFF;
GO

-- ---------------------------------------------------------------------------
-- Tickets (al menos uno en cada estado, con y sin asignar)
-- Estados: 1 = Abierto, 2 = En Proceso, 3 = Resuelto, 4 = Cerrado
-- ---------------------------------------------------------------------------
DECLARE @Ahora DATETIME2(0) = SYSDATETIME();

SET IDENTITY_INSERT dbo.Tickets ON;

INSERT INTO dbo.Tickets (Id, Titulo, Descripcion, IdUsuarioSolicitante, IdUsuarioAsignado, IdEstado, FechaCreacion, FechaActualizacion, Categoria)
VALUES
    (1, N'Restablecer contraseña del ERP',
        N'No recuerdo la contraseña del ERP y la cuenta quedó bloqueada.',
        5, 1, 4, DATEADD(DAY, -60, @Ahora), DATEADD(HOUR, 2, DATEADD(DAY, -60, @Ahora)), N'Accesos'),
    (2, N'No enciende el monitor',
        N'El monitor de mi estación no enciende desde esta mañana; el indicador de energía está apagado.',
        3, 2, 4, DATEADD(DAY, -45, @Ahora), DATEADD(DAY, -43, @Ahora), N'Hardware'),
    (3, N'Error al abrir Outlook',
        N'Outlook muestra "No se puede iniciar Microsoft Outlook" al abrirlo.',
        3, 2, 3, DATEADD(DAY, -20, @Ahora), DATEADD(DAY, -19, @Ahora), N'Correo'),
    (4, N'Lentitud general del equipo',
        N'El equipo tarda varios minutos en iniciar y las aplicaciones se congelan con frecuencia.',
        3, 1, 3, DATEADD(DAY, -10, @Ahora), DATEADD(DAY, -8, @Ahora), N'Hardware'),
    (5, N'Sin acceso a la carpeta compartida de Finanzas',
        N'Al abrir \\servidor\finanzas aparece "Acceso denegado". Necesito acceso de lectura.',
        4, 1, 2, DATEADD(DAY, -6, @Ahora), DATEADD(DAY, -2, @Ahora), N'Accesos'),
    (6, N'La VPN se desconecta cada pocos minutos',
        N'Desde casa la VPN se desconecta cada 5 a 10 minutos y debo volver a conectarme.',
        4, 2, 2, DATEADD(DAY, -3, @Ahora), DATEADD(DAY, -1, @Ahora), N'Red'),
    (7, N'Instalar Power BI Desktop',
        N'Necesito Power BI Desktop para preparar los informes mensuales del área.',
        3, NULL, 1, DATEADD(DAY, -1, @Ahora), DATEADD(DAY, -1, @Ahora), N'Software'),
    (8, N'La impresora del segundo piso atasca el papel',
        N'La impresora compartida del segundo piso atasca cada hoja que se imprime a doble cara.',
        4, NULL, 1, DATEADD(HOUR, -5, @Ahora), DATEADD(HOUR, -5, @Ahora), N'Hardware');

SET IDENTITY_INSERT dbo.Tickets OFF;
GO

-- ---------------------------------------------------------------------------
-- Comentarios
-- ---------------------------------------------------------------------------
DECLARE @Ahora DATETIME2(0) = SYSDATETIME();

INSERT INTO dbo.Comentarios (IdTicket, IdUsuario, Texto, Fecha)
VALUES
    (1, 1, N'Se desbloqueó la cuenta y se envió una contraseña temporal a su correo.',          DATEADD(HOUR, 2, DATEADD(DAY, -60, @Ahora))),

    (2, 2, N'¿El cable de energía está bien conectado al monitor y al tomacorriente?',           DATEADD(HOUR, 1, DATEADD(DAY, -45, @Ahora))),
    (2, 3, N'Sí, ya lo revisé y probé otro tomacorriente.',                                     DATEADD(HOUR, 2, DATEADD(DAY, -45, @Ahora))),
    (2, 2, N'Se reemplazó la fuente de poder del monitor. Queda funcionando.',                   DATEADD(DAY, -43, @Ahora)),

    (3, 2, N'Se reparó el perfil de Outlook. Por favor confirme si ya puede abrirlo.',           DATEADD(DAY, -19, @Ahora)),

    (4, 1, N'Se liberó espacio en disco y se desactivaron programas de inicio innecesarios.',   DATEADD(DAY, -8, @Ahora)),

    (5, 1, N'Se solicitó la aprobación de acceso a la jefatura de Finanzas.',                    DATEADD(DAY, -5, @Ahora)),
    (5, 4, N'Mi jefatura ya envió la aprobación por correo.',                                    DATEADD(DAY, -2, @Ahora)),

    (6, 2, N'¿Qué tipo de conexión usa en casa: cable o Wi-Fi?',                                DATEADD(DAY, -2, @Ahora)),
    (6, 4, N'Uso Wi-Fi.',                                                                       DATEADD(DAY, -1, @Ahora));
GO
