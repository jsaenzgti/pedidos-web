namespace PedidosWeb.Core.Totales;

public class LineaCalculo
{
    public LineaCalculo(int cantidad, decimal precioUnitario, string categoria)
    {
        Cantidad = cantidad;
        PrecioUnitario = precioUnitario;
        Categoria = categoria;
    }

    public int Cantidad { get; }

    public decimal PrecioUnitario { get; }

    public string Categoria { get; }
}
