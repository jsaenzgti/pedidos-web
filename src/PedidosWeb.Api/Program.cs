using PedidosWeb.Core.Pedidos;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IPedidoRepositorio, PedidoRepositorioEnMemoria>();
builder.Services.AddSingleton<ListarPedidosServicio>();

var app = builder.Build();

// Para comprobar que la API está arriba.
app.MapGet("/salud", () => Results.Ok(new { estado = "ok" }));

// Detalle de un pedido por id.
app.MapGet("/api/pedidos/{id:int}", (int id, IPedidoRepositorio repositorio) =>
    repositorio.ObtenerPorId(id) is { } pedido
        ? Results.Ok(pedido)
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
        return Results.ValidationProblem(errores);
    }

    return Results.Ok(resultado.Pagina);
});

app.Run();

// Permite crear tests de integración con WebApplicationFactory<Program> más adelante.
public partial class Program { }
