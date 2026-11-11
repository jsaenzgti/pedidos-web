# Plan de migración · Pedidos Legado → Pedidos Web

Base: [matriz-decision.md](../diagnostico/matriz-decision.md), [riesgos.md](../diagnostico/riesgos.md) y [defectos-corregidos.md](defectos-corregidos.md).

## 1. Etapas
Un módulo a la vez (strangler fig), en el orden de la matriz de decisión.
1. **Cálculo de totales** (`src/PedidosWeb.Core/Totales/`): hecho en la Unidad 5. 60 casos en verde y R05 corregido.
2. **Pedidos** (`UFrmPedidos`): modernizar (3,70). Incluye transacción al grabar y la unificación de la regla de descuento (H1).
3. **Maestros** (clientes y productos): modernizar (3,90).
4. **Stock** (`UStock`): modernizar (3,45), con tabla de movimientos y auditoría.
5. **Informes** (`UFrmInformes`): reemplazar (4,25).

## 2. Convivencia
- El legado sigue siendo la fuente de verdad de cada módulo hasta su corte; el portal calcula en paralelo.
- Comparación: el golden master (`migracion/golden-master.csv`) congela el cálculo. Cada módulo nuevo necesita el suyo, con pedidos reales del legado.
- Diferencias esperadas y documentadas: R05 (hasta 1 peso por partida) y H1 (umbrales exactos 500.000 y 1.000.000). El negocio las concilia mientras conviven.

## 3. Riesgos
| Riesgo | Mitigación |
|---|---|
| Pantalla y grabación calculan distinto el descuento (RS-08, H1) | Contabilidad decide la regla; se registra en defectos-corregidos.md y se cubre con un caso del golden master |
| Diferencias de 1 peso por R05 al comparar sistemas | Documentado como D1; la conciliación usa tolerancia de 1 peso por partida hasta el corte |
| Pedido sin transacción y folio con `MAX()+1` (RS-04, RS-05) | Transacción y secuencia de base de datos en la etapa de Pedidos; prueba de concurrencia |
| Stock negativo, sin movimientos ni bloqueo (RS-06, RS-18) | Etapa de Stock con tabla de movimientos, bloqueo y pruebas antes de cortar |
| Reglas duplicadas con «NO TOCAR» (RS-15) | Una sola implementación en `PedidosWeb.Core`, con pruebas por criterio |

## 4. Criterio de corte
- Se apaga el legado de un módulo cuando sus casos del golden master pasan al 100 %, las diferencias restantes están registradas y aprobadas por el negocio, y hubo un periodo de convivencia sin diferencias nuevas.
- Vuelta atrás: el legado se conserva sin cambios y activo hasta el siguiente módulo; si algo falla, se vuelve a dirigir el uso al legado y se corrige en el portal.
