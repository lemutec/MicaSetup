if (Debugger.IsAttached)
{
    Directory.SetCurrentDirectory("../../../");
}

string output = Directory.GetCurrentDirectory();
Directory.SetCurrentDirectory("../../../");

HashSet<string> subset = new();

foreach (string filter in new string[] { "*.cs", "*.xaml", "*.txt" })
{
    foreach (string file in Directory.GetFiles(".", filter, SearchOption.AllDirectories))
    {
        Console.WriteLine(file);
        string text = File.ReadAllText(file);

        foreach (char t in text)
        {
            subset.Add(t.ToString());
        }
    }
}

File.WriteAllText(Path.Combine(output, "subset.txt"), string.Join(string.Empty, subset.OrderBy(s => s).ToList()).Replace("\r", string.Empty).Replace("\n", string.Empty).Replace("\t", string.Empty));
