import { describe, it, expect } from "vitest";
import { normalizePagedResponse } from "../../lib/apiUtils";

describe("normalizePagedResponse", () => {
  it("deve normalizar array direto como resultado paginado", () => {
    const raw = [{ id: "1" }, { id: "2" }];
    const result = normalizePagedResponse(raw);

    expect(result.items).toHaveLength(2);
    expect(result.total).toBe(2);
    expect(result.page).toBe(1);
  });

  it("deve normalizar resposta com propriedades camelCase", () => {
    const raw = { items: [{ id: "1" }], total: 10, page: 2, pageSize: 5 };
    const result = normalizePagedResponse(raw);

    expect(result.items).toHaveLength(1);
    expect(result.total).toBe(10);
    expect(result.page).toBe(2);
    expect(result.pageSize).toBe(5);
  });

  it("deve normalizar resposta com propriedades PascalCase", () => {
    const raw = { Items: [{ id: "a" }, { id: "b" }], Total: 20, Page: 1, PageSize: 10 };
    const result = normalizePagedResponse(raw);

    expect(result.items).toHaveLength(2);
    expect(result.total).toBe(20);
    expect(result.page).toBe(1);
    expect(result.pageSize).toBe(10);
  });

  it("deve retornar resultado vazio para null", () => {
    const result = normalizePagedResponse(null);

    expect(result.items).toEqual([]);
    expect(result.total).toBe(0);
  });

  it("deve retornar resultado vazio para undefined", () => {
    const result = normalizePagedResponse(undefined);

    expect(result.items).toEqual([]);
    expect(result.total).toBe(0);
  });

  it("deve retornar resultado vazio para string", () => {
    const result = normalizePagedResponse("invalid");

    expect(result.items).toEqual([]);
  });

  it("deve usar items.length como total quando total não está presente", () => {
    const raw = { items: [{ id: "x" }, { id: "y" }, { id: "z" }] };
    const result = normalizePagedResponse(raw);

    expect(result.total).toBe(3);
  });

  it("deve lidar com array vazio", () => {
    const raw: unknown[] = [];
    const result = normalizePagedResponse(raw);

    expect(result.items).toEqual([]);
    expect(result.total).toBe(0);
  });
});