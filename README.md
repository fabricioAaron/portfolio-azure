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
