using System.Reflection;
using System.Reflection.Emit;

namespace MakeMui.Core;

internal static class DynamicClassGenerator
{
    public static Type GenerateClass(params string[] names)
    {
        AssemblyName assemblyName = new("DynamicAssembly");
        AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
        ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("DynamicModule");
        TypeBuilder typeBuilder = moduleBuilder.DefineType("DynamicClass", TypeAttributes.Public);

        // Key
        FieldBuilder keyField = typeBuilder.DefineField("_key", typeof(string), FieldAttributes.Private);
        PropertyBuilder keyProperty = typeBuilder.DefineProperty("Key", PropertyAttributes.None, typeof(string), null);
        MethodBuilder keyGet = typeBuilder.DefineMethod("get_Key", MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, typeof(string), Type.EmptyTypes);
        ILGenerator keyGetIL = keyGet.GetILGenerator();
        keyGetIL.Emit(OpCodes.Ldarg_0);
        keyGetIL.Emit(OpCodes.Ldfld, keyField);
        keyGetIL.Emit(OpCodes.Ret);
        keyProperty.SetGetMethod(keyGet);
        MethodBuilder keySet = typeBuilder.DefineMethod("set_Key", MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, null, [typeof(string)]);
        ILGenerator keySetIL = keySet.GetILGenerator();
        keySetIL.Emit(OpCodes.Ldarg_0);
        keySetIL.Emit(OpCodes.Ldarg_1);
        keySetIL.Emit(OpCodes.Stfld, keyField);
        keySetIL.Emit(OpCodes.Ret);
        keyProperty.SetSetMethod(keySet);

        // Order
        FieldBuilder orderField = typeBuilder.DefineField("_order", typeof(string), FieldAttributes.Private);
        PropertyBuilder orderProperty = typeBuilder.DefineProperty("Order", PropertyAttributes.None, typeof(string), null);
        MethodBuilder orderGet = typeBuilder.DefineMethod("get_Order", MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, typeof(string), Type.EmptyTypes);
        ILGenerator orderGetIL = orderGet.GetILGenerator();
        orderGetIL.Emit(OpCodes.Ldarg_0);
        orderGetIL.Emit(OpCodes.Ldfld, orderField);
        orderGetIL.Emit(OpCodes.Ret);
        orderProperty.SetGetMethod(orderGet);
        MethodBuilder orderSet = typeBuilder.DefineMethod("set_Order", MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, null, [typeof(string)]);
        ILGenerator orderSetIL = orderSet.GetILGenerator();
        orderSetIL.Emit(OpCodes.Ldarg_0);
        orderSetIL.Emit(OpCodes.Ldarg_1);
        orderSetIL.Emit(OpCodes.Stfld, orderField);
        orderSetIL.Emit(OpCodes.Ret);
        orderProperty.SetSetMethod(orderSet);

        foreach (string name in names)
        {
            FieldBuilder nameField = typeBuilder.DefineField($"_{name}", typeof(string), FieldAttributes.Private);
            PropertyBuilder nameProperty = typeBuilder.DefineProperty(name, PropertyAttributes.None, typeof(string), null);
            MethodBuilder nameGet = typeBuilder.DefineMethod($"get_{name}", MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, typeof(string), Type.EmptyTypes);
            ILGenerator nameGetIL = nameGet.GetILGenerator();
            nameGetIL.Emit(OpCodes.Ldarg_0);
            nameGetIL.Emit(OpCodes.Ldfld, nameField);
            nameGetIL.Emit(OpCodes.Ret);
            nameProperty.SetGetMethod(nameGet);
            MethodBuilder nameSet = typeBuilder.DefineMethod($"set_{name}", MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, null, [typeof(string)]);
            ILGenerator nameSetIL = nameSet.GetILGenerator();
            nameSetIL.Emit(OpCodes.Ldarg_0);
            nameSetIL.Emit(OpCodes.Ldarg_1);
            nameSetIL.Emit(OpCodes.Stfld, nameField);
            nameSetIL.Emit(OpCodes.Ret);
            nameProperty.SetSetMethod(nameSet);
        }

        Type dynamicType = typeBuilder.CreateType();

        return dynamicType;
    }
}
