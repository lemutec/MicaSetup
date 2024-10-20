using System.IO;
using System.Xml;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using Wpf.Ui.Violeta.Resources;

namespace MakeMica.Design.Controls;

public class JsonCodeBox : CodeBox
{
    public JsonCodeBox() : base()
    {
        RegisterHighlighting();
    }

    private void RegisterHighlighting()
    {
        using Stream s = ResourcesProvider.GetStream(@"pack://application:,,,/Resources/Syntax/JSON.xshd");
        using XmlReader reader = new XmlTextReader(s);
        IHighlightingDefinition luaHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);

        HighlightingManager.Instance.RegisterHighlighting("Json", [".json"], luaHighlighting);
        SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("Json");
    }
}
