using System;

namespace Model
{
    /// <summary>
    /// Contrato (interface) que descreve as propriedades de um UserClient.
    /// Usada para depender de abstrações em vez de classes concretas.
    /// </summary>
    public interface IUserClient
    {
        string? Id { get; set; }
        string Username { get; set; }
        int Cfp { get; set; }
        string Email { get; set; }
        string Address { get; set; }
        int PhoneNumber { get; set; }
        int Cep { get; set; }
        string PasswordHash { get; set; }
        DateTime CreatedAt { get; set; }
    }
}
