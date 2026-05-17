# Documentación de Proyecto: Gestión de Costes y Control Financiero

## 📋 Índice

1. [Gestión de Costes en Azure](#1-gestion-de-costes-en-azure)
   1. [Segmentación mediante Etiquetas (Tags)](#11-segmentacion-mediante-etiquetas-tags)
   2. [Análisis de Costes (Cost Analysis)](#12-analisis-de-costes-cost-analysis)
2. [Análisis del Grupo de Recursos y Presupuesto](#2-analisis-del-grupo-de-recursos-y-presupuesto)
   1. [Desglose por Servicio](#21-desglose-por-servicio)
   2. [Configuración de Budget y Alertas](#22-configuracion-de-budget-y-alertas)

---

## 1. Gestión de Costes en Azure

Para controlar el gasto de la migración y asegurar la sostenibilidad del laboratorio, se han aplicado dos mecanismos principales: **segmentación mediante etiquetas** y **análisis de costes con Cost Management**.

### 1.1. Segmentación mediante Etiquetas (Tags)

- Se ha utilizado la tag `departamento: it` en todos los recursos relacionados con la migración (App Service, SQL, Service Bus, Storage, Application Gateway, etc.). 
- Gracias a esta tag, en el panel de **Cost Management → Cost analysis** se puede filtrar específicamente por `departamento: it`, aislando el coste de la solución de migración híbrida frente a otros posibles gastos de la suscripción.

Esta práctica es básica en **FinOps** porque permite asignar el coste a un departamento o proyecto concreto y facilita la imputación de gastos.

<img width="938" height="785" alt="filtrmaos por tag" src="https://github.com/user-attachments/assets/0e054dd3-1a6a-461e-b64e-2007f99cd6c8" />

`dashboard `

---

### 1.2. Análisis de Costes (Cost Analysis)

- **Ámbito**: suscripción `Azure for Students` y grupo de recursos `lab-migration-onpremise`.
- **Rango temporal**: abril – mayo de 2026.  
- **Vista utilizada**: `AccumulatedCosts`, lo que permite ver cómo se acumula el gasto día a día y qué servicios son los que más contribuyen al coste total.

Con esta vista filtrada por la tag `departamento: it` se puede identificar con rapidez qué parte del coste corresponde a App Service, SQL, Storage, Service Bus o servicios de seguridad, y cómo evoluciona el consumo en el tiempo.

<img width="1630" height="730" alt="cost analiys de grupo de recurso listo " src="https://github.com/user-attachments/assets/28e7cca7-7045-461b-a566-184036e0d7d0" />

`Cost Analysis filtrando por etiqueta`

---

## 2. Análisis del Grupo de Recursos y Presupuesto

### 2.1. Desglose por Servicio

En el análisis detallado del grupo de recursos `lab-migration-onpremise` se observa un **coste acumulado muy bajo**, acorde con un entorno educativo y de pruebas, pero con un control fino por servicio.
Resumen del desglose (valores aproximados según el Cost Analysis):

# Reporte de Costos de Azure

## Resumen de Facturación
- **Total (EUR):** €22.94 (+1687%)
- **Promedio:** €1.43 / día
- **Presupuesto:** €8.00 / mes (budget-miragtion)
- **Suscripción:** Azure for Students

## Detalle de Recursos (Resource Group: lab-migration-onpremise)

# Análisis de Costes Estimados - Azure

## Desglose de Costes por Recurso

| Servicio / Recurso | Coste 5 días | Coste mensual (est. x6) | Coste anual (est. x72) |
| :--- | :---: | :---: | :---: |
| **Application Gateway** (ayacucho.aventuras) | 12,99 € | ≈ 77,94 € | ≈ 935,28 € |
| **Application Gateway** (ayacucho-aventuras) | 3,24 € | ≈ 19,44 € | ≈ 233,28 € |
| **App Service Plan** (asp-lab...) | 4,02 € | ≈ 24,12 € | ≈ 289,44 € |
| **SQL Server** (server-reservas) | 0,40 € | ≈ 2,40 € | ≈ 28,80 € |
| **SQL Database** (sqlsrv-reservas-lab) | 0,14 € | ≈ 0,84 € | ≈ 10,08 € |
| **Storage Account** (backupdbreservas) | 0,19 € | ≈ 1,14 € | ≈ 13,68 € |
| **Azure Arc / HybridCompute** (ad) | 0,19 € | ≈ 1,14 € | ≈ 13,68 € |
| **Public IP** (pip-appgw) | 0,19 € | ≈ 1,14 € | ≈ 13,68 € |
| **Private Endpoint** (pe-sql) | 0,43 € | ≈ 2,58 € | ≈ 30,96 € |
| **App Service Web App** (ayacucho-...) | 0,04 € | ≈ 0,24 € | ≈ 2,88 € |

## Proyección Total Anual

Basado en el patrón de uso actual, la estimación anual es:
**≈ 1.550 € – 1.600 € / año**

### 2.2. Configuración de Budget y Alertas

Para evitar sobrecostes y aplicar buenas prácticas de **FinOps**, se ha configurado un presupuesto específico llamado `budget-migration`:

- **Scope**: suscripción `Azure for Students`, filtrada por la tag `departamento: it`.  
- **Periodo de reseteo**: `Monthly` (mensual).  
- **Importe del presupuesto**: 8,00 € (umbral establecido para el laboratorio).  
- **Ventana temporal**: creación en mayo de 2026 y expiración en abril de 2028.

En las evidencias de Cost Management y Budget se observa que:

- El sistema detectó un consumo de **13,86 €**, lo que representa aproximadamente un **173 %** del presupuesto de 8,00 €.
- Se disparó la alerta al superar el **80 %** del presupuesto (6,40 €), generando una notificación automática.  
- Se recibió un correo del **Azure Virtual Assistant** indicando que el presupuesto `budget-migration` había alcanzado su límite, permitiendo reaccionar a tiempo (apagado de recursos, escalado a menos capacidad, etc.).

<img width="1602" height="763" alt="crear un badget " src="https://github.com/user-attachments/assets/7bfc619a-a068-4f6a-87e1-e25d35d78026" />

`creamos el budget con alertas enviados por email `

---

<img width="756" height="625" alt="imagen (1)" src="https://github.com/user-attachments/assets/337314ca-5957-4083-90a7-bff09febfa47" />

`comprobamos que la alerta nos llegó a nuestro correo`

---

<img width="1158" height="527" alt="imagen (2)" src="https://github.com/user-attachments/assets/298017b6-5cd1-439e-8bd9-60c8a1cb2fd8" />

`cost analyst`

---

#### Interpretación y conclusión técnica

Aunque el coste unitario por servicio es bajo, el uso de:

- **Tags** para segmentar costes,  
- **Cost Analysis** para entender el consumo por servicio y en el tiempo, y  
- **Budgets + alertas** para actuar antes de superar los límites,

demuestra una implementación profesional de **control financiero** en una migración híbrida.  
Con esta configuración se evita tener “sorpresas” en la facturación y se sientan las bases de un modelo FinOps que sería escalable a un entorno productivo real.[file:176][file:177][file:178]
