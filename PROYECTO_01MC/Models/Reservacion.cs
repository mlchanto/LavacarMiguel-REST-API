//lIBRARY PARA VALIDACIONES DE DATOS
using System.ComponentModel.DataAnnotations;
//LIBRARY PARA CREACION DE TABLAS Y COLUMNAS
using System.ComponentModel.DataAnnotations.Schema;

namespace PROYECTO_01MC.Models
{
    public class Reservacion
    {
        [Key]
        public int IdReservacion { get; set; }

        public int IdVehiculo { get; set; }

        [Column(TypeName = "date")]
        public DateTime FechaReservacion { get; set; }

        [Column(TypeName = "time")]
        public TimeSpan HoraReservacion { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal PrecioServicio { get; set; }

        [Required]
        [Column(TypeName = "varchar(20)")]
        public string Estado { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime FechaRegistro { get; set; }

        // Relacion con Vehiculo
        public Vehiculo Vehiculo { get; set; }
    }
}