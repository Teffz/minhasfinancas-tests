using Xunit;
using FluentAssertions;
using MinhasFinancas.Application.DTOs;
using MinhasFinancas.Application.Services;
using MinhasFinancas.Domain.Entities;
using MinhasFinancas.Domain.Interfaces;
using Moq;

namespace MinhasFinancas.Tests.Unit.Application;

public class PessoaServiceTests
{
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly Mock<IPessoaRepository> _pessoaRepoMock;
    private readonly PessoaService _sut;

    public PessoaServiceTests()
    {
        _uowMock = new Mock<IUnitOfWork>();
        _pessoaRepoMock = new Mock<IPessoaRepository>();
        _uowMock.Setup(u => u.Pessoas).Returns(_pessoaRepoMock.Object);
        _uowMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
        _sut = new PessoaService(_uowMock.Object);
    }

    [Fact(DisplayName = "CreateAsync â€“ retorna PessoaDto com dados corretos")]
    public async Task CreateAsync_DadosValidos_DeveRetornarPessoaDto()
    {
        var dto = new CreatePessoaDto { Nome = "Carlos", DataNascimento = new DateTime(1990, 6, 15) };
        _pessoaRepoMock.Setup(r => r.AddAsync(It.IsAny<Pessoa>())).Returns(Task.CompletedTask);

        var result = await _sut.CreateAsync(dto);

        result.Should().NotBeNull();
        result.Nome.Should().Be("Carlos");
        result.DataNascimento.Should().Be(new DateTime(1990, 6, 15));
    }

    [Fact(DisplayName = "CreateAsync â€“ lanÃ§a ArgumentNullException quando dto Ã© null")]
    public async Task CreateAsync_DtoNulo_DeveLancarArgumentNullException()
    {
        var act = () => _sut.CreateAsync(null!);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact(DisplayName = "CreateAsync â€“ SaveChangesAsync chamado uma vez")]
    public async Task CreateAsync_DadosValidos_DeveChamarSaveChangesUmaVez()
    {
        var dto = new CreatePessoaDto { Nome = "Ana", DataNascimento = DateTime.Today.AddYears(-20) };
        _pessoaRepoMock.Setup(r => r.AddAsync(It.IsAny<Pessoa>())).Returns(Task.CompletedTask);

        await _sut.CreateAsync(dto);

        _uowMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact(DisplayName = "GetByIdAsync â€“ retorna null quando pessoa nÃ£o existe")]
    public async Task GetByIdAsync_NaoExiste_DeveRetornarNull()
    {
        var id = Guid.NewGuid();
        _pessoaRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Pessoa?)null);

        var result = await _sut.GetByIdAsync(id);

        result.Should().BeNull();
    }

    [Fact(DisplayName = "GetByIdAsync â€“ retorna PessoaDto quando pessoa existe")]
    public async Task GetByIdAsync_Existe_DeveRetornarPessoaDto()
    {
        var id = Guid.NewGuid();
        var pessoa = new Pessoa { Nome = "Fernanda", DataNascimento = new DateTime(1995, 3, 20) };
        typeof(MinhasFinancas.Domain.Entities.EntityBase).GetProperty("Id")!.SetValue(pessoa, id);
        _pessoaRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(pessoa);

        var result = await _sut.GetByIdAsync(id);

        result.Should().NotBeNull();
        result!.Nome.Should().Be("Fernanda");
    }

    [Fact(DisplayName = "UpdateAsync â€“ lanÃ§a KeyNotFoundException quando pessoa nÃ£o existe")]
    public async Task UpdateAsync_NaoExiste_DeveLancarKeyNotFoundException()
    {
        var id = Guid.NewGuid();
        _pessoaRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Pessoa?)null);

        var act = () => _sut.UpdateAsync(id, new UpdatePessoaDto { Nome = "X", DataNascimento = DateTime.Today });

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact(DisplayName = "UpdateAsync â€“ atualiza nome e data de nascimento corretamente")]
    public async Task UpdateAsync_DadosValidos_DeveAtualizarCampos()
    {
        var id = Guid.NewGuid();
        var pessoa = new Pessoa { Nome = "Antigo", DataNascimento = new DateTime(2000, 1, 1) };
        typeof(MinhasFinancas.Domain.Entities.EntityBase).GetProperty("Id")!.SetValue(pessoa, id);
        _pessoaRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(pessoa);
        _pessoaRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Pessoa>())).Returns(Task.CompletedTask);

        await _sut.UpdateAsync(id, new UpdatePessoaDto { Nome = "Novo", DataNascimento = new DateTime(1998, 5, 10) });

        pessoa.Nome.Should().Be("Novo");
        pessoa.DataNascimento.Should().Be(new DateTime(1998, 5, 10));
        _uowMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact(DisplayName = "DeleteAsync â€“ chama DeleteAsync do repositÃ³rio e SaveChanges")]
    public async Task DeleteAsync_DeveRepassarParaRepositorioESalvar()
    {
        var id = Guid.NewGuid();
        _pessoaRepoMock.Setup(r => r.DeleteAsync(id)).Returns(Task.CompletedTask);

        await _sut.DeleteAsync(id);

        _pessoaRepoMock.Verify(r => r.DeleteAsync(id), Times.Once);
        _uowMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }
}
