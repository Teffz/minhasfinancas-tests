using Xunit;
using FluentAssertions;
using MinhasFinancas.Domain.Entities;

namespace MinhasFinancas.Tests.Unit.Domain;

public class TransacaoTests
{
    private static Pessoa CriarPessoaMaiorDeIdade(string nome = "Adulto")
        => new() { Nome = nome, DataNascimento = DateTime.Today.AddYears(-25) };

    private static Pessoa CriarMenorDeIdade(string nome = "Menor")
        => new() { Nome = nome, DataNascimento = DateTime.Today.AddYears(-15) };

    private static Categoria CriarCategoria(Categoria.EFinalidade finalidade, string descricao = "Categoria")
        => new() { Descricao = descricao, Finalidade = finalidade };

    private static void SetPessoa(Transacao transacao, Pessoa pessoa)
    {
        typeof(Transacao).GetProperty("Pessoa")!
            .SetValue(transacao, pessoa);
    }

    private static void SetCategoria(Transacao transacao, Categoria categoria)
    {
        typeof(Transacao).GetProperty("Categoria")!
            .SetValue(transacao, categoria);
    }

    [Fact(DisplayName = "Transacao – menor de idade não pode ter receita")]
    public void SetPessoa_MenorDeIdade_TipoReceita_DeveLancarExcecao()
    {
        var transacao = new Transacao { Descricao = "Mesada", Valor = 100m, Tipo = Transacao.ETipo.Receita };
        var menor = CriarMenorDeIdade();

        var act = () => SetPessoa(transacao, menor);

        act.Should().Throw<Exception>().WithInnerException<InvalidOperationException>()
           .WithMessage("*18*");
    }

    [Fact(DisplayName = "Transacao – menor de idade pode ter despesa")]
    public void SetPessoa_MenorDeIdade_TipoDespesa_NaoDeveLancarExcecao()
    {
        var transacao = new Transacao { Descricao = "Lanche", Valor = 10m, Tipo = Transacao.ETipo.Despesa };
        var menor = CriarMenorDeIdade();

        var act = () => SetPessoa(transacao, menor);

        act.Should().NotThrow();
    }

    [Fact(DisplayName = "Transacao – adulto pode ter receita")]
    public void SetPessoa_MaiorDeIdade_TipoReceita_NaoDeveLancarExcecao()
    {
        var transacao = new Transacao { Descricao = "Salário", Valor = 5000m, Tipo = Transacao.ETipo.Receita };
        var adulto = CriarPessoaMaiorDeIdade();

        var act = () => SetPessoa(transacao, adulto);

        act.Should().NotThrow();
    }

    [Fact(DisplayName = "Transacao – pessoa com exatamente 18 anos pode ter receita")]
    public void SetPessoa_ExatamenteDezOitoAnos_TipoReceita_NaoDeveLancarExcecao()
    {
        var transacao = new Transacao { Descricao = "Primeiro Salário", Valor = 1500m, Tipo = Transacao.ETipo.Receita };
        var pessoa = new Pessoa { Nome = "Dezoito", DataNascimento = DateTime.Today.AddYears(-18) };

        var act = () => SetPessoa(transacao, pessoa);

        act.Should().NotThrow();
    }

    [Fact(DisplayName = "Transacao – não pode registrar receita em categoria de despesa")]
    public void SetCategoria_CategoriaParaDespesa_TipoReceita_DeveLancarExcecao()
    {
        var transacao = new Transacao { Descricao = "Salário", Valor = 3000m, Tipo = Transacao.ETipo.Receita };
        var categoriaDespesa = CriarCategoria(Categoria.EFinalidade.Despesa, "Mercado");

        var act = () => SetCategoria(transacao, categoriaDespesa);

        act.Should().Throw<Exception>().WithInnerException<InvalidOperationException>()
           .WithMessage("*receita*despesa*");
    }

    [Fact(DisplayName = "Transacao – não pode registrar despesa em categoria de receita")]
    public void SetCategoria_CategoriaParaReceita_TipoDespesa_DeveLancarExcecao()
    {
        var transacao = new Transacao { Descricao = "Compras", Valor = 200m, Tipo = Transacao.ETipo.Despesa };
        var categoriaReceita = CriarCategoria(Categoria.EFinalidade.Receita, "Salário");

        var act = () => SetCategoria(transacao, categoriaReceita);

        act.Should().Throw<Exception>().WithInnerException<InvalidOperationException>()
           .WithMessage("*despesa*receita*");
    }

    [Fact(DisplayName = "Transacao – pode registrar despesa em categoria de despesa")]
    public void SetCategoria_CategoriaParaDespesa_TipoDespesa_NaoDeveLancarExcecao()
    {
        var transacao = new Transacao { Descricao = "Aluguel", Valor = 1200m, Tipo = Transacao.ETipo.Despesa };
        var categoriaDespesa = CriarCategoria(Categoria.EFinalidade.Despesa, "Moradia");

        var act = () => SetCategoria(transacao, categoriaDespesa);

        act.Should().NotThrow();
    }

    [Fact(DisplayName = "Transacao – pode registrar receita em categoria de receita")]
    public void SetCategoria_CategoriaParaReceita_TipoReceita_NaoDeveLancarExcecao()
    {
        var transacao = new Transacao { Descricao = "Freela", Valor = 800m, Tipo = Transacao.ETipo.Receita };
        var categoriaReceita = CriarCategoria(Categoria.EFinalidade.Receita, "Renda Extra");

        var act = () => SetCategoria(transacao, categoriaReceita);

        act.Should().NotThrow();
    }

    [Fact(DisplayName = "Transacao – categoria Ambas aceita despesa")]
    public void SetCategoria_CategoriaAmbas_TipoDespesa_NaoDeveLancarExcecao()
    {
        var transacao = new Transacao { Descricao = "Compra", Valor = 50m, Tipo = Transacao.ETipo.Despesa };
        var categoriaAmbas = CriarCategoria(Categoria.EFinalidade.Ambas, "Geral");

        var act = () => SetCategoria(transacao, categoriaAmbas);

        act.Should().NotThrow();
    }

    [Fact(DisplayName = "Transacao – categoria Ambas aceita receita")]
    public void SetCategoria_CategoriaAmbas_TipoReceita_NaoDeveLancarExcecao()
    {
        var transacao = new Transacao { Descricao = "Ganho", Valor = 200m, Tipo = Transacao.ETipo.Receita };
        var categoriaAmbas = CriarCategoria(Categoria.EFinalidade.Ambas, "Geral");

        var act = () => SetCategoria(transacao, categoriaAmbas);

        act.Should().NotThrow();
    }
}