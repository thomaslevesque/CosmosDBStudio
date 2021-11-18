namespace CosmosDBStudio.Model.Services;

public interface IApplication
{
    void Quit();

    ApplicationVersionInfo GetVersionInfo();
}

public record ApplicationVersionInfo(string ProductName, string Version, string Author);