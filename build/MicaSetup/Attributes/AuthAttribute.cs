using System;

namespace MicaSetup.Attributes;

[AttributeUsage(AttributeTargets.All)]
internal sealed class AuthAttribute(Auth requiredAuth) : Attribute
{
    public Auth RequiredAuth { get; } = requiredAuth;
}

[Flags]
internal enum Auth
{
    User = 0b0000,
    Admin = 0b0001,
    Unknown = 0b0010,
}
