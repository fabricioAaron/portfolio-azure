## Indice

1. [Migración de la aplicación .NET a Azure App Service](#migracion-de-la-aplicacion-net-a-azure-app-service)
2. [Publicación segura de la aplicación con Azure Application Gateway + WAF](#publicacion-segura-de-la-aplicacion-con-azure-application-gateway--waf)

## Migración de la aplicación .NET a Azure App Service

Tras migrar la base de datos a Azure SQL Database y la mensajería a Azure Service Bus, el siguiente paso ha sido mover la aplicación web ASP.NET Core al servicio PaaS **Azure App Service**.

### Paso 1: Crear el App Service y el App Service Plan

En el portal de Azure:

1. Se crea un recurso **Web App** con estas opciones:[file:147][file:153]
   - Subscription: `Azure for Students`.
   - Resource group: `lab-migration-onpremise`.
   - Name: `Ayacucho-Aventura`.
   - Publish: `Code`.
   - Runtime stack: `.NET 10 (LTS)`.
   - Operating system: `Windows`.
2. Se crea un **App Service Plan** asociado:
   - Name: `ASP-labmigrationonpremise-9657`.
   - Region: `Spain Central`.
   - SKU: `Basic`, tamaño `Small` (100 ACU, 1,75 GB RAM).[file:147]
3. Se habilita **Application Insights** para monitorizar la aplicación.[file:147]

El App Service queda disponible en una URL del tipo:  
`https://ayacucho-aventura-<id>.spaincentral-01.azurewebsites.net`.[file:153]

### Paso 2: Preparar la publicación desde Visual Studio

En Visual Studio:

1. Se selecciona el proyecto web ASP.NET Core (`MiWebAPP` o equivalente).[file:148]
2. Menú **Build → Publish** (o botón *Publish* en el proyecto) y se elige como destino **Azure**.[file:149]
3. Se selecciona **App Service** como tipo de destino y se inicia sesión con la cuenta de Azure usada en el portal.[file:149]
4. En la lista de recursos se elige el App Service existente `Ayacucho-Aventura` dentro del grupo `lab-migration-onpremise`.[file:150]

En el perfil de publicación se configuran:

- Configuration: `Release`.
- Target Framework: `net10.0`.
- Deployment Mode: `Framework-dependent`.
- Target Runtime: `Portable`.  
- Site: URL del App Service (copiada desde el portal).[file:151]

En **Service Dependencies** se mantiene la cadena de conexión hacia la base de datos `db-reservas` en Azure SQL (connection string `DefaultConnection`).[file:151]

### Paso 3: Publicar la aplicación en Azure App Service

Con el perfil configurado:

1. Se pulsa **Publish** en Visual Studio.[file:151]
2. El proceso de publicación:
   - Compila la aplicación en modo Release.
   - Despliega los archivos al App Service.
   - Instala la extensión `Microsoft.AspNetCore.AzureAppServices.SiteExtension` si es necesario.
   - Reinicia la Web App al finalizar.[file:151]
3. En la salida de Visual Studio se comprueba que el publish ha finalizado con éxito y se muestra la URL del sitio:  
   `https://ayacucho-aventura-<id>.spaincentral-01.azurewebsites.net/`.[file:151][file:152]

### Paso 4: Verificar la aplicación en Azure

Con la aplicación ya desplegada:

1. Se accede a la URL del App Service desde el navegador.  
2. Se abre la página principal y la sección **Hacer una reserva**, donde aparece el mismo formulario que en la versión on‑premise.[file:154]
3. Se realiza una reserva de prueba (por ejemplo, nombre `migracion azure`, correo `migracion.azure@gmail.com`, tipo de aventura `Rafting Clásico`).[file:154]
4. Se comprueba que:
   - El registro aparece en la tabla `Reservas` de la base de datos `db-reservas` en Azure SQL (consulta `SELECT * FROM Reservas;`).[file:146]
   - Se genera un mensaje en la cola `servicebus` de Azure Service Bus con el texto de la reserva (visible en **Service Bus Explorer**), por ejemplo:  
     `Reserva creada: migracion azure - migracion.azure@gmail.com - 29/05/2026 10:20:00 AM - Clasico`.[file:145][file:155]

Con estos pasos, la aplicación ha pasado de ejecutarse en un servidor Windows on‑premise (`APP.template.local`) a funcionar como **Azure App Service**, utilizando servicios gestionados de Azure para la base de datos y la mensajería.


## Publicación segura de la aplicación con Azure Application Gateway + WAF

Para proteger la aplicación `Ayacucho-Aventura` desplegada en Azure App Service se ha colocado delante un **Azure Application Gateway** con Web Application Firewall (WAF).  
El Application Gateway actúa como punto de entrada HTTPS público, balanceador de carga y capa de seguridad (WAF y limitación de peticiones).

### 1. Configuración del backend hacia App Service

En el Application Gateway:

1. Se crea un **backend pool** con la URL del App Service como destino, por ejemplo:  
   `ayacucho-aventura-hbe3hha5vfqe0ed.spaincentral-01.azurewebsites.net`.[file:157][file:159]
2. Se define un **Backend setting** llamado `Settings-AppService`:
   - Protocolo: `HTTP`.
   - Puerto: `80`.
   - Tiempo de espera de petición: 20 segundos.[file:157]
3. En la sección **Backend health** se comprueba que el backend aparece como **Healthy**, indicando que el Gateway puede comunicarse correctamente con el App Service.[file:159]

### 2. Listener HTTPS con certificado

Para exponer la aplicación de forma segura:

1. Se crea una **regla de enrutamiento** `Rule-HTTPS-Final` asociada a un listener HTTPS en el puerto 443.[file:156]
2. Parámetros clave del listener:
   - Frontend IP: pública (IPv4).
   - Protocolo: `HTTPS`.
   - Puerto: `443`.
   - Certificado: se sube un certificado PFX (por ejemplo `cert-ayacucho.pfx`) con su contraseña.[file:156]
3. El listener recibe las conexiones HTTPS desde Internet y las enruta al backend pool que contiene el App Service utilizando el backend setting `Settings-AppService`.

### 3. WAF y regla de rate limiting

Se ha configurado una **Web Application Firewall Policy (WAF-PG)** asociada al Application Gateway:[file:158]

1. La política WAF se crea con protección de bots habilitada.[file:158]
2. Dentro de la política se define una **regla personalizada de tipo rate limit** llamada `DenyManyRequests`:[file:162]
   - Action: `Deny traffic`.
   - Priority: `100`.
   - Rule type: `Rate limit`.
   - Rate limit duration: `1 minute`.
   - Threshold: `10` requests.
   - Group rate limit traffic by: `Client address`.
3. Se añade una condición de tipo **IP address** sobre la dirección remota (`Remote address`) con el rango `0.0.0.0/0`, lo que significa “cualquier IP del mundo”; de esta forma, la regla se aplica por igual a cualquier visitante.[file:163]

Resultado: si un mismo cliente supera 10 peticiones por minuto, el WAF empieza a denegar el tráfico, mitigando ataques de fuerza bruta o abusos de la API/formulario.

### 4. Restricción de acceso en el App Service (solo a través del Gateway)

Para que el App Service no sea accesible directamente desde Internet, se han configurado **Access Restrictions**:

1. En el App Service, en **Networking → Access Restrictions**, se selecciona `Enabled from select virtual networks and IP addresses` para el acceso público.[file:160]
2. Se crea una regla de tipo **Service Tag**:
   - Name: `Permitir-Gateway`.
   - Action: `Allow`.
   - Priority: `100`.
   - Type: `Service Tag`.
   - Service Tag: `GatewayManager` (conjunto de IPs usadas por Application Gateway para comunicarse con el backend).[file:161]
3. La idea es permitir solo el tráfico que venga del Application Gateway y denegar el resto de direcciones IP públicas.  
   De este modo, los usuarios acceden a la aplicación únicamente a través de la IP/URL del Application Gateway, donde se aplican el WAF y las reglas de seguridad.

En conjunto, esta configuración hace que la aplicación `Ayacucho-Aventura` esté:

- Publicada en HTTPS mediante Application Gateway.
- Protegida por WAF con reglas personalizadas (como limitación de peticiones).
- Accesible al App Service solo desde el Gateway, reforzando el modelo de seguridad en capas.
