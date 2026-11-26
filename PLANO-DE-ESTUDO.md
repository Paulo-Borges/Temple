# ⛪ Temple: Plano de Estudo (React + C#/.NET)

> Sistema de gestão de igreja construído passo a passo, linha a linha, como projeto de estudo.

**Legenda de status:** ⬜ Não iniciada · 🔄 Em andamento · ✅ Concluída

---

## 📊 Progresso geral

| Módulo | Tema | Aulas | Concluídas | Status |
|---|---|:---:|:---:|:---:|
| 0 | Ambiente | 2 | 2 | ✅ |
| 1 | Fundamentos do Backend | 4 | 4 | ✅ |
| 2 | Persistência (EF Core + SQL Server) | 3 | 3 | ✅ |
| 3 | Application e API | 5 | 5 | ✅ |
| 4 | Segurança | 4 | 4 | ✅ |
| 5 | Regras de Negócio | 4 | 4 | ✅ |
| 5B | Financeiro | 7 | 5 | 🔄 |
| 6 | Fundamentos do Frontend | 4 | 0 | ⬜ |
| 7 | Integração Front + API | 4 | 0 | ⬜ |
| 8 | Funcionalidades Finais | 4 | 0 | ⬜ |
| 9 | Qualidade | 2 | 0 | ⬜ |
| **Total** | | **43** | **27** | **63%** |

---

## 🧱 Stack definida

