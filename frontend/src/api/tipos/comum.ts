// Tipos usados por todas as partes da API (a pasta Comum da Application no C#).

// Apelidos que documentam o formato do texto. Para o TypeScript, os três
// continuam sendo string: qualquer string serve onde se pede um Guid.

/** Guid do C#. Exemplo: "715f866b-0ecb-4924-a363-3ae6ff0cccbc". */
export type Guid = string;

/** DateOnly do C#: só a data, no formato ISO. Exemplo: "1990-05-05". */
export type DataIso = string;

/** DateTimeOffset do C#: data, hora e fuso. Exemplo: "2026-11-27T15:24:25.6087829-03:00". */
export type DataHoraIso = string;

// PaginaResponse<T> no C#: o T continua genérico aqui.
export type PaginaResponse<T> = {
  itens: readonly T[];
  pagina: number;
  tamanhoPagina: number;
  totalItens: number;
  totalPaginas: number;
};

export type EnderecoDto = {
  logradouro: string;
  numero: string;
  complemento: string | null;
  bairro: string;
  cidade: string;
  uf: string;
  cep: string;
};

// O corpo de toda resposta de erro da API (RFC 9457, application/problem+json),
// montado pelo TratadorGlobalDeExcecoes e pelo AddProblemDetails.
export type ProblemDetails = {
  type: string;
  title: string;
  status: number;
  detail?: string;
  traceId: string;
};

// O erro 400 da validação: os campos inválidos, cada um com as suas mensagens.
export type HttpValidationProblemDetails = ProblemDetails & {
  errors: Record<string, string[]>;
};
