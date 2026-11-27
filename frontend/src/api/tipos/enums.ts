// Os enums do C# chegam da API como texto ("Pastor", "Diacono"), porque o
// Program.cs liga o JsonStringEnumConverter. O número do enum nunca é enviado.
//
// Cada enum vira três coisas:
// 1. uma lista "as const", que existe em tempo de execução (para um <select>);
// 2. um tipo derivado da lista, que só existe na compilação;
// 3. os rótulos para exibir na tela, com acento.
// A ordem da lista é a mesma do enum no C#.

export const PERFIS = [
  "Pastor",
  "Tesoureiro",
  "Secretario",
  "Lider",
  "LiderVaroes",
  "LiderIrmas",
  "LiderJovens",
  "LiderCriancas",
  "Funcionario",
  "Cozinheira",
] as const;

export type Perfil = (typeof PERFIS)[number];

export const ROTULOS_DE_PERFIL: Record<Perfil, string> = {
  Pastor: "Pastor",
  Tesoureiro: "Tesoureiro",
  Secretario: "Secretário",
  Lider: "Líder",
  LiderVaroes: "Líder dos Varões",
  LiderIrmas: "Líder das Irmãs",
  LiderJovens: "Líder dos Jovens",
  LiderCriancas: "Líder das Crianças",
  Funcionario: "Funcionário",
  Cozinheira: "Cozinheira",
};

export const TIPOS_DE_CARGO = [
  "Membro",
  "Cooperador",
  "Diacono",
  "Presbitero",
  "Evangelista",
  "Missionario",
  "Pastor",
] as const;

export type TipoCargo = (typeof TIPOS_DE_CARGO)[number];

export const ROTULOS_DE_CARGO: Record<TipoCargo, string> = {
  Membro: "Membro",
  Cooperador: "Cooperador",
  Diacono: "Diácono",
  Presbitero: "Presbítero",
  Evangelista: "Evangelista",
  Missionario: "Missionário",
  Pastor: "Pastor",
};

export const GRUPOS_DE_MEMBRO = [
  "Varoes",
  "Irmas",
  "Jovens",
  "Criancas",
] as const;

export type GrupoMembro = (typeof GRUPOS_DE_MEMBRO)[number];

export const ROTULOS_DE_GRUPO: Record<GrupoMembro, string> = {
  Varoes: "Varões",
  Irmas: "Irmãs",
  Jovens: "Jovens",
  Criancas: "Crianças",
};
