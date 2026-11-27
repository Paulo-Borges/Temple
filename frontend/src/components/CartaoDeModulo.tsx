type CartaoDeModuloProps = {
  titulo: string;
  descricao: string;
};

export function CartaoDeModulo({ titulo, descricao }: CartaoDeModuloProps) {
  return (
    <li className="rounded-lg border border-slate-200 bg-white p-5 shadow-sm transition-shadow hover:shadow-md">
      <h3 className="font-semibold text-temple-dourado">{titulo}</h3>
      <p className="mt-2 text-sm text-slate-600">{descricao}</p>
    </li>
  );
}
