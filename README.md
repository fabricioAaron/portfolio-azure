# 🌩️ Portfolio de Laboratorios Multicloud (Azure & AWS)

Bienvenido a mi portfolio de laboratorios orientados a Cloud Computing. Este repositorio reúne proyectos y pruebas de concepto que realizo para poner en práctica y consolidar conocimientos en Microsoft Azure y Amazon Web Services (AWS), con un enfoque claro hacia administración y arquitectura cloud.

---

## 👨‍💻 Sobre mí

Soy un Administrador Multicloud con experiencia en:

- Administración de entornos virtualizados con VMware vSphere.
- Gestión de copias de seguridad y monitorización con herramientas como Zabbix.
- Soporte y colaboración con equipos de Desarrollo y Marketing.

Trabajo con Azure y AWS optimizando entornos híbridos mediante herramientas nativas e Infrastructure as Code.

Mi objetivo es evolucionar hacia Arquitecto Cloud diseñando soluciones seguras, escalables y automatizadas.

## 📚 Laboratorios incluidos

### 🖥️ lab01-AppServices
**Plataforma**: Azure App Service + GitHub Actions

**Lo que implementé**:
- Portfolio estático desplegado desde GitHub con CI/CD automático.
- Workflow GitHub Actions simplificado para sitios estáticos.
- Identidad administrada (user-assigned identity) para despliegue seguro.
- Verificación en Kudu (wwwroot) y actualización automática al hacer push.

**Finalidad**:
Pipeline DevOps completo para despliegue continuo de aplicaciones web con integración GitHub-Azure y seguridad en CI/CD.

### 🗄️ proyecto02-leagcy-file-servver-migration-azure
**Plataforma**: Azure File Sync / Híbrido

**Lo que implementé**:
- Storage Account + File Share (`companydata`) con rendimiento Transaction Optimized.
- Storage Sync Service (`sync-ws-files-001`) + Sync Group (`sg-companydata`).
- VM Windows Server 2022 en dominio con carpeta compartida `\\fileserver\CompanyData`.
- Azure File Sync Agent + Cloud Tiering.
- Sincronización bidireccional preservando permisos NTFS.

**Finalidad**:
Migración sin downtime de file servers legacy a Azure manteniendo experiencia SMB tradicional, escalabilidad cloud y optimización de almacenamiento local.

### 🛠️ proyecto06-migración Infra On-Premise a Azure
**Plataforma**: ASP.NET Core MVC + Azure DevOps

**Lo que implementé**:
- Aplicación web de reservas preparada para ejecutar localmente con configuración por entorno.
- Integración con RabbitMQ sin credenciales incrustadas.
- Servicio de correo tolerante a configuración vacía.
- Pipeline de Azure DevOps para compilar y publicar el proyecto.

**Finalidad**:
Base práctica para documentar y desplegar una migración desde una infraestructura on-premise hacia Azure.

---

## 🎯 Por qué estos proyectos

Cada laboratorio simula escenarios reales que resuelvo como Administrador Multicloud:

- App Services: automatización DevOps y despliegues seguros.
- File Sync: modernización híbrida sin impacto al negocio.
- AI Chatbot: IA controlada para casos empresariales reales.
- Migración Infra On-Premise a Azure: preparación de una app y su entrega por Azure DevOps.

---

## 📫 Contacto

Fabricio Castillo - Administrador Sistemas Multicloud
Madrid, España

- LinkedIn: https://www.linkedin.com/in/fabriciolcastilloleon
- Email: k.fabriciocastilloleon@gmail.com

---

⭐ Explora mis labs y abre issues con feedback. Seguiré creciendo este portfolio.
<<<<<<< HEAD
# 🌩️ Portfolio de Laboratorios Multicloud (Azure & AWS)

Bienvenido a mi portfolio de laboratorios orientados a **Cloud Computing**.  
Este repositorio reúne los proyectos y pruebas de concepto que realizo para **poner en práctica y consolidar** mis conocimientos en **Microsoft Azure** y **Amazon Web Services (AWS)**, con un enfoque claro hacia **administración y arquitectura Cloud**.

---

## 👨‍💻 Sobre mí

Soy un **Administrador Multicloud** con experiencia en:

