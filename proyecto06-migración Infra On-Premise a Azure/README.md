# Migración híbrida On‑Premise → Azure (Resumen + Costes)

## 📋 Índice

1. [Resumen de la Arquitectura Migrada](#resumen-de-la-arquitectura-migrada)  
2. [Componentes Migrados a Azure](#componentes-migrados-a-azure)  
3. [Gestión de Costes en Azure](#gestion-de-costes-en-azure)  
4. [Análisis de Costes del Grupo de Recursos](#analisis-de-costes-del-grupo-de-recursos)  
5. [Presupuesto (Budget) y Control Proactivo](#presupuesto-budget-y-control-proactivo)

---

## Resumen de la Arquitectura Migrada

La infraestructura on‑premise original (AD + APP .NET/SQL + RabbitMQ + Veeam + carpeta compartida) se ha modernizado de forma progresiva hacia Azure, manteniendo un enfoque híbrido:  
- La aplicación de reservas se ejecuta ahora en **Azure App Service**, utilizando **Azure SQL Database** como base de datos y **Azure Service Bus** como mensajería.  
- La carpeta compartida `\\AD\shares` se sincroniza con un **Azure File Share** mediante **Azure File Sync**, manteniendo acceso SMB local pero con datos replicados en la nube.  
- El frontal web se protege con **Azure Application Gateway + WAF**, que añade HTTPS, inspección de tráfico y reglas de rate limiting por IP.[file:147][file:140][file:169]

---

## Componentes Migrados a Azure

Los principales servicios implicados en el grupo de recursos `lab-migration-onpremise` son:[file:176]

- **Azure App Service** (`Ayacucho-Aventura` + App Service Plan básico B1)  
  - Aloja la aplicación ASP.NET Core.  
- **Azure SQL Database** (`db-reservas` en `sqlsrv-reservas-lab`)  
  - Recibe las reservas migradas desde el SQL on‑premise (.bacpac) y las nuevas operaciones de la web.[file:139]  
- **Azure Service Bus** (namespace `cola-reservas`, cola `servicebus`)  
  - Sustituye a RabbitMQ como cola de mensajes de reservas.[file:145]  
- **Azure Storage Account** (`backupdbreservas`) + **Azure File Share** (`shares`)  
  - Almacena la copia en la nube de la carpeta compartida de AD mediante Azure File Sync.[file:169][file:175]  
- **Azure File Sync (Storage Sync Service `sync-file-ad`)**  
  - Sincroniza `E:\shares` del servidor AD con el File Share `shares` y aplica Cloud Tiering.[file:171]  
- **Azure Application Gateway + WAF**  
  - Publica la web por HTTPS, revisa el tráfico con WAF y aplica una regla de rate limiting (10 peticiones/minuto por IP).[file:156][file:162]  

Todos estos recursos se agrupan bajo el mismo **Resource Group** etiquetado con la tag `departamento: it`, lo que permite filtrar fácilmente su consumo en Cost Management.[file:176][file:177]

---

## Gestión de Costes en Azure

Para controlar el gasto de la migración se han aplicado dos mecanismos principales:

1. **Etiquetas (tags) para segmentar costes**  
   - Se ha utilizado la tag `departamento: it` en los recursos del laboratorio.  
   - En **Cost Management → Cost analysis** se filtra por esta tag para ver únicamente el coste de la solución de migración híbrida.[file:177]  

2. **Análisis de Costes (Cost analysis)**  
   - En el ámbito de la suscripción `Azure for Students` y del grupo de recursos `lab-migration-onpremise` se ha abierto la vista de **Cost analysis**.[file:176]  
   - Rango de fechas: abril–mayo 2026.  
   - Vista: `AccumulatedCosts`, filtrada por tag `departamento: it`.[file:177]  

Esto permite identificar qué servicios de Azure son los que más contribuyen al coste total y cómo evoluciona el gasto a lo largo del tiempo.

---

## Análisis de Costes del Grupo de Recursos

En el análisis de costes del Resource Group `lab-migration-onpremise` se observa un coste acumulado muy bajo (≈0,51 € en el periodo mostrado, gracias a la suscripción educativa y al uso ligero).[file:177]

Desglose por servicio (valores aproximados mostrados en el Cost Analysis):[file:176][file:177]

- **Azure App Service**  
  - Plan básico B1: ~0,06 € en el periodo.  
- **SQL Database / SQL Server**  
  - Instancia `sqlsrv-reservas-lab` y su base de datos `db-reservas`: ~0,14 € cada una (servidor y base de datos).[file:176]  
- **Storage (File Share + Storage Account)**  
  - Cuenta `backupdbreservas` (File Share `shares` y otros datos de la migración): <0,01–0,04 €.  
- **Service Bus**  
  - Namespace `cola-reservas`: coste inferior a 0,01 € debido al bajo volumen de mensajes.[file:177]  
- **Otros servicios**  
  - Defender for Cloud u otros componentes de seguridad: ~0,06 € en total.[file:177]  

La vista de Cost Analysis permite además ver en un gráfico de área la evolución del coste diario/acumulado, mostrando cómo el total crece ligeramente a medida que se utilizan la web, la base de datos y el Service Bus.[file:177]

---

## Presupuesto (Budget) y Control Proactivo

Para evitar sobrecostes y practicar buenas prácticas de FinOps, se ha configurado un **presupuesto (Budget)** específico para el entorno de migración:[file:178]

- **Scope**: suscripción `Azure for Students`, filtrada por la tag `departamento: it`.  
- **Nombre**: `budget-migration`.  
- **Periodo de reseteo**: `Monthly`.  
- **Rango de fechas**: creación mayo 2026, expiración abril 2028.  
- **Importe del presupuesto**: (definido por el alumno; el gráfico muestra el coste real frente al umbral de presupuesto).[file:178]  

Con este budget, Azure puede enviar alertas cuando el coste estimado o real se acerque a cierto porcentaje del límite definido (por ejemplo 80 % y 100 %), lo que ayuda a vigilar el consumo de:

- App Service  
- SQL Database  
- Service Bus  
- Storage y File Sync  

Aunque en el laboratorio el coste real es muy bajo, este apartado demuestra cómo se implementaría el **control financiero** de una migración híbrida en un entorno real utilizando **Cost Management + Billing** y **Budgets** en Azure.[file:176][file:178]
