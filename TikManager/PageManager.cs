using System.Reflection;
using Terminal.Gui;

namespace TikManager;

internal sealed class PageManager
{
    private readonly FrameView _content;

    private readonly Dictionary<string, Page> _pages = new(StringComparer.OrdinalIgnoreCase);

    private Page? _current;

    public IReadOnlyList<string> Titles => _pages.Values.OrderBy(p => p.Index).Select(p => p.Title).ToList();

    public PageManager(FrameView content)
    {
        _content = content;

        var pageType = typeof(Page);

        var types = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => !t.IsAbstract && pageType.IsAssignableFrom(t));

        foreach (var type in types)
        {
            var page = (Page)Activator.CreateInstance(type)!;

            page.Init(content);
            page.Deactivate();

            _pages.Add(page.Title, page);
        }

        Activate(Titles.ToList()[0]);
    }

    public void Activate(string title)
    {
        if (!_pages.TryGetValue(title, out var page))
        {
            throw new KeyNotFoundException($"Page '{title}' not found");
        }

        _current?.Deactivate();
        _current = page;
        _content.Title = title;

        _current.Activate();
    }
}