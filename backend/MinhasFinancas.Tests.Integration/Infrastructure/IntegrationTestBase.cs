using Microsoft.Extensions.DependencyInjection;
using Xunit;
using MinhasFinancas.Domain.Entities;
using MinhasFinancas.Infrastructure.Data;

namespace MinhasFinancas.Tests.Integration.Infrastructure;

public abstract class IntegrationTestBase : IClassFixture<TestWebApplicationFactory>, IDisposable
{
    protected readonly HttpClient Client;
    protected readonly IServiceScope Scope;
    protected readonly MinhasFinancasDbContext DbContext;

    protected IntegrationTestBase(TestWebApplicationFactory factory)
    {
        Client = factory.CreateClient();
        Scope = factory.Services.CreateScope();
        DbContext = Scope.ServiceProvider.GetRequiredService<MinhasFinancasDbContext>();
    }

    protected async Task<Pessoa> SeedPessoaAsync(string nome = "Teste", int idadeAnos = 30)
    {
        var nascimento = DateTime.Today.AddYears(-idadeAnos);
        var pessoa = new Pessoa { Nome = nome, DataNascimento = nascimento };
        DbContext.Pessoas.Add(pessoa);
        await DbContext.SaveChangesAsync();
        DbContext.ChangeTracker.Clear();
        return pessoa;
    }

    protected async Task<Categoria> SeedCategoriaAsync(
        string descricao = "Categoria Teste",
        Categoria.EFinalidade finalidade = Categoria.EFinalidade.Despesa)
    {
        var categoria = new Categoria { Descricao = descricao, Finalidade = finalidade };
        DbContext.Categorias.Add(categoria);
        await DbContext.SaveChangesAsync();
        DbContext.ChangeTracker.Clear();
        return categoria;
    }

    protected async Task<Transacao> SeedTransacaoAsync(
        Pessoa pessoa,
        Categoria categoria,
        Transacao.ETipo tipo = Transacao.ETipo.Despesa,
        decimal valor = 100m)
    {
        var transacao = new Transacao
        {
            Descricao = "TransaÃ§Ã£o Seed",
            Valor = valor,
            Tipo = tipo,
            Data = DateTime.Today
        };
        DbContext.Entry(pessoa).State = Microsoft.EntityFrameworkCore.EntityState.Unchanged;
        typeof(MinhasFinancas.Domain.Entities.Transacao).GetProperty("Pessoa")!.SetValue(transacao, pessoa);
        DbContext.Entry(categoria).State = Microsoft.EntityFrameworkCore.EntityState.Unchanged;
        typeof(MinhasFinancas.Domain.Entities.Transacao).GetProperty("Categoria")!.SetValue(transacao, categoria);
        DbContext.Transacoes.Add(transacao);
        await DbContext.SaveChangesAsync();
        DbContext.ChangeTracker.Clear();
        return transacao;
    }

    public void Dispose()
    {
        Scope.Dispose();
        Client.Dispose();
        GC.SuppressFinalize(this);
    }
}



