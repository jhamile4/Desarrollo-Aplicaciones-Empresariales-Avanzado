using Eventos.Web.Models;

namespace Eventos.Web.ViewModels;

// Modelo de vista (no es tabla): evento + inscritos confirmados y cupos libres.
public class EventoResumen
{
    public Evento Evento { get; set; } = null!;
    public int Inscritos { get; set; }
    public int Cupos => Math.Max(Evento.Aforo - Inscritos, 0);
}
