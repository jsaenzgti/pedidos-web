namespace PedidosWeb.Core.Pedidos;

/// <summary>Resultado de validar un filtro: el filtro válido o los errores por campo.</summary>
public sealed record ResultadoFiltro(
    FiltroPedidos? Filtro,
    IReadOnlyDictionary<string, string[]> Errores)
{
    public bool EsValido => Filtro is not null;
}
