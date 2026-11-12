using PedidosWeb.Core.Pedidos;

namespace PedidosWeb.Core.Tests.Pedidos;

/// <summary>Tests de HU-03 (specs/HU-03-pedidos-por-fecha/spec.md): un test por criterio de aceptación (CA-01 a CA-04).</summary>
public class PedidosPorFechaTests
{
    private static readonly DateOnly Desde = new DateOnly(2026, 10, 1);
    private static readonly DateOnly Hasta = new DateOnly(2026, 10, 31);

    private static Pedido CrearPedido(int id, DateOnly fecha)
    {
        return new Pedido(id, "PED-ANT-" + id.ToString("000000"), "C-001", "Cliente de prueba", fecha, "Pendiente", 10000m, "ANT");
    }

    [Fact]
    public void EntreFechas_PedidoConFechaDesde_Aparece()
    {
        List<Pedido> pedidos = new List<Pedido> { CrearPedido(1, Desde) };

        List<Pedido> resultado = PedidosPorFecha.EntreFechas(pedidos, Desde, Hasta).ToList();

        Assert.Single(resultado);
        Assert.Equal(1, resultado[0].Id);
    }

    [Fact]
    public void EntreFechas_PedidoConFechaHasta_Aparece()
    {
        List<Pedido> pedidos = new List<Pedido> { CrearPedido(2, Hasta) };

        List<Pedido> resultado = PedidosPorFecha.EntreFechas(pedidos, Desde, Hasta).ToList();

        Assert.Single(resultado);
        Assert.Equal(2, resultado[0].Id);
    }

    [Fact]
    public void EntreFechas_PedidoFueraDelRango_NoAparece()
    {
        List<Pedido> pedidos = new List<Pedido>
        {
            CrearPedido(3, Desde.AddDays(-1)),
            CrearPedido(4, Hasta.AddDays(1))
        };

        List<Pedido> resultado = PedidosPorFecha.EntreFechas(pedidos, Desde, Hasta).ToList();

        Assert.Empty(resultado);
    }

    [Fact]
    public void EntreFechas_DesdeMayorQueHasta_LanzaArgumentException()
    {
        List<Pedido> pedidos = new List<Pedido> { CrearPedido(5, Desde) };

        ArgumentException error = Assert.Throws<ArgumentException>(
            () => PedidosPorFecha.EntreFechas(pedidos, Hasta, Desde).ToList());

        Assert.Contains("desde", error.Message);
        Assert.Contains("hasta", error.Message);
    }
}
