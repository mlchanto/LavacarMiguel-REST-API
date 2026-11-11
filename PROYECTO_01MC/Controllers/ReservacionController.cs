//Library Dto
using PROYECTO_01MC.Dto;

//LIBRARY MODELS
using PROYECTO_01MC.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace PROYECTO_01MC.Controllers
{
    [ApiController] //permite implementar los metodos para una API
    [Route("api/[Controller]")] //CONSTRUYE LA RUTA PARA LOS ENDPOINTS
    public class ReservacionesController : ControllerBase
    {
        //VARIABLE DBCONTEXT PARA MANEJAR LA REFERENCIA DEL ORM
        private LavacarMiguelContext context = null;

        /// <summary>
        /// Constructor con parametros recibe la referencia del ORM
        /// </summary>
        /// <param name="pContext"></param>
        public ReservacionesController(LavacarMiguelContext pContext)
        {
            //Se asigna la referencia del ORM
            this.context = pContext;
        }

        //1. Metodo para consultar todas las reservaciones
        [HttpGet]
        [Route("List")]
        public List<Reservacion> List()
        {
            //Se utiliza el ORM para leer todas las reservaciones
            

            return this.context.Reservaciones
                    .Include(r => r.Vehiculo)
                    .ThenInclude(v => v.Propietario)
                    .ToList();

        }

        //2. Metodo para buscar reservacion por ID
        [HttpGet]
        [Route("SearchByID")]
        public Reservacion SearchByID(int id)
        {
            return this.context.Reservaciones.FirstOrDefault(r => r.IdReservacion == id);
        }

        //3.Metodo para registrar una reservacion
        [HttpPost]
        [Route("Create")]
        public async Task<string> Create(ReservacionRequest datos)
        {
            string msj = "";

            try
            {
                //Validar datos
                if (datos == null)
                {
                    msj = "Error, no se encontraron datos";
                    return msj;
                }

                //Validar placa
                if (string.IsNullOrWhiteSpace(datos.Placa) ||
                      datos.Placa.Trim().ToLower() == "string")
                {
                    msj = "Error, debe ingresar una placa válida";
                    return msj;
                }
                string placa = datos.Placa.Trim().ToUpper();

                if (placa.Length > 10 || !placa.All(char.IsLetterOrDigit))
                {
                    msj = "Error, placa no debe ser de mas de 10 digitos";
                    return msj;
                }

                //Validar longitud del nombre propietario
                if (string.IsNullOrWhiteSpace(datos.NombrePropietario) || datos.NombrePropietario.Trim().Length < 5)
                {
                    msj = "Error, el nombre debe tener como minimo 5 caracteres";
                    return msj;
                }

                //Validar telefono
                
                if (string.IsNullOrWhiteSpace(datos.Telefono))
                {
                    msj = "Error, el telefono es obligatorio";
                    return msj;
                }

                //Validar ingreso de tipo de vehiculo
                if (string.IsNullOrWhiteSpace(datos.TipoVehiculo))
                {
                    msj = "Error, ingresar el tipo de vehiculo es obligatorio";
                    return msj;
                }

                string tipo = datos.TipoVehiculo.Trim().ToUpper();

                //Asignar precio automaticamente y valida que sea el tipo de carro adecuado
                decimal precio = 0;

                switch (tipo)
                {
                    case "SEDAN":
                        precio = 9000;
                        break;

                    case "4X4":
                        precio = 15000;
                        break;

                    case "MOTOCICLETA":
                        precio = 7000;
                        break;

                    case "CAMION GANADERO":
                        precio = 45000;
                        tipo = "CAMION GANADERO";
                        break;

                    default:
                        msj = "Error, tipo de vehiculo no permitido";
                        return msj;
                }

                //Validar fecha
                if (datos.FechaReservacion.Date < DateTime.Today)
                {
                    msj = "Error, no se permiten fechas anteriores";
                    return msj;
                }

                //Valida si es domingo con Dafofweek
                if (datos.FechaReservacion.DayOfWeek == DayOfWeek.Sunday)
                {
                    msj = "Error, los domingos no se atiende";
                    return msj;
                }

                //Validar horario
                if (datos.HoraReservacion.Hours < 8 || datos.HoraReservacion.Hours >= 17)
                {
                    msj = "Error, el horario es de 8:00 a 17:00";
                    return msj;
                }

                //Validar intervalos de 30 minutos
                if ((datos.HoraReservacion.Minutes != 0 && datos.HoraReservacion.Minutes != 30) ||
                    datos.HoraReservacion.Seconds != 0 || datos.HoraReservacion.Ticks % TimeSpan.TicksPerSecond != 0)
                {
                    msj = "Error, las reservaciones deben ser cada 30 minutos";
                    return msj;
                }

                //Validar hora pasada
                if (datos.FechaReservacion.Date == DateTime.Today && datos.HoraReservacion <= DateTime.Now.TimeOfDay)
                {
                    msj = "Error, no se puede reservar una hora pasada";
                    return msj;
                }

                //Verificar horario ocupado
                var ocupado = this.context.Reservaciones.FirstOrDefault(r =>
                    r.FechaReservacion == datos.FechaReservacion.Date &&
                    r.HoraReservacion == datos.HoraReservacion &&
                    r.Estado != "Cancelada");

                if (ocupado != null)
                {
                    msj = "Error, el horario ya esta ocupado";
                    return msj;
                }

                //Buscar vehiculo por placa
                var vehiculo = this.context.Vehiculos
                    .Include(v => v.Propietario)
                    .FirstOrDefault(v => v.Placa == placa);

                if (vehiculo == null)
                {
                    //Crear propietario
                    Propietario propietario = new Propietario();

                    propietario.NombreCompleto = datos.NombrePropietario.Trim();
                    propietario.Telefono = datos.Telefono.Trim();

                    //Crear vehiculo
                    vehiculo = new Vehiculo();

                    vehiculo.Placa = placa;
                    vehiculo.TipoVehiculo = tipo;
                    vehiculo.Propietario = propietario;

                    this.context.Vehiculos.Add(vehiculo);
                }
                else
                {
                    //Validar que la placa pertenezca al mismo vehiculo
                    if (vehiculo.TipoVehiculo.ToUpper() != tipo ||
                        vehiculo.Propietario.NombreCompleto.Trim().ToUpper() !=
                        datos.NombrePropietario.Trim().ToUpper() ||
                        vehiculo.Propietario.Telefono.Trim() != datos.Telefono.Trim())
                    {
                        msj = "Error, la placa ya pertenece a otro vehiculo o propietario";
                        return msj;
                    }
                }

                //Crear reservacion
                Reservacion reservacion = new Reservacion();

                reservacion.Vehiculo = vehiculo;
                reservacion.FechaReservacion = datos.FechaReservacion.Date;
                reservacion.HoraReservacion = datos.HoraReservacion;
                reservacion.PrecioServicio = precio;
                reservacion.Estado = "Pendiente";
                reservacion.FechaRegistro = DateTime.Now;

                //Guardar reservacion
                this.context.Reservaciones.Add(reservacion);

                await this.context.SaveChangesAsync();

                msj = "Reservacion guardada correctamente";
            }
            catch (Exception ex)
            {
                msj = $"Error, {ex.InnerException ?? (object)ex.Message}";
            }

            return msj;
        }

        //4. Metodo para modificar una reservacion
        [HttpPut]
        [Route("Update")]
        public async Task<string> Update(int id, ReservacionRequest datos, string? estado)
        {
            string msj = "";

            try
            {
                if (datos != null)
                {
                    //Buscar reservacion por ID
                    var aux = this.context.Reservaciones.FirstOrDefault(r => r.IdReservacion == id);

                    if (aux != null)
                    {
                        //Validar estado
                        if (aux.Estado == "Cancelada" || aux.Estado == "Completada")
                        {
                            msj = "Error, la reservacion no se puede actualizar una vez completada";
                            return msj;
                        }

                        //Validar el nuevo estado
                        if (!string.IsNullOrEmpty(estado) &&
                            estado != "Confirmada" &&
                            estado != "Completada")
                        {
                            msj = "Error, estado no permitido";
                            return msj;
                        }

                        //Validar fecha
                        if (datos.FechaReservacion.Date < DateTime.Today)
                        {
                            msj = "Error, no se permiten fechas anteriores";
                            return msj;
                        }

                        //Validar domingo
                        if (datos.FechaReservacion.DayOfWeek == DayOfWeek.Sunday)
                        {
                            msj = "Error, los domingos no se atiende";
                            return msj;
                        }

                        //Validar horario
                        if (datos.HoraReservacion.Hours < 8 || datos.HoraReservacion.Hours >= 17)
                        {
                            msj = "Error, horario fuera del rango permitido";
                            return msj;
                        }

                        //Validar intervalos de 30 minutos
                        if (datos.HoraReservacion.Minutes != 0 &&
                            datos.HoraReservacion.Minutes != 30)
                        {
                            msj = "Error, las reservaciones deben ser cada 30 minutos";
                            return msj;
                        }

                       
                        //Validar hora pasada
                        if (datos.FechaReservacion.Date == DateTime.Today && datos.HoraReservacion <= DateTime.Now.TimeOfDay)
                        {
                            msj = "Error, no se puede reservar una hora que ya pasó";
                            return msj;
                        }

                        //Verificar horario ocupado
                        var ocupado = this.context.Reservaciones.FirstOrDefault(r =>
                            r.IdReservacion != id &&
                            r.FechaReservacion == datos.FechaReservacion.Date &&
                            r.HoraReservacion == datos.HoraReservacion &&
                            r.Estado != "Cancelada");

                        if (ocupado != null)
                        {
                            msj = "Error, el horario ya esta ocupado";
                            return msj;
                        }

                        //Actualizar reservacion
                        aux.FechaReservacion = datos.FechaReservacion.Date;
                        aux.HoraReservacion = datos.HoraReservacion;

                        //Actualizar estado
                        if (estado == "Confirmada" || estado == "Completada")
                        {
                            aux.Estado = estado;
                        }

                        //Guardar cambios
                        this.context.Reservaciones.Update(aux);
                        await this.context.SaveChangesAsync();

                        msj = "Reservacion actualizada correctamente";
                    }
                    else
                    {
                        msj = "Error, la reservacion no existe";
                    }
                }
                else
                {
                    msj = "No se encontraron datos";
                }
            }
            catch (Exception ex)
            {
                msj = $"Error, {ex.InnerException ?? (object)ex.Message}";
            }

            return msj;
        }

        //5.Metodo para cancelar una reservacion
        [HttpDelete]
        [Route("Delete")]
        public async Task<string> Delete(int id)
        {
            string msj = "";

            try
            {
                //Buscar reservacion
                var aux = this.context.Reservaciones .FirstOrDefault(r => r.IdReservacion == id);

                //Validar si existe
                if (aux == null)
                {
                    msj = "Error, la reservacion no existe";
                }
                else if (aux.Estado == "Completada" || aux.Estado == "Cancelada")
                {
                    msj = "Error, la reservacion no se puede cancelar";
                }
                else
                {
                    //Cancelar reservacion
                    aux.Estado = "Cancelada";

                    //Guardar cambios
                    this.context.Reservaciones.Update(aux);

                    await this.context.SaveChangesAsync();

                    msj = "Reservacion cancelada correctamente";
                }
            }
            catch (Exception ex)
            {
                msj = $"Error, {ex.InnerException}";
            }

            return msj;
        }

        //6.Metodo para buscar reservaciones por placa
        [HttpGet]
        [Route("SearchByPlaca")]
        public List<Reservacion> SearchByPlaca(string placa)
        {
            //Buscar reservaciones por placa
            return this.context.Reservaciones
                .Include(r => r.Vehiculo)
                .ThenInclude(v => v.Propietario)
                .Where(r => r.Vehiculo.Placa == placa.Trim().ToUpper())
                .ToList();
        }

        //7.Metodo para buscar reservaciones por fecha
        [HttpGet]
        [Route("SearchByFecha")]
        public List<Reservacion> SearchByFecha(DateTime fecha)
        {
            //Buscar reservaciones por fecha
            return this.context.Reservaciones
                .Include(r => r.Vehiculo)
                .ThenInclude(v => v.Propietario)
                .Where(r => r.FechaReservacion == fecha.Date)
                .ToList();
        }

        //8.Metodo para consultar horarios disponibles
        [HttpGet]
        [Route("Disponibilidad")]
        public List<string> Disponibilidad(DateTime fecha)
        {
            //Lista para guardar horarios disponibles
            List<string> horarios = new List<string>();

            //Validar fecha
            if (fecha.Date < DateTime.Today)
            {
                return horarios;
            }
            //Validar domingo
            if (fecha.DayOfWeek == DayOfWeek.Sunday)
            {
                horarios.Add("El lavacar no atiende los domingos");
                return horarios;
            }
            //Recorrer horarios de 8:00 a 17:00
            for (int hora = 8; hora < 17; hora++)
            {
                for (int minuto = 0; minuto < 60; minuto += 30)
                {
                    TimeSpan horario = new TimeSpan(hora, minuto, 0);

                    //No mostrar horas pasadas
                    if (fecha.Date == DateTime.Today &&
                        horario <= DateTime.Now.TimeOfDay)
                    {
                        continue;
                    }

                    //Verificar si el horario esta ocupado
                    var ocupado = this.context.Reservaciones.FirstOrDefault(r =>
                        r.FechaReservacion == fecha.Date &&
                        r.HoraReservacion == horario &&
                        r.Estado != "Cancelada");

                    //Agregar horario disponible
                    if (ocupado == null)
                    {
                        horarios.Add(horario.ToString(@"hh\:mm"));
                    }
                }
            }

            return horarios;
        }
    }
}