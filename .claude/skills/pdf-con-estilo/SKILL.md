---
name: pdf-con-estilo
description: Convierte un documento Markdown (.md) en un PDF con diseño profesional: portada, títulos, tablas legibles y número de página. Úsala siempre que pidan generar, exportar o convertir a PDF un informe, plan, política o cualquier .md del proyecto, aunque no nombren la Skill.
---

# PDF con estilo

Separa el diseño del contenido: el diseño vive en `assets/plantilla.html` y el contenido es el Markdown. Para un documento nuevo solo cambias el Markdown; no se toca el CSS.

## Pasos
1. Confirma la ruta del `.md` de origen y dónde debe quedar el PDF (por defecto, junto al `.md` con el mismo nombre y extensión `.pdf`).
2. Ejecuta, desde la raíz del repo:
   `python .claude/skills/pdf-con-estilo/scripts/html_a_pdf.py <origen.md> [<destino.pdf>]`
3. Revisa el PDF generado y comprueba: portada, títulos, tablas completas y fondos con color.
4. Si algo se ve mal, corrige el Markdown. Cambia `assets/plantilla.html` solo si el problema afecta a todos los documentos.

## Cómo se arma el documento
- El primer `# Título` del Markdown es el título de la portada.
- Todo lo que está entre ese título y el primer `## Sección` (notas, autor, fecha, etc.) va también en la portada.
- El resto del Markdown va en las páginas siguientes. Se admiten tablas, listas, citas y bloques de código.

## Requisitos
Python 3 con `playwright` y `markdown` instalados (`python -m pip install playwright markdown`) y Chromium (`python -m playwright install chromium`).

## Notas
- El script imprime los colores de fondo; el CSS no debe desactivarlos.
- El script no modifica el Markdown de origen.
