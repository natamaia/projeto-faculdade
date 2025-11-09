# CadastroAcoes

## Visão Geral do Projeto

`CadastroAcoes` é uma aplicação web desenvolvida em ASP.NET Core que combina uma API RESTful com uma Single Page Application (SPA). O objetivo principal é gerenciar o cadastro de usuários (clientes e vendedores), produtos e outras ações relacionadas.

**Tecnologias Principais:**
*   **Backend:** C# (.NET 8), ASP.NET Core Web API
*   **Banco de Dados:** MongoDB
*   **Frontend:** HTML, CSS (Tailwind CSS), JavaScript
*   **Autenticação:** JWT (JSON Web Tokens)

## Estrutura do Projeto

A estrutura de diretórios do projeto é organizada da seguinte forma:

```
CadastroAcoes/                      # Raiz do projeto (contém o .csproj principal)
├─ Controller/                       # Controladores da API RESTful (Auth, Products, Clients, Vendors)
│  ├─ AuthController.cs              # Lógica de registro e login de usuários, geração de JWT
│  ├─ ClientsController.cs           # Gerenciamento de perfis de clientes
│  ├─ ProductsController.cs          # Gerenciamento de produtos
│  └─ VendorsController.cs           # Gerenciamento de perfis de vendedores
├─ Model/                             # Modelos de dados e lógica de repositório
│  ├─ Repository/                     # Interfaces e implementações de repositórios para interação com MongoDB
│  │  ├─ IProductRepository.cs
│  │  ├─ IUserClientRepository.cs
│  │  ├─ IUserRepository.cs
│  │  ├─ IUserVendorRepository.cs
│  │  ├─ ProductRepository.cs
│  │  ├─ UserClientRepository.cs
│  │  ├─ UserRepository.cs
│  │  └─ UserVendorRepository.cs
│  ├─ Product.cs                     # Modelo de dados para produtos
│  ├─ User.cs                        # Modelo de dados principal para usuários (autenticação)
│  ├─ UserClient.cs                  # Modelo de dados para perfis de clientes
│  └─ UserVendor.cs                  # Modelo de dados para perfis de vendedores
├─ wwwroot/                           # Arquivos estáticos do frontend (SPA)
│  ├─ assets/                         # Arquivos CSS (Tailwind), JavaScript customizados (register.js, ui.js)
│  ├─ content/                        # Scripts e conteúdo específico (login.js, App.js)
│  ├─ layouts/                        # Páginas HTML principais (index.html) e subdiretórios
│  │  ├─ Pages/                       # Páginas de navegação (login.html, register.html)
│  │  └─ Properties/                  # Componentes de layout (footer.html, header.html, sidebar.html)
│  └─ app/                            # Área autenticada da SPA (home.html)
├─ Properties/                        # Configurações de execução (e.g., launchSettings.json)
├─ appsettings.json                   # Configuração geral da aplicação (inclui MongoDB e JWT)
├─ appsettings.Development.json       # Configurações específicas para ambiente de desenvolvimento
├─ Program.cs                         # Ponto de entrada da aplicação: configuração de serviços (DI), autenticação, MongoDB
├─ CadastroAcoes.csproj               # Arquivo de projeto C#
├─ CadastroAcoes.sln                  # Arquivo de solução Visual Studio
├─ .gitignore                         # Regras para ignorar arquivos no controle de versão
├─ package.json                       # Dependências e scripts do frontend (se houver)
├─ package-lock.json                  # Bloqueio de versões de dependências do frontend
├─ postcss.config.cjs                 # Configuração do PostCSS (para Tailwind CSS)
├─ tailwind.config.cjs                # Configuração do Tailwind CSS
└─ README.md                          # Este arquivo
```

## Configuração e Instalação

### Pré-requisitos
*   .NET 8 SDK
*   MongoDB (servidor em execução, preferencialmente em `localhost:27017`)
*   Node.js e npm (para ferramentas de frontend como Tailwind CSS, se necessário)

### Passos para Rodar (Desenvolvimento)

1.  **Clonar o Repositório:**
    ```bash
    git clone https://github.com/natamaia/projeto-faculdade.git
    cd projeto-faculdade/CadastroAcoes
    ```

2.  **Configurar o Banco de Dados:**
    *   Certifique-se de que o servidor MongoDB esteja em execução. A aplicação está configurada para se conectar a `mongodb://localhost:27017` no banco de dados `CadastroAcoesDb`.

3.  **Configurar JWT:**
    *   As configurações JWT (Key, Issuer, Audience, ExpiresMinutes) estão definidas em `appsettings.json`. A chave (`Jwt:Key`) foi atualizada para garantir um tamanho mínimo de 256 bits.

4.  **Instalar Dependências do Frontend (se aplicável):**
    *   Se houver dependências de frontend (e.g., para Tailwind CSS), execute:
        ```bash
        npm install
        ```

5.  **Executar a Aplicação:**
    *   No terminal, na pasta raiz do projeto (`CadastroAcoes/`), execute:
        ```powershell
        dotnet run
        ```
    *   A aplicação estará acessível em `http://localhost:5000` (ou na porta configurada em `Properties/launchSettings.json`).

## API Endpoints Principais

*   `/api/auth/register`: Registro de novos usuários.
*   `/api/auth/login`: Autenticação de usuários e geração de JWT.
*   `/api/clients`: Gerenciamento de perfis de clientes.
*   `/api/vendors`: Gerenciamento de perfis de vendedores.
*   `/api/products`: Gerenciamento de produtos.

## Alterações Recentes

*   **Remoção da dependência `.env`**: As configurações de conexão do MongoDB e JWT foram movidas para `Program.cs` e `appsettings.json`, respectivamente. O arquivo `.env` foi removido.
*   **Correção da Chave JWT**: A chave `Jwt:Key` em `appsettings.json` foi estendida para garantir um tamanho mínimo de 256 bits, resolvendo o erro de autenticação `IDX10720`.
*   **Remoção de Campos de Cadastro**:
    *   O campo `CEP` foi removido do formulário de cadastro de vendedor no frontend (`wwwroot/layouts/Pages/register.html`) e de qualquer lógica JavaScript associada (`wwwroot/assets/register.js`).
    *   O campo `Idade` foi removido do formulário de cadastro de cliente no frontend (`wwwroot/layouts/Pages/register.html`), da lógica JavaScript associada (`wwwroot/assets/register.js`), do modelo `Model/UserClient.cs` e do DTO/lógica no `Controller/ClientsController.cs`.
*   **Melhoria na Geração de JWT**: O método `GenerateToken` em `Controller/AuthController.cs` foi aprimorado para incluir `ClaimTypes.NameIdentifier` (com o ID do usuário) no token, tornando-o mais robusto.

---
