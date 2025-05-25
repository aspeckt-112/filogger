namespace FiLogger.Integration.Tests.Fixtures;

public class FiLoggerFixture : IDisposable
{
    private readonly List<string> _logFiles = [];
    
    public void AddLogFile(string filePath)
    {
        _logFiles.Add(filePath);
    }

    public void Dispose()
    {
        foreach (string file in _logFiles)
        {
            File.Delete(file);
        }
    }
}