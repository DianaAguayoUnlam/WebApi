using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Entidades;
using Entidades.Views;
using DAO;
using DAO.Context;

namespace Servicios
{
    public class ServicioDonacion
    {

        DonacionMonetariaDao DonacionMonetariaDao ;

        public ServicioDonacion(TpDBContext context)
            {
            DonacionMonetariaDao = new DonacionMonetariaDao(context);
        }

       
        public decimal TotalRecaudado(int IdNeceDonacionMonetaria)
        {
            List<DonacionesMonetarias> lista = DonacionMonetariaDao.ObtenerPorId(IdNeceDonacionMonetaria);

            decimal sumatoria = lista.Sum(item => item.Dinero);
            return sumatoria;
        }


        public NecesidadesDonacionesMonetarias CantidadSolicitada(int IdNecesidadDonacionMonetaria)
        {
            return DonacionMonetariaDao.CantidadSolicitada(IdNecesidadDonacionMonetaria);
        }

        public decimal CalculoRestaDonacion(decimal Suma, decimal CantSolicitada)
        {
            decimal calculo = CantSolicitada - Suma;
            return calculo;
        }

        //ToDo: No va harcodeado el id de necesidadDonacionMonetaria, y idusuario
        public DonacionesMonetarias GuardarDonacionM(VMDonacionMonetaria VMDonacionMonetaria, int idUsuario)
        {
            DonacionesMonetarias donacionM = new DonacionesMonetarias()
            {
                Dinero = VMDonacionMonetaria.Dinero,
                IdNecesidadDonacionMonetaria = 5,
                IdUsuario = idUsuario,
                FechaCreacion = DateTime.Now,
                ArchivoTransferencia = ""
            };

            return DonacionMonetariaDao.Guardar(donacionM);

        }

    }
}