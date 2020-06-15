using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models
{
    public class NecesidadesDTO
    {
        /* public int IdNecesidad { get; set; }
         public string Nombre { get; set; }
         public string Descripcion { get; set; }*/


        public int IdNecesidad { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public System.DateTime FechaCreacion { get; set; }
        public System.DateTime FechaFin { get; set; }
        public string TelefonoContacto { get; set; }
        public int TipoDonacion { get; set; }
        public string Foto { get; set; }
        public int IdUsuarioCreador { get; set; }
        public int Estado { get; set; }
        public Nullable<decimal> Valoracion { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Denuncias> Denuncias { get; set; }
        public virtual Usuarios Usuarios { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public List<NecesidadesDonacionesInsumosDTO> NecesidadesDonacionesInsumos { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NecesidadesDonacionesMonetarias> NecesidadesDonacionesMonetarias { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NecesidadesReferencias> NecesidadesReferencias { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NecesidadesValoraciones> NecesidadesValoraciones { get; set; }

        public NecesidadesDTO()
        {
        }

        //mapeamos necesidades de Entity a necesidades DTO}
        //cuando recupero un objeto de la BD y quiero mostrarlo o devolverlo con la api

        public NecesidadesDTO(Necesidades necesidadesEntidad)
        {
            this.IdNecesidad = necesidadesEntidad.IdNecesidad;
            this.Nombre = necesidadesEntidad.Nombre;
            this.Descripcion = necesidadesEntidad.Descripcion;
            this.TelefonoContacto = necesidadesEntidad.TelefonoContacto;
            this.TipoDonacion = necesidadesEntidad.TipoDonacion;
            this.Foto = necesidadesEntidad.Foto;
            this.IdUsuarioCreador = necesidadesEntidad.IdUsuarioCreador;
            this.Estado = necesidadesEntidad.Estado;
            this.Valoracion = necesidadesEntidad.Valoracion;
            //this.Denuncias = necesidadesEntidad.Denuncias;
            //this.Usuarios = necesidadesEntidad.Usuarios;
            this.NecesidadesDonacionesInsumos = NecesidadesDonacionesInsumosDTO.MapearListaEF(necesidadesEntidad.NecesidadesDonacionesInsumos.ToList(), false);
             }

        //mapeamos necesidades DTO a necesidades EF}
        //cuando quiero enviar a la BD algo que llega a traves de la api

        public Necesidades MapearEF()
        {
            Necesidades necesidades = new Necesidades();
            necesidades.IdNecesidad = this.IdNecesidad;
            necesidades.Nombre = this.Nombre;
            necesidades.Descripcion = this.Descripcion;
          //  necesidades.NecesidadesDonacionesInsumos = this.NecesidadesDonacionesInsumos.);

            return necesidades;
            
        }



        public static List<Necesidades> MapearListaDTO(List<NecesidadesDTO> listaNecesidadesDTO)
        {
            List<Necesidades> listaNecesidadesEF = new List<Necesidades>();

            //mapeamos las necesidades a EF y las agregamos a la lista que quiero retornar
            foreach (NecesidadesDTO necesidadDTO in listaNecesidadesDTO)
            {
                listaNecesidadesEF.Add(necesidadDTO.MapearEF());
            }

            return listaNecesidadesEF;
        }

        public static List<NecesidadesDTO> MapearListaEF(List<Necesidades> listaNecesidadesEF)
        {
            List<NecesidadesDTO> listaNecesidadesDTO = new List<NecesidadesDTO>();

           //mapeamos las necesidades a DTO y las agregamos a la lista que quiero retornar
            foreach (Necesidades necesidadEF in listaNecesidadesEF)
            {
                listaNecesidadesDTO.Add(new NecesidadesDTO(necesidadEF) );
            }

           return listaNecesidadesDTO;
        }
    }
}