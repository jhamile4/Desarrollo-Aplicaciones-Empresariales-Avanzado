namespace Veterinaria.Vacunas;

// Modelo propio del microservicio (no comparte clases con la API de la Semana 14).
public record Vacuna(
    int Id,
    string Nombre,
    string Especie,
    string Previene,
    int EdadMinimaSemanas,
    int RefuerzoMeses,
    decimal Precio);

// Datos en memoria: este servicio no usa base de datos.
public static class CatalogoVacunas
{
    public static readonly IReadOnlyList<Vacuna> Lista = new List<Vacuna>
    {
        new(1, "Séxtuple canina",       "Perro", "Moquillo, parvovirus, hepatitis, leptospirosis, parainfluenza y coronavirus", 6, 12, 65.00m),
        new(2, "Antirrábica",           "Perro", "Rabia",                                                                    12, 12, 40.00m),
        new(3, "Bordetella (tos de las perreras)", "Perro", "Traqueobronquitis infecciosa",                                 8, 12, 55.00m),
        new(4, "Triple felina",         "Gato",  "Panleucopenia, rinotraqueítis y calicivirus",                              8, 12, 60.00m),
        new(5, "Leucemia felina",       "Gato",  "Virus de la leucemia felina (FeLV)",                                       9, 12, 70.00m),
        new(6, "Antirrábica felina",    "Gato",  "Rabia",                                                                    12, 12, 40.00m),
        new(7, "Mixomatosis",           "Conejo","Mixomatosis",                                                               6, 6,  45.00m),
    };
}
