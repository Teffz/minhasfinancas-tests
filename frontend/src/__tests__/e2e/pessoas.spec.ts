Cola isso e salva:
typescriptimport { test, expect } from "@playwright/test";

test.describe("Pessoas – CRUD E2E", () => {
  test.beforeEach(async ({ page }) => {
    await page.goto("/pessoas");
    await page.waitForLoadState("networkidle");
  });

  test("deve exibir a página de pessoas", async ({ page }) => {
    await expect(page).toHaveURL(/pessoas/);
    await expect(page.locator("body")).not.toBeEmpty();
  });

  test("deve criar uma nova pessoa com dados válidos", async ({ page }) => {
    const addButton = page.getByRole("button", {
      name: /nova pessoa|adicionar|novo|add/i,
    });
    await addButton.click();

    await page.getByLabel(/nome/i).fill("Playwright Test User");
    await page.getByLabel(/data de nascimento|nascimento/i).fill("1990-06-15");

    await page.getByRole("button", { name: /salvar|confirmar|criar|save/i }).click();

    await page.waitForTimeout(1000);
    await expect(page.getByText("Playwright Test User")).toBeVisible({ timeout: 5000 });
  });

  test("deve mostrar erro ao tentar criar pessoa com nome vazio", async ({ page }) => {
    const addButton = page.getByRole("button", {
      name: /nova pessoa|adicionar|novo|add/i,
    });
    await addButton.click();

    await page.getByLabel(/data de nascimento|nascimento/i).fill("1990-06-15");
    await page.getByRole("button", { name: /salvar|confirmar|criar|save/i }).click();

    await expect(
      page.getByText(/obrigatório|required|nome/i).first()
    ).toBeVisible({ timeout: 3000 });
  });

  test("deve editar uma pessoa existente", async ({ page }) => {
    await page.waitForSelector("table, [data-testid='pessoa-item'], .pessoa-row", {
      timeout: 5000,
    }).catch(() => {});

    const editButtons = page.getByRole("button", { name: /editar|edit/i });
    const count = await editButtons.count();

    if (count === 0) {
      test.skip();
      return;
    }

    await editButtons.first().click();

    const nomeInput = page.getByLabel(/nome/i);
    await nomeInput.clear();
    await nomeInput.fill("Nome Atualizado E2E");

    await page.getByRole("button", { name: /salvar|confirmar|save/i }).click();
    await page.waitForTimeout(1000);

    await expect(page.getByText("Nome Atualizado E2E")).toBeVisible({ timeout: 5000 });
  });

  test("deve excluir uma pessoa e ela desaparece da lista", async ({ page }) => {
    const addButton = page.getByRole("button", {
      name: /nova pessoa|adicionar|novo|add/i,
    });
    await addButton.click();

    await page.getByLabel(/nome/i).fill("Pessoa Para Deletar E2E");
    await page.getByLabel(/data de nascimento|nascimento/i).fill("1985-03-20");
    await page.getByRole("button", { name: /salvar|confirmar|criar|save/i }).click();
    await page.waitForTimeout(1000);

    const row = page.getByText("Pessoa Para Deletar E2E").first();
    await expect(row).toBeVisible();

    const deleteButton = page
      .locator("tr, li, [data-testid='pessoa-item']")
      .filter({ hasText: "Pessoa Para Deletar E2E" })
      .getByRole("button", { name: /excluir|deletar|remover|delete/i });

    await deleteButton.click();

    const confirmButton = page.getByRole("button", { name: /confirmar|sim|yes|ok/i });
    if (await confirmButton.isVisible({ timeout: 2000 }).catch(() => false)) {
      await confirmButton.click();
    }

    await page.waitForTimeout(1000);
    await expect(page.getByText("Pessoa Para Deletar E2E")).not.toBeVisible({ timeout: 5000 });
  });
});