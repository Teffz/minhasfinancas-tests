using System.Net;
using Xunit;
using System.Net.Http.Json;
using FluentAssertions;
using MinhasFinancas.Domain.Entities;
using MinhasFinancas.Tests.Integration.Infrastructure;

namespace MinhasFinancas.Tests.Integration.Api;

public class TransacoesControllerTests : IntegrationTestBase
{
    public TransacoesControllerTests(TestWebApplicationFactory factory) : base(factory) { }

    [Fact(DisplayName = "GET /transacoes â€“ retorna 200 OK")]
    public async Task GetAll_DeveRetornar200()
    {
        var response = await Client.GetAsync("/api/v1.0/transacoes");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact(DisplayName = "POST /transacoes â€“ cria despesa para adulto com categoria de despesa")]
    public async Task Post_DespesaAdultoCategoriaDespesa_DeveRetornar201()
    {
        var pessoa = await SeedPessoaAsync("Adulto VÃ¡lido", 30);
        var categoria = await SeedCategoriaAsync("AlimentaÃ§Ã£o", Categoria.EFinalidade.Despesa);

        var payload = new
        {
            Descricao = "Supermercado",
            Valor = 250.00m,
            Tipo = 0,
            CategoriaId = categoria.Id,
            PessoaId = pessoa.Id,
            Data = DateTime.Today
        };

        var response = await Client.PostAsJsonAsync("/api/v1.0/transacoes", payload);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact(DisplayName = "POST /transacoes â€“ cria receita para adulto com categoria de receita")]
    public async Task Post_ReceitaAdultoCategoriaReceita_DeveRetornar201()
    {
        var pessoa = await SeedPessoaAsync("Trabalhador", 28);
        var categoria = await SeedCategoriaAsync("SalÃ¡rio", Categoria.EFinalidade.Receita);

        var payload = new
        {
            Descricao = "SalÃ¡rio Julho",
            Valor = 4500.00m,
            Tipo = 1,
            CategoriaId = categoria.Id,
            PessoaId = pessoa.Id,
            Data = DateTime.Today
        };

        var response = await Client.PostAsJsonAsync("/api/v1.0/transacoes", payload);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact(DisplayName = "POST /transacoes â€“ menor de idade pode criar despesa")]
    public async Task Post_DespesaMenorIdadeCategoriaDespesa_DeveRetornar201()
    {
        var menor = await SeedPessoaAsync("Menor VÃ¡lido", 15);
        var categoria = await SeedCategoriaAsync("Lazer", Categoria.EFinalidade.Despesa);

        var payload = new
        {
            Descricao = "Cinema",
            Valor = 30.00m,
            Tipo = 0,
            CategoriaId = categoria.Id,
            PessoaId = menor.Id,
            Data = DateTime.Today
        };

        var response = await Client.PostAsJsonAsync("/api/v1.0/transacoes", payload);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact(DisplayName = "POST /transacoes â€“ menor de idade tentando receita deve falhar (BUG-001: retorna 500 em vez de 400)")]
    public async Task Post_ReceitaMenorIdade_DeveRetornarErro()
    {
        var menor = await SeedPessoaAsync("Menor InvÃ¡lido", 16);
        var categoria = await SeedCategoriaAsync("Renda", Categoria.EFinalidade.Receita);

        var payload = new
        {
            Descricao = "Mesada",
            Valor = 100.00m,
            Tipo = 1,
            CategoriaId = categoria.Id,
            PessoaId = menor.Id,
            Data = DateTime.Today
        };

        var response = await Client.PostAsJsonAsync("/api/v1.0/transacoes", payload);

        // BUG-001: deveria ser 400, mas retorna 500
        response.StatusCode.Should().NotBe(HttpStatusCode.Created,
            "menor de idade nÃ£o pode registrar receita");
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError,
            "BUG-001: InvalidOperationException nÃ£o Ã© tratada como 400");
    }

    [Fact(DisplayName = "POST /transacoes â€“ categoria de despesa para receita deve falhar (BUG-001)")]
    public async Task Post_CategoriaIncompativelComTipo_DeveRetornarErro()
    {
        var pessoa = await SeedPessoaAsync("Adulto", 30);
        var categoriaDespesa = await SeedCategoriaAsync("Mercado", Categoria.EFinalidade.Despesa);

        var payload = new
        {
            Descricao = "Receita em categoria errada",
            Valor = 1000.00m,
            Tipo = 1,
            CategoriaId = categoriaDespesa.Id,
            PessoaId = pessoa.Id,
            Data = DateTime.Today
        };

        var response = await Client.PostAsJsonAsync("/api/v1.0/transacoes", payload);

        // BUG-001: deveria ser 400, mas retorna 500
        response.StatusCode.Should().NotBe(HttpStatusCode.Created);
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError,
            "BUG-001: InvalidOperationException nÃ£o Ã© tratada como 400");
    }

    [Fact(DisplayName = "POST /transacoes â€“ retorna 400 quando categoria nÃ£o existe")]
    public async Task Post_CategoriaNaoExiste_DeveRetornar400()
    {
        var pessoa = await SeedPessoaAsync("Pessoa VÃ¡lida", 25);

        var payload = new
        {
            Descricao = "Teste",
            Valor = 100m,
            Tipo = 0,
            CategoriaId = Guid.NewGuid(),
            PessoaId = pessoa.Id,
            Data = DateTime.Today
        };

        var response = await Client.PostAsJsonAsync("/api/v1.0/transacoes", payload);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact(DisplayName = "POST /transacoes â€“ retorna 400 quando pessoa nÃ£o existe")]
    public async Task Post_PessoaNaoExiste_DeveRetornar400()
    {
        var categoria = await SeedCategoriaAsync("Categoria VÃ¡lida", Categoria.EFinalidade.Despesa);

        var payload = new
        {
            Descricao = "Teste",
            Valor = 100m,
            Tipo = 0,
            CategoriaId = categoria.Id,
            PessoaId = Guid.NewGuid(),
            Data = DateTime.Today
        };

        var response = await Client.PostAsJsonAsync("/api/v1.0/transacoes", payload);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact(DisplayName = "POST /transacoes â€“ retorna 400 quando valor Ã© zero")]
    public async Task Post_ValorZero_DeveRetornar400()
    {
        var pessoa = await SeedPessoaAsync("Pessoa", 30);
        var categoria = await SeedCategoriaAsync("Cat", Categoria.EFinalidade.Despesa);

        var payload = new
        {
            Descricao = "Teste",
            Valor = 0m,
            Tipo = 0,
            CategoriaId = categoria.Id,
            PessoaId = pessoa.Id,
            Data = DateTime.Today
        };

        var response = await Client.PostAsJsonAsync("/api/v1.0/transacoes", payload);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact(DisplayName = "GET /transacoes/{id} â€“ retorna 404 quando nÃ£o existe")]
    public async Task GetById_NaoExiste_DeveRetornar404()
    {
        var response = await Client.GetAsync($"/api/v1.0/transacoes/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact(DisplayName = "GET /transacoes/{id} â€“ retorna 200 com dados quando existe")]
    public async Task GetById_Existe_DeveRetornar200ComDados()
    {
        var pessoa = await SeedPessoaAsync("Fulano", 30);
        var categoria = await SeedCategoriaAsync("Despesas", Categoria.EFinalidade.Despesa);
        var transacao = await SeedTransacaoAsync(pessoa, categoria, Transacao.ETipo.Despesa, 300m);

        var response = await Client.GetAsync($"/api/v1.0/transacoes/{transacao.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("300");
    }
}

