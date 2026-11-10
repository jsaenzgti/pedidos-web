# Plan HU-01 · Listar pedidos

## Enfoque
La lógica (parseo, validación, filtro, orden, paginación) va en Core; la Api solo enlaza el endpoint y traduce errores a `ProblemDetails`. `IPedidoRepositorio` no cambia: el servicio filtra sobre `ObtenerTodos()`.

## Diseño
**Core** (`src/PedidosWeb.Core/Pedidos/`)
- `EstadosPedido`: los 5 estados válidos (`Pendiente`, `Confirmado`, `Despachado`, `Entregado`, `Anulado`).
- `FiltroPedidos` (record): filtro ya validado y con tipos reales: `ClienteId` (con trim), `Estado?`, `Desde?` (`DateOnly?`), `Hasta?` (`DateOnly?`), `Pagina`, `TamanoPagina`.
  Se crea con `FiltroPedidos.Crear(string? clienteId, string? estado, string? desde, string? hasta, string? pagina, string? tamanoPagina)`, que valida los strings y devuelve el filtro o los errores.
- `ResultadoPaginado<T>` (record): `Items`, `Total`, `Pagina`, `TamanoPagina`.
- `ListarPedidosServicio`: recibe `IPedidoRepositorio`.
  - `Listar(strings...)` crea el filtro y, si es válido, consulta. Devuelve `ResultadoListado`: éxito con `ResultadoPaginado<Pedido>` o errores `IReadOnlyDictionary<string, string[]>`.
  - La consulta aplica, en este orden: `clienteId` y `estado` sin distinguir mayúsculas, rango inclusivo, `OrderBy(Fecha).ThenBy(Id)`, `Total` y luego `Skip/Take`.
- Reglas de validación:
  - `clienteId` obligatorio (vacío o solo espacios es error).
  - `estado` dentro de `EstadosPedido`, `OrdinalIgnoreCase`.
  - Fechas con `DateOnly.TryParseExact("yyyy-MM-dd", InvariantCulture)`; `desde` posterior a `hasta` es error en `desde`.
  - `pagina` entero ≥ 1 (por defecto 1); `tamanoPagina` entero de 1 a 50 (por defecto 20). Si no son numéricos, error en el campo.
  - Se devuelven todos los errores, uno por campo.
- Una `pagina` mayor que la última no es error: devuelve `items` vacío.

**Api** (`src/PedidosWeb.Api/Program.cs`)
- Registrar `ListarPedidosServicio` (singleton, como el repositorio).
- `GET /api/pedidos` recibe los 6 parámetros como `string?` y delega en el servicio. Éxito → `Results.Ok`; errores → `Results.ValidationProblem(errores)` (400, `ProblemDetails`).
- JSON camelCase (default de ASP.NET Core). No se toca `GET /api/pedidos/{id}`.

## Pasos (en este orden, con aprobación en cada uno)
1. Crear stubs en Core para que compile (`EstadosPedido`, `FiltroPedidos`, `ResultadoPaginado`, `ListarPedidosServicio`), con `NotImplementedException`.
2. Escribir tests de Core en `tests/PedidosWeb.Core.Tests/Pedidos/ListarPedidosServicioTests.cs`: un test por criterio (tabla de abajo). Ver que fallan (rojo).
3. Implementar `FiltroPedidos.Crear` y `ListarPedidosServicio` hasta ponerlos en verde.
4. Crear `tests/PedidosWeb.Api.Tests` (xUnit + `Microsoft.AspNetCore.Mvc.Testing`) y agregarlo a `PedidosWeb.sln`. **Aprobado** (proyecto nuevo y paquete). `Program` ya es `partial`.
5. Escribir los 4 tests de Api (tabla de abajo) con `WebApplicationFactory<Program>`. Ver rojo.
6. Registrar el servicio y mapear `GET /api/pedidos` en `Program.cs`. Ver verde.
7. `dotnet build`, `dotnet test` y prueba manual con `dotnet run --project src/PedidosWeb.Api`.
8. Commit solo con tests en verde y con aprobación.

## Trazabilidad: criterio → test
Los 19 criterios de la spec, en su orden, con un test de Core cada uno (archivo `ListarPedidosServicioTests`, nombre `Metodo_Escenario_Resultado`).

