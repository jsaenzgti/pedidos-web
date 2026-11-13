# Plan de evolución de la IA para una Business Unit

> Fuente de datos: `docs/adopcion/analytics-ficticio.csv` (4 equipos × 8 semanas, datos ficticios). Las cifras de diagnóstico salen del CSV; los objetivos y plazos de las secciones 3 y 5 son propuestas.

**BU:** Desarrollo de la distribuidora · **Autor:** Julio Saenz · **Fecha:** 13-nov-2026

## 1. Resumen para la gerencia

La BU está dividida en tres velocidades: Pedidos Web trabaja en nivel 4 (80 % de sus PR con Claude Code y aceptación estable de 0,74 a 0,78), Bodega y Reportes usan la IA de forma individual (nivel 2) y Facturacion casi no la usa (nivel 1). En conjunto, 135 de los 336 PR de las 8 semanas se hicieron con Claude Code (40 %), pero el 64 % de ellos viene de un solo equipo. Proponemos instalar en los otros tres equipos la forma de trabajar de Pedidos Web, empaquetada en el plugin `pedidos-playbook` (se instala con dos comandos), y medir el avance cada semana con los mismos indicadores del CSV.

## 2. Diagnóstico por equipo

Modelo de madurez del curso: 1 Uso a escondidas · 2 Uso individual · 3 Equipo con reglas · 4 Proceso AI-native · 5 Organización que mide y mejora.

| Equipo | Nivel | Evidencia 1 (indicador del CSV) | Evidencia 2 (indicador del CSV) |
|---|---|---|---|
| Pedidos Web | 4 · Proceso AI-native | Tasa de aceptación estable entre 0,74 y 0,78 en las 8 semanas, con 5 a 6 usuarios activos y 674 sesiones en total | 86 de 107 PR hechos con Claude Code (80 %) y 17.570 líneas aceptadas |
| Facturacion | 1 · Uso a escondidas | 0 a 1 usuario activo por semana y 15 sesiones en 8 semanas (2 semanas sin ninguna sesión: la 2 y la 5) | 1 de 71 PR con Claude Code (1 %); solo 200 líneas aceptadas |
| Bodega | 2 · Uso individual | Uso alto y constante: 7 usuarios activos todas las semanas y 921 sesiones, el mayor volumen de la BU | Tasa de aceptación muy baja, entre 0,11 y 0,17 (5.110 líneas aceptadas); solo 30 de 109 PR con Claude Code (28 %) |
| Reportes | 2 · Uso individual | Uso irregular: 3 a 4 usuarios en las semanas 1, 3, 5 y 7; 0 a 1 usuario en las semanas 2, 4, 6 y 8 (184 sesiones en total) | Tasa de aceptación media (0,50 a 0,58) y 18 de 49 PR con Claude Code (37 %) |

> Ningún indicador solo: mira el uso, la tasa de aceptación y los PR juntos. Una celda vacía en `tasa_aceptacion` significa que esa semana no hubo sesiones.

## 3. Objetivo por equipo para el próximo trimestre

| Equipo | Objetivo medible | Indicador que lo muestra |
|---|---|---|
| Pedidos Web | Mantener el nivel 4 y documentar su forma de trabajar para que la repliquen los demás | Tasa de aceptación ≥ 0,74 y PR con Claude Code ≥ 80 % todas las semanas |
| Facturacion | Pasar de uso a escondidas a uso regular: ninguna semana sin sesiones y al menos 3 usuarios activos por semana | `usuarios_activos` ≥ 3 y `sesiones` > 0 en las 12 semanas; PR con Claude Code de 1 % a 20 % o más |
| Bodega | Convertir el mucho uso en uso útil: subir la tasa de aceptación de 0,11–0,17 a 0,40 o más, manteniendo los 7 usuarios | `tasa_aceptacion` ≥ 0,40; PR con Claude Code de 28 % a 50 % o más |
| Reportes | Hacer el uso constante: al menos 3 usuarios activos todas las semanas y más PR con Claude Code | `usuarios_activos` ≥ 3 en las 12 semanas; PR con Claude Code de 37 % a 60 % o más |

## 4. El playbook

`pedidos-playbook` (versión 1.0.0) es un plugin de Claude Code que empaqueta la forma de trabajar de Pedidos Web. Se publica desde el repo `jsaenzgti/pedidos-web`, que actúa como marketplace con el nombre `pedidos-jsaenzgti`.

