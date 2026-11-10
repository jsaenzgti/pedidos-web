# Matriz de decisión de Pedidos Legado

Pesos y puntajes propuestos por Claude y aprobados por Julio Saenz. Puntaje ponderado = suma de (puntaje 1–5 × peso), calculado con un script.

## Pesos

| Criterio | Qué mide | Peso |
|---|---|---|
| Costo | Cuánto cuesta en horas y dinero | 20 % |
| Complejidad | Qué tan difícil es técnicamente | 15 % |
| Riesgo | Qué puede salir mal en el camino | 30 % |
| Impacto | Cuánto valor entrega al negocio | 25 % |
| Personas | Conocimiento disponible en el equipo | 10 % |
| **Total** |  | **100 %** |

Riesgo es el criterio con más peso porque los hallazgos son de dinero y datos (totales distintos entre pantalla y grabado, pedidos sin transacción, stock sin auditoría). Impacto es el segundo porque justifica la inversión.

## Estrategias

- **Mantener:** dejar el módulo como está.
- **Refactorizar:** ordenar el código Delphi existente.
- **Modernizar:** migrar a .NET conservando las reglas de negocio.
- **Reemplazar:** sustituirlo por otra solución nueva.

## Puntajes por criterio (1–5; 5 = mejor opción)

Orden: Costo · Complejidad · Riesgo · Impacto · Personas.

| Módulo | Mantener | Refactorizar | Modernizar | Reemplazar |
|---|---|---|---|---|
| Cálculo y utilidades (`UCalculoTotales`, `UUtilidades`) | 5·5·2·1·2 | 4·4·3·2·2 | 4·4·4·5·5 | 2·2·2·4·4 |
| Pedidos (`UFrmPedidos`) | 5·5·2·1·2 | 3·3·3·3·2 | 3·3·3·5·5 | 1·1·2·4·3 |
| Stock (`UStock`) | 5·5·1·1·2 | 4·4·3·2·2 | 3·3·3·4·5 | 2·2·3·4·4 |
| Maestros (clientes y productos) | 5·5·3·2·2 | 4·4·3·2·2 | 3·4·4·4·5 | 3·3·3·4·4 |
| Informes (`UFrmInformes`) | 5·5·3·2·2 | 4·4·3·3·2 | 3·3·4·4·5 | 4·4·4·5·4 |

## Matriz (puntaje ponderado)

| Módulo | Mantener | Refactorizar | Modernizar | Reemplazar | Recomendación |
|---|---|---|---|---|---|
| Cálculo y utilidades | 2,80 | 3,00 | **4,35** | 2,70 | Modernizar |
| Pedidos | 2,80 | 2,90 | **3,70** | 2,25 | Modernizar |
| Stock | 2,50 | 3,00 | **3,45** | 3,00 | Modernizar |
| Maestros | 3,35 | 3,00 | **3,90** | 3,35 | Modernizar |
| Informes | 3,35 | 3,25 | 3,75 | **4,25** | Reemplazar |

Notas:
- La recomendación de cada módulo es la columna con mayor puntaje.
- Los márgenes más cortos son Stock (3,45 frente a 3,00) e Informes (4,25 frente a 3,75): un cambio pequeño de puntaje podría cambiar el ganador.
- Los puntajes son juicio sustentado en la evidencia de `inventario.md`, `reglas-negocio.md` y `riesgos.md`.
- Antes de modernizar o reemplazar, conviene cerrar con el negocio los 4 puntos por verificar: pedido mínimo (30.000 o 25.000), umbral de descuento (400.000 o 500.000), significado del estado `A` y vigencia de la promoción de Navidad.

## Resumen ejecutivo

1. **Qué es y su estado:** El sistema de pedidos de la distribuidora funciona desde hace años, pero está en una tecnología antigua (Delphi, última actualización en 2009) y su manual describe una base de datos que ya no usa.
2. **Hallazgo principal (con evidencia):** Un mismo pedido puede mostrar un total en pantalla y guardarse con otro cuando el monto es justo 500.000 o 1.000.000, porque el cálculo está escrito dos veces con reglas distintas (folios 1008 y 1011).
3. **Riesgo más urgente:** Los pedidos se guardan sin protección ante fallos, el control de stock falla sin avisar y cualquiera que escriba «ADMIN» obtiene permisos de administrador, sin contraseña.
4. **Estrategia y por dónde empezar:** Modernizar pedidos, stock, clientes y productos conservando sus reglas, y reemplazar los informes por una herramienta de reportes; se empieza por el cálculo de totales, que es la parte más contenida y fácil de probar.
5. **Próximo paso que se pide aprobar:** Confirmar con contabilidad el monto mínimo del pedido y el umbral de descuento, y autorizar el inicio de la modernización del cálculo de totales.
