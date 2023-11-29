using System;
using System.IO;

public static class Renamer
{
    public static void Action()
    {
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
                    pascalCaseName += word[..1].ToUpper() + word[1..].ToLower();
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
            string newFilePath = Path.Combine(svgFile.DirectoryName!, newFileName);
            svgFile.MoveTo(newFilePath);
        }
        Console.WriteLine("Rename completed.");
    }
}
