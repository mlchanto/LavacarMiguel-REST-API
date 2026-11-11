using PROYECTO_01MC.Models;
using Microsoft.EntityFrameworkCore;

namespace PROYECTO_01MC.Dto
{
    public class LavacarMiguelContext : DbContext
    {
        /// <summary>
        /// Constructors con parametros recibe la referencia del ORM
        /// </summary>
        /// <param name="options"></param>
        public LavacarMiguelContext(DbContextOptions<LavacarMiguelContext> options) : base(options)
        {

        }

        /// <summary>
        /// Propiedad para manejar la referencia con la table en la database
        /// </summary>
        public DbSet<Propietario> Propietarios { get; set; }

        public DbSet<Vehiculo> Vehiculos { get; set; }

        public DbSet<Reservacion> Reservaciones { get; set; }


        /// <summary>
        /// Configuracion de relaciones y restricciones Fluent API
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // La placa unica
            modelBuilder.Entity<Vehiculo>().HasIndex(v => v.Placa).IsUnique();

            // Relacion entre Propietario y Vehiculo
            modelBuilder.Entity<Vehiculo>().HasOne(v => v.Propietario).WithMany(p => p.Vehiculos).HasForeignKey(v => v.IdPropietario);

            // Relacion entre Vehiculo y Reservacion
            modelBuilder.Entity<Reservacion>().HasOne(r => r.Vehiculo).WithMany(v => v.Reservaciones).HasForeignKey(r => r.IdVehiculo);
        }
    }
}