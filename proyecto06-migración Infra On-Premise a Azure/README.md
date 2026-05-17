# Resumen global: Infraestructura On‑Premise vs Azure y comparación de costes

## 1. Infraestructura On‑Premise (técnica y funcional)

En el entorno local la empresa pequeña se apoya en un hipervisor VMware ESXi / vSphere Essentials con varias máquinas virtuales:

- **AD / Ficheros (VM1)**  
  - Servicios: Active Directory, DNS, carpeta compartida de usuarios/departamentos.  
  - Función: identidad y autenticación, resolución de nombres interna, servidor de ficheros central.

- **APP (.NET + SQL Server) (VM2)**  
  - Servicios: IIS o Kestrel para la aplicación de reservas en .NET, base de datos SQL Server 2022 Standard.  
  - Función: lógica de negocio (web de reservas) y persistencia de datos (tabla `Reservas`).

- **RabbitMQ (Ubuntu) (VM3)**  
  - Servicios: Ubuntu Server con RabbitMQ (a veces montado en Docker).  
  - Función: cola de mensajes para desacoplar la lógica de envío de correos / procesos asíncronos de las reservas.

- **Veeam Backup (VM4)**  
  - Servicios: Veeam Backup & Replication/Essentials.  
  - Función: copias de seguridad de VMs, SQL y carpeta compartida hacia un repositorio en NAS/SAN.

- **Almacenamiento local**  
  - Datastores de ESXi para las VMs (unos cientos de GB útiles).  
  - NAS/SAN adicional para copias de Veeam y ficheros de usuarios (≈1 TB).

Arquitectónicamente, todo depende de un único host físico (o pocos hosts) y del almacenamiento local; la empresa asume el coste y la gestión del hardware, energía, refrigeración, reemplazos, etc.

---

## 2. Infraestructura en Azure (técnica y funcional)

En Azure has “troceado” esa infraestructura en servicios gestionados (PaaS/IaaS/híbridos):

- **Identidad y servidor de ficheros híbrido**  
  - AD on‑premise sigue existiendo, pero se integra con **Azure Arc** para gestión desde Azure.  
  - La carpeta compartida `\\AD\shares` se sincroniza con un **Azure File Share** (`shares`) usando **Azure File Sync**, con backup gestionado en Azure.  
  - Función: mantener la experiencia SMB local pero con resiliencia y backup en la nube.

- **Capa de datos**  
  - La antigua BD en SQL Server on‑premise se migra a **Azure SQL Database** (`db-reservas` en `sqlsrv-reservas-lab`).  
  - Función: base de datos PaaS con copia de seguridad automática, alta disponibilidad gestionada por Azure.

- **Capa de aplicación**  
  - La aplicación .NET pasa de VM/IIS a **Azure App Service** (`Ayacucho-Aventura`) sobre un App Service Plan B1.  
  - Función: ejecutar la web de reservas sin tener que administrar SO, parches ni IIS, solo el código.

- **Mensajería**  
  - RabbitMQ se sustituye por **Azure Service Bus** (namespace `cola-reservas`, cola `servicebus`).  
  - Función: cola de mensajes totalmente gestionada, integrada con la aplicación .NET.

- **Perímetro y seguridad web**  
  - Se publica la aplicación a través de **Azure Application Gateway + WAF** (dos gateways en tu coste, probablemente pruebas / despliegues).  
  - Función: terminación TLS, balanceo, WAF, regla de rate limiting (10 peticiones/min por IP), acceso al App Service restringido al gateway.

- **Gestión híbrida**  
  - El servidor AD on‑premise está registrado en Azure Arc (`microsoft.hybridcompute/machines`), lo que permite aplicar políticas, monitorización, etc., desde Azure.  

En resumen: pasas de un entorno centrado en VMs sobre un hipervisor a una arquitectura basada en servicios gestionados (PaaS) y sincronización híbrida, con menos dependencia de hardware propio.

---

## 3. Costes On‑Premise: visión anual

Tu tabla de on‑premise, para una empresa pequeña, da este rango aproximado de costes anuales de software/licencias y almacenamiento:

- vSphere Essentials Kit (hipervisor y gestión)  
  - **600–900 €/año**

- Windows Server (3 VMs) + CAL de usuarios  
  - Licencias Windows Server Standard (3 servidores): **900–1.500 €/año**  
  - CAL de usuario para AD/shares (25 CAL): **300–600 €/año**

- SQL Server 2022 Standard (4 cores)  
  - **800–1.200 €/año**

- Veeam Backup Essentials / por instancia  
  - **700–1.000 €/año**

- Almacenamiento local  
  - Datastores ESXi (≈300 GB útiles): **50–150 €/año**  
  - Repositorio de backups (≈1 TB): **100–200 €/año**

- Ubuntu + RabbitMQ: coste de licencia **0 €** (aunque habría coste de tiempo/soporte si se profesionaliza).

**Total anual estimado on‑premise (solo software/licencias/almacenamiento):**  
👉 Entre **3.450 y 5.550 € / año**  
(≈ **287–462 €/mes**, sin contar hardware, energía, mantenimiento físico, horas de administración, etc.)

