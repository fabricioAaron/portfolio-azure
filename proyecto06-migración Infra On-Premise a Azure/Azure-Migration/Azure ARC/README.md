# Migración Híbrida y Modernización de Infraestructura On-Premise



## 📋 Índice

1. [Integración con Azure ARC](#integración-con-azure-arc)

# Documentación de Servidor On-Premise: AD (VM1)

<img width="735" height="547" alt="configuracion de azure arc en el servidor AD" src="https://github.com/user-attachments/assets/55e2b7e2-cce8-4ba6-8ceb-0646ce18892f" />

`configuración de azure ARC`

---

<img width="1337" height="1176" alt="ChatGPT Image 17 may 2026, 12_17_13" src="https://github.com/user-attachments/assets/3004b0bc-75b8-4869-a6e3-8a693247f2c4" />

`conexión con azure`

---

## Integración con Azure ARC
El servidor principal de la infraestructura local, identificado como **AD**, se ha proyectado hacia la nube utilizando **Azure Arc-enabled servers**. Esta configuración permite la gestión híbrida del servidor sin necesidad de migrar la carga de trabajo completa a Azure.

### Definición y Finalidad
**Azure Arc** es un servicio que extiende el plano de control de Azure (*Azure Resource Manager*) hacia recursos situados fuera de la plataforma (on-premise).

* **Finalidad en VM1:** Permitir que el servidor local sea visible en el inventario de Azure, facilitando la monitorización, la aplicación de políticas de cumplimiento (**Azure Policy**) y la gestión de actualizaciones desde una consola centralizada.

---

## 2. Justificación de la Solución
Se seleccionó **Azure Arc** como la herramienta de gestión híbrida tras descartar otras opciones debido a las siguientes restricciones técnicas del entorno:

### A. Incompatibilidad con Azure Migrate
* **Restricción:** La suscripción **Azure for Students** y las políticas de seguridad del entorno educativo limitan el despliegue del "appliance" necesario para Azure Migrate.
* **Limitación Técnica:** El proceso requiere permisos de administración profundos en el hipervisor (escenario restringido en el laboratorio) y cuotas de recursos que superan los límites permitidos para cuentas de estudiante.

### B. Restricción de Azure Hybrid Join
* **Restricción de Licenciamiento:** El uso de **Azure Hybrid Join** se vio imposibilitado por el tipo de licenciamiento disponible (**Education**).
* **Limitación de Identidad:** Este nivel de licencia, junto con las políticas de *Tenant* aplicadas, restringe el registro automático de dispositivos en **Microsoft Entra ID** (antiguo Azure AD), impidiendo la unión híbrida completa del dispositivo físico/virtual local.

---

> **Nota de Implementación:** Gracias a esta arquitectura basada en Arc, el servidor **AD** mantiene su rol local operativo mientras aprovecha las capacidades de gobernanza y seguridad de la nube.