| Pieza | Para qué sirve |
|---|---|
| Subagente `revisor` | Revisa en local los cambios de la rama contra `AGENTS.md` y `REVIEW.md` antes de abrir un PR. Solo lee, nunca modifica |
| Skill `documentar-modulo` | Genera la documentación de un módulo o servicio en Markdown dentro de `docs/modulos` |
| Skill `planificador-proyectos-web` | Planifica y documenta un proyecto web con un proceso proporcional a su complejidad |
| Guardián (hook) | Bloquea toda acción que nombre un archivo `.env` o `.pem`, según la política de IA. Necesita `bash` (Git Bash en Windows) |
| Plantilla de `AGENTS.md` | Reglas de trabajo para copiar a cada repo: comandos, estilo, tests, seguridad y flujo spec → plan → tests → código |
| MCP `claude-academy` | Acceso de consulta a los datos de Claude Academy |

`REVIEW.md` y los permisos de cada repo no viajan en el plugin: van como plantilla y cada equipo los adapta.

**Cómo lo instala otro equipo** (en Claude Code, desde su repo):

```
/plugin marketplace add jsaenzgti/pedidos-web
/plugin install pedidos-playbook@pedidos-jsaenzgti
```

Después, `/plugin` debe mostrar `pedidos-playbook` y `/mcp` debe mostrar `claude-academy`. Para recibir una versión nueva, el equipo actualiza el marketplace; solo llega si en el plugin se subió el campo `version`.

## 5. Hoja de ruta a 6 y 12 meses

| Hito | Mes | Equipo | Champion | Cómo sabemos que se cumplió |
|---|---|---|---|---|
| Instalar `pedidos-playbook` y copiar la plantilla de `AGENTS.md` | 1 | Facturacion, Bodega, Reportes | Líder técnico de cada equipo | `/plugin` muestra el plugin en cada equipo y existe `AGENTS.md` en sus repos |
| Cumplir los objetivos del trimestre de la sección 3 | 3 | Los cuatro equipos | Líder de cada equipo | Los indicadores de la sección 3 se cumplen en las últimas 4 semanas |
| Facturacion y Reportes en nivel 3 (equipo con reglas) | 6 | Facturacion, Reportes | Líder técnico de cada equipo | Uso semanal sin interrupciones y revisor usado en los PR |
| Bodega en nivel 3 | 6 | Bodega | Líder técnico de Bodega | Tasa de aceptación ≥ 0,40 sostenida y PR con Claude Code ≥ 50 % |
| Todos los equipos en nivel 4 (proceso AI-native) | 12 | Los cuatro equipos | Un champion por equipo, coordinados por el líder de la BU | PR con Claude Code ≥ 80 % y tasa de aceptación estable en cada equipo |
| La BU mide y mejora por sí sola (nivel 5) | 12 | BU completa | Líder de la BU | Revisión mensual de indicadores con ajustes registrados |

El ciclo que se repite en cada trimestre: estandarizar (instalar el playbook) → medir (indicadores semanales) → ajustar (corregir lo que no mueve los números) → escalar (llevarlo al siguiente equipo).

## 6. Indicadores que vamos a seguir

Los mismos del CSV, por equipo y por semana:

- `usuarios_activos` y `sesiones`: miden si el uso es constante.
- `tasa_aceptacion`: mide si lo que sugiere la IA sirve. Mucho uso con baja aceptación es una señal de alerta, no un éxito.
- `lineas_aceptadas`: mide el volumen de código aprovechado.
- `pr_con_cc` sobre `pr_totales`: mide cuánto del trabajo real pasa por Claude Code.

Se registran cada semana y los revisa cada champion con su equipo. El líder de la BU los consolida una vez al mes y propone los ajustes del siguiente ciclo.

## 7. Riesgos y próximos pasos

- Los datos son ficticios y de solo 8 semanas → tratar los niveles como una primera lectura y confirmarlos con datos reales antes de fijar metas definitivas.
- Bodega puede seguir con mucho uso y poco provecho (aceptación de 0,11–0,17) → empezar por revisar con ellos qué pedidos fallan y darles el `AGENTS.md` y el revisor.
- Facturacion no tiene una práctica que replicar (15 sesiones en 8 semanas) → empezar con un champion y un caso concreto, en vez de forzar el uso en todo el equipo.
- El uso de Reportes cae a 0–1 usuario en semanas alternas → averiguar la causa (carga de trabajo, falta de casos de uso) antes de imponer metas.
- Con 80 % de PR hechos con IA en Pedidos Web, hay riesgo de confiar demasiado → mantener el revisor y la revisión humana antes de cada merge.
- El guardián depende de `bash` → confirmar que cada equipo tiene Git Bash antes de instalar el plugin.
- Próximos pasos: instalar el playbook en los tres equipos (mes 1), generar este plan en PDF para la gerencia y reunir los datos del primer trimestre.
