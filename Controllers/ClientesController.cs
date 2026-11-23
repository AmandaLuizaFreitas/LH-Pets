using LHPets.Api.Data;
using LHPets.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace LHPets.Api.Controllers;

/// <summary>
/// Cadastro e listagem de clientes da LH-Pets.
/// </summary>
[ApiController]
[Route("api/[controller]")] // => /api/clientes
public class ClientesController : ControllerBase
{
    private readonly ArmazenamentoEmMemoria _armazenamento;

    // O serviço Singleton é injetado pelo ASP.NET Core via construtor.
    public ClientesController(ArmazenamentoEmMemoria armazenamento)
    {
        _armazenamento = armazenamento;
    }

    /// <summary>
    /// Retorna a lista completa de clientes.
    /// </summary>
    /// <response code="200">Lista de clientes (vazia se ainda não houver cadastros).</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<Cliente>), StatusCodes.Status200OK)]
    public ActionResult<List<Cliente>> Get()
    {
        return Ok(_armazenamento.ListarClientes());
    }

    /// <summary>
    /// Cadastra um novo cliente na lista em memória.
    /// </summary>
    /// <remarks>
    /// O Id é gerado automaticamente. O CPF é salvo apenas com números e o e-mail em minúsculas.
    /// </remarks>
    /// <response code="201">Cliente criado com sucesso.</response>
    /// <response code="400">Dados inválidos (validação do modelo).</response>
    /// <response code="409">Já existe um cliente com este CPF.</response>
    [HttpPost]
    [ProducesResponseType(typeof(Cliente), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public ActionResult<Cliente> Post([FromBody] Cliente cliente)
    {
        // Conversão/normalização dos dados antes de salvar
        cliente.Nome = cliente.Nome.Trim();
        cliente.Cpf = new string(cliente.Cpf.Where(char.IsDigit).ToArray()); // só dígitos
        cliente.Email = cliente.Email.Trim().ToLowerInvariant();

        if (!_armazenamento.TentarAdicionarCliente(cliente))
        {
            return Conflict(new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "CPF já cadastrado.",
                Detail = $"Já existe um cliente com o CPF {cliente.Cpf}."
            });
        }

        return StatusCode(StatusCodes.Status201Created, cliente);
    }
}
