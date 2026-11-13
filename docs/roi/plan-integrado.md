# Plan integrado de adopción de IA · Pedidos Web

**Para:** Gerencia general · **Autor:** Julio Saenz · **Fecha:** 13-nov-2026

> **Recomendación:** extender la forma de trabajo con IA a los 25 desarrolladores, con revisión a los 3 meses: se sigue solo si el ahorro medido es de al menos 2,22 horas por desarrollador al mes. **ROI a 6 meses: entre −32,7 % y −2,2 %.** Decisión antes del 13-dic-2026.

*Preparado con apoyo de IA; cifras verificadas por Julio Saenz.*

## Informe ejecutivo (1 página)

### 1. Problema
Hay que decidir si la forma de trabajar con IA de Pedidos Web se extiende a los 25 desarrolladores de la empresa (25 asientos de Claude). Decidirlo ahora evita que los otros equipos usen la IA cada uno por su cuenta y sin medir.

### 2. Qué se hizo
Durante 12 semanas, un equipo de 6 desarrolladores trabajó 6 semanas como siempre y 6 con IA, midiendo entregas, fallas, defectos y retrabajo. Los datos son ficticios y la muestra es pequeña: este es un ejercicio de método.

### 3. Resultados
- El tiempo de entrega bajó de 6,58 a 5,17 días en promedio (4,38 en las semanas 9 a 12).
- Los defectos que llegan a producción bajaron de 2,0 a 1,5 por semana (1,0 en las semanas 9 a 12).
- Las dos primeras semanas con IA empeoraron (3 de 4 entregas con falla): el equipo estaba aprendiendo.
- **ROI a 6 meses, entre −32,7 % y −2,2 %:** negativo en ambos escenarios. Lo mueve el costo de 25 asientos y su capacitación (7.850 USD) frente a un beneficio medido solo en 6 personas (entre 5.280 y 7.680 USD). Si el ahorro fuera la mitad, el ROI sería −66,4 %.
- **Punto de equilibrio:** cada desarrollador debe ahorrar 2,22 horas al mes (unos 33 minutos por semana) para que la inversión se pague; el piloto reportó entre 5,67 y 7,33.

### 4. Recomendación
Extender la forma de trabajo a los 25 asientos, con la condición de medir el ahorro a los 3 meses: si no llega a 2,22 horas por desarrollador al mes (el punto de equilibrio), se detiene la ampliación.

### 5. Próximo paso
Julio Saenz presenta la solicitud y la gerencia decide antes del 13-dic-2026.

*Preparado con apoyo de IA; cifras verificadas por Julio Saenz.*

<div style="page-break-before: always"></div>

## Anexo

No repite los entregables del curso: los cita.

### Roadmap
Mes 1: instalar el paquete de reglas y herramientas del piloto (el «playbook») en los otros equipos. Mes 3: cumplir los objetivos del trimestre. Mes 6: Facturación, Reportes y Bodega en nivel 3. Mes 12: todos los equipos en nivel 4. · Respaldo: `docs/adopcion/plan-evolucion.md` (U9)

### Indicadores
Se mide cada semana el uso, la tasa de aceptación de lo que sugiere la IA y la proporción de cambios de código hechos con Claude Code. Metas del trimestre: tasa de aceptación de 0,40 o más en Bodega, 3 o más usuarios activos por semana en Facturación y Reportes, y 80 % de cambios con Claude Code sostenido en Pedidos Web. La calidad se cuida con pruebas automáticas (149 en verde al 13-nov-2026) y con una lista de revisión de cambios. · Respaldo: `docs/calidad/registro-u6.md` (U6) · `docs/checklist-revision.md` (U7)

### Gobernanza
Controles probados: bloqueo de lectura de archivos `.env` y `.pem`, bloqueo de edición del sistema legado, un guardián que revisa cada comando y un chequeo de pruebas tras cada edición. Cada cambio de código declara el uso de IA. No funcionaron en el editor las reglas que preguntan antes de actuar (descargas y edición de los resultados de referencia del cálculo): ese riesgo queda abierto. · Respaldo: `docs/gobernanza/politica-ia.md` (U8)

### ROI
Costo de 7.850 USD a 6 meses: 25 asientos a 25 USD al mes (precio oficial con facturación mensual, consultado el 13-nov-2026; con facturación anual serían 20 USD), capacitación de 8 horas por persona y la curva de aprendizaje. El beneficio cuenta solo las 6 personas del piloto, a 20 USD la hora y 100 USD por defecto en producción (ambos estimados). Escenarios: conservador −32,7 % y optimista −2,2 %. Sensibilidad: con el ahorro a la mitad, −66,4 %. Las horas ahorradas las reporta el propio equipo. Punto de equilibrio (ROI = 0): 332,5 horas ahorradas en total, o 2,22 horas por desarrollador al mes con los 25 desarrolladores; es el 39 % de lo que reportó el piloto en el escenario conservador (5,67 horas por desarrollador al mes), pero no está probado que los otros 19 lo repitan. Si solo se beneficiaran los 6 del piloto, harían falta 9,24 horas por desarrollador al mes, más que lo medido. · Respaldo: `docs/roi/modelo-roi.md` · `docs/migracion/defectos-corregidos.md` (U5)

### Evolución
Hoy: Pedidos Web en nivel 4, Bodega y Reportes en nivel 2, Facturación en nivel 1. Meta a 12 meses: los cuatro equipos en nivel 4. · Respaldo: `docs/adopcion/plan-evolucion.md` (U9)
