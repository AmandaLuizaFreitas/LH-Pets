using System.ComponentModel.DataAnnotations;

namespace LHPets.Api.Models;

/// <summary>
/// Cliente da LH-Pets (dono de um ou mais pets).
/// </summary>
public class Cliente
{
    /// <summary>Identificador único.</summary>
    public int Id { get; set; }

    /// <summary>Nome completo do cliente.</summary>
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 100 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    /// <summary>CPF com 11 dígitos, com ou sem pontuação.</summary>
    [Required(ErrorMessage = "O CPF é obrigatório.")]
    [RegularExpression(@"^\d{3}\.?\d{3}\.?\d{3}-?\d{2}$",
        ErrorMessage = "CPF inválido. Use 11 dígitos, com ou sem pontuação.")]
    public string Cpf { get; set; } = string.Empty;

    /// <summary>E-mail de contato.</summary>
    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "E-mail inválido.")]
    [StringLength(150, ErrorMessage = "O e-mail deve ter no máximo 150 caracteres.")]
    public string Email { get; set; } = string.Empty;
}
