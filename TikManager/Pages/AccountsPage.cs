using Terminal.Gui;

namespace TikManager.Pages;

internal sealed class AccountsPage : Page
{
    public override string Title => "Аккаунты";

    private View _root = null!;
    private ListView _list = null!;
    private List<string> _sessions = [];

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

        _list = new ListView(_sessions)
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill() - 2
        };

        var addButton = new Button("Добавить")
        {
            X = 0,
            Y = Pos.Bottom(_list)
        };

        var removeButton = new Button("Удалить")
        {
            X = Pos.Right(addButton) + 2,
            Y = Pos.Bottom(_list)
        };

        addButton.Clicked += AddSession;
        removeButton.Clicked += RemoveSession;

        _root.Add(_list, addButton, removeButton);
        content.Add(_root);
    }

    public override void Activate()
    {
        _root.Visible = true;
        _list.SetFocus();
    }

    public override void Deactivate()
    {
        _root.Visible = false;
    }

    private void AddSession()
    {
        var dialog = new Dialog("Добавить sessionId", 50, 8);

        var input = new TextField("")
        {
            X = 1,
            Y = 1,
            Width = Dim.Fill() - 2
        };

        dialog.Add(input);

        var okButton = new Button("OK");
        okButton.Clicked += () =>
            {
                if (!string.IsNullOrWhiteSpace(input.Text.ToString()))
                {
                    _sessions.Add(input.Text.ToString()!);
                    _list.SetSource(_sessions);
                }

                Application.RequestStop();
            };

        var cancelButton = new Button("Cancel");
        cancelButton.Clicked += () => Application.RequestStop();

        dialog.AddButton(okButton);
        dialog.AddButton(cancelButton);

        Application.Run(dialog);
    }

    private void RemoveSession()
    {
        if (_sessions.Count == 0 || _list.SelectedItem < 0)
        {
            return;
        }

        var index = _list.SelectedItem;
        _sessions.RemoveAt(index);
        _list.SetSource(_sessions);
    }
}