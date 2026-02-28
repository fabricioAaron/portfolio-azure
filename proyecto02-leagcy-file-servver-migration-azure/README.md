# ☁️ Modernización de Servidor de Archivos con Azure File Sync (Sin Impacto al Usuario)

---

##  Descripción del Proyecto

Este proyecto demuestra cómo modernizar un servidor de archivos tradicional de Windows llevándolo a la nube con Azure, **sin que los usuarios perciban ningún cambio en su experiencia diaria**.

El objetivo fue simular un escenario real de empresa donde:

* Los usuarios siguen accediendo igual que siempre
* IT migra el almacenamiento a la nube
  * No hay cambios en rutas ni en forma de trabajo

Los usuarios siguen conectándose a:

\fileserver\CompanyData

La única diferencia es que los datos realmente están en Azure.

---

##  Objetivo

Diseñar una arquitectura híbrida que permita:

* Migrar almacenamiento a la nube
* Mantener el acceso SMB tradicional
* No cambiar la experiencia del usuario
* Mantener permisos NTFS
* Mejorar resiliencia y disponibilidad
* Preparar el entorno para backup y DR

Todo esto sin impacto en producción.

---


---

### Capa Híbrida

<img width="1024" height="559" alt="image" src="https://github.com/user-attachments/assets/09c084a0-8b5b-4863-b6e0-9dcb82865794" />

---


---

## 🛠️ Tecnologías Usadas

* Windows Server 2022
* Azure Virtual Machine
* Azure Storage Account
* Azure File Sync
* Azure File Share
* Active Directory
* NTFS Permissions
* SMB

---

##  Implementación Paso a Paso

### 1. Crear Storage Account

Ir a:

Azure Portal → Storage Accounts → Crear

Configurar:

* Performance: Standard
* Redundancia: LRS
* Región: misma que la VM

<img width="762" height="740" alt="2" src="https://github.com/user-attachments/assets/5c6a9fff-0ad9-4899-b14a-806430bfbe79" />

---

### 2️. Crear File Share

Storage Account → File Shares → Crear

Nombre:

companydata

Performance:

Transaction Optimized :  pensado para cargas típicas de ficheros de usuario (muchas operaciones, tamaño moderado) con buen balance coste/rendimiento.

<img width="768" height="375" alt="3" src="https://github.com/user-attachments/assets/13f752ef-ebe1-4c67-9cce-c7fb54775655" />

---

### 3️. Crear Storage Sync Service

Buscar:

Storage Sync Services → Crear

Nombre: sync-ws-files-001 : es el servicio central de Azure File Sync. Actúa como “controlador” que sabe qué servidores están registrados, qué shares debe sincronizar y en qué grupos.



<img width="720" height="488" alt="4" src="https://github.com/user-attachments/assets/76d3d658-7c0d-4e46-bb1a-db3f16fe5f2e" />

---

### 4️. Crear Sync Group

Un Sync Group es como una “burbuja de sincronización”. Dentro de esa burbuja se definen:

Un cloud endpoint (el Azure File Share).

Uno o varios server endpoints (carpetas en servidores Windows on‑prem o en otras VMs).

sg-companydata agrupa todos los endpoints que deben contener la misma carpeta lógica “CompanyData”, tanto en Azure como en los servidores.

Al elegir Storage Account stfilesmig001 + File share companydata se define el cloud endpoint, que es la verdad “maestra” en Azure.

### Pasos​
Dentro del Sync Service:

Sync Groups → Crear

Nombre:

sg-companydata

<img width="776" height="405" alt="5" src="https://github.com/user-attachments/assets/6f742b85-32b0-49dc-a575-8842ec9fcd7d" />

<img width="625" height="442" alt="6" src="https://github.com/user-attachments/assets/9a572499-8694-4d2e-947c-bbaec04fc804" />

---

### 5️. Añadir Cloud Endpoint

Seleccionar:

Storage Account
File Share → companydata

---

### 6️. Preparar Windows Server

En la VM lo llamaremos wscompany001

Agregaremos a dominio

Crear ruta local:

D:\wscompany\CompanyData

Compartirla como:

\fileserver\CompanyData

<img width="689" height="722" alt="7" src="https://github.com/user-attachments/assets/32051f66-4710-40f5-aa74-736fb4819c13" />
<img width="1225" height="754" alt="8" src="https://github.com/user-attachments/assets/efb719a9-ff62-4890-9056-134e661a01a9" />
<img width="866" height="479" alt="9" src="https://github.com/user-attachments/assets/eb154f23-727b-4f80-ac99-50f1edb3620c" />
<img width="360" height="307" alt="10" src="https://github.com/user-attachments/assets/f3d61adb-4888-45d5-bc26-f312c1b8c754" />



