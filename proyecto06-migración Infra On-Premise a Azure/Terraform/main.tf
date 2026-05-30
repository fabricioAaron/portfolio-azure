
# 1. CREACIÓN DEL GRUPO DE RECURSOS 

resource "azurerm_resource_group" "rg" {
  name     = var.resource_group_name
  location = var.location
  tags     = var.tags
}

# 2. RED VIRTUAL 

resource "azurerm_virtual_network" "vnet" {
  name                = "vnet-migration"
  address_space       = ["10.0.0.0/16"]
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  tags                = var.tags
}

# Subred App Service
resource "azurerm_subnet" "subnet_appservice" {
  name                 = "subnet-appservice"
  resource_group_name  = azurerm_resource_group.rg.name
  virtual_network_name = azurerm_virtual_network.vnet.name
  address_prefixes     = ["10.0.1.0/24"]

  delegation {
    name = "appservice-delegation"
    service_delegation {
      name    = "Microsoft.Web/serverFarms"
      actions = ["Microsoft.Network/virtualNetworks/subnets/action"]
    }
  }
}


# 3. CREACIÓN DEL APP SERVICE 

resource "azurerm_service_plan" "app_plan" {
  name                = var.app_service_plan_name
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  os_type             = "Windows"
  sku_name            = "B1" 
  tags                = var.tags
}

resource "azurerm_windows_web_app" "app_service" {
  name                = var.app_service_name
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  service_plan_id     = azurerm_service_plan.app_plan.id
  tags                = var.tags

  site_config {
    always_on = false
    
    application_stack {
      current_stack  = "dotnet"
      dotnet_version = "v10.0" # Framework .NET 10 LTS
    }
  }
}

# Conexión para habilitar la integración regional de VNet para la salida del App Service
resource "azurerm_app_service_virtual_network_swift_connection" "vnet_integration" {
  app_service_id = azurerm_windows_web_app.app_service.id
  subnet_id      = azurerm_subnet.subnet_appservice.id
}
