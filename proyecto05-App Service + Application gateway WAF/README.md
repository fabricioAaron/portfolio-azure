# Laboratorio:  Application Gateway WAF 

El objetivo de este despliegue es demostrar la capacidad de proteger aplicaciones críticas en la nube. No basta con subir una web; 
es necesario asegurar que sea resistente a ataques (WAF), que su estado de salud sea monitoreado constantemente.

---

## Paso 1. Backend: Azure App Service (CV)

**Objetivo:** Hospedar tu página `index.html` en una plataforma gestionada (PaaS).

1. En el portal de Azure:
   - `Crear un recurso` → **Web App**.
   - Suscripción: `Azure for Students`.
   - Resource group: `RG_App` (o el que uses para el laboratorio).
   - Name: `cv-webb` .
   - Publish: **Code**.
   - Runtime stack: por ejemplo **.Net**
   - OS: Windows.
   - Region: `Norway East` 
   - Plan: el más pequeño disponible (Free/Basic).

<br>

   <img width="558" height="769" alt="1" src="https://github.com/user-attachments/assets/f171e704-5ad6-458a-bcdd-52b483d0930f" />

<br>

<img width="921" height="496" alt="Captura de pantalla 2026-04-05 042149" src="https://github.com/user-attachments/assets/d20353a2-6362-4625-b684-cfc6ecf767f2" />


<br>

2. Subir el `index.html`:
   - Ir al Web App → **Development ** → `filex` → `.zip`.
    - Confirmar que la URL  
     `https://cv-app-fabri-XXXX.azurewebsites.net/`  
     muestra tu CV.

<br>

<img width="1600" height="784" alt="2" src="https://github.com/user-attachments/assets/3a554b34-c369-49a4-a107-6ef2654e48ff" />

<br>

- Pantalla de **Overview** del Web App, mostrando:
  - `Default domain` (`cv-app-fabri-XXXX.azurewebsites.net`).
  - Estado `Healthy`.
  - Runtime stack.


---

## Paso 2. Capa 7: Azure Application Gateway (reverse proxy)

**Objetivo:** Tener un único punto de entrada HTTP/HTTPS que entienda el tráfico y lo envíe al App Service.

### 2.1. Red (VNet y subred)

1. `Crear un recurso` → **Virtual network**.
   - Name: `app-gateway-web`.
   - Region: misma que el App Service.
   - Address space: `10.10.0.0/16`.

2. Subredes:
   - `subnet-appgw` → `10.10.0.0/24` (reservada solo para Application Gateway).

### 2.2. Crear Application Gateway

1. `Crear un recurso` → **Application Gateway**.
2. Pestaña **Basics**:
   - Name: `agw-cv-fabri`.
   - Tier: **WAF v2**.
   - Autoscaling: activado, min 2 instancia.
   - WAF policy 
   - Virtual network: `vnet-appgw-cv`.
   - Subnet: `subnet-appgw`.

<br>
  
<img width="777" height="763" alt="Captura de pantalla 2026-04-05 023819" src="https://github.com/user-attachments/assets/c948ac23-271d-4238-9f71-5fec58612e62" />

<br>

3. **Frontend**:
   - Tipo: **Public**.
   - Nueva IP pública: `ip-public.web`.

<br>

<img width="772" height="269" alt="4" src="https://github.com/user-attachments/assets/dea41df9-19a9-452b-8fb3-3cc13187c527" />

<br>

4. **Backend pool**:
   - Name: `bp-cv-app`.
   - Backend target:
     - Tipo: `App Service`.
     - Seleccionar `cv-webb`.

<img width="565" height="321" alt="5" src="https://github.com/user-attachments/assets/6c5adc11-a508-4ac0-aa6b-4e6db06879a0" />


5. **HTTP Settings**:
   - Name: `https-setting-cv`.
   - Protocol: **HTTP** (o HTTPS si quieres cifrar hasta el backend).
   - Port: `80` (o `443` si usas HTTPS en el App Service).
   - **Importante:** marcar  
     `Pick host name from backend target`  
     para que la cabecera `Host` sea la del App Service y evitar errores 400.

<img width="832" height="696" alt="Captura de pantalla 2026-04-05 012021" src="https://github.com/user-attachments/assets/08c30b72-3be7-4ae8-abd6-4f15f1227854" />

