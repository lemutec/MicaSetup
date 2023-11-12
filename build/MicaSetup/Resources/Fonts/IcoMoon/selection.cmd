@REM @echo off
echo ^
StringBuilder sb = new(); ^
string json = File.ReadAllText("selection.json"); ^
var jsonObj = System.Text.Json.JsonSerializer.Deserialize^<Dictionary^<string, object^>^>(json)!; ^
var iconsObj = System.Text.Json.JsonSerializer.Deserialize^<List^<object^>^>(jsonObj["icons"].ToString()!)!; ^
sb.Append("namespace MicaSetup.Design.Controls;\r\n\r\n"); ^
sb.Append("public static class Selection\r\n"); ^
sb.Append("{\r\n"); ^
foreach (object iconObj in iconsObj) { ^
    var icon = System.Text.Json.JsonSerializer.Deserialize^<Dictionary^<string, object^>^>(iconObj.ToString()!)!; ^
    var propertiesObj = System.Text.Json.JsonSerializer.Deserialize^<Dictionary^<string, dynamic^>^>(icon["properties"].ToString()!)!; ^
    string name = propertiesObj["name"].ToString(); ^
    int code = (int)propertiesObj["code"].GetInt32(); ^
    sb.Append($"    public const string {name} = \"{@"\x" + code.ToString("x4")}\";\r\n"); ^
} ^
sb.Append("}\r\n"); ^
File.WriteAllText("Selection.cs", sb.ToString()); > Selection.cs
dotnet-exec Selection.cs
copy /y Selection.cs ..\..\..\Controls\Styles\
del Selection.cs
@pause
