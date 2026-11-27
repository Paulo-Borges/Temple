# 💼 Temple: Para a Entrevista

> As perguntas que mais aparecem em entrevistas de emprego sobre o que foi feito em cada aula, com uma resposta curta e direta. Este arquivo é atualizado a cada aula, junto com o [PLANO-DE-ESTUDO.md](PLANO-DE-ESTUDO.md).

**Como usar:** leia a pergunta, tente responder em voz alta e só depois confira a resposta. Na entrevista, a resposta curta vem primeiro; o detalhe, só se o entrevistador pedir.

---

## 📑 Índice

| Módulo | Tema | Aulas com perguntas |
|---|---|---|
| 6 | [Fundamentos do Frontend](#módulo-6-fundamentos-do-frontend) | 6.1, 6.2 |
| — | [Git](#git) | — |

---

## Módulo 6: Fundamentos do Frontend

### Aula 6.1: Vite + React + TypeScript strict + Tailwind CSS

**O que é o Vite, e por que não o Create React App?**
> O Vite é uma ferramenta de build. Em desenvolvimento, ele serve os arquivos como ES Modules nativos do navegador, sem empacotar, então sobe na hora, e o HMR (*Hot Module Replacement*) atualiza só o módulo alterado, sem recarregar a página. Em produção, ele empacota e minifica tudo em `dist/`. O Create React App foi descontinuado; hoje a recomendação é o Vite ou um framework como o Next.js.

**SPA × SSR: qual a diferença?**
> A SPA (*Single Page Application*) entrega um HTML quase vazio, com uma `div` raiz, e o JavaScript monta e troca as telas no navegador. É ótima para sistemas internos com login, como o Temple. O SSR (*Server-Side Rendering*, como no Next.js) gera o HTML no servidor: é melhor para SEO e para a primeira carga de sites públicos.

**Para que serve o `StrictMode`? Por que meu componente renderiza duas vezes?**
> É uma ajuda só de desenvolvimento: ele renderiza cada componente duas vezes e roda os efeitos duas vezes, para revelar componentes impuros e efeitos sem limpeza. Em produção, não faz nada.

**Para que serve a `key` numa lista?**
> Para o React identificar cada item entre uma renderização e outra e atualizar só o que mudou. Ela tem de ser única e estável, como o `id` que vem da API. O índice da lista só serve se a lista nunca muda de ordem.

**Por que `className` e não `class` no JSX?**
> Porque o JSX vira JavaScript, e `class` é uma palavra reservada do JavaScript.

**Por que o nome de um componente começa com letra maiúscula?**
> É assim que o React diferencia um componente (`<App />`) de uma tag HTML (`<div>`).

**`type` ou `interface`?**
> Para descrever objetos, os dois funcionam. A `interface` pode ser estendida e reaberta (*declaration merging*). O `type` também descreve uniões (`'Pix' | 'Dinheiro'`), tuplas e tipos derivados. Muitos times usam `type` por padrão; o importante é ser consistente.

**O que o `strict` do TypeScript faz? E o `any`?**
> O `strict` liga as checagens importantes: proíbe `any` implícito, trata `null` e `undefined` (*strictNullChecks*) e outras. O `any` desliga a checagem e deve ser evitado; para um valor desconhecido, use `unknown` e faça o *narrowing* antes de usar.

**O que é o `!` (*non-null assertion*), e por que evitar?**
> É dizer ao compilador "confie em mim, isto não é nulo", o que desliga a checagem. Se o valor for nulo, o erro aparece só em produção. O melhor é tratar o caso com um `if` (*narrowing*): depois do `if (x === null) throw ...`, o TypeScript já sabe que `x` não é nulo.

**O que o `noUncheckedIndexedAccess` muda?**
> Ler `lista[0]` ou `dicionario[chave]` passa a ter o tipo `T | undefined`, porque a posição pode não existir. O compilador obriga a tratar o caso vazio antes de usar.

**`dependencies` × `devDependencies`?**
> As `dependencies` vão para o código que roda no navegador (React, React DOM). As `devDependencies` só servem para desenvolver e gerar o build (TypeScript, Vite, Tailwind, linter).

**`npm install` × `npm ci`? Para que serve o `package-lock.json`?**
> O `package-lock.json` grava a versão exata de cada pacote, inclusive os pacotes dos pacotes, para todo mundo instalar igual. O `npm install` pode atualizar o lock dentro das faixas do `^`. O `npm ci` instala **exatamente** o que está no lock, apaga a `node_modules` antes e falha se o lock não bater com o `package.json`: é o comando de CI e de deploy.

**O que significa o `^` em `"^4.3.5"`?**
> Aceita atualizações compatíveis (4.3.6, 4.4.0), mas nunca a próxima versão principal (5.0), que pode quebrar o código. É o versionamento semântico (*semver*): MAJOR.MINOR.PATCH.

**Por que Tailwind? Não fica uma bagunça de classes?**
> O Tailwind é *utility-first*: o estilo fica junto do componente, sem inventar nomes de classe e sem CSS morto, porque só as classes usadas entram no build. A responsividade (`sm:`, `lg:`) e os estados (`hover:`) ficam explícitos. A desvantagem é o HTML mais longo; quando uma combinação se repete, ela vira um **componente React**, e não uma classe CSS.

**O que é *mobile-first* no Tailwind?**
> A classe sem prefixo vale para todas as telas, e os prefixos `sm:`, `md:` e `lg:` valem dali para cima. Primeiro se pensa na tela do celular; depois, nos ajustes para telas maiores.

**Tailwind 3 × 4?**
> No 3, a configuração ficava num `tailwind.config.js`, e era preciso o PostCSS. No 4, ela fica no próprio CSS (`@import "tailwindcss"` e `@theme`), com um plugin para o Vite. As classes são praticamente as mesmas.

### Aula 6.2: Componentes, props tipadas, `useState` e `useEffect`

**O que é um componente React?**
> Uma função que recebe dados (as props) e devolve JSX, a descrição do que aparece na tela. O nome começa com maiúscula. Quebrar a tela em componentes dá responsabilidade única, reaproveitamento e código mais fácil de ler e testar.

**Props × estado?**
> As props vêm **de fora** (do componente pai) e são somente leitura: o componente nunca as altera. O estado é a memória **interna** do componente e muda com o `set` do `useState`. Mudar o estado faz o componente renderizar de novo.

**Como tipar props no TypeScript?**
> Com um `type` (ou `interface`) que descreve cada prop, com `?` nas opcionais, e desestruturando na assinatura: `function Cartao({ titulo }: CartaoProps)`. Se faltar uma prop obrigatória ou vier do tipo errado, o build quebra.

**O que é `children`?**
> A prop especial com tudo o que fica entre a abertura e o fechamento da tag do componente. O tipo é `ReactNode`. Serve para **composição**: o componente define onde o conteúdo aparece, sem saber qual é o conteúdo. É o jeito React de reaproveitar, no lugar de herança.

**Como o filho avisa o pai de alguma coisa?**
> O pai passa uma **função** como prop (por exemplo, `aoMudar={setBusca}`), e o filho a chama. Os dados descem por props, e os eventos sobem por funções (*one-way data flow*).

**O que acontece quando chamo o `setState`?**
> O React agenda uma nova renderização: chama a função do componente de novo com o valor novo, compara o JSX novo com o anterior (reconciliação) e atualiza na página só o que mudou. Nunca altere o estado direto, como `busca = "x"`: só o `set` avisa o React.

**O que é estado derivado? Quando NÃO usar `useState`?**
> É o valor que dá para calcular a partir de outro estado ou das props, como a lista filtrada a partir do texto da busca. Calcule no corpo do componente, em vez de guardar num segundo `useState`: dois estados para a mesma informação acabam ficando diferentes.

**O que é um componente controlado?**
> Um campo de formulário cujo valor vem do estado do React (`value={valor}`) e é atualizado pelo `onChange`. O React é a "fonte da verdade", e não o navegador. No não controlado, o valor fica no próprio campo, e se lê com uma `ref` quando precisa.

**O que é "subir o estado" (*lifting state up*)?**
> Quando dois componentes irmãos precisam do mesmo estado, ele fica no ancestral comum mais próximo e desce para os dois por props. Na busca do Temple, o texto fica no `App`, que entrega o valor ao campo e usa o mesmo valor para filtrar a lista.

**Para que serve o `useEffect`?**
> Para sincronizar o componente com algo **fora do React**: o título da aba, um timer, uma assinatura de eventos, uma chamada de API. Ele roda depois que a tela é desenhada.

**Como funciona o array de dependências do `useEffect`?**
> `[]` roda só na montagem. `[a, b]` roda na montagem e sempre que `a` ou `b` mudar. Sem o array, roda depois de toda renderização. Tudo o que o efeito lê do componente deve estar no array; se faltar, o efeito usa um valor velho (*stale closure*).

**O que é a função de limpeza (*cleanup*) do `useEffect`, e por que importa?**
> É a função devolvida pelo efeito. Ela roda antes do próximo efeito e quando o componente sai da tela. Serve para desligar o que o efeito criou: `clearInterval`, `removeEventListener`, cancelar uma requisição (`AbortController`). Sem ela, há vazamento de memória e atualizações em componentes que já sumiram.

**Quando NÃO usar `useEffect`?**
> Para calcular valores a partir do estado ou das props: calcule direto no corpo do componente. Um efeito que só faz `setAlgo(...)` a partir de outro estado causa uma renderização a mais e um estado duplicado.

**Quais são as regras dos Hooks?**
> Só chamar Hooks no topo do componente (nunca dentro de `if`, de laço ou de função interna) e só dentro de componentes ou de outros Hooks. O React identifica cada Hook pela **ordem** em que é chamado; se um `if` pular um deles, os estados se misturam. O linter (`rules-of-hooks`) confere isso.

**O que é a inicialização preguiçosa do `useState`?**
> Passar uma função, como em `useState(() => new Date())`, em vez do valor. A função só roda na primeira renderização; com o valor direto, a expressão seria calculada (e descartada) em toda renderização. Vale a pena quando o valor inicial é caro de calcular.

**Como fazer renderização condicional no JSX?**
> Com o ternário (`{cond ? <A /> : <B />}`) quando há um "senão", e com o `&&` (`{cond && <A />}`) quando é só "mostra ou não mostra". Cuidado: `{lista.length && <A />}` desenha um `0` na tela quando a lista está vazia; use uma condição booleana, como `lista.length > 0`.

**Export nomeado × export default?**
> No nomeado (`export function Cartao`), quem importa usa o mesmo nome (`import { Cartao }`): o editor renomeia em todo lugar e o autocomplete acha. No default, quem importa escolhe qualquer nome, o que facilita erros. Muitos times preferem o nomeado.

**Por que `import type`?**
> Para importar algo que é só tipo (como o `ReactNode`). A linha some inteira no build. Com o `verbatimModuleSyntax` ligado, o TypeScript exige o `import type` nesses casos.

---

## Git

**`git rm` × `git rm --cached`?**
> O `git rm` tira o arquivo do controle do Git **e** apaga do disco. O `git rm --cached` só para de rastrear o arquivo e o mantém no disco: é o comando para quando alguém commitou sem querer um arquivo que devia estar no `.gitignore`, como um `.env` ou a `node_modules`.

**Por que usar `git mv` em vez de mover a pasta pelo Windows?**
> O `git mv` já registra a mudança como renomeação, e o histórico do arquivo continua (`git log --follow`). Movendo por fora, o Git até costuma detectar a renomeação depois, mas o `git mv` deixa isso explícito.

**Por que a `node_modules` e a `dist` ficam no `.gitignore`?**
> Porque são **geradas**: a `node_modules` sai do `npm install` (ou `npm ci`), e a `dist` sai do `npm run build`. O que vai para o Git é a receita (`package.json` e `package-lock.json`), não o resultado.
