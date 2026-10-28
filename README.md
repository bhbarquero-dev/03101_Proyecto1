# 03101_Proyecto1
UNED - Proyecto 1 del curso 03101

Sistema de gestión de solicitudes de soporte técnico (Help Desk) con ASP.NET MVC (.NET Framework 4.8.1), SQL Server, JavaScript y AJAX.

## Requisitos

- Visual Studio 2026 con la carga de trabajo **ASP.NET and web development** y las herramientas de .NET Framework 4.8.1
- SQL Server Express LocalDB (instancia `(localdb)\MSSQLLocalDB`, incluida con Visual Studio)

## Crear la base de datos

Los scripts están en la carpeta `Database` y deben ejecutarse en este orden:

| Script | Contenido |
|---|---|
| `00_crear_bd.sql` | Crea la base de datos `HelpDesk` |
| `01_EstadosTicket.sql` | Catálogo de estados y sus valores |
| `02_Usuarios.sql` | Tabla de usuarios |
| `03_Tickets.sql` | Tabla de tickets |
| `04_Comentarios.sql` | Tabla de comentarios |
| `05_datos_semilla.sql` | Usuarios, tickets y comentarios de prueba |

> **Atención:** `00_crear_bd.sql` elimina la base de datos `HelpDesk` si ya existe. Es solo para desarrollo.

**Desde Visual Studio:** abrir cada script desde la carpeta de solución `Database`, conectarse a `(localdb)\MSSQLLocalDB` y ejecutarlo (Ctrl+Shift+E).

**Desde la terminal**, en la raíz del repositorio:

```
sqlcmd -S "(localdb)\MSSQLLocalDB" -b -i Database/00_crear_bd.sql -i Database/01_EstadosTicket.sql -i Database/02_Usuarios.sql -i Database/03_Tickets.sql -i Database/04_Comentarios.sql -i Database/05_datos_semilla.sql
```

## Ejecutar la aplicación

1. Verificar que la cadena de conexión `HelpDesk` de `HelpDesk/Web.config` apunte a `(localdb)\MSSQLLocalDB`.
2. Abrir `HelpDesk.slnx` en Visual Studio y ejecutar con F5.

## Cuentas de prueba

Las crea `05_datos_semilla.sql` (solo para desarrollo):

| Correo | Contraseña | Rol |
|---|---|---|
| admin@helpdesk.test | Admin#2026 | Administrador |
| tecnico@helpdesk.test | Tecnico#2026 | Administrador |
| ana.jimenez@helpdesk.test | Usuario#2026 | Usuario |
| carlos.vargas@helpdesk.test | Usuario#2026 | Usuario |
| laura.solis@helpdesk.test | Usuario#2026 | Usuario (inactivo, no puede iniciar sesión) |

Los técnicos de soporte son usuarios con rol Administrador. El inicio de sesión tarda unos 3 segundos porque las contraseñas usan PBKDF2-HMAC-SHA256 con 600 000 iteraciones.
