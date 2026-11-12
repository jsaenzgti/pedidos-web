using PedidosWeb.Core.Totales;

namespace PedidosWeb.Core.Tests.Totales;

// Casos de borde que el golden master no cubre.
public class CalculoTotalesBordeTests
{
    [Fact]
    public void Calcular_SinLineas_TodoEnCero()
    {
        List<LineaCalculo> lineas = new List<LineaCalculo>();

        ResultadoTotales resultado = CalculoTotales.Calcular(lineas, false, 11, 15000m, 0.19m);

        Assert.Equal(0m, resultado.Subtotal);
        Assert.Equal(0m, resultado.Descuento);
        Assert.Equal(0m, resultado.Flete);
        Assert.Equal(0m, resultado.Iva);
        Assert.Equal(0m, resultado.Total);
    }

    [Fact]
    public void Calcular_CategoriaOfertaEnMinusculaConEspacios_NoEntraEnBaseDescuento()
    {
        List<LineaCalculo> lineas = new List<LineaCalculo>
        {
            new LineaCalculo(1, 600000m, " ofe "),
            new LineaCalculo(1, 400000m, "ABA")
        };

        ResultadoTotales resultado = CalculoTotales.Calcular(lineas, false, 1, 5000m, 0.19m);

        Assert.Equal(400000m, resultado.BaseDescuento);
        Assert.Equal(0m, resultado.Descuento);
    }

    [Fact]
    public void Calcular_CategoriaNula_CuentaEnLaBaseYNoFalla()
    {
        List<LineaCalculo> lineas = new List<LineaCalculo>
        {
            new LineaCalculo(1, 600000m, null!)
        };

        ResultadoTotales resultado = CalculoTotales.Calcular(lineas, false, 1, 5000m, 0.19m);

        Assert.Equal(600000m, resultado.BaseDescuento);
        Assert.Equal(30000m, resultado.Descuento);
    }

    [Fact]
    public void Calcular_TasaIvaCero_IvaCeroYTotalSinIva()
    {
        List<LineaCalculo> lineas = new List<LineaCalculo>
        {
            new LineaCalculo(1, 100000m, "ABA")
        };

        ResultadoTotales resultado = CalculoTotales.Calcular(lineas, false, 1, 5000m, 0m);

        Assert.Equal(0m, resultado.Iva);
        Assert.Equal(105000m, resultado.Total);
    }

    [Fact]
    public void Calcular_BaseUnPesoSobreElUmbral_AplicaCincoPorCiento()
    {
        List<LineaCalculo> lineas = new List<LineaCalculo>
        {
            new LineaCalculo(1, 500001m, "ABA")
        };

        ResultadoTotales resultado = CalculoTotales.Calcular(lineas, false, 1, 5000m, 0.19m);

        Assert.Equal(0.05m, resultado.PorcDescuento);
        Assert.Equal(25000m, resultado.Descuento);
    }
}
