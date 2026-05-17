# Migración Híbrida y Modernización de Infraestructura On-Premise



## 📋 Índice

1. [Integración con Azure ARC](#integración-con-azure-arc)
2. [Integración de la carpeta compartida de AD con Azure File Sync](#integracion-de-la-carpeta-compartida-de-ad-con-azure-file-sync)

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

## Integración de la carpeta compartida de AD con Azure File Sync

Además de registrar el servidor **AD** en Azure Arc, se ha modernizado la carpeta compartida `\\AD\shares` mediante **Azure File Sync**, manteniendo el acceso SMB on‑premise pero sincronizando su contenido a un Azure File Share para disponer de copia en la nube y backup gestionado.

### 1. Creación del Azure File Share y backup

1. En una cuenta de almacenamiento (por ejemplo `backupdbreservas`) se crea un **Azure File Share** llamado `shares`, con tier `Transaction optimized`.
2. Durante la creación se habilita **Azure Backup**:
   - Recovery Services Vault en el grupo `lab-migration-onpremise`.
   - Política de backup diaria (`DailyPolicy-...`) con retención de 30 días.
3. En el explorador del File Share se crean las subcarpetas `Finanzas`, `RRHH` y `Ventas`, que reflejan la estructura de la carpeta `shares` del servidor AD.

<img width="724" height="486" alt="configuración de azure file sync" src="https://github.com/user-attachments/assets/7a10618f-7236-4db5-be33-e4618f9855bf" />

`configuramos Azure File Sync `

---

<img width="1009" height="339" alt="instalamos en nuestro servidor local " src="https://github.com/user-attachments/assets/00c2dc69-7618-4a9e-a92a-88e4cf2dfbe6" />

`instalamos el agente en nuestro servidor AD.template.local para instalar`

---

### 2. Despliegue de Azure File Sync (Storage Sync Service)

1. En Azure se crea un **Storage Sync Service** llamado `sync-file-ad` en el grupo `lab-migration-onpremise` y región `Spain Central`.
2. Desde el portal se descarga el **Azure File Sync Agent** para Windows Server y se instala en el servidor `AD.template.local`.
3. Durante la instalación, el agente registra el servidor en el Storage Sync Service:
   - Subscription: `Azure for Students`.
   - Resource group: `lab-migration-onpremise`.
   - Storage Sync Service: `sync-file-ad`.

<img width="830" height="525" alt="azure file sync en on premise apuntando a azure " src="https://github.com/user-attachments/assets/de267885-85fa-46ac-961f-7e73f8100565" />

---
   
### 3. Creación del grupo de sincronización

1. En el Storage Sync Service se crea un **Sync group** llamado `sg-shares-ad`.
2. En el apartado **Cloud endpoint** se selecciona:
   - Subscription: la de laboratorio.
   - Storage account: `backupdbreservas`.
   - Azure File Share: `shares`.

Esto define el extremo en Azure con el que se sincronizarán los datos del servidor AD.


<img width="606" height="459" alt="creamos la sync de onpremise a azure " src="https://github.com/user-attachments/assets/dc938ed4-0e1b-4ea4-8162-7fbed85f1681" />

`creamos la sincronización`

---

### 4. Añadir el servidor AD como endpoint y habilitar Cloud Tiering

1. Dentro del Sync group `sg-shares-ad` se selecciona **Add server endpoint**.
2. Se configura:
   - Registered Server: `AD.template.local`.
   - Path: `E:\shares` (ruta local de la carpeta compartida donde están Finanzas, RRHH y Ventas).
3. Se habilita **Cloud Tiering** para que solo una parte de los archivos permanezca físicamente en el servidor y el resto se mantenga en Azure:
   - Cloud Tiering: `Enable`.
   - Volume Free Space Policy: 20 % de espacio libre mínimo en el volumen.

### 5. Configuración de conectividad mediante Service Endpoint

Para asegurar la conectividad privada entre el servidor y la cuenta de almacenamiento:

1. En la red virtual donde se integra el servidor se configura un **Service Endpoint** hacia el servicio `Microsoft.Storage` asociado a la cuenta `backupdbreservas`.
2. Se añade el endpoint apuntando a la subred utilizada por el servidor, permitiendo que el tráfico SMB de Azure File Sync vaya de forma directa y segura hacia el File Share.

<img width="1137" height="491" alt="configuramos el service enpoint" src="https://github.com/user-attachments/assets/0a77e302-c997-4cf6-81ac-586f2a07d41d" />

`configuramos el cloud endpoint (se alamcena en un storage accounte en un file server) y configuramos el server endpoint es donde apuntamos nuestro servidor local`

---

<img width="568" height="816" alt="add endpoint " src="https://github.com/user-attachments/assets/b8bd45ff-f087-4af5-8ee3-7b105e2e0ea0" />

`configuramos nuestro endpoint`

---


### 6. Resultado: carpeta compartida híbrida

Tras completar la sincronización inicial:

- La carpeta `\\AD\shares` sigue siendo accesible para los usuarios on‑premise con la misma estructura de departamentos (Finanzas, RRHH, Ventas).
- El contenido se replica en el Azure File Share `shares`, donde:
  - Se dispone de **backup automático** mediante Azure Backup.
  - Se puede recuperar información incluso en caso de fallo del servidor local.

<img width="1151" height="399" alt="la carpet ade share funciona " src="https://github.com/user-attachments/assets/2727aa40-7d8d-4a9d-a971-36252fb39222" />

`comprobamos el funcionamiento`

---
De este modo, el servidor de ficheros on‑premise se convierte en un recurso híbrido: mantiene la funcionalidad SMB local para los usuarios del dominio, pero respalda sus datos en la nube con alta durabilidad y capacidades de recuperación avanzadas gracias a Azure File Sync y Azure Backup.


