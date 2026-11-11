//LIBRARY PARA VALIDACIONES
using System.ComponentModel.DataAnnotations;

namespace PROYECTO_01MC.Dto
{
    public class ReservacionRequest
    {
        [Required]
        public string Placa { get; set; }

        [Required]
        public string NombrePropietario { get; set; }

        [Required]
        public string Telefono { get; set; }

        [Required]
        public string TipoVehiculo { get; set; }

        public DateTime FechaReservacion { get; set; }

        public TimeSpan HoraReservacion { get; set; }

         
    }
}