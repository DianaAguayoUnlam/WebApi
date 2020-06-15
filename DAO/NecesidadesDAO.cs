using System;
using System.Collections.Generic;
using System.Linq;
using Entidades;
using DAO.Abstract;
using System.Data.Entity.Validation;

using System.Net.Http;
using System.Diagnostics.Eventing.Reader;

using DAO.Context;
using Entidades.Enum;

namespace DAO
{
    public class NecesidadesDAO : Crud<Necesidades>
    {

        public NecesidadesDAO(TpDBContext context) :base(context)
        {
            
        }
        public override Necesidades ObtenerPorID(int idNecesidad)
        {
            Necesidades necesidad = context.Necesidades.Find(idNecesidad);
            return necesidad;
        }
        public override Necesidades Crear(Necesidades necesidadAGuardar)
        {
            Necesidades necesidades = context.Necesidades.Add(necesidadAGuardar);
            context.SaveChanges();
            return necesidades;
        }

        public List<Necesidades> TraerNecesidadesActivasDelUsuario(int idSession)
        {
            List<Necesidades> necesidadesActivas = (from c in context.Necesidades.Include("Denuncias")
                                                    where c.IdUsuarioCreador == idSession
                                                    where c.Estado == (int)TipoEstadoNecesidad.Activa
                                                    where c.FechaFin > DateTime.Now
                                                    select c).ToList();
            return necesidadesActivas;
        }

        public List<Necesidades> TraerTodasLasNecesidadesDelUsuario(int idSession)
        {
            List<Necesidades> todasLasNecesidadesDelUsuario = (from c in context.Necesidades
                                                               where c.IdUsuarioCreador.Equals(idSession)
                                                               where c.FechaFin > DateTime.Now
                                                               select c).ToList();

            return todasLasNecesidadesDelUsuario;
        }

        public List<Necesidades> ListarTodasLasNecesidades()
        {
            List<Necesidades> listadoNecesidades = new List<Necesidades>();

            var listaObtenida = (from nec in context.Necesidades.
                                 Include("Usuarios").
                                 Include("Denuncias").
                                 Include("NecesidadesDonacionesInsumos")
                                 .Include("NecesidadesDonacionesMonetarias").
                                 Include("NecesidadesReferencias")
                                .Include("NecesidadesValoraciones")
                                 where nec.FechaFin > DateTime.Now
                                 where nec.Estado == 1
                                 select nec);

            foreach (var item in listaObtenida)
            {
                listadoNecesidades.Add(item);
            }

            return listadoNecesidades;
        }

        public override Necesidades Actualizar(Necesidades necesidadObtenida)
        {
            {
                Necesidades necesidadBd = ObtenerPorID(necesidadObtenida.IdNecesidad);

                necesidadBd.Valoracion = (decimal)necesidadObtenida.Valoracion;
                necesidadBd.Descripcion = necesidadObtenida.Descripcion;
                necesidadBd.Estado = necesidadObtenida.Estado;
                necesidadBd.Foto = necesidadObtenida.Foto;
                necesidadBd.Nombre = necesidadObtenida.Nombre;
                necesidadBd.TelefonoContacto = necesidadObtenida.TelefonoContacto;
                necesidadBd.NecesidadesValoraciones = necesidadObtenida.NecesidadesValoraciones;


                try
                {

                    context.SaveChanges();
                }
                catch (DbEntityValidationException dbEx)
                {
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            System.Console.WriteLine("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                        }
                    }
                }
                return necesidadBd;
            }
        }


        /// <summary>
        /// Buscar necesidades en relación al nombre de las necesidades existentes o bien según el nombre del
        /// usuario creador. Ordenado por fecha más cercana de cierre de necesidad y, luego,
        /// por mayor valoración de la necesidad.El resultado de la búsqueda no deberá incluir sus propias
        /// necesidades
        /// </summary>
        /// <param name="input"></param>
        /// <param name="idUser"></param>
        /// <returns></returns>
        public List<Necesidades> Buscar(string input, int idUser)
        {
            List<Necesidades> necesidadesObtenidas =
              (
              from necesidad in context.Necesidades.Include("Usuarios")
              where necesidad.Usuarios.Nombre.Contains(input) || necesidad.Nombre.Contains(input)
              where !necesidad.IdUsuarioCreador.Equals(idUser)
              where necesidad.Estado.Equals((int)TipoEstadoNecesidad.Activa)
              select necesidad
              ).OrderBy(o => o.FechaFin).ThenByDescending(o => o.Valoracion).ToList();
            return necesidadesObtenidas;
        }

        public List<Necesidades> ListarTodasLasNecesidadesActivas()
        {
            List<Necesidades> listadoNecesidades = new List<Necesidades>();

            var listaObtenida = (from nec in context.Necesidades
                                 where nec.FechaFin > DateTime.Now
                                 where nec.Estado == (int)TipoEstadoNecesidad.Activa
                                 select nec);

            foreach (var item in listaObtenida)
            {
                listadoNecesidades.Add(item);
            }

            return listadoNecesidades;

        }

        public List<Necesidades> TraerNecesidadesQueNoSonDelUsuario(int idSession)
        {
            List<Necesidades> listaNecesidades = new List<Necesidades>();

            var listaObtenida = (from nec in context.Necesidades
                                 where nec.FechaFin > DateTime.Now
                                 where nec.Estado.Equals((int)TipoEstadoNecesidad.Activa)
                                 where !nec.IdUsuarioCreador.Equals(idSession)
                                 select nec);

            foreach (var item in listaObtenida)
            {
                listaNecesidades.Add(item);
            }

            return listaNecesidades;
        }
    }
}
