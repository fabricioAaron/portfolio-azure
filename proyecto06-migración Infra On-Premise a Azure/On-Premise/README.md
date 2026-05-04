# 🏗️  Infraestructura On-Premise

<img width="2816" height="1536" alt="Gemini_Generated_Image_8vwqht8vwqht8vwq (1)" src="https://github.com/user-attachments/assets/9f63fc77-5769-4ae8-b9bb-db3c1f96f40c" />

# Esquema 

| Nombre de la Máquina | Dirección IP     | Sistema Operativo        | Roles / Servicios Instalados                                                                 | Función en la Infraestructura                                      |
|----------------------|------------------|---------------------------|------------------------------------------------------------------------------------------------|---------------------------------------------------------------------|
| ESXi Hypervisor      | 192.168.1.139    | VMware ESXi (nested)     | Hipervisor, 2 datastores, red interna 192.168.1.0/24                                           | Plataforma de virtualización donde corren las 4 VMs                 |
| AD                   | 192.168.1.10     | Windows Server            | Active Directory Domain Services, DNS, Carpeta compartida `\\AD\shares`, Usuarios y grupos     | Controlador de dominio, DNS, recursos compartidos                   |
| APP                  | 192.168.1.20     | Windows Server            | Aplicación web .NET, SQL Server (BD reservas), integración con RabbitMQ                        | Servidor de aplicación y base de datos                              |
| RabbitMQ / Docker    | 192.168.1.30     | Ubuntu Server + Docker    | Docker Engine, Contenedor RabbitMQ (build propio con Dockerfile)                               | Mensajería asíncrona, colas para la aplicación                      |
| Veeam Backup Server  | 192.168.1.40     | Windows Server            | Veeam Backup & Replication, conexión a ESXi, agente Veeam en AD, repositorio local             | Backup de AD, shares, restauración granular, protección de datos    |



