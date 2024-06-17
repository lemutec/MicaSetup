using System;

namespace MicaSetup.Attributes;

[AttributeUsage(AttributeTargets.All)]
internal sealed class StableAttribute(bool isStabled) : Attribute
{
    public bool IsStabled { get; } = isStabled;
}
