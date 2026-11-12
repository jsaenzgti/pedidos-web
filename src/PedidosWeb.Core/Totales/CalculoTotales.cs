namespace PedidosWeb.Core.Totales;

// Migración de legado/src/UCalculoTotales.pas. Debe calcular igual que el legado,
// peso a peso. Los montos usan decimal (nunca double ni float).
public static class CalculoTotales
{
    private const decimal UmbralVolumenAlto = 1000000m;
    private const decimal UmbralVolumenMedio = 500000m;
    private const decimal PorcVolumenAlto = 0.08m;
    private const decimal PorcVolumenMedio = 0.05m;
    private const decimal PorcAdicionalVip = 0.03m;
    private const decimal PorcMaximo = 0.10m;
    private const decimal RecargoFleteRegionesExtremas = 1.15m;
    private const string CategoriaOferta = "OFE";

    public static ResultadoTotales Calcular(
        IReadOnlyList<LineaCalculo> lineas,
        bool esVip,
        int region,
        decimal fleteBase,
        decimal tasaIva)
    {
        ResultadoTotales resultado = new ResultadoTotales();

        resultado.Subtotal = CalcularSubtotal(lineas);
        resultado.BaseDescuento = CalcularBaseDescuento(lineas);
        resultado.PorcDescuento = CalcularPorcentajeDescuento(resultado.BaseDescuento, esVip);

        // El descuento se calcula sobre la base, no sobre el subtotal.
        resultado.Descuento = RedondearPesos(resultado.BaseDescuento * resultado.PorcDescuento);
        resultado.Neto = resultado.Subtotal - resultado.Descuento;

        if (lineas.Count == 0)
        {
            resultado.Flete = 0m;
        }
        else
        {
            resultado.Flete = CalcularFlete(fleteBase, region);
        }

        resultado.Iva = RedondearPesos((resultado.Neto + resultado.Flete) * 0.19m);
        resultado.Total = resultado.Neto + resultado.Flete + resultado.Iva;

        return resultado;
    }

    // Redondeo comercial al peso: el .50 exacto sube.
    // Corrige R05 (el legado usaba Trunc(valor + 0.49) y el .50 bajaba).
    private static decimal RedondearPesos(decimal valor)
    {
        return Math.Round(valor, 0, MidpointRounding.AwayFromZero);
    }

    private static decimal CalcularSubtotal(IReadOnlyList<LineaCalculo> lineas)
    {
        decimal subtotal = 0m;

        foreach (LineaCalculo linea in lineas)
        {
            subtotal += linea.Cantidad * linea.PrecioUnitario;
        }

        return subtotal;
    }

    // Las líneas de categoría OFE suman al subtotal pero no a la base del descuento.
    private static decimal CalcularBaseDescuento(IReadOnlyList<LineaCalculo> lineas)
    {
        decimal baseDescuento = 0m;

        foreach (LineaCalculo linea in lineas)
        {
            string categoria = (linea.Categoria ?? string.Empty).Trim().ToUpperInvariant();

            if (categoria != CategoriaOferta)
            {
                baseDescuento += linea.Cantidad * linea.PrecioUnitario;
            }
        }

        return baseDescuento;
    }

    // Umbrales con ">" estricto, como UCalculoTotales.pas (no ">=").
    private static decimal CalcularPorcentajeVolumen(decimal baseDescuento)
    {
        if (baseDescuento > UmbralVolumenAlto)
        {
            return PorcVolumenAlto;
        }

        if (baseDescuento > UmbralVolumenMedio)
        {
            return PorcVolumenMedio;
        }

        return 0m;
    }

    private static decimal CalcularPorcentajeDescuento(decimal baseDescuento, bool esVip)
    {
        decimal porcentaje = CalcularPorcentajeVolumen(baseDescuento);

        if (esVip)
        {
            porcentaje += PorcAdicionalVip;
        }

        if (porcentaje > PorcMaximo)
        {
            porcentaje = PorcMaximo;
        }

        return porcentaje;
    }

    // Las regiones 11 y 12 tienen un recargo de 15 % sobre el flete base.
    private static decimal CalcularFlete(decimal fleteBase, int region)
    {
        if (region == 11 || region == 12)
        {
            return RedondearPesos(fleteBase * RecargoFleteRegionesExtremas);
        }

        return RedondearPesos(fleteBase);
    }
}
