import type { ReactNode } from "react";

type CabecalhoProps = {
  titulo: string;
  subtitulo: string;
  // O que for colocado entre <Cabecalho> e </Cabecalho> aparece à direita do título.
  children?: ReactNode;
};

export function Cabecalho({ titulo, subtitulo, children }: CabecalhoProps) {
  return (
    <header className="bg-temple-azul text-white">
      <div className="mx-auto flex max-w-5xl flex-wrap items-end justify-between gap-4 px-6 py-8">
        <div>
          <h1 className="text-3xl font-bold tracking-tight">{titulo}</h1>
          <p className="mt-1 text-sm text-white/80">{subtitulo}</p>
        </div>
        {children}
      </div>
    </header>
  );
}
