namespace PedidosWeb.Core.Pedidos;

/// <summary>
/// Filtra pedidos por rango de fechas (HU-03).
/// </summary>
public static class PedidosPorFecha
{
    public static IEnumerable<Pedido> EntreFechas(
        IEnumerable<Pedido> pedidos,
        DateOnly desde,
        DateOnly hasta)
    {
        ArgumentNullException.ThrowIfNull(pedidos);

        if (desde > hasta)
        {
            throw new ArgumentException("La fecha desde no puede ser posterior a la fecha hasta.");
        }

        // Se evalúa aquí (ToList) para que la validación y el filtro ocurran al llamar el método.
        return pedidos
            .Where(pedido => pedido.Fecha >= desde && pedido.Fecha <= hasta)
            .ToList();
    }
}
