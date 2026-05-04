graph TD
    subgraph Host ["💻 Physical Host (VMware Workstation)"]
        ESXi["<strong>ESXi Hypervisor</strong><br/>IP: 192.168.1.139"]
        
        subgraph Net ["🌐 Virtual Network: 192.168.1.0/24"]
            
            AD["<strong>VM 1: Active Directory</strong><br/>IP: 192.168.1.10<br/>DNS: template.local<br/>(Shares & Users)"]
            
            APP["<strong>VM 2: APP Server</strong><br/>IP: 192.168.1.20<br/>.NET + SQL Server<br/>(DB: reservas)"]
            
            DOCKER["<strong>VM 3: Ubuntu Server</strong><br/>IP: 192.168.1.30<br/>Docker: RabbitMQ"]
            
            VEEAM["<strong>VM 4: Veeam B&R</strong><br/>IP: 192.168.1.40<br/>Backup Management"]
            
        end
    end

    %% Relaciones de Servicio
    APP -- "1. Registro en DB" --> APP
    APP -- "2. Encola Mensajes" --> DOCKER
    
    %% Relaciones de Infraestructura
    APP -. "DNS Lookup" .-> AD
    DOCKER -. "DNS Lookup" .-> AD
    VEEAM -. "DNS Lookup" .-> AD
    
    %% Relaciones de Backup
    VEEAM -- "Gestiona Agente" --> AD
    AD -- "Copia de Seguridad" --> AD
    
    style ESXi fill:#f9f,stroke:#333,stroke-width:2px
    style AD fill:#e1f5fe,stroke:#01579b
    style APP fill:#e8f5e9,stroke:#2e7d32
    style DOCKER fill:#fff3e0,stroke:#ef6c00
    style VEEAM fill:#f3e5f5,stroke:#7b1fa2
