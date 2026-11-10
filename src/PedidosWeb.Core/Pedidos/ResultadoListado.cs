namespace PedidosWeb.Core.Pedidos;

/// <summary>Resultado de listar pedidos: la página de pedidos o los errores de validación por campo.</summary>
public sealed record ResultadoListado(
    ResultadoPaginado<Pedido>? Pagina,
    IReadOnlyDictionary<string, string[]> Errores)
{
    public bool EsValido => Pagina is not null;
}
