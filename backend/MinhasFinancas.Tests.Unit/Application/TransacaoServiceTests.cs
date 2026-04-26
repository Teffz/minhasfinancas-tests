using Xunit;
using FluentAssertions;
using MinhasFinancas.Application.DTOs;
using MinhasFinancas.Application.Services;
using MinhasFinancas.Domain.Entities;
using MinhasFinancas.Domain.Interfaces;
using Moq;

namespace MinhasFinancas.Tests.Unit.Application;

public class TransacaoServiceTests
{
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly Mock<ICategoriaRepository> _categoriaRepoMock;
    private readonly Mock<IPessoaRepository> _pessoaRepoMock;
    private readonly Mock<ITransacaoRepository> _transacaoRepoMock;
    private readonly TransacaoService _sut;

    public TransacaoServiceTests()
    {
        _uowMock = new Mock<IUnitOfWork>();
        _categoriaRepoMock = new Mock<ICategoriaRepository>();
        _pessoaRepoMock = new Mock<IPessoaRepository>();
        _transacaoRepoMock = new Mock<ITransacaoRepository>();

        _uowMock.Setup(u => u.Categorias).Returns(_categoriaRepoMock.Object);
        _uowMock.Setup(u => u.Pessoas).Returns(_pessoaRepoMock.Object);
        _uowMock.Setup(u => u.Transacoes).Returns(_transacaoRepoMock.Object);
        _uowMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        _sut = new TransacaoService(_uowMock.Object);
    }

    [Fact(DisplayName = "CreateAsync â€“ lanÃ§a ArgumentException quando categoria nÃ£o encontrada")]
    public async Task CreateAsync_CategoriaNaoEncontrada_DeveLancarArgumentException()
    {
        var dto = BuildDto(Transacao.ETipo.Despesa);
        _categoriaRepoMock.Setup(r => r.GetByIdAsync(dto.CategoriaId)).ReturnsAsync((Categoria?)null);

        var act = () => _sut.CreateAsync(dto);

        await act.Should().ThrowAsync<ArgumentException>().WithMessage("*Categoria*");
    }

    [Fact(DisplayName = "CreateAsync â€“ lanÃ§a ArgumentException quando pessoa nÃ£o encontrada")]
    public async Task CreateAsync_PessoaNaoEncontrada_DeveLancarArgumentException()
    {
        var dto = BuildDto(Transacao.ETipo.Despesa);
        var categoria = CriarCategoria(Categoria.EFinalidade.Despesa);

        _categoriaRepoMock.Setup(r => r.GetByIdAsync(dto.CategoriaId)).ReturnsAsync(categoria);
        _pessoaRepoMock.Setup(r => r.GetByIdAsync(dto.PessoaId)).ReturnsAsync((Pessoa?)null);

        var act = () => _sut.CreateAsync(dto);

        await act.Should().ThrowAsync<ArgumentException>().WithMessage("*Pessoa*");
    }

    [Fact(DisplayName = "CreateAsync â€“ lanÃ§a InvalidOperationException quando menor de idade tenta criar receita")]
    public async Task CreateAsync_MenorDeIdadeReceita_DeveLancarInvalidOperationException()
    {
        var dto = BuildDto(Transacao.ETipo.Receita);
        var categoria = CriarCategoria(Categoria.EFinalidade.Receita);
        var menor = CriarPessoa(dataNascimento: DateTime.Today.AddYears(-15));

        _categoriaRepoMock.Setup(r => r.GetByIdAsync(dto.CategoriaId)).ReturnsAsync(categoria);
        _pessoaRepoMock.Setup(r => r.GetByIdAsync(dto.PessoaId)).ReturnsAsync(menor);

        var act = () => _sut.CreateAsync(dto);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*18*");
    }

    [Fact(DisplayName = "CreateAsync â€“ menor de idade pode criar despesa sem exceÃ§Ã£o")]
    public async Task CreateAsync_MenorDeIdadeDespesa_NaoDeveLancarExcecao()
    {
        var dto = BuildDto(Transacao.ETipo.Despesa);
        var categoria = CriarCategoria(Categoria.EFinalidade.Despesa);
        var menor = CriarPessoa(dataNascimento: DateTime.Today.AddYears(-15));

        _categoriaRepoMock.Setup(r => r.GetByIdAsync(dto.CategoriaId)).ReturnsAsync(categoria);
        _pessoaRepoMock.Setup(r => r.GetByIdAsync(dto.PessoaId)).ReturnsAsync(menor);
        _transacaoRepoMock.Setup(r => r.AddAsync(It.IsAny<Transacao>())).Returns(Task.CompletedTask);

        var result = await _sut.CreateAsync(dto);

        result.Should().NotBeNull();
        result.Tipo.Should().Be(Transacao.ETipo.Despesa);
    }

