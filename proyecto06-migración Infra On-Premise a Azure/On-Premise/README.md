<img width="2816" height="1536" alt="Gemini_Generated_Image_8vwqht8vwqht8vwq (1)" src="https://github.com/user-attachments/assets/9f63fc77-5769-4ae8-b9bb-db3c1f96f40c" />

Ç
# 🏗️ Laboratorio de Infraestructura Híbrida (Nested Virtualization)

Este proyecto consiste en un entorno de laboratorio completo montado sobre una arquitectura de virtualización anidada (**Nested Virtualization**). La base principal es **VMware Workstation**, que aloja un hipervisor **ESXi**, el cual gestiona una red de servidores Windows y Linux para servicios de identidad, aplicaciones .NET, mensajería con RabbitMQ y protección de datos con Veeam.

## 📊 Esquema de la Infraestructura

```mermaid
graph TD
    subgraph F_Host ["💻 Host Físico (VMware Workstation)"]
        ESXi["<strong>Hipervisor ESXi</strong><br/>IP: 192.168.1.139"]
        
        subgraph V_Net ["🌐 Red Virtual (192.168.1.0/24)"]
            
            AD["🗄️ <strong>VM 1: Active Directory</strong><br/>IP: 192.168.1.10<br/>Dominio: template.local<br/>DNS | File Shares"]
            
            APP["🌐 <strong>VM 2: APP Server</strong><br/>IP: 192.168.1.20<br/>Stack: .NET + SQL Server<br/>DB: reservas"]
            
            DOCKER["🐳 <strong>VM 3: Ubuntu Server</strong><br/>IP: 192.168.1.30<br/>Docker: RabbitMQ"]
            
            VEEAM["🛡️ <strong>VM 4: Veeam B&R</strong><br/>IP: 192.168.1.40<br/>Backup Management"]
            
        end
    end

    %% Relaciones de Aplicación
    APP -- "1. Persistencia SQL" --> APP
    APP -- "2. Produce Mensaje" --> DOCKER
    
    %% Relaciones de Dominio
    APP -. "Join Domain" .-> AD
    VEEAM -. "Join Domain" .-> AD
    
    %% Relaciones de Backup
    VEEAM -- "Gestiona Agente" --> AD
    AD -- "Copia de Seguridad" --> AD
    
    style ESXi fill:#f5f5f5,stroke:#333,stroke-width:2px
    style AD fill:#e1f5fe,stroke:#01579b
    style APP fill:#e8f5e9,stroke:#2e7d32
    style DOCKER fill:#fff3e0,stroke:#ef6c00
    style VEEAM fill:#f3e5f5,stroke:#7b1fa2