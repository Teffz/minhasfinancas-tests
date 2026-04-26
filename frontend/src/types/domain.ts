export type ID = string;

export enum TipoTransacao {
  Despesa = "despesa",
  Receita = "receita",
}

export enum Finalidade {
  Despesa = "despesa",
  Receita = "receita",
  Ambas = "ambas",
}

export interface Categoria {
  id: ID;
  descricao: string;
  finalidade: Finalidade;
}

export interface Pessoa {
  id: ID;
  nome: string;
  dataNascimento: Date;
  idade: number;
}

export interface Transacao {
  id: ID;
  descricao: string;
  valor: number;
  tipo: TipoTransacao;
  categoriaId: ID;
  pessoaId: ID;
  data: Date;
}

export interface TotalPorPessoa {
  pessoaId: ID;
  nome: string;
  totalReceitas: number;
  totalDespesas: number;
  saldo: number;
}

export type PagedResult<T> = {
  items: T[];
  total: number;
  page: number;
  pageSize: number;
};