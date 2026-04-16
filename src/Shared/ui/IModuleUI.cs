namespace sistema_de_gestion_de_tiquetes_Aereos.src.shared.ui;

public interface IModuleUI
{
    string Key { get; }
    string Title { get; }
    Task RunAsync(CancellationToken cancellationToken = default);
}