| Camada | Tecnologia |
|---|---|
| Backend | C# + .NET 10 (LTS), ASP.NET Core Web API |
| Arquitetura | Clean Architecture (Domain, Application, Infrastructure, Api, Tests) |
| Banco de dados | SQL Server + Entity Framework Core **10.0.x** (não usar o 11, que exige .NET 11) (dev: LocalDB `(localdb)\MSSQLLocalDB`) |
| Autenticação | ASP.NET Identity + JWT + Roles/Policies |
| Validação | FluentValidation |
| PDF | QuestPDF |
| Frontend | React 19 + TypeScript (strict) + Vite |
| Estilo | Tailwind CSS |
| Dados no front | TanStack Query + Axios tipado |
| Formulários | React Hook Form + Zod |
| Testes | xUnit (C#) · Vitest (React) |

---

## 📁 Estrutura de pastas

```
Temple/
├── backend/
│   ├── Temple.Domain/
│   ├── Temple.Application/
│   ├── Temple.Infrastructure/
│   ├── Temple.Api/
│   └── Temple.Tests/
└── frontend/
    └── temple-web/
```

---

## 🔐 Matriz de permissões (aprovada)

| Perfil | Igrejas | Membros | Certificados | Aniversariantes | Financeiro |
|---|---|---|---|---|---|
| Pastor (Admin) | Total | Total | Total | ✔ | Total |
| Secretário | Ver | Criar/Editar | Emitir | ✔ | — |
| Tesoureiro | Ver | Ver | — | ✔ | Criar/Editar/Relatórios |
| Líderes (Varões/Irmãs/Crianças) | — | Ver o próprio grupo | — | ✔ do grupo | — |
| Funcionário / Cozinheira | — | Ver o próprio perfil | — | — | — |

---

## 📌 Decisões de domínio

- **Igreja:** a matriz tem filiais vinculadas (auto-relacionamento por `IgrejaMatrizId`).
  - Campos: Nome, CNPJ, Telefone e Data de fundação (todos obrigatórios), Ativa (informada no cadastro) e Endereço (entra na Aula 1.3).
  - Só 2 níveis: uma filial **não** pode ter filiais.
  - CNPJ guardado sem máscara, aceitando o formato alfanumérico (em vigor desde jul/2026).
- **Membro:** nome, endereço, matrícula, data de nascimento (`DateOnly`).
  - Matrícula: `int` gerado pelo banco, **única no sistema inteiro** (não reinicia por igreja).
  - Pertence obrigatoriamente a uma igreja (`Guid`), e **não pode ser cadastrado em igreja inativa**.
- **Endereco (value object):** Logradouro, Número (`string`, aceita "S/N"), Bairro, Cidade, UF e CEP obrigatórios; Complemento opcional. Obrigatório na Igreja e no Membro.
- **Cargos e perfis (dois enums separados):**
  - `TipoCargo` (eclesiástico): Membro, Cooperador, Diácono, Presbítero, Evangelista, Missionário e Pastor. **Um por vez**, informado no cadastro, e só muda por `PromoverCargo`, que grava o `HistoricoCargo`.
  - `Perfil` (acesso ao sistema): Pastor, Tesoureiro, Secretário, Líder, Líder dos Varões, das Irmãs, dos Jovens e das Crianças, Funcionário e Cozinheira. **Vários por usuário**, ligado ao login no Módulo 4.
  - `GrupoMembro`: Varões, Irmãs, Jovens e Crianças. **Opcional** no membro (`GrupoMembro?`).
- **Certificados:** Batismo, Apresentação de Criança e Promoção de cargo (membros e funcionários).
- **Financeiro:** entradas e saídas (dízimos, ofertas, outras entradas e despesas).
  - Dízimo: membro **obrigatório** (`Guid`).
  - Oferta: membro **opcional**, pode ser anônima (`Guid?`).
  - Valores sempre em `decimal`.
  - `FormaPagamento`: Dinheiro, Pix, Cartão, Transferência.

---

## 📚 Aulas

### Módulo 0: Ambiente
| # | Aula | Status |
|---|---|:---:|
| 0.1 | Instalação: .NET 10 SDK, Node LTS, SQL Server, extensões do VS Code | ✅ |
| 0.2 | Git e `.gitignore` | ✅ |

### Módulo 1: Fundamentos do Backend
| # | Aula | Status |
|---|---|:---:|
| 1.1 | Solution e projetos com o `dotnet` CLI e referências entre camadas | ✅ |
| 1.2 | Domain: entidade `Igreja` (classes, propriedades, encapsulamento) | ✅ |
| 1.3 | Domain: `Membro`, value object `Endereco`, `DateOnly` | ✅ |
| 1.4 | Domain: enums `TipoCargo`, `TipoCertificado` e `HistoricoCargo` | ✅ |

### Módulo 2: Persistência
| # | Aula | Status |
|---|---|:---:|
| 2.1 | `DbContext` e `IEntityTypeConfiguration<T>` | ✅ |
| 2.2 | Relacionamento matriz ↔ filiais | ✅ |
| 2.3 | Migrations e seed (matriz de exemplo só em dev; cargos viraram enums e o Pastor vai para o Módulo 4) | ✅ |

### Módulo 3: Application e API
| # | Aula | Status |
|---|---|:---:|
| 3.1 | DTOs com `record` e por que não expor a entidade | ✅ |
| 3.2 | Services, injeção de dependência e FluentValidation | ✅ |
| 3.3 | Controllers REST, Swagger/OpenAPI e `ProblemDetails` | ✅ |
| 3.4A | CRUD de Igrejas: listar, editar, ativar e desativar | ✅ |
| 3.4B | CRUD de Membros | ✅ |

### Módulo 4: Segurança
| # | Aula | Status |
|---|---|:---:|
| 4.1 | ASP.NET Identity e login com JWT | ✅ |
| 4.2 | Roles e Policies por perfil | ✅ |
| 4.3A | Vínculo entre usuário e membro (`meu-cadastro`) | ✅ |
| 4.3B | Escopo por igreja (a matriz vê as filiais, a filial só a própria, o líder só o grupo) | ✅ |

### Módulo 5: Regras de Negócio
| # | Aula | Status |
|---|---|:---:|
| 5.1 | Aniversariantes do mês (LINQ + `DateOnly`) | ✅ |
| 5.2 | Promoção de cargo com histórico | ✅ |
| 5.3A | Certificados em PDF com QuestPDF: promoção de cargo, registro da emissão e 2ª via | ✅ |
| 5.3B | Registros de Batismo e Apresentação de Criança e os certificados deles | ✅ |

### Módulo 5B: Financeiro
| # | Aula | Status |
|---|---|:---:|
| 5B.1 | Domain: movimentação financeira (entrada/saída), `decimal` e enum `FormaPagamento` | ✅ |
| 5B.2 | Domain: Dízimo (membro obrigatório), Oferta (pode ser anônima), Despesa | ✅ |
| 5B.3 | EF Core: `decimal(18,2)` e índices por data e igreja | ✅ |
| 5B.4A | API: criação dos 4 tipos, consulta por id e estorno, com as permissões de Tesoureiro/Pastor | ✅ |
| 5B.4B | API: consulta paginada dos lançamentos com filtros | ✅ |
| 5B.5 | Relatórios: saldo mensal, por igreja e consolidado matriz + filiais | ⬜ |
| 5B.6 | Comprovante de dízimo em PDF | ⬜ |

### Módulo 6: Fundamentos do Frontend
| # | Aula | Status |
|---|---|:---:|
| 6.1 | Vite + React + TypeScript strict + Tailwind CSS | ⬜ |
| 6.2 | Componentes, props tipadas, `useState`, `useEffect` | ⬜ |
| 6.3 | Tipos TypeScript espelhando os DTOs do C# | ⬜ |
| 6.4 | React Router: layout e rotas | ⬜ |

### Módulo 7: Integração Front + API
| # | Aula | Status |
|---|---|:---:|
| 7.1 | Cliente Axios tipado e TanStack Query | ⬜ |
| 7.2 | Login, token e `AuthContext` | ⬜ |
| 7.3 | Rotas protegidas por perfil | ⬜ |
| 7.4 | Telas de Igrejas e Membros (React Hook Form + Zod) | ⬜ |

### Módulo 8: Funcionalidades Finais
| # | Aula | Status |
|---|---|:---:|
| 8.1 | Painel de aniversariantes do mês | ⬜ |
| 8.2 | Emissão, download e impressão de certificados | ⬜ |
| 8.3 | Histórico de promoções | ⬜ |
| 8.4 | Financeiro: lançamentos de entradas/saídas e relatórios | ⬜ |

### Módulo 9: Qualidade
| # | Aula | Status |
|---|---|:---:|
| 9.1 | Testes: xUnit (backend) e Vitest (frontend) | ⬜ |
| 9.2 | Revisão de boas práticas e próximos passos | ⬜ |

---

## 📝 Diário de aulas

| Data | Aula | O que foi feito | Observações |
|---|---|---|---|
| 2026-11-12 | Planejamento | Stack, domínio, permissões e roteiro definidos | — |
| 2026-11-12 | 0.1 | Ambiente verificado: .NET SDK 10.0.401, Node 26.10.0, npm 11.19.1, Git 2.42, SQL Server 2025 LocalDB, extensões do VS Code | Docker instalado, mas não estava rodando |
| 2026-11-12 | 0.2 | Aula de Git e `.gitignore`. Primeiro commit (`7b1a5f9`) feito pelo usuário na branch `main`, com remoto `origin` | `.gitignore` ajustado para .NET: removido `*.sln`; incluídos `bin/`, `obj/`, `*.user`, `*.rsuser`, `.vs/`, `TestResults/`, `*.trx` |
| 2026-11-12 | 1.1 | Backend criado: `global.json` (SDK 10.0.401), `Temple.slnx`, 5 projetos, referências entre as camadas e `Directory.Build.props` (avisos tratados como erros). Build com 0 avisos e 0 erros | Erro no Passo 5 (faltava o `Temple.slnx`), resolvido rodando o Passo 3 |
| 2026-11-13 | 1.2 | Entidade `Igreja` em `Temple.Domain/Entities`: `private set`, construtor privado com validações, `CriarMatriz` (estático) e `CriarFilial` (só a partir da matriz), `Ativar`/`Desativar`. Build com 0 avisos e 0 erros | Arquivo criado primeiro em `backend/Domain/`, fora do projeto, e por isso não compilava: movido. Removido o parâmetro `Igreja matriz` de `CriarFilial`, que sobrava porque a matriz já é o `this`. Pendente: dígitos verificadores do CNPJ (value object) e `TimeProvider` (Módulo 9) |
| 2026-11-13 | 1.3 | Value object `Endereco` (`sealed record`, imutável, só `get`, validação de UF e CEP). Entidade `Membro` (matrícula `int` gerada pelo banco, `IgrejaId` obrigatório, `CalcularIdade`, `AlterarNome`, `AlterarEndereco`). `Endereco` adicionado à `Igreja`. Build com 0 avisos e 0 erros | Regra nova: não cadastrar membro em igreja inativa. Removida uma linha `Ativa = ativa` duplicada no construtor da `Igreja`. Para a Aula 2.1: matrícula única no sistema inteiro (índice único). Em aberto: o que acontece com os membros quando a igreja é desativada (Módulo 5) |
| 2026-11-15 | 1.4 | Enums `TipoCargo`, `Perfil`, `GrupoMembro` e `TipoCertificado` (valores explícitos começando em 1). Entidade `HistoricoCargo` com construtor `internal`. `Membro` ganhou `CargoAtual`, `Grupo`, `HistoricoCargos`, `PromoverCargo` e `AlterarGrupo`, com validação por `Enum.IsDefined`. Build com 0 avisos e 0 erros | Código igual à versão verificada (só mudou a ordem dos métodos e o uso de `IReadOnlyList`). Em aberto para o Módulo 5: hierarquia obrigatória ou rebaixamento na promoção, e grupo × idade |
| 2026-11-16 | 2.1 | EF Core SqlServer 10.0.15 na Infrastructure. `TempleDbContext` com `DbSet => Set<T>()` e `ApplyConfigurationsFromAssembly`. Configurações de Igreja, Membro e HistoricoCargo, com o `Endereco` como complex type (configuração reaproveitada). Matrícula `IDENTITY` com índice único, CNPJ com índice único, FK Membro→Igreja `Restrict` e Histórico `Cascade`. Build com 0 avisos e 0 erros | O EF não passa complex type por construtor, então cada entidade ganhou um construtor privado só para o EF, com `#pragma CS8618` localizado (decisão do usuário). Removida a navegação `Membro.Igreja`, que agora guarda só o `IgrejaId`. Testado no LocalDB (criação, gravação e leitura OK) |
| 2026-11-16 | 2.2 | Auto-relacionamento configurado de forma explícita: `HasOne(IgrejaMatriz).WithMany(Filiais).HasForeignKey(IgrejaMatrizId)`, `IsRequired(false)`, `OnDelete(Restrict)` e `HasField("_filiais")`. Build com 0 avisos e 0 erros | Testado no LocalDB: o SQL Server recusa `Cascade` em auto-referência (*cycles or multiple cascade paths*); o `Restrict` bloqueia apagar matriz com filiais; sem `Include`, `Filiais` vem vazia (armadilha); o `EhMatriz` funciona sem `Include` porque usa o `IgrejaMatrizId`. A regra dos 2 níveis é garantida só pelo Domain |
| 2026-11-16 | 2.3 | `dotnet-ef` atualizado para 10.0.15. Pacote `EntityFrameworkCore.Design` na Api. Connection string `TempleDb` no `appsettings.Development.json`. `AddDbContext` no `Program.cs`, com a validação da connection string sem `!`/`??`. Seed idempotente `DadosDeDesenvolvimento` (`UseSeeding`/`UseAsyncSeeding`, só em Development). Migration `CriacaoInicial` aplicada: banco `TempleDb` criado no LocalDB, com a "Igreja Sede Exemplo" | O `Program.cs` foi digitado mas não estava salvo no primeiro build (dica: Auto Save). Na Aula 3.2, mover o registro para `AddInfrastructure()` |
| 2026-11-16 | 3.1 | DTOs (`sealed record` positional) organizados por funcionalidade na Application: `Comum/EnderecoDto`, `Igrejas/CriarIgrejaRequest` + `IgrejaResponse`, `Membros/CriarMembroRequest` + `MembroResponse` + `HistoricoCargoResponse`. Mapeamento manual com métodos de extensão (`ParaResponse`, `ParaDto`, `ParaEndereco`, `ParaMatriz`, `ParaFilialDe`, `ParaMembro`), sempre passando pelos construtores e factories do Domain. Build com 0 avisos e 0 erros | Decisões: pastas por funcionalidade, nomes Request/Response, mapeamento manual (sem AutoMapper ou Mapperly). Para depois: FluentValidation (3.2), `RespectNullableAnnotations` e enums como texto no JSON (3.3) |
| 2026-11-17 | 3.2 | `DomainException` no Domain: 25 trocas de `throw` em `Igreja`, `Membro` e `Endereco` (os `ThrowIfNull` ficaram). Constantes de tamanho públicas e `Endereco.EhUfValida`. Na Application: `NaoEncontradoException` e `ConflitoException`, as interfaces `IIgrejaRepository` e `IUnitOfWork`, a FluentValidation 12.2.0 (pt-BR, `CascadeMode.Stop`) com `EnderecoDtoValidator` e `CriarIgrejaRequestValidator`, o `IgrejaService` (criar matriz e filial, obter por Id) e o `AddApplication()`. Na Infrastructure: `IgrejaRepository`, `UnitOfWork` (`internal`) e o `AddInfrastructure()`, que recebeu o `AddDbContext`. `Program.cs` só liga as camadas. Build com 0 avisos e 0 erros; a Api sobe em Development com o DI validado. Sem migration nova | Tropeços: texto da tabela colado como código (linha 159 da `Igreja`), arquivo `ConflitosException.cs` com nome errado e Api rodando, que travou a DLL no build (MSB3027). A Parte A foi aplicada pelo Claude a pedido. Próxima: 3.3, com o handler global e ProblemDetails |
| 2026-11-17 | 3.3 | `IgrejasController` com `[ApiController]` e 3 rotas: `POST /api/igrejas` (matriz, 201 + Location), `POST /api/igrejas/{matrizId:guid}/filiais` e `GET /api/igrejas/{id:guid}`. `TratadorGlobalDeExcecoes` (`IExceptionHandler` + `IProblemDetailsService`): Validation → 400 (erros em camelCase), Domain → 422, NaoEncontrado → 404, Conflito → 409, outras → 500. `AddProblemDetails`, `UseExceptionHandler` e `UseStatusCodePages`. JSON com `RespectNullableAnnotations`, `RespectRequiredConstructorParameters` e `JsonStringEnumConverter`. Swagger UI (Swashbuckle.AspNetCore.SwaggerUI 10.3.1) sobre o OpenAPI nativo, só em Development. `Temple.Api.http` com `@baseUrl` e requisição nomeada. Template `WeatherForecast` removido. Testado pelo Swagger no `TempleDb`: matriz e filial com 201 | Decisões: Swagger UI, `DomainException` → 422 e filial como sub-recurso da matriz. Tropeços: `});;` no `Program.cs` e o corpo da filial enviado à rota de matriz no Swagger (virou matriz; lição: em REST, a URL carrega significado). O `complemento` precisa vir no JSON, nem que seja `null`. Próxima: 3.4, CRUD de Igrejas e Membros |
| 2026-11-19 | 3.4A | A 3.4 foi dividida em 3.4A (Igrejas) e 3.4B (Membros). Rotas novas: `GET /api/igrejas` (por nome, com `?ativa=` opcional), `GET /api/igrejas/{id}/filiais`, `PUT /api/igrejas/{id}` (nome, telefone e endereço, 200 com corpo), `POST /{id}/ativar` e `POST /{id}/desativar` (204). Domain: `AlterarNome`, `AlterarTelefone` e regras de ativação (matriz com filial ativa não desativa; filial de matriz inativa não ativa; filial ativa não é criada em matriz inativa; todas 422). `AtualizarIgrejaRequest` + validador; regras compartilhadas em `RegrasDaIgreja` (extensões do `IRuleBuilder`). O repositório faz `Include` da matriz e das filiais no `ObterPorIdAsync` e `AsNoTracking` na listagem. Sem migration nova. Testado no `TempleDb` pelo Swagger | Decisões: só exclusão lógica (sem DELETE), editar nome, telefone e endereço, e listagem simples. Tropeços: `.http` não salvo e Swagger com a Api parada ("Failed to fetch", "Undocumented"). Os valores `"string"` de exemplo do Swagger foram gravados (texto válido, mas sem sentido). Próxima: 3.4B, CRUD de Membros |
| 2026-11-19 | 3.4B | CRUD de Membros em `/api/membros`: `POST` (201 + Location; matrícula gerada pelo banco), `GET` paginado com filtros `igrejaId`, `nome` (parcial, `LIKE`), `cargo` e `ativo` (`PaginaResponse<T>` na Api e `ResultadoPaginado<T>` no repositório; padrão 20, máximo 100; ordem por nome e matrícula; `Skip`/`Take`), `GET /{id}`, `PUT /{id}` (nome, endereço e grupo), `POST /{id}/ativar` e `/desativar` (204), `POST /{id}/promocoes` (200 com o membro) e `GET /{id}/historico-cargos`. Domain: `Membro.Ativo` (nasce ativo), `Ativar(Igreja)` exige igreja ativa (422), `Desativar`, e membro desativado não é promovido (422). Validadores com `RegrasDoMembro` e `IsInEnum` (o conversor JSON aceita números). `MembroService` usa dois repositórios e um único `IUnitOfWork`. Migration `AdicionaAtivoAoMembro` com `defaultValue` trocado à mão para `true`. Testado pelo Swagger no `TempleDb`: cadastro, desativar, ativar e promoção Cooperador → Diácono com histórico gravado | Lição de ouro: migration gerada não é migration revisada. Tropeços: migration gerada duas vezes (o nome já existia; para refazer, usar `migrations remove`), 4 arquivos com maiúscula errada no nome (corrigidos) e `"string"` do Swagger no complemento. A busca por nome ignora maiúsculas, mas não acentos (collation CI_AS). Pendente (Módulo 5): o que fazer com os membros quando a igreja é desativada. Próxima: Módulo 4, Segurança |
| 2026-11-19 | 4.1 | ASP.NET Identity (`AddIdentityCore` + `AddEntityFrameworkStores`) no próprio `TempleDbContext`, que passou a herdar `IdentityDbContext<Usuario, IdentityRole<Guid>, Guid>` (o `base.OnModelCreating` vem primeiro). `Usuario : IdentityUser<Guid>` com `Nome`, na Infrastructure; o Domain não conhece o login. Na Application: `AutenticacaoService`, contratos `IServicoDeIdentidade` e `IGeradorDeToken`, `LoginRequest` + validador e `NaoAutorizadoException` (401). JWT HS256 (`JsonWebTokenHandler`) de 8h, com claims `sub`, `email`, `name` e `jti`; `OpcoesJwt` exige chave de 32 bytes ou mais. `JwtBearer` valida emissor, audiência, assinatura, algoritmo e validade. Política padrão: toda rota exige login, menos o `POST /api/autenticacao/login` (`[AllowAnonymous]`). `GET /api/autenticacao/me`. Senha com 8 caracteres ou mais e bloqueio de 15 min após 5 erros, com mensagem genérica. Seed do Pastor em Development, com chave e senha no user-secrets. Swagger com Authorize (transformador do OpenAPI). `.http` com `# @prompt senha` e `Authorization` em todas as requisições. Migration `AdicionaIdentity` (7 tabelas). Testado pelo Swagger no `TempleDb` | Decisões: Identity no mesmo contexto, seed do Pastor agora (a rota de criar usuário fica para a 4.2), tudo protegido e só access token. Tropeços: o EF não aceita 2 construtores com o mesmo número de parâmetros (o do EF ficou só com `nome`); os segredos precisam vir antes da migration; nome de arquivo `NaoAutorizasoException` e um `Authorization` faltando no `.http` (corrigidos). As mensagens do Identity estão em inglês (traduzir na 4.2). Próxima: 4.2, Roles e Policies por perfil |
| 2026-11-20 | 4.2 | Perfis como papéis do Identity (`AddRoles<IdentityRole<Guid>>`), sem migration (as tabelas já existiam). `SemeadorDePerfis` cria um papel por valor do enum `Perfil` ao subir a Api, em qualquer ambiente, e o seed do Pastor passou a sempre garantir o papel Pastor. O token ganhou uma claim `role` por perfil (`ClaimsDoTemple`), e o `JwtBearer` usa `RoleClaimType = "role"` (necessário por causa do `MapInboundClaims = false`). Autorização **por permissão**: `Permissoes` (`Igrejas.Ver`/`Gerenciar`, `Membros.Ver`/`Gerenciar`/`Promover`, `Usuarios.Gerenciar`) e `PoliticasDeAutorizacao` (a matriz perfil → permissão num lugar só, com `RequireRole`); `[Authorize(Policy = ...)]` em cada ação de igrejas e membros. Rotas `/api/usuarios` só para o Pastor: criar (201), listar, obter e `PUT /{id}/perfis`. `UsuarioService` com a regra do último Pastor (422). `GerenciadorDeUsuarios` com `UserManager` e transação; erros do Identity em pt-BR (`DescritorDeErrosEmPortugues`): `Duplicate` → 409, `Password` → 400 no campo `Senha`. `/me` devolve os perfis. `.http` com `# @prompt senhaInicial`. Testado pelo Swagger no `TempleDb`: perfis criados, Pastor com papel, Secretária criada, erros 409/400, 403 do Secretário em promoção, usuários e igrejas, e 422 ao tirar o último Pastor | Tropeços: a promoção ficou com `GerenciarMembros` (o Secretário poderia promover; o build não pega, só o diff e o teste de acesso) e o arquivo `PoliticaDeAutorizacao.cs` no singular (corrigidos). Os perfis mudam só no próximo login, pois o token vale 8h (pendência do Módulo 9: security stamp ou refresh token). Líderes, Funcionário e Cozinheira levam 403 em membros até a 4.3. Próxima: 4.3, escopo por igreja |
| 2026-11-20 | 4.3A | A 4.3 foi dividida em 4.3A (vínculo) e 4.3B (escopo), e o plano passou a 41 aulas. `Usuario.MembroId` (`Guid?`) com `VincularMembro`, mapeado como 1:1 sem navegação (`HasOne<Membro>().WithOne()`, FK em `AspNetUsers`, `Restrict`). Migration `VinculaUsuarioAoMembro`: coluna anulável, índice único **filtrado** (`[MembroId] IS NOT NULL`, para aceitar vários usuários sem membro) e FK. `CriarUsuarioRequest` exige `MembroId`; `UsuarioService` confere se o membro existe (404), está ativo (422) e está livre (409). Rota `PUT /api/usuarios/{id}/membro` (só o Pastor) para ligar ou trocar o membro. `/me` mostra o `membroId`. `GET /api/membros/meu-cadastro` para qualquer usuário logado, lendo o vínculo no banco (não no token): sem membro → 404. Extensão `User.ObterIdDoUsuario()` (`UsuarioLogadoExtensions`) no lugar da leitura da claim `sub` dentro do controller. Testado pelo Swagger no `TempleDb`: `secretaria@temple.local` ligada ao membro "Maria da Secretaria"; o Pastor inicial continua sem membro | Decisões: usuário ligado a membro, matriz vê filiais, líderes só leem (o de grupo vê o grupo, o genérico vê a igreja), `meu-cadastro` e 404 para o que estiver fora do alcance (as três últimas valem na 4.3B). Tropeços: um `;` sobrando depois de um `return` (instrução vazia, corrigida), um 404 por confundir o id do usuário com o do membro (os dois se chamam Maria) e um 401 por o Swagger ter perdido a autorização. No SQL Server, o `Restrict` do EF aparece como `NO_ACTION`. Próxima: 4.3B, escopo por igreja |
| 2026-11-23 | 4.3B | Escopo de acesso por igreja e por grupo, sem migration. Na Application (`Comum/Acesso`): `EscopoDeAcesso` (`Completo`, `Nenhum`, `DasIgrejas`, `DosGrupos`; `AlcancaIgreja` e `AlcancaMembro`), o contrato `IUsuarioAtual` e o `ProvedorDeEscopo` (Scoped, calcula uma vez por requisição a partir dos perfis **no banco**): sem membro → Pastor tem acesso total, os demais nenhum; membro inexistente ou desativado → nenhum; Pastor, Secretário e Tesoureiro → a própria igreja e, se for matriz, as filiais; Líder genérico → a igreja; líder de grupo → só o grupo na igreja; o resto → nenhum. `AcessoNegadoException` → 403. Os services aplicam o escopo: fora do alcance dá **404** (mesma mensagem de inexistente); só o acesso total cria matriz (403); `meu-cadastro` não passa pelo escopo. O login recusa membro desativado (401 genérico). Os repositórios filtram no banco (`IN (...)` por igreja e por grupo; usuários com `EXISTS` no membro), para a paginação e o total ficarem certos. Na Api: `UsuarioAtual` com `IHttpContextAccessor` e `ObterIdDoUsuario()`, e os 5 perfis de líder ganharam `VerMembros` (a política diz *se* pode ver; o escopo diz *o quê*). Testado pelo Swagger no `TempleDb`: Pastor geral vê as 5 igrejas; a secretária só a Sede Exemplo (404 nas outras, 200 no `meu-cadastro`); `pastor.sede@temple.local` (Borges) vê a matriz e a filial Pra Deus, só ele em usuários e leva 403 ao criar matriz; `lider.jovens@temple.local` (Lucas Jovem) vê só o Lucas e leva 403 em igrejas; membro desativado perde o login | Decisões: só o acesso total cria matriz, gerenciar segue o mesmo alcance de ver e membro desativado perde o acesso. Tropeços: o `ObterEntidadeAsync` do `IgrejaService` ficou sem o `AlcancaIgreja` (o build passa, pois uma variável não lida não gera aviso; sem a checagem, qualquer Pastor abriria e editaria igreja de outra matriz pelo id) e só foi salvo na terceira conferência; o 403 documentado no `CriarFilial` em vez do `CriarMatriz`. Próxima: Módulo 5, Regras de Negócio |
| 2026-11-23 | 5.1 | Rota `GET /api/membros/aniversariantes?mes=&igrejaId=` (`AniversariantesController` próprio), sem migration e sem paginação. Pasta `Application/Aniversariantes`: `ListarAniversariantesRequest` (`int? Mes`, `Guid? IgrejaId`) com validador (`InclusiveBetween(1, 12)`, que ignora o `null`), o read model `AniversarianteDoMes` (lido pelo repositório, já com o nome da igreja), o `AniversarianteResponse` (com `Dia` e `IdadeQueCompleta`, sem endereço) e o `AniversarianteService`, que escolhe o mês (padrão = mês atual pelo `TimeProvider` injetado, `GetLocalNow`) e calcula dia e idade. `TimeProvider.System` registrado como Singleton. No `MembroRepository`: `ListarAniversariantesAsync` filtra só ativos e `DataNascimento.Month` (vira `DATEPART(month, ...)` no SQL), faz `Join` com Igrejas num tipo anônimo, ordena por dia e nome e projeta no read model; o filtro de escopo foi extraído para o `AplicarEscopo` (privado e estático), usado também pela listagem de membros. Permissão nova `Aniversariantes.Ver`, com os mesmos perfis de `VerMembros`. `.http` com 4 exemplos. Testado pelo Swagger no `TempleDb` com "Ana Novembro" (Sede Teste, Irmãs) e "Pedro Novembro" (Sede Exemplo, Jovens): o acesso total vê os dois, o Pastor da Sede Teste só a Ana, a secretária só o Pedro e o líder dos jovens a lista vazia; mês 13 → 400 | Decisões: mês opcional com padrão no mês atual, permissão própria, só ativos e resumo próprio. Tropeço: a linha `GerenciarUsuarios` ficou duplicada nas políticas (o build não pega e a Api não reclama, porque o `AddPolicy` repetido substitui o anterior; corrigida). A seção do `.http` ficou depois da de Membros, e não no fim: aceita. Próxima: 5.2, promoção de cargo com histórico |
| 2026-11-23 | 5.2 | Regras da promoção e histórico como trilha de auditoria. Domain: `Membro.PromoverCargo` recebe a observação, o autor (id e nome) e o `DateTimeOffset agora` (saiu o `DateTime.Today`); subir pode pular degraus, descer (`novoCargo < CargoAtual`, comparando os enums pela hierarquia) exige motivo na observação (422); a data não pode ser anterior ao nascimento nem à última promoção (`Max` do histórico, mesmo dia permitido; 422 com a data na mensagem). `HistoricoCargo` ganhou `Observacao` (limpa e com no máximo 500), `PromovidoPorId`, `PromovidoPorNome` (cópia do nome) e `RegistradoEm`, todos anuláveis por causa do registro antigo do Borges; `EhRebaixamento` é calculado (não vira coluna); dois construtores: o `internal` valida e o `private` é do EF (sem `#pragma`). Application: `PromoverCargoRequest` com `Observacao = null` (opcional no JSON), validador recebendo o `TimeProvider` e limitando a observação, `HistoricoCargoResponse` com `Rebaixamento`, observação, autor e `RegistradoEm`, e o `MembroService` buscando o autor no Identity pelo `IUsuarioAtual` e ordenando o histórico por data e depois `RegistradoEm`. Infrastructure: tamanhos das colunas, sem FK para `AspNetUsers`. Migration `AdicionaAutorEObservacaoAoHistorico` (4 colunas anuláveis). Testado pelo Swagger no `TempleDb`: Borges com data antes da última promoção (422), Presbítero com observação (200), rebaixamento sem motivo (422) e com motivo (200); Lucas com data antes do nascimento (422) e Membro → Diácono pulando degrau (200) | Decisões: sobe livre e desce com motivo, datas limitadas pelo nascimento e pela última promoção, autor e observação gravados e, depois de 2 falhas nos testes da referência (promoções no mesmo dia em ordem aleatória), o `RegistradoEm` para desempatar. Tropeços: faltou o `ThenBy(RegistradoEm)` (o build passa; corrigido) e o teste do Lucas, refeito. Pendente (Módulo 9): os outros `DateTime.Today` (nascimento, fundação e validadores de cadastro). Próxima: 5.3, certificados em PDF |
| 2026-11-24 | 5.3A | Certificado de promoção de cargo em PDF (QuestPDF **2026.9.2**, licença Community), com o registro de cada emissão. Domain: entidade `CertificadoEmitido` (`Numero` `int` IDENTITY e único, `Tipo`, `MembroId`, `HistoricoCargoId` anulável, emissor id e nome em cópia e `EmitidoEm`), criada pela fábrica `DePromocao`, que recusa membro desativado e rebaixamento (422). Application (`Certificados/`): `ICertificadoRepository`, `IGeradorDeCertificadoPdf`, `DadosDoCertificado` (o texto pronto: o gerador só desenha), `CertificadoPdf`, `TextosDoCertificado` (nome do cargo com acento, data por extenso em pt-BR e `certificado-000001.pdf`) e `CertificadoService` (escopo → 404, promoção procurada no histórico do próprio membro → 404, já emitida → 409, emissor buscado no Identity; a 2ª via reimprime com o mesmo número e a data da emissão original, e um número fora do escopo dá o mesmo 404 de um inexistente). Infrastructure: `GeradorDeCertificadoPdf` (A4 paisagem, borda dupla, `AlignMiddle`, assinaturas com rótulos "Pastor" e "Secretário(a)" e o número no rodapé), `CertificadoEmitidoConfiguration` (FKs `Restrict` para o membro e o histórico, índice único filtrado em `HistoricoCargoId`) e `CertificadoRepository`; o gerador é Singleton. Migration `AdicionaCertificadosEmitidos`. Api: permissão `Certificados.Emitir` (Pastor e Secretário), `CertificadosController` com `POST /api/membros/{membroId}/historico-cargos/{historicoCargoId}/certificado` e `GET /api/certificados/{numero:int}`, devolvendo `File(...)` (download), e `ProducesResponseType<Stream>` para o OpenAPI mostrar `string/binary`. Testado pelo Swagger no `TempleDb`: Borges, promoção a Presbítero (200, nº 000001), repetida (409), rebaixamento (422), registro antigo sem autor (200, nº 000002), 2ª via (200), número inexistente (404) e líder (403) | Decisões: dividir a 5.3 em A e B (o plano passa a ter 42 aulas), registrar a emissão sem guardar o PDF, número sequencial global, 2ª via com o mesmo número, um certificado por promoção (409), permissão própria e layout A4 paisagem sem logo, com as assinaturas à mão. Tropeços: dois arquivos com nome diferente do tipo (`DadosCertificados.cs` e `TextoDoCertificado.cs`, renomeados); no Swagger, o id colado com um espaço sobrando foi barrado pela validação do próprio navegador. A seção de certificados do `.http` ficou no fim do arquivo, e não depois dos aniversariantes, o que não muda o funcionamento. Próxima: 5.3B |
| 2026-11-25 | 5.3B | Registros de Batismo e de Apresentação de Criança e os certificados deles, reaproveitando a emissão, o número, a 2ª via e o layout da 5.3A. Domain: `Batismo` (ligado ao membro, no máximo um por membro; data entre o nascimento e hoje, oficiante obrigatório e local opcional; `Registrar` recusa membro desativado) e `ApresentacaoCrianca` (registro avulso, ligado à igreja; nome e nascimento da criança, pai e mãe opcionais, mas pelo menos um, data entre o nascimento e hoje, menos de 12 anos na data e igreja ativa). Os dois têm `Corrigir`, que recebe `temCertificadoEmitido` e recusa a correção depois do certificado (422), e **validam tudo antes de atribuir**, para uma correção inválida não deixar o registro pela metade. `CertificadoEmitido`: `MembroId` passou a `Guid?` (a apresentação não tem membro), mais `BatismoId?` e `ApresentacaoCriancaId?`, construtor de criação genérico e fábricas `DeBatismo` e `DeApresentacao`, que preenchem a sua ligação com inicializador de objeto. Application: `Batismos/` (request único para POST e PUT, validador, response, mapeamento, repositório e service com escopo pelo membro, 409 no segundo batismo) e `Apresentacoes/` (requests de criar e atualizar com os pais no fim e `= null`, por causa do `RespectRequiredConstructorParameters`; `RegrasDaApresentacao` com extensões genéricas e o `Must((request, _) => ...)` para exigir um dos pais; service com escopo pela igreja). `CertificadoService` refatorado: as três emissões seguem o mesmo roteiro, e a 2ª via faz um `switch` pelo `Tipo`; o texto de cada tipo é separado do desenho comum (`GerarPdfAsync`, que recebe a igreja do membro ou a da apresentação); `TextosDoCertificado.Filiacao` usa um `switch` sobre tupla ("filha" concorda com "a criança"). Infrastructure: configurações com índice único em `Batismos.MembroId` e índices únicos filtrados em `BatismoId` e `ApresentacaoCriancaId`, FKs `Restrict`, repositórios (a lista de apresentações é filtrada pelo escopo no banco e ordenada pela data mais recente). Migration `AdicionaBatismosEApresentacoes`, testada também por cima de um certificado já existente e no rollback. Api: permissão `Registros.Gerenciar` (Pastor e Secretário), `BatismosController` (`/api/membros/{membroId}/batismo`: POST 201, GET e PUT), `ApresentacoesController` (`/api/apresentacoes`: POST 201, GET com `?igrejaId=`, GET e PUT por id) e, no `CertificadosController`, `POST .../batismo/certificado` e `POST /api/apresentacoes/{id}/certificado`. Testado pelo Swagger no `TempleDb`: batismo do Borges (201, 409, correção 200, certificado nº 000003, correção depois dele 422), apresentação (201, sem pais 400, 12 anos 422, certificado nº 000004, correção depois dele 422), 2ª via dos nº 1, 3 e 4 (200) e líder (403) | Decisões: batismo como entidade própria, apresentação avulsa (a criança não é membro), pais em texto com pelo menos um, menos de 12 anos (ECA), permissão própria para os registros, correção só antes do certificado, oficiante digitado e obrigatório e batismo sem idade mínima. Regras que derivei: membro desativado não recebe batismo nem certificado de batismo, igreja inativa não recebe apresentação e a lista de apresentações não é paginada. Tropeços: `IApresentacaoRepsitory.cs` (renomeado), o `DependencyInjection` da Application sem os dois registros (o build passa, mas a Api não subiria) e, no `CertificadosController`, as ações novas coladas entre os atributos e o método da 2ª via: o build passa, mas o `ObterSegundaVia` ficou sem rota e a rota da 2ª via foi para o `EmitirDeBatismo` (corrigido pelo Claude, a pedido). Próxima: Módulo 5B, Financeiro |
| 2026-11-26 | 5B.1 | Domain da movimentação financeira, sem migration. Enums `FormaPagamento` (Dinheiro, Pix, Cartao, Transferencia), `TipoLancamento` (Dizimo, Oferta, OutraEntrada, Despesa) e `NaturezaLancamento` (Entrada, Saida), todos começando em 1. Entidade `Lancamento`: igreja, tipo, data, valor em `decimal`, forma de pagamento, descrição opcional, autor (id e nome em cópia) e `RegistradoEm`; os quatro campos do estorno (motivo, autor e momento) são anuláveis. Dois construtores `private`: o do EF, que não valida, e o de criação, que recebe a `Igreja` (inativa → 422) e o `descricaoObrigatoria`, a ser usado pelas fábricas da 5B.2. Valor: maior que 0, no máximo `ValorMaximo` (o limite de `decimal(18,2)`) e com no máximo 2 casas (`decimal.Round(valor, 2) != valor`; 10,500 passa, 10,005 não). Data não futura. Calculados: `Natureza` (Despesa é saída, o resto é entrada), `Estornado` e `ValorNoSaldo()` (entrada +, saída −, estornado 0). `Estornar` recusa um segundo estorno e valida antes de alterar. Testado com 20 cenários (por reflexão, num console de teste) na referência | Decisões do módulo: uma entidade com tipo e fábricas, estorno com motivo em vez de edição ou exclusão, despesa com categoria fixa e descrição obrigatória, fechamento mensal por igreja (5B.5), dízimo só com a data e lançado na igreja do membro, cartão como um valor só e descrição obrigatória também na Outra Entrada. Regras que derivei: igreja inativa não recebe lançamento, valor com 2 casas e dentro de `decimal(18,2)`, data não futura e autor gravado. Código igual à referência na primeira tentativa. Próxima: 5B.2, as fábricas de cada tipo |
| 2026-11-26 | 5B.2 | Fábricas de cada tipo de lançamento, sem migration. Enum `CategoriaDespesa` (Agua, Energia, Aluguel, Manutencao, Salarios, Missoes, Material, Outras, começando em 1). `Lancamento` ganhou `MembroId` (`Guid?`, sem navegação) e `Categoria` (`CategoriaDespesa?`), nos dois construtores. Fábricas: `Dizimo` (exige `Membro` ativo e da mesma igreja, senão 422; descrição vira observação opcional), `Oferta` (`Membro?`: nulo é anônima; identificada exige membro ativo e pode ser em qualquer igreja; `if (membro is not null)` em vez de `?.`), `OutraEntrada` (sem membro, descrição obrigatória) e `Despesa` (categoria obrigatória e validada com `Enum.IsDefined`, descrição obrigatória). Argumentos nomeados (`categoria: null`, `descricaoObrigatoria: true`) na chamada ao construtor de 12 parâmetros. Auxiliar `ValidarMembroAtivo`. Testado com 16 cenários num console de teste na referência | Decisões: oferta identificada em qualquer igreja, membro desativado → 422, despesa sem nada além de categoria e descrição. Primeiro build com 4 erros CS8323 (argumento nomeado fora de posição): o construtor de criação não tinha recebido `membroId` e `categoria` (passo 2.3 pulado) e o comentário novo tinha ido para o construtor do EF. Após a correção, código igual à referência (só indentação diferente). Próxima: 5B.3, EF com `decimal(18,2)` e índices |
| 2026-11-26 | 5B.3 | Tabela `Lancamentos` com a migration `AdicionaLancamentos`. `LancamentoConfiguration`: `Valor` com `HasPrecision(18, Lancamento.CasasDecimais)` (o `ValorMaximo` cabe exatamente), tamanhos das strings vindos das constantes do Domain, FKs para `Igreja` e `Membro` com `Restrict` e sem navegação, e os índices compostos `(IgrejaId, Data)` (relatórios e fechamento) e `(MembroId, Data)` (contribuições do membro), que também cobrem as FKs. Quatro CHECK constraints dentro de `ToTable("Lancamentos", tabela => ...)`: `CK_Lancamentos_Valor` (> 0), `CK_Lancamentos_Membro` (dízimo com membro, oferta livre, outra entrada e despesa sem; também recusa tipo inexistente), `CK_Lancamentos_Categoria` (só e sempre na despesa) e `CK_Lancamentos_Estorno` (os quatro campos todos nulos ou todos preenchidos), com os números vindos de `(int)TipoLancamento.X` em string interpolada, sem número mágico. `Natureza` e `Estornado` não viram colunas (só `get`). Testado com 20 cenários (EF e SQL cru) e rollback na referência | Decisões: precisão na propriedade, CHECK nas regras principais e os dois índices compostos. Código igual à referência na primeira tentativa. Próxima: 5B.4, a Api dos lançamentos |
| 2026-11-26 | 5B.4A | A 5B.4 foi dividida em 5B.4A (criação, consulta por id e estorno) e 5B.4B (consulta paginada), e o plano passou a 43 aulas. Permissões `Financeiro.Lancar` (Pastor e Tesoureiro, também para consultar) e `Financeiro.Estornar` (só Pastor). `LancamentosController` em `api/lancamentos`: `POST dizimos`, `ofertas`, `outras-entradas` e `despesas` (201 + Location), `GET {id}` e `POST {id}/estornar` (200 com o lançamento; exige as duas políticas, a da classe e a do método, e o Tesoureiro recebe 403). Pasta `Application/Lancamentos`: um request por tipo espelhando as fábricas (dízimo com `Guid MembroId`, oferta com `Guid? MembroId = null`, só a despesa com `Categoria`), 5 validadores com o `RegrasDoLancamento` (valor > 0, até `ValorMaximo` e com 2 casas; data não futura; descrição; observação), `IsInEnum` nos enums, `LancamentoResponse` com `Natureza` e `Estornado`, `ILancamentoRepository` e `LancamentoService`. O autor (id e nome) vem do usuário logado. Alcance: a igreja do lançamento precisa estar no alcance (404); o membro do dízimo passa pelo `AlcancaMembro`; o da oferta só precisa existir. Testado com 32 cenários pela Api na referência e pelo usuário no Swagger/.http | Decisões: só o Pastor estorna, uma rota por tipo, consulta paginada com filtros (5B.4B) e oferta identificada com membro de qualquer igreja. Regras que derivei: Guid vazio na oferta → 400, membro do dízimo fora do alcance → 404, estorno devolve 200 com o corpo. Tropeço: dois arquivos com nome errado (`EstornatlancamentoRequest.cs` e `LancamentoRequest.cs`), renomeados; depois, tudo igual à referência. Próxima: 5B.4B |
| 2026-11-26 | 5B.4B | `GET /api/lancamentos` paginado, no padrão da listagem de membros: `ListarLancamentosRequest` por `[FromQuery]` (`IgrejaId?`, `De?`, `Ate?`, `Tipo?`, `MembroId?`, `IncluirEstornados = true`, `Pagina = 1`, `TamanhoPagina = 20`) e validador (`Ate` >= `De` só quando os dois vêm, com `When`; `IsInEnum`; página >= 1; tamanho de 1 a 100). `ListarAsync` no repositório: `AsNoTracking`, o escopo por igreja aplicado antes dos filtros (o filtro só estreita; fora do alcance dá página vazia, não 404), um `Where` por filtro com pattern matching, `EstornadoEm == null` para excluir os estornados (a propriedade calculada `Estornado` não vira SQL), ordem estável (data desc, `RegistradoEm` desc, `Id`) e `Skip`/`Take`, usando o índice `(IgrejaId, Data)` da 5B.3. O service monta o `PaginaResponse<LancamentoResponse>`. Três requisições novas no `.http`. README atualizado com o módulo financeiro. Testado com 51 cenários pela Api na referência (32 da 5B.4A e 19 novos) e pelo usuário | Regra que derivei: período invertido → 400 no campo `ate`. Desvios só de posição (o `ListarAsync` no topo do service e um `using` fora de ordem no controller), sem efeito no comportamento. Próxima: 5B.5, relatórios e fechamento mensal |
