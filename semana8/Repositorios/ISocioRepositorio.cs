using Biblioteca.Web.Models;

namespace Biblioteca.Web.Repositorios
{
    public interface ISocioRepositorio
    {
        Task<IEnumerable<Socio>> ListarActivosAsync();
        Task<int> InsertarAsync(Socio socio);
        Task<bool> ExisteDNIAsync(string dni);
    }
}
