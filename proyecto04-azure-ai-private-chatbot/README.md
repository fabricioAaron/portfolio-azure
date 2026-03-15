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
<br>


![iamgen](https://github.com/fabricioAaron/portfolio-azure/blob/main/proyecto04-azure-ai-private-chatbot/.github/images/4.png)

<br>

   - Crear un contenedor privado.
<br>


 ![iamgen](https://github.com/fabricioAaron/portfolio-azure/blob/main/proyecto04-azure-ai-private-chatbot/.github/images/5.png)
 
   - Subir los PDFs de políticas de empresa.
<br>

![iamgen](https://github.com/fabricioAaron/portfolio-azure/blob/main/proyecto04-azure-ai-private-chatbot/.github/images/6.png)

<br>
1. **Azure AI Search**
   - Crear un servicio de Azure AI Search (por ejemplo `search-politicas-empresa`).
<br>

![iamgen](https://github.com/fabricioAaron/portfolio-azure/blob/main/proyecto04-azure-ai-private-chatbot/.github/images/7.png)


   - Usar el asistente **Import data** para:
<br>

![iamgen](https://github.com/fabricioAaron/portfolio-azure/blob/main/proyecto04-azure-ai-private-chatbot/.github/images/8.png)

<br>
    - Origen: Azure Blob Storage → contenedor `documents`.
     - Habilitar extracción de texto y habilidades cognitivas básicas.
     - Crear índice `idx-politicas` con:
       - Campo `key` (string) como clave.
       - Campo `content` marcado como `searchable` y `retrievable`.
</br>

![iamgen](https://github.com/fabricioAaron/portfolio-azure/blob/main/proyecto04-azure-ai-private-chatbot/.github/images/9.png)

<br>
     - Crear y ejecutar el indexador (comprobar que el estado es `Success`).
<br>

![iamgen](https://github.com/fabricioAaron/portfolio-azure/blob/main/proyecto04-azure-ai-private-chatbot/.github/images/10.png)

<br>
2. **Proyecto en Azure AI Foundry**
   - Crear un AI Hub y un Proyecto en la misma región.
<br>

   ![iamgen](https://github.com/fabricioAaron/portfolio-azure/blob/main/proyecto04-azure-ai-private-chatbot/.github/images/1.png)
   
<br>
   - En el catálogo de modelos, desplegar un modelo LLM (por ejemplo `gpt-4.1-mini` o `gpt-4o-mini`, según cuota).
<br>

   ![iamgen](https://github.com/fabricioAaron/portfolio-azure/blob/main/proyecto04-azure-ai-private-chatbot/.github/images/2.png)
<br>
<br>

   ![iamgen](https://github.com/fabricioAaron/portfolio-azure/blob/main/proyecto04-azure-ai-private-chatbot/.github/images/3.png)

<br>


<br>
   - En el *Centro de administración* del proyecto:
     - Crear una **conexión** a `search-politicas-empresa` usando clave de administrador.
     
   - En **Datos e índices**:
     - Registrar el índice `idx-politicas` como origen de datos del proyecto.
<br>

![iamgen](https://github.com/fabricioAaron/portfolio-azure/blob/main/proyecto04-azure-ai-private-chatbot/.github/images/11.5.png)

<br>

![iamgen](https://github.com/fabricioAaron/portfolio-azure/blob/main/proyecto04-azure-ai-private-chatbot/.github/images/11.6.png)

<br>

  -  Despues de haber implementado, tendremos que ir a Mis recursos > Modelos + punto de conexión, allí agregaremos nuestras APIKEY automaticamente (blob storage + Azure AI search)
  
<br>

  ![iamgen](https://github.com/fabricioAaron/portfolio-azure/blob/main/proyecto04-azure-ai-private-chatbot/.github/images/11.7.png)
  
   ![iamgen](https://github.com/fabricioAaron/portfolio-azure/blob/main/proyecto04-azure-ai-private-chatbot/.github/images/12.png)

<br>   

![iamgen](https://github.com/fabricioAaron/portfolio-azure/blob/main/proyecto04-azure-ai-private-chatbot/.github/images/13.png)


3. **Configuración del agente de chat**
   - Crear un nuevo agente / chat en el área de juegos.
   - Seleccionar la implementación del modelo LLM.
   - Añadir como origen de datos el índice de Azure AI Search (`search-politicas-empresa` + `idx-politicas`).
   - Mensaje de sistema recomendado:


     > “Eres el asistente de políticas internas de la empresa. Solo puedes responder utilizando la información de los documentos de políticas conectados.  
     > Si la información no está en los documentos, responde claramente que no está definida.”

<br>     

![iamgen](https://github.com/fabricioAaron/portfolio-azure/blob/main/proyecto04-azure-ai-private-chatbot/.github/images/14.png)

4. **Pruebas**
   - Consultas de ejemplo:
     - “Dime cuales son los permisos retribuidos y com ose solicitan y aparte comentame sobre cuales son los tipos de remuneración y beneficios de la empresa”
  
   - Validar que:
    

## Retos y soluciones

- **Error 403 al conectar con Azure AI Search**  
  - Solución: usar autenticación por clave en la conexión desde Foundry y habilitar el acceso de red público o servicios de confianza.

- **Limitaciones de cuota en Azure OpenAI / cuenta de estudiante**  
  - Ajustar el tipo de implementación (Estándar global) y, si es necesario, usar modelos alternativos o suscripción de pago.



## Cómo ejecutar / reproducir

Este proyecto se basa principalmente en recursos de Azure creados desde el portal. Para reproducirlo:

1. Crear los servicios indicados en la sección de pasos.  
2. Configurar el agente de chat con el mismo mensaje de sistema y origen de datos.  
3. Probar desde el *Chat Playground* de Azure AI Foundry.




