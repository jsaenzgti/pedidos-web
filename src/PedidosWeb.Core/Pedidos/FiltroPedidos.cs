using System.Globalization;

namespace PedidosWeb.Core.Pedidos;

/// <summary>
/// Filtro de listado ya validado y con tipos reales (HU-01).
/// El constructor es privado: la única forma de obtener uno es <see cref="Crear"/>,
/// que valida los textos de la query string. Así un filtro existente siempre es válido.
/// </summary>
public sealed record FiltroPedidos
{
    public const int PaginaPorDefecto = 1;
    public const int TamanoPaginaPorDefecto = 20;
    public const int TamanoPaginaMaximo = 50;

    private const string FormatoFecha = "yyyy-MM-dd";

    /// <summary>Código del cliente, sin espacios al inicio ni al final.</summary>
    public string ClienteId { get; }

    /// <summary>Estado a filtrar (en su forma canónica), o null para todos.</summary>
    public string? Estado { get; }

    /// <summary>Fecha mínima inclusiva, o null.</summary>
    public DateOnly? Desde { get; }

    /// <summary>Fecha máxima inclusiva, o null.</summary>
    public DateOnly? Hasta { get; }

    /// <summary>Número de página, desde 1.</summary>
    public int Pagina { get; }

    /// <summary>Pedidos por página, de 1 a 50.</summary>
    public int TamanoPagina { get; }

    private FiltroPedidos(
        string clienteId,
        string? estado,
        DateOnly? desde,
        DateOnly? hasta,
        int pagina,
        int tamanoPagina)
    {
        ClienteId = clienteId;
        Estado = estado;
        Desde = desde;
        Hasta = hasta;
        Pagina = pagina;
        TamanoPagina = tamanoPagina;
    }

    /// <summary>
    /// Valida los textos recibidos y devuelve el filtro o los errores por campo.
    /// Un parámetro nulo cuenta como "no enviado". Solo clienteId se recorta con trim (spec);
    /// el resto se valida tal cual llega. Se informan todos los campos inválidos, no solo el primero.
    /// </summary>
    public static ResultadoFiltro Crear(
        string? clienteId,
        string? estado,
        string? desde,
        string? hasta,
        string? pagina,
        string? tamanoPagina)
    {
        var errores = new Dictionary<string, string[]>();

        // clienteId: obligatorio.
        string clienteIdLimpio = (clienteId ?? string.Empty).Trim();
        if (clienteIdLimpio.Length == 0)
        {
            errores["clienteId"] = new[] { "El clienteId es obligatorio." };
        }

        // estado: opcional, uno de los 5 estados, sin distinguir mayúsculas.
        string? estadoNormalizado = null;
        if (estado is not null)
        {
            estadoNormalizado = EstadosPedido.Validos.FirstOrDefault(
                valido => string.Equals(valido, estado, StringComparison.OrdinalIgnoreCase));

            if (estadoNormalizado is null)
            {
                errores["estado"] = new[]
                {
                    "El estado no es válido. Valores permitidos: " + string.Join(", ", EstadosPedido.Validos) + "."
                };
            }
        }

        // desde y hasta: opcionales, formato yyyy-MM-dd.
        DateOnly? fechaDesde = LeerFecha(desde, "desde", errores);
        DateOnly? fechaHasta = LeerFecha(hasta, "hasta", errores);

        if (fechaDesde.HasValue && fechaHasta.HasValue && fechaDesde.Value > fechaHasta.Value)
        {
            errores["desde"] = new[] { "La fecha desde no puede ser posterior a la fecha hasta." };
        }

        // pagina y tamanoPagina: enteros, con valores por defecto.
        int numeroPagina = LeerEntero(
            pagina, "pagina", PaginaPorDefecto, 1, int.MaxValue, "Debe ser mayor o igual a 1.", errores);
        int tamano = LeerEntero(
            tamanoPagina, "tamanoPagina", TamanoPaginaPorDefecto, 1, TamanoPaginaMaximo,
            "Debe estar entre 1 y " + TamanoPaginaMaximo + ".", errores);

        if (errores.Count > 0)
        {
            return new ResultadoFiltro(null, errores);
        }

        var filtro = new FiltroPedidos(clienteIdLimpio, estadoNormalizado, fechaDesde, fechaHasta, numeroPagina, tamano);
        return new ResultadoFiltro(filtro, errores);
    }

    private static DateOnly? LeerFecha(string? texto, string campo, Dictionary<string, string[]> errores)
    {
        if (texto is null)
        {
            return null;
        }

        bool esValida = DateOnly.TryParseExact(
            texto,
            FormatoFecha,
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out DateOnly fecha);

        if (!esValida)
        {
            errores[campo] = new[] { "La fecha debe tener el formato yyyy-MM-dd." };
            return null;
        }

        return fecha;
    }

    private static int LeerEntero(
        string? texto,
        string campo,
        int valorPorDefecto,
        int minimo,
        int maximo,
        string mensajeFueraDeRango,
        Dictionary<string, string[]> errores)
    {
        if (texto is null)
        {
            return valorPorDefecto;
        }

        bool esNumero = int.TryParse(texto, NumberStyles.None, CultureInfo.InvariantCulture, out int valor);

        if (!esNumero)
        {
            errores[campo] = new[] { "Debe ser un número entero." };
            return valorPorDefecto;
        }

        if (valor < minimo || valor > maximo)
        {
            errores[campo] = new[] { mensajeFueraDeRango };
            return valorPorDefecto;
        }

        return valor;
    }
}
