using System;

namespace MicaSetup.Design.Commands;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public class RelayCommandAttribute : Attribute
{
}
