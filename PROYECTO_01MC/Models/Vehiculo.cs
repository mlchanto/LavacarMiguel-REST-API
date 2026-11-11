//lIBRARY PARA VALIDACIONES DE DATOS
using System.ComponentModel.DataAnnotations;
//LIBRARY PARA CREACION DE TABLAS Y COLUMNAS
using System.ComponentModel.DataAnnotations.Schema;

using System.Text.Json.Serialization;

namespace PROYECTO_01MC.Models
{
    public class Vehiculo
    {
        [Key]
        public int IdVehiculo { get; set; }

        [Required]
        [Column(TypeName = "varchar(10)")]
        public string Placa { get; set; }

        [Required]
        [Column(TypeName = "varchar(20)")]
        public string TipoVehiculo { get; set; }

        public int IdPropietario { get; set; }


        // Relacion con Propietario
        public Propietario Propietario { get; set; }
        // Relacion con Reservaciones

        [JsonIgnore] 
        public List<Reservacion> Reservaciones { get; set; } = new();
    }
}