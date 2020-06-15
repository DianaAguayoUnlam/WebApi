using DAO.Context;
using Entidades;
using Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApi.Models;

namespace WebApi.Controllers
{
    public class NecesidadesController : ApiController
    {
        
        ServicioNecesidad necesidadServicio;

        public NecesidadesController()
        {
            TpDBContext ctx = new TpDBContext();
            necesidadServicio = new ServicioNecesidad(ctx);
        }
        public List<NecesidadesDTO> Get()
        {
            
            //lista que obtengo de la bd
            List<Necesidades> listaNecesidadesEF = necesidadServicio.ListarTodasLasNecesidades();
            //lista que quiero retornar
            /* List<NecesidadesDTO> listaNecesidadesDTO = new List<NecesidadesDTO>();

             //mapeamos las necesidades a DTO y las agregamos a la lista que quiero retornar
             foreach (Necesidades necesidadEF in listaNecesidadesEF)
             {
                 listaNecesidadesDTO.Add(new NecesidadesDTO(necesidadEF));
             }*/

            //devuelve una lista de necesidades DTO
            return NecesidadesDTO.MapearListaEF(listaNecesidadesEF);
        }

    }
}
