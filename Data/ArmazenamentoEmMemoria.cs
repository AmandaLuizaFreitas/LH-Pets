using LHPets.Api.Models;

namespace LHPets.Api.Data;

/// <summary>
/// Guarda os dados em memória usando List&lt;T&gt; (sem banco de dados).
/// </summary>
public class ArmazenamentoEmMemoria
{
    private readonly List<Cliente> _clientes = new();
    private readonly List<Pet> _pets = new();

    // Requisições HTTP podem ser simultâneas; o lock evita corrupção das listas e Ids duplicados.
    private readonly object _trava = new();

    private int _proximoClienteId = 1;
    private int _proximoPetId = 1;

    // ---------- Clientes ----------

    public List<Cliente> ListarClientes()
    {
        lock (_trava)
        {
            return _clientes.ToList(); // devolve uma cópia da lista
        }
    }

    public bool ClienteExiste(int clienteId)
    {
        lock (_trava)
        {
            return _clientes.Any(c => c.Id == clienteId);
        }
    }

    /// <summary>
    /// Adiciona o cliente gerando o Id sequencial.
    /// Retorna false (e não adiciona) se já existir cliente com o mesmo CPF.
    /// </summary>
    public bool TentarAdicionarCliente(Cliente cliente)
    {
        lock (_trava)
        {
            if (_clientes.Any(c => c.Cpf == cliente.Cpf))
                return false;

            cliente.Id = _proximoClienteId++;
            _clientes.Add(cliente);
            return true;
        }
    }

    // ---------- Pets ----------

    public List<Pet> ListarPets()
    {
        lock (_trava)
        {
            return _pets.ToList();
        }
    }

    public List<Pet> ListarPetsPorCliente(int clienteId)
    {
        lock (_trava)
        {
            return _pets.Where(p => p.ClienteId == clienteId).ToList();
        }
    }

    /// <summary>
    /// Adiciona o pet gerando o Id sequencial.
    /// Retorna false (e não adiciona) se o ClienteId informado não existir.
    /// </summary>
    public bool TentarAdicionarPet(Pet pet)
    {
        lock (_trava)
        {
            if (!_clientes.Any(c => c.Id == pet.ClienteId))
                return false;

            pet.Id = _proximoPetId++;
            _pets.Add(pet);
            return true;
        }
    }
}
