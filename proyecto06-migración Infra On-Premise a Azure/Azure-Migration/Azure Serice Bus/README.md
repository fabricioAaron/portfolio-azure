## Migración de RabbitMQ on‑premise a Azure Service Bus

En la infraestructura original, la aplicación .NET enviaba un mensaje a RabbitMQ (Ubuntu + Docker) cada vez que se creaba una reserva.  
En la versión en Azure, esta mensajería se sustituye por **Azure Service Bus**, manteniendo la misma lógica de negocio: cada reserva genera un mensaje en una cola de Azure.

### Paso 1: Crear el namespace de Azure Service Bus

En el portal de Azure:

1. Se crea un recurso **Service Bus Namespace** con los siguientes parámetros:
   - Subscription: `Azure for Students` (en este laboratorio).
   - Resource group: `lab-migration-onpremise`.
   - Namespace name: `cola-reservas` (host: `cola-reservas.servicebus.windows.net`).
   - Location: `Spain Central`.
   - Pricing tier: `Basic` (~0,05 USD por 1 millón de operaciones al mes), suficiente para pruebas.
2. Una vez aprovisionado, en la vista general del namespace se comprueba que el estado es **Active**.

<img width="849" height="591" alt="service bus" src="https://github.com/user-attachments/assets/7c279183-507f-4549-9102-e19aca98ebaa" />

`creación del namespace`

---

### Paso 2: Crear la cola de reservas

Dentro del namespace `cola-reservas`:

1. En el apartado **Entities → Queues** se crea una cola llamada `servicebus`.
2. La cola queda visible en la lista de colas del namespace, con estado **Active** y conteo de mensajes inicial en 0.

Esta cola será el destino de los mensajes de reserva enviados por la aplicación .NET.

### Paso 3: Configurar las credenciales de acceso (SAS)

Para que la aplicación pueda enviar mensajes a la cola se usa una política de acceso compartido (SAS):

1. En el namespace `cola-reservas` se accede a **Shared access policies**.
2. Se crea una política llamada, por ejemplo, `politica`, con permisos:
   - **Manage**
   - **Send**
   - **Listen**.
3. La política genera:
   - `Primary key` y `Secondary key`.
   - `Primary connection string` y `Secondary connection string`.

<img width="979" height="319" alt="crear la cola " src="https://github.com/user-attachments/assets/b1837faf-8812-410a-b261-7217ab02f901" />

`creamos una cola y una política para ponder en connection string `

---

<img width="569" height="633" alt="cola connection string" src="https://github.com/user-attachments/assets/b19d9f5a-2dc9-4539-9807-9ca74270488d" />

`usamos el connection string para vincularlos a nuestro .net y se trasnfiera a azure`

---

La **primary connection string** de esta política es la que se utiliza en la configuración de la aplicación .NET para conectarse a Azure Service Bus.

### Paso 4: Actualizar la aplicación .NET para usar Service Bus

En la aplicación .NET (antes apuntando a RabbitMQ):

1. Se sustituyen las librerías de cliente de RabbitMQ por las de Azure Service Bus (por ejemplo, paquete `Azure.Messaging.ServiceBus`).
2. En el archivo de configuración (por ejemplo `appsettings.json`) se añade la cadena de conexión obtenida de la política `politica` de Service Bus.
3. El código que antes publicaba mensajes en RabbitMQ se adapta para:
   - Crear un `ServiceBusClient` usando la connection string.
   - Crear un `ServiceBusSender` asociado a la cola `servicebus`.
   - Enviar un mensaje con el texto de la reserva, por ejemplo:  
     `Reserva creada: <Nombre> - <Email> - <Fecha> - <TipoAventura>`.
4. La aplicación se despliega y se prueba desde el formulario de reservas (ya sea on‑premise o en Azure App Service según la fase de migración).

<img width="968" height="589" alt="imagen" src="https://github.com/user-attachments/assets/be7211a4-6872-4f78-bd15-6d23254e179f" />

`apuntamos al Service Bus en el .net`

---

<img width="1341" height="809" alt="confi service bus " src="https://github.com/user-attachments/assets/05fe3ce0-ce25-4c6d-960a-eb3e6244e20c" />

`dashboard`

---

### Paso 5: Verificar que los mensajes llegan a la cola

Con el nuevo flujo en marcha:

1. Se realiza una reserva desde la web (por ejemplo con el nombre “Patricio Alberto Sánchez Paredes”).
2. La reserva se guarda en la base de datos `db-reservas` en Azure SQL (se puede comprobar con un `SELECT * FROM Reservas;`).
3. En el portal de Azure, dentro del namespace `cola-reservas`, se abre la cola `servicebus` y el apartado **Service Bus Explorer**.
4. En modo **Receive mode → Peek** se visualizan los mensajes activos de la cola.  
   En el **Message Body** se ve el texto generado por la aplicación, por ejemplo:  
   `Reserva creada: Patricio Alberto Sánchez Paredes - patricoleon@gmail.com - 29/05/2026 15:51:00 - Extreme`.

<img width="1013" height="461" alt="test migrato todo - appservice" src="https://github.com/user-attachments/assets/2e02636a-9719-4e9d-b622-43cb4f7b629b" />

`en local enviamos un formulario para comprobar el funcionamiento`

---

<img width="1138" height="487" alt="sql testeado " src="https://github.com/user-attachments/assets/a84af5c5-5684-4b9b-9f8c-a14685cc4a30" />

`comprobamos que en Azure sql server funciona correctamente`

---

<img width="1457" height="579" alt="service bus comprobado" src="https://github.com/user-attachments/assets/94a0e7fc-2ad5-4754-9f11-4dc538ce516e" />

`comprobación que en service bus, funciona correctamente`

---

Con esto se confirma que:

- La aplicación .NET sigue funcionando con el mismo formulario de reservas.
- Los datos se almacenan en Azure SQL Database.
- Cada reserva genera un mensaje en la cola de Azure Service Bus en lugar de RabbitMQ.

El patrón de arquitectura se mantiene: la web escribe la reserva en la base de datos y envía un evento a una cola, pero ahora toda la parte de mensajería está gestionada como servicio PaaS en Azure. 