<img width="565" height="321" alt="5" src="https://github.com/user-attachments/assets/f84cd04f-d04a-4997-8f4c-cbcc3eff04f9" />

6. **Routing Rule**:
   - Name: `rule-cv-all`.
   - Listener:
     - Protocol: `HTTP`.
     - Port: `80`.
   - Backend target:
     - `bp-cv-app` .

<img width="922" height="330" alt="Captura de pantalla 2026-04-05 011822" src="https://github.com/user-attachments/assets/116cc6a4-2a40-4a5e-af72-bda49291e530" />

7. En la sección de WAF seleccionar crear nueva directiva:
   - Policy name: `wafpolicy-cv`.
   - Mode: **Prevention**.

Crear y esperar a que se complete la implementación.

### 2.3. Comprobar salud del backend

1. Ir al recurso `agw-cv-fabri`.
2. Menú `Backend health`.
3. Verificar que el backend `cv-webb` aparece como **Healthy**.

<img width="1412" height="466" alt="6" src="https://github.com/user-attachments/assets/a62fd198-61d6-46f4-a2ba-15f63f897b08" />

<br>

### Importante

Podremos ver en **Frontendip** la ip pública (esto es la ip pública con lo que los clientes pueden acceder y también nos servirá para hacer los test )

<img width="409" height="325" alt="7" src="https://github.com/user-attachments/assets/72e90542-e56b-4b29-b245-9eabf2dc65f1" />

---

## Paso 3. Web Application Firewall (WAF) en Application Gateway

**Objetivo:** Inspeccionar y bloquear tráfico malicioso antes de que llegue al App Service. Función: Es el "escudo" que 
inspecciona cada paquete de datos antes de que llegue a nuestra web. Protege contra las 10 vulnerabilidades más críticas (OWASP).

### 3.1. Activar y asociar la WAF Policy

1. En el Application Gateway, sección `Web application firewall`.
2. Confirmar que está asociado a la directiva `wafpolicy-cv` en modo **Prevention**.

### 3.2. Reglas administradas OWASP

1. Abrir el recurso **WAF Policy (`wafpolicy-cv`)**.
2. Sección `Managed rules`.
3. Habilitar el rule set por defecto (OWASP/DRS) en modo **Blocking**.


<img width="945" height="717" alt="Captura de pantalla 2026-04-05 012959" src="https://github.com/user-attachments/assets/69ae7548-c0b5-4796-a31e-3b5b006ce040" />

### 3.3. Reglas personalizadas (Custom Rules)

#### 3.3.1. Regla Block-BadBot (User-Agent)

- Name: `Block-BadBot`.
- Priority: `1`.
- Rule type: `Match`.
- Match condition:
  - Match variable: `RequestHeaders`.
  - Selector: `User-Agent`.
  - Operator: `Contains`.
  - Value: `BadBot`.
- Action: **Block**.


<img width="559" height="649" alt="9" src="https://github.com/user-attachments/assets/f6c030f2-c626-4a1a-9054-228114aefee6" />


<img width="559" height="649" alt="Captura de pantalla 2026-04-05 031715" src="https://github.com/user-attachments/assets/b35f5bba-6056-4154-9178-aa627824d936" />


Interpretación:

<p>
    Cualquier petición que lleve User-Agent: ...BadBot... será considerada sospechosa y se bloqueará (403), según la acción que hayas definido en la custom rule.
    Esto encaja con el script test-badbot-agw.js de k6: todas esas peticiones deberían contar como fallidas (403) y nunca llegar al backend.
</p>

#### 3.3.2. Regla de rate limiting simple por IP (simulación DoS)

Application Gateway WAF no tiene un rate limit tan flexible como Front Door, pero puedes aproximarlo:

- Name: `BloqueoPorVolumen`.
- Rule type: `Match`.
- Match variable: `RemoteAddr`.
- Operator: `IPMatch`.
- Values: una o varias IPs (por ejemplo tu IP pública) cuando quieras bloquearlas después de una prueba.

(Para simular “rate limit”, puedes automatizar que añades esa IP a la lista tras observar muchas peticiones de ella.)

<img width="812" height="716" alt="8" src="https://github.com/user-attachments/assets/24adf59c-f22f-4c84-a9fe-1222bee18e2d" />

