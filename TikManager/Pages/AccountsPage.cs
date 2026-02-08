using Terminal.Gui;
using TikManager.Models;

namespace TikManager.Pages;

internal sealed class AccountsPage : Page
{
    public override string Title => "Аккаунты";

    private List<Session> _cachedSessions = new List<Session>();

    private View _root = null!;
    private ListView _list = null!;

    public override void Init(FrameView content)
    {
        LoadSessions();

        _root = new View
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            Visible = false
        };

        _list = new ListView(_cachedSessions.Select(s => s.SessionId).ToList())
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

    private void LoadSessions()
    {
        using var db = new SessionDbContext();
        _cachedSessions = db.Sessions.ToList();
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

        dialog.Add(input);
        dialog.AddButton(okButton);
        dialog.AddButton(cancelButton);

        Application.Run(dialog);
    }

    private void RemoveSession()
    {
        if (!_cachedSessions.Any() || _list.SelectedItem < 0)
        {
            return;
        }    

        var sessionToRemove = _cachedSessions[_list.SelectedItem];

        using var context = new SessionDbContext();
        var session = context.Sessions.FirstOrDefault(s => s.Id == sessionToRemove.Id);
        if (session != null)
        {
            context.Sessions.Remove(session);
            context.SaveChanges();
        }

        _cachedSessions.Remove(sessionToRemove);
        _list.SetSource(_cachedSessions.Select(s => s.SessionId).ToList());
    }

    private void SubmitSession(string? sessionId)
    {
        if (string.IsNullOrWhiteSpace(sessionId))
        {
            return;
        }

        var session = new Session { SessionId = sessionId };

        using var context = new SessionDbContext();
        context.Sessions.Add(session);
        context.SaveChanges();

        _cachedSessions.Add(session);
        _list.SetSource(_cachedSessions.Select(s => s.SessionId).ToList());
    }
}