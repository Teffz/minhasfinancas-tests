using Xunit;
using FluentAssertions;
using MinhasFinancas.Application.DTOs;
using System.ComponentModel.DataAnnotations;

namespace MinhasFinancas.Tests.Unit.Application;

public class PessoaValidationTests
{
    [Fact(DisplayName = "ValidarDataNascimento â€“ aceita data no passado")]
    public void ValidarDataNascimento_DataNoPassado_DeveRetornarSucesso()
    {
        var dataNascimento = DateTime.Today.AddYears(-20);
        var context = new ValidationContext(new object());

        var result = PessoaValidation.ValidarDataNascimento(dataNascimento, context);

        result.Should().Be(ValidationResult.Success);
    }

    [Fact(DisplayName = "ValidarDataNascimento â€“ aceita hoje como data de nascimento")]
    public void ValidarDataNascimento_Hoje_DeveRetornarSucesso()
    {
        var context = new ValidationContext(new object());

        var result = PessoaValidation.ValidarDataNascimento(DateTime.Today, context);

        result.Should().Be(ValidationResult.Success);
    }

    [Fact(DisplayName = "ValidarDataNascimento â€“ rejeita data futura")]
    public void ValidarDataNascimento_DataFutura_DeveRetornarErro()
    {
        var dataFutura = DateTime.Today.AddDays(1);
        var context = new ValidationContext(new object());

        var result = PessoaValidation.ValidarDataNascimento(dataFutura, context);

        result.Should().NotBe(ValidationResult.Success);
        result!.ErrorMessage.Should().NotBeNullOrEmpty();
    }

    [Fact(DisplayName = "ValidarDataNascimento â€“ rejeita data 1 ano no futuro")]
    public void ValidarDataNascimento_DataUmAnoFuturo_DeveRetornarErro()
    {
        var dataFutura = DateTime.Today.AddYears(1);
        var context = new ValidationContext(new object());

        var result = PessoaValidation.ValidarDataNascimento(dataFutura, context);

        result.Should().NotBe(ValidationResult.Success);
    }
}
