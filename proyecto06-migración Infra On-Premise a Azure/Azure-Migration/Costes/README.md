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

Para controlar el gasto de la migración y asegurar la sostenibilidad del laboratorio, se han aplicado dos mecanismos principales: **segmentación mediante etiquetas** y **análisis de costes con Cost Management**.[file:176][file:177]

### 1.1. Segmentación mediante Etiquetas (Tags)

- Se ha utilizado la tag `departamento: it` en todos los recursos relacionados con la migración (App Service, SQL, Service Bus, Storage, Application Gateway, etc.).[file:176][file:177]  
- Gracias a esta tag, en el panel de **Cost Management → Cost analysis** se puede filtrar específicamente por `departamento: it`, aislando el coste de la solución de migración híbrida frente a otros posibles gastos de la suscripción.[file:177]  

Esta práctica es básica en **FinOps** porque permite asignar el coste a un departamento o proyecto concreto y facilita la imputación de gastos.

### 1.2. Análisis de Costes (Cost Analysis)

- **Ámbito**: suscripción `Azure for Students` y grupo de recursos `lab-migration-onpremise`.[file:176]  
- **Rango temporal**: abril – mayo de 2026.  
- **Vista utilizada**: `AccumulatedCosts`, lo que permite ver cómo se acumula el gasto día a día y qué servicios son los que más contribuyen al coste total.[file:177]  

Con esta vista filtrada por la tag `departamento: it` se puede identificar con rapidez qué parte del coste corresponde a App Service, SQL, Storage, Service Bus o servicios de seguridad, y cómo evoluciona el consumo en el tiempo.[file:177]

---

## 2. Análisis del Grupo de Recursos y Presupuesto

### 2.1. Desglose por Servicio

En el análisis detallado del grupo de recursos `lab-migration-onpremise` se observa un **coste acumulado muy bajo**, acorde con un entorno educativo y de pruebas, pero con un control fino por servicio.[file:176][file:177]

Resumen del desglose (valores aproximados según el Cost Analysis):[file:176][file:177]

| Servicio            | Recurso / Instancia                          | Coste estimado en el periodo |
|---------------------|----------------------------------------------|------------------------------|
| Azure App Service   | Plan básico B1 (`asp-labmigrationonpremise`)| ~0,06 €                      |
| SQL Database        | `sqlsrv-reservas-lab` / `db-reservas`       | ~0,14 € (cada uno)           |
| Storage             | `backupdbreservas` (File Share `shares`)    | <0,01 € – 0,04 €             |
| Service Bus         | Namespace `cola-reservas`                   | <0,01 €                      |
| Otros (seguridad)   | Defender for Cloud y servicios asociados    | ~0,06 €                      |

La vista de Cost Analysis también muestra un gráfico de área con el coste acumulado, donde se aprecia cómo el total crece ligeramente a medida que se utilizan la web de reservas, la base de datos y la mensajería.[file:177]

### 2.2. Configuración de Budget y Alertas

Para evitar sobrecostes y aplicar buenas prácticas de **FinOps**, se ha configurado un presupuesto específico llamado `budget-migration`:[file:178]

- **Scope**: suscripción `Azure for Students`, filtrada por la tag `departamento: it`.  
- **Periodo de reseteo**: `Monthly` (mensual).  
- **Importe del presupuesto**: 8,00 € (umbral establecido para el laboratorio).  
- **Ventana temporal**: creación en mayo de 2026 y expiración en abril de 2028.[file:178]  

En las evidencias de Cost Management y Budget se observa que:

- El sistema detectó un consumo de **13,86 €**, lo que representa aproximadamente un **173 %** del presupuesto de 8,00 €.[file:178]  
- Se disparó la alerta al superar el **80 %** del presupuesto (6,40 €), generando una notificación automática.  
- Se recibió un correo del **Azure Virtual Assistant** indicando que el presupuesto `budget-migration` había alcanzado su límite, permitiendo reaccionar a tiempo (apagado de recursos, escalado a menos capacidad, etc.).[file:178]  

#### Interpretación y conclusión técnica

Aunque el coste unitario por servicio es bajo, el uso de:

- **Tags** para segmentar costes,  
- **Cost Analysis** para entender el consumo por servicio y en el tiempo, y  
- **Budgets + alertas** para actuar antes de superar los límites,

demuestra una implementación profesional de **control financiero** en una migración híbrida.  
Con esta configuración se evita tener “sorpresas” en la facturación y se sientan las bases de un modelo FinOps que sería escalable a un entorno productivo real.[file:176][file:177][file:178]
