using System;
using System.Reflection;

namespace MicaSetup.Attributes;

[AttributeUsage(AttributeTargets.Assembly)]
public sealed class RequestExecutionLevelAttribute(string level = RequestExecutionLevelAttribute.None) : Attribute
{
    public const string None = "none";
    public const string User = "user";
    public const string Admin = "admin";

    public string Level { get; } = level;

    public bool IsUser => Level.Equals(User, StringComparison.OrdinalIgnoreCase);
    public bool IsAdmin => Level.Equals(Admin, StringComparison.OrdinalIgnoreCase);
    public static bool IsRequestAdmin => ((RequestExecutionLevelAttribute)typeof(RequestExecutionLevelAttribute).Assembly.GetCustomAttribute(typeof(RequestExecutionLevelAttribute))).IsAdmin;
}
