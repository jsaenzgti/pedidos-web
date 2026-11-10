namespace PedidosWeb.Core.Pedidos;

/// <summary>HU-01: lista pedidos de un cliente con filtros opcionales, orden y paginación.</summary>
public sealed class ListarPedidosServicio
{
    private readonly IPedidoRepositorio _repositorio;

    public ListarPedidosServicio(IPedidoRepositorio repositorio)
    {
        _repositorio = repositorio;
    }

    /// <summary>Valida los textos de la query string y, si son válidos, consulta los pedidos.</summary>
    public ResultadoListado Listar(
        string? clienteId,
        string? estado,
        string? desde,
        string? hasta,
        string? pagina,
        string? tamanoPagina)
    {
        ResultadoFiltro resultadoFiltro = FiltroPedidos.Crear(clienteId, estado, desde, hasta, pagina, tamanoPagina);

        if (!resultadoFiltro.EsValido)
        {
            return new ResultadoListado(null, resultadoFiltro.Errores);
        }

        ResultadoPaginado<Pedido> paginaPedidos = Consultar(resultadoFiltro.Filtro!);
        return new ResultadoListado(paginaPedidos, resultadoFiltro.Errores);
    }

    /// <summary>Aplica el filtro, el orden (Fecha, luego Id) y la paginación.</summary>
    // TODO(U5): hoy se cargan todos los pedidos y se filtra en memoria (son 45 de ejemplo).
    // Con PostgreSQL el filtro, el orden y la paginación deben ejecutarse en la base:
    // habrá que agregar a IPedidoRepositorio un método como Buscar(FiltroPedidos).
    // Los tests de este servicio siguen valiendo, porque prueban el resultado y no el repositorio.
    private ResultadoPaginado<Pedido> Consultar(FiltroPedidos filtro)
    {
        IEnumerable<Pedido> consulta = _repositorio.ObtenerTodos()
            .Where(p => string.Equals(p.ClienteId, filtro.ClienteId, StringComparison.OrdinalIgnoreCase));

        if (filtro.Estado is not null)
        {
            consulta = consulta.Where(p => string.Equals(p.Estado, filtro.Estado, StringComparison.OrdinalIgnoreCase));
        }

        if (filtro.Desde.HasValue)
        {
            consulta = consulta.Where(p => p.Fecha >= filtro.Desde.Value);
        }

        if (filtro.Hasta.HasValue)
        {
            consulta = consulta.Where(p => p.Fecha <= filtro.Hasta.Value);
        }

        List<Pedido> ordenados = consulta
            .OrderBy(p => p.Fecha)
            .ThenBy(p => p.Id)
            .ToList();

        int total = ordenados.Count;

        // long evita desbordar con páginas enormes; si se pasa de la última, la página queda vacía.
        long saltar = (long)(filtro.Pagina - 1) * filtro.TamanoPagina;
        IReadOnlyList<Pedido> items = saltar >= total
            ? new List<Pedido>()
            : ordenados.Skip((int)saltar).Take(filtro.TamanoPagina).ToList();

        return new ResultadoPaginado<Pedido>(items, total, filtro.Pagina, filtro.TamanoPagina);
    }
}
