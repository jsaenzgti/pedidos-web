# Modelo de ROI · piloto de Pedidos Web

> Plantilla de la Unidad 10. Complétala con Claude Code en las etapas 1 y 2 y revísala tú.
> Fuente de datos: `docs/roi/datos-piloto.csv` (equipo Pedidos Web de 6 devs; semanas 1-6 antes, 7-12 con IA; datos ficticios) y tus entregables de las unidades 5 a 9.
> Regla: ningún número sin su fuente o su supuesto. Si un dato no está, escribe FALTA.

**Autor:** Julio Saenz · **Fecha:** 13-nov-2026 · **Horizonte:** 6 meses

## 1. Datos (etapa 1)

| Dato | Antes | Con IA | Fuente (archivo y semanas) |
|---|---|---|---|
| Despliegues por semana | 2,0 | 3,0 (sem. 9-12: 3,5) | `datos-piloto.csv`, semanas 1-6 frente a 7-12 y 9-12 |
| Lead time (días) | 6,58 | 5,17 (sem. 9-12: 4,38) | `datos-piloto.csv`, semanas 1-6 frente a 7-12 y 9-12 |
| Change fail rate | 41,7 % (5 de 12 despliegues) | 33,3 % (6 de 18) (sem. 9-12: 21,4 %, 3 de 14) | `datos-piloto.csv`: `cambios_con_falla` / `despliegues` |
| Tiempo de recuperación (horas por falla) | 6,0 (30 h en 5 fallas) | 4,0 (24 h en 6 fallas) (sem. 9-12: 3,3) | `datos-piloto.csv`: `horas_recuperacion` / `cambios_con_falla` |
| Defectos escapados por semana | 2,0 | 1,5 (sem. 9-12: 1,0) | `datos-piloto.csv`, semanas 1-6 frente a 7-12 y 9-12 |
| Horas de retrabajo por semana | 12,0 | 10,7 (sem. 9-12: 8,75) | `datos-piloto.csv`, semanas 1-6 frente a 7-12 y 9-12 |
| Horas ahorradas por semana (equipo) | 0 | 8,5 (sem. 9-12: 11,0) | `datos-piloto.csv`, semanas 7-12 y 9-12. Las reporta el equipo (auto-reporte), no se miden |

Promedios simples por semana, calculados a mano con los valores del CSV. Para el CFR, el cociente va sobre el total de despliegues del período, no sobre el promedio de porcentajes semanales.

**Evidencia propia** (una línea por archivo: qué dato aporta):

