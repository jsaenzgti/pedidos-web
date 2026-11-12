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

- **`ci.yml`:** <enlace al primer run en verde>
- **`claude.yml` en `main`:** <sí / no>
- **Revisé:** permisos mínimos ☐ · timeout ☐ · versiones fijas ☐ · token solo como secreto ☐

## Etapa 3 · La falla

- **PR:** <enlace>
- **Qué rompí (a mano):** <archivo y cambio>
- **Diagnóstico de `@claude` (en una línea):** <resumen>
- **Mi decisión:** Aceptar ☐ · Ajustar ☐ · Rechazar ☐
- **Por qué:** <tu razón>
- **Corrección y merge:** <commit de la corrección; CI en verde ☐; merge ☐>
