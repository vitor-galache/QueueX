namespace QueueX.Configuration;

/// <summary>
/// Opções de configuração para o QueueX.Abstractions.
/// </summary>
public class QueueOptions
{
    public string Host { get; set; } = string.Empty;
    
    public int Port { get; set; }
    
    public string User { get; set; } = string.Empty;
    
    public string Password { get; set; } = string.Empty;
    
    public string? Instance { get; set; }
}