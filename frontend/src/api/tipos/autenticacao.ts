import type { DataHoraIso, Guid } from "./comum";
import type { Perfil } from "./enums";

// POST /api/autenticacao/login
export type LoginResponse = {
  token: string;
  expiraEm: DataHoraIso;
};

// GET /api/autenticacao/me
export type UsuarioLogadoResponse = {
  id: Guid;
  nome: string;
  email: string;
  membroId: Guid | null;
  perfis: readonly Perfil[];
};
