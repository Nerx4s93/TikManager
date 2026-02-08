using Terminal.Gui;
using TikManager.Models;

namespace TikManager.Pages;

internal sealed class AccountsPage : Page
{
    public override string Title => "Аккаунты";

    private View _root = null!;
    private ListView _list = null!;

    public override void Init(FrameView content)
    {
        using var context = new SessionDbContext();

        _root = new View
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            Visible = false
        };

        _list = new ListView(context.Sessions.Select(s => s.SessionId).ToList())
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

        input.KeyPress += (e) =>
        {
            if (e.KeyEvent.Key == Key.Enter)
            {
                SubmitSession(input.Text.ToString());
                e.Handled = true;
                Application.RequestStop();
            }
        };

        var okButton = new Button("OK");
        okButton.Clicked += () =>
        {
            SubmitSession(input.Text.ToString());
            Application.RequestStop();
        };

        var cancelButton = new Button("Отмена");
        cancelButton.Clicked += () => Application.RequestStop();

        dialog.AddButton(okButton);
        dialog.AddButton(cancelButton);

        Application.Run(dialog);
    }

    private void RemoveSession()
    {
        using var context = new SessionDbContext();

        if (!context.Sessions.Any() || _list.SelectedItem < 0)
        {
            return;
        }

        var sessionToRemove = _list.Source.ToList()[_list.SelectedItem]!.ToString();
        if (sessionToRemove == null) return;

        var session = context.Sessions.FirstOrDefault(s => s.SessionId == sessionToRemove);
        if (session != null)
        {
            context.Sessions.Remove(session);
            context.SaveChanges();
        }

        _list.SetSource(context.Sessions.Select(s => s.SessionId).ToList());
    }

    private void SubmitSession(string? sessionId)
    {
        if (string.IsNullOrWhiteSpace(sessionId))
        {
            return;
        }    

        using var context = new SessionDbContext();
        context.Sessions.Add(new Session { SessionId = sessionId });
        context.SaveChanges();

        _list.SetSource(context.Sessions.Select(s => s.SessionId).ToList());
    }
}