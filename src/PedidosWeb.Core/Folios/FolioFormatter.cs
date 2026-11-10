namespace PedidosWeb.Core.Folios;

/// <summary>
/// Genera el folio de un pedido con el mismo formato del sistema legado.
/// Las reglas completas y los ejemplos están en el README, sección «La función del taller».
/// </summary>
public static class FolioFormatter
{
    /// <summary>
    /// Devuelve el folio con el formato PED-{SUCURSAL}-{NUMERO}, por ejemplo "PED-STGO-000123".
    /// </summary>
    /// <param name="numero">Número correlativo del pedido, entre 1 y 999999.</param>
    /// <param name="sucursal">Código de la sucursal: de 2 a 4 letras de la A a la Z.</param>
    public static string FormatearFolio(int numero, string sucursal)
    {
        // R1 (y R5: se valida primero el número).
        if (numero < NumeroMinimo || numero > NumeroMaximo)
        {
            throw new ArgumentOutOfRangeException(
                nameof(numero),
                numero,
                $"El número debe estar entre {NumeroMinimo} y {NumeroMaximo}.");
        }

        // R3: null o vacía.
        if (string.IsNullOrWhiteSpace(sucursal))
        {
            throw sucursal is null
                ? new ArgumentNullException(nameof(sucursal))
                : new ArgumentException("La sucursal no puede estar vacía.", nameof(sucursal));
        }

        // R2: sin espacios en los extremos y en mayúsculas.
        // Se valida solo A-Z/a-z antes de pasar a mayúsculas para que ninguna letra
        // no ASCII (Ñ, tildes, etc.) se cuele por reglas de cultura.
        string codigo = sucursal.Trim();

        if (codigo.Length < LargoMinimoSucursal
            || codigo.Length > LargoMaximoSucursal
            || !codigo.All(EsLetraAscii))
        {
            throw new ArgumentException(
                $"La sucursal debe tener entre {LargoMinimoSucursal} y {LargoMaximoSucursal} letras de la A a la Z.",
                nameof(sucursal));
        }

        // R4: número con ceros a la izquierda hasta 6 dígitos.
        return $"PED-{codigo.ToUpperInvariant()}-{numero:D6}";
    }

    private const int NumeroMinimo = 1;
    private const int NumeroMaximo = 999999;
    private const int LargoMinimoSucursal = 2;
    private const int LargoMaximoSucursal = 4;

    private static bool EsLetraAscii(char c)
    {
        return (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z');
    }
}
