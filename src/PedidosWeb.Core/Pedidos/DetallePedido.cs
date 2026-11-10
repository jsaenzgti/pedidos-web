namespace PedidosWeb.Core.Pedidos;

/// <summary>El detalle de un pedido: sus datos, el cliente, las líneas y el total (HU-02).</summary>
/// <param name="Id">Número correlativo del pedido.</param>
/// <param name="Folio">Folio del pedido, por ejemplo "PED-STGO-000008".</param>
/// <param name="Fecha">Fecha en que se emitió el pedido.</param>
/// <param name="Estado">Estado del pedido como texto.</param>
/// <param name="Sucursal">Código de la sucursal que emitió el pedido.</param>
/// <param name="ClienteId">Código del cliente, tomado del pedido.</param>
/// <param name="ClienteNombre">Nombre del cliente, tomado del pedido.</param>
/// <param name="Lineas">Líneas del pedido, ordenadas por número.</param>
/// <param name="Total">Suma de los subtotales de las líneas, en CLP, sin IVA.</param>
public sealed record DetallePedido(
    int Id,
    string Folio,
    DateOnly Fecha,
    string Estado,
    string Sucursal,
    string ClienteId,
    string ClienteNombre,
    IReadOnlyList<LineaDetalle> Lineas,
    decimal Total);
