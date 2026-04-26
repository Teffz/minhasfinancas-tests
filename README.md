# MinhasFinanças — Repositório de Testes

Repositório de testes automatizados para o sistema de controle de gastos residenciais.
Contém **exclusivamente testes** — o código-fonte da aplicação não está incluído.

---

## 📐 Estrutura do Repositório

minhasfinancas-tests/
│
├── backend/
│   ├── MinhasFinancas.Tests.Unit/
│   │   ├── Domain/
│   │   │   ├── PessoaTests.cs
│   │   │   ├── CategoriaTests.cs
│   │   │   └── TransacaoTests.cs
│   │   │
│   │   └── Application/
│   │       ├── TransacaoServiceTests.cs
│   │       ├── PessoaServiceTests.cs
│   │       └── PessoaValidationTests.cs
│   │
│   └── MinhasFinancas.Tests.Integration/
│       ├── Infrastructure/
│       │   ├── TestWebApplicationFactory.cs
│       │   └── IntegrationTestBase.cs
│       │
│       └── Api/
│           ├── PessoasControllerTests.cs
│           ├── TransacoesControllerTests.cs
│           └── CategoriasControllerTests.cs
│
├── frontend/
│   └── src/tests/
│       ├── unit/
│       │   ├── apiUtils.test.ts
│       │   ├── schemas.test.ts
│       │   └── mappers.test.ts
│       │
│       └── e2e/
│           ├── pessoas.spec.ts
│           ├── categorias.spec.ts
│           └── transacoes.spec.ts
│
│   ├── vitest.config.ts
│   └── playwright.config.ts
│
├── docs/
│   ├── BUG-001-invalid-operation-exception-500.md
│   └── BUG-002-003-frontend-validation-gaps.md
│
└── README.md



---

## 🏗️ Pirâmide de Testes
    /\
   /E2E\          Playwright — fluxos completos no browser
  /------\
 /  Integ  \      xUnit + WebApplicationFactory + SQLite in-memory
/------------\
/   Unitários  \   xUnit + Moq + FluentAssertions  |  Vitest
/________________\


---

## ⚙️ Pré-requisitos

O código-fonte da aplicação **não** está neste repositório. Clone-o separadamente:
<workspace>/
source/api/   ← código .NET
source/web/   ← código React
tests/        ← este repositório

---

## 🚀 Como Rodar os Testes

### Testes Unitários — Backend

```bash
cd backend

dotnet test MinhasFinancas.Tests.Unit/MinhasFinancas.Tests.Unit.csproj \
  -p:SourceRoot=C:\caminho\para\source\api \
  --logger "console;verbosity=normal"
```

### Testes de Integração — Backend

```bash
dotnet test MinhasFinancas.Tests.Integration/MinhasFinancas.Tests.Integration.csproj \
  -p:SourceRoot=C:\caminho\para\source\api \
  --logger "console;verbosity=normal"
```

### Testes Unitários — Frontend

```bash
cd frontend
npm install
npm run test:unit
```

### Testes E2E — Frontend

Inicie a aplicação primeiro:

```bash
# Terminal 1 — API
cd source/api/MinhasFinancas.API
dotnet run

# Terminal 2 — Frontend
cd source/web
bun dev
```

Depois:

```bash
cd frontend
npx playwright install chromium
npm run test:e2e
```

---

## 🐛 Bugs Encontrados

### BUG-001 — `InvalidOperationException` retorna 500 em vez de 400
**Severidade:** Alta  
Violações de regras de negócio (menor com receita, categoria errada) retornam 500.  
Detalhes: [`docs/BUG-001-invalid-operation-exception-500.md`](docs/BUG-001-invalid-operation-exception-500.md)

### BUG-002 — Data de nascimento futura aceita pelo schema Zod
**Severidade:** Média  
Validação de data futura ausente no frontend.  
Detalhes: [`docs/BUG-002-003-frontend-validation-gaps.md`](docs/BUG-002-003-frontend-validation-gaps.md)

### BUG-003 — Sem validação frontend de compatibilidade categoria/tipo
**Severidade:** Média  
Schema Zod não valida compatibilidade entre tipo de transação e finalidade da categoria.  
Detalhes: [`docs/BUG-002-003-frontend-validation-gaps.md`](docs/BUG-002-003-frontend-validation-gaps.md)

---

## 🧪 Justificativas das Escolhas

| Decisão | Justificativa |
|---------|---------------|
| **xUnit** | Framework padrão .NET, integração nativa com `dotnet test` |
| **Moq** | Mocking idiomático para .NET, sintaxe fluente |
| **FluentAssertions** | Mensagens de erro mais descritivas, facilita diagnóstico |
| **WebApplicationFactory + SQLite in-memory** | Testa pipeline completo sem banco externo |
| **Vitest** | Nativo ao Vite, zero config, compatível com TypeScript |
| **Playwright** | Seletores ARIA robustos, trace/screenshot on failure |
| **Testes documentam bugs** | Afirmam o comportamento atual incorreto — quebram quando o bug for corrigido |
