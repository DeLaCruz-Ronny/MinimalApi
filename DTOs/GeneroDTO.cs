using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MinimalApiPeliculas.DTOs
{
    public class GeneroDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
    }
}