using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace PedidosWeb.Api.Tests;

/// <summary>
/// HU-01 a nivel HTTP. La lógica y cada criterio se prueban en PedidosWeb.Core.Tests;
/// aquí solo se prueba lo que Core no puede: ruta, registro en DI, enlace de la query,
/// JSON en camelCase y la traducción de errores a ProblemDetails.
/// </summary>
public class ListarPedidosEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _cliente;

    public ListarPedidosEndpointTests(WebApplicationFactory<Program> fabrica)
    {
        _cliente = fabrica.CreateClient();
    }

    [Fact]
    public async Task Get_ListadoValido_Devuelve200ConJsonCamelCase()
    {
        // HTTP de CA-01
        var respuesta = await _cliente.GetAsync("/api/pedidos?clienteId=C-001");

        Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);

        var json = await respuesta.Content.ReadFromJsonAsync<JsonElement>();

        // GetProperty distingue mayúsculas: si el JSON no sale en camelCase, el test falla.
        Assert.Equal(5, json.GetProperty("total").GetInt32());
        Assert.Equal(1, json.GetProperty("pagina").GetInt32());
        Assert.Equal(20, json.GetProperty("tamanoPagina").GetInt32());

        var items = json.GetProperty("items");
        Assert.Equal(5, items.GetArrayLength());
        Assert.Equal(40, items[0].GetProperty("id").GetInt32());
        Assert.Equal("Ferretería El Clavo", items[0].GetProperty("clienteNombre").GetString());
    }

    [Fact]
    public async Task Get_ConParametrosDePaginacion_LosEnlazaAlServicio()
    {
        // HTTP de CA-09
        var respuesta = await _cliente.GetAsync("/api/pedidos?clienteId=C-001&tamanoPagina=2&pagina=2");

        Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);

        var json = await respuesta.Content.ReadFromJsonAsync<JsonElement>();

        Assert.Equal(2, json.GetProperty("pagina").GetInt32());
        Assert.Equal(2, json.GetProperty("tamanoPagina").GetInt32());

        var ids = json.GetProperty("items").EnumerateArray().Select(i => i.GetProperty("id").GetInt32()).ToArray();
        Assert.Equal(new[] { 24, 16 }, ids);
    }

    [Fact]
    public async Task Get_SinClienteId_Devuelve400ProblemDetails()
    {
        // HTTP de CA-11
        var respuesta = await _cliente.GetAsync("/api/pedidos");

        Assert.Equal(HttpStatusCode.BadRequest, respuesta.StatusCode);
        Assert.Equal("application/problem+json", respuesta.Content.Headers.ContentType?.MediaType);

        var json = await respuesta.Content.ReadFromJsonAsync<JsonElement>();

        Assert.Equal(400, json.GetProperty("status").GetInt32());
        Assert.True(json.GetProperty("errors").TryGetProperty("clienteId", out _));
    }

    [Fact]
    public async Task Get_VariosErrores_Devuelve400ConUnErrorPorCampo()
    {
        // HTTP de CA-17
        var respuesta = await _cliente.GetAsync("/api/pedidos?estado=Cancelado&desde=hoy");

        Assert.Equal(HttpStatusCode.BadRequest, respuesta.StatusCode);

        var json = await respuesta.Content.ReadFromJsonAsync<JsonElement>();
        var errores = json.GetProperty("errors");

        Assert.True(errores.TryGetProperty("clienteId", out _));
        Assert.True(errores.TryGetProperty("estado", out _));
        Assert.True(errores.TryGetProperty("desde", out _));
    }
}
