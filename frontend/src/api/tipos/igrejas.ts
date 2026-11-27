import type { DataIso, EnderecoDto, Guid } from "./comum";

// GET /api/igrejas e GET /api/igrejas/{id}
export type IgrejaResponse = {
  id: Guid;
  nome: string;
  cnpj: string;
  telefone: string;
  dataFundacao: DataIso;
  ativa: boolean;
  ehMatriz: boolean;
  igrejaMatrizId: Guid | null;
  endereco: EnderecoDto;
};
