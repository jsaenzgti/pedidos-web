namespace PedidosWeb.Core.Totales;

public class ResultadoTotales
{
    public decimal Subtotal { get; set; }

    public decimal BaseDescuento { get; set; }

    public decimal PorcDescuento { get; set; }

    public decimal Descuento { get; set; }

    public decimal Neto { get; set; }

    public decimal Flete { get; set; }

    public decimal Iva { get; set; }

    public decimal Total { get; set; }
}
