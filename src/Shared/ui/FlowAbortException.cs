namespace Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

public sealed class FlowAbortException : Exception
{
    public FlowAbortException()
        : base("Operación cancelada por el usuario.")
    {
    }
}
