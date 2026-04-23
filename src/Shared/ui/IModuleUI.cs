namespace Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;






public interface IModuleUI
{
    
    
    
    
    string Key { get; }

    
    
    
    
    string Title { get; }

    
    
    
    
    
    Task RunAsync(CancellationToken cancellationToken = default);
}
