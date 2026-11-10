using PedidosWeb.Core.Lineas;

namespace PedidosWeb.Core.Pedidos;

/// <summary>HU-02: arma el detalle de un pedido (cliente, líneas y total).</summary>
public sealed class DetallePedidoServicio
{
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
