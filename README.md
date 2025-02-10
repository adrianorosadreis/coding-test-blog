# Visão Geral do Projeto

Este projeto é um blog simples implementado com o framework .NET 6+, utilizando ASP.NET Core MVC, SignalR para notificações em tempo real, Identity para autenticação e SQL Server como banco de dados. A aplicação permite que usuários criem, editem e excluam postagens, com funcionalidades de login e gerenciamento de usuários. Além disso, implementamos notificações em tempo real, enviadas aos usuários conectados via SignalR, sempre que uma nova postagem for criada.

## Configuração do SignalR

O SignalR é utilizado para fornecer atualizações em tempo real para todos os usuários conectados sempre que uma nova postagem é criada. O fluxo de comunicação acontece da seguinte maneira:

### Backend (Server-Side)

O SignalR é configurado no arquivo `Program.cs`, onde é adicionado o serviço `AddSignalR()` e o endpoint `/postHub` é mapeado para o `PostHub`.

### Frontend (Client-Side)

No lado do cliente, utilizamos o `HubConnectionBuilder` para conectar o navegador ao hub de SignalR. Ao criar uma nova postagem, o backend chama o método `SendNewPostNotification` no SignalR Hub, notificando todos os clientes conectados.

## Configuração do Identity

O Identity foi utilizado para a autenticação e gerenciamento de usuários. Durante o processo de login, a autenticação é gerida pelo `UserManager` e `SignInManager`, fornecendo suporte para autenticação padrão com e-mail e senha.

A base de dados do Identity é configurada no `ApplicationDbContext`, que é o contexto principal para o gerenciamento de dados de usuários e postagens. O `AddDefaultIdentity<User>` configura o Identity para suportar as funções básicas de login e registro de usuários. O modelo de usuário personalizado, `User`, pode ser expandido conforme necessário.

## Comunicação Backend e Frontend

O backend foi desenvolvido com ASP.NET Core MVC, onde as views são manipuladas no controlador e passadas para o frontend. A comunicação entre o backend e frontend ocorre por meio de:

### Formulários

O frontend usa formulários HTML simples para capturar dados como título e conteúdo das postagens, enquanto o backend lida com a lógica de salvar no banco de dados e gerar notificações via SignalR.

### SignalR

A comunicação em tempo real para notificações de novas postagens é gerenciada via WebSockets, configurada no `Program.cs` para enviar atualizações aos clientes.

## Dependências

Este projeto utiliza as seguintes dependências:

- `Microsoft.AspNetCore.SignalR`: Para comunicação em tempo real entre o servidor e o cliente.
- `Microsoft.EntityFrameworkCore.SqlServer`: Para interação com o banco de dados SQL Server.
- `Microsoft.AspNetCore.Identity.EntityFrameworkCore`: Para gerenciar autenticação e autorização de usuários.
- `Microsoft.AspNetCore.Mvc`: Para renderização de views e manipulação de controladores.

## Instruções de Configuração

### 1. Configuração do Ambiente Local

#### **Pré-requisitos**

Certifique-se de ter o seguinte instalado:

- .NET SDK (versão 6 ou superior): [Download](https://dotnet.microsoft.com/en-us/download)
- SQL Server (ou SQL Server Express): [Download](https://www.microsoft.com/pt-br/sql-server/sql-server-downloads)
- Visual Studio 2022 (ou VS Code com C# extension): [Download Visual Studio](https://visualstudio.microsoft.com/)

#### **Configuração do Banco de Dados**

No `appsettings.json`, configure a string de conexão para o SQL Server:

```json
"ConnectionStrings": {
   "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=SimpleBlogDb;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

Caso deseje utilizar uma instância do SQL Server diferente, altere o valor de `DefaultConnection`. Em seguida, crie o banco de dados e as tabelas com o comando:

```bash
dotnet ef database update
```

#### **Configuração do Identity**

Se você estiver utilizando uma configuração personalizada de `User` (como no projeto), o `ApplicationDbContext` já estará configurado para usar o modelo de usuário extendido.

O projeto já inclui as migrações necessárias para o banco de dados de autenticação, então execute o seguinte para garantir que todas as migrações sejam aplicadas corretamente:

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

#### **Configuração do SignalR**

O SignalR já está configurado automaticamente no `Program.cs`, portanto, não há necessidade de realizar nenhuma configuração adicional além da conexão no frontend:

##### **Frontend (JavaScript)**

No arquivo `site.js`, você pode verificar a conexão do SignalR com o servidor, como mostrado abaixo:

```js
var connection = new signalR.HubConnectionBuilder()
    .withUrl("/postHub")
    .build();
```

##### **Backend (C#)**

O SignalR já foi configurado no backend dentro do `Program.cs`:

```csharp
app.MapHub<PostHub>("/postHub");
```

### 2. Rodando o Projeto Localmente

Certifique-se de que todas as dependências estejam instaladas e o banco de dados esteja configurado corretamente.

Para rodar o projeto, execute o seguinte comando na raiz do projeto:

```bash
dotnet run
```

O servidor será iniciado e poderá ser acessado via `http://localhost:5000`. Caso esteja configurando o ambiente de produção, altere a URL para a configuração apropriada.


