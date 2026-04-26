import { describe, it, expect } from "vitest";
import { TipoTransacao, Finalidade } from "../../types/domain";

function mapTipoTransacao(value: number | undefined): TipoTransacao {
  return value === 1 ? TipoTransacao.Receita : TipoTransacao.Despesa;
}

interface TransacaoApiResponse {
  id?: string; Id?: string;
  descricao?: string; Descricao?: string;
  valor?: number; Valor?: number;
  tipo?: number; Tipo?: number;
  categoriaId?: string; CategoriaId?: string;
  pessoaId?: string; PessoaId?: string;
  data?: string; Data?: string;
}

function mapTransacaoResponse(item: TransacaoApiResponse) {
  const id = String(item.id ?? item.Id ?? "");
  const descricao = String(item.descricao ?? item.Descricao ?? "");
  const valor = Number(item.valor ?? item.Valor ?? 0);
  const tipo = mapTipoTransacao(item.tipo ?? item.Tipo);
  const categoriaId = String(item.categoriaId ?? item.CategoriaId ?? "");
  const pessoaId = String(item.pessoaId ?? item.PessoaId ?? "");
  const dataVal = item.data ?? item.Data;
  const data = dataVal ? new Date(dataVal) : new Date();
  return { id, descricao, valor, tipo, categoriaId, pessoaId, data };
}

interface PessoaApiResponse {
  id?: string; Id?: string;
  nome?: string; Nome?: string;
  dataNascimento?: string | null; DataNascimento?: string | null;
  idade?: number; Idade?: number;
}

function mapPessoaResponse(item: PessoaApiResponse) {
  const id = String(item.id ?? item.Id ?? "");
  const nome = String(item.nome ?? item.Nome ?? "");
  const dataNascimentoStr = item.dataNascimento ?? item.DataNascimento;
  const dataNascimento = dataNascimentoStr ? new Date(dataNascimentoStr) : new Date();
  const idade = item.idade ?? item.Idade ?? 0;
  return { id, nome, dataNascimento, idade };
}

interface CategoriaApiResponse {
  id?: string; Id?: string;
  descricao?: string; Descricao?: string;
  finalidade?: number; Finalidade?: number;
}

const FINALIDADE_MAP: Record<number, Finalidade> = {
  0: Finalidade.Despesa,
  1: Finalidade.Receita,
  2: Finalidade.Ambas,
};

function mapCategoriaResponse(item: CategoriaApiResponse) {
  const id = String(item.id ?? item.Id ?? "");
  const descricao = String(item.descricao ?? item.Descricao ?? "");
  const rawFinalidade = item.finalidade ?? item.Finalidade ?? 0;
  const finalidade = FINALIDADE_MAP[rawFinalidade] ?? Finalidade.Despesa;
  return { id, descricao, finalidade };
}

describe("mapTipoTransacao", () => {
  it("mapeia 0 para Despesa", () => {
    expect(mapTipoTransacao(0)).toBe(TipoTransacao.Despesa);
  });
  it("mapeia 1 para Receita", () => {
    expect(mapTipoTransacao(1)).toBe(TipoTransacao.Receita);
  });
  it("mapeia undefined para Despesa (default)", () => {
    expect(mapTipoTransacao(undefined)).toBe(TipoTransacao.Despesa);
  });
  it("mapeia valor desconhecido para Despesa (fallback)", () => {
    expect(mapTipoTransacao(99)).toBe(TipoTransacao.Despesa);
  });
});

describe("mapTransacaoResponse", () => {
  it("mapeia resposta camelCase corretamente", () => {
    const raw: TransacaoApiResponse = {
      id: "t1", descricao: "Aluguel", valor: 1200, tipo: 0,
      categoriaId: "c1", pessoaId: "p1", data: "2024-07-15T00:00:00Z",
    };
    const result = mapTransacaoResponse(raw);
    expect(result.id).toBe("t1");
    expect(result.valor).toBe(1200);
    expect(result.tipo).toBe(TipoTransacao.Despesa);
  });
  it("mapeia resposta PascalCase corretamente", () => {
    const raw: TransacaoApiResponse = {
      Id: "t2", Descricao: "Salário", Valor: 4000, Tipo: 1,
      CategoriaId: "c2", PessoaId: "p2", Data: "2024-07-01T00:00:00Z",
    };
    const result = mapTransacaoResponse(raw);
    expect(result.id).toBe("t2");
    expect(result.tipo).toBe(TipoTransacao.Receita);
  });
  it("usa valor padrão 0 quando valor está ausente", () => {
    const raw: TransacaoApiResponse = { id: "t3", tipo: 0 };
    expect(mapTransacaoResponse(raw).valor).toBe(0);
  });
});

describe("mapPessoaResponse", () => {
  it("mapeia resposta camelCase corretamente", () => {
    const raw: PessoaApiResponse = {
      id: "p1", nome: "Alice", dataNascimento: "1990-03-20T00:00:00Z", idade: 34,
    };
    const result = mapPessoaResponse(raw);
    expect(result.id).toBe("p1");
    expect(result.nome).toBe("Alice");
    expect(result.idade).toBe(34);
  });
  it("mapeia resposta PascalCase corretamente", () => {
    const raw: PessoaApiResponse = { Id: "p2", Nome: "Bob", DataNascimento: "2000-11-05T00:00:00Z", Idade: 23 };
    const result = mapPessoaResponse(raw);
    expect(result.id).toBe("p2");
    expect(result.nome).toBe("Bob");
  });
  it("trata dataNascimento null sem lançar erro", () => {
    const raw: PessoaApiResponse = { id: "p3", nome: "Sem data", dataNascimento: null };
    expect(() => mapPessoaResponse(raw)).not.toThrow();
  });
});

describe("mapCategoriaResponse", () => {
  it("mapeia finalidade 0 para Despesa", () => {
    expect(mapCategoriaResponse({ id: "c1", finalidade: 0 }).finalidade).toBe(Finalidade.Despesa);
  });
  it("mapeia finalidade 1 para Receita", () => {
    expect(mapCategoriaResponse({ id: "c2", finalidade: 1 }).finalidade).toBe(Finalidade.Receita);
  });
  it("mapeia finalidade 2 para Ambas", () => {
    expect(mapCategoriaResponse({ id: "c3", finalidade: 2 }).finalidade).toBe(Finalidade.Ambas);
  });
  it("usa PascalCase Finalidade quando presente", () => {
    expect(mapCategoriaResponse({ Id: "c4", Finalidade: 0 }).finalidade).toBe(Finalidade.Despesa);
  });
});