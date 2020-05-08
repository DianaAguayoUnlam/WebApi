namespace WebCovid19
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class NecesidadesValoraciones
    {
        [Key]
        public int IdValoracion { get; set; }

        public int IdUsuario { get; set; }

        public int IdNecesidad { get; set; }

        public bool Valoracion { get; set; }

        public virtual Necesidades Necesidades { get; set; }

        public virtual Usuarios Usuarios { get; set; }
    }
}