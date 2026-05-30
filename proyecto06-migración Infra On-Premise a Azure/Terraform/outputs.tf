output "resource_group_name" {
  value       = azurerm_resource_group.rg.name
  description = "Nombre del grupo de recursos utilizado."
}

output "virtual_network_name" {
  value       = azurerm_virtual_network.vnet.name
  description = "Nombre de la red virtual (VNet) creada."
}

output "virtual_network_id" {
  value       = azurerm_virtual_network.vnet.id
  description = "ID de recurso de la red virtual."
}

output "app_service_default_hostname" {
  value       = azurerm_windows_web_app.app_service.default_hostname
  description = "Hostname público predeterminado del App Service."
}

output "app_service_plan_id" {
  value       = azurerm_service_plan.app_plan.id
  description = "ID del plan de App Service."
}
