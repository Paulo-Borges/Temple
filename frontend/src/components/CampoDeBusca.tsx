type CampoDeBuscaProps = {
  valor: string;
  aoMudar: (valor: string) => void;
};

// Componente controlado: o texto mora no estado do pai, e o campo só mostra e avisa quando muda.
export function CampoDeBusca({ valor, aoMudar }: CampoDeBuscaProps) {
  return (
    <label className="block space-x-2">
      <span className="text-sm font-medium text-slate-700">Buscar módulo</span>
      <input
        type="search"
        value={valor}
        onChange={(evento) => aoMudar(evento.target.value)}
        placeholder="Ex.: dízimo, batismo, certificado..."
        className="mt-1 w-full rounded-md border border-slate-300 bg-white px-3 py-2 text-sm shadow-sm focus:border-temple-azul focus:ring-2 focus:ring-temple-azul/30 focus:outline-none sm:max-w-sm"
      />
    </label>
  );
}
