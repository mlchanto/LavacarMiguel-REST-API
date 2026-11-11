//lIBRARY PARA VALIDACIONES DE DATOS
using System.ComponentModel.DataAnnotations;
//LIBRARY PARA CREACION DE TABLAS Y COLUMNAS
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;



namespace PROYECTO_01MC.Models
{
    public class Propietario
    {
        [Key]
        public int IdPropietario { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(120)")]
        public string NombreCompleto { get; set; }

        [Required]
        [Column(TypeName = "varchar(20)")]
        public string Telefono { get; set; }

        // Relacion con Vehiculos

        [JsonIgnore]
        public List<Vehiculo> Vehiculos { get; set; } = new();
    }
}