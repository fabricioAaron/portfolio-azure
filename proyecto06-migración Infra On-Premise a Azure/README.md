# Resumen global: Infraestructura On‑Premise vs Azure y comparación de costes

## 📋 Índice

1. [Infraestructura On‑Premise (técnica y funcional)](#1-infraestructura-on-premise-tecnica-y-funcional)  
2. [Infraestructura en Azure (técnica y funcional)](#2-infraestructura-en-azure-tecnica-y-funcional)  
3. [Costes On‑Premise: visión anual](#3-costes-on-premise-vision-anual)  
4. [Costes en Azure: estimación anual](#4-costes-en-azure-estimacion-anual)  
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

En Azure has “troceado” esa infraestructura en servicios gestionados (PaaS/IaaS/híbridos):

- **Identidad y servidor de ficheros híbrido**  
  - AD on‑premise sigue existiendo, pero se integra con Azure Arc para gestión centralizada.  
  - La carpeta compartida `\\AD\shares` se sincroniza con un Azure File Share (`shares`) usando Azure File Sync, con backup gestionado en Azure.  
  - Función: mantener la experiencia SMB local pero con resiliencia y backup en la nube.

- **Capa de datos**  
  - La BD en SQL Server on‑premise se migra a Azure SQL Database (`db-reservas` en `sqlsrv-reservas-lab`).  
  - Función: base de datos PaaS con copias automáticas, alta disponibilidad y mantenimiento gestionado por Azure.

- **Capa de aplicación**  
  - La aplicación .NET pasa de VM/IIS a Azure App Service (`Ayacucho-Aventura`) sobre un App Service Plan B1.  
  - Función: ejecutar la web de reservas sin administrar sistema operativo ni IIS, sólo el código.

- **Mensajería**  
  - RabbitMQ se sustituye por Azure Service Bus (namespace `cola-reservas`, cola `servicebus`).  
  - Función: cola de mensajes totalmente gestionada e integrada con la aplicación .NET.

- **Perímetro y seguridad web**  
  - La aplicación se publica a través de Azure Application Gateway + WAF, con listener HTTPS, terminación TLS, inspección WAF y una regla de rate limiting (10 peticiones/min por IP).  
  - El App Service sólo acepta tráfico que viene del Application Gateway (restricción de acceso).

- **Gestión híbrida**  
  - El servidor AD on‑premise está registrado en Azure Arc, lo que permite aplicar políticas, monitorización y otras capacidades de Azure sobre el servidor local.

En resumen: pasas de un entorno basado en VMs en un hipervisor a una arquitectura basada en servicios gestionados y sincronización híbrida, reduciendo la dependencia de hardware propio.

---

## 3. Costes On‑Premise: visión anual

Para una empresa pequeña, el coste anual aproximado de software/licencias y almacenamiento es:

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

- Ubuntu + RabbitMQ  
  - Coste de licencia **0 €** (aunque hay coste de tiempo/soporte si se profesionaliza).

**Total anual estimado on‑premise (software/licencias/almacenamiento):**  
👉 Entre **3.450 y 5.550 € / año**  
(≈ **287–462 €/mes**, sin contar hardware, energía, mantenimiento físico ni horas de administración).

---

## 4. Costes en Azure: estimación anual

Entre el **11 y el 15** (5 días) el coste del Resource Group `lab-migration-onpremise` fue:

- **Total periodo (5 días):** 22,02 €  
  - Coste medio diario:  
    - 22,02 € ÷ 5 ≈ **4,40 €/día**

### 4.1. Estimación mensual

Suponiendo que el entorno se mantiene igual de activo todos los días del mes:

- 4,40 €/día × 30 días ≈ **132 €/mes**

### 4.2. Estimación anual

Proyectando ese patrón a un año completo:

- 132 €/mes × 12 meses ≈ **1.584 €/año**

Para no dar una cifra falsa de precisión, se puede redondear a un rango:

👉 **Coste anual estimado en Azure con este diseño y uso actual: ~1.500–1.600 €/año**

(Esto incluye principalmente Application Gateway + WAF, App Service Plan B1, SQL Database, Storage, Arc, IP pública y Private Endpoint).

---

## 5. Comparación funcional y de costes

### 5.1. Funcionalidad

- **On‑premise**  
  - Control total, pero la empresa asume hardware, licencias, parches, copias de seguridad, capacidad y alta disponibilidad.  
  - Escalar implica CAPEX: comprar más servidores, almacenamiento, licencias, etc.

- **Azure**  
  - Las piezas críticas (app, datos, mensajería, ficheros) son servicios gestionados con HA, backup y actualizaciones incluidos.  
  - Escalar es OPEX: cambiar tier de App Service, aumentar SQL, etc., sin invertir en hardware.

### 5.2. Comparación de costes anuales (estimados)

| Concepto               | On‑Premise                                      | Azure (estimación actual)         |
|------------------------|-------------------------------------------------|-----------------------------------|
| Coste anual directo    | 3.450–5.550 €/año                               | ~1.500–1.600 €/año                |
| Tipo de gasto          | Principalmente CAPEX (licencias + hardware)     | OPEX (pago mensual por consumo)   |
| Costes ocultos         | Hardware, energía, climatización, averías       | Menos visibles (principalmente servicio) |
| Escalado               | Lento, requiere compras y proyectos             | Rápido, cambio de tier/configuración |
| Operación diaria       | Alta carga (patching SO, backups, DR tests)     | Menor carga (te centras en app/datos) |

**Conclusión:**  
En tu escenario, incluso con un Application Gateway relativamente caro para un lab, la arquitectura en Azure se sitúa en torno a **un 30–60 % más barata** que mantener toda la solución on‑premise solo en licencias y almacenamiento, además de simplificar operación, backup y escalado.  

Si tuvieras que defender esto en 1 frase ante el tribunal, ¿cómo resumirías tú la diferencia clave entre pagar 3.500–5.500 €/año on‑premise frente a unos 1.500–1.600 €/año en Azure para esta solución híbrida?
