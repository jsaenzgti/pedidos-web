# Política de uso de IA · pedidos-web

Probado el 2026-11-13 en este repo. Solo se afirma lo que se probó; lo demás dice «no probado».

## 1. Datos que nunca entran a Claude

- Claves, contraseñas y tokens (por ejemplo `DB_PASSWORD`, `API_KEY` o el token de GitHub).
- Archivos `.env` y `.pem`, y cadenas de conexión.
- Datos reales de clientes o pedidos. En este repo todo es ficticio.
- La clave que el legado tiene escrita en su código (AU-01 de la auditoría): no se copia ni se repite.

## 2. Controles y si se probaron

| Qué se quiere evitar | Control | ¿Probado? | Resultado |
|---|---|---|---|
| Que Claude lea `.env` | Regla `deny` de lectura | Sí | Bloqueado |
| Que Claude lea un `.pem` | Regla `deny` de lectura | Sí | Bloqueado |
| Que Claude modifique `legado/` | Regla `deny` de edición (U4) | Sí | Bloqueado |
| Un comando que nombre `.env` o `.pem` | Hook `guardian.sh`, que revisa cada acción antes de ejecutarla | Sí | Bloqueó `cat .env`. Tenía un error (bloqueaba de más) y se corrigió |
| Que Claude use `curl` o edite `golden-master.csv` sin avisar | Regla `ask` (pregunta antes) | Sí, **no funcionó** | No preguntó en la extensión de VS Code, en dos modos |
| Lo mismo con `wget`, `Invoke-WebRequest` y `WebFetch` | Regla `ask` | **No probado** | — |
| Que un cambio rompa los tests | Hook que corre `dotnet test` tras cada edición (U5) | Sí | Muestra los fallos |

## 3. Lo no cubierto

- **Buscar un secreto sin nombrar el archivo.** `grep -r API_KEY .` no se bloqueó y encontró `./.env`. Riesgo: ver un secreto sin que ninguna regla lo note. Mitigación: nunca guardar un secreto real en el repo.
- **Las reglas `ask` no preguntaron** en VS Code. Riesgo: una descarga o una edición del golden master pasa sin aviso. Mitigación: lo que nunca debe pasar va en `deny`. Falta comprobar si preguntan en la terminal.
- **El hook solo lee el texto del comando.** Por eso también bloquea nombres como `.env.example`, y no se probó contra comodines ni nombres armados por partes.
- **Sin aislamiento del sistema operativo:** no se probó.

## 4. Escenarios

| Escenario | Control que lo cubre |
|---|---|
| Alguien le pide a Claude leer el `.env` o una llave `.pem` | Regla `deny` y hook `guardian.sh` |
| Claude intenta modificar `legado/`, que es solo lectura | Regla `deny` de edición |
| Un cambio rompe el cálculo del IVA | Hook `dotnet test`, CI y revisión del PR |

## 5. Transparencia en los PR

Cada PR incluye esta línea:

`Uso de IA: <herramienta y modelo>; <qué parte generó>; <quién la revisó>.`
