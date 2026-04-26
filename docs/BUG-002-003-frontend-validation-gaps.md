# BUG-002 — Ausência de validação de data futura no schema Zod do frontend

## Severidade
**Média** — Usuário pode submeter data de nascimento no futuro e só recebe erro do backend.

## Descrição
O `pessoaSchema` Zod aceita qualquer `Date` para `dataNascimento`, incluindo datas
futuras. A validação existe apenas no backend (`PessoaValidation.ValidarDataNascimento`),
sem feedback imediato ao usuário no formulário.

## Como reproduzir
1. Navegar para `/pessoas`
2. Abrir o formulário de nova pessoa
3. Informar uma data de nascimento no futuro (ex: 2030)
4. Submeter

## Resultado atual
O formulário é submetido, a API é chamada e retorna `400`. O frontend exibe o erro
apenas após o round-trip HTTP.

## Resultado esperado
O schema Zod deveria rejeitar a data antes da chamada de API.

## Correção sugerida
```typescript
dataNascimento: z.date().refine(d => d <= new Date(), {
  message: "Data de nascimento não pode ser no futuro.",
}),
```

## Teste que documenta este bug
- `frontend/src/__tests__/unit/schemas.test.ts`
  - `⚠️ NÃO rejeita data de nascimento futura (BUG-002)`

---

# BUG-003 — Ausência de validação de compatibilidade categoria/tipo no frontend

## Severidade
**Média** — Usuário só descobre incompatibilidade após submeter o formulário e receber
erro 500 da API (ver BUG-001).

## Descrição
O `transacaoSchema` Zod não valida se a categoria selecionada é compatível com o
tipo de transação escolhido. Combinado com o BUG-001, o usuário recebe
"Ocorreu um erro interno no servidor" — mensagem completamente inutilizável.

## Correção sugerida
Filtrar as opções de categoria no formulário com base no tipo selecionado, ou
validar via `refine` no schema antes do submit.

## Teste que documenta este bug
- `frontend/src/__tests__/unit/schemas.test.ts`
  - `⚠️ não valida compatibilidade entre tipo e categoria (BUG-003)`