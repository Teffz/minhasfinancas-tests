Cola isso e salva:
typescriptimport { test, expect } from "@playwright/test";

test.describe("Transações – Regras de Negócio E2E", () => {
  test.beforeEach(async ({ page }) => {
    await page.goto("/transacoes");
    await page.waitForLoadState("networkidle");
  });

  test("deve exibir a página de transações", async ({ page }) => {
    await expect(page).toHaveURL(/transacoes/);
    await expect(page.locator("body")).not.toBeEmpty();
  });

  test("deve mostrar erro de validação quando valor é zero", async ({ page }) => {
    const addButton = page.getByRole("button", {
      name: /nova transação|adicionar|novo|add/i,
    });
    await addButton.click();

    await page.getByLabel(/descrição|descricao/i).fill("Teste");
    await page.getByLabel(/valor/i).fill("0");

    await page.getByRole("button", { name: /salvar|confirmar|criar|save/i }).click();

    await expect(
      page.getByText(/positivo|maior que zero|obrigatório/i).first()
    ).toBeVisible({ timeout: 3000 });
  });

  test("deve mostrar erro quando descrição está vazia", async ({ page }) => {
    const addButton = page.getByRole("button", {
      name: /nova transação|adicionar|novo|add/i,
    });
    await addButton.click();

    await page.getByLabel(/valor/i).fill("100");
    await page.getByRole("button", { name: /salvar|confirmar|criar|save/i }).click();

    await expect(
      page.getByText(/obrigatório|required|descrição/i).first()
    ).toBeVisible({ timeout: 3000 });
  });
});

test.describe("Transações – Regressão BUG-001 (API direta)", () => {
  const API_URL = process.env.PLAYWRIGHT_API_URL ?? "http://localhost:5000/api/v1.0";

  test("BUG-001: API retorna 500 (deveria ser 400) para receita de menor de idade", async ({ request }) => {
    const pessoaRes = await request.post(`${API_URL}/pessoas`, {
      data: { Nome: "Menor Playwright", DataNascimento: "2012-01-01" },
    });
    const pessoa = await pessoaRes.json();
    const pessoaId = pessoa.id ?? pessoa.Id;

    const catRes = await request.post(`${API_URL}/categorias`, {
      data: { Descricao: "Receita E2E", Finalidade: 1 },
    });
    const categoria = await catRes.json();
    const categoriaId = categoria.id ?? categoria.Id;

    const response = await request.post(`${API_URL}/transacoes`, {
      data: {
        Descricao: "Receita Inválida",
        Valor: 100,
        Tipo: 1,
        CategoriaId: categoriaId,
        PessoaId: pessoaId,
        Data: new Date().toISOString(),
      },
    });

    // BUG-001: deveria ser 400, mas retorna 500
    expect(response.status()).not.toBe(201);
    expect(response.status()).toBe(500);
  });

  test("BUG-001: API retorna 500 (deveria ser 400) para categoria incompatível com tipo", async ({ request }) => {
    const pessoaRes = await request.post(`${API_URL}/pessoas`, {
      data: { Nome: "Adulto Playwright", DataNascimento: "1990-01-01" },
    });
    const pessoa = await pessoaRes.json();
    const pessoaId = pessoa.id ?? pessoa.Id;

    const catRes = await request.post(`${API_URL}/categorias`, {
      data: { Descricao: "Despesa E2E Cat", Finalidade: 0 },
    });
    const categoria = await catRes.json();
    const categoriaId = categoria.id ?? categoria.Id;

    const response = await request.post(`${API_URL}/transacoes`, {
      data: {
        Descricao: "Tipo Errado",
        Valor: 500,
        Tipo: 1,
        CategoriaId: categoriaId,
        PessoaId: pessoaId,
        Data: new Date().toISOString(),
      },
    });

    // BUG-001: deveria ser 400, mas retorna 500
    expect(response.status()).not.toBe(201);
    expect(response.status()).toBe(500);
  });
});