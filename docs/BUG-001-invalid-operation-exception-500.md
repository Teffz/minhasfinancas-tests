# BUG-001 — InvalidOperationException retorna HTTP 500 em vez de 400

## Severidade
**Alta** — Violações de regras de negócio retornam erro de servidor ao cliente.

## Descrição
Quando uma regra de negócio é violada ao criar uma transação, o sistema lança uma
`InvalidOperationException` no nível de domínio. O `ExceptionMiddleware` captura
**qualquer** exceção não tratada e retorna `HTTP 500 Internal Server Error`, sem
distinguir erros de domínio (que deveriam ser `400 Bad Request`) de erros reais
de infraestrutura.

## Regras afetadas
1. **Menor de idade não pode ter receitas**
2. **Categoria incompatível com o tipo da transação**

## Como reproduzir

```bash
# 1. Criar um menor de idade
POST /api/v1.0/pessoas
{ "Nome": "Menor Teste", "DataNascimento": "2012-01-01" }

# 2. Criar uma categoria de receita
POST /api/v1.0/categorias
{ "Descricao": "Receita", "Finalidade": 1 }

# 3. Tentar criar receita para o menor
POST /api/v1.0/transacoes
{
  "Descricao": "Mesada",
  "Valor": 100,
  "Tipo": 1,
  "CategoriaId": "<id da categoria>",
  "PessoaId": "<id do menor>",
  "Data": "2024-07-01"
}
```

## Resultado atual
```json
HTTP 500 Internal Server Error
{
  "StatusCode": 500,
  "Message": "Ocorreu um erro interno no servidor.",
  "Detailed": "Menores de 18 anos não podem registrar receitas."
}
```

## Resultado esperado
```json
HTTP 400 Bad Request
{
  "StatusCode": 400,
  "Message": "Menores de 18 anos não podem registrar receitas."
}
```

## Localização do código
**Arquivo:** `api/MinhasFinancas.API/Middlewares/ExceptionMiddleware.cs`

O middleware mapeia toda exceção para 500 sem distinguir o tipo.

## Correção sugerida
```csharp
context.Response.StatusCode = exception switch
{
    InvalidOperationException => (int)HttpStatusCode.BadRequest,
    KeyNotFoundException       => (int)HttpStatusCode.NotFound,
    ArgumentException          => (int)HttpStatusCode.BadRequest,
    _                          => (int)HttpStatusCode.InternalServerError,
};
```

## Testes que documentam este bug
- `backend/MinhasFinancas.Tests.Integration/Api/TransacoesControllerTests.cs`
  - `Post_ReceitaMenorIdade_DeveRetornarErro`
  - `Post_CategoriaIncompativelComTipo_DeveRetornarErro`
- `frontend/src/__tests__/e2e/transacoes.spec.ts`
  - `BUG-001: API retorna 500 para receita de menor de idade`
  - `BUG-001: API retorna 500 para categoria incompatível com tipo`