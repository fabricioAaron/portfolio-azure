## Indice

1. [Migración de la aplicación .NET a Azure App Service](#migracin-de-la-aplicacin-net-a-azure-app-service)
2. [Publicación segura de la aplicación con Azure Application Gateway + WAF](#publicacin-segura-de-la-aplicacin-con-azure-application-gateway--waf)

## Migración de la aplicación .NET a Azure App Service

Tras migrar la base de datos a Azure SQL Database y la mensajería a Azure Service Bus, el siguiente paso ha sido mover la aplicación web ASP.NET Core al servicio PaaS **Azure App Service**

### Paso 1: Crear el App Service y el App Service Plan

En el portal de Azure:

1. Se crea un recurso **Web App** con estas opciones:
   - Subscription: `Azure for Students`.
   - Resource group: `lab-migration-onpremise`.
   - Name: `Ayacucho-Aventura`.
   - Publish: `Code`.
   - Runtime stack: `.NET 10 (LTS)`.
   - Operating system: `Windows`.
2. Se crea un **App Service Plan** asociado:
   - Name: `ASP-labmigrationonpremise-9657`.
   - Region: `Spain Central`.
   - SKU: `Basic`, tamaño `Small` (100 ACU, 1,75 GB RAM).
3. Se habilita **Application Insights** para monitorizar la aplicación.

El App Service queda disponible en una URL del tipo:  
`https://ayacucho-aventura-<id>.spaincentral-01.azurewebsites.net`.

<img width="602" height="755" alt="creamos un appservice" src="https://github.com/user-attachments/assets/8a0ab055-48da-486b-9c28-5a80ee9c96ea" />

`configuración del app service`

---

### Paso 2: Preparar la publicación desde Visual Studio

En Visual Studio:

1. Se selecciona el proyecto web ASP.NET Core (`MiWebAPP` o equivalente).
2. Menú **Build → Publish** (o botón *Publish* en el proyecto) y se elige como destino **Azure**.
3. Se selecciona **App Service** como tipo de destino y se inicia sesión con la cuenta de Azure usada en el portal.
4. En la lista de recursos se elige el App Service existente `Ayacucho-Aventura` dentro del grupo `lab-migration-onpremise`.

En el perfil de publicación se configuran:

- Configuration: `Release`.
- Target Framework: `net10.0`.
- Deployment Mode: `Framework-dependent`.
- Target Runtime: `Portable`.  
- Site: URL del App Service (copiada desde el portal).

En **Service Dependencies** se mantiene la cadena de conexión hacia la base de datos `db-reservas` en Azure SQL (connection string `DefaultConnection`).

<img width="875" height="585" alt="preparar visual para migrar app" src="https://github.com/user-attachments/assets/d8fe5ced-03c5-4ae4-8513-bbfab5850c55" />

`preparamos visual Studio para poder migrar a azure`

---

<img width="829" height="573" alt="preparar vsc para la migracion aputnando " src="https://github.com/user-attachments/assets/ef5b26cf-3acb-468c-8eb7-5dbd5366a420" />

`apuntamos a nuestra app service antes creada`

---

<img width="797" height="719" alt="preparado para la migra app" src="https://github.com/user-attachments/assets/0d9e39f3-7be0-4127-8270-edbaa733178b" />

`con esta configuración ya está listo para publicar y luego le daremos a publish`

---

### Paso 3: Publicar la aplicación en Azure App Service

Con el perfil configurado:

1. Se pulsa **Publish** en Visual Studio.
2. El proceso de publicación:
   - Compila la aplicación en modo Release.
   - Despliega los archivos al App Service.
   - Instala la extensión `Microsoft.AspNetCore.AzureAppServices.SiteExtension` si es necesario.
   - Reinicia la Web App al finalizar.
3. En la salida de Visual Studio se comprueba que el publish ha finalizado con éxito y se muestra la URL del sitio:  
   `https://ayacucho-aventura-<id>.spaincentral-01.azurewebsites.net/`.

<img width="1252" height="751" alt="appservice existoso" src="https://github.com/user-attachments/assets/b40f0bf6-08e7-4555-b451-45fd3705981c" />

`comprobamos que la extensión y se esta cargando y vemos que se realizó correctamente `

---

### Paso 4: Verificar la aplicación en Azure

Con la aplicación ya desplegada:

1. Se accede a la URL del App Service desde el navegador.  
2. Se abre la página principal y la sección **Hacer una reserva**, donde aparece el mismo formulario que en la versión on‑premise.
3. Se realiza una reserva de prueba (por ejemplo, nombre `migracion azure`, correo `migracion.azure@gmail.com`, tipo de aventura `Rafting Clásico`).
4. Se comprueba que:
   - El registro aparece en la tabla `Reservas` de la base de datos `db-reservas` en Azure SQL (consulta `SELECT * FROM Reservas;`).
   - Se genera un mensaje en la cola `servicebus` de Azure Service Bus con el texto de la reserva (visible en **Service Bus Explorer**), por ejemplo:  
     `Reserva creada: migracion azure - migracion.azure@gmail.com - 29/05/2026 10:20:00 AM - Clasico`.

Con estos pasos, la aplicación ha pasado de ejecutarse en un servidor Windows on‑premise (`APP.template.local`) a funcionar como **Azure App Service**, utilizando servicios gestionados de Azure para la base de datos y la mensajería.


<img width="1716" height="531" alt="app service subida" src="https://github.com/user-attachments/assets/2eef5086-530e-42ef-a859-7f76023ca701" />

`comprobamos que está publicado y subido correctamente`

---

<img width="1328" height="728" alt="app service en azure" src="https://github.com/user-attachments/assets/b97200e6-d63b-4614-a899-cf91cc28c93a" />

`volvemos hacer una comporbación que todo esta correcto`

---

<img width="1258" height="527" alt="test comrpobado" src="https://github.com/user-attachments/assets/3bbb677a-7902-44df-80da-71664b5d6090" />

`comprobamos que Azure Service Bus funciona correctamente`

---

<img width="1146" height="447" alt="testcomprobado2" src="https://github.com/user-attachments/assets/728510fd-573e-45c1-ae23-ac9e212c1b39" />

`comprobamos que Azure SQL funciona`

---

## Publicación segura de la aplicación con Azure Application Gateway + WAF

Para proteger la aplicación `Ayacucho-Aventura` desplegada en Azure App Service se ha colocado delante un **Azure Application Gateway** con Web Application Firewall (WAF).  
El Application Gateway actúa como punto de entrada HTTPS público, balanceador de carga y capa de seguridad (WAF y limitación de peticiones).

### 1. Configuración del backend hacia App Service

En el Application Gateway:

1. Se configura un **frontend** que es la ip pública que se monstrará a los usuarios. 
1. Se crea un **backend pool** con la URL del App Service como destino, por ejemplo:  
   `ayacucho-aventura-hbe3hha5vfqe0ed.spaincentral-01.azurewebsites.net`.[file:157][file:159]
2. Se define un **Backend setting** llamado `Settings-AppService`:
   - Protocolo: `HTTP`.
   - Puerto: `80`.
   - Tiempo de espera de petición: 20 segundos.[file:157]
3. En la sección **Backend health** se comprueba que el backend aparece como **Healthy**, indicando que el Gateway puede comunicarse correctamente con el App Service.[file:159]

<img width="1748" height="364" alt="Captura de pantalla 2026-05-12 193842" src="https://github.com/user-attachments/assets/0c3e0aef-0f6d-4a48-a4d7-a464d0bb5736" />

`configuración del app gateway`

---

<img width="841" height="814" alt="backend http" src="https://github.com/user-attachments/assets/84a92633-67c3-4a3a-bab8-9f7c62f99e73" />

`configuración del backend`

---

<img width="1525" height="418" alt="Captura de pantalla 2026-05-12 203812" src="https://github.com/user-attachments/assets/6319cb3e-ce73-4a5d-9be4-b5828760e954" />

`configuración del fronted con la ip pública`

---

### 2. Listener HTTP con certificado

Para exponer la aplicación de forma segura:

1. Se crea una **regla de enrutamiento** `Rule-HTTPS-Final` asociada a un listener HTTPS en el puerto 443.
2. Parámetros clave del listener:
   - Frontend IP: pública (IPv4).
   - Protocolo: `HTTP`.
   - Puerto: `80`.
3. El listener recibe las conexiones HTTP desde Internet y las enruta al backend pool que contiene el App Service utilizando el backend setting `Settings-AppService`.


<img width="841" height="814" alt="backend http" src="https://github.com/user-attachments/assets/e8388981-5175-41d7-8973-779744c1eb37" />

`configuración del backend`

---

### 3. WAF y regla de rate limiting

Se ha configurado una **Web Application Firewall Policy (WAF-PG)** asociada al Application Gateway:

1. La política WAF se crea con protección de bots habilitada.
2. Dentro de la política se define una **regla personalizada de tipo rate limit** llamada `DenyManyRequests`:
   - Action: `Deny traffic`.
   - Priority: `100`.
   - Rule type: `Rate limit`.
   - Rate limit duration: `1 minute`.
   - Threshold: `10` requests.
   - Group rate limit traffic by: `Client address`.
3. Se añade una condición de tipo **IP address** sobre la dirección remota (`Remote address`) con el rango `0.0.0.0/0`, lo que significa “cualquier IP del mundo”; de esta forma, la regla se aplica por igual a cualquier visitante.

Resultado: si un mismo cliente supera 10 peticiones por minuto, el WAF empieza a denegar el tráfico, mitigando ataques de fuerza bruta o abusos de la API/formulario.

<img width="851" height="270" alt="crear de un waf en el app gateway" src="https://github.com/user-attachments/assets/5500f6e5-1421-4551-870a-476bb68a8f05" />

`creamso el WAF par asociarlo a nuestro app gateway`

---

### Regla WAF de rate limiting y acceso desde Application Gateway

En la política WAF del Application Gateway se ha creado una regla personalizada para limitar peticiones por IP y se ha configurado el App Service para aceptar tráfico solo desde el propio Gateway.[file:165][file:164]

### Regla WAF de rate limit

- Nombre: `DenyManyRequests`
- Acción: `Deny traffic`
- Tipo: `Rate limit`
- Duración: `1 minute`
- Umbral: `10` peticiones por minuto
- Agrupación: por dirección del cliente (`Client address`)
- Condición:
  - Tipo: `IP address`
  - Variable: `Remote address`
  - Operación: `Does contain`
  - Rango: `0.0.0.0/0` (cualquier IP de Internet)

Con esto, cualquier IP que haga más de 10 peticiones en un minuto es bloqueada temporalmente por el WAF.

<img width="838" height="658" alt="dentro del waf que esta mi appgateway cremo un ratelimit" src="https://github.com/user-attachments/assets/26081dec-135a-4b83-991c-b9c1acc07d9f" />

`creamos una regla para evitar y bloquear ataques icmp`

---

<img width="586" height="402" alt="En el lenguaje de redessignifica Cualquier IP del mundo  Al poner esto, la regla se aplicará a todos los visitantes por igual" src="https://github.com/user-attachments/assets/bb2f21de-7980-4cf0-9e14-e41f435c1c44" />

`agregamos una condición`

---


### Regla de acceso en App Service

En el App Service se ha añadido una regla de acceso para permitir solo tráfico procedente del Application Gateway:

- Nombre: `Permitir-Gateway`
- Acción: `Allow`
- Prioridad: `100`
- Tipo de origen: `Service Tag`
- Service Tag: `GatewayManager`

Así, el App Service solo acepta tráfico que llega a través del Gateway (donde ya se aplica el WAF y el rate limit).
