using Microsoft.EntityFrameworkCore;
using Veterinaria.Datos.Models;
using Veterinaria.Entidades;

namespace Veterinaria.Datos.Repositorios;

// Acceso a datos de Usuario: solo LINQ y SaveChangesAsync, sin reglas de negocio.
public class UsuarioRepositorio : IUsuarioRepositorio
{
    private readonly VeterinariaContext _context;

    public UsuarioRepositorio(VeterinariaContext context)
    {
        _context = context;
    }

    public async Task<Usuario?> ObtenerPorCorreoAsync(string correo)
    {
        return await _context.Usuario
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Correo == correo);
    }

    public async Task<bool> ExisteCorreoAsync(string correo)
    {
        return await _context.Usuario.AnyAsync(u => u.Correo == correo);
    }

    public async Task AgregarAsync(Usuario usuario)
    {
        _context.Usuario.Add(usuario);
        await _context.SaveChangesAsync();
    }

    public async Task GuardarCambiosAsync()
    {
        await _context.SaveChangesAsync();
    }
}
