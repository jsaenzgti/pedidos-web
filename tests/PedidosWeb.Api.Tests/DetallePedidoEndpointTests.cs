using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace PedidosWeb.Api.Tests;

/// <summary>
/// HU-02 a nivel HTTP (specs/HU-02-detalle-pedido/spec.md). La lógica se prueba en
/// PedidosWeb.Core.Tests; aquí solo la ruta, el registro en DI y la forma del JSON.
/// </summary>
public class DetallePedidoEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _cliente;

    public DetallePedidoEndpointTests(WebApplicationFactory<Program> fabrica)
    {
        _cliente = fabrica.CreateClient();
    }

    [Fact]
    public async Task Get_DetalleDelPedido8_Devuelve200ConJsonCamelCase()
    {
        // HTTP de CA-01 y CA-02
        var respuesta = await _cliente.GetAsync("/api/pedidos/8/detalle");

        Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);

        var json = await respuesta.Content.ReadFromJsonAsync<JsonElement>();

        // GetProperty distingue mayúsculas: si el JSON no sale en camelCase, el test falla.
        Assert.Equal(8, json.GetProperty("id").GetInt32());
        Assert.Equal("PED-STGO-000008", json.GetProperty("folio").GetString());
        Assert.Equal("2026-10-20", json.GetProperty("fecha").GetString());
        Assert.Equal("Entregado", json.GetProperty("estado").GetString());
        Assert.Equal("STGO", json.GetProperty("sucursal").GetString());
        Assert.Equal("C-001", json.GetProperty("clienteId").GetString());
        Assert.Equal("Ferretería El Clavo", json.GetProperty("clienteNombre").GetString());
        Assert.Equal(120100m, json.GetProperty("total").GetDecimal());

        var lineas = json.GetProperty("lineas");
        Assert.Equal(3, lineas.GetArrayLength());
        Assert.Equal(1, lineas[0].GetProperty("numero").GetInt32());
        Assert.Equal("Harina para pan 25 kg", lineas[0].GetProperty("producto").GetString());
        Assert.Equal(3, lineas[0].GetProperty("cantidad").GetInt32());
        Assert.Equal(21500m, lineas[0].GetProperty("precioUnitario").GetDecimal());
        Assert.Equal(64500m, lineas[0].GetProperty("subtotal").GetDecimal());
    }

    [Fact]
    public async Task Get_DetalleDePedidoInexistente_Devuelve404()
    {
        // CA-03
        var respuesta = await _cliente.GetAsync("/api/pedidos/999/detalle");

        Assert.Equal(HttpStatusCode.NotFound, respuesta.StatusCode);
    }

    [Fact]
    public async Task Get_PedidoPorId_NoCambiaConLaRutaNueva()
    {
        // Restricción de la spec: GET /api/pedidos/{id} no cambia (sin lineas, con el Total guardado).
        var respuesta = await _cliente.GetAsync("/api/pedidos/8");

        Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);

        var json = await respuesta.Content.ReadFromJsonAsync<JsonElement>();

        Assert.Equal(8, json.GetProperty("id").GetInt32());
        Assert.Equal(120100m, json.GetProperty("total").GetDecimal());
        Assert.False(json.TryGetProperty("lineas", out _));
    }
}
