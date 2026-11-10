# Reglas de negocio de Pedidos Legado

Hallazgos del subagente `analista-legado`. Rutas relativas a `legado/`.
Columna «Verificada»: «Sí» solo cuando el responsable abre el archivo en esa línea y lee el código. Todas están en «No» hasta esa revisión.
Lo que el subagente no pudo confirmar en el código está marcado «por verificar» en la columna Efecto.

| # | Regla | Archivo:línea | Condición | Efecto | Verificada |
|---|---|---|---|---|---|
| 1 | Pedido mínimo, duplicado con valores distintos | `src/UFrmPedidos.pas:231`; `db/datos/PARAMETROS.csv:3` | Subtotal < 30.000 y región ≠ 13 (Santiago exento) | Rechaza el pedido. PARAMETROS dice 25.000 y ningún código lo lee. Valor vigente: por verificar. | Sí |
| 2 | Descuento por volumen sobre base sin ofertas, con umbrales distintos | `src/UCalculoTotales.pas:77-82` (`>`); `src/UFrmPedidos.pas:296-301` (`>=`) | Base sobre 1.000.000 da 8 %; sobre 500.000 da 5 % | En el monto exacto, la pantalla da 0 % y lo grabado da 5 % u 8 %. Los folios 1008 y 1011 (base 500.000) coinciden con `>=`. | No |
| 3 | El comentario contradice al código en el umbral | `src/UCalculoTotales.pas:4` frente a `:79`; `src/UFrmPedidos.pas:298` | Comentario: «5 % sobre 400.000 (acta comercial 2011)» | El código aplica 500.000. Cuál es el vigente: por verificar con contabilidad. | Sí |
| 4 | Descuento VIP +3 puntos y tope de 10 % | `src/UCalculoTotales.pas:88-91`; `src/UFrmPedidos.pas:302-307` | Cliente tipo `V` | Suma 0,03 y limita a 0,10 (8 % + 3 % queda en 10 %). Igual en ambos lugares. | No |
| 5 | Descuento de Navidad solo al grabar, y nunca activo | `src/UFrmPedidos.pas:304-305`; `src/UGlobales.pas:33`; `db/datos/PARAMETROS.csv:6` | `gDescNavidad` y mes = 12 | Suma 0,02, pero el flag queda en False y `DESC_NAVIDAD` de la tabla no se lee. `CalcularTotales` no lo contempla. Vigencia de la promo: por verificar. | Sí |
| 6 | Categoría `OFE` no entra a la base de descuento | `src/UCalculoTotales.pas:70`; `src/UFrmPedidos.pas:287` | Categoría distinta de `OFE` | Las ofertas suman al subtotal pero no al descuento. Una versión usa `UpperCase(Trim())` y la otra compara exacto. Categorías nulas: por verificar. | No |
| 7 | Flete por región con recargo de 15 % | `src/UCalculoTotales.pas:97-100`; `src/UFrmPedidos.pas:312-315` | Regiones 11 y 12 | Flete base × 1,15; en otro caso, flete base. En pantalla es 0 si no hay líneas. | No |
| 8 | IVA sobre neto + flete; tasa desde PARAMETROS con respaldo fijo | `src/UFrmPrincipal.pas:62-64`; `src/UGlobales.pas:31`; `src/UFrmPedidos.pas:318`; `src/UCalculoTotales.pas:105` | `gTasaIVA` = TASA_IVA / 100, o 0,19 si no hay parámetro | Los pedidos usan el parámetro; el informe usa 0,19 fijo (ver RS-07). | No |
| 9 | Redondeo al peso duplicado | `src/UCalculoTotales.pas:50` (`Calc3`); `src/UUtilidades.pas:30` (`RedondeaPesos`) | `Trunc(valor + 0.49)` | El .50 exacto baja (985,5 pasa a 985). Dos copias idénticas. | No |
| 10 | Bloqueo por deuda vencida con gracia de 60 días | `src/UFrmPedidos.pas:110-117` | `SALDO_VENCIDO` > 0 y `DaysBetween(hoy, FECHA_VENCIMIENTO)` > 60 | No se vende. Los 60 días no están documentados. `DaysBetween` no tiene signo: un vencimiento futuro lejano también bloquea, y una fecha nula también. | Sí |
| 11 | Cliente inactivo no se bloquea | `src/UFrmPedidos.pas:99-102`; `src/UFrmClientes.pas:93`; `db/datos/CLIENTES.csv:28,31` | `ACTIVO` solo se escribe al crear el cliente | Los clientes 27 y 30 (inactivos) pueden recibir pedidos; el pedido 1020 es del 30. Si fue baja posterior: por verificar. | No |
| 12 | Stock: VIP hasta -20, normal no baja de 0 | `src/UStock.pas:40-50` | VIP: nuevo stock < -20 se rechaza; normal: < 0 | Solo sale un mensaje (`UStock.pas:71`); el pedido ya quedó grabado (`UFrmPedidos.pas:263,324`). `btnAgregar` nunca revisa stock. | No |
| 13 | Máximo 40 líneas y precio mayor que 0 | `src/UFrmPedidos.pas:214-226` | Más de 40 líneas o precio ≤ 0 | Rechaza al grabar, no al agregar. | No |
| 14 | Observación truncada a 200 caracteres | `src/UFrmPedidos.pas:257-258` | Longitud mayor que 200 | Corta sin avisar. | No |
| 15 | Aviso «primer pedido del cliente, avisar a cobranza» | `src/UFrmPedidos.pas:334-344` | Cuenta de pedidos del cliente = 1 | Solo un mensaje. Se cuenta después del INSERT, así que incluye el propio pedido. | No |
| 16 | Corte de las 18:00 pasa al siguiente día hábil | `src/UUtilidades.pas:128-140`; `src/UFrmPedidos.pas:240-243` | Hora ≥ 18 | La fecha salta al próximo lunes a viernes. Antes de las 18:00 en fin de semana no se mueve; no hay feriados. | No |
| 17 | Permisos por nombre de usuario | `src/UFrmPrincipal.pas:46-50,95-99`; `src/UFrmClientes.pas:143`; `src/UFrmProductos.pas:44-45` | `ADMIN` o `JEFEVENTAS` dan nivel 9; informes y VIP piden nivel ≥ 2; borrar producto, nivel 9 | El nivel 2 nunca se asigna. Sin contraseña. | No |
| 18 | Región válida 1 a 16, por defecto 13 | `src/UFrmClientes.pas:91,148-149` | Región fuera de 1..16 | Se rechaza. Rango fijo en código, no consulta REGIONES. | No |
| 19 | RUT validado con dos algoritmos distintos | `src/UUtilidades.pas:72-79` (`ValidaRut`); `src/UFrmClientes.pas:192-199` (`RutOk`) | Resto 11 y resto 10 | `ValidaRut` da `0` y `K`; `RutOk` da `K` y `0` (invertido). Un RUT con K o 0 pasa en pedidos y falla en el mantenedor. Ejemplos: clientes 7, 11, 18 y 23. | Sí |
| 20 | RUT con `k` minúscula no pasa en pedidos | `src/UUtilidades.pas:39-49,79`; `db/datos/CLIENTES.csv:19` | `LimpiaRut` no pasa a mayúscula | El cliente 18 (`76500019-k`) no se puede buscar en pedidos. | No |
| 21 | Búsqueda de cliente por RUT sin normalizar | `src/UFrmPedidos.pas:90,102` | Valida con puntos y guiones, pero el SQL compara el texto tal cual | «76.500.005-K» valida pero da «Cliente no existe». | No |
| 22 | Pedido grabado con total 0 y luego actualizado | `src/UFrmPedidos.pas:262,320` | El INSERT pone TOTAL = 0 y el UPDATE lo corrige | Un corte entre ambos deja pedidos con total 0. | No |
| 23 | Estado `A` en datos sin código que lo genere | `db/datos/PEDIDOS.csv:19,43`; `src/UFrmPedidos.pas:262`; `src/UFrmInformes.pas:71` | El código solo graba `C`; el informe filtra `C` si el check está marcado | Significado de `A` (¿anulado?): por verificar. Con el check desmarcado, el informe suma los `A`. | No |

## Sugerencia: 5 reglas fáciles de verificar en Rider

Ctrl+Shift+N y escribe `archivo:línea`.

1. Regla 1: `UFrmPedidos.pas:231` (30.000 fijo).
2. Regla 3: `UCalculoTotales.pas:4` y `:79` (comentario 400.000 frente a código 500.000).
3. Regla 19: `UFrmClientes.pas:192-199` frente a `UUtilidades.pas:72-79` (K y 0 invertidos).
4. Regla 5: `UFrmPedidos.pas:304-305` (Navidad solo al grabar).
5. Regla 10: `UFrmPedidos.pas:110-117` (60 días de gracia).
