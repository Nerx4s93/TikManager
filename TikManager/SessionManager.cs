namespace TikManager;

internal class SessionManager
{
    private const string FilePath = "sessions.txt";

    private readonly List<string> _sessions;

    public IReadOnlyCollection<string> Sessions => _sessions;

    public SessionManager()
    {
        if (File.Exists(FilePath))
        {
            var sessions = File.ReadAllLines(FilePath);
            _sessions = [.. sessions];
        }
        else
        {
            _sessions = [];
        }

    }

    public void AddSession(string session) => _sessions.Add(session);

    public void DeleteSession(string session) => _sessions.Remove(session);

    public void Save()
    {
        File.WriteAllLines(FilePath, _sessions);
    }
}
