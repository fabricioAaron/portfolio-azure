## Migración de la base de datos SQL on‑premise a Azure SQL Database

Esta sección describe cómo se migró la base de datos de reservas desde el servidor on‑premise (`APP` con SQL Server) a un servicio **Azure SQL Database**, manteniendo la aplicación .NET on‑premise
pero apuntando ahora a la base de datos en Azure.

### Paso 1: Copia de seguridad y exportación de la BD on‑premise

En el servidor `APP` (SQL Server):

1. Se realiza un **backup completo** de la base de datos `BD_APP` desde SQL Server Management Studio (SSMS) para tener un punto de recuperación previo a la migración.
2. A continuación, sobre la base de datos `BD_APP` se usa la opción  
   `Tasks → Extract Data-tier Application...` para generar un fichero **.bacpac** con el esquema y los datos.
3. El fichero resultante `DB_APP.bacpac` se guarda en disco local para utilizarlo después en Azure.

<img width="905" height="621" alt="backup onpremise-sql" src="https://github.com/user-attachments/assets/4821bd24-7860-4619-b76c-024ff731ee0c" />

`creamos un backup de la base de datos`

---

<img width="823" height="320" alt="exportal los datos bd " src="https://github.com/user-attachments/assets/dca2500c-45c5-483e-be01-9d1770ba741e" />

`exportamos la bd`

---

### Paso 2: Importar la base de datos a Azure SQL

En el portal de Azure:

1. Se ha creado previamente un **servidor lógico de Azure SQL** (por ejemplo `sqlsrv-reservas-lab`) dentro de un grupo de recursos de laboratorio (por ejemplo `az-lab-migration`).
2. En la página del servidor SQL se usa la opción **Import database**.
3. En el asistente de importación se selecciona el archivo `.bacpac` que se ha subido a una cuenta de almacenamiento de Azure (por ejemplo contenedor `backup-db` con el archivo `DB_APP.bacpac`).
4. Se define:
   - Nombre de la base de datos en Azure: `db-reservas`.
   - Nivel de servicio: `Standard S0` (10 DTU, 250 GB de almacenamiento), suficiente para este laboratorio.
   - Credenciales del administrador de la base de datos en Azure SQL.
5. Tras confirmar, Azure crea la base de datos `db-reservas` a partir del .bacpac.

<img width="792" height="746" alt="miragar sql server - servidor" src="https://github.com/user-attachments/assets/729f8279-73a0-4743-91ec-59d82d9960b9" />

`cremos un sql server`

---

<img width="1230" height="265" alt="sqlserver-importarbd" src="https://github.com/user-attachments/assets/ffa008de-f301-4481-8433-d747d18888dd" />

`importamos la BD`

---

<img width="636" height="755" alt="importacion de la bd " src="https://github.com/user-attachments/assets/ca1ae4cb-b2a0-4901-94e0-17d1191037b8" />

`para importal la bd crearemos un sotrage para subir .bacpac  `

---

### Paso 3: Verificación de los datos en Azure SQL

Una vez completada la importación:

1. Desde el portal de Azure, en la base de datos `db-reservas`, se abre el editor de consultas (Query editor).  
2. Se ejecuta la consulta:

   ```sql
   SELECT * FROM Reservas;
   ```

<img width="1600" height="799" alt="migrado la bd" src="https://github.com/user-attachments/assets/e63ea468-01e6-4a96-8de1-451598a04765" />

`comprobación de la importación fue correctamente`

---

3. En los resultados se comprueban todas las filas migradas, incluyendo nuevas filas de prueba creadas para validar la conexión (por ejemplo registros con nombres `azure-sql` o similares).

Con esto se confirma que la tabla `Reservas` y sus datos están correctamente migrados a Azure SQL Database.

### Paso 4: Actualizar la aplicación .NET para usar Azure SQL

Por último, se modifica la configuración de la aplicación .NET (servidor `APP`) para apuntar a la nueva base de datos en Azure:

1. En el archivo de configuración (por ejemplo `appsettings.json` o `web.config`, según el tipo de proyecto) se actualiza la **cadena de conexión** para que apunte a:

   - Servidor: `server-reservas.database.windows.net`
   - Base de datos: `db-reservas`.
   - Usuario y contraseña definidos como administrador de la BD en Azure.
   - Cifrado y TrustServerCertificate según las recomendaciones de Azure.

<img width="1058" height="271" alt="imagen" src="https://github.com/user-attachments/assets/72640f92-3d3c-4ebe-b5f5-2a9c22a88740" />

`en nuestro .net apuntamos el endpoint de nuestro azure sql, esto en nuestro app services lo pondremos en una variable de entorno`

---

<img width="1234" height="601" alt="migracionbdenazure y app en onpremise" src="https://github.com/user-attachments/assets/fd17e55b-5bbe-4370-b5f2-f42a7c8098c2" />

`en nuestro formulario onpremise mandamos el formulario y comprobaremos que se va almacenar en nuestro azure sql `

---

<img width="1377" height="730" alt="comprobacionque sql escucha bien " src="https://github.com/user-attachments/assets/b5c1f2c1-4b00-48a7-a1b0-6b0b82be6b1d" />

`comprobación`

---

2. Se reinicia la aplicación y se realizan nuevas reservas desde el formulario web.  
3. Se verifica que:

   - Las nuevas reservas aparecen en el **Panel de reservas** de la aplicación, igual que antes.
   - Los registros también se ven en la consulta `SELECT * FROM Reservas` ejecutada contra la base de datos `db-reservas` en Azure SQL (confirmando que la app ya escribe directamente en Azure).

De esta forma, la aplicación sigue ejecutándose on‑premise en el servidor `APP`, pero la **capa de datos** se ha movido a Azure SQL Database, lo que permite aprovechar características de plataforma gestionada (backup automático, alta disponibilidad gestionada, escalado, etc.) sin cambiar el código principal de la aplicación.
