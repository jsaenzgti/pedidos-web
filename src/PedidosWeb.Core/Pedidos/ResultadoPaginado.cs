namespace PedidosWeb.Core.Pedidos;

/// <summary>Una página de resultados.</summary>
/// <param name="Items">Elementos de la página pedida.</param>
/// <param name="Total">Cantidad de elementos que cumplen el filtro, sin paginar.</param>
/// <param name="Pagina">Número de página devuelta.</param>
/// <param name="TamanoPagina">Tamaño de página usado.</param>
public sealed record ResultadoPaginado<T>(
    IReadOnlyList<T> Items,
    int Total,
    int Pagina,
    int TamanoPagina);
