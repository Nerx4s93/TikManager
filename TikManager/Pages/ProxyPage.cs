using Terminal.Gui;
using TikManager.Models;

namespace TikManager.Pages;

internal class ProxyPage : Page
{
    public override int Index => 2;
    public override string Title => "Прокси";

    private List<Proxy> _cachedProxies = [];

    private View _root = null!;
    private ListView _list = null!;

    public override void Init(FrameView content)
    {
        LoadProxies();

        _root = new View
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            Visible = false
        };

        _list = new ListView(_cachedProxies.Select(s => s.ToString()).ToList())
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill() - 2
        };

        var addButton = new Button("Добавить")
        {
            X = 0,
            Y = Pos.Bottom(_list) + 1
        };

        var removeButton = new Button("Удалить")
        {
            X = Pos.Right(addButton) + 2,
            Y = Pos.Bottom(_list) + 1
        };

        var editButton = new Button("Редактировать")
        {
            X = Pos.Right(removeButton) + 2,
            Y = Pos.Bottom(_list) + 1
        };

        addButton.Clicked += AddProxy;
        removeButton.Clicked += RemoveProxy;
        editButton.Clicked += EditProxy;

        _root.Add(_list, addButton, removeButton, editButton);
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

    private void LoadProxies()
    {
        using var db = new AppDbContext();
        _cachedProxies = db.Proxies.ToList();
    }

    private void AddProxy()
    {
        var dialog = new Dialog("Добавить прокси", 50, 12);

        var hostLabel = new Label("Host:") { X = 1, Y = 1 };
        var inputHost = new TextField("")
        {
            X = 12,
            Y = 1,
            Width = Dim.Fill() - 2
        };

        var portLabel = new Label("Port:") { X = 1, Y = Pos.Bottom(hostLabel) + 1 };
        var inputPort = new TextField("")
        {
            X = 12,
            Y = Pos.Bottom(hostLabel) + 1,
            Width = Dim.Fill() - 2
        };

        var loginLabel = new Label("Login:") { X = 1, Y = Pos.Bottom(inputPort) + 1 };
        var inputLogin = new TextField("")
        {
            X = 12,
            Y = Pos.Bottom(inputPort) + 1,
            Width = Dim.Fill() - 2
        };

        var passwordLabel = new Label("Password:") { X = 1, Y = Pos.Bottom(loginLabel) + 1 };
        var inputPassword = new TextField("")
        {
            X = 12,
            Y = Pos.Bottom(loginLabel) + 1,
            Width = Dim.Fill() - 2
        };

        SetEnterFocus(inputHost, inputPort);
        SetEnterFocus(inputPort, inputLogin);
        SetEnterFocus(inputLogin, inputPassword);
        inputHost.KeyPress += e =>
        {
            char c = (char)e.KeyEvent.KeyValue;
            if (!char.IsDigit(c) && c != '.' && e.KeyEvent.Key != Key.Backspace)
            {
                e.Handled = true;
            }    
        };
        inputPort.KeyPress += e =>
        {
            if (!char.IsDigit((char)e.KeyEvent.KeyValue) && e.KeyEvent.Key != Key.Backspace)
            {
                e.Handled = true;
            }
        };
        inputPassword.KeyPress += (e) =>
        {
            if (e.KeyEvent.Key == Key.Enter)
            {
                SubmitProxy(
                    inputHost.Text.ToString()!,
                    int.Parse(inputPort.Text.ToString() ?? "0"),
                    inputLogin.Text.ToString(),
                    inputPassword.Text.ToString());
                e.Handled = true;
                Application.RequestStop();
            }
        };

        var okButton = new Button("OK");
        okButton.Clicked += () =>
        {
            SubmitProxy(
                inputHost.Text.ToString()!,
                int.Parse(inputPort.Text.ToString() ?? "0"),
                inputLogin.Text.ToString(),
                inputPassword.Text.ToString());
            Application.RequestStop();
        };

        var cancelButton = new Button("Отмена");
        cancelButton.Clicked += () => Application.RequestStop();

        dialog.Add(hostLabel, portLabel, loginLabel, passwordLabel);
        dialog.Add(inputHost, inputPort, inputLogin, inputPassword);
        dialog.AddButton(okButton);
        dialog.AddButton(cancelButton);

        Application.Run(dialog);
    }

    private void RemoveProxy()
    {
        if (_cachedProxies.Count == 0 || _list.SelectedItem < 0)
        {
            return;
        }

        var proxyToRemove = _cachedProxies[_list.SelectedItem];

        using var context = new AppDbContext();
        var session = context.Proxies.FirstOrDefault(s => s.Id == proxyToRemove.Id);
        if (session != null)
        {
            context.Proxies.Remove(session);
            context.SaveChanges();
        }

        _cachedProxies.Remove(proxyToRemove);
        _list.SetSource(_cachedProxies.Select(s => s.ToString()).ToList());
    }

    private void EditProxy()
    {
        if (_cachedProxies.Count == 0 || _list.SelectedItem < 0)
        {
            return;
        }

        var proxy = _cachedProxies[_list.SelectedItem];

        var dialog = new Dialog("Редактирование прокси", 50, 12);

        var hostLabel = new Label("Host:") { X = 1, Y = 1 };
        var inputHost = new TextField(proxy.Host)
        {
            X = 12,
            Y = 1,
            Width = Dim.Fill() - 2
        };

        var portLabel = new Label("Port:") { X = 1, Y = Pos.Bottom(hostLabel) + 1 };
        var inputPort = new TextField(proxy.Port.ToString())
        {
            X = 12,
            Y = Pos.Bottom(hostLabel) + 1,
            Width = Dim.Fill() - 2
        };

        var loginLabel = new Label("Login:") { X = 1, Y = Pos.Bottom(inputPort) + 1 };
        var inputLogin = new TextField(proxy.Login ?? "")
        {
            X = 12,
            Y = Pos.Bottom(inputPort) + 1,
            Width = Dim.Fill() - 2
        };

        var passwordLabel = new Label("Password:") { X = 1, Y = Pos.Bottom(loginLabel) + 1 };
        var inputPassword = new TextField(proxy.Password ?? "")
        {
            X = 12,
            Y = Pos.Bottom(loginLabel) + 1,
            Width = Dim.Fill() - 2
        };

        SetEnterFocus(inputHost, inputPort);
        SetEnterFocus(inputPort, inputLogin);
        SetEnterFocus(inputLogin, inputPassword);
        inputHost.KeyPress += e =>
        {
            char c = (char)e.KeyEvent.KeyValue;
            if (!char.IsDigit(c) && c != '.' && e.KeyEvent.Key != Key.Backspace)
            {
                e.Handled = true;
            }
        };
        inputPort.KeyPress += e =>
        {
            if (!char.IsDigit((char)e.KeyEvent.KeyValue) && e.KeyEvent.Key != Key.Backspace)
            {
                e.Handled = true;
            }
        };
        inputPassword.KeyPress += (e) =>
        {
            if (e.KeyEvent.Key == Key.Enter)
            {
                SubmitEditProxy(
                    proxy.Id,
                    proxy,
                    inputHost.Text.ToString()!,
                    int.Parse(inputPort.Text.ToString() ?? "0"),
                    inputLogin.Text.ToString(),
                    inputPassword.Text.ToString());
                Application.RequestStop();
            }
        };

        var okButton = new Button("OK");
        okButton.Clicked += () =>
        {
            SubmitEditProxy(
                proxy.Id,
                proxy,
                inputHost.Text.ToString()!,
                int.Parse(inputPort.Text.ToString() ?? "0"),
                inputLogin.Text.ToString(),
                inputPassword.Text.ToString());
            Application.RequestStop();
        };

        var cancelButton = new Button("Отмена");
        cancelButton.Clicked += () => Application.RequestStop();

        dialog.Add(hostLabel, portLabel, loginLabel, passwordLabel);
        dialog.Add(inputHost, inputPort, inputLogin, inputPassword);
        dialog.AddButton(okButton);
        dialog.AddButton(cancelButton);

        Application.Run(dialog);
    }

    private void SubmitProxy(string host, int port, string? login, string? password)
    {
        if (string.IsNullOrWhiteSpace(host) || port == 0)
        {
            return;
        }

        var proxy = new Proxy { Host = host, Port = port, Login = login, Password = password };

        using var context = new AppDbContext();
        context.Proxies.Add(proxy);
        context.SaveChanges();

        _cachedProxies.Add(proxy);
        _list.SetSource(_cachedProxies.Select(s => s.ToString()).ToList());
    }

    private void SubmitEditProxy(int proxyId, Proxy cacheProxy, string host, int port, string? login, string? password)
    {
        if (string.IsNullOrWhiteSpace(host) || port == 0)
        {
            return;
        }

        using var context = new AppDbContext();
        var proxy = context.Proxies.FirstOrDefault(p => p.Id == proxyId);
        if (proxy == null)
        {
            return;
        }

        proxy.Host = host;
        proxy.Port = port;
        proxy.Login = login;
        proxy.Password = password;

        context.SaveChanges();

        cacheProxy.Host = host;
        cacheProxy.Port = port;
        cacheProxy.Login = login;
        cacheProxy.Password = password;

        _list.SetSource(_cachedProxies.Select(s => s.ToString()).ToList());
    }

    private void SetEnterFocus(TextField from, TextField to)
    {
        from.KeyPress += e =>
        {
            if (e.KeyEvent.Key == Key.Enter)
            {
                e.Handled = true;
                to.SetFocus();
            }
        };
    }
}
