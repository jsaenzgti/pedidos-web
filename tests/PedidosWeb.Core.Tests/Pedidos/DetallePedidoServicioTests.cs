using PedidosWeb.Core.Lineas;
using PedidosWeb.Core.Pedidos;

namespace PedidosWeb.Core.Tests.Pedidos;

/// <summary>Tests de HU-02 (specs/HU-02-detalle-pedido/spec.md). Incremento 1: cálculo del total.</summary>
public class DetallePedidoServicioTests
{
    private readonly LineaPedidoRepositorioEnMemoria _lineas = new();
    private readonly PedidoRepositorioEnMemoria _pedidos = new();

    [Fact]
    public void CalcularSubtotal_Linea_EsCantidadPorPrecioUnitario()
    {
        // CA-01: los subtotales del pedido 8 son 64500, 35400 y 20200.
        var subtotales = _lineas.ObtenerPorPedido(8)
            .Select(DetallePedidoServicio.CalcularSubtotal)
            .ToArray();

        Assert.Equal(new[] { 64500m, 35400m, 20200m }, subtotales);
    }

    [Fact]
    public void CalcularTotal_Pedido8_Es120100()
    {
        // CA-01
        var total = DetallePedidoServicio.CalcularTotal(_lineas.ObtenerPorPedido(8));

        Assert.Equal(120100m, total);
    }

    [Fact]
    public void CalcularTotal_SinLineas_EsCero()
    {
        // CA-04 (la lógica; el 200 con lineas vacío se prueba en el incremento 2).
        var total = DetallePedidoServicio.CalcularTotal(Array.Empty<LineaPedido>());

        Assert.Equal(0m, total);
    }

    [Fact]
    public void CalcularTotal_LosPedidosDeEjemplo_CoincideConElTotalGuardado()
    {
        // CA-01: el total calculado desde las líneas iguala el Total del pedido, en los 45.
        var distintos = _pedidos.ObtenerTodos()
            .Where(p => DetallePedidoServicio.CalcularTotal(_lineas.ObtenerPorPedido(p.Id)) != p.Total)
            .Select(p => p.Id)
            .ToList();

        Assert.Empty(distintos);
    }
}
