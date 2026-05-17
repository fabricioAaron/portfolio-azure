# Resumen global: Infraestructura On‑Premise vs Azure y comparación de costes

## 📋 Índice

1. [Infraestructura On‑Premise (técnica y funcional)](#1-infraestructura-on-premise-tecnica-y-funcional)  
2. [Infraestructura en Azure (técnica y funcional)](#2-infraestructura-en-azure-tecnica-y-funcional)  
3. [Costes On‑Premise: visión anual](#3-costes-on-premise-vision-anual)  
4. [Costes en Azure: estimación anual por servicios](#4-costes-en-azure-estimacion-anual-por-servicios)  
5. [Comparación funcional y de costes](#5-comparacion-funcional-y-de-costes)

---

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

Arquitectónicamente, todo depende de uno o pocos hosts físicos y del almacenamiento local; la empresa asume el coste y la gestión del hardware, energía, refrigeración, reemplazos y operaciones diarias.

---

## 2. Infraestructura en Azure (técnica y funcional)

En Azure se ha “troceado” esa infraestructura en servicios gestionados (PaaS/IaaS/híbridos):

- **Identidad y servidor de ficheros híbrido**  
  - AD on‑premise sigue existiendo, pero se integra con Azure Arc para gestión centralizada.  
  - La carpeta compartida `\\AD\shares` se sincroniza con un Azure File Share (`shares`) usando Azure File Sync, con backup gestionado en Azure.  

- **Capa de datos**  
  - La BD en SQL Server on‑premise se migra a Azure SQL Database (`db-reservas` en `sqlsrv-reservas-lab`).  
  - Función: base de datos PaaS con copias automáticas, alta disponibilidad y mantenimiento gestionado por Azure.

- **Capa de aplicación**  
  - La aplicación .NET pasa de VM/IIS a Azure App Service (`Ayacucho-Aventura`) sobre un App Service Plan B1.  

- **Mensajería**  
  - RabbitMQ se sustituye por Azure Service Bus (namespace `cola-reservas`, cola `servicebus`).  

- **Perímetro y seguridad web**  
  - La aplicación se publica a través de Azure Application Gateway + WAF, con listener HTTPS, terminación TLS, inspección WAF y regla de rate limiting (10 peticiones/min por IP).  
  - El App Service sólo acepta tráfico que viene del Application Gateway (restricción de acceso).

- **Gestión híbrida**  
  - El servidor AD on‑premise está registrado en Azure Arc, lo que permite aplicar políticas, monitorización y otras capacidades de Azure sobre el servidor local.

En resumen: se pasa de un entorno basado en VMs sobre un hipervisor a una arquitectura basada en servicios gestionados y sincronización híbrida, reduciendo la dependencia de hardware propio.

---

## 3. Costes On‑Premise: visión anual

Para una empresa pequeña, el coste anual aproximado de software/licencias y almacenamiento es:

- vSphere Essentials Kit (hipervisor y gestión): **600–900 €/año**  
- Windows Server (3 VMs): **900–1.500 €/año**  
- CAL de usuarios (25 CAL): **300–600 €/año**  
- SQL Server 2022 Standard (4 cores): **800–1.200 €/año**  
- Veeam Backup Essentials: **700–1.000 €/año**  
- Almacenamiento local datastores ESXi (≈300 GB): **50–150 €/año**  
- Almacenamiento para backups (≈1 TB): **100–200 €/año**  
- Ubuntu + RabbitMQ: coste de licencia **0 €** (no incluye soporte/tiempo).

**Total anual estimado on‑premise (software/licencias/almacenamiento):**  
👉 Entre **3.450 y 5.550 € / año**  
(≈ **287–462 €/mes**, sin contar hardware, energía, mantenimiento físico ni horas de administración).

---

## 4. Costes en Azure: estimación anual por servicios

Entre el 11 y el 15 (5 días) el coste del Resource Group `lab-migration-onpremise` fue **22,02 €**.  
Si suponemos el mismo patrón de uso todos los días del año, proyectamos:

- Factor mensual (30 días): × 30 / 5 = × 6  
- Factor anual (12 meses): × 72  

### 4.1. Estimación mensual y anual desglosada

| Servicio / Recurso                         | Coste 5 días | Coste mensual estimado (×6) | Coste anual estimado (×72) |
|-------------------------------------------|-------------:|----------------------------:|----------------------------:|
| Application Gateway `ayacucho.aventuras`  | 12,99 €      | ≈ 77,94 €                   | ≈ 935,28 €                  |
| Application Gateway `ayacucho-aventuras`  | 3,24 €       | ≈ 19,44 €                   | ≈ 233,28 €                  |
| App Service Plan `asp-labmigrationonpremise-9657` | 4,02 € | ≈ 24,12 €                   | ≈ 289,44 €                  |
| SQL Server `server-reservas`              | 0,40 €       | ≈ 2,40 €                    | ≈ 28,80 €                   |
| SQL Database `sqlsrv-reservas-lab / sql-db-reservas` | 0,14 € | ≈ 0,84 €          | ≈ 10,08 €                   |
| Storage Account `backupdbreservas`        | 0,19 €       | ≈ 1,14 €                    | ≈ 13,68 €                   |
| Azure Arc / HybridCompute `ad`            | 0,19 €       | ≈ 1,14 €                    | ≈ 13,68 €                   |
| Public IP `pip-appgw`                     | 0,19 €       | ≈ 1,14 €                    | ≈ 13,68 €                   |
| Private Endpoint `pe-sql`                 | 0,43 €       | ≈ 2,58 €                    | ≈ 30,96 €                   |
| App Service Web App `ayacucho-aventura`   | 0,04 €       | ≈ 0,24 €                    | ≈ 2,88 €                    |

**Suma anual aproximada con este patrón de uso:**  
👉 ≈ **1.550–1.600 €/año**

Se observa que:

- El **Application Gateway** es el componente más caro (≈ 1.170 € sumando ambos gateways).  
- El **App Service Plan** supone unos 290 €/año.  
- SQL, Storage, Arc, IP y Private Endpoint tienen un coste relativamente bajo en comparación.

---

## 5. Comparación funcional y de costes

### 5.1. Funcionalidad

- **On‑premise**  
  - Control total, pero la empresa asume hardware, licencias, parches, copias, capacidad y alta disponibilidad.  
  - Escalar implica CAPEX: comprar más servidores, almacenamiento, licencias, etc.

- **Azure**  
  - Las piezas críticas (app, datos, mensajería, ficheros) son servicios gestionados con alta disponibilidad, backup y actualizaciones incluidos.  
  - Escalar es OPEX: cambiar tiers o tamaño de los servicios, sin inversión en hardware.

### 5.2. Comparación de costes anuales (estimados)

| Concepto               | On‑Premise                                      | Azure (estimación actual)                |
|------------------------|-------------------------------------------------|------------------------------------------|
| Coste anual directo    | 3.450–5.550 €/año                               | ~1.550–1.600 €/año                       |
| Tipo de gasto          | Principalmente CAPEX (licencias + hardware)     | OPEX (pago mensual por consumo)          |
| Costes ocultos         | Hardware, energía, climatización, averías       | Menos visibles (principalmente servicio) |
| Escalado               | Lento, requiere compras y proyectos             | Rápido, cambio de tier/configuración     |
| Operación diaria       | Alta carga (patching, backups, pruebas DR)      | Menor carga (enfoque en app y datos)     |

**Conclusión:**  
Con esta arquitectura, incluso con un Application Gateway relativamente caro para un laboratorio, la solución en Azure se sitúa alrededor de **un 30–60 % más barata** que mantener toda la solución sólo on‑premise en licencias y almacenamiento, y además simplifica operación, backup y escalado.
