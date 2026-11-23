using System.ComponentModel.DataAnnotations;

namespace LHPets.Api.Models;

/// <summary>
/// Pet cadastrado 
/// </summary>
public class Pet
{
    /// <summary>Identificador único.</summary>
    public int Id { get; set; }

    /// <summary>Nome do pet.</summary>
    [Required(ErrorMessage = "O nome do pet é obrigatório.")]
    [StringLength(60, MinimumLength = 1, ErrorMessage = "O nome do pet deve ter entre 1 e 60 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    /// <summary>Espécie do pet (ex.: Cachorro, Gato).</summary>
    [Required(ErrorMessage = "A espécie é obrigatória.")]
    [StringLength(40, ErrorMessage = "A espécie deve ter no máximo 40 caracteres.")]
    public string Especie { get; set; } = string.Empty;

    /// <summary>Raça do pet (ex.: Labrador, Siamês).</summary>
    [Required(ErrorMessage = "A raça é obrigatória.")]
    [StringLength(60, ErrorMessage = "A raça deve ter no máximo 60 caracteres.")]
    public string Raca { get; set; } = string.Empty;

    /// <summary>Id do cliente dono do pet. Deve existir previamente.</summary>
    [Range(1, int.MaxValue, ErrorMessage = "ClienteId deve ser um número maior que zero.")]
    public int ClienteId { get; set; }
}
