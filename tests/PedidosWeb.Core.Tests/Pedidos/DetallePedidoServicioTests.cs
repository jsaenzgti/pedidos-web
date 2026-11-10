using PedidosWeb.Core.Lineas;
using PedidosWeb.Core.Pedidos;

namespace PedidosWeb.Core.Tests.Pedidos;

/// <summary>
/// Tests de HU-02 (specs/HU-02-detalle-pedido/spec.md).
/// Incremento 1: cálculo del total. Incremento 2: armado del detalle.
/// </summary>
public class DetallePedidoServicioTests
{
    private readonly LineaPedidoRepositorioEnMemoria _lineas = new();
    private readonly PedidoRepositorioEnMemoria _pedidos = new();

    // Repositorio de líneas falso: devuelve exactamente las líneas que se le pasan.
    private sealed class LineasFalsas : ILineaPedidoRepositorio
    {
        private readonly IReadOnlyList<LineaPedido> _lineas;

        public LineasFalsas(params LineaPedido[] lineas)
        {
            _lineas = lineas;
        }

        public IReadOnlyList<LineaPedido> ObtenerPorPedido(int pedidoId)
        {
            return _lineas;
        }
    }

    private DetallePedidoServicio CrearServicio()
    {
        return new DetallePedidoServicio(_pedidos, _lineas);
    }

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

    [Fact]
    public void ObtenerDetalle_Pedido8_TraeElContenidoCompleto()
    {
        // CA-02
        var detalle = CrearServicio().ObtenerDetalle(8);

        Assert.NotNull(detalle);
        Assert.Equal(8, detalle.Id);
        Assert.Equal("PED-STGO-000008", detalle.Folio);
        Assert.Equal(new DateOnly(2026, 10, 20), detalle.Fecha);
        Assert.Equal("Entregado", detalle.Estado);
        Assert.Equal("STGO", detalle.Sucursal);
        Assert.Equal("C-001", detalle.ClienteId);
        Assert.Equal("Ferretería El Clavo", detalle.ClienteNombre);

        Assert.Equal(new[] { 1, 2, 3 }, detalle.Lineas.Select(l => l.Numero));
        Assert.Equal(new[] { 64500m, 35400m, 20200m }, detalle.Lineas.Select(l => l.Subtotal));
        Assert.Equal(3, detalle.Lineas[0].Cantidad);
        Assert.Equal(21500m, detalle.Lineas[0].PrecioUnitario);
        Assert.Equal("Harina para pan 25 kg", detalle.Lineas[0].Producto);
    }

    [Fact]
    public void ObtenerDetalle_Pedido8_ElTotalEsLaSumaDeLosSubtotales()
    {
        // CA-01
        var detalle = CrearServicio().ObtenerDetalle(8);

        Assert.NotNull(detalle);
        Assert.Equal(120100m, detalle.Total);
        Assert.Equal(detalle.Total, detalle.Lineas.Sum(l => l.Subtotal));
    }

    [Fact]
    public void ObtenerDetalle_LineasDesordenadas_LasDevuelvePorNumero()
    {
        // CA-02: la spec pide las líneas ordenadas por número, venga como venga del repositorio.
        var lineasDesordenadas = new LineasFalsas(
            new LineaPedido(8, 3, "C", 1, 300m),
            new LineaPedido(8, 1, "A", 1, 100m),
            new LineaPedido(8, 2, "B", 1, 200m));
        var servicio = new DetallePedidoServicio(_pedidos, lineasDesordenadas);

        var detalle = servicio.ObtenerDetalle(8);

        Assert.NotNull(detalle);
        Assert.Equal(new[] { 1, 2, 3 }, detalle.Lineas.Select(l => l.Numero));
    }

    [Fact]
    public void ObtenerDetalle_PedidoInexistente_DevuelveNull()
    {
        // CA-03 (la parte de Core; el 404 se prueba en el incremento 3).
        var detalle = CrearServicio().ObtenerDetalle(999);

        Assert.Null(detalle);
    }

    [Fact]
    public void ObtenerDetalle_PedidoSinLineas_DevuelveLineasVaciasYTotalCero()
    {
        // CA-04
        var servicio = new DetallePedidoServicio(_pedidos, new LineasFalsas());

        var detalle = servicio.ObtenerDetalle(8);

        Assert.NotNull(detalle);
        Assert.Empty(detalle.Lineas);
        Assert.Equal(0m, detalle.Total);
    }
}
