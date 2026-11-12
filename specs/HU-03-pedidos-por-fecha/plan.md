# Plan HU-03 · Pedidos por rango de fechas

## Enfoque
Un filtro pequeño y puro en Core. No toca la API, `IPedidoRepositorio` ni los pedidos de ejemplo (fuera de alcance de la spec).

## Diseño
**Core** (`src/PedidosWeb.Core/Pedidos/`)
- `FiltroPedidos` ya existe (el record de HU-01) y no se modifica. Por eso la operación va en una clase estática nueva: `PedidosPorFecha`.
- `PedidosPorFecha.EntreFechas(IEnumerable<Pedido> pedidos, DateOnly desde, DateOnly hasta)` devuelve los pedidos con `desde <= Fecha <= hasta`, en el mismo orden en que llegan.
- Si `desde > hasta`, lanza `ArgumentException` con un mensaje que explique el problema.

## Pasos (con aprobación en cada uno)
1. Stub en Core que compile, con `NotImplementedException`.
2. Un test xUnit por criterio en `tests/PedidosWeb.Core.Tests/Pedidos/PedidosPorFechaTests.cs`. Ver que fallan (rojo).
3. Implementar `EntreFechas` hasta ponerlos en verde.
4. `dotnet build` y `dotnet test`.
5. Correr el subagente `revisor`, abrir el PR y redactar su descripción.

## Trazabilidad: criterio → test
| Criterio | Test |
|---|---|
| CA-01 · fecha igual a `desde` aparece | `EntreFechas_PedidoConFechaDesde_Aparece` |
| CA-02 · fecha igual a `hasta` aparece | `EntreFechas_PedidoConFechaHasta_Aparece` |
| CA-03 · fuera del rango no aparece | `EntreFechas_PedidoFueraDelRango_NoAparece` |
| CA-04 · `desde` mayor que `hasta` lanza `ArgumentException` | `EntreFechas_DesdeMayorQueHasta_LanzaArgumentException` |