<img width="1893" height="746" alt="imagen" src="https://github.com/user-attachments/assets/a15a4276-12dc-4c4a-a684-691d538df88d" />

Interpretación: 

“Para cualquier IP de cliente, si envía más de 20 peticiones en 1 minuto, deniega el tráfico”.

---

## Paso 4. Pruebas de estrés y seguridad con k6

**Objetivo:** Simular tráfico legítimo y malicioso, y verificar que el WAF actúa.

### 4.1. Instalación rápida de k6 (Windows + PowerShell)

1. Abrir en K6

   - Descargamos un msi para windows 


Haremos testeos: 

<img width="999" height="585" alt="Captura de pantalla 2026-04-05 135148" src="https://github.com/user-attachments/assets/2e6ed602-3dbb-4986-9ac0-47b4b2964132" />

<br>

<img width="2116" height="743" alt="imagen" src="https://github.com/user-attachments/assets/47a2575d-815a-4d27-9103-db54d3585243" />

### Interpretacion 

“En el escenario de ataque intenso (boot.js, 10 VUs, ~6000 peticiones en 60 s), la combinación de la regla de User‑Agent Block‑BadBot y la regla de rate‑limit AtaqueDDoS provoca que el 100% de las solicitudes sean rechazadas (http_req_failed 100%). La latencia media de ~50 ms indica que el WAF corta el tráfico en el borde, sin llegar al backend.”“En el escenario de ataque intenso (boot.js, 10 VUs, ~6000 peticiones en 60 s), la combinación de la regla de User‑Agent Block‑BadBot y la regla de rate‑limit AtaqueDDoS 
provoca que el 100% de las solicitudes sean rechazadas (http_req_failed 100%). La latencia media de ~50 ms indica que el WAF corta el tráfico en el borde, sin llegar al backend.”


<br>

<img width="1006" height="511" alt="Captura de pantalla 2026-04-05 135120" src="https://github.com/user-attachments/assets/11bc8a4c-60b7-4732-94a9-e6b48058ac75" />


<br>


<img width="2116" height="743" alt="imagen" src="https://github.com/user-attachments/assets/999bc54a-7915-4e1b-9822-3b97e0be01d4" />
/>


### Interpretacion 

“En la prueba de carga leve (test‑suave‑agw.js, 5 VUs y 145 solicitudes), las reglas WAF siguen devolviendo un 100% de peticiones fallidas. Esto se debe a que la regla de rate‑limit AtaqueDDoS está aplicada al rango completo 0.0.0.0/0 con un umbral muy bajo (20 requests/min por IP). Tras superar este valor, 
todas las solicitudes de la misma IP quedan bloqueadas, lo que demuestra la eficacia del control pero también muestra el riesgo de configurar un límite demasiado agresivo.”


---

## Paso 5. Monitorización y topología

**Objetivo:** Visualizar el impacto de las pruebas y cómo fluye el tráfico.

### 5.1. Métricas en Application Gateway

1. Ir a `agw-cv-fabri` → menú `Metrics`.
2. Métrica: `Total Requests`.
3. Agregar filtro/segmento por `HttpStatusCode` o métrica de WAF (por ejemplo `BlockedRequests`).
4. Ejecutar los tests de k6 y observar:
   - Incremento de 2xx en pruebas suaves.
   - Picos de 4xx (403) durante `Block-BadBot`.

**Captura sugerida**

- Gráfica de `Total Requests` mostrando un pico y distribución por códigos 2xx/4xx.
  
<img width="778" height="351" alt="imagen" src="https://github.com/user-attachments/assets/9eef1df2-ae47-4504-a9ee-6ae3279bcb99" />


---

## Notas finales

- En un entorno real podrías añadir **TLS end-to-end**, dominios personalizados y reglas WAF adicionales (geolocalización, tamaños de cuerpo, etc.).  
- Este laboratorio está pensado para que puedas hacer las pruebas, tomar capturas y destruir los recursos en menos de una hora de uso efectivo.
# Laboratorio: Protección de un CV en Azure con App Service + Application Gateway WAF + k6

Este README describe paso a paso cómo desplegar un CV en Azure, protegerlo con **Application Gateway WAF** y simular ataques con **k6**, recopilando evidencias gráficas para tu proyecto.

