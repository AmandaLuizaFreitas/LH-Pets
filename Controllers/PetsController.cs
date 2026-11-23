using LHPets.Api.Data;
using LHPets.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace LHPets.Api.Controllers;

/// <summary>
/// Cadastro e listagem de pets da LH-Pets.
/// </summary>
[ApiController]
[Route("api/[controller]")] // => /api/pets
public class PetsController : ControllerBase
{
    private readonly ArmazenamentoEmMemoria _armazenamento;

    public PetsController(ArmazenamentoEmMemoria armazenamento)
    {
        _armazenamento = armazenamento;
    }

    /// <summary>
    /// Retorna todos os pets cadastrados.
    /// </summary>
    /// <response code="200">Lista de pets (vazia se ainda não houver cadastros).</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<Pet>), StatusCodes.Status200OK)]
    public ActionResult<List<Pet>> Get()
    {
        return Ok(_armazenamento.ListarPets());
    }

    /// <summary>
    /// Cadastra um novo pet vinculado a um cliente existente (ClienteId).
    /// </summary>
    /// <response code="201">Pet criado com sucesso.</response>
    /// <response code="400">Dados inválidos (validação do modelo).</response>
    /// <response code="404">O ClienteId informado não existe.</response>
    [HttpPost]
    [ProducesResponseType(typeof(Pet), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public ActionResult<Pet> Post([FromBody] Pet pet)
    {
        pet.Nome = pet.Nome.Trim();
        pet.Especie = pet.Especie.Trim();
        pet.Raca = pet.Raca.Trim();

        if (!_armazenamento.TentarAdicionarPet(pet))
        {
            return NotFound(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Cliente não encontrado.",
                Detail = $"Não existe cliente com Id {pet.ClienteId}. Cadastre o cliente antes de cadastrar o pet."
            });
        }

        return StatusCode(StatusCodes.Status201Created, pet);
    }

    /// <summary>
    /// Retorna apenas os pets pertencentes ao cliente informado.
    /// </summary>
    /// <param name="clienteId">Id do cliente.</param>
    /// <response code="200">Pets do cliente (lista vazia se ele não tiver pets).</response>
    /// <response code="404">O cliente informado não existe.</response>
    [HttpGet("cliente/{clienteId:int}")] // => /api/pets/cliente/{clienteId}
    [ProducesResponseType(typeof(List<Pet>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public ActionResult<List<Pet>> GetPorCliente(int clienteId)
    {
        if (!_armazenamento.ClienteExiste(clienteId))
        {
            return NotFound(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Cliente não encontrado.",
                Detail = $"Não existe cliente com Id {clienteId}."
            });
        }

        return Ok(_armazenamento.ListarPetsPorCliente(clienteId));
    }
}
