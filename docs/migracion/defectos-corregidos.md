# Defectos corregidos · migración del cálculo de totales

Rutas `legado/src/...` verificadas por el subagente `analista-legado`. Lo no probado está marcado «por verificar».

## D1 · R05 · Redondeo de pesos (CORREGIDO)

- **Qué hace el legado:** `Calc3` redondea con `Trunc(valor + 0.49)`, así que una fracción de .50 exacto baja al peso inferior; solo sube desde .51 (`legado/src/UCalculoTotales.pas:48-51`). Se aplica al descuento (`:114`), al flete (`:98`, `:100`) y al IVA (`:105`). `RedondeaPesos` es una copia idéntica (`legado/src/UUtilidades.pas:27-31`) que usa la grabación del pedido (`legado/src/UFrmPedidos.pas:308,313,315,318`).
- **Corrección:** `RedondearPesos` en `src/PedidosWeb.Core/Totales/CalculoTotales.cs` usa `Math.Round(valor, 0, MidpointRounding.AwayFromZero)`: el .50 sube.
- **Por qué:** el sesgo del legado es siempre a la baja en las tres partidas, hasta 0,51 de peso por partida.
- **Casos del golden master que cambian:** 13 de 60 (2, 6, 8, 18, 21, 23, 25, 27, 36, 37, 42, 46 y 54).
- **Suma de la columna total:** 58.202.135 → 58.202.144.
- **Commit:** `0846cbb` «fix: corrige R05, redondeo comercial al peso (13 casos del golden master)», separado del commit `6561dc6` «igual al legado».
- **Convivencia:** mientras convivan ambos sistemas, el negocio debe conciliar diferencias de hasta 1 peso por partida entre el legado y el portal.
- **Por verificar:** en el legado, el dinero es `Double` (RS-10); un valor como .51 podría quedar en 100,99999… tras sumar 0.49 y truncar a 100. No está probado.

## H1 · Umbrales de descuento por volumen (HALLAZGO, NO CORREGIDO)

- **Qué encontraste:** la pantalla y la grabación usan reglas distintas para el mismo descuento (riesgo RS-08 del diagnóstico).
  - Cálculo y pantalla (`Proc2`): `> 1.000.000` da 8 %, `> 500.000` da 5 % (`legado/src/UCalculoTotales.pas:77-82`; `legado/src/UFrmPedidos.pas:371-376`).
  - Grabación: `>= 1.000.000` da 8 %, `>= 500.000` da 5 % (`legado/src/UFrmPedidos.pas:296-301`).
- **Efecto:** solo pedidos con base de descuento exacta de 500.000 (0 % en pantalla, 5 % al grabar) o de 1.000.000 (5 % frente a 8 %). El total grabado puede diferir del que vio el usuario. La alerta solo aparece con `gModoPrueba`, que siempre es `False` (`legado/src/UFrmPedidos.pas:326-332`).
- **Quién decide:** contabilidad (`legado/src/UCalculoTotales.pas:5`: «NO TOCAR sin hablar con contabilidad»). El C# replica la versión de la pantalla (`>`), que es la que congela el golden master.
