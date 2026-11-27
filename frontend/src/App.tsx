import { useEffect, useState } from "react";
import { Cabecalho } from "./components/Cabecalho";
import { CampoDeBusca } from "./components/CampoDeBusca";
import { CartaoDeModulo } from "./components/CartaoDeModulo";
import { Saudacao } from "./components/Saudacao";

type Modulo = {
  titulo: string;
  descricao: string;
};

// readonly: a lista é fixa, e o TypeScript recusa push, pop ou qualquer alteração nela.
const modulos: readonly Modulo[] = [
  {
    titulo: "Membros",
    descricao:
      "Cadastro, cargos, aniversariantes e o alcance por igreja e por grupo.",
  },
  {
    titulo: "Registros",
    descricao: "Batismos, apresentações de crianças e certificados em PDF.",
  },
  {
    titulo: "Financeiro",
    descricao:
      "Dízimos, ofertas, despesas, relatórios, fechamento do mês e recibos.",
  },
];

// Minúsculas e sem acento: "dizimo" encontra "Dízimos".
function normalizar(texto: string): string {
  return texto
    .normalize("NFD")
    .replace(/\p{Diacritic}/gu, "")
    .toLowerCase()
    .trim();
}

function App() {
  const [busca, setBusca] = useState("");

  // Estado derivado: calculado a cada renderização a partir da busca, sem um useState próprio.
  const termo = normalizar(busca);
  const modulosVisiveis = modulos.filter((modulo) =>
    normalizar(`${modulo.titulo} ${modulo.descricao}`).includes(termo),
  );

  // Sincroniza algo de fora do React (o título da aba) com o estado.
  useEffect(() => {
    document.title =
      termo === "" ? "Temple" : `Temple · busca: ${busca.trim()}`;
  }, [termo, busca]);

  return (
    <div className="min-h-screen bg-slate-50 text-slate-800">
      <Cabecalho titulo="Temple" subtitulo="Sistema de gestão de igrejas">
        <Saudacao />
      </Cabecalho>

      <main className="mx-auto max-w-5xl px-6 py-10">
        <h2 className="text-lg font-semibold text-temple-azul">
          O que o sistema já faz
        </h2>

        <div className="mt-4">
          <CampoDeBusca valor={busca} aoMudar={setBusca} />
          <p className="mt-2 text-xs text-slate-500">
            {modulosVisiveis.length} de {modulos.length} módulos
          </p>
        </div>

        {modulosVisiveis.length === 0 ? (
          <p className="mt-6 rounded-lg border border-dashed border-slate-300 bg-white p-6 text-center text-sm text-slate-500">
            Nenhum módulo encontrado para “{busca.trim()}”.
          </p>
        ) : (
          <ul className="mt-6 grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
            {modulosVisiveis.map((modulo) => (
              <CartaoDeModulo
                key={modulo.titulo}
                titulo={modulo.titulo}
                descricao={modulo.descricao}
              />
            ))}
          </ul>
        )}
      </main>
    </div>
  );
}

export default App;
