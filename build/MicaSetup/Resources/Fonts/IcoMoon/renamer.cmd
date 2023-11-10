@echo off
cd svgs
echo DirectoryInfo directoryInfo = new("."); >> Renamer.cs
echo FileInfo[] svgFiles = directoryInfo.GetFiles("*.svg"); >> Renamer.cs
echo. >> Renamer.cs
echo foreach (FileInfo svgFile in svgFiles) >> Renamer.cs
echo { >> Renamer.cs
echo    string originalFileName = Path.GetFileNameWithoutExtension(svgFile.Name); >> Renamer.cs
echo    string[] words = originalFileName.Split('_'); >> Renamer.cs
echo    string pascalCaseName = string.Empty; >> Renamer.cs
echo    foreach (string word in words) >> Renamer.cs
echo    { >> Renamer.cs
echo        if (word.Length ^> 0) >> Renamer.cs
echo        { >> Renamer.cs
echo            pascalCaseName += word.Substring(0, 1).ToUpper() + word.Substring(1).ToLower(); >> Renamer.cs
echo        } >> Renamer.cs
echo    } >> Renamer.cs
echo    if (originalFileName == pascalCaseName) >> Renamer.cs
echo    { >> Renamer.cs
echo        continue; >> Renamer.cs
echo    } >> Renamer.cs
echo    if (char.IsUpper(originalFileName[0])) >> Renamer.cs
echo    { >> Renamer.cs
echo        pascalCaseName = char.ToUpper(pascalCaseName[0]) + pascalCaseName.Substring(1); >> Renamer.cs
echo    } >> Renamer.cs
echo    string newFileName = pascalCaseName + ".svg"; >> Renamer.cs
echo    string newFilePath = Path.Combine(svgFile.DirectoryName, newFileName); >> Renamer.cs
echo    svgFile.MoveTo(newFilePath); >> Renamer.cs
echo } >> Renamer.cs
echo Console.WriteLine("Rename completed."); >> Renamer.cs
dotnet-exec Renamer.cs
del Renamer.cs
@pause
