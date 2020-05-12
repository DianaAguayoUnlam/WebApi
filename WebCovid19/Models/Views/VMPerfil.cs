﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebCovid19.Models.Views
{
    public class VMPerfil
    {
        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "Nombre obligatorio")]
        [StringLength(50)]
        public string Nombre { get; set; }

        [Display(Name = "Apellido")]
        [Required(ErrorMessage = "Apellido obligatorio")]
        [StringLength(50)]
        public string Apellido { get; set; }

        [Display(Name = "Fecha de nacimiento")]
        [Required(ErrorMessage = "Fecha de nacimiento obligatoria")]
        public DateTime FechaNacimiento { get; set; }

        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Formato de Email erroneo")]
        [StringLength(50, ErrorMessage = "Email demaciado largo")]
        public string Email { get; set; }

        [Display(Name = "Foto de perfil")]
        [Required(ErrorMessage = "Foto de perfil obligatoria")]
        [StringLength(100)]
        public string Foto { get; set; }
    }
}