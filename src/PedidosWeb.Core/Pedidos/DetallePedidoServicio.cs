using PedidosWeb.Core.Lineas;

namespace PedidosWeb.Core.Pedidos;

/// <summary>HU-02: arma el detalle de un pedido (cliente, líneas y total).</summary>
public sealed class DetallePedidoServicio
{
    private readonly IPedidoRepositorio _pedidos;
    private readonly ILineaPedidoRepositorio _lineas;

    public DetallePedidoServicio(IPedidoRepositorio pedidos, ILineaPedidoRepositorio lineas)
    {
        _pedidos = pedidos;
        _lineas = lineas;
    }

    /// <summary>El detalle del pedido, o null si el pedido no existe.</summary>
    public DetallePedido? ObtenerDetalle(int id)
    {
        Pedido? pedido = _pedidos.ObtenerPorId(id);

        if (pedido is null)
        {
            return null;
        }

        List<LineaPedido> lineas = _lineas.ObtenerPorPedido(id)
            .OrderBy(linea => linea.Numero)
            .ToList();

        List<LineaDetalle> lineasDetalle = new List<LineaDetalle>();

        foreach (LineaPedido linea in lineas)
        {
            lineasDetalle.Add(new LineaDetalle(
                linea.Numero,
                linea.Producto,
                linea.Cantidad,
                linea.PrecioUnitario,
                CalcularSubtotal(linea)));
        }

        return new DetallePedido(
            pedido.Id,
            pedido.Folio,
            pedido.Fecha,
            pedido.Estado,
            pedido.Sucursal,
            pedido.ClienteId,
            pedido.ClienteNombre,
            lineasDetalle,
            CalcularTotal(lineas));
    }

    /// <summary>Subtotal de una línea: cantidad por precio unitario (CLP, sin IVA).</summary>
    public static decimal CalcularSubtotal(LineaPedido linea)
    {
        return linea.Cantidad * linea.PrecioUnitario;
    }

    /// <summary>Total de un pedido: la suma de los subtotales de sus líneas. Sin líneas, 0.</summary>
    public static decimal CalcularTotal(IEnumerable<LineaPedido> lineas)
    {
        decimal total = 0m;

        foreach (LineaPedido linea in lineas)
        {
            total += CalcularSubtotal(linea);
        }

        return total;
    }
}
