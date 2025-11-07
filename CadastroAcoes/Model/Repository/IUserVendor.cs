using System;

namespace Model
{
    /// <summary>
    /// Contrato (interface) que descreve as propriedades de um UserVendor.
    /// Usada para depender de abstrações em vez de classes concretas.
    /// </summary>
    public interface IUserVendor
    {
        string? Id { get; set; }
        string Username { get; set; }
        int Cnpj { get; set; }
        string Email { get; set; }
        int PhoneNumber { get; set; }
        string Empresa { get; set; }
        string PasswordHash { get; set; }
        DateTime CreatedAt { get; set; }
    }
}
