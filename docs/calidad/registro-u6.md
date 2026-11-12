# Registro de calidad · Unidad 6

<!--
Lo completas tú durante el taller. Frases cortas: lo que hiciste y por qué.
Borra estos comentarios al terminar.
-->

## Preparación

- **Tests en `Skip` (pendientes de unidades anteriores):** ninguno (132 tests en Core y 7 en Api, todos en verde, 0 omitidos).

## Etapa 1 · Tests que sí prueban

- **Módulo elegido:** CalculoTotales.
- **Tests que propuso Claude:** 5 de borde que el golden master no cubre (pedido sin líneas, categoría OFE en minúscula con espacios, categoría nula, IVA 0, base 1 peso sobre el umbral).
- **Cambio que hice a mano para probarlos:** quité el `if (lineas.Count == 0)` del flete en `CalculoTotales.Calcular`. Falló `Calcular_SinLineas_TodoEnCero`.
- **Tests que me quedé:** los 5. Solo probé la ruptura del test 1; los otros 4 no se rompieron a mano.
- **Tests que borré y por qué:** ninguno.

## Etapa 2 · Pipeline seguro

- **`ci.yml`:** https://github.com/jsaenzgti/pedidos-web/actions/runs/43242591350 (verde)
- **`claude.yml` en `main`:** sí (aún sin probar; se prueba en la Etapa 3)
- **Revisé:** permisos mínimos ☑ · timeout ☑ · versiones fijas ☑ · token solo como secreto ☑

## Etapa 3 · La falla

- **PR:** https://github.com/jsaenzgti/pedidos-web/pull/1
- **Qué rompí (a mano):** `src/PedidosWeb.Core/Totales/CalculoTotales.cs`: el IVA pasó de `* tasaIva` a `* 0.19m` (commit `b48476e`). Falló `Calcular_TasaIvaCero_IvaCeroYTotalSinIva`.
- **Diagnóstico de `@claude` (en una línea):** el paso `dotnet test` falla porque el IVA quedó fijo en 19 % y `tasaIva` ya no se usa; propone revertir `b48476e`.
- **Mi decisión:** Aceptar ☑ · Ajustar ☐ · Rechazar ☐
- **Por qué:** la causa y la solución eran correctas. Su única suposición errónea (que fallarían casos del golden master) la marcó como no confirmada: los 60 casos usan 0,19 y solo falló 1 test.
- **Corrección y merge:** commit `8fb093b` «fix: restaura tasaIva en el cálculo del IVA»; CI en verde ☑ (run del PR sobre `8fb093b`); merge ☑ (PR #1, 2026-11-12).
- **Hallazgos del pipeline:** `claude-code-action@v1.0.0` usaba un modelo retirado (404); se subió a `v1.0.271` y se fijó `--model claude-sonnet-5-5`. Con `--max-turns 5` `@claude` se quedaba sin turnos; se subió a 10 (se aparta de los 5 de la guía).
