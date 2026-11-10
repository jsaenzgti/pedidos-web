namespace PedidosWeb.Core.Pedidos;

/// <summary>Estados válidos de un pedido para filtrar (HU-01).</summary>
public static class EstadosPedido
{
    public const string Pendiente = "Pendiente";
    public const string Confirmado = "Confirmado";
    public const string Despachado = "Despachado";
    public const string Entregado = "Entregado";
    public const string Anulado = "Anulado";

    public static readonly IReadOnlyList<string> Validos = new[]
    {
        Pendiente,
        Confirmado,
        Despachado,
        Entregado,
        Anulado
    };
}
