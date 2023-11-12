# Icon Sets

## IcoMoon Free

https://github.com/Keyamoon/IcoMoon-Free

## HarmonyOS Icons

https://developer.harmonyos.com/cn/design/harmonyos-icon/

# IcoMoon CLI

> create.cmd

**Requests**

```bash
npm i -g icomoon-cli
```

Add svg icon to [icomoon app project](https://icomoon.io/app).

**Executes**

```bash
icomoon-cli -i 1.svg -s selection.json -n 1 -o output
```

> fix#29.cmd

Use [fix#29.cmd](fix#29.cmd) to fix [icomoon-cli](https://github.com/Yuyz0112/icomoon-cli) [#29](https://github.com/Yuyz0112/icomoon-cli/issues/29).

> overlay.cmd

Use it to cover `selection.json` to save the project.

# IcoMoon Gen

> selection.cmd

Convert from `selection.json` to `Selection.cs`.

> Selection.cs

```cs
var sb = new System.Text.StringBuilder();
string json = File.ReadAllText("selection.json");
var jsonObj = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(json)!;
var iconsObj = System.Text.Json.JsonSerializer.Deserialize<List<object>>(jsonObj["icons"].ToString()!)!;
sb.Append("namespace MicaSetup.Design.Controls;\r\n\r\n");
sb.Append("public static class Selection\r\n");
sb.Append("{\r\n");
foreach (object iconObj in iconsObj) {
    var icon = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(iconObj.ToString()!)!;
    var propertiesObj = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, dynamic>>(icon["properties"].ToString()!)!;
    string name = propertiesObj["name"].ToString();
    int code = (int)propertiesObj["code"].GetInt32();
    sb.Append($"    public const string {name} = \"{@"\x" + code.ToString("x4")}\";\r\n");
}
sb.Append("}\r\n");
File.WriteAllText("Selection.cs", sb.ToString());

```

> selection.json

```
{
  "height": 1024,
  "IcoMoonType": "selection",
  "icons": [
    {
      "icon": {
        "paths": [
          "M42.667 512c0 259.206 210.128 469.333 469.333 469.333s469.333-210.128 469.333-469.333v-0c0-259.206-210.128-469.333-469.333-469.333s-469.333 210.128-469.333 469.333h0z"
        ],
        "attrs": [
          {}
        ],
        "isMulticolor": false,
        "isMulticolor2": false,
        "grid": 0,
        "tags": [
          "Circle"
        ]
      },
      "attrs": [
        {}
      ],
      "properties": {
        "order": 3,
        "id": 1,
        "name": "Circle",
        "prevSize": 32,
        "code": 59649
      },
      "setIdx": 0,
      "setId": 0,
      "iconIdx": 0
    },
    {
      "icon": {
        "paths": [
          "M259.52 512c0-65.408 53.056-118.464 118.464-118.464s118.464 53.056 118.464 118.464c0 65.408-53.056 118.464-118.464 118.464s-118.464-53.056-118.464-118.464zM512 0c-282.752 0-512 229.248-512 512s229.248 512 512 512c282.752 0 512-229.248 512-512s-229.248-512-512-512zM379.392 959.296c-153.984-89.6-257.472-256.32-257.472-447.296s103.488-357.696 257.472-447.296c153.984 89.536 257.6 256.32 257.6 447.296s-103.552 357.696-257.6 447.296z"
        ],
        "attrs": [
          {}
        ],
        "isMulticolor": false,
        "isMulticolor2": false,
        "grid": 0,
        "tags": [
          "IcoMoon"
        ]
      },
      "attrs": [
        {}
      ],
      "properties": {
        "order": 2,
        "id": 0,
        "name": "IcoMoon",
        "prevSize": 32,
        "code": 59648
      },
      "setIdx": 0,
      "setId": 0,
      "iconIdx": 1
    }
  ],
  "metadata": {
    "name": "icomoon"
  },
  "preferences": {
    "fontPref": {
      "embed": false,
      "metadata": {
        "fontFamily": "icomoon"
      },
      "metrics": {
        "baseline": 6.25,
        "emSize": 1024,
        "whitespace": 50
      },
      "prefix": "icon-"
    },
    "historySize": 50,
    "imagePref": {
      "bgColor": 16777215,
      "color": 0,
      "png": true,
      "prefix": "icon-",
      "useClassSelector": true
    },
    "showCodes": true,
    "showGlyphs": true,
    "showQuickUse": true,
    "showQuickUse2": true,
    "showSVGs": true
  }
}
```

# Renamer

> renamer.cmd

**Requests**

```bash
dotnet tool update -g dotnet-execute
```

**Executes**

```bash
dotnet-exec Renamer.cs
```

```cs
DirectoryInfo directoryInfo = new(".");
FileInfo[] svgFiles = directoryInfo.GetFiles("*.svg");

foreach (FileInfo svgFile in svgFiles)
{
    string originalFileName = Path.GetFileNameWithoutExtension(svgFile.Name);
    string[] words = originalFileName.Split('_');
    string pascalCaseName = string.Empty;
    foreach (string word in words)
    {
        if (word.Length > 0)
        {
            pascalCaseName += word.Substring(0, 1).ToUpper() + word.Substring(1).ToLower();
        }
    }
    if (originalFileName == pascalCaseName)
    {
        continue;
    }
    if (char.IsUpper(originalFileName[0]))
    {
        pascalCaseName = char.ToUpper(pascalCaseName[0]) + pascalCaseName.Substring(1);
    }
    string newFileName = pascalCaseName + ".svg";
    string newFilePath = Path.Combine(svgFile.DirectoryName, newFileName);
    svgFile.MoveTo(newFilePath);
}
Console.WriteLine("Rename completed.");
```