- Administración de entornos virtualizados con **VMware vSphere**.  
- Gestión de **copias de seguridad** y **monitorización** con herramientas como **Zabbix**.  
- Soporte y colaboración con equipos de **Desarrollo** y **Marketing**.  

Trabajo con **Azure** y **AWS** optimizando entornos híbridos mediante herramientas nativas y **Infrastructure as Code**.

Mi objetivo: evolucionar hacia **Arquitecto Cloud** diseñando soluciones **seguras**, **escalables** y **automatizadas**.

---

## 📚 Laboratorios incluidos

### 🖥️ **lab01-AppServices** 
**Plataforma**: Azure App Service + GitHub Actions

**Lo que implementé**:
- Portfolio estático desplegado desde GitHub con **CI/CD automático**
- Workflow GitHub Actions simplificado para sitios estáticos
- **Identidad administrada** (User-assigned identity) para despliegue seguro
- Verificación en Kudu (wwwroot) y actualización automática al hacer push

**Finalidad**:
Pipeline **DevOps completo** para despliegue continuo de aplicaciones web con integración GitHub-Azure y seguridad en CI/CD.

---

### 🗄️ **proyecto02-leagcy-file-servver-migration-azure**
**Plataforma**: Azure File Sync / Híbrido

**Lo que implementé**:
- Storage Account + File Share (`companydata`) con rendimiento Transaction Optimized
- **Storage Sync Service** (`sync-ws-files-001`) + **Sync Group** (`sg-companydata`)
- VM Windows Server 2022 en dominio con carpeta compartida `\\fileserver\CompanyData`
- **Azure File Sync Agent** + **Cloud Tiering** (archivos fríos → Azure)
- Sincronización bidireccional preservando permisos NTFS

**Finalidad**:
**Migración sin downtime** de file servers legacy a Azure manteniendo experiencia SMB tradicional, escalabilidad cloud y optimización de almacenamiento local.

---

### 🤖 **proyecto04-azure-ai-private-chatbot**
**Plataforma**: Azure AI Foundry + RAG

**Lo que implementé**:
- PDFs de políticas RRHH en **Azure Blob Storage** (contenedor privado)
- **Azure AI Search** (`search-politicas-empresa`) con índice `idx-politicas`
- **Azure AI Foundry** con modelo **GPT-4o-mini** y conexión segura a AI Search
- Agente de chat con mensaje restrictivo: *"Solo responde con documentos conectados"*
- RAG para consultas semánticas sobre políticas internas

**Finalidad**:
**Asistente IA empresarial privado** que responde únicamente con documentación interna, eliminando alucinaciones y garantizando cumplimiento de datos.

---

## 🎯 Por qué estos proyectos

Cada laboratorio simula escenarios reales que resuelvo como **Administrador Multicloud**:

- **App Services**: Automatización DevOps y despliegues seguros
- **File Sync**: Modernización híbrida sin impacto al negocio  
- **AI Chatbot**: IA controlada para casos empresariales reales

---

## 📫 Contacto

**Fabricio Castillo** - Administrador Sistemas Multicloud  
**Madrid, España**  
- LinkedIn: https://www.linkedin.com/in/fabriciocastilloleon  
- Email: k.fabriciocastilloleon@gmail.com  

---

⭐ **¡Explora mis labs y abre issues con feedback!** Seguiré creciendo este portfolio.
=======
# Introduction 
TODO: Give a short introduction of your project. Let this section explain the objectives or the motivation behind this project. 

# Getting Started
TODO: Guide users through getting your code up and running on their own system. In this section you can talk about:
1.	Installation process
2.	Software dependencies
3.	Latest releases
4.	API references

# Build and Test
TODO: Describe and show how to build your code and run the tests. 

# Contribute
TODO: Explain how other users and developers can contribute to make your code better. 

If you want to learn more about creating good readme files then refer the following [guidelines](https://docs.microsoft.com/en-us/azure/devops/repos/git/create-a-readme?view=azure-devops). You can also seek inspiration from the below readme files:
- [ASP.NET Core](https://github.com/aspnet/Home)
- [Visual Studio Code](https://github.com/Microsoft/vscode)
- [Chakra Core](https://github.com/Microsoft/ChakraCore)
