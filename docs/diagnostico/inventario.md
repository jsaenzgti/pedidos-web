# Inventario de Pedidos Legado

Sistema Delphi con base DBISAM. Cubre cada `.pas` y `.dfm` de `legado/src/`, el `.dpr` y las 6 tablas del esquema.
Líneas medidas con `wc -l`, no estimadas.

## Código (legado/src/ y proyecto)

| Archivo | Tipo | Responsabilidad | Líneas |
|---|---|---|---|
| PedidosLegado.dpr | Proyecto | Punto de entrada: crea el data module `DMPrincipal` y el formulario `FrmPrincipal`. | 24 |
| UCalculoTotales.pas | Unidad de lógica | Calcula subtotal, base de descuento, descuento por volumen y VIP (tope 10 %), flete por región, IVA y total; administra las líneas del pedido en memoria. | 147 |
| UDMPrincipal.pas | Data module | Abre y cierra la base DBISAM y sus tablas; ofrece `EjecutarSQL` y `LeerParametro`. | 100 |
| UDMPrincipal.dfm | Data module (diseño) | Componentes de datos: base, 5 tablas y 2 consultas (`qryGeneral`, `qryAux`). | 65 |
| UFrmClientes.pas | Formulario | Mantenedor de clientes: buscar, crear, grabar y eliminar, con validación de RUT, tipo, región y permiso VIP. | 215 |
| UFrmClientes.dfm | Formulario (diseño) | Pantalla del mantenedor de clientes (grilla, campos y botones). | 213 |
| UFrmInformes.pas | Formulario | Informe de ventas por rango de fechas, con totales y exportación a CSV para contabilidad. | 137 |
| UFrmInformes.dfm | Formulario (diseño) | Pantalla del informe (filtros, grilla y resumen). | 160 |
| UFrmPedidos.pas | Formulario | Toma de pedidos: valida cliente y morosidad, agrega líneas, aplica reglas, graba pedido y líneas, recalcula el total y descuenta stock. | 420 |
| UFrmPedidos.dfm | Formulario (diseño) | Pantalla de pedidos (cliente, líneas, totales y observación). | 226 |
| UFrmPrincipal.pas | Formulario | Menú principal: pide el usuario, asigna el nivel, conecta la base, lee parámetros y abre las demás pantallas. | 104 |
| UFrmPrincipal.dfm | Formulario (diseño) | Menú principal y barra de estado. | 62 |
| UFrmProductos.pas | Formulario | Mantenedor de productos: filtro, productos bajo el stock mínimo y validación al grabar; solo admin borra. | 105 |
| UFrmProductos.dfm | Formulario (diseño) | Pantalla de productos (grilla, filtro y navegador). | 98 |
| UGlobales.pas | Globales | Variables globales (usuario, nivel, IVA, flete, flags, ruta de datos) y su inicialización. | 41 |
| UStock.pas | Unidad de lógica | Consulta y descuenta stock (los VIP pueden quedar hasta -20) y registra movimientos. | 93 |
| UUtilidades.pas | Utilidades | Validación de RUT (módulo 11), fechas, cierre de pedidos a las 18:00, formato y redondeo de pesos. | 189 |

Total: 10 `.pas` + 6 `.dfm` + 1 `.dpr` = 17 archivos (los 16 de `legado/src/` más el `.dpr`).

## Tablas (legado/db/esquema-dbisam.sql, 74 líneas)

| Tabla | Tipo | Responsabilidad | Filas en CSV |
|---|---|---|---|
| REGIONES | Tabla | Catálogo de regiones con su flete base. | 16 |
| CLIENTES | Tabla | Clientes: RUT, tipo (N/V), región, saldo vencido y fecha de vencimiento. | 30 |
| PRODUCTOS | Tabla | Productos: código, categoría, precio, stock y stock mínimo. | 40 |
| PEDIDOS | Tabla | Cabecera del pedido: folio, fecha, cliente, usuario, estado y total. | 60 |
| PEDIDO_LINEAS | Tabla | Líneas del pedido (clave folio + línea). | 169 |
| PARAMETROS | Tabla | Parámetros clave/valor (IVA, pedido mínimo, impresora, versión, Navidad, empresa). | 6 |

## Lo que me llamó la atención

- `legado/README.md` dice Paradox/BDE e informes en Crystal Reports; el código usa DBISAM (`dbisamtb`, `UDMPrincipal.pas:59`) y el informe es una grilla con exportación CSV (`UFrmInformes.pas:101`). El README está desactualizado.
- `UStock.pas:85` inserta en `MOV_STOCK`, una tabla que no existe en el esquema; el `except` vacío (`UStock.pas:89`) oculta el error.
- La lógica de totales vive duplicada: `UCalculoTotales.pas` y `btnGrabarClick` (`UFrmPedidos.pas:295-319`) usan umbrales distintos (`>` frente a `>=`) y solo el segundo suma el extra de Navidad. El pedido mínimo está fijo en 30.000 (`UFrmPedidos.pas:231`), pero PARAMETROS dice `PEDIDO_MINIMO = 25000`.
- SQL armado por concatenación de texto (`UFrmPedidos.pas:102`, `UFrmClientes.pas:76`, `UFrmInformes.pas:69`) y clave de la base en el código (`UDMPrincipal.pas:41`). Se anotan para la etapa Encontrar.
