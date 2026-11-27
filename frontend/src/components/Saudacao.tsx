import { useEffect, useState } from "react";

// Criado uma vez só, fora do componente: não precisa ser refeito a cada renderização.
const formatoDaHora = new Intl.DateTimeFormat("pt-BR", {
  hour: "2-digit",
  minute: "2-digit",
  second: "2-digit",
});

function saudacaoPara(hora: number): string {
  if (hora >= 5 && hora < 12) {
    return "Bom dia";
  }

  if (hora >= 12 && hora < 18) {
    return "Boa tarde";
  }

  return "Boa noite";
}

export function Saudacao() {
  // A função passada ao useState só roda na primeira renderização (inicialização preguiçosa).
  const [agora, setAgora] = useState(() => new Date());

  useEffect(() => {
    const intervalo = window.setInterval(() => {
      setAgora(new Date());
    }, 1000);

    // Limpeza: roda quando o componente sai da tela. Sem ela, o relógio continuaria rodando para sempre.
    return () => {
      window.clearInterval(intervalo);
    };
  }, []);

  return (
    <p className="text-right text-sm text-white/80">
      <span className="block font-semibold text-white">
        {saudacaoPara(agora.getHours())}!
      </span>
      {formatoDaHora.format(agora)}
    </p>
  );
}
