Projeto: CadastroAcoes

Aplicação ASP.NET Core (API + SPA) para cadastro de usuários, produtos e vendedores.
Backend em C# (.NET 8) e MongoDB como banco de dados.

Observações rápidas:
- As variáveis de ambiente podem ser definidas em `.env` (não comitar). Há um arquivo `.env.example` com placeholders.
- Arquivos estáticos (HTML/CSS/JS) ficam em `wwwroot`.

## Estrutura de diretório (visão geral)
```
CadastroAcoes/                      # raiz do projeto (contém o .csproj)
├─ Controller/                       # Controllers API (Auth, Products, Clients, Vendors...)
├─ Model/                             # Modelos e classes de domínio
│  ├─ Repository/                     # Interfaces e implementações de repositórios (MongoDB)
│  ├─ User.cs                          # Exemplo de entidade User
│  ├─ UserClient.cs
│  ├─ UserVendor.cs
│  └─ Product.cs
├─ wwwroot/                           # Arquivos estáticos (SPA)
│  ├─ assets/                         # CSS, JS customizados
│  ├─ content/                        # scripts e conteúdo (login.js, etc.)
│  ├─ layouts/                        # páginas estáticas (index, Pages/login.html, Pages/register.html)
│  └─ app/                            # SPA / área autenticada (home.html)
├─ Properties/                        # launchSettings.json e configurações de execução
├─ appsettings.json                   # configuração geral (pode ser sobrescrita por .env)
├─ .env.example                       # Exemplo de variáveis de ambiente (não comitar)
├─ .gitignore                         # Ignora .env, bin/, obj/ etc.
├─ Program.cs                         # configuração de inicialização (DI, auth, MongoDB)
└─ CadastroAcoes.csproj
```
## Como rodar (desenvolvimento)
1. Copie `.env.example` para `.env` e preencha as variáveis (MongoDb__ConnectionString, MongoDb__Database, Jwt__Key, etc.).
2. No terminal, na pasta do projeto, rode:
   ```powershell
   dotnet run
   ```
3. Acesse a aplicação em `http://localhost:5000` (ou na porta definida em `ASPNETCORE_URLS`).

## Notas

website pra cadastro de ações, produtos, serviços, para o trabalho da faculdade

modelo mvc
api + spa (single page application)

banco de dados mongoDB

