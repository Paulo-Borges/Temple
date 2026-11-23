# ⛪ Temple: Sistema de Gestão de Igrejas

Sistema web para a gestão de igrejas: **matriz e filiais, membros, cargos eclesiásticos, perfis de acesso, aniversariantes, certificados e financeiro**.

O projeto é construído **passo a passo, como um curso**, com foco em boas práticas de **C#/.NET** e **React/TypeScript**: arquitetura limpa, tipagem forte, validação em camadas e segurança desde o início.

> 🚧 **Em desenvolvimento.** O **backend** (API REST) está funcional, com cadastro de igrejas, membros e usuários, login com JWT, permissões por perfil, **escopo de acesso por igreja e por grupo** e aniversariantes do mês. O **frontend** em React começa no Módulo 6 e, por enquanto, ainda é o template inicial do Vite. Veja o [Roteiro](#-roteiro).

---

## 📑 Sumário

- [Funcionalidades](#-funcionalidades)
- [Tecnologias](#-tecnologias)
- [Arquitetura](#-arquitetura)
- [Estrutura de pastas](#-estrutura-de-pastas)
- [Endpoints da API](#-endpoints-da-api)
- [Como executar](#-como-executar)
- [Boas práticas adotadas](#-boas-práticas-adotadas)
- [Roteiro](#-roteiro)

---

## ✨ Funcionalidades

### ✅ Prontas (backend)

**Igrejas**
- Cadastro de **igreja matriz** e de **filiais** vinculadas a ela. São só 2 níveis: uma filial não tem filiais.
- CNPJ com validação, guardado sem máscara e já aceitando o **formato alfanumérico** (em vigor desde jul/2026).
- Edição de nome, telefone e endereço.
- Ativação e desativação, com regras:
  - uma matriz com filiais ativas não pode ser desativada;
  - uma filial não pode ser ativada se a matriz estiver inativa.

**Membros**
- Cadastro com matrícula numérica única, gerada pelo banco.
- Listagem **paginada**, com filtros por igreja, nome (busca parcial), cargo e situação.
- Edição de nome, endereço e grupo.
- Ativação e desativação.
- **Promoção de cargo** com **histórico** gravado. Os cargos, em ordem de hierarquia, são Membro, Cooperador, Diácono, Presbítero, Evangelista, Missionário e Pastor. As regras:
  - subir pode pular degraus (de Membro direto para Diácono, por exemplo);
  - descer de cargo (**rebaixamento**) é permitido, mas exige o motivo na observação;
  - a data não pode estar no futuro nem ser anterior ao nascimento do membro ou à última promoção, para o histórico ficar sempre em ordem;
  - membro desativado não pode ser promovido.
- O histórico funciona como **trilha de auditoria**: cada registro guarda quem fez a promoção (id e uma cópia do nome), a observação opcional, se foi um rebaixamento e o momento em que foi registrado. Os registros anteriores a essa auditoria aparecem sem autor.

**Aniversariantes**
- Lista dos **aniversariantes do mês**: por padrão o mês atual, ou outro mês informado (`?mes=1` a `12`), com filtro opcional por igreja.
- Entram só os **membros ativos**, em ordem de dia e nome.
- Cada item traz o dia, a **idade que a pessoa completa no ano** e o nome da igreja. O endereço não aparece.
- Respeita o [escopo de acesso](#-funcionalidades): a matriz vê as filiais, e o líder de grupo vê só o próprio grupo.
- O mês atual vem de um relógio injetado (`TimeProvider`), que pode ser trocado nos testes.

**Autenticação**
- Login com e-mail e senha (ASP.NET Identity), devolvendo um **token JWT** que vale 8 horas.
- **Todas as rotas exigem login**, exceto a própria rota de login.
- A conta é **bloqueada por 15 minutos** após 5 senhas erradas.
- Em desenvolvimento, um usuário **Pastor** é criado automaticamente.

**Perfis, permissões e usuários**
- Cada usuário tem **um ou mais perfis**, gravados como papéis do Identity e enviados no token (claim `role`).
- Autorização **por permissão**: cada rota exige uma permissão (`Membros.Ver`, `Membros.Promover`…), e um único arquivo diz quais perfis têm cada uma.
- O **Pastor** cadastra usuários, lista, consulta e troca os perfis deles.
- O sistema nunca fica sem Pastor: tirar o perfil do último Pastor é recusado.
- Erros do Identity em português: e-mail repetido (409) e senha fraca (400).
- A troca de perfis vale a partir do **próximo login** do usuário (o token vale 8 horas).
- Cada usuário é **ligado ao seu cadastro de membro**. Um membro só pode ter um usuário, e precisa estar ativo para ser vinculado. O Pastor pode trocar o vínculo depois.
- Qualquer usuário logado consulta o **próprio cadastro** em `GET /api/membros/meu-cadastro`.

**Escopo de acesso**
- A permissão diz **se** o perfil pode usar a rota; o escopo diz **quais dados** ele alcança:

| Quem | Alcança |
|---|---|
| Pastor sem membro vinculado (administração geral) | Tudo |
| Pastor, Secretário e Tesoureiro de uma **matriz** | A matriz e as filiais dela |
| Pastor, Secretário e Tesoureiro de uma **filial** | Só a própria filial |
| Líder (genérico) | Os membros da própria igreja |
| Líder dos Varões, das Irmãs, dos Jovens ou das Crianças | Só os membros do próprio grupo, na própria igreja |
| Demais perfis | Só o próprio cadastro |

- O que está fora do alcance responde **404**, igual ao que não existe. Assim, ninguém descobre dados de outras igrejas.
- Só a administração geral cadastra uma **nova matriz** (os demais recebem **403**).
- As listagens são filtradas **no banco**, então a paginação e o total continuam corretos.
- O membro desativado **perde o acesso**: o login é recusado com a mesma mensagem de senha errada.
- O escopo é calculado a partir dos dados do banco a cada requisição, e não do token.

### 🔜 Planejadas
- **Certificados em PDF**: Batismo, Apresentação de Criança e Promoção de Cargo.
- **Financeiro**: dízimos, ofertas (que podem ser anônimas), outras entradas, despesas e relatórios consolidados da matriz com as filiais.
- Interface web em **React**.

### 🔐 Matriz de permissões (aprovada)

| Perfil | Igrejas | Membros | Certificados | Aniversariantes | Financeiro |
|---|---|---|---|---|---|
| Pastor (administrador) | Total | Total | Total | ✔ | Total |
| Secretário | Ver | Criar e editar | Emitir | ✔ | — |
| Tesoureiro | Ver | Ver | — | ✔ | Criar, editar e relatórios |
| Líderes (Varões, Irmãs, Jovens, Crianças) | — | Ver o próprio grupo | — | ✔ do grupo | — |
| Funcionário / Cozinheira | — | Ver o próprio perfil | — | — | — |

> Esta é a matriz completa, o objetivo do projeto. **Já em vigor:** as permissões de igrejas, membros e usuários (veja a coluna *Permissão* em [Endpoints da API](#-endpoints-da-api)) e o [escopo de acesso](#-funcionalidades): os Líderes veem os membros e os aniversariantes do próprio grupo, e o Funcionário e a Cozinheira veem o próprio cadastro. Os aniversariantes já estão em vigor para todos os perfis da coluna. Certificados e financeiro chegam nos Módulos 5 e 5B.

---

## 🧰 Tecnologias

### Backend (em uso)
| Tecnologia | Uso |
|---|---|
| **C# 14 + .NET 10** (LTS) | ASP.NET Core Web API com Controllers |
| **Entity Framework Core 10** | ORM, migrations e seed |
| **SQL Server** | Banco de dados (LocalDB em desenvolvimento) |
| **ASP.NET Core Identity** | Usuários, hash de senha e bloqueio de conta |
| **JWT Bearer** | Autenticação stateless, com token assinado em HMAC-SHA256 |
| **FluentValidation** | Validação das requisições, com mensagens em português |
| **OpenAPI + Swagger UI** | Documentação interativa da API (só em desenvolvimento) |
| **xUnit** | Projeto de testes criado; os testes chegam no Módulo 9 |

### Frontend
| Tecnologia | Situação |
|---|---|
| **React 19 + TypeScript 7 + Vite 8** | ✅ Projeto criado (template inicial), com TypeScript em modo `strict` |
| **Oxlint** | ✅ Lint configurado |
| Tailwind CSS | 🔜 Módulo 6 |
| React Router | 🔜 Módulo 6 |
| TanStack Query + Axios tipado | 🔜 Módulo 7 |
| React Hook Form + Zod | 🔜 Módulo 7 |
| Vitest | 🔜 Módulo 9 |

### Planejadas no backend
- **QuestPDF**, para gerar os certificados e comprovantes em PDF.

---

## 🧱 Arquitetura

O backend segue a **Clean Architecture**: as dependências apontam sempre **para dentro**, e o domínio não conhece banco, HTTP ou framework.

```
            ┌──────────────────────────────────────────┐
            │  Temple.Api                              │  Controllers, JWT, Swagger,
            │  (ASP.NET Core)                          │  tratamento global de erros
            └───────┬──────────────────────┬───────────┘
                    │                      │
                    ▼                      ▼
┌──────────────────────────┐    ┌──────────────────────────┐
│  Temple.Infrastructure   │───▶│  Temple.Application      │  Services, DTOs, validadores
│  EF Core, Identity, JWT, │    │  e contratos (interfaces)│
│  repositórios, migrations│    └────────────┬─────────────┘
└──────────────────────────┘                 │
                                             ▼
                                ┌──────────────────────────┐
                                │  Temple.Domain           │  Entidades, value objects,
                                │  (sem dependências)      │  enums e regras de negócio
                                └──────────────────────────┘
```

| Camada | Responsabilidade |
|---|---|
| **Domain** | Entidades (`Igreja`, `Membro`, `HistoricoCargo`), o value object `Endereco`, os enums e as **regras de negócio**. Nada é alterado "por fora": tudo passa por métodos que validam. |
| **Application** | Casos de uso (services), DTOs (`record`), validadores e as **interfaces** que a Infrastructure implementa: repositórios, Unit of Work, identidade, usuários e token. |
| **Infrastructure** | Implementações técnicas: `TempleDbContext`, mapeamentos do EF, repositórios, ASP.NET Identity (usuários, perfis e mensagens em português), geração do JWT e seeds. |
| **Api** | Entrada HTTP: controllers, autenticação, políticas de autorização por permissão, `ProblemDetails` e Swagger. |

**Tradução de erros para HTTP** (handler global com `ProblemDetails`):

| Exceção | HTTP |
|---|---|
| Validação (`ValidationException`) | **400**, com os erros agrupados por campo |
| Login inválido (`NaoAutorizadoException`) | **401** |
| Ação que exige acesso total, como cadastrar uma matriz (`AcessoNegadoException`) | **403** |
| Recurso inexistente ou fora do escopo (`NaoEncontradoException`) | **404** |
| Duplicidade, como um CNPJ repetido (`ConflitoException`) | **409** |
| Regra de negócio (`DomainException`) | **422** |

Sem token, ou com um token inválido ou vencido, a resposta é **401**. Com um token válido, mas sem a permissão da rota, é **403**.

---

## 📁 Estrutura de pastas

```
Temple/
├── backend/
│   ├── Temple.slnx                  # solution (formato .slnx)
│   ├── global.json                  # fixa o SDK do .NET 10
│   ├── Directory.Build.props        # avisos tratados como erro, análise de código
│   ├── Temple.Domain/               # Entities, ValueObjects, Enums, Exceptions
│   ├── Temple.Application/          # Igrejas, Membros, Usuarios, Autenticacao, Comum
│   ├── Temple.Infrastructure/       # Persistence (DbContext, Configurations,
│   │                                #   Migrations, Repositorios, Seed), Identidade
│   ├── Temple.Api/                  # Controllers, Autorizacao, Configuracao, Tratamento,
│   │                                #   Program.cs, Temple.Api.http
│   └── Temple.Tests/                # xUnit
├── src/                             # frontend React (template inicial do Vite)
├── public/
├── index.html
├── package.json
├── PLANO-DE-ESTUDO.md               # roteiro das aulas e diário do projeto
└── README.md
```

---

## 🌐 Endpoints da API

Todas as rotas, **exceto o login**, exigem o cabeçalho `Authorization: Bearer <token>`. A coluna **Permissão** mostra quais perfis podem usar cada rota. Os dados que cada um vê e altera ainda passam pelo [escopo de acesso](#-funcionalidades).

### Autenticação
| Método | Rota | Descrição | Permissão |
|---|---|---|---|
| `POST` | `/api/autenticacao/login` | Login com e-mail e senha; devolve `{ token, expiraEm }` | Pública |
| `GET` | `/api/autenticacao/me` | Dados e perfis do usuário logado | Qualquer usuário logado |

### Igrejas
| Método | Rota | Descrição | Permissão |
|---|---|---|---|
| `POST` | `/api/igrejas` | Cadastra uma matriz | Pastor com acesso total |
| `POST` | `/api/igrejas/{matrizId}/filiais` | Cadastra uma filial da matriz | Pastor |
| `GET` | `/api/igrejas?ativa=true` | Lista as igrejas (o filtro `ativa` é opcional) | Pastor, Secretário, Tesoureiro |
| `GET` | `/api/igrejas/{id}` | Busca uma igreja | Pastor, Secretário, Tesoureiro |
| `GET` | `/api/igrejas/{id}/filiais` | Lista as filiais de uma matriz | Pastor, Secretário, Tesoureiro |
| `PUT` | `/api/igrejas/{id}` | Edita nome, telefone e endereço | Pastor |
| `POST` | `/api/igrejas/{id}/ativar` · `/desativar` | Ativa ou desativa | Pastor |

### Membros
| Método | Rota | Descrição | Permissão |
|---|---|---|---|
| `POST` | `/api/membros` | Cadastra um membro | Pastor, Secretário |
| `GET` | `/api/membros?igrejaId=&nome=&cargo=&ativo=&pagina=1&tamanhoPagina=20` | Lista paginada com filtros (máximo de 100 por página) | Pastor, Secretário, Tesoureiro, Líderes |
| `GET` | `/api/membros/meu-cadastro` | Cadastro de membro do usuário logado | Qualquer usuário logado |
| `GET` | `/api/membros/{id}` | Busca um membro | Pastor, Secretário, Tesoureiro, Líderes |
| `PUT` | `/api/membros/{id}` | Edita nome, endereço e grupo | Pastor, Secretário |
| `POST` | `/api/membros/{id}/ativar` · `/desativar` | Ativa ou desativa | Pastor, Secretário |
| `POST` | `/api/membros/{id}/promocoes` | Promove ou rebaixa de cargo (`novoCargo`, `dataPromocao` e `observacao`, obrigatória no rebaixamento) e grava o histórico com o autor | Pastor |
| `GET` | `/api/membros/{id}/historico-cargos` | Histórico de cargos, em ordem de data e de registro, com autor, observação e rebaixamento | Pastor, Secretário, Tesoureiro, Líderes |

### Aniversariantes
| Método | Rota | Descrição | Permissão |
|---|---|---|---|
| `GET` | `/api/membros/aniversariantes?mes=&igrejaId=` | Membros ativos que fazem aniversário no mês (sem `mes`, o mês atual), por dia e nome | Pastor, Secretário, Tesoureiro, Líderes |

### Usuários
| Método | Rota | Descrição | Permissão |
|---|---|---|---|
| `POST` | `/api/usuarios` | Cadastra um usuário com nome, e-mail, senha inicial, membro e perfis | Pastor |
| `GET` | `/api/usuarios` | Lista os usuários, seus perfis e o membro vinculado | Pastor |
| `GET` | `/api/usuarios/{id}` | Busca um usuário | Pastor |
| `PUT` | `/api/usuarios/{id}/perfis` | Troca os perfis (vale a partir do próximo login do usuário) | Pastor |
| `PUT` | `/api/usuarios/{id}/membro` | Vincula o usuário a outro membro | Pastor |

> 💡 Com a API rodando em desenvolvimento, a documentação interativa fica em **`/swagger`**. O arquivo [`backend/Temple.Api/Temple.Api.http`](backend/Temple.Api/Temple.Api.http) tem exemplos prontos de todas as rotas, para a extensão **REST Client** do VS Code.

---

## 🚀 Como executar

### Pré-requisitos
- [.NET 10 SDK](https://dotnet.microsoft.com/download) (a versão fica fixada no `backend/global.json`)
- SQL Server. Em desenvolvimento usamos o **LocalDB** (`(localdb)\MSSQLLocalDB`), que vem com o Visual Studio ou com o SQL Server Express.
- Ferramenta do EF Core: `dotnet tool install --global dotnet-ef`
- [Node.js](https://nodejs.org/) LTS, só para o frontend

### 1. Configure os segredos (fora do Git)
A chave do JWT e a senha do Pastor inicial **não ficam no repositório**: elas vão para o *user-secrets* do .NET. Na pasta `backend`:

```bash
# Chave do JWT: no mínimo 32 bytes aleatórios
dotnet user-secrets set "Jwt:Chave" "$(openssl rand -base64 48)" --project Temple.Api

# Senha do Pastor inicial: 8 caracteres ou mais, com maiúscula, minúscula, número e símbolo
dotnet user-secrets set "PastorInicial:Senha" '<defina-uma-senha-forte>' --project Temple.Api
```

> ⚠️ **Troque `<defina-uma-senha-forte>` por uma senha sua.** O marcador não funciona de propósito: ele não tem maiúscula nem número, e a API se recusa a subir até você definir uma senha forte. Use aspas **simples**, para que símbolos como `$` e `!` não sejam interpretados pelo terminal.

<details>
<summary>PowerShell (gerar a chave sem o openssl)</summary>

```powershell
$bytes = New-Object byte[] 48
[System.Security.Cryptography.RandomNumberGenerator]::Create().GetBytes($bytes)
dotnet user-secrets set "Jwt:Chave" ([Convert]::ToBase64String($bytes)) --project Temple.Api
dotnet user-secrets set "PastorInicial:Senha" '<defina-uma-senha-forte>' --project Temple.Api
```
</details>

### 2. Crie o banco
```bash
cd backend
dotnet ef database update --project Temple.Infrastructure --startup-project Temple.Api
```
Em desenvolvimento, esse comando também cria uma **igreja matriz de exemplo**.

### 3. Rode a API
```bash
dotnet run --project Temple.Api
```
- API: `http://localhost:5022`
- Swagger: `http://localhost:5022/swagger`
- Login inicial: `pastor@temple.local`, com a senha definida no passo 1. O usuário é criado ao subir a API.

No Swagger, faça o login em `POST /api/autenticacao/login`, copie o `token`, clique em **🔒 Authorize** e cole o token. Os demais usuários são cadastrados pelo Pastor, em `POST /api/usuarios`, e cada um precisa ser ligado a um membro ativo já cadastrado.

### 4. Frontend (template inicial)
Na raiz do repositório:
```bash
npm install
npm run dev
```

---

## 🧭 Boas práticas adotadas

**C# / .NET**
- **Tipagem forte, sem atalhos**: nada de `dynamic`, `object`, `!` (*null-forgiving*), `?.`, `??` ou casts de contorno. Os valores nulos são tratados com *pattern matching* (`is not string email`).
- **Avisos tratados como erros** (`TreatWarningsAsErrors`) e análise de estilo no build (`EnforceCodeStyleInBuild`).
- **Entidades encapsuladas**: `private set`, construtores que validam e métodos de negócio (`PromoverCargo`, `Desativar`…). Um construtor separado é usado só pelo EF Core.
- **DTOs com `record`**: as entidades nunca saem pela API.
- **Validação em duas camadas**: FluentValidation na entrada (400) e regras de domínio na entidade (422).
- **Repositórios + Unit of Work**: `AsNoTracking` nas listagens, `Include` só onde a regra precisa, e paginação feita no banco (`Skip`/`Take`, com ordem estável).
- **Erros padronizados** com `ProblemDetails` (RFC 9457) e um handler global.
- **Segurança**: senhas com hash (Identity), bloqueio após tentativas erradas, JWT com emissor, audiência, algoritmo e validade conferidos, rotas **fechadas por padrão** (*fallback policy*), **autorização por permissão** (*policies* que concentram a matriz perfil → permissão num lugar só), **escopo de acesso** aplicado na consulta ao banco (fora do alcance é 404, para não revelar dados de outras igrejas) e segredos no *user-secrets*.
- **Migrations revisadas à mão** antes de aplicar. Por exemplo, a coluna `Ativo` entrou com valor padrão `true`, para não desativar os membros que já existiam, e as colunas de auditoria do histórico entraram anuláveis, para não inventar um autor para os registros antigos.
- **Relógio injetado** (`TimeProvider`) no lugar de `DateTime.Now` nas regras novas: o domínio recebe a data e a hora de quem chama, e os testes podem simular qualquer dia.

**React / TypeScript** *(a partir do Módulo 6)*
- TypeScript em modo `strict`, com tipos que espelham os DTOs do C#.
- Componentes funcionais com props tipadas.
- Dados do servidor com TanStack Query.
- Formulários com React Hook Form, validados por esquemas Zod.
- Rotas protegidas por perfil.

**Commits** no padrão [Conventional Commits](https://www.conventionalcommits.org/pt-br/) (`feat:`, `chore:`…).

---

## 📅 Roteiro

| Módulo | Tema | Situação |
|---|---|:---:|
| 0 | Ambiente e Git | ✅ |
| 1 | Fundamentos do backend: solution, Domain, entidades e value objects | ✅ |
| 2 | Persistência: EF Core, relacionamentos, migrations e seed | ✅ |
| 3 | Application e API: DTOs, services, validação, controllers, CRUD de igrejas e membros | ✅ |
| 4 | Segurança: Identity + JWT, perfis e policies, vínculo usuário ↔ membro e escopo por igreja | ✅ |
| 5 | Regras de negócio: aniversariantes ✅ · promoções ✅ · certificados em PDF ⬜ | 🔄 |
| 5B | Financeiro: dízimos, ofertas, despesas e relatórios | ⬜ |
| 6 | Fundamentos do frontend: React, TypeScript, Tailwind e React Router | ⬜ |
| 7 | Integração front + API: Axios, TanStack Query, login e rotas protegidas | ⬜ |
| 8 | Funcionalidades finais: aniversariantes, certificados e financeiro na tela | ⬜ |
| 9 | Qualidade: testes (xUnit e Vitest) e revisão | ⬜ |

O passo a passo de cada aula, as decisões tomadas e o diário do projeto estão no [PLANO-DE-ESTUDO.md](PLANO-DE-ESTUDO.md).
