using System.Diagnostics;

namespace Interbank.Productos.Comercial.Fcd.Application.Contracts.Abstractions
{
    public interface IProcessRunner
    {
        Process? Start(ProcessStartInfo psi);

    }
}
