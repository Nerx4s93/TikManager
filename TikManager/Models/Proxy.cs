namespace TikManager.Models;

public class Proxy
{
    public int Id { get; set; }

    public string Host { get; set; } = null!;
    public int Port { get; set; }

    public string? Login { get; set; }
    public string? Password { get; set; }

    public override string ToString()
    {
        var result = $"{Host}:{Port}";

        if (Login != null && Password != null)
        {
            result += $" {Login}:{Password}";
        }

        return result;
    }   
}