#!/bin/bash
# Frena toda acción cuya ruta o comando nombre un .env o un .pem.
# Edit y Write: solo se mira la ruta, no el contenido (para poder documentar .env).
ENTRADA=$(cat)
HERRAMIENTA=$(echo "$ENTRADA" | sed -n 's/.*"tool_name"[[:space:]]*:[[:space:]]*"\([^"]*\)".*/\1/p' | head -1)

case "$HERRAMIENTA" in
  Bash|PowerShell)
    # Solo el valor de "command": hasta la primera comilla sin escapar (no arrastra otros campos).
    OBJETIVO=$(echo "$ENTRADA" | sed -n -E 's/.*"command"[[:space:]]*:[[:space:]]*"(([^"\\]|\\.)*)".*/\1/p' | head -1) ;;
  *)
    OBJETIVO=$(echo "$ENTRADA" | grep -oE '"(file_path|path|pattern)"[[:space:]]*:[[:space:]]*"[^"]*"') ;;
esac

if echo "$OBJETIVO" | grep -qE '\.env($|[^A-Za-z0-9_-])|\.pem($|[^A-Za-z0-9_])'; then
  echo "Bloqueado por guardian.sh: la acción nombra un .env o un .pem (política de IA)." >&2
  exit 2
fi
exit 0
