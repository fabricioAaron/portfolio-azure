# 🏗️  Infraestructura On-Premise

<img width="2816" height="1536" alt="Gemini_Generated_Image_8vwqht8vwqht8vwq (1)" src="https://github.com/user-attachments/assets/9f63fc77-5769-4ae8-b9bb-db3c1f96f40c" />

# Esquema 

| Nombre de la Máquina | Dirección IP     | Sistema Operativo        | Roles / Servicios Instalados                                                                 | Función en la Infraestructura                                      |
|----------------------|------------------|---------------------------|------------------------------------------------------------------------------------------------|---------------------------------------------------------------------|
| ESXi Hypervisor      | 192.168.1.139    | VMware ESXi (nested)     | Hipervisor, 2 datastores, red interna 192.168.1.0/24                                           | Plataforma de virtualización donde corren las 4 VMs                 |
| AD                   | 192.168.1.10     | Windows Server            | Active Directory Domain Services, DNS, Carpeta compartida `\\AD\shares`, Usuarios y grupos     | Controlador de dominio, DNS, recursos compartidos                   |
| APP                  | 192.168.1.20     | Windows Server            | Aplicación web .NET, SQL Server (BD reservas), integración con RabbitMQ                        | Servidor de aplicación y base de datos                              |
| RabbitMQ / Docker    | 192.168.1.30     | Ubuntu Server + Docker    | Docker Engine, Contenedor RabbitMQ (build propio con Dockerfile)                               | Mensajería asíncrona, colas para la aplicación                      |
| Veeam Backup Server  | 192.168.1.40     | Windows Server            | Veeam Backup & Replication, conexión a ESXi, agente Veeam en AD, repositorio local             | Backup de AD, shares, restauración granular, protección de datos    |

# Objetivo del proyecto

Este proyecto simula una  infraestructura de empresa con:
- Directorio activo y DNS.
- Servidor de aplicaciones .NET + SQL Server.
- Servidor de mensajería RabbitMQ.
- Servidor de copias de seguridad con Veeam.

El entorno se usa para practicar administración de sistemas, copias de seguridad y flujos de datos entre servicios que posteriormente el cliente va migrar la nube (Azure).


## Índice de la infraestructura

