namespace Tradify.Core;

public sealed record User(string Id, string Username, string Name, UserRole Role, string CompanyId)
{
    public bool IsStaff => Role is UserRole.Admin or UserRole.FieldWorker;
}
