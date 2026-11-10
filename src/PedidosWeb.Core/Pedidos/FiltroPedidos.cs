using System.Globalization;

namespace PedidosWeb.Core.Pedidos;

/// <summary>
/// Filtro de listado ya validado y con tipos reales (HU-01).
/// Se crea con <see cref="Crear"/> a partir de los textos de la query string.
/// </summary>
/// <param name="ClienteId">Código del cliente, sin espacios al inicio ni al final.</param>
/// <param name="Estado">Estado a filtrar, o null para todos.</param>
/// <param name="Desde">Fecha mínima inclusiva, o null.</param>
/// <param name="Hasta">Fecha máxima inclusiva, o null.</param>
/// <param name="Pagina">Número de página, desde 1.</param>
/// <param name="TamanoPagina">Pedidos por página, de 1 a 50.</param>
public sealed record FiltroPedidos(
    string ClienteId,
    string? Estado,
    DateOnly? Desde,
    DateOnly? Hasta,
    int Pagina,
    int TamanoPagina)
{
    public const int PaginaPorDefecto = 1;
    public const int TamanoPaginaPorDefecto = 20;
    public const int TamanoPaginaMaximo = 50;

    private const string FormatoFecha = "yyyy-MM-dd";

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
        int numeroPagina = LeerEntero(pagina, "pagina", PaginaPorDefecto, 1, int.MaxValue, errores);
        int tamano = LeerEntero(tamanoPagina, "tamanoPagina", TamanoPaginaPorDefecto, 1, TamanoPaginaMaximo, errores);

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
            string rango = maximo == int.MaxValue
                ? "Debe ser mayor o igual a " + minimo + "."
                : "Debe estar entre " + minimo + " y " + maximo + ".";
            errores[campo] = new[] { rango };
            return valorPorDefecto;
        }

        return valor;
    }
}
