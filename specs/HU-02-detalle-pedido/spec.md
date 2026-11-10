# HU-02 · Detalle de un pedido

**Historia:** Como vendedor, quiero ver el detalle de un pedido, para responderle al
cliente sin entrar al sistema legado.

## Qué devuelve

La ruta `GET /api/pedidos/{id}/detalle` devuelve:

- Del pedido: `id`, `folio`, `fecha`, `estado` y `sucursal`.
- Del cliente: `clienteId` y `clienteNombre`, tomados del pedido.
- De cada línea: `numero`, `producto`, `cantidad`, `precioUnitario` y `subtotal`
  (`cantidad × precioUnitario`), ordenadas por `numero`.
- Total: la suma de los subtotales de las líneas (no se usa el `Total` guardado en el pedido).

Los montos son pesos chilenos (CLP), `decimal` sin decimales, sin IVA.

## Criterios de aceptación

- **CA-01** · Dado el pedido 8, cuando pido su detalle, entonces el `total` es 120100 y es
  igual a la suma de los subtotales de sus 3 líneas (64500, 35400 y 20200).
- **CA-02** · Dado el pedido 8, cuando pido su detalle, entonces trae `id` 8, folio
  `PED-STGO-000008`, fecha 2026-10-20, estado `Entregado`, sucursal `STGO`, cliente `C-001`
  (`Ferretería El Clavo`) y sus 3 líneas ordenadas por `numero`.
- **CA-03** · Dado un id que no existe (por ejemplo 999), cuando pido su detalle, entonces
  la API responde 404.
- **CA-04** · Dado un pedido que existe pero no tiene líneas, cuando pido su detalle,
  entonces responde 200 con `lineas` vacío y `total` 0.

## Si el pedido no existe

Responde 404 sin cuerpo, igual que `GET /api/pedidos/{id}`.

## Fuera de alcance

- IVA, descuentos y recargos.
- Formato de moneda para mostrar en pantalla.
- Datos extra del cliente (RUT, zona, estado activo).
- Cambiar la ruta existente `GET /api/pedidos/{id}`.
