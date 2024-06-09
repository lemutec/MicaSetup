using System;

namespace MicaSetup.Design.ComponentModel;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class INotifyPropertyChangedAttribute : Attribute
{
    public bool IncludeAdditionalHelperMethods { get; init; } = true;
}
