using System.ComponentModel;

namespace CasaDaRosa.Domain.Entities.Users;

public enum UserRole
{
    [Description("Customer")]
    Customer = 1,

    [Description("Admin")]
    Admin = 2
}
