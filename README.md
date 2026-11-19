# Seguros LAFISE S.A. — API de Emisión de Pólizas de Auto

Prototipo funcional en **ASP.NET Core Web API (.NET 10)** para la emisión, consulta e historial de pólizas de seguros de automóviles.

---

## Stack y Arquitectura

* **Framework:** .NET 10 (`ASP.NET Core Web API`)
* **ORM y Base de Datos:** Entity Framework Core (Code First) + SQL Server
* **Arquitectura en Capas:**
  * `Controllers`: Exposición de endpoints REST y respuestas HTTP semánticas (`200`, `201`, `204`, `400`, `404`, `409`).
  * `Services`: Lógica de negocio, cálculo de prima en servidor y validaciones de dominio.
  * `Repositories` + `UnitOfWork`: Acceso a datos desacoplado con EF Core.
  * `Exceptions`: Manejo centralizado de errores bajo el estándar RFC 9457 (`ProblemDetails`).

---

## Requisitos Previos

* [.NET 10 SDK](https://dotnet.microsoft.com/download)
* SQL Server (LocalDB, Express o instancia local)
* Herramienta CLI de EF Core (`dotnet tool install --global dotnet-ef`, si se usa la Opción A)

---

## Configuración y Base de Datos

1. **Configurar la cadena de conexión** en `prueba-tecnica-api/appsettings.json` (por defecto apunta a `Server=localhost;Database=prueba-slni;...`; ajústala según tu instancia, por ejemplo `(localdb)\mssqllocaldb`, `localhost\SQLEXPRESS` o `localhost,PUERTO`):

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=prueba-slni;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

> **Nota sobre `User Secrets` en entorno `Development`:** El proyecto tiene habilitado `UserSecretsId`. Si configuras la cadena mediante `dotnet user-secrets set "ConnectionStrings:DefaultConnection" "..." --project ./prueba-tecnica-api/prueba-tecnica-api.csproj`, ese valor tendrá prioridad sobre `appsettings.json` cuando se ejecute en modo `Development`.

2. **Crear la base de datos** (elige una de las dos opciones):

* **Opción A — Migraciones de EF Core (Recomendado):**
  Desde la raíz del repositorio, restaura los paquetes y aplica las migraciones:
  ```bash
  dotnet restore ./prueba-tecnica-api/prueba-tecnica-api.csproj
  dotnet ef database update --project ./prueba-tecnica-api/prueba-tecnica-api.csproj
  ```

* **Opción B — Script SQL:**
  Crea primero la base de datos vacía (por ejemplo `prueba-slni`) y ejecuta el archivo [`database.sql`](./database.sql) sobre ella en SQL Server Management Studio (SSMS) o con `sqlcmd`:
  ```bash
  sqlcmd -S localhost -Q "IF DB_ID(N'prueba-slni') IS NULL CREATE DATABASE [prueba-slni];" -C -b
  sqlcmd -S localhost -d prueba-slni -i ./database.sql -C -b
  ```

> **Nota:** La base de datos se inicializa automáticamente con datos semilla (*Seed Data*):
> * **Clientes:** `Id: 1` (María Fernanda López), `Id: 2` (Carlos Eduardo Martínez).
> * **Coberturas:** `Id: 1` (Robo - `3.50%`), `Id: 2` (Choque - `4.50%`), `Id: 3` (Responsabilidad Civil - `2.00%`).

---

## Ejecución de la API

Desde la raíz del repositorio, ejecuta:

```bash
dotnet run --project ./prueba-tecnica-api/prueba-tecnica-api.csproj
```

* **Swagger UI (HTTP por defecto):** `http://localhost:5277/swagger`
* **Swagger UI con perfil HTTPS:** ejecuta `dotnet run --project ./prueba-tecnica-api/prueba-tecnica-api.csproj --launch-profile https` y abre `https://localhost:7063/swagger` (o `http://localhost:5277/swagger`).
* **Colección de Postman:** Importa el archivo [`SegurosLafise.postman_collection.json`](./SegurosLafise.postman_collection.json) incluido en la raíz del proyecto.

---

## Endpoints Principales

| Método | Ruta | Descripción |
| :--- | :--- | :--- |
| `GET` | `/api/clientes` | Lista el catálogo de clientes registrados. |
| `POST` | `/api/clientes` | Registra un nuevo cliente (`GET/{id}`, `PUT/{id}` y `DELETE/{id}` también disponibles). |
| `GET` | `/api/coberturas` | Lista el catálogo de coberturas disponibles y sus tasas (%). |
| `POST` | `/api/polizas/emitir` | **Emisión principal:** Valida reglas de negocio, calcula la prima total en el servidor y genera la póliza. |
| `GET` | `/api/polizas/{id}` | Obtiene el detalle de una póliza emitida por su ID. |
| `GET` | `/api/polizas` | Lista el historial completo de pólizas emitidas. |
| `PATCH` | `/api/polizas/{id}/anular` | Anula lógicamente una póliza activa. |

### Ejemplo de Emisión (`POST /api/polizas/emitir`)

**Request Body:**
```json
{
  "clienteId": 1,
  "vehiculo": {
    "placa": "M123456",
    "marca": "Toyota",
    "modelo": "Corolla",
    "anio": 2022,
    "valorComercial": 18500.00
  },
  "coberturaIds": [1, 2, 3]
}
```

---

## Reglas de Negocio Implementadas

1. **Antigüedad máxima:** No se permite emitir pólizas para vehículos con más de **20 años** de antigüedad (`400 Bad Request`).
2. **Validación de placa y valor comercial:** La placa es obligatoria y debe cumplir un formato alfanumérico válido, ej. `M123456` o `ABC-123`; el valor comercial admite como máximo 2 decimales (`400 Bad Request`).
3. **Unicidad de póliza activa por placa:** Un vehículo no puede tener dos pólizas activas simultáneamente, incluso variando el guion opcional (`ABC-123` vs `ABC123`) (`409 Conflict`), garantizado tanto en la capa de servicio como mediante índices únicos en SQL Server.
4. **Manejo estandarizado de errores:** Todas las respuestas de error (`400`, `404`, `409`, `500`) siguen el estándar RFC 9457 (`ProblemDetails`) en español e incluyen `correlationId`.
