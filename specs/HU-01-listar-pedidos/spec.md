# HU-01 · Listar pedidos

## Historia de usuario
Como vendedor, quiero listar pedidos filtrando por cliente, estado y rango de fechas.

## Objetivo
Exponer `GET /api/pedidos` que devuelve, de forma paginada, los pedidos de un cliente, con
filtros opcionales por estado y rango de fechas, ordenados por fecha ascendente y luego por Id.

## Reglas de negocio
- `clienteId` es obligatorio. `estado`, `desde`, `hasta`, `pagina` y `tamanoPagina` son opcionales
  y los filtros se combinan con AND.
- Parámetros por query string: `clienteId`, `estado`, `desde`, `hasta` (fechas `yyyy-MM-dd`),
  `pagina`, `tamanoPagina`.
- `clienteId` que no existe en los datos no es un error: responde 200 con lista vacía.
- Estados válidos para filtrar: `Pendiente`, `Confirmado`, `Despachado`, `Entregado`, `Anulado`.
  La comparación no distingue mayúsculas (`entregado` es válido). Cualquier otro texto es error.
- Sin filtro de estado se devuelven todos los estados, incluido `Anulado`.
- `desde` y `hasta` son inclusivos: se cumple `Fecha >= desde` y `Fecha <= hasta`.
  Se puede enviar solo uno de los dos.
- `desde` posterior a `hasta` es un error de validación (no se intercambian ni se devuelve vacío).
- Orden: `Fecha` ascendente, desempate por `Id` ascendente.
- Paginación: `pagina` empieza en 1 (por defecto 1); `tamanoPagina` por defecto 20, máximo 50.
  `pagina` o `tamanoPagina` no numéricos son un error de validación.
- Una `pagina` mayor que la última no es error: responde 200 con `items` vacío.
- `clienteId` se compara sin distinguir mayúsculas y después de quitar espacios al inicio y al final.
- Respuesta 200: `{ items, total, pagina, tamanoPagina }`, donde `total` es la cantidad de
  pedidos que cumplen los filtros, sin paginar. Los nombres JSON van en camelCase.
- Los errores de validación responden 400 con `ProblemDetails` (RFC 9457), con el detalle por
  campo en `errors`.
- Si hay varios campos inválidos en la misma petición, `errors` devuelve todos, uno por campo.

## Criterios de aceptación
- [ ] `GET /api/pedidos?clienteId=C-001` devuelve 200 con `total` = 5 y los pedidos Id 40, 32, 24, 16, 8
      en ese orden (fecha ascendente).
- [ ] `GET /api/pedidos?clienteId=C-002` devuelve `total` = 6 y los Id 13, 5, 45, 37, 29, 21 en ese orden.
- [ ] `GET /api/pedidos?clienteId=C-001&estado=Entregado` devuelve los Id 40, 24, 8.
- [ ] `GET /api/pedidos?clienteId=C-001&estado=entregado` devuelve lo mismo (sin distinguir mayúsculas).
- [ ] `GET /api/pedidos?clienteId=C-001&desde=2026-09-14&hasta=2026-09-26` devuelve los Id 32 y 24
      (ambos extremos incluidos).
- [ ] `GET /api/pedidos?clienteId=C-001&desde=2026-10-08` devuelve los Id 16 y 8 (solo `desde`).
- [ ] `GET /api/pedidos?clienteId=C-002&estado=Anulado` devuelve el Id 29, y
      `GET /api/pedidos?clienteId=C-002` incluye el Id 29 sin filtrar por estado.
- [ ] `GET /api/pedidos?clienteId=C-999` devuelve 200 con `items` vacío y `total` = 0.
- [ ] `GET /api/pedidos?clienteId=C-001&tamanoPagina=2&pagina=2` devuelve los Id 24 y 16, con `total` = 5.
- [ ] `GET /api/pedidos?clienteId=C-001` usa `pagina` = 1 y `tamanoPagina` = 20 por defecto.
- [ ] Sin `clienteId` (o vacío) responde 400 `ProblemDetails` con error en `clienteId`.
- [ ] `estado=Cancelado` (no válido) responde 400 con error en `estado`.
- [ ] `desde=2026-10-31&hasta=2026-09-01` responde 400 con error de fechas invertidas.
- [ ] `desde=31-10-2026` (formato incorrecto) responde 400 con error en `desde`.
- [ ] `GET /api/pedidos?clienteId=c-001` (minúscula) devuelve los mismos 5 pedidos que `C-001`.
- [ ] `GET /api/pedidos?clienteId=C-001&pagina=9` devuelve 200 con `items` vacío y `total` = 5.
- [ ] `GET /api/pedidos?estado=Cancelado&desde=hoy` (sin `clienteId`, estado inválido, fecha mal formada)
      responde 400 con `errors` para `clienteId`, `estado` y `desde` a la vez.
- [ ] `tamanoPagina=51` o `tamanoPagina=0` o `pagina=0` responde 400 con error en el parámetro.
- [ ] `tamanoPagina=abc` o `pagina=abc` responde 400 con error en el parámetro.

## Ejemplos de entrada y salida
- Entrada: `GET /api/pedidos?clienteId=C-001&desde=2026-09-14&hasta=2026-09-26`
  Salida: 200
  ```json
  {
    "items": [
      { "id": 32, "folio": "PED-STGO-000032", "clienteId": "C-001", "clienteNombre": "Ferretería El Clavo",
        "fecha": "2026-09-14", "estado": "Confirmado", "total": 91900, "sucursal": "STGO" },
      { "id": 24, "folio": "PED-STGO-000024", "clienteId": "C-001", "clienteNombre": "Ferretería El Clavo",
        "fecha": "2026-09-26", "estado": "Entregado", "total": 101300, "sucursal": "STGO" }
    ],
    "total": 2, "pagina": 1, "tamanoPagina": 20
  }
  ```
- Entrada: `GET /api/pedidos?clienteId=C-001&desde=2026-10-31&hasta=2026-09-01`
  Salida: 400 `ProblemDetails`, `errors` con el mensaje de fechas invertidas.
- Entrada: `GET /api/pedidos?estado=Entregado`
  Salida: 400 `ProblemDetails`, `errors.clienteId` indica que es obligatorio.

## Errores esperados
| Caso | Código | Cuerpo |
|---|---|---|
| Falta `clienteId` o está vacío | 400 | `ProblemDetails`, `errors.clienteId` |
| `estado` fuera de la lista válida | 400 | `ProblemDetails`, `errors.estado` |
| `desde` o `hasta` con formato distinto de `yyyy-MM-dd` | 400 | `ProblemDetails`, `errors.desde` o `errors.hasta` |
| `desde` posterior a `hasta` | 400 | `ProblemDetails`, `errors.desde` (fechas invertidas) |
| `pagina` < 1 o no numérica; `tamanoPagina` no numérico o fuera de 1–50 | 400 | `ProblemDetails`, `errors.pagina` o `errors.tamanoPagina` |

## Fuera de alcance
- Crear, editar o anular pedidos.
- Base de datos (los datos siguen en memoria).
- Autenticación y autorización.
- Filtro por sucursal o por total.
- Ordenar por otro campo distinto del definido.
- Cambiar el endpoint de detalle `GET /api/pedidos/{id}`.

## Preguntas abiertas
Ninguna. Las tres propuestas de la redacción fueron confirmadas y pasaron a las reglas de negocio.