- `docs/migracion/defectos-corregidos.md` (U5): D1 (redondeo R05) cambia 13 de 60 casos del golden master y la suma del total pasa de 58.202.135 a 58.202.144 (+9 pesos). H1 (umbrales de descuento) queda sin corregir. No trae horas ni costo por defecto: costo por defecto en producción = FALTA.
- `docs/calidad/registro-u6.md` (U6): 132 tests en Core y 7 en Api, todos en verde. Un cambio del IVA a 0,19 fijo (PR #1, 12-nov-2026) lo detectó el test `Calcular_TasaIvaCero_IvaCeroYTotalSinIva` y `@claude` lo diagnosticó bien. De los 5 tests nuevos, solo 1 se probó rompiendo el código a mano.
- `docs/checklist-revision.md` (U7): el PR #7 (HU-03) lo revisó Claude en 4 turnos, con 0 permisos negados y sin hallazgos; no hubo hallazgo humano. Es una sola observación, no una tasa de defectos detectados por la revisión.
- `docs/gobernanza/politica-ia.md` (U8): controles probados (`deny`, hook `guardian.sh`, hook `dotnet test`). Las reglas `ask` no preguntaron en VS Code. Aporta riesgos y controles; no aporta horas ni costos. Costo de la gobernanza = FALTA.
- `docs/adopcion/plan-evolucion.md` (U9): Pedidos Web en nivel 4, con 86 de 107 PR con Claude Code (80 %) y aceptación de 0,74 a 0,78, según `docs/adopcion/analytics-ficticio.csv` (4 equipos, 8 semanas). Es otro período y otra fuente que `datos-piloto.csv`: no se mezclan en los promedios.

**Lo que veo en las semanas 7 y 8:** con IA empeoran frente al "antes": 3 de 4 despliegues con falla (75 %, contra 41,7 %), retrabajo de 14,5 h por semana (contra 12,0) y lead time de 6,75 días (contra 6,58); los defectos suben a 2,5 por semana (contra 2,0). Se ve la curva de aprendizaje, y en esas dos semanas el equipo reporta solo 3 y 4 h ahorradas, mientras el retrabajo medido sube. La mejora aparece desde la semana 9.

**Datos que el piloto no trae (FALTA):** precio del asiento, horas o costo de capacitación, valor hora, costo de un defecto en producción, horizonte y porcentajes de la curva de adopción. Se resuelven en la etapa 2.

## 2. Supuestos (etapa 2)

Tipos: **oficial** (publicado, con fecha) · **medido** (dato del piloto) · **estimado** (juicio del equipo, el más débil).

| Supuesto | Valor | Tipo | Fuente |
|---|---|---|---|
| Horizonte | 6 meses = 26 semanas (24 con beneficio, tras 2 de curva) | estimado | Decisión de Julio Saenz; coincide con el hito de 6 meses de `docs/adopcion/plan-evolucion.md` |
| Número de asientos | 25 | estimado | Dato de Julio Saenz |
| Asiento | 25 USD por asiento al mes | oficial | claude.com/pricing, Team Standard con facturación mensual (con facturación anual son 20 USD), consultado el 13-nov-2026 |
| Capacitación | 8 h por dev × 25 devs, a valor hora; sin otro costo | estimado | Dato de Julio Saenz |
| Curva de adopción | 2 semanas sin beneficio y 5 h de retrabajo extra: (15 − 12) + (14 − 12) h | medido | `datos-piloto.csv`, semanas 7-8 frente a la media de 12 h de las semanas 1-6 |
| Valor hora | 20 USD | estimado | Dato de Julio Saenz, sin fuente publicada |
| Horas ahorradas | Conservador 8,5 h/sem; optimista 11 h/sem (equipo de 6 devs) | medido (auto-reporte) | `datos-piloto.csv`: media de semanas 7-12 y de 9-12 |
| Defectos evitados | Conservador 0,5/sem (2,0 → 1,5); optimista 1,0/sem (2,0 → 1,0) | medido | `datos-piloto.csv`: semanas 1-6 frente a 7-12 y 9-12 |
| Costo de un defecto en producción | 100 USD | estimado | Dato de Julio Saenz, sin fuente publicada |
| Escalado del beneficio | Solo los 6 devs del piloto; los 19 asientos restantes se cuentan como costo sin beneficio | estimado | Decisión de Julio Saenz (la más conservadora) |

## 3. Costos

| Costo | Cálculo | USD |
|---|---|---|
| Asientos | 25 asientos × 25 USD × 6 meses | 3.750 |
| Capacitación | 25 devs × 8 h × 20 USD | 4.000 |
| Curva de adopción | 5 h de retrabajo extra × 20 USD (las 2 semanas sin beneficio ya están excluidas en la sección 4) | 100 |
| **Total** | 3.750 + 4.000 + 100 | **7.850** |

## 4. Beneficios y escenarios

ROI = (beneficio − costo) / costo

Beneficio solo de los 6 devs del piloto, sobre 24 semanas.

| | Conservador | Optimista |
|---|---|---|
| Horas ahorradas × valor hora | 8,5 h × 24 sem × 20 USD = 4.080 | 11 h × 24 sem × 20 USD = 5.280 |
| Defectos evitados × costo en producción | 0,5 × 24 sem × 100 USD = 1.200 | 1,0 × 24 sem × 100 USD = 2.400 |
| **Beneficio total** | **5.280** | **7.680** |
| **ROI** | (5.280 − 7.850) / 7.850 = **−32,7 %** | (7.680 − 7.850) / 7.850 = **−2,2 %** |

El ROI en 6 meses va **entre −32,7 % y −2,2 %**: negativo en ambos escenarios. No es un error: el costo cubre 25 asientos y el beneficio medido es de 6 devs.

## 5. Sensibilidad

**Supuesto que más pesa:** el escalado del beneficio. Los 19 asientos que no tienen beneficio medido son el 76 % de los asientos y de la capacitación (5.890 de 7.850 USD: 76 % de 7.750 entre asientos y capacitación). Con el valor hora de 20 USD, cada hora ahorrada vale poco frente al costo de 25 asientos.

| Si… | ROI |
|---|---|
| El ahorro (horas y defectos evitados) es la mitad del conservador: (4.080 + 1.200) / 2 = 2.640 USD | (2.640 − 7.850) / 7.850 = **−66,4 %** |

## 6. Punto de equilibrio (Tu reto)

**Pregunta:** ¿cuántas horas ahorradas por dev al mes dejan el ROI en 0?

**Qué vale el ROI en el punto de equilibrio:** ROI = 0 significa (beneficio − costo) / costo = 0, o sea **beneficio = costo = 7.850 USD**.

**Qué se despeja y qué queda fijo:**
- Se despeja: las horas ahorradas (H).
- Quedan fijos: costo (7.850 USD), valor hora (20 USD), defectos evitados del escenario conservador (0,5 por semana → 1.200 USD), 24 semanas con beneficio, que se toman como 6 meses de 4 semanas, y 25 devs.

**Despeje paso a paso:**
1. Beneficio = H × 20 USD + 1.200 USD = 7.850 USD
2. H × 20 = 7.850 − 1.200 = 6.650 USD
3. H = 6.650 / 20 = **332,5 horas** ahorradas en total en las 24 semanas (13,85 h por semana del conjunto)
4. Por dev al mes: 332,5 h / 25 devs / 6 meses = **2,22 h por dev al mes** (unos 33 minutos por semana)

**Comprobación:** 2,217 h × 25 devs × 6 meses × 20 USD = 6.650 USD; 6.650 + 1.200 = 7.850 USD; ROI = (7.850 − 7.850) / 7.850 = 0 %.

**Comparación con el piloto** (`datos-piloto.csv`, horas auto-reportadas, 6 devs, 4 semanas por mes):

| | Horas por dev al mes | Cálculo |
|---|---|---|
| Piloto conservador (semanas 7-12) | 5,67 | 8,5 h/sem / 6 devs × 4 |
| Piloto optimista (semanas 9-12) | 7,33 | 11 h/sem / 6 devs × 4 |
| **Punto de equilibrio con 25 devs** | **2,22** | 39 % del conservador y 30 % del optimista |
| Punto de equilibrio si solo se benefician los 6 devs | 9,24 | 332,5 h / 6 devs / 6 meses: 63 % más que el conservador y 26 % más que el optimista |

**¿Es realista?** Con los 25 devs, la meta de 2,22 h por dev al mes es menos de la mitad de lo que reportó el piloto, así que es alcanzable **si los otros 19 devs repiten, al menos, el 39 % del ahorro del piloto**. Eso no está probado: la muestra es de 6 devs, las horas son auto-reportadas y los otros equipos están en niveles de madurez más bajos (Facturación en nivel 1, Bodega y Reportes en nivel 2, según `docs/adopcion/plan-evolucion.md`). Si solo se beneficiaran los 6 del piloto, la meta (9,24 h) supera lo medido, y por eso el ROI del modelo es negativo.

**Una línea para la gerencia:** cada desarrollador debe ahorrar 2,2 horas al mes (unos 33 minutos por semana) para que la inversión se pague; el piloto reportó entre 5,7 y 7,3.

## 7. Limitaciones

- Los datos son ficticios y la muestra es pequeña: este ROI es un ejercicio de método.
- Las horas ahorradas son auto-reportadas por el equipo, no medidas; en las semanas 7 y 8 reportan ahorro mientras el retrabajo medido sube.
- El beneficio sale de 6 devs y 12 semanas; no se probó que los otros equipos lo repitan.
- Valor hora (20 USD), costo de un defecto (100 USD), horas de capacitación y número de asientos son estimados, sin fuente publicada.
- No se cuenta el costo de gobernanza (U8) ni la administración del plugin: FALTA.
- No se cuenta como beneficio la mejora en lead time ni en frecuencia de despliegue (DORA) porque no se convirtió a USD.
