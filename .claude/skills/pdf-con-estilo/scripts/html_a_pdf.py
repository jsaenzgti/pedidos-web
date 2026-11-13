"""Convierte un Markdown en un PDF con la plantilla de la Skill pdf-con-estilo.

Uso: python html_a_pdf.py <origen.md> [<destino.pdf>]
"""
import html
import sys
from pathlib import Path

import markdown
from playwright.sync_api import sync_playwright

PLANTILLA = Path(__file__).resolve().parent.parent / "assets" / "plantilla.html"
EXTENSIONES = ["tables", "fenced_code", "sane_lists"]


def separar_markdown(texto):
    """Devuelve (titulo, portada_md, cuerpo_md).

    titulo: el primer '# ' del documento.
    portada_md: lo que hay entre ese titulo y el primer '## '.
    cuerpo_md: desde el primer '## ' hasta el final.
    """
    lineas = texto.splitlines()
    titulo = "Documento"
    inicio = 0
    for indice, linea in enumerate(lineas):
        if linea.startswith("# "):
            titulo = linea[2:].strip()
            inicio = indice + 1
            break

    corte = len(lineas)
    en_codigo = False
    for indice in range(inicio, len(lineas)):
        if lineas[indice].lstrip().startswith("```"):
            en_codigo = not en_codigo
        if not en_codigo and lineas[indice].startswith("## "):
            corte = indice
            break

    portada_md = "\n".join(lineas[inicio:corte]).strip()
    cuerpo_md = "\n".join(lineas[corte:]).strip()
    return titulo, portada_md, cuerpo_md


def a_html(texto_md):
    return markdown.markdown(texto_md, extensions=EXTENSIONES)


def main():
    if len(sys.argv) < 2:
        print(__doc__)
        return 1

    origen = Path(sys.argv[1])
    destino = Path(sys.argv[2]) if len(sys.argv) > 2 else origen.with_suffix(".pdf")
    if not origen.is_file():
        print("No existe el archivo de origen: " + str(origen))
        return 1

    titulo, portada_md, cuerpo_md = separar_markdown(origen.read_text(encoding="utf-8"))
    pagina = PLANTILLA.read_text(encoding="utf-8")
    pagina = pagina.replace("{{TITULO}}", html.escape(titulo))
    pagina = pagina.replace("{{PORTADA}}", a_html(portada_md))
    pagina = pagina.replace("{{CONTENIDO}}", a_html(cuerpo_md))

    pie = (
        '<div style="width:100%;font-size:8px;color:#5b6770;padding:0 20mm;'
        'display:flex;justify-content:space-between;font-family:Arial,sans-serif;">'
        "<span>" + html.escape(titulo) + "</span>"
        '<span><span class="pageNumber"></span> / <span class="totalPages"></span></span></div>'
    )

    destino.parent.mkdir(parents=True, exist_ok=True)
    with sync_playwright() as playwright:
        navegador = playwright.chromium.launch()
        hoja = navegador.new_page()
        hoja.set_content(pagina, wait_until="load")
        hoja.pdf(
            path=str(destino),
            format="A4",
            print_background=True,
            prefer_css_page_size=True,
            display_header_footer=True,
            header_template="<div></div>",
            footer_template=pie,
            margin={"top": "0", "bottom": "12mm", "left": "0", "right": "0"},
        )
        navegador.close()

    print("PDF generado: " + str(destino))
    return 0


if __name__ == "__main__":
    sys.exit(main())
