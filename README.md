# LH-Pets — Web API (ASP.NET Core)

Web API em **C# / ASP.NET Core** (padrão **Controllers**) para o cadastro e a listagem de **Clientes** e **Pets** da empresa LH-Pets.

> Protótipo: **não usa banco de dados**. Todos os dados ficam em memória (`List<T>`) e são perdidos ao reiniciar a aplicação.
## Swagger
<img width="1478" height="542" alt="Captura de tela de 2026-11-24 11-20-20" src="https://github.com/user-attachments/assets/735d5c01-b467-458f-a066-7c9698464ce5" />
### Cadastro de cliente (POST /api/Clientes)
<img width="1478" height="542" alt="Captura de tela de 2026-11-24 11-21-05" src="https://github.com/user-attachments/assets/4479b6e4-c22b-4e55-ad5c-5bbe23b1d20a" />

---

## Tecnologias

- C# e .NET 8 (ASP.NET Core Web API com Controllers)
- Swagger / OpenAPI (Swashbuckle.AspNetCore) para documentar e testar
- Armazenamento em memória com `List<T>` (serviço Singleton)

## Estrutura do projeto

```
LHPets.Api/
├── Controllers/
│   ├── ClientesController.cs   # GET e POST /api/clientes
│   └── PetsController.cs       # GET, POST /api/pets e GET /api/pets/cliente/{clienteId}
├── Data/
│   └── ArmazenamentoEmMemoria.cs   # List<Cliente> e List<Pet> compartilhadas entre os controllers
├── Models/
│   ├── Cliente.cs              # Id, Nome, Cpf, Email
│   └── Pet.cs                  # Id, Nome, Especie, Raca, ClienteId
├── Properties/launchSettings.json
├── Program.cs                  # Configuração (Controllers, DI, Swagger)
└── LHPets.Api.csproj
```

## Como executar

Pré-requisitos: [.NET SDK 8](https://dotnet.microsoft.com/download) e [Git](https://git-scm.com/).

```bash
git clone <URL-DO-SEU-REPOSITORIO>
cd <pasta-do-repositorio>/LHPets.Api   # entre na pasta que contém o LHPets.Api.csproj
dotnet run
```

A API sobe em **http://localhost:5080** e o Swagger abre em **http://localhost:5080/swagger**
(a raiz `/` também redireciona para o Swagger).

> Se você usa uma versão mais nova do SDK (ex.: .NET 10) e quer rodar sem instalar o .NET 8,
> altere `<TargetFramework>net8.0</TargetFramework>` no `LHPets.Api.csproj` para a sua versão
> (ex.: `net10.0`) e rode `dotnet run` novamente.

## Endpoints

| Método | Rota                              | Descrição                                   | Respostas          |
|--------|-----------------------------------|---------------------------------------------|--------------------|
| GET    | `/api/clientes`                   | Lista todos os clientes                     | 200                |
| POST   | `/api/clientes`                   | Cadastra um cliente                         | 201, 400, 409      |
| GET    | `/api/pets`                       | Lista todos os pets                         | 200                |
| POST   | `/api/pets`                       | Cadastra um pet vinculado a um `ClienteId`  | 201, 400, 404      |
| GET    | `/api/pets/cliente/{clienteId}`   | Lista os pets de um cliente                 | 200, 404           |

### Regras e validações

- O `Id` é gerado automaticamente (sequencial); o valor enviado no corpo é ignorado.
- `Nome`, `Cpf` e `Email` do cliente são obrigatórios; o e-mail precisa ser válido.
- O CPF aceita `12345678909` ou `123.456.789-09` e é salvo **somente com números**; o e-mail é salvo em **minúsculas**.
- Não é possível cadastrar dois clientes com o mesmo CPF (**409 Conflict**).
- Ao cadastrar um pet, o `ClienteId` precisa existir (senão **404 Not Found**).
- Erros de validação retornam **400 Bad Request** com a lista de mensagens por campo.

### Exemplos

**POST /api/clientes**

```json
{
  "nome": "Maria Silva",
  "cpf": "123.456.789-09",
  "email": "maria@email.com"
}
```

Resposta `201 Created`:

```json
{ "id": 1, "nome": "Maria Silva", "cpf": "12345678909", "email": "maria@email.com" }
```

**POST /api/pets**

```json
{
  "nome": "Rex",
  "especie": "Cachorro",
  "raca": "Labrador",
  "clienteId": 1
}
```

Resposta `201 Created`:

```json
{ "id": 1, "nome": "Rex", "especie": "Cachorro", "raca": "Labrador", "clienteId": 1 }
```

**POST /api/pets** com `ClienteId` inexistente — `404 Not Found`:

```json
{
  "title": "Cliente não encontrado.",
  "status": 404,
  "detail": "Não existe cliente com Id 99. Cadastre o cliente antes de cadastrar o pet."
}
```

## Roteiro de testes no Swagger

Abra `/swagger`, clique em cada endpoint, **Try it out**, preencha e **Execute**.

| # | Ação | Corpo / parâmetro | Resultado esperado |
|---|------|-------------------|--------------------|
| 1 | `POST /api/clientes` | Maria Silva, CPF `123.456.789-09`, `MARIA@Email.com` | 201 — `id: 1`, CPF só números, e-mail em minúsculas |
| 2 | `POST /api/clientes` | João Souza, CPF `98765432100`, `joao@email.com` | 201 — `id: 2` |
| 3 | `POST /api/clientes` | mesmo CPF da Maria | 409 — CPF já cadastrado |
| 4 | `POST /api/clientes` | nome vazio, CPF `123`, e-mail inválido | 400 — mensagens de validação |
| 5 | `GET /api/clientes` | — | 200 — lista com os 2 clientes |
| 6 | `POST /api/pets` | Rex, Cachorro, Labrador, `clienteId: 1` | 201 — `id: 1` |
| 7 | `POST /api/pets` | Mia, Gato, Siamês, `clienteId: 2` | 201 — `id: 2` |
| 8 | `POST /api/pets` | Thor, Cachorro, Poodle, `clienteId: 1` | 201 — `id: 3` |
| 9 | `POST /api/pets` | qualquer pet com `clienteId: 99` | 404 — cliente não encontrado |
| 10 | `GET /api/pets` | — | 200 — lista com os 3 pets |
| 11 | `GET /api/pets/cliente/{clienteId}` | `1` | 200 — Rex e Thor |
| 12 | `GET /api/pets/cliente/{clienteId}` | `2` | 200 — Mia |
| 13 | `GET /api/pets/cliente/{clienteId}` | `99` | 404 — cliente não encontrado |

