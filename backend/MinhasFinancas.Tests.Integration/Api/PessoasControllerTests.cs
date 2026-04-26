using System.Net;
using Xunit;
using System.Net.Http.Json;
using FluentAssertions;
using MinhasFinancas.Domain.Entities;
using MinhasFinancas.Tests.Integration.Infrastructure;

namespace MinhasFinancas.Tests.Integration.Api;

public class PessoasControllerTests : IntegrationTestBase
{
    public PessoasControllerTests(TestWebApplicationFactory factory) : base(factory) { }

    [Fact(DisplayName = "GET /pessoas â€“ retorna 200 OK com lista paginada")]
    public async Task GetAll_DeveRetornar200ComListaPaginada()
    {
        await SeedPessoaAsync("Ana");
        await SeedPessoaAsync("Bruno");

        var response = await Client.GetAsync("/api/v1.0/pessoas");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact(DisplayName = "GET /pessoas/{id} â€“ retorna 200 com dados da pessoa existente")]
    public async Task GetById_Existe_DeveRetornar200()
    {
        var pessoa = await SeedPessoaAsync("Carlos", 25);

        var response = await Client.GetAsync($"/api/v1.0/pessoas/{pessoa.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("Carlos");
    }

    [Fact(DisplayName = "GET /pessoas/{id} â€“ retorna 404 quando pessoa nÃ£o existe")]
    public async Task GetById_NaoExiste_DeveRetornar404()
    {
        var response = await Client.GetAsync($"/api/v1.0/pessoas/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact(DisplayName = "POST /pessoas â€“ cria pessoa com dados vÃ¡lidos e retorna 201")]
    public async Task Post_DadosValidos_DeveRetornar201()
    {
        var payload = new { Nome = "Daniela", DataNascimento = "1990-05-15" };

        var response = await Client.PostAsJsonAsync("/api/v1.0/pessoas", payload);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("Daniela");
    }

    [Fact(DisplayName = "POST /pessoas â€“ retorna 400 quando nome estÃ¡ vazio")]
    public async Task Post_NomeVazio_DeveRetornar400()
    {
        var payload = new { Nome = "", DataNascimento = "1990-05-15" };

        var response = await Client.PostAsJsonAsync("/api/v1.0/pessoas", payload);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact(DisplayName = "POST /pessoas â€“ retorna 400 quando data de nascimento Ã© futura")]
    public async Task Post_DataNascimentoFutura_DeveRetornar400()
    {
        var payload = new { Nome = "Futurista", DataNascimento = DateTime.Today.AddYears(1).ToString("yyyy-MM-dd") };

        var response = await Client.PostAsJsonAsync("/api/v1.0/pessoas", payload);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact(DisplayName = "PUT /pessoas/{id} â€“ atualiza pessoa existente e retorna 204")]
    public async Task Put_Existe_DeveRetornar204()
    {
        var pessoa = await SeedPessoaAsync("Eduardo", 28);
        var payload = new { Nome = "Eduardo Atualizado", DataNascimento = "1996-03-10" };

        var response = await Client.PutAsJsonAsync($"/api/v1.0/pessoas/{pessoa.Id}", payload);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact(DisplayName = "PUT /pessoas/{id} â€“ retorna 404 quando pessoa nÃ£o existe")]
    public async Task Put_NaoExiste_DeveRetornar404()
    {
        var payload = new { Nome = "NinguÃ©m", DataNascimento = "1990-01-01" };

        var response = await Client.PutAsJsonAsync($"/api/v1.0/pessoas/{Guid.NewGuid()}", payload);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact(DisplayName = "DELETE /pessoas/{id} â€“ remove pessoa existente e retorna 204")]
    public async Task Delete_Existe_DeveRetornar204()
    {
        var pessoa = await SeedPessoaAsync("ExcluÃ­da", 35);

        var response = await Client.DeleteAsync($"/api/v1.0/pessoas/{pessoa.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact(DisplayName = "DELETE /pessoas/{id} â€“ transaÃ§Ãµes da pessoa sÃ£o removidas em cascata")]
    public async Task Delete_ComTransacoes_DeveRemoverTransacoesEmCascata()
    {
        var pessoa = await SeedPessoaAsync("Com Transacoes", 40);
        var categoria = await SeedCategoriaAsync("Despesa Geral", Categoria.EFinalidade.Despesa);
        await SeedTransacaoAsync(pessoa, categoria, Transacao.ETipo.Despesa, 200m);

        var deleteResponse = await Client.DeleteAsync($"/api/v1.0/pessoas/{pessoa.Id}");

        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var transacoesNoDb = DbContext.Transacoes.Where(t => t.PessoaId == pessoa.Id).ToList();
        transacoesNoDb.Should().BeEmpty("transaÃ§Ãµes devem ser removidas em cascata");
    }
}