    [Fact(DisplayName = "CreateAsync â€“ lanÃ§a InvalidOperationException ao usar categoria de despesa para receita")]
    public async Task CreateAsync_CategoriaDespesaParaReceita_DeveLancarInvalidOperationException()
    {
        var dto = BuildDto(Transacao.ETipo.Receita);
        var categoriaDespesa = CriarCategoria(Categoria.EFinalidade.Despesa);
        var adulto = CriarPessoa();

        _categoriaRepoMock.Setup(r => r.GetByIdAsync(dto.CategoriaId)).ReturnsAsync(categoriaDespesa);
        _pessoaRepoMock.Setup(r => r.GetByIdAsync(dto.PessoaId)).ReturnsAsync(adulto);

        var act = () => _sut.CreateAsync(dto);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact(DisplayName = "CreateAsync â€“ retorna TransacaoDto com dados corretos quando sucesso")]
    public async Task CreateAsync_DadosValidos_DeveRetornarTransacaoDtoCorreto()
    {
        var dto = BuildDto(Transacao.ETipo.Despesa, descricao: "Aluguel", valor: 1500m);
        var categoria = CriarCategoria(Categoria.EFinalidade.Despesa, "Moradia");
        var adulto = CriarPessoa(nome: "JoÃ£o");

        _categoriaRepoMock.Setup(r => r.GetByIdAsync(dto.CategoriaId)).ReturnsAsync(categoria);
        _pessoaRepoMock.Setup(r => r.GetByIdAsync(dto.PessoaId)).ReturnsAsync(adulto);
        _transacaoRepoMock.Setup(r => r.AddAsync(It.IsAny<Transacao>())).Returns(Task.CompletedTask);

        var result = await _sut.CreateAsync(dto);

        result.Should().NotBeNull();
        result.Descricao.Should().Be("Aluguel");
        result.Valor.Should().Be(1500m);
        result.Tipo.Should().Be(Transacao.ETipo.Despesa);
        result.CategoriaDescricao.Should().Be("Moradia");
        result.PessoaNome.Should().Be("JoÃ£o");
    }

    [Fact(DisplayName = "CreateAsync â€“ SaveChangesAsync Ã© chamado exatamente uma vez")]
    public async Task CreateAsync_DadosValidos_DeveChamarSaveChangesUmaVez()
    {
        var dto = BuildDto(Transacao.ETipo.Despesa);
        var categoria = CriarCategoria(Categoria.EFinalidade.Despesa);
        var adulto = CriarPessoa();

        _categoriaRepoMock.Setup(r => r.GetByIdAsync(dto.CategoriaId)).ReturnsAsync(categoria);
        _pessoaRepoMock.Setup(r => r.GetByIdAsync(dto.PessoaId)).ReturnsAsync(adulto);
        _transacaoRepoMock.Setup(r => r.AddAsync(It.IsAny<Transacao>())).Returns(Task.CompletedTask);

        await _sut.CreateAsync(dto);

        _uowMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact(DisplayName = "GetByIdAsync â€“ retorna null quando transaÃ§Ã£o nÃ£o existe")]
    public async Task GetByIdAsync_NaoExiste_DeveRetornarNull()
    {
        var id = Guid.NewGuid();
        _transacaoRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Transacao?)null);

        var result = await _sut.GetByIdAsync(id);

        result.Should().BeNull();
    }

    private static CreateTransacaoDto BuildDto(
        Transacao.ETipo tipo,
        string descricao = "TransaÃ§Ã£o Teste",
        decimal valor = 100m)
        => new()
        {
            Descricao = descricao,
            Valor = valor,
            Tipo = tipo,
            CategoriaId = Guid.NewGuid(),
            PessoaId = Guid.NewGuid(),
            Data = DateTime.Today
        };

    private static Categoria CriarCategoria(
        Categoria.EFinalidade finalidade,
        string descricao = "Categoria Teste")
        => new() { Descricao = descricao, Finalidade = finalidade };

    private static Pessoa CriarPessoa(
        string nome = "Pessoa Teste",
        DateTime? dataNascimento = null)
        => new()
        {
            Nome = nome,
            DataNascimento = dataNascimento ?? DateTime.Today.AddYears(-30)
        };
}
