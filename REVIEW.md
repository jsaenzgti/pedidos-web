# Reglas de revisión · pedidos-web

## Qué es Importante
Solo lo que rompe comportamiento, dinero o seguridad:
- Dinero con `double` o `float` (debe ser `decimal`).
- Fechas con `DateTime` donde corresponde `DateOnly`.
- Lógica de negocio en `PedidosWeb.Api`, o un endpoint que dependa de la clase en memoria en vez de un servicio de Core.
- Secretos, claves o cadenas de conexión escritos en el código.

## Qué no reportar
- Lo que ya valida el CI (compilación y tests).
- Formato, nombres y estilo.
- Archivos de `legado/`.

## Tope
Máximo 5 detalles menores por revisión.

## Revisar siempre
1. Cada criterio de aceptación de la spec tiene su test xUnit, con nombre `Metodo_Escenario_Resultado`.
2. El dinero usa `decimal` y las fechas `DateOnly`.
3. `PedidosWeb.Api` solo tiene endpoints; la lógica está en `PedidosWeb.Core`.