1. [Hipervisor VMware ESXi](#hipervisor-vmware-esxi)
2. [Servidor de Directorio Activo, DNS y Carpeta compartida](#servidor-de-directorio-activo-dns-y-carpeta-compartida)
3. [Servidor de Aplicaciones (.NET + SQL Server)](#servidor-de-aplicaciones-net--sql-server)
4. [Servidor de Mensajería (Ubuntu + RabbitMQ)](#servidor-de-mensajeria-ubuntu--rabbitmq)
5. [Servidor de Copias de Seguridad (Veeam)](#servidor-de-copias-de-seguridad-veeam)


## Hipervisor VMware ESXi

### Arquitectura general
<img width="1821" height="590" alt="imagen" src="https://github.com/user-attachments/assets/0c8f365b-78cf-400c-97cc-1dddf8473552" />

**Capa física / host**

- Equipo físico: Intel Core i7‑11800H, 16 GB RAM.
- Sistema operativo host: Windows con VMware Workstation instalado.
- Hipervisor nested: VMware ESXi 8.0.3.
- IP de gestión ESXi (estática): `192.168.1.139`.

<img width="1011" height="491" alt="1" src="https://github.com/user-attachments/assets/6e31dd16-03e8-4b3c-b17a-4de23eed7ea0" />

**Red**

- Rango de red: `192.168.1.0/24`.
- vSwitch: `vSwitch0`.
- Port group: `Lan-Servers` (VLAN ID 0).
- Todas las VMs están conectadas al port group `Lan-Servers`.

<img width="802" height="541" alt="2" src="https://github.com/user-attachments/assets/ba49a047-9091-4190-b38e-1d91aeec571f" />

<img width="1226" height="269" alt="3" src="https://github.com/user-attachments/assets/c4b956e7-4637-44ed-bc2b-e32050005fdc" />

---
## Servidor de Directorio Activo, DNS y Carpeta compartida

### 1. Información general

- Nombre del servidor: `AD.template.local`
- Rol: Controlador de dominio, servidor DNS y servidor de ficheros.
- Dominio: `template.local`
- Usuarios, grupos y recursos organizados mediante Unidades Organizativas (OU).

### 2. Estructura de Active Directory

Se ha creado la siguiente estructura básica de OUs:

- `template.local`
  - `Departamentos`
  - `Usuarios`
  - `Recursos`
    - `Shares`

La OU **Departamentos** agrupa los grupos de seguridad por área de la empresa.[file:57]  
La OU **Usuarios** contiene las cuentas de usuario finales que se usan en el laboratorio.[file:56]

### 3. Grupos de seguridad por departamento

Dentro de la OU `Departamentos` se han creado los siguientes grupos de seguridad de tipo global:

- `grupos_finanzas`
- `grupos_RRHH`
- `grupos_ventas`[file:57]

Estos grupos representan a los departamentos de la organización y se utilizan para asignar permisos de acceso a recursos compartidos.

### 4. Usuarios de dominio

En la OU `Usuarios` se han creado usuarios de ejemplo, por ejemplo:

- `Ana Lopez`
- `Carlos Ruiz`
- `Juan Peres`
- `Maria Gomez`[file:56]

Cada usuario pertenece al grupo predeterminado **Domain Users** y, adicionalmente, al grupo de su departamento.  
En el ejemplo mostrado, el usuario **Ana Lopez** es miembro de:

- `Domain Users`
- `grupos_finanzas` (ubicado en `template.local/Departamentos`)[file:56]

Esto permite controlar el acceso a recursos basados en el departamento al que pertenece cada usuario.

### 5. Grupos para acceso a carpetas compartidas

En la OU `Recursos` → `Shares` se han creado grupos de seguridad específicos para controlar el acceso a cada carpeta compartida:

- `DL_carpeta_compartida_Finanzas`
- `DL_carpeta_compartida_RRHH`
- `DL_carpeta_compartida_Ventas`[file:58]

Cada uno de estos grupos tiene como miembros al grupo de departamento correspondiente.  
Por ejemplo:

- `DL_carpeta_compartida_Finanzas` incluye como miembro al grupo `grupos_finanzas`.[file:58]

De esta forma se aplica el modelo **AGDLP** (Accounts → Global Groups → Domain Local Groups → Permissions):  
las cuentas de usuario se añaden a grupos globales de departamento, y estos grupos globales se añaden a grupos de dominio locales que se usan para asignar permisos sobre las carpetas compartidas.

### 6. Uso previsto

- Asignar permisos NTFS y de recurso compartido sobre las carpetas del servidor de ficheros utilizando los grupos `DL_carpeta_compartida_*`.  
- Permitir que solo los usuarios del departamento correspondiente (Finanzas, RRHH, Ventas) accedan a su carpeta y a los datos protegidos por Veeam.

### 4.2. Servidor APP (.NET + SQL Server)

- Miembro del dominio `template.local`.
- Servicios:
  - Aplicación web desarrollada en .NET.
  - SQL Server con base de datos `reservas`.
- Flujo funcional:
  1. El usuario accede a la página web.
  2. Rellena un formulario de reserva.
  3. Los datos se guardan en la base de datos `reservas`.
  4. La aplicación envía un mensaje a RabbitMQ (cola) con la información del formulario.

Configuración de red:
- IP estática: `192.168.1.20`.
- DNS: servidor AD (`192.168.1.10`).

### 4.3. Servidor RabbitMQ (Ubuntu + Docker)

- Sistema operativo: Ubuntu Server 64 bits.
- RabbitMQ desplegado mediante Docker (Dockerfile y build propios).
- Función:
  - Recibir mensajes desde la aplicación .NET.
  - Gestionar colas de mensajes para procesamientos asíncronos.

Configuración de red:
- IP estática: `192.168.1.30`.
- Acceso típico:
  - Cliente RabbitMQ configurado en la app (.NET) con host `192.168.1.30` y puerto (por defecto 5672).

### 4.4. Servidor Veeam

- Miembro del dominio `template.local`.
- Software: Veeam Backup con agente instalado en el servidor AD.
- Escenario de backup:
  - Origen: servidor AD (en especial la carpeta `\\AD\shares`).
  - Destino: carpeta compartida `shares` (u otra ubicación configurada en Veeam).
- Objetivo:
  - Poder restaurar archivos o carpetas borradas por los usuarios.

Configuración de red:
- IP estática: `192.168.1.40`.
- DNS: servidor AD (`192.168.1.10`).

---

## 5. Flujo de funcionamiento del proyecto

1. **Infraestructura**  
   - El host ESXi está disponible en `https://192.168.1.139`.  
   - Las cuatro VMs se encuentran en el datastore `Datastore2`, organizadas en carpetas (`AD`, `APP`, etc.).

2. **Autenticación y DNS**  
   - Los equipos del dominio usan `192.168.1.10` como DNS.  
   - El AD resuelve los nombres `AD.template.local`, `APP.template.local`, `veeam.template.local`, etc.

3. **Aplicación y mensajería**  
   - La app .NET en `APP` guarda las reservas en SQL Server.  
   - Además, envía un mensaje a RabbitMQ en `RabbitMQ` con los datos de la reserva.

4. **Copias de seguridad**  
   - El agente de Veeam en el servidor AD realiza copias de seguridad en la carpeta compartida configurada.  
   - En caso de borrado de un fichero, se puede lanzar un restore desde el servidor `veeam`.

---

## 6. Procedimientos habituales

### 6.1. Puesta en marcha del proyecto

1. Encender el host físico y VMware Workstation.  
2. Arrancar la VM que contiene ESXi.  
3. Desde un navegador, entrar a `https://192.168.1.139` y encender las VMs en este orden recomendado:
   1. `AD`
   2. `APP`
   3. `RabbitMQ`
   4. `veeam`

### 6.2. Comprobaciones básicas

- Desde `APP`, hacer ping a `AD`, `RabbitMQ` y `veeam`.  
- Verificar que la resolución DNS de los nombres `*.template.local` funciona.  
- Acceder a la aplicación web y crear una reserva de prueba.  
- Verificar que:
  - El registro aparece en la base de datos `reservas`.
  - Se ha generado un mensaje en RabbitMQ (cola esperada).

### 6.3. Prueba de restauración con Veeam

1. En el servidor AD, eliminar un fichero de prueba de la carpeta `\\AD\shares`.  
2. Desde el servidor `veeam`, abrir la consola de Veeam.  
3. Seleccionar el job correspondiente al servidor AD.  
4. Iniciar un restore a nivel de fichero y recuperar el archivo borrado.  
5. Confirmar que el fichero vuelve a aparecer en la carpeta `shares`.

---

## 7. Notas y mejoras futuras

- Separar la red en varias VLAN (producción, backup, gestión) para practicar segmentación.  
- Añadir más jobs de Veeam (por ejemplo, backup de `APP` o de la base de datos).  
- Incluir un servidor de monitorización para tener métricas del entorno.

