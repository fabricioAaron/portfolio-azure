# Migración Híbrida y Modernización de Infraestructura On-Premise




## Índice de la infraestructura

1. [Integración con Azure ARC](#Integración-con-azure-arc)
2. [Servidor de Directorio Activo, DNS y Carpeta compartida](#servidor-de-directorio-activo-dns-y-carpeta-compartida)
3. [Servidor de Aplicaciones (.NET + SQL Server)](#servidor-de-aplicaciones-net--sql-server)
4. [Servidor de Mensajería (Ubuntu + RabbitMQ)](#servidor-de-mensajeria-ubuntu--rabbitmq)
5. [Servidor de Copias de Seguridad (Veeam)](#servidor-de-copias-de-seguridad-veeam)

On-Premise: AD (VM1)
1. Integración con Azure ARC

El servidor principal de la infraestructura local, identificado como AD, se ha proyectado hacia la nube utilizando Azure Arc-enabled servers. Esta configuración permite la gestión híbrida del servidor sin necesidad de migrar la carga de trabajo completa.
Definición y Finalidad

Azure Arc es un servicio que extiende el plano de control de Azure (Azure Resource Manager) hacia recursos situados fuera de la plataforma.

    Finalidad en VM1: Permitir que el servidor local sea visible en el inventario de Azure, facilitando la monitorización, la aplicación de políticas de cumplimiento (Azure Policy) y la gestión de actualizaciones desde una consola centralizada.

Justificación de la Solución

Se seleccionó Azure Arc como la herramienta de gestión híbrida tras descartar otras opciones por las siguientes restricciones técnicas:

    Incompatibilidad con Azure Migrate: * La suscripción Azure for Students y las políticas de seguridad del entorno educativo limitan el despliegue del "appliance" necesario para Azure Migrate, el cual requiere permisos de administración profundos en el hipervisor y cuotas de recursos que superan los límites de la cuenta de estudiante.

    Restricción de Azure Hybrid Join:

        El uso de Azure Hybrid Join se vio imposibilitado por el tipo de licenciamiento disponible (Education). Este nivel de licencia, junto con las políticas de Tenant aplicadas, restringe el registro automático de dispositivos en Microsoft Entra ID (antiguo Azure AD), impidiendo la unión híbrida completa del dispositivo.
