using Veterinaria.Entidades;

namespace Veterinaria.Datos.Repositorios;

public interface IMascotaRepositorio
{
    Task<List<Mascota>> ListarAsync(string? especie, int? propietarioId, decimal? pesoMax);
    Task<List<Mascota>> ListarPorPropietarioAsync(int propietarioId);
    Task<Mascota?> ObtenerPorIdAsync(int id);
    Task<Mascota?> ObtenerParaEditarAsync(int id);
    Task<int> ContarPorPropietarioAsync(int propietarioId);
    Task<bool> ExisteNombreEnPropietarioAsync(string nombre, int propietarioId, int? excluirId = null);
    Task AgregarAsync(Mascota mascota);
    Task GuardarCambiosAsync();
    Task EliminarAsync(Mascota mascota);
}