| # | Criterio (resumen) | Test de Core |
|---|---|---|
| 1 | C-001 → total 5, ids 40, 32, 24, 16, 8 | `Listar_ClienteC001_DevuelveCincoPedidosOrdenadosPorFechaEId` |
| 2 | C-002 → total 6, ids 13, 5, 45, 37, 29, 21 | `Listar_ClienteC002_DevuelveSeisPedidosOrdenados` |
| 3 | C-001 y `estado=Entregado` → 40, 24, 8 | `Listar_ClienteYEstado_DevuelveSoloEseEstado` |
| 4 | `estado=entregado` igual que el anterior | `Listar_EstadoEnMinuscula_NoDistingueMayusculas` |
| 5 | rango 2026-09-14 a 2026-09-26 → 32, 24 | `Listar_RangoDeFechas_IncluyeAmbosExtremos` |
| 6 | solo `desde=2026-10-08` → 16, 8 | `Listar_SoloDesde_FiltraDesdeEsaFecha` |
| 7 | C-002 y `Anulado` → 29; sin estado lo incluye | `Listar_PedidoAnulado_AparecePorDefectoYFiltrando` |
| 8 | C-999 → `items` vacío, total 0 | `Listar_ClienteInexistente_DevuelveListaVacia` |
| 9 | `tamanoPagina=2&pagina=2` → 24, 16, total 5 | `Listar_PaginaDos_DevuelveSegundaPagina` |
| 10 | defaults pagina 1 y tamanoPagina 20 | `Listar_SinPaginacion_UsaPagina1Y20PorPagina` |
| 11 | sin `clienteId` → error en `clienteId` | `Listar_SinClienteId_DevuelveErrorEnClienteId` |
| 12 | `estado=Cancelado` → error en `estado` | `Listar_EstadoInvalido_DevuelveErrorEnEstado` |
| 13 | desde 2026-10-31, hasta 2026-09-01 → error en `desde` | `Listar_FechasInvertidas_DevuelveErrorEnDesde` |
| 14 | `desde=31-10-2026` → error en `desde` | `Listar_FormatoDeFechaIncorrecto_DevuelveErrorEnDesde` |
| 15 | `clienteId=c-001` → los mismos 5 que `C-001` | `Listar_ClienteEnMinuscula_NoDistingueMayusculas` |
| 16 | `pagina=9` → `items` vacío, total 5 | `Listar_PaginaMayorQueLaUltima_DevuelveItemsVacio` |
| 17 | varios inválidos → 3 errores a la vez | `Listar_VariosCamposInvalidos_DevuelveTodosLosErrores` |
| 18 | `tamanoPagina` 0 o 51, `pagina=0` → error | `Listar_PaginacionFueraDeRango_DevuelveErrorEnElCampo` |
| 19 | `pagina=abc` o `tamanoPagina=abc` → error | `Listar_PaginacionNoNumerica_DevuelveErrorEnElCampo` |

**Tests mínimos de Api** (`tests/PedidosWeb.Api.Tests`, solo lo que Core no puede probar):

| Test de Api | Qué verifica |
|---|---|
| `Get_ListadoValido_Devuelve200ConJsonCamelCase` | ruta enlazada, servicio registrado, nombres `items`, `total`, `pagina`, `tamanoPagina`, `clienteNombre` |
| `Get_ConParametrosDePaginacion_LosEnlazaAlServicio` | los nombres `tamanoPagina` y `pagina` llegan bien desde la query |
| `Get_SinClienteId_Devuelve400ProblemDetails` | 400 con `ProblemDetails` y `errors.clienteId` |
| `Get_VariosErrores_Devuelve400ConUnErrorPorCampo` | `errors` trae todos los campos inválidos |

## Riesgos y decisiones
- Parsear strings en Core permite probar el formato de fecha sin HTTP; el costo es una firma con strings. Alternativa descartada: enlazar tipos en la Api (`DateOnly?`, `int?`), que dejaría los errores de formato sin control propio.
- Proyecto de tests de Api y paquete NuGet: cambio de dependencias, aprobado.
- `AGENTS.md` ya fue actualizado para reflejar el servicio de Core y el segundo proyecto de tests.

## Verificación
- `dotnet test`: 60 tests en verde. En `Core.Tests` son 56: los 19 criterios de HU-01 (22 casos) y los 34 anteriores. En `Api.Tests` son 4.
- `GET http://localhost:5080/api/pedidos?clienteId=C-001` → 200, `total` 5, ids 40, 32, 24, 16, 8.
- `GET /api/pedidos` sin `clienteId` → 400 `ProblemDetails`.
