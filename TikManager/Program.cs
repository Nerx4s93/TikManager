using Terminal.Gui;

namespace TikManager;

internal class Program
{
    private static FrameView contentArea = null!;

    private static void Main()
    {
        Application.Init();

        try
        {
            var mainWindow = new Window()
            {
                Title = "Tik Manager",
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };

            var leftPanel = new FrameView("Меню")
            {
                X = 0,
                Y = 0,
                Width = 20,
                Height = Dim.Fill()
            };

            contentArea = new FrameView()
            {
                X = 20,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };

            var pageManager = new PageManager(contentArea);
            var menu = new ListView(pageManager.Titles.ToList())
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };

            menu.SelectedItemChanged += e =>
            {
                var title = (string)menu.Source.ToList()[e.Item]!;
                pageManager.Activate(title);
            };

            leftPanel.Add(menu);
            mainWindow.Add(leftPanel, contentArea);

            Application.Top.Add(mainWindow);
            Application.Run();
        }
        finally
        {
            Application.Shutdown();
        }
    }
}
