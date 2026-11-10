# Mapa de uso de IA en el SDLC · Pedidos Web

> Copia este archivo como `docs/mapa-sdlc.md` y complétalo (paso 4 del taller de la Unidad 1).
> Para cada etapa: qué tarea concreta se delega a Claude, qué artefacto queda versionado,
> quién decide y qué revisa, qué puede salir mal y en qué evento de Scrum ocurre.
> Modelo: el caso de Valentina y TurnoVet (`caso-scrum-y-sdd.pdf`).

**Autor:** Julio Saenz
**Fecha:** 2026-11-09

| Etapa | Se delega a Claude | Artefacto versionado | Revisa una persona | Riesgo | Evento Scrum |
|---|---|---|---|---|---|
| Requisitos | Revisar la historia y las reglas del legado (por ejemplo R1 a R5 de `FormatearFolio`), detectar vacíos y redactar criterios de aceptación que se puedan probar, con su sección «Fuera de alcance» | `docs/specs/<historia>/spec.md` (objetivo, criterios de aceptación, fuera de alcance); la historia en el Product Backlog | El Product Owner prioriza la historia y aprueba la spec: cada criterio debe ser concreto y comprobable; lo que no esté escrito se pregunta (regla 8 de `AGENTS.md`) | Criterios flojos que cada persona o agente interpreta distinto; que Claude invente una regla | Sprint Planning (elegir la historia) y refinamiento del Product Backlog |
| Diseño | En plan mode, leer la spec y el repo y proponer qué archivos tocar, en qué orden, la firma y las excepciones, sin editar nada | `docs/specs/<historia>/plan.md`, guardado a pedido después de aprobarlo | El líder técnico corrige el plan (por ejemplo «usa la clase que ya existe») y lo aprueba antes de cualquier edición (regla 2 de `AGENTS.md`) | Que Claude edite sin plan aprobado, sobrediseñe o rompa la estructura de `PedidosWeb.Core` sin dependencias de infraestructura | Sprint (al empezar cada historia) |
| Código | Dividir el plan en tareas chicas e implementarlas una por una (por ejemplo: validar `numero`, validar `sucursal`, formatear) | `src/PedidosWeb.Core/` (por ejemplo `Folios/FolioFormatter.cs`) | Quien desarrolla revisa cada tarea antes de pasar a la siguiente y lee el diff completo | Código que compila pero omite una regla; cambios grandes difíciles de revisar; tocar archivos fuera de `src/`, `tests/` y `docs/` | Sprint (trabajo diario, seguido en la Daily) |
| Pruebas | Escribir un test xUnit por cada criterio de la spec, incluidos los casos límite (rangos, trim, `Ñ`, orden de R5) | `tests/PedidosWeb.Core.Tests/` (un archivo por clase) | Quien desarrolla comprueba la Definition of Done: cada criterio con su test en verde, sin warnings nuevos y la spec refleja lo construido | Tests que pasan sin probar nada o que copian el error de la implementación; criterios sin test | Sprint, al cerrar cada historia con la Definition of Done |
| Revisión y despliegue | Resumir el diff y señalar riesgos; en las próximas unidades, apoyar en el pipeline de CI y en revisar los PR | Commit y pull request en GitHub; pipeline de CI (unidades siguientes) | Una persona lee y entiende el diff y hace siempre el commit, push y merge (regla 3 de `AGENTS.md`); el despliegue lo aprueba una persona después de que el pipeline pase en verde | Hacer commit sin leer el diff; desplegar cambios no revisados o con tests en rojo; romper la compatibilidad de folios con el legado | Sprint (commit) y Sprint Review (se muestra el incremento al negocio) |
| Mejora continua | Analizar lo que pasó en el sprint (tiempos, errores, correcciones) y proponer reglas nuevas para `AGENTS.md` | `AGENTS.md` con una sección «Aprendido en la retro del sprint N»; `docs/mapa-sdlc.md` actualizado | El equipo decide qué reglas se agregan; una persona edita `AGENTS.md` (Claude no lo toca, regla 6) | Repetir los mismos errores en cada historia; reglas obsoletas que el agente sigue aplicando | Retrospectiva |

## Evidencia del paso 3

| | Sin IA | Con Claude Code |
|---|---|---|
| Tiempo (min) | | |
| Errores encontrados | | |
| Qué tuve que corregir | | |
