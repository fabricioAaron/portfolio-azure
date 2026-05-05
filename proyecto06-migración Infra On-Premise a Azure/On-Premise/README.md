# 🏗️  Infraestructura On-Premise

<img width="2816" height="1536" alt="Gemini_Generated_Image_x48wqux48wqux48w" src="https://github.com/user-attachments/assets/fb4d57d2-5800-42dd-bafe-6537e05fa1e3" />


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

<img width="1538" height="581" alt="4" src="https://github.com/user-attachments/assets/c22bdcec-0316-4d3c-95b2-5529be8126f6" />

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

La OU **Departamentos** agrupa los grupos de seguridad por área de la empresa.
La OU **Usuarios** contiene las cuentas de usuario finales que se usan en el laboratorio.

<img width="714" height="282" alt="6" src="https://github.com/user-attachments/assets/847833bf-daaf-49cd-860a-0e2f850d6c97" />

### 3. Grupos de seguridad por departamento

Dentro de la OU `Departamentos` se han creado los siguientes grupos de seguridad de tipo global:

- `grupos_finanzas`
- `grupos_RRHH`
- `grupos_ventas`

Estos grupos representan a los departamentos de la organización y se utilizan para asignar permisos de acceso a recursos compartidos.

### 4. Usuarios de dominio

En la OU `Usuarios` se han creado usuarios de ejemplo, por ejemplo:

- `Ana Lopez`
- `Carlos Ruiz`
- `Juan Peres`
- `Maria Gomez`
<img width="801" height="377" alt="7" src="https://github.com/user-attachments/assets/a3ec04d7-16ed-482e-92e1-a5338791950b" />

Cada usuario pertenece al grupo predeterminado **Domain Users** y, adicionalmente, al grupo de su departamento.  
En el ejemplo mostrado, el usuario **Ana Lopez** es miembro de:

- `Domain Users`
- `grupos_finanzas` (ubicado en `template.local/Departamentos`)

Esto permite controlar el acceso a recursos basados en el departamento al que pertenece cada usuario.

### 5. Grupos para acceso a carpetas compartidas

En la OU `Recursos` → `Shares` se han creado grupos de seguridad específicos para controlar el acceso a cada carpeta compartida:

- `DL_carpeta_compartida_Finanzas`
- `DL_carpeta_compartida_RRHH`
- `DL_carpeta_compartida_Ventas`

Cada uno de estos grupos tiene como miembros al grupo de departamento correspondiente.  
Por ejemplo:

- `DL_carpeta_compartida_Finanzas` incluye como miembro al grupo `grupos_finanzas`.

<img width="360" height="310" alt="8 5" src="https://github.com/user-attachments/assets/bc783399-f932-4c35-ab95-ece9f9eec027" />


De esta forma se aplica el modelo **AGDLP** (Accounts → Global Groups → Domain Local Groups → Permissions):  
las cuentas de usuario se añaden a grupos globales de departamento, y estos grupos globales se añaden a grupos de dominio locales que se usan para asignar permisos sobre las carpetas compartidas.

<img width="762" height="288" alt="8" src="https://github.com/user-attachments/assets/bd0ba32f-70cb-473b-a675-cb8df6fa0778" />

### 6. Uso previsto

- Asignar permisos NTFS y de recurso compartido sobre las carpetas del servidor de ficheros utilizando los grupos `DL_carpeta_compartida_*`.  
- Permitir que solo los usuarios del departamento correspondiente (Finanzas, RRHH, Ventas) accedan a su carpeta y a los datos protegidos por Veeam.

## Servidor de Aplicaciones (.NET + SQL Server)

- Servidor: `APP.template.local`
- Rol: web de reservas y base de datos.
- Tecnologías: ASP.NET Core MVC (.NET 10.0 LTS) y SQL Server.
- URL de pruebas: `https://localhost:7092`.

<img width="888" height="544" alt="9" src="https://github.com/user-attachments/assets/42f6bd7c-a32f-4560-9249-7fb190ceb040" />

### Funcionamiento de la web

La web muestra una página principal de la empresa de aventura y un **formulario de reserva** con nombre, email, fecha y tipo de aventura.  
Cuando el usuario envía el formulario, la aplicación crea un registro en la tabla `Reservas` de SQL Server con esos datos.

<img width="1476" height="610" alt="10" src="https://github.com/user-attachments/assets/b5478dce-655e-49b3-88b0-fd9d7ceedfe6" />

<img width="1672" height="609" alt="11" src="https://github.com/user-attachments/assets/a6c8b95f-4128-44a5-b475-26f414888510" />

### Base de datos de reservas

Tabla principal: `Reservas`.

Campos principales:
- `Id`
- `Nombre`
- `Email`
- `FechaReserva`
- `TipoAventura`

La consulta `SELECT * FROM Reservas` muestra todas las reservas guardadas.

<img width="683" height="304" alt="12" src="https://github.com/user-attachments/assets/e1a13c31-08c3-4580-9ea1-fcfed93fb214" />

### Panel de reservas (CRUD)

La aplicación incluye un **Panel de reservas** solo para administración.  
En ese panel se muestra una tabla con todas las reservas y, para cada una, dos acciones:

- **Editar**: modificar los datos de una reserva.
- **Eliminar**: borrar una reserva.

En resumen, el servidor APP recibe las reservas desde el formulario, las guarda en SQL Server y permite gestionarlas (ver, editar y eliminar) desde el panel de control.

<img width="1231" height="700" alt="13-CRUD" src="https://github.com/user-attachments/assets/a4ca9d96-0c18-4868-9fb5-31160cfb0f16" />

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

