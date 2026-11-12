# ⛪ Temple: Plano de Estudo (React + C#/.NET)

> Sistema de gestão de igreja construído passo a passo, linha a linha, como projeto de estudo.

**Legenda de status:** ⬜ Não iniciada · 🔄 Em andamento · ✅ Concluída

---

## 📊 Progresso geral

| Módulo | Tema | Aulas | Concluídas | Status |
|---|---|:---:|:---:|:---:|
| 0 | Ambiente | 2 | 2 | ✅ |
| 1 | Fundamentos do Backend | 4 | 1 | 🔄 |
| 2 | Persistência (EF Core + SQL Server) | 3 | 0 | ⬜ |
| 3 | Application e API | 4 | 0 | ⬜ |
| 4 | Segurança | 3 | 0 | ⬜ |
| 5 | Regras de Negócio | 3 | 0 | ⬜ |
| 5B | Financeiro | 6 | 0 | ⬜ |
| 6 | Fundamentos do Frontend | 4 | 0 | ⬜ |
| 7 | Integração Front + API | 4 | 0 | ⬜ |
| 8 | Funcionalidades Finais | 4 | 0 | ⬜ |
| 9 | Qualidade | 2 | 0 | ⬜ |
| **Total** | | **39** | **3** | **8%** |

---

## 🧱 Stack definida

| Camada | Tecnologia |
|---|---|
| Backend | C# + .NET 10 (LTS), ASP.NET Core Web API |
| Arquitetura | Clean Architecture (Domain, Application, Infrastructure, Api, Tests) |
| Banco de dados | SQL Server + Entity Framework Core 10 (dev: LocalDB `(localdb)\MSSQLLocalDB`) |
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
- **Membro:** nome, endereço, matrícula, data de nascimento (`DateOnly`).
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
| 1.2 | Domain: entidade `Igreja` (classes, propriedades, encapsulamento) | ⬜ |
| 1.3 | Domain: `Membro`, value object `Endereco`, `DateOnly` | ⬜ |
| 1.4 | Domain: enums `TipoCargo`, `TipoCertificado` e `HistoricoCargo` | ⬜ |

### Módulo 2: Persistência
| # | Aula | Status |
|---|---|:---:|
| 2.1 | `DbContext` e `IEntityTypeConfiguration<T>` | ⬜ |
| 2.2 | Relacionamento matriz ↔ filiais | ⬜ |
| 2.3 | Migrations e seed (cargos e Pastor inicial) | ⬜ |

### Módulo 3: Application e API
| # | Aula | Status |
|---|---|:---:|
| 3.1 | DTOs com `record` e por que não expor a entidade | ⬜ |
| 3.2 | Services, injeção de dependência e FluentValidation | ⬜ |
| 3.3 | Controllers REST, Swagger/OpenAPI e `ProblemDetails` | ⬜ |
| 3.4 | CRUD de Igrejas e Membros | ⬜ |

### Módulo 4: Segurança
| # | Aula | Status |
|---|---|:---:|
| 4.1 | ASP.NET Identity e login com JWT | ⬜ |
| 4.2 | Roles e Policies por perfil | ⬜ |
| 4.3 | Escopo por igreja (a filial só vê os próprios dados) | ⬜ |

### Módulo 5: Regras de Negócio
| # | Aula | Status |
|---|---|:---:|
| 5.1 | Aniversariantes do mês (LINQ + `DateOnly`) | ⬜ |
| 5.2 | Promoção de cargo com histórico | ⬜ |
| 5.3 | Certificados em PDF com QuestPDF | ⬜ |

### Módulo 5B: Financeiro
| # | Aula | Status |
|---|---|:---:|
| 5B.1 | Domain: movimentação financeira (entrada/saída), `decimal` e enum `FormaPagamento` | ⬜ |
| 5B.2 | Domain: Dízimo (membro obrigatório), Oferta (pode ser anônima), Despesa | ⬜ |
| 5B.3 | EF Core: `decimal(18,2)` e índices por data e igreja | ⬜ |
| 5B.4 | API: lançamentos com a Policy de Tesoureiro/Pastor | ⬜ |
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
