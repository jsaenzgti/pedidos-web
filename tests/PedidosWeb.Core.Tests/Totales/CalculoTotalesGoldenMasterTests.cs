using System.Globalization;
using PedidosWeb.Core.Totales;

namespace PedidosWeb.Core.Tests.Totales;

public class CalculoTotalesGoldenMasterTests
{
    private const string RutaCsv = "migracion/golden-master.csv";

    public static IEnumerable<object[]> Casos()
    {
        string rutaArchivo = BuscarGoldenMaster();
        string[] filas = File.ReadAllLines(rutaArchivo);

        // La fila 0 es el encabezado.
        for (int i = 1; i < filas.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(filas[i]))
            {
                continue;
            }

            string[] c = filas[i].Split(',');

            yield return new object[]
            {
                int.Parse(c[0], CultureInfo.InvariantCulture),      // caso
                c[2],                                               // tipo_cliente
                int.Parse(c[3], CultureInfo.InvariantCulture),      // region
                Decimal(c[4]),                                      // flete_base
                Decimal(c[5]),                                      // tasa_iva
                c[6],                                               // lineas
                Decimal(c[10]),                                     // descuento esperado
                Decimal(c[11]),                                     // flete esperado
                Decimal(c[13]),                                     // iva esperado
                Decimal(c[14])                                      // total esperado
            };
        }
    }

    [Theory]
    [MemberData(nameof(Casos))]
    public void Calcular_PedidoDelLegado_CoincideConGoldenMaster(
        int caso,
        string tipoCliente,
        int region,
        decimal fleteBase,
        decimal tasaIva,
        string lineasTexto,
        decimal descuentoEsperado,
        decimal fleteEsperado,
        decimal ivaEsperado,
        decimal totalEsperado)
    {
        List<LineaCalculo> lineas = LeerLineas(lineasTexto);
        bool esVip = tipoCliente == "V";

        ResultadoTotales resultado = CalculoTotales.Calcular(lineas, esVip, region, fleteBase, tasaIva);

        Assert.Equal(descuentoEsperado, resultado.Descuento);
        Assert.Equal(fleteEsperado, resultado.Flete);
        Assert.Equal(ivaEsperado, resultado.Iva);
        Assert.Equal(totalEsperado, resultado.Total);
    }

    private static decimal Decimal(string texto)
    {
        return decimal.Parse(texto, NumberStyles.Number, CultureInfo.InvariantCulture);
    }

    // Formato de cada línea: cantidad x precio x categoría, separadas por "|".
    private static List<LineaCalculo> LeerLineas(string texto)
    {
        List<LineaCalculo> lineas = new List<LineaCalculo>();

        foreach (string item in texto.Split('|'))
        {
            string[] partes = item.Split('x');
            int cantidad = int.Parse(partes[0], CultureInfo.InvariantCulture);
            decimal precio = Decimal(partes[1]);
            lineas.Add(new LineaCalculo(cantidad, precio, partes[2]));
        }

        return lineas;
    }

    // Sube desde la carpeta de salida hasta encontrar migracion/golden-master.csv.
    private static string BuscarGoldenMaster()
    {
        DirectoryInfo? carpeta = new DirectoryInfo(AppContext.BaseDirectory);

        while (carpeta != null)
        {
            string candidato = Path.Combine(carpeta.FullName, RutaCsv);
            if (File.Exists(candidato))
            {
                return candidato;
            }

            carpeta = carpeta.Parent;
        }

        throw new FileNotFoundException("No se encontró " + RutaCsv + " subiendo desde " + AppContext.BaseDirectory);
    }
}
