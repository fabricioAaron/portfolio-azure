# Proyecto de migración Infra On-Premise a Azure

Este repositorio contiene el caso de estudio de migración de una infraestructura on-premise hacia Azure, junto con la aplicación web de reservas en ASP.NET Core usada como base funcional del escenario.

## Contenido

- `On-Premise/README.md`: descripción de la infraestructura física y lógica.
- `On-Premise/codigo-pagina-web/`: aplicación web ASP.NET Core MVC.
- `azure-pipelines.yml`: pipeline de Azure DevOps para compilar el proyecto.

## Cómo ejecutar la aplicación

1. Entra en `On-Premise/codigo-pagina-web`.
2. Ejecuta `dotnet build` para validar la solución.
3. Ejecuta `dotnet run` para levantar la aplicación.

La aplicación usa una base de datos en memoria por defecto si no se configura una cadena de conexión SQL Server.

## Configuración

Si quieres conectar la app a servicios reales, define estas secciones en `appsettings.json` o con variables de entorno:

- `ConnectionStrings:ReservasDb`
- `RabbitMq`
- `EmailSettings`

## Azure DevOps

La pipeline incluida compila el proyecto ASP.NET Core con .NET 8. Si quieres que suba el código a un repositorio de Azure DevOps, necesito la URL del repo remoto o que cambies el `origin` local al repositorio de Azure DevOps.