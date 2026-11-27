import type { DataHoraIso, DataIso, EnderecoDto, Guid } from "./comum";
import type { GrupoMembro, TipoCargo } from "./enums";

// GET /api/membros/{id}; GET /api/membros devolve PaginaResponse<MembroResponse>.
export type MembroResponse = {
  id: Guid;
  matricula: number;
  nome: string;
  dataNascimento: DataIso;
  endereco: EnderecoDto;
  igrejaId: Guid;
  cargoAtual: TipoCargo;
  grupo: GrupoMembro | null;
  ativo: boolean;
};

// GET /api/membros/{id}/historico-cargos
export type HistoricoCargoResponse = {
  id: Guid;
  cargoAnterior: TipoCargo;
  cargoNovo: TipoCargo;
  dataPromocao: DataIso;
  rebaixamento: boolean;
  observacao: string | null;
  promovidoPorId: Guid | null;
  promovidoPorNome: string | null;
  registradoEm: DataHoraIso | null;
};
