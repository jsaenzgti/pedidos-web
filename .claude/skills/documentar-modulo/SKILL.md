---
name: documentar-modulo
description: Genera la documentación de un módulo o servicio del proyecto en un archivo Markdown dentro de docs/modulos. Úsala cuando pidan documentar, explicar o describir un módulo o servicio, aunque no nombren la Skill.
---

# Documentar un módulo

## Pasos
1. Identifica el módulo que piden. Si no está claro cuál es, pregunta y espera la respuesta.
2. Lee el código del módulo en `src/` y sus tests en `tests/`. No asumas nada: si algo no se entiende (para qué sirve, por qué se hace así), pregunta al usuario y espera su respuesta.
3. Escribe `docs/modulos/<Modulo>.md`, en español, con estas secciones: Propósito, API pública, Ejemplo de uso y Diagrama Mermaid.
4. Escribe solo lo que el código hace hoy y lo que el usuario te confirmó. No inventes comportamiento y saca los ejemplos del código o de los tests.
5. Verifica que lo escrito coincide con el código (nombres, parámetros, valores devueltos y ejemplos). Si hay una diferencia que no puedes resolver, pregunta en vez de decidir tú.
6. No modifiques el código del módulo: solo escribe la documentación.
