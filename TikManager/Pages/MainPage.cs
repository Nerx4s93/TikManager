using System.Reflection;
using Terminal.Gui;

namespace TikManager.Pages;

internal class MainPage : Page
{
    public override int Index => 0;
    public override string Title => "Главная";

    private View _root = null!;

    public override void Init(FrameView content)
    {
        _root = new View
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            Visible = false
        };

        var versionObj = Assembly.GetExecutingAssembly().GetName().Version;
        var version = versionObj != null ? $"{versionObj.Major}.{versionObj.Minor}.{versionObj.Build}" : "1.0.0";
        var infoTextTemplate = ReadEmbeddedText("TikManager.Resources.InfoText.txt");

        var infoText = string.Format(infoTextTemplate, version);

        var textView = new TextView()
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            Text = infoText,
            ReadOnly = true,
            WordWrap = true,
            AllowsTab = false
        };

        _root.Add(textView);
        content.Add(_root);
    }

    public override void Activate()
    {
        _root.Visible = true;
    }

    public override void Deactivate()
    {
        _root.Visible = false;
    }

    private string ReadEmbeddedText(string resourceName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream(resourceName) ?? throw new Exception($"Resource '{resourceName}' not found");
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
