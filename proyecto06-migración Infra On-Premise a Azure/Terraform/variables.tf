variable "resource_group_name" {
  type        = string
  description = "Nombre del grupo de recursos en Azure."
  default     = "lab-migration-onpremise"
}

variable "location" {
  type        = string
  description = "Región de Azure donde se desplegarán los recursos."
  default     = "spaincentral"
}

variable "tags" {
  type        = map(string)
  description = "Etiquetas aplicadas a todos los recursos."
  default = {
    departamento = "it"
  }
}

variable "app_service_plan_name" {
  type        = string
  description = "Nombre del App Service Plan."
  default     = "asp-labmigrationonpremise-9657"
}

variable "app_service_name" {
  type        = string
  description = "Nombre único de la Web App en Azure App Service."
  default     = "Ayacucho-Aventura"
}

variable "subscription_id" {
  type        = string
  description = "ID de la suscripción de Azure para completar opcionalmente las rutas de importación de recursos."
  default     = "00000000-0000-0000-0000-000000000000"
}
