using PedidosWeb.Core.Lineas;
using PedidosWeb.Core.Pedidos;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IPedidoRepositorio, PedidoRepositorioEnMemoria>();
builder.Services.AddSingleton<ListarPedidosServicio>();
builder.Services.AddSingleton<ILineaPedidoRepositorio, LineaPedidoRepositorioEnMemoria>();
builder.Services.AddSingleton<DetallePedidoServicio>();

var app = builder.Build();

// Para comprobar que la API está arriba.
app.MapGet("/salud", () => Results.Ok(new { estado = "ok" }));

// Detalle de un pedido por id.
app.MapGet("/api/pedidos/{id:int}", (int id, IPedidoRepositorio repositorio) =>
    repositorio.ObtenerPorId(id) is { } pedido
        ? Results.Ok(pedido)
        : Results.NotFound());

// HU-02 · Detalle de un pedido con su cliente, sus líneas y su total (spec: specs/HU-02-detalle-pedido/spec.md).
// La lógica vive en Core; el endpoint solo traduce: 200 con el detalle, o 404 si el pedido no existe.
app.MapGet("/api/pedidos/{id:int}/detalle", (int id, DetallePedidoServicio servicio) =>
    servicio.ObtenerDetalle(id) is { } detalle
        ? Results.Ok(detalle)
        : Results.NotFound());

// HU-01 · Listar pedidos de un cliente con filtros (spec: specs/HU-01-listar-pedidos/spec.md).
// Los parámetros llegan como texto; la validación y la consulta viven en Core.
// El endpoint solo traduce el resultado a HTTP: 200 con la página, o 400 con ProblemDetails.
app.MapGet("/api/pedidos", (
    string? clienteId,
    string? estado,
    string? desde,
    string? hasta,
    string? pagina,
    string? tamanoPagina,
    ListarPedidosServicio servicio) =>
{
    ResultadoListado resultado = servicio.Listar(clienteId, estado, desde, hasta, pagina, tamanoPagina);

    if (!resultado.EsValido)
    {
        var errores = resultado.Errores.ToDictionary(error => error.Key, error => error.Value);
        return Results.ValidationProblem(errores, title: "Uno o más errores de validación.");
    }

    return Results.Ok(resultado.Pagina);
});

app.Run();

// Permite crear tests de integración con WebApplicationFactory<Program> (ver tests/PedidosWeb.Api.Tests).
public partial class Program { }
