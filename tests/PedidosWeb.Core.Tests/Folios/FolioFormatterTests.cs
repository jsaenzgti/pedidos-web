using PedidosWeb.Core.Folios;

namespace PedidosWeb.Core.Tests.Folios;

public class FolioFormatterTests
{
    // Casos base del README (tres primeras filas de la tabla de ejemplos).
    [Theory]
    [InlineData(123, "STGO", "PED-STGO-000123")]
    [InlineData(1, "VAL", "PED-VAL-000001")]
    [InlineData(999999, "CO", "PED-CO-999999")]
    public void FormatearFolio_CasosBase_DevuelveElFolioEsperado(int numero, string sucursal, string esperado)
    {
        var folio = FolioFormatter.FormatearFolio(numero, sucursal);

        Assert.Equal(esperado, folio);
    }

    // R2: trim y mayúsculas.
    [Theory]
    [InlineData(45, " stgo ", "PED-STGO-000045")]
    [InlineData(45, "stgo", "PED-STGO-000045")]
    [InlineData(45, "StGo", "PED-STGO-000045")]
    [InlineData(45, "\tval\n", "PED-VAL-000045")]
    public void FormatearFolio_SucursalConEspaciosOMinusculas_LaNormaliza(int numero, string sucursal, string esperado)
    {
        Assert.Equal(esperado, FolioFormatter.FormatearFolio(numero, sucursal));
    }

    // R1: límites del número.
    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    [InlineData(1000000)]
    [InlineData(int.MinValue)]
    [InlineData(int.MaxValue)]
    public void FormatearFolio_NumeroFueraDeRango_LanzaArgumentOutOfRange(int numero)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => FolioFormatter.FormatearFolio(numero, "STGO"));
    }

    // R3: sucursales inválidas.
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("S")]
    [InlineData("SANTI")]
    [InlineData("ÑUÑO")]
    [InlineData("ST1")]
    [InlineData("ST GO")]
    [InlineData("ÁB")]
    [InlineData("S-T")]
    [InlineData(" S ")]
    public void FormatearFolio_SucursalInvalida_LanzaArgumentException(string sucursal)
    {
        Assert.Throws<ArgumentException>(() => FolioFormatter.FormatearFolio(7, sucursal));
    }

    [Fact]
    public void FormatearFolio_SucursalNull_LanzaArgumentException()
    {
        // ArgumentNullException deriva de ArgumentException.
        Assert.ThrowsAny<ArgumentException>(() => FolioFormatter.FormatearFolio(7, null!));
    }

    // R5: si ambos son inválidos, manda el número.
    [Theory]
    [InlineData(0, "S")]
    [InlineData(0, "")]
    [InlineData(1000000, "ÑUÑO")]
    public void FormatearFolio_NumeroYSucursalInvalidos_ValidaPrimeroElNumero(int numero, string sucursal)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => FolioFormatter.FormatearFolio(numero, sucursal));
    }

    // R4: relleno de ceros hasta 6 dígitos.
    [Theory]
    [InlineData(1, "PED-AB-000001")]
    [InlineData(99999, "PED-AB-099999")]
    [InlineData(100000, "PED-AB-100000")]
    public void FormatearFolio_Numero_SeCompletaConCerosAIzquierda(int numero, string esperado)
    {
        Assert.Equal(esperado, FolioFormatter.FormatearFolio(numero, "AB"));
    }
}
