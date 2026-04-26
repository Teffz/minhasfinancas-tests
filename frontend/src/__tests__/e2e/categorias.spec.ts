import { test, expect } from "@playwright/test";

test.describe("Categorias – CRUD E2E", () => {
  test.beforeEach(async ({ page }) => {
    await page.goto("/categorias");
    await page.waitForLoadState("networkidle");
  });

  test("deve exibir a página de categorias", async ({ page }) => {
    await expect(page).toHaveURL(/categorias/);
    await expect(page.locator("body")).not.toBeEmpty();
  });

  test("deve criar categoria com finalidade Despesa", async ({ page }) => {
    const addButton = page.getByRole("button", {
      name: /nova categoria|adicionar|novo|add/i,
    });
    await addButton.click();

    await page.getByLabel(/descrição|descricao/i).fill("Alimentação E2E");

    const finalidadeSelect = page.getByLabel(/finalidade/i);
    await finalidadeSelect.selectOption({ label: /despesa/i });

    await page.getByRole("button", { name: /salvar|confirmar|criar|save/i }).click();
    await page.waitForTimeout(1000);

    await expect(page.getByText("Alimentação E2E")).toBeVisible({ timeout: 5000 });
  });

  test("deve criar categoria com finalidade Receita", async ({ page }) => {
    const addButton = page.getByRole("button", {
      name: /nova categoria|adicionar|novo|add/i,
    });
    await addButton.click();

    await page.getByLabel(/descrição|descricao/i).fill("Salário E2E");

    const finalidadeSelect = page.getByLabel(/finalidade/i);
    await finalidadeSelect.selectOption({ label: /receita/i });

    await page.getByRole("button", { name: /salvar|confirmar|criar|save/i }).click();
    await page.waitForTimeout(1000);

    await expect(page.getByText("Salário E2E")).toBeVisible({ timeout: 5000 });
  });

  test("deve criar categoria com finalidade Ambas", async ({ page }) => {
    const addButton = page.getByRole("button", {
      name: /nova categoria|adicionar|novo|add/i,
    });
    await addButton.click();

    await page.getByLabel(/descrição|descricao/i).fill("Geral E2E");

    const finalidadeSelect = page.getByLabel(/finalidade/i);
    await finalidadeSelect.selectOption({ label: /ambas/i });

    await page.getByRole("button", { name: /salvar|confirmar|criar|save/i }).click();
    await page.waitForTimeout(1000);

    await expect(page.getByText("Geral E2E")).toBeVisible({ timeout: 5000 });
  });

  test("deve mostrar erro ao criar categoria com descrição vazia", async ({ page }) => {
    const addButton = page.getByRole("button", {
      name: /nova categoria|adicionar|novo|add/i,
    });
    await addButton.click();

    await page.getByRole("button", { name: /salvar|confirmar|criar|save/i }).click();

    await expect(
      page.getByText(/obrigatório|required|descrição/i).first()
    ).toBeVisible({ timeout: 3000 });
  });
});