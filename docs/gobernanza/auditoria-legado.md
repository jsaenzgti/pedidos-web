# Auditoría del legado · pedidos-web

Alcance: `legado/src/UFrmPedidos.pas` y `legado/src/UDMPrincipal.pas`. Los hallazgos los propuso el subagente `analista-legado`; cada línea citada se abrió y se comprobó antes de decidir su estado. Rutas relativas a `legado/`.

## Hallazgos

| ID | Hallazgo | Evidencia (archivo:línea) | Tipo | Severidad | Estado | Argumento |
|---|---|---|---|---|---|---|
| AU-01 | La clave de la base de datos está escrita en el código y se carga en cada conexión. | `src/UDMPrincipal.pas:41`, `:59` | Seguridad | Alta | Validado | `:41` dice `CLAVE_BD = 'Distrib#2008'` y `:59` hace `Session.AddPassword(CLAVE_BD)`. La clave viaja en el fuente y en el ejecutable. |
| AU-02 | Las consultas concatenan texto del usuario sin parámetros ni escape (inyección SQL). | `src/UFrmPedidos.pas:152`, `:261-262`; `src/UDMPrincipal.pas:85` | Seguridad | Alta | Validado | El código de producto (`:152`), `gUsuario` y la observación (`:261-262`) y la clave de parámetro (`UDMPrincipal.pas:85`) se concatenan entre comillas. La observación solo se recorta a 200 caracteres (`:257-258`). El RUT (`:102`) pasa antes por `ValidaRut` (`:91`); si bloquea un apóstrofe, queda **por verificar**. |
| AU-03 | El pedido se graba en varios pasos sin transacción. | `src/UFrmPedidos.pas:255`, `:263`, `:266-273`, `:320`, `:324` | Integridad | Alta | Validado | `:255` dice `// TODO transaccion`. Cabecera, líneas, total y stock son pasos separados. El único `try/finally` (`:276-291`) solo libera una consulta; no es transacción ni rollback. |
| AU-04 | El folio se calcula con `MAX(FOLIO)+1` y puede duplicarse. | `src/UFrmPedidos.pas:249-251`; `db/esquema-dbisam.sql:52` | Integridad | Alta | Descartado | El cálculo es cierto (`:251`), pero `esquema-dbisam.sql:52` define `PRIMARY KEY ("FOLIO")`: la base rechaza un folio repetido, así que **no hay folios duplicados**. Lo que puede pasar es que el segundo `INSERT` falle a mitad de la grabación, y eso ya lo cubre AU-03. |
| AU-05 | El bloqueo por deuda vencida solo se evalúa al buscar el cliente, no al grabar. | `src/UFrmPedidos.pas:111-112`, `:204` | Regla de negocio | Media | Validado | La regla está en `:111-112` dentro de la búsqueda. Al grabar, `:204` solo comprueba `FIdCliente = 0`. Una deuda que cambie con la pantalla abierta no se vuelve a evaluar. |
| AU-06 | Las reglas de negocio viven solo en la pantalla; `EjecutarSQL` ejecuta cualquier texto sin control. | `src/UDMPrincipal.pas:92-98`; `src/UFrmPedidos.pas:214`, `:221`, `:231` | Regla de negocio | Media | Validado | Máximo de 40 líneas (`:214`), precio mayor que 0 (`:221`) y mínimo de 30.000 (`:231`) se validan solo en el formulario. `EjecutarSQL` no valida nada, así que otro código que use el módulo de datos se salta todas. |
| AU-07 | Reglas ocultas de descuento y flete que difieren de las de la pantalla. | `src/UFrmPedidos.pas:296-308`, `:312-313`, `:327-332` | Regla de negocio | Media | Validado | Umbrales con `>=` (`:296`, `:298`), VIP `+3 %` (`:302-303`), Navidad `+2 %` (`:304-305`), tope de 10 % (`:306`) y flete `×1,15` en regiones 11 y 12 (`:312-313`). Los totales se comparan en `:327-328`. |
| AU-08 | El stock se descuenta después de grabar y no se verifica disponibilidad antes. | `src/UFrmPedidos.pas:324` | Regla de negocio | Media | Validado | La única referencia al stock en el formulario es `DescontarStockPedido` (`:324`), tras los `INSERT`. No hay ninguna consulta de existencias al agregar la línea ni al validar. |

## Cruce con el diagnóstico de la U4

- **Coinciden:** AU-01 (RS-02), AU-02 (RS-01), AU-03 (RS-04), AU-05 (RS-14), AU-07 (RS-08) y AU-08 (RS-06).
- **Nuevos en esta auditoría:** AU-06 (las reglas viven solo en la pantalla y `EjecutarSQL` no controla nada).
- **Contradicen la U4:** AU-04. RS-05 de la U4 decía que el folio con `MAX()+1` podía duplicarse; el esquema (`db/esquema-dbisam.sql:52`) tiene clave primaria sobre `FOLIO`, así que ese riesgo estaba sobreestimado. El riesgo real es un error de `INSERT` en medio de la grabación.
- **De la U4 que el subagente no vio:** RS-03 (permisos por nombre de usuario) y RS-07 (IVA fijo en informes), porque están en archivos fuera del alcance. RS-09 (pedido mínimo fijo en 30.000 en lugar del parámetro) está en `UFrmPedidos.pas:231`, la línea que AU-06 cita, pero el subagente no notó que ignora el parámetro.
