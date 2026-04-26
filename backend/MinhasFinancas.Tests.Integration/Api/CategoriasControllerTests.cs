using Xunit;
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using MinhasFinancas.Domain.Entities;
using MinhasFinancas.Tests.Integration.Infrastructure;

namespace MinhasFinancas.Tests.Integration.Api;

public class CategoriasControllerTests : IntegrationTestBase
{
    public CategoriasControllerTests(TestWebApplicationFactory factory) : base(factory) { }

    [Fact(DisplayName = "GET /categorias â€“ retorna 200 OK")]
    public async Task GetAll_DeveRetornar200()
    {
        var response = await Client.GetAsync("/api/v1.0/categorias");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact(DisplayName = "POST /categorias â€“ cria categoria Despesa e retorna 201")]
    public async Task Post_CategoriaDespesa_DeveRetornar201()
    {
        var payload = new { Descricao = "AlimentaÃ§Ã£o", Finalidade = 0 };

        var response = await Client.PostAsJsonAsync("/api/v1.0/categorias", payload);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact(DisplayName = "POST /categorias â€“ cria categoria Receita e retorna 201")]
    public async Task Post_CategoriaReceita_DeveRetornar201()
    {
        var payload = new { Descricao = "SalÃ¡rio", Finalidade = 1 };

        var response = await Client.PostAsJsonAsync("/api/v1.0/categorias", payload);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact(DisplayName = "POST /categorias â€“ cria categoria Ambas e retorna 201")]
    public async Task Post_CategoriaAmbas_DeveRetornar201()
    {
        var payload = new { Descricao = "Geral", Finalidade = 2 };

        var response = await Client.PostAsJsonAsync("/api/v1.0/categorias", payload);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact(DisplayName = "POST /categorias â€“ retorna 400 quando descriÃ§Ã£o estÃ¡ vazia")]
    public async Task Post_DescricaoVazia_DeveRetornar400()
    {
        var payload = new { Descricao = "", Finalidade = 0 };

        var response = await Client.PostAsJsonAsync("/api/v1.0/categorias", payload);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact(DisplayName = "GET /categorias/{id} â€“ retorna 200 com dados quando existe")]
    public async Task GetById_Existe_DeveRetornar200()
    {
        var categoria = await SeedCategoriaAsync("Transporte", Categoria.EFinalidade.Despesa);

        var response = await Client.GetAsync($"/api/v1.0/categorias/{categoria.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("Transporte");
    }

    [Fact(DisplayName = "GET /categorias/{id} â€“ retorna 404 quando nÃ£o existe")]
    public async Task GetById_NaoExiste_DeveRetornar404()
    {
        var response = await Client.GetAsync($"/api/v1.0/categorias/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
