# AGENTS.md

## Proyecto
Portal web "Pedidos": modernización de un sistema de escritorio en Delphi (legado).
API REST en ASP.NET Core (.NET 10) con datos de ejemplo en memoria (sin base de datos).

## Comandos
- Compilar: `dotnet build`
- Testear: `dotnet test`
- Ejecutar la API: `dotnet run --project src/PedidosWeb.Api` (http://localhost:5080)
- Comprobar que está arriba: `GET /salud`

## Estilo de código
- Nombres de dominio en español (`Pedido`, `Sucursal`, `ObtenerPorId`); código C# estándar.
- Dinero: `decimal` en CLP, sin decimales. Nunca `double` ni `float`.
- Fechas: `DateOnly` (sin hora).
- `src/PedidosWeb.Core`: lógica de dominio y repositorios. `src/PedidosWeb.Api`: solo
  endpoints (Minimal API); sin lógica de negocio.
- Un endpoint nuevo depende de un servicio de Core (que usa `IPedidoRepositorio`), nunca de la clase en memoria.

## Tests
- xUnit en `tests/`: la lógica en `PedidosWeb.Core.Tests` (espejo de `src/`); pruebas HTTP mínimas en `PedidosWeb.Api.Tests`.
- Nombre: `Metodo_Escenario_Resultado`.
- Se ejecutan con `dotnet test` antes de cada commit.

## Seguridad
- Nunca secretos, claves ni cadenas de conexión en el código.

## Reglas de trabajo
- Toda funcionalidad parte de una spec aprobada en `specs/<historia>/spec.md`.
- Flujo: spec aprobada → plan.md → tests que fallan → código, con aprobación en cada paso.
- Un test xUnit por criterio de aceptación.
- No hacer commit sin tests en verde.
- Sin pedir permiso solo se puede leer, compilar y testear. Editar archivos, commit y
  push requieren aprobación previa.