---

### 7️. Instalar Azure File Sync Agent

Qué es el Agent:
Es un servicio Windows que se instala en cada servidor físico/virtual que va a participar en la sincronización. Actúa como “puerta de enlace” entre el directorio local del servidor y Azure File Sync.
El Agent se registra como servicio Windows (Storage Sync Agent) y crea USN Journal listeners para detectar cambios en tiempo real en las carpetas sincronizadas.

Incluye el WebView2 para el login de registro y el Storage Sync Service que se comunica con Azure sobre los cambios.

Reinicio obligatorio porque crea servicios críticos y modifica el filesystem filter driver.

### Pasos: 
Descargar desde Microsoft
Instalar en el servidor

<img width="961" height="310" alt="12" src="https://github.com/user-attachments/assets/fce215ad-e44a-4029-bf03-8e38b004f741" />


---

### 8️. Registrar el Servidor

Función técnica:

Token de Azure AD → autentica la VM con permisos mínimos en el Storage Sync Service.

Server ID único → Azure asigna un identificador al servidor registrado (wscompany.company.local).

Heartbeat → el agente empieza a enviar “pulso vital” cada 5 minutos para que Azure sepa que está online.

Selecciona sync-files-we-001 → vincula este servidor específico a tu servicio de sincronización de West Europe.

Resultado: La VM aparece en Azure como “Registered Server” (Online/Healthy). Ahora Azure “conoce” este servidor y puede asignarle endpoints de sincronización.
Abrir:

Server Registration

Conectarlo al:

sync-companydata

<img width="805" height="487" alt="15" src="https://github.com/user-attachments/assets/eaeada8e-0126-4458-9b3a-455666357864" />
<img width="1016" height="393" alt="16" src="https://github.com/user-attachments/assets/73ca6e51-d1a6-4354-a7f4-7e0a91896b4f" />


---

### 9️ . Crear Server Endpoint

Qué es un Server Endpoint:
Es la carpeta específica del servidor (D:\Shares\CompanyData) que se vincula a un cloud endpoint (el Azure File Share companydata). Es el “contrato de sincronización” entre local y cloud.

Dentro del Sync Group:

Add Server Endpoint

Ruta:

D:\wscompanydata\CompanyData

Activar:Cloud Tiering: 

###¿Cúal es su fúnción y en que implica ?
Los archivos “calientes” (usados recientemente) se mantienen completos en el disco local de la VM.

Los archivos “fríos” (no usados durante tiempo o cuando falta espacio) se convierten en stubs:

En el disco solo queda una entrada pequeña (4 KB) con metadatos.

El contenido real vive en el share de Azure Files.

En que repercute: 
* En la Fase 9 se habilita Cloud tiering en el server endpoint.
  Esto convierte la carpeta D:\Shares\CompanyData en una caché local del Azure File Share, manteniendo solo los archivos más usados en el servidor y
  dejando el resto como referencias ligeras cuyos datos residen en Azure. De este modo, el file server on‑premise reduce el uso de disco y la empresa puede escalar capacidad únicamente en la nube,
  sin ampliar almacenamiento local, a costa de depender de la conectividad a Azure para acceder a archivos “fríos”.
​

Cuando un usuario abre un stub desde \\fileserver\CompanyData, el agente recupera el contenido desde Azure de forma transparente.
<img width="779" height="502" alt="18" src="https://github.com/user-attachments/assets/03cce979-d62e-4d6f-9b0a-cb3efa8ce96e" />
<img width="545" height="795" alt="19" src="https://github.com/user-attachments/assets/89485e7b-1912-4af1-ab43-bc72cc90aa0c" />
<img width="1377" height="193" alt="20" src="https://github.com/user-attachments/assets/701830a4-16ef-4172-a8f8-915fd777fb71" />


---

## 🧪 Validación

Crear archivo:

test.txt

En:

 \\wscompanydata\CompanyData

Verificar que aparece en:

Azure → File Share → companydata

---

## 🏁 Resultado Final

El usuario sigue usando:

 \\wscompanydata\CompanyData


<img width="1086" height="262" alt="21" src="https://github.com/user-attachments/assets/4008130c-b0cb-4a5f-8ec4-9ee2fa04cad3" />




