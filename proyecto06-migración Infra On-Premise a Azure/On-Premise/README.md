# 🏗️  Infraestructura On-Premise

<img width="2816" height="1536" alt="Gemini_Generated_Image_8vwqht8vwqht8vwq (1)" src="https://github.com/user-attachments/assets/9f63fc77-5769-4ae8-b9bb-db3c1f96f40c" />

# Esquema 

| Nombre de la Máquina | Dirección IP     | Sistema Operativo        | Roles / Servicios Instalados                                                                 | Función en la Infraestructura                                      |
|----------------------|------------------|---------------------------|------------------------------------------------------------------------------------------------|---------------------------------------------------------------------|
| ESXi Hypervisor      | 192.168.1.139    | VMware ESXi (nested)     | Hipervisor, 2 datastores, red interna 192.168.1.0/24                                           | Plataforma de virtualización donde corren las 4 VMs                 |
| AD                   | 192.168.1.10     | Windows Server            | Active Directory Domain Services, DNS, Carpeta compartida `\\AD\shares`, Usuarios y grupos     | Controlador de dominio, DNS, recursos compartidos                   |
| APP                  | 192.168.1.20     | Windows Server            | Aplicación web .NET, SQL Server (BD reservas), integración con RabbitMQ                        | Servidor de aplicación y base de datos                              |
| RabbitMQ / Docker    | 192.168.1.30     | Ubuntu Server + Docker    | Docker Engine, Contenedor RabbitMQ (build propio con Docker  
| Veeam Backup Server  | 192.168.1.40     | Windows Server            | Veeam Backup & Replication, conexión a ESXi, agente Veeam en AD, repositorio local             | Backup de AD, shares, restauración granular, protección de datos    |

# Coste de la infraestructura: 

| Elemento                                           | Detalle (escenario empresa pequeña)                                   | Unidad / Cantidad    | Coste anual estimado (EUR) |
|----------------------------------------------------|------------------------------------------------------------------------|-----------------------|----------------------------|
| Licencia ESXi / vSphere                            | vSphere Essentials Kit (hasta 3 hosts) + soporte básico               | 1 kit                 | 600–900 €                  |
| Licencias Windows Server (3 VMs)                   | 3 × Windows Server Standard (16 cores) amortizadas + soporte          | 3 servidores          | 900–1.500 €                |
| Licencias de acceso de usuario (CAL AD)            | Windows Server CAL para usuarios que acceden a AD, shares, etc.       | 25 CAL de usuario     | 300–600 €                  |
| SQL Server 2022 Standard                           | Licencia por 4 cores + soporte, amortizada a varios años              | 1 instancia (4 cores) | 800–1.200 €                |
| Ubuntu Server + RabbitMQ                           | Software libre; sin soporte de pago                                   | 1 VM                  | 0 €                        |
| Licencia de Veeam Backup                           | Veeam Essentials / per‑instance para hasta 6–10 cargas                | 1 licencia            | 700–1.000 €                |
| Almacenamiento para datastores ESXi                | 300 GB útiles en cabina/NAS (parte proporcional de un equipo de 1–2 TB) | 300 GB                | 50–150 €                   |
| Almacenamiento para backups (repositorio Veeam)    | Espacio en NAS/SAN para copias de la carpeta compartida y VMs         | 1 TB reservado        | 100–200 €                  |
| **TOTAL ANUAL APROXIMADO**                         | Software (ESXi, Windows, SQL, Veeam, CALs) + almacenamiento            | —                     | **3.450–5.550 € / año**    |


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

`configuración de nuestro ESXi `

---

**Red**

- Rango de red: `192.168.1.0/24`.
- vSwitch: `vSwitch0`.
- Port group: `Lan-Servers` (VLAN ID 0).
- Todas las VMs están conectadas al port group `Lan-Servers`.

<img width="802" height="541" alt="2" src="https://github.com/user-attachments/assets/ba49a047-9091-4190-b38e-1d91aeec571f" />

`configuración de nuestro Lan-Server`

---

<img width="1226" height="269" alt="3" src="https://github.com/user-attachments/assets/c4b956e7-4637-44ed-bc2b-e32050005fdc" />

`nuestras MV con sus SO y su hosts`

---

## Servidor de Directorio Activo, DNS y Carpeta compartida

<img width="1538" height="581" alt="4" src="https://github.com/user-attachments/assets/c22bdcec-0316-4d3c-95b2-5529be8126f6" />

`el Dashboard de nuestro servidor AD, podemos ver métricas,cpu`

---

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

`se crea una UO y se crea los diferentes grupos  que se le asignará a los usuarios`

---

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

`comprobamos que cada usurios son miembros de: `

---

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

`creamos la carpeta compartida cada uno con sus permisos` 

---

De esta forma se aplica el modelo **AGDLP** (Accounts → Global Groups → Domain Local Groups → Permissions):  
las cuentas de usuario se añaden a grupos globales de departamento, y estos grupos globales se añaden a grupos de dominio locales que se usan para asignar permisos sobre las carpetas compartidas.

<img width="762" height="288" alt="8" src="https://github.com/user-attachments/assets/bd0ba32f-70cb-473b-a675-cb8df6fa0778" />

`creamos domain local groups que se asignará al grupo correspondiente`

---

### 6. Uso previsto

- Asignar permisos NTFS y de recurso compartido sobre las carpetas del servidor de ficheros utilizando los grupos `DL_carpeta_compartida_*`.  
- Permitir que solo los usuarios del departamento correspondiente (Finanzas, RRHH, Ventas) accedan a su carpeta y a los datos protegidos por Veeam.

## Servidor de Aplicaciones (.NET + SQL Server)

### 1. Información general

- Nombre del servidor: `APP.template.local`
- Rol: Servidor web y de base de datos para la aplicación de reservas.
- Tecnologías:
  - ASP.NET Core MVC (.NET 10.0 LTS) para la aplicación web.
  - SQL Server para el almacenamiento de datos de las reservas.
- Dirección web (entorno de pruebas): `https://localhost:7092` (publicada en el propio servidor APP).

<img width="1476" height="610" alt="10" src="https://github.com/user-attachments/assets/95ae5da9-0b34-484a-8bab-53a39e4dbfac" />

`comprobación del funcionamiento de nuestra página web`

---

### 2. Aplicación web ASP.NET Core

La aplicación se ha creado como **ASP.NET Core Web App (Model–View–Controller)** con las siguientes opciones: 

- Framework: `.NET 10.0 (Long Term Support)`.
- Tipo de autenticación: `None` (aplicación pública sin login integrado).
- HTTPS habilitado: opción **Configure for HTTPS** activada.
- Sin soporte de contenedores (se ejecuta directamente en IIS/Kestrel dentro de Windows Server).

La aplicación simula un sitio de reservas de actividades de aventura (“Ayacucho Aventura”) con varias secciones informativas en el menú (Experiencias, Por qué elegirnos, Seguridad, Opiniones) y un botón principal de llamada a la acción **Reservar mi aventura** en la página de inicio.

<img width="888" height="544" alt="9" src="https://github.com/user-attachments/assets/c0a89bf9-9e1d-4604-b4f3-f06fb18d1494" />

`configuración de nuestro .net`

---

### 3. Formulario de reserva

En la sección de reservas se muestra un formulario donde el usuario introduce:

- **Nombre**.
- **Email de contacto**.
- **Fecha y hora de la reserva**.
- **Tipo de aventura** (por ejemplo: “Rafting Clásico”, “Extreme”, “Combo”).


<img width="1672" height="609" alt="11" src="https://github.com/user-attachments/assets/34a8af6b-3d2d-4078-9eed-85d1ec38d9c0" />

`se crea un formulario cuando se envie se almacene en la base de datos de SQL server`

---

Al pulsar el botón **Enviar reserva**:

1. El controlador MVC recibe los datos del formulario.
2. Se valida la información básica.
3. Se crea un nuevo registro en la base de datos SQL Server en la tabla `Reservas`.
4. Opcionalmente, la aplicación envía un mensaje a RabbitMQ (según la configuración del servicio de mensajería del laboratorio).


### 4. Base de datos SQL Server

La información de las reservas se almacena en SQL Server en una tabla llamada `Reservas`.

Estructura lógica de la tabla:

- `Id` (clave primaria, entero).
- `Nombre` (texto).
- `Email` (texto).
- `FechaReserva` (datetime).
- `TipoAventura` (texto).

Ejemplo de consulta:

```sql
SELECT * FROM Reservas;
```

<img width="683" height="304" alt="12" src="https://github.com/user-attachments/assets/0224fe67-9097-4fb2-910e-224f32ac072d" />

`El resultado de esta consulta muestra los registros enviados desde el formulario web, con sus campos de nombre, correo, fecha y tipo de aventura.`

---
### 5. Panel de control de reservas (CRUD)

La aplicación dispone de un **Panel de reservas** accesible desde el menú principal.

Funcionalidad del panel:

- Listado de todas las reservas almacenadas en la tabla `Reservas`.
- Columnas visibles:
  - Nombre
  - Email
  - Fecha
  - Aventura
- Acciones disponibles por cada registro:
  - **Editar**: permite modificar los datos de una reserva existente.
  - **Eliminar**: permite borrar una reserva de la base de datos.

El panel implementa un CRUD completo (Create, Read, Update, Delete):

- **Create**: altas mediante el formulario público de reservas.
- **Read**: visualización en el panel de reservas.
- **Update**: edición de una reserva existente desde el panel.
- **Delete**: eliminación de reservas desde el panel con la acción correspondiente.

<img width="1231" height="700" alt="13-CRUD" src="https://github.com/user-attachments/assets/de3f9400-81f1-4202-b1d0-a3d217905d8d" />

`panle de reservas donde podemos editar o eliminar y se eliminará o editará en nuestra base de datos`

---

### 6. Flujo de funcionamiento del servidor APP

1. El usuario accede a la página principal y revisa la información de la empresa de aventura.
2. Navega a la sección de reservas y rellena el formulario con sus datos.
3. Al enviar el formulario:
   - La aplicación guarda la reserva en la base de datos `Reservas`.
   - La reserva pasa a formar parte del listado del panel de administración.
4. Desde el **Panel de reservas**, el administrador puede:
   - Ver todas las reservas realizadas.
   - Editar o eliminar reservas según sea necesario.


## Servidor de Mensajería (Ubuntu + RabbitMQ)

### Resumen

- Servidor: Ubuntu Server (VM `RabbitMQ`).
- Rol: cola de mensajes para las reservas realizadas en la web.
- Despliegue: contenedor Docker con imagen `rabbitmq:3-management` (incluye consola web).
- Puertos típicos:
  - 5672/TCP: tráfico AMQP de la aplicación.
  - 15672/TCP: consola de administración web de RabbitMQ.


<img width="1384" height="179" alt="14 instalar docker rabbit mq " src="https://github.com/user-attachments/assets/5639d863-6d6f-4255-ae01-f264f6737678" />

`configuración en docker con la imagen de rabbitmq`

---

<img width="351" height="483" alt="16-instalar-rabbitcliente" src="https://github.com/user-attachments/assets/89b5d67a-94e1-4d52-9539-b9245bf59fe8" />

`instalación en .net de rabbitmq`

---

### Configuración de RabbitMQ

En la consola de RabbitMQ se ha creado:

- **Exchange**: `test_exchange`, de tipo `direct`, durable.
- **Routing key**: `test_rk`
- **Cola**: `test_queue`, enlazada al exchange mediante la routing key.

La aplicación .NET del servidor APP se conecta a RabbitMQ usando estos parámetros (host = VM de Ubuntu, exchange `test_exchange`, routing key `test_rk`) y publica un mensaje cada vez que se crea una reserva.


<img width="692" height="550" alt="17 5 creacion exchange rabbitmq" src="https://github.com/user-attachments/assets/befbbc62-3952-45e1-b090-b8974bd4103a" />

`configuración de exchange`

---

<img width="1136" height="585" alt="20 anidar rabbit mq " src="https://github.com/user-attachments/assets/48c1e58e-2593-4655-bdce-f2cf993f7064" />

`anidamos apuntando en el .net hacia rabbitmq`

---

### Flujo completo de una reserva

1. El usuario rellena el formulario en la web de APP y pulsa **Enviar reserva**.
2. El servidor APP:
   - Guarda la reserva en SQL Server (tabla `Reservas`), que luego se ve en el Panel de reservas.
   - Envía un mensaje a RabbitMQ con un texto tipo:  
     `Reserva creada: <nombre> - <email> - <fecha> - <tipo aventura>`.
3. En la consola de RabbitMQ, en la cola `test_queue`, se puede ver cada mensaje en la pestaña **Queues and Streams**, incluido el payload con los datos de la reserva. 

De esta forma, cada reserva queda registrada en dos sitios:
- En la base de datos SQL para gestión desde el panel.
- En RabbitMQ como mensaje en cola, listo para ser procesado por otros servicios si se amplía el laboratorio.


<img width="900" height="701" alt="17 1" src="https://github.com/user-attachments/assets/be5fde52-ea25-451d-9beb-53835511ac63" />

`las colas de los mensajes`

---

## Servidor de Copias de Seguridad (Veeam)

El servidor `veeam.template.local` es el punto central de backup del laboratorio.  
Desde aquí se protegen los datos del controlador de dominio `AD.template.local`, en concreto la carpeta compartida `C:\shares` donde los departamentos guardan sus ficheros. 

### 1. Incorporación de ESXi y del servidor Veeam a la infraestructura de backup

En la consola de Veeam se ha configurado primero la infraestructura básica:

1. En el menú **Backup Infrastructure → Managed Servers** se ha añadido el host VMware ESXi. 
2. Al pulsar **Add Server → Virtualization Platforms → VMware vSphere**, se indica:
   - La IP del host ESXi: `192.168.1.139`. 
   - Las credenciales de administrador (`root` + contraseña del ESXi). 
3. Veeam se conecta al ESXi y recopila la información de discos y máquinas virtuales. 

En el mismo panel se ve también el propio servidor `veeam.template.local` como servidor Windows gestionado, que actúa como servidor de backup. 

<img width="626" height="486" alt="23 conifg-que tivo de servidor" src="https://github.com/user-attachments/assets/f32981aa-6ae5-4080-bf98-2894932299bb" />

`se agrega un servidor`

---

<img width="650" height="241" alt="24 elegimos nuestro servidor " src="https://github.com/user-attachments/assets/7201b6cd-ab36-41af-acdc-1dacf260ad60" />

`elegimos vSphere > ESXi `

---


<img width="803" height="337" alt="25 ponemos la ip de nuestro esxi" src="https://github.com/user-attachments/assets/f31776fc-180c-469d-b93b-28cd1bdd5ccb" />

`apuntamos a nuestra ip de nuestro esxi`

---

<img width="806" height="463" alt="26 credenciales esxi" src="https://github.com/user-attachments/assets/e14c5116-017d-4567-a654-39e19f4430e4" />

`agregamos las credenciales de nuestro esxi para poder acceder y ver las mv que tenemos alogados en nuestro cluster`

---

<img width="1025" height="283" alt="29 comprobacion2 de our mv" src="https://github.com/user-attachments/assets/e066c6e4-d93f-49a4-a3fc-65d6dc6c1ee4" />

`comprobamos que funcionó correctamente`

---

Nota: como tenemos la licencia gratuita de veeam no podemos hacer backup de nuestras mv que está dentro de nuestro ESXi, así agregaremos un agente en nuestro servidor de AD.template.local

### 2. Creación del Protection Group para el servidor AD

Para instalar y gestionar el agente de backup en `AD.template.local` se crea un Protection Group:

1. En **Inventory → Physical and Cloud Infrastructure** se selecciona **Create Protection Group**.
2. En la pestaña **Computers** se añade el equipo con:
   - Dirección: `192.168.1.10` (servidor AD).
   - Cuenta usada: `administrator@template.local`. 
3. En **Options** se indica que el servidor de distribución es `veeam.template.local`, se marca **Install backup agent** y se habilita la actualización automática de componentes. 
4. Al aplicar la configuración, Veeam despliega el agente en el servidor AD y se comprueba que la instalación y detección del servidor han sido correctas. 


<img width="530" height="394" alt="34 licencia vmware" src="https://github.com/user-attachments/assets/a4fe0311-f6ff-4165-b41d-fa59a17164f8" />

`creamos en proteccion group`

---

<img width="765" height="208" alt="35 agresar AD" src="https://github.com/user-attachments/assets/b1ac5ef7-8efa-461e-b93e-4e0801978c55" />

`apuntamos a la ip de nuestro servidor de AD`

---

<img width="873" height="454" alt="36 instalar agent VM" src="https://github.com/user-attachments/assets/8cdcf0f6-283d-4a4f-9fbc-f6346da5b77b" />

`programamos que el backup sea dirario a alas 21 e instalamos el agente`

---

<img width="891" height="576" alt="37 agente instalado" src="https://github.com/user-attachments/assets/60489fb9-4b06-434b-a26f-ef0e04ee13f7" />

`aplicamos los cambios`

---

### 3. Creación del trabajo de backup del AD (backup a nivel de archivos)

Una vez instalado el agente, se crea un **Agent Backup Job** específico para el servidor AD:

1. En el inventario, dentro del Protection Group del AD, se lanza la creación de un nuevo trabajo de agente. 
2. En la pestaña **Name** se asigna un nombre descriptivo al trabajo, por ejemplo `Backup  
3. En **Computers** se selecciona `AD.template.local` como máquina protegida. 
4. En **Backup Mode** se elige la opción **  
5. En **Objects** se define qué se va a copiar:
   - Se añade la carpeta `C:\shares` del servidor AD, que es donde están las carpetas de Finanzas, RRHH y Ventas. 
6. En **Storage** se selecciona el repositorio `Default Backup Repository (Created by Veeam Backup)` y se establece una retención de, por ejemplo, 5 días. 
7. En **Schedule** se configura que el trabajo se ejecute automáticamente todos los días a las 22:00, con reintentos en caso de errores. 

Tras guardar el trabajo, se lanza una primera ejecución manual para generar el primer punto de restauración. 


<img width="1161" height="524" alt="38 config del agente mv " src="https://github.com/user-attachments/assets/b6c95847-991a-44a6-995b-70a8107706e7" />

`creamos un job`

---

<img width="927" height="415" alt="40 backup carpeta compatida" src="https://github.com/user-attachments/assets/60b62213-4062-4e09-8b45-00fca324696c" />

`elegimos el modo, en este caso file level backup, para restaurar una carpeta compartida`

---

<img width="919" height="416" alt="41 carpeta compartida de AD" src="https://github.com/user-attachments/assets/01fcaf79-588f-40fd-9616-8c84f8ba7454" />

`apuntamos a la carpeta compartida que tenemos en nuestro servidor de AD`

---

<img width="921" height="461" alt="42 donde se alamcena los bakcup " src="https://github.com/user-attachments/assets/e0f74d62-ceae-4ea4-9569-48f6e923527f" />

`en donde almacenamos nuestros backup y el periodo de retencion`

---

<img width="918" height="396" alt="43 " src="https://github.com/user-attachments/assets/e437cca9-b1e1-4780-b978-f13af9b1e40d" />

`hacemos el backup diario,  y reitentará 3 veces cada 10 minutos en caso de que falle` 

---


### 4. Ejecución y verificación del backup

Cuando se inicia el trabajo `Backup  

- En la vista de trabajos se ve el progreso del backup con el objeto `AD.template.local`. 
- Una vez completado, el estado cambia a **Success** y se muestran los datos procesados de la carpeta `C:\shares`. 
- El repositorio de backup almacena el punto de restauración asociado a la fecha y hora de la ejecución. 


<img width="914" height="459" alt="45 backup hecho" src="https://github.com/user-attachments/assets/1868f169-514d-4646-b17e-183055cca56d" />

`ejecutamos el backup`

---

### 5. Restauración de archivos desde la carpeta compartida

Para comprobar que la protección funciona, se realiza una restauración a nivel de archivos:

1. En la consola de Veeam, en la sección de backups, se selecciona el trabajo `Backup  
2. Se abre el **Backup Browser** y se navega por la estructura del servidor `AD.template.local` hasta `C:\shares`. 
3. Dentro de `C:\shares` se accede a las subcarpetas de departamento (por ejemplo `Finanzas → Costos-2026`) y se visualizan los archivos de prueba (`costos_2026.txt`, `costos_2026.xlsx`). 
4. Desde este navegador se pueden restaurar archivos o carpetas concretas al servidor original o a otra ubicación, permitiendo recuperar datos borrados por los usuarios.

Con esta configuración, la carpeta compartida principal del dominio (`C:\shares` en el servidor AD) queda protegida mediante copias diarias y se dispone de un procedimiento sencillo para restaurar ficheros críticos en caso de eliminación o modificación accidental. 


<img width="1169" height="291" alt="46 boton de restaturar" src="https://github.com/user-attachments/assets/f27f2510-9919-4835-99f5-d51005896e64" />

`restauración del backup > iremos a restore Guest file `

---

<img width="1006" height="410" alt="47 archivos para restaurar" src="https://github.com/user-attachments/assets/8d139ed8-f930-4836-ba76-b846ce1d134a" />

`comprobamos que ya tenemos el backup de nuestro servidor en el que podemos restaurar`

---

