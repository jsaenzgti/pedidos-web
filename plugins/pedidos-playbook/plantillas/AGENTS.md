# AGENTS.md

> Plantilla del playbook de Pedidos Web. Cópiala a la raíz de tu repo y completa los <campos>.

## Proyecto
<Qué es el sistema, qué stack usa y si hay un legado que se está migrando.>

## Comandos
- Compilar: `<comando>`
- Testear: `<comando>`
- Ejecutar: `<comando>`
- Comprobar que está arriba: `<endpoint o verificación>`

## Estilo de código
- <Convenciones de nombres del dominio.>
- <Reglas de tipos sensibles: dinero, fechas, etc.>
- <Qué capa contiene la lógica de negocio y cuál no.>

## Tests
- <Framework y ubicación de los tests.>
- Nombre: `Metodo_Escenario_Resultado`.
- Se ejecutan antes de cada commit.

## Seguridad
- Nunca secretos, claves ni cadenas de conexión en el código.

## Reglas de trabajo
- Toda funcionalidad parte de una spec aprobada en `specs/<historia>/spec.md`.
- Flujo: spec aprobada → plan.md → tests que fallan → código, con aprobación en cada paso.
- Un test por criterio de aceptación.
- No hacer commit sin tests en verde.
- Sin pedir permiso solo se puede leer, compilar y testear. Editar archivos, commit y push requieren aprobación previa.

## Complementos que van aparte (no viajan en el plugin)
- `REVIEW.md`: qué es Importante, qué no reportar y el tope de detalles para el revisor.
- `.claude/settings.json`: reglas deny/ask de permisos propias de cada repo.
