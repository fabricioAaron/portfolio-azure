# Laboratorios y proyectos Cloud · Fabricio Castillo

Repositorio donde documento mis laboratorios y proyectos personales orientados a entornos cloud y multicloud.  
Aquí concentro el trabajo práctico mi experiencia en el área de sistemas y acentando mis conocimientos de mi master Multicloud realizando proyectos en Azure.

## Sobre mí

- Profesional de Sistemas con background en ASIR y experiencia en soporte, administración y operación de infraestructuras.
- Estudiante de máster en entornos multicloud, con foco en buenas prácticas, automatización y despliegue.
- Certificado **Microsoft Azure Administrator Associate (AZ‑104)**, profundizando ahora en escenarios más prácticos y reales.
- Certificado ** Azure Security Engineer Associate (AZ‑500)**, profundizando ahora en escenarios más prácticos y reales.

Este repositorio nace con un objetivo claro: transformar conocimiento teórico en proyectos concretos que pueda explicar en una entrevista técnica o en una reunión con un equipo de ingeniería.

## Qué encontrarás en este repositorio

- **Labs de Azure**:  
  - App Service, Web Apps y despliegue de aplicaciones.  
  - Integración con GitHub / Azure DevOps y CI/CD.  
  - Uso de servicios PaaS y recursos básicos (grupos de recursos, redes, almacenamiento, identidades).



## Objetivo de estos proyectos

- Afianzar conceptos de administración y operación en Azure más allá de la preparación de certificaciones.
- Disponer de ejemplos reales que muestren cómo trabajo: cómo diseño, documento, despliego y mantengo servicios.
- Construir un portfolio técnico que complemente mi perfil de sistemas y mi trayectoria hacia roles Cloud / DevOps / SRE.

## Pasos a seguir 

- `lab-01-portfolio-azure-appservice/`  
  Portfolio estático (HTML/CSS) desplegado en Azure App Service, integrado con GitHub.

1. Crear una carpeta en tu PC para el proyecto, por ejemplo portfolio-azure.

   *Dentro, crear los archivos básicos:

      * index.html
      * styles.css

2. Inicializar Git en la carpeta del proyecto:

```
bash
git init
git branch -M main
```
3. Subir el repositorio a GitHub
Crear un nuevo repositorio vacío en GitHub (por ejemplo portfolio-azure).

```
Conectar el repositorio local con GitHub:
git remote add origin https://github.com/fabricioAaron/portfolio-azure.git
git push -u origin main
```
4.  Crear la Web App en Azure

  1. Buscar App Services → Create → Web App.
​

 <img width="886" height="855" alt="image" src="https://github.com/user-attachments/assets/d2f9938e-5113-465b-96fb-25e517def77d" />

5. Conectar la Web App con GitHub (Deployment Center)
    1. En la Web App, ir a Deployment Center.

    2. Elegir:

          * Source: GitHub.

          * Organización: tu cuenta.

          * Repositorio: portfolio-azure.

          * Branch: main.

 <img width="886" height="837" alt="image" src="https://github.com/user-attachments/assets/c7e61306-87f4-4995-93c9-ef4bb01b2c3b" />

 <img width="627" height="466" alt="image" src="https://github.com/user-attachments/assets/75cd5c57-e7d0-4ea3-88f1-ffc753ce2a79" />

6. En Authentication type, seleccionar User-assigned identity para usar identidad administrada segura.

    *Confirmar y guardar.

   *Azure crea automáticamente una identidad administrada y un archivo de workflow en .github/workflows/main_proyecto1.yml en el repositorio.

7. Ajustar el workflow de GitHub Actions

    * Abrir el archivo .github/workflows/main_proyecto1.yml en GitHub.

Simplificar el workflow para un sitio estático:

Mantener:

 *Paso Checkout (actions/checkout@v4).
  
  *Paso Login to Azure (azure/login@v2) con los valores de client-id, tenant-id y subscription-id que ha generado Azure.
  
  *Paso Deploy to Azure Web App (azure/webapps-deploy@v3) con:
  
       app-name: nombre de la Web App (ej. proyecto1).
  
       slot-name: Production.
  
       package: . (raíz del repositorio).
  
       Eliminar pasos de compilación .NET que no hagan falta (dotnet build, dotnet publish, etc.).

8. Comprobación de que los repositorios se han subido correctamente. 

  * Ingresamos a Develoment Tools > Advananced tooll
<img width="619" height="827" alt="image" src="https://github.com/user-attachments/assets/31a41677-50fc-419c-a55a-1f5c044e0fb6" />

Comprobaremos que se haya sincronizado correctamente nuestro repositorio en wwwroot
<img width="886" height="641" alt="image" src="https://github.com/user-attachments/assets/a1ae827b-b5af-4b63-beb7-25be29bd52b8" />
<img width="886" height="208" alt="image" src="https://github.com/user-attachments/assets/889142d9-6d7b-4557-bde1-04801b136778" />

9. Ingresamos en nuestro dashboard  y en defelult domain veremos nuestra página web. 

<img width="886" height="263" alt="image" src="https://github.com/user-attachments/assets/3591fe64-e244-4a45-abc0-4ed3f6faeb5b" />
<img width="1230" height="608" alt="image" src="https://github.com/user-attachments/assets/3dd03b62-5fe7-4bae-b450-e82f0bc50917" />

10. Comprobar que los cambios en el repositorio  se comprueben automaticamente en la pagina web de azure. 

<img width="886" height="509" alt="image" src="https://github.com/user-attachments/assets/6eac8177-a97a-4dec-a9c9-b29ea73775a2" />
<img width="500" height="119" alt="image" src="https://github.com/user-attachments/assets/f1013d6d-ba8b-434d-b32e-33a170fdebfa" />
<img width="886" height="290" alt="image" src="https://github.com/user-attachments/assets/94d3392a-df70-4968-8ae0-1a2911e53296" />







