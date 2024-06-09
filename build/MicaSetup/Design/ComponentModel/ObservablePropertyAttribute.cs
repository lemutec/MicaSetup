using System;

namespace MicaSetup.Design.ComponentModel;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public sealed class ObservablePropertyAttribute : Attribute
{
}