---

## 4. Costes en Azure: visión del laboratorio

Del reporte de costes de Azure para el grupo de recursos `lab-migration-onpremise`:

- **Total (periodo observado):** 22,94 €  
  - Promedio 1,43 €/día  
  - Presupuesto configurado: 8 €/mes (se superó por pruebas intensivas).

Detalle por recursos (mismos servicios que en la migración):

| Servicio / Recurso                         | Rol aproximado en la arquitectura                       | Coste en el periodo |
|-------------------------------------------|---------------------------------------------------------|---------------------|
| Application Gateway (`ayacucho.aventuras` + otro) | Frontal HTTPS, WAF, balanceo hacia App Service     | 12,99 € + 3,24 €    |
| App Service Plan B1 (`asp-labmigrationonpremise`) | Hosting PaaS de la app .NET                          | 4,02 €              |
| SQL Server / SQL Database (`server-reservas`, `sqlsrv-reservas-lab/sql-db-reservas`) | BD de reservas gestionada en Azure SQL     | ≈0,54 € (0,40 + 0,14) |
| Storage Account (`backupdbreservas`)      | File Share `shares` + backups                         | 0,19 €              |
| Azure Arc / HybridCompute (`ad`)          | Registro del servidor AD on‑premise                   | 0,19 €              |
| Public IP (`pip-appgw`)                   | IP pública del Application Gateway                    | 0,19 €              |
| App Service Web App (`ayacucho-aventura`) | Sitio web de producción                                | 0,04 €              |
| Private Endpoint (`pe-sql`)               | Acceso privado a SQL                                   | 0,43 €              |
| **TOTAL**                                 |                                                         | **≈22,02 €**        |

Importante: este total corresponde al **periodo de pruebas** concreto, no a un año entero estable. Si lo proyectases linealmente (solo como referencia):

- 22,02 € en, por ejemplo, unas dos semanas de laboratorio intensivo → no es todavía un “mes típico”.  
- Si supusieras un coste similar cada mes (laboratorio activo): podrías estar en el orden de **20–30 €/mes** para este entorno pequeño, es decir **240–360 €/año** solo en Azure (sin contar posibles ajustes de tamaño/tier).

---

## 5. Comparación funcional y de costes

### 5.1. Funcionalidad

- **On‑premise**  
  - Control total del entorno, pero necesitas gestionar hardware, parches, copias, capacidad de almacenamiento y alta disponibilidad.  
  - Escalar (añadir CPU/RAM, más almacenamiento, segundo host ESXi) implica inversión de capital (CAPEX) y planificación de compra.

- **Azure**  
  - App, base de datos, mensajería y almacenamiento se convierten en servicios gestionados (PaaS) con backup, actualización y alta disponibilidad “incluidos” en la plataforma.  
  - Escalar es principalmente cambiar de tier o añadir instancias (OPEX puro): subir App Service Plan, subir DTU/vCore de SQL, etc.

### 5.2. Costes: On‑Premise vs Azure (visión conceptual)

| Aspecto               | On‑Premise (aprox.)                                | Azure (laboratorio actual)                              |
|-----------------------|----------------------------------------------------|---------------------------------------------------------|
| Coste anual directo   | 3.450–5.550 €/año (software + almacenamiento)      | Si extrapolamos: 240–360 €/año (solo consumo actual)    |
| Tipo de gasto         | Mayormente CAPEX (licencias + hardware)           | OPEX (pago por uso mensual de servicios)               |
| Alta disponibilidad   | Requiere más hosts/licencias (sube coste)         | Inherente a muchos servicios PaaS sin coste extra alto |
| Escalado              | Comprar hardware/licencias nuevas                  | Cambiar tiers o ajustar tamaños                         |
| Punto único de fallo  | Host ESXi / cabina                                | Azure gestiona redundancia de plataforma               |
| Esfuerzo operativo    | Alto (patching, hardware, backups, pruebas DR)    | Más bajo (te centras en la app y datos)                 |

En tu escenario de laboratorio, **Azure sale mucho más barato** porque:

- No estás pagando licencias completas enterprise, sino consumo educativo reducido.  
- No tienes costes de hardware ni de energía.  
- Los servicios PaaS están en tiers básicos.

En un entorno real de producción, la comparación exacta dependería de:

- Volumen de usuarios, transacciones y datos.  
- SLA requerido (9’s de disponibilidad).  
- Necesidad de varias regiones, copias GRS, etc.

Pero tu documentación ya transmite una idea clave que sí es trasladable a producción:  
**On‑premise concentra muchos costes fijos (licencias + hardware + mantenimiento), mientras que en Azure puedes convertir casi todo en coste variable y controlable con Cost Management, tags y budgets.**

Para preparar la defensa oral: ¿qué frase usarías tú para resumir en 10–15 segundos por qué, en este escenario, la arquitectura en Azure es más eficiente económicamente que mantener toda la solución solo on‑premise?
