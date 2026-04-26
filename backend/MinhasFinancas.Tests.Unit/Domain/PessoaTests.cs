using Xunit;
using FluentAssertions;
using MinhasFinancas.Domain.Entities;

namespace MinhasFinancas.Tests.Unit.Domain;

public class PessoaTests
{
    [Fact(DisplayName = "Pessoa â€“ idade calculada corretamente quando aniversÃ¡rio jÃ¡ passou no ano atual")]
    public void Idade_AniversarioJaPassou_DeveRetornarIdadeCorreta()
    {
        var hoje = DateTime.Today;
        var nascimento = hoje.AddYears(-25).AddDays(-1);
        var pessoa = new Pessoa { Nome = "Alice", DataNascimento = nascimento };

        pessoa.Idade.Should().Be(25);
    }

    [Fact(DisplayName = "Pessoa â€“ idade calculada corretamente quando aniversÃ¡rio ainda nÃ£o chegou no ano atual")]
    public void Idade_AniversarioAindaNaoPassou_DeveRetornarIdadeCorreta()
    {
        var hoje = DateTime.Today;
        var nascimento = hoje.AddYears(-25).AddDays(1);
        var pessoa = new Pessoa { Nome = "Bob", DataNascimento = nascimento };

        pessoa.Idade.Should().Be(24);
    }

    [Fact(DisplayName = "Pessoa â€“ idade Ã© zero quando nasceu hoje")]
    public void Idade_NasceuHoje_DeveRetornarZero()
    {
        var pessoa = new Pessoa { Nome = "RecÃ©m-Nascido", DataNascimento = DateTime.Today };

        pessoa.Idade.Should().Be(0);
    }

    [Fact(DisplayName = "Pessoa â€“ idade Ã© exatamente 18 no dia do aniversÃ¡rio")]
    public void Idade_DiaDoAniversario18Anos_DeveRetornar18()
    {
        var nascimento = DateTime.Today.AddYears(-18);
        var pessoa = new Pessoa { Nome = "Jovem", DataNascimento = nascimento };

        pessoa.Idade.Should().Be(18);
    }

    [Fact(DisplayName = "EhMaiorDeIdade â€“ retorna true para pessoa com exatamente 18 anos")]
    public void EhMaiorDeIdade_Com18Anos_DeveRetornarTrue()
    {
        var nascimento = DateTime.Today.AddYears(-18);
        var pessoa = new Pessoa { Nome = "Adulto", DataNascimento = nascimento };

        pessoa.EhMaiorDeIdade().Should().BeTrue();
    }

    [Fact(DisplayName = "EhMaiorDeIdade â€“ retorna true para pessoa com mais de 18 anos")]
    public void EhMaiorDeIdade_ComMaisDe18Anos_DeveRetornarTrue()
    {
        var pessoa = new Pessoa { Nome = "Adulto", DataNascimento = DateTime.Today.AddYears(-30) };

        pessoa.EhMaiorDeIdade().Should().BeTrue();
    }

    [Fact(DisplayName = "EhMaiorDeIdade â€“ retorna false para pessoa com 17 anos")]
    public void EhMaiorDeIdade_Com17Anos_DeveRetornarFalse()
    {
        var nascimento = DateTime.Today.AddYears(-18).AddDays(1);
        var pessoa = new Pessoa { Nome = "Menor", DataNascimento = nascimento };

        pessoa.EhMaiorDeIdade().Should().BeFalse();
    }

    [Fact(DisplayName = "EhMaiorDeIdade â€“ retorna false para recÃ©m-nascido")]
    public void EhMaiorDeIdade_RecemNascido_DeveRetornarFalse()
    {
        var pessoa = new Pessoa { Nome = "BebÃª", DataNascimento = DateTime.Today };

        pessoa.EhMaiorDeIdade().Should().BeFalse();
    }
}
