using Xunit;
using FluentAssertions;
using MinhasFinancas.Domain.Entities;

namespace MinhasFinancas.Tests.Unit.Domain;

public class CategoriaTests
{
    [Fact(DisplayName = "PermiteTipo â€“ categoria Despesa permite transaÃ§Ã£o do tipo Despesa")]
    public void PermiteTipo_CategoriaDespesa_TransacaoDespesa_DeveRetornarTrue()
    {
        var categoria = new Categoria { Descricao = "AlimentaÃ§Ã£o", Finalidade = Categoria.EFinalidade.Despesa };

        categoria.PermiteTipo(Transacao.ETipo.Despesa).Should().BeTrue();
    }

    [Fact(DisplayName = "PermiteTipo â€“ categoria Despesa nÃ£o permite transaÃ§Ã£o do tipo Receita")]
    public void PermiteTipo_CategoriaDespesa_TransacaoReceita_DeveRetornarFalse()
    {
        var categoria = new Categoria { Descricao = "AlimentaÃ§Ã£o", Finalidade = Categoria.EFinalidade.Despesa };

        categoria.PermiteTipo(Transacao.ETipo.Receita).Should().BeFalse();
    }

    [Fact(DisplayName = "PermiteTipo â€“ categoria Receita permite transaÃ§Ã£o do tipo Receita")]
    public void PermiteTipo_CategoriaReceita_TransacaoReceita_DeveRetornarTrue()
    {
        var categoria = new Categoria { Descricao = "SalÃ¡rio", Finalidade = Categoria.EFinalidade.Receita };

        categoria.PermiteTipo(Transacao.ETipo.Receita).Should().BeTrue();
    }

    [Fact(DisplayName = "PermiteTipo â€“ categoria Receita nÃ£o permite transaÃ§Ã£o do tipo Despesa")]
    public void PermiteTipo_CategoriaReceita_TransacaoDespesa_DeveRetornarFalse()
    {
        var categoria = new Categoria { Descricao = "SalÃ¡rio", Finalidade = Categoria.EFinalidade.Receita };

        categoria.PermiteTipo(Transacao.ETipo.Despesa).Should().BeFalse();
    }

    [Fact(DisplayName = "PermiteTipo â€“ categoria Ambas permite transaÃ§Ã£o do tipo Despesa")]
    public void PermiteTipo_CategoriaAmbas_TransacaoDespesa_DeveRetornarTrue()
    {
        var categoria = new Categoria { Descricao = "Geral", Finalidade = Categoria.EFinalidade.Ambas };

        categoria.PermiteTipo(Transacao.ETipo.Despesa).Should().BeTrue();
    }

    [Fact(DisplayName = "PermiteTipo â€“ categoria Ambas permite transaÃ§Ã£o do tipo Receita")]
    public void PermiteTipo_CategoriaAmbas_TransacaoReceita_DeveRetornarTrue()
    {
        var categoria = new Categoria { Descricao = "Geral", Finalidade = Categoria.EFinalidade.Ambas };

        categoria.PermiteTipo(Transacao.ETipo.Receita).Should().BeTrue();
    }
}
