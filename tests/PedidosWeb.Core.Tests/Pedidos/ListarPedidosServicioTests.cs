using PedidosWeb.Core.Pedidos;

namespace PedidosWeb.Core.Tests.Pedidos;

/// <summary>Tests de HU-01 (specs/HU-01-listar-pedidos/spec.md): un test por criterio de aceptación (CA-01 a CA-19).</summary>
public class ListarPedidosServicioTests
{
    private readonly ListarPedidosServicio _servicio = new(new PedidoRepositorioEnMemoria());

    // Atajo: lista con solo clienteId y el resto de parámetros opcionales.
    private ResultadoListado Listar(
        string? clienteId,
        string? estado = null,
        string? desde = null,
        string? hasta = null,
        string? pagina = null,
        string? tamanoPagina = null)
    {
        return _servicio.Listar(clienteId, estado, desde, hasta, pagina, tamanoPagina);
    }

    private static int[] Ids(ResultadoListado resultado)
    {
        Assert.True(resultado.EsValido, "Se esperaba un listado válido.");
        return resultado.Pagina!.Items.Select(p => p.Id).ToArray();
    }

    [Fact]
    public void Listar_ClienteC001_DevuelveCincoPedidosOrdenadosPorFechaEId()
    {
        // CA-01
        var resultado = Listar("C-001");

        Assert.Equal(new[] { 40, 32, 24, 16, 8 }, Ids(resultado));
        Assert.Equal(5, resultado.Pagina!.Total);
    }

    [Fact]
    public void Listar_ClienteC002_DevuelveSeisPedidosOrdenados()
    {
        // CA-02
        var resultado = Listar("C-002");

        Assert.Equal(new[] { 13, 5, 45, 37, 29, 21 }, Ids(resultado));
        Assert.Equal(6, resultado.Pagina!.Total);
    }

    [Fact]
    public void Listar_ClienteYEstado_DevuelveSoloEseEstado()
    {
        // CA-03
        var resultado = Listar("C-001", estado: "Entregado");

        Assert.Equal(new[] { 40, 24, 8 }, Ids(resultado));
    }

    [Fact]
    public void Listar_EstadoEnMinuscula_NoDistingueMayusculas()
    {
        // CA-04
        var resultado = Listar("C-001", estado: "entregado");

        Assert.Equal(new[] { 40, 24, 8 }, Ids(resultado));
    }

    [Fact]
    public void Listar_RangoDeFechas_IncluyeAmbosExtremos()
    {
        // CA-05
        var resultado = Listar("C-001", desde: "2026-09-14", hasta: "2026-09-26");

        Assert.Equal(new[] { 32, 24 }, Ids(resultado));
    }

    [Fact]
    public void Listar_SoloDesde_FiltraDesdeEsaFecha()
    {
        // CA-06
        var resultado = Listar("C-001", desde: "2026-10-08");

        Assert.Equal(new[] { 16, 8 }, Ids(resultado));
    }

    [Fact]
    public void Listar_PedidoAnulado_AparecePorDefectoYFiltrando()
    {
        // CA-07
        var filtrado = Listar("C-002", estado: "Anulado");
        var sinFiltroDeEstado = Listar("C-002");

        Assert.Equal(new[] { 29 }, Ids(filtrado));
        Assert.Contains(29, Ids(sinFiltroDeEstado));
    }

    [Fact]
    public void Listar_ClienteInexistente_DevuelveListaVacia()
    {
        // CA-08
        var resultado = Listar("C-999");

        Assert.Empty(Ids(resultado));
        Assert.Equal(0, resultado.Pagina!.Total);
    }

    [Fact]
    public void Listar_PaginaDos_DevuelveSegundaPagina()
    {
        // CA-09
        var resultado = Listar("C-001", pagina: "2", tamanoPagina: "2");

        Assert.Equal(new[] { 24, 16 }, Ids(resultado));
        Assert.Equal(5, resultado.Pagina!.Total);
    }

    [Fact]
    public void Listar_SinPaginacion_UsaPagina1Y20PorPagina()
    {
        // CA-10
        var resultado = Listar("C-001");

        Assert.True(resultado.EsValido);
        Assert.Equal(1, resultado.Pagina!.Pagina);
        Assert.Equal(20, resultado.Pagina.TamanoPagina);
    }

    [Fact]
    public void Listar_SinClienteId_DevuelveErrorEnClienteId()
    {
        // CA-11
        var resultado = Listar(clienteId: null);

        Assert.False(resultado.EsValido);
        Assert.True(resultado.Errores.ContainsKey("clienteId"));
    }

    [Fact]
    public void Listar_EstadoInvalido_DevuelveErrorEnEstado()
    {
        // CA-12
        var resultado = Listar("C-001", estado: "Cancelado");

        Assert.False(resultado.EsValido);
        Assert.True(resultado.Errores.ContainsKey("estado"));
    }

    [Fact]
    public void Listar_FechasInvertidas_DevuelveErrorEnDesde()
    {
        // CA-13
        var resultado = Listar("C-001", desde: "2026-10-31", hasta: "2026-09-01");

        Assert.False(resultado.EsValido);
        Assert.True(resultado.Errores.ContainsKey("desde"));
    }

    [Fact]
    public void Listar_FormatoDeFechaIncorrecto_DevuelveErrorEnDesde()
    {
        // CA-14
        var resultado = Listar("C-001", desde: "31-10-2026");

        Assert.False(resultado.EsValido);
        Assert.True(resultado.Errores.ContainsKey("desde"));
    }

    [Fact]
    public void Listar_ClienteEnMinuscula_NoDistingueMayusculas()
    {
        // CA-15
        var resultado = Listar("c-001");

        Assert.Equal(new[] { 40, 32, 24, 16, 8 }, Ids(resultado));
    }

    [Fact]
    public void Listar_PaginaMayorQueLaUltima_DevuelveItemsVacio()
    {
        // CA-16
        var resultado = Listar("C-001", pagina: "9");

        Assert.Empty(Ids(resultado));
        Assert.Equal(5, resultado.Pagina!.Total);
    }

    [Fact]
    public void Listar_VariosCamposInvalidos_DevuelveTodosLosErrores()
    {
        // CA-17
        var resultado = Listar(clienteId: null, estado: "Cancelado", desde: "hoy");

        Assert.False(resultado.EsValido);
        Assert.True(resultado.Errores.ContainsKey("clienteId"));
        Assert.True(resultado.Errores.ContainsKey("estado"));
        Assert.True(resultado.Errores.ContainsKey("desde"));
    }

    [Theory]
    [InlineData("1", "0", "tamanoPagina")]
    [InlineData("1", "51", "tamanoPagina")]
    [InlineData("0", "20", "pagina")]
    public void Listar_PaginacionFueraDeRango_DevuelveErrorEnElCampo(string pagina, string tamanoPagina, string campo)
    {
        // CA-18
        var resultado = Listar("C-001", pagina: pagina, tamanoPagina: tamanoPagina);

        Assert.False(resultado.EsValido);
        Assert.True(resultado.Errores.ContainsKey(campo));
    }

    [Theory]
    [InlineData("abc", "20", "pagina")]
    [InlineData("1", "abc", "tamanoPagina")]
    public void Listar_PaginacionNoNumerica_DevuelveErrorEnElCampo(string pagina, string tamanoPagina, string campo)
    {
        // CA-19
        var resultado = Listar("C-001", pagina: pagina, tamanoPagina: tamanoPagina);

        Assert.False(resultado.EsValido);
        Assert.True(resultado.Errores.ContainsKey(campo));
    }
}
