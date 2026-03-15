# Azure Private Chatbot AI (RAG con PDFs internos)

Proyecto de asistente de IA donde su única fuente de información es le establecí, en este caso subí al storage documentos como : la política general de recursos humanos. pdf o política de vaciones y permisos, en la que Azure Ai foundry que ingrega el recurso de GPTO4-mini responde en base a dicho almacenamiento en el storage en Azure. El chatbot usa un modelo LLM en Azure AI Foundry y recuperación aumentada por búsqueda (RAG) con Azure AI Search y Azure Blob Storage.

## Objetivo del proyecto

- Permitir que cualquier empleado haga preguntas relacionadas relacionadas en base a la documentación de la empresa y si son preguntas ajenas a ella el chatbot responderá que no tiene esa información. 
- Centralizar las políticas en Azure Storage y exponerlas a través de un asistente controlado.
- Demostrar experiencia práctica con Azure AI Foundry, Azure AI Search y servicios de almacenamiento.
- En este caso usamos azure blob storage y  documentación de la empresa, pero se puede integar AzureCosmos DB for mongo DB se podría consultar en el chatbot en función de la base de datos.

## Arquitectura

Componentes principales:

- **Azure Blob Storage**  
  - Contiene los PDFs de políticas en un contenedor privado.

- **Azure AI Search**  
  - Indexa el contenido de los PDFs (campo `content` searchable).  
  - Expone el índice `idx-politicas` para consultas semánticas.

- **Azure AI Foundry**  
  - Proyecto que orquesta el agente de chat.  
  - Implementación de un modelo LLM (p.ej. `gpt-4.1-mini` o similar).  
  - Conexión al servicio `search-politicas-empresa` y al índice `idx-politicas` como origen de datos.

- **Área de juegos de chat (Chat Playground)**  
  - Interfaz para probar el asistente y hacer preguntas de negocio.

> Flujo: Usuario → Chat en Foundry → LLM → Azure AI Search (recupera fragmentos de PDFs) → LLM genera respuesta citando las políticas.

## Servicios de Azure utilizados

- Azure AI Foundry (Proyecto, agente y área de juegos de chat)
- Azure AI Search (servicio de búsqueda + índice + indexador)
- Azure Storage Account (Blob Storage para PDFs)

## Pasos de implementación 


1. **Almacenamiento de PDFs**
   - Crear una Storage Account.



   - Crear un contenedor privado (por ejemplo `politicas-pdf`).
   - Subir los PDFs de políticas de empresa.



1. **Azure AI Search**
   - Crear un servicio de Azure AI Search (por ejemplo `search-politicas-empresa`).
   - Usar el asistente **Import data** para:
     - Origen: Azure Blob Storage → contenedor `politicas-pdf`.
     - Habilitar extracción de texto y habilidades cognitivas básicas.
     - Crear índice `idx-politicas` con:
       - Campo `key` (string) como clave.
       - Campo `content` marcado como `searchable` y `retrievable`.
     - Crear y ejecutar el indexador (comprobar que el estado es `Success`).

2. **Proyecto en Azure AI Foundry**
   - Crear un AI Hub y un Proyecto en la misma región.
   - En el catálogo de modelos, desplegar un modelo LLM (por ejemplo `gpt-4.1-mini` o `gpt-4o-mini`, según cuota).
   - En el *Centro de administración* del proyecto:
     - Crear una **conexión** a `search-politicas-empresa` usando clave de administrador.
   - En **Datos e índices**:
     - Registrar el índice `idx-politicas` como origen de datos del proyecto.

3. **Configuración del agente de chat**
   - Crear un nuevo agente / chat en el área de juegos.
   - Seleccionar la implementación del modelo LLM.
   - Añadir como origen de datos el índice de Azure AI Search (`search-politicas-empresa` + `idx-politicas`).
   - Mensaje de sistema recomendado:

     > “Eres el asistente de políticas internas de la empresa. Solo puedes responder utilizando la información de los documentos de políticas conectados.  
     > Si la información no está en los documentos, responde claramente que no está definida.”

   - Ajustar creatividad baja y número de documentos recuperados (3–5).

4. **Pruebas**
   - Consultas de ejemplo:
     - “Enumera todas las políticas de la empresa que aparecen en los documentos.”
     - “¿Cuál es la política sobre vacaciones y días libres?”
   - Validar que:
     - El asistente cita fragmentos que existen en los PDFs.
     - Si algo no está definido, lo indica en lugar de inventar información.

## Retos y soluciones

- **Error 403 al conectar con Azure AI Search**  
  - Solución: usar autenticación por clave en la conexión desde Foundry y habilitar el acceso de red público o servicios de confianza.

- **Limitaciones de cuota en Azure OpenAI / cuenta de estudiante**  
  - Ajustar el tipo de implementación (Estándar global) y, si es necesario, usar modelos alternativos o suscripción de pago.

## Posibles mejoras

- Publicar una interfaz web (por ejemplo con Azure Static Web Apps) para que usuarios no técnicos accedan al bot.
- Añadir autenticación (Azure AD) para limitar el acceso a empleados.
- Métricas y logging detallado de consultas y documentos utilizados.
- Integración con Microsoft Teams como bot corporativo.

## Cómo ejecutar / reproducir

Este proyecto se basa principalmente en recursos de Azure creados desde el portal. Para reproducirlo:

1. Crear los servicios indicados en la sección de pasos.  
2. Configurar el agente de chat con el mismo mensaje de sistema y origen de datos.  
3. Probar desde el *Chat Playground* de Azure AI Foundry.

> Nota: los nombres de recursos (`search-politicas-empresa`, `idx-politicas`, etc.) se pueden adaptar, siempre que se actualicen en las conexiones del proyecto.


