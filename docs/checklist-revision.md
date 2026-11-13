# Checklist de revisión · pedidos-web

## Antes de pedir revisión
- [ ] `dotnet test` en verde y el subagente `revisor` corrido sin hallazgos Importantes.
- [ ] Spec, plan y tests están alineados: cada criterio tiene su test.
- [ ] La descripción del PR tiene Qué cambia, Por qué (mío) y Cómo probarlo, sin [COMPLETAR].

## Al revisar (persona)
- [ ] ¿El cambio parte de un criterio de la spec? La IA no sabe si la spec está al día.
- [ ] ¿Los documentos (spec, plan) quedaron en sincronía con el código?
- [ ] ¿El porqué explica el motivo y no solo repite el qué?

## Con los hallazgos de la IA
- [ ] Cada hallazgo tiene respuesta: acepto, discutir o descarto, con argumento.
- [ ] Si la IA no comenta, confirmo en el log que sí revisó (turnos y permisos negados en 0).

## Merge
- [ ] CI en verde, ningún hallazgo sin responder y este checklist completo.

## Mi PR de HU-03
- **PR:** https://github.com/jsaenzgti/pedidos-web/pull/7
- **Hallazgo de la IA que más discutimos:** ninguno. Claude revisó el PR (4 turnos, 0 permisos negados) y no halló nada que reportar.
- **Mi hallazgo humano:** ninguno. No encontré nada que la IA no hubiera visto.
- **Nota:** el desajuste del plan (faltaba CA-05 en la tabla de trazabilidad) lo señaló Claude en la sesión; se corrigió en este PR.
