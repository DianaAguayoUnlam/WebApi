﻿using DAO;
using Entidades;
using Entidades.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Web;

namespace Servicios
{
    public class ServicioUsuario
    {
        UsuarioDao usuarioDao = new UsuarioDao();
        
            public Usuarios obtenerUsuarioPorID(int idUsuario)
            {
            Usuarios usuarioObtenido = usuarioDao.ObtenerPorID(idUsuario);
            return usuarioObtenido;
            }

            public Usuarios obtenerUsuarioPorEmail(string email)
            {
            Usuarios usuarioObtenido = usuarioDao.obtenerUsuarioPorEmail(email);
            return usuarioObtenido;
            }

        public Usuarios asignoDatosAUsuarioDelRegistro(VMRegistro registro)
        {
            Usuarios usuario = new Usuarios()
            {
                Email = registro.Email,
                Password = EncriptarPassword.GetSha256(registro.Password),
                TipoUsuario = 1,
                Activo = false,
                FechaCracion = DateTime.Now,
                FechaNacimiento = registro.FechaNacimiento.AddHours(11).AddMinutes(04).AddSeconds(04)

            };

            return usuario;
        }

        public Usuarios asignoDatosAUsuarioDelLogin(VMLogin login)
        {
            Usuarios usuario = new Usuarios();
            usuario.Email = login.Email;
            usuario.Password = login.Password;
            return usuario;
        }



        public Usuarios asignoDatosAUsuarioDelPerfil(VMPerfil perfil)
        {
            Usuarios usuario = new Usuarios();
            usuario.Nombre = perfil.Nombre;
            usuario.Apellido = perfil.Apellido;
            usuario.Foto = perfil.Foto;
            usuario.UserName = perfil.Nombre + "." + perfil.Apellido;
            return usuario;
        }


        public string CodigoDeActivacion()
        {
            try
            {   //Generar un numero random para enviar el token al email del usuario
                Random random = new Random();
                const string allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890-/$%&!";
                int longitud = allowedChars.Length;
                string res = "";
                int maxLenght = random.Next(10, 30);

                for (int i = 0; i < maxLenght; i++)
                {
                    res += allowedChars[random.Next(longitud)];
                }

                return res;
            }
            catch (Exception)
            {
                throw new Exception("No se puede generar una cadena aleatoria");
            }
        }

        public String EnviarCodigoPorEmail(Usuarios usuario)
        {
            string emailEquipoCrear = "Equipoayudar@gmail.com";
            string host = "smtp.gmail.com";

            MailMessage email = new MailMessage();
            SmtpClient smtp = new SmtpClient();

            // A quien va dirigido
            email.To.Add(new MailAddress(usuario.Email)); //En Gmail: aca va una cuenta @gmail.com
            // Quien se lo envia
            email.From = new MailAddress(emailEquipoCrear); // Para enviar por gmail: Aca tiene que ir una cuenta de @gmail.com
            // Titulo del mensahe
            email.Subject = "Codigo de seguridad para activar mi cuenta";
            // Caracteres en UTF - 8 
            email.SubjectEncoding = System.Text.Encoding.UTF8;
            // Cuerpo del mensaje
            email.Body = " <h1> Bienvenido a nuestro sitio web Ayudar </h1> <p> Para activar tu email: " + usuario.Email + " tenes que ingresar al siguiente enlace: <h3><b>  https://localhost:44303/Usuario/ActivarMiCuenta?token=" + usuario.Token + "  </br> <h4> Equipo Ayudar - 2020 </h4> <br>";
            // Aca activo que acepte etiquetes html en el mensaje
            email.IsBodyHtml = true;
            // El envio tiene prioridad normal
            email.Priority = MailPriority.Normal;

            //Protocolo de mensajeria hecho por gmail
            smtp.Host = host; //Para enviar por gmail: smtp.gmail.com / Outlook: smtp.live.com
            //Puerto utilizado, recomendado
            smtp.Port = 587; /*SMTP | Port 587 (Transporte inseguro, pero se puede actualizar a una conexión segura usando STARTTLS)
                               SMTP | Port 465 (Transporte Seguro - función SSL habilitada) relentizó demaciado la app, entro en un bucle.
                              
                                INFO: El puerto 587 es un puerto alternativo altamente recomendado, porque los ISP (proveedores de Internet por sus
                               siglas en Inglés) suelen bloquear el puerto 25. Asegúrate de que has habilitado el STARTTLS al usar el puerto 587.*/
            // SSl disponible
            smtp.EnableSsl = true;
            // No tenemos credenciales por default
            smtp.UseDefaultCredentials = false;
            //Asigno el email y password utilizados para este caso
            smtp.Credentials = new NetworkCredential(emailEquipoCrear, "Aguayodelgadoirachetakarlen2020");      //Para enviar por gmail: cuenta @gmail.com

            string output;
            try
            {
                //Envio del mensaje
                smtp.Send(email);
                email.Dispose();
                //Asigno un ok para asegurarme en el servicio utilizado de que se envio el mensaje
                output = "Ok";
            }
            catch (Exception ex)
            {
                output = "Error enviando correo electrónico: " + ex.Message;
            }

            return output;
        }

        public string ReenviarEmail(Usuarios usuarioObtenido)
        {
            //Obtengo datos del usuario
            Usuarios usuarioRegistrado = obtenerUsuarioPorEmail(usuarioObtenido.Email);

            //Se le envia nuevamente su token al usuario ya registrado
            string mensajeEnviado = EnviarCodigoPorEmail(usuarioRegistrado);

            return mensajeEnviado;
        }

        public TipoEmail ValidoEstadoEmail(Usuarios usuario)
        {


            Usuarios usuarioObtenido = usuarioDao.obtenerUsuarioPorEmail(usuario.Email);

            //Si no se encontro usuario, el email es nuevo
            if (usuarioObtenido == null)
            {
                return TipoEmail.EmailNuevo;
            }
            else if (!usuarioObtenido.Activo) //Si el email aun no fue activo, entonces ya fue solicitado
            {
                return TipoEmail.EmailSolicitado;
            }
            else //Y sino, el email ya esta anotado
            {
                return TipoEmail.EmailActivo;
            }

        }

        public Usuarios ValidarCodigoDeActivacion(Usuarios usuario)
        {
            bool existeCodigo = true;
            UsuarioDao usuarioDao = new UsuarioDao();
            Usuarios usuarioObtenido = new Usuarios();
            ServicioUsuario servicioUsuario = new ServicioUsuario();
            do
            {
                usuario.Token = servicioUsuario.CodigoDeActivacion();

                usuarioObtenido = usuarioDao.obtenerUsuarioPorCodigoDeActivacion(usuario.Token);

                if (usuarioObtenido == null)
                {
                    usuarioObtenido = usuario;
                    existeCodigo = false;
                }
                else
                {
                    existeCodigo = true;
                }
            } while (existeCodigo != false);

            return usuarioObtenido;
        }

        public void SetearSession(Usuarios usuario)
        {
            Usuarios user = obtenerUsuarioPorEmail(usuario.Email);
            HttpContext.Current.Session["UserId"] = user.IdUsuario;
        }

        public int registrarUsuario(Usuarios usuario)
        {
            UsuarioDao usuarioDao = new UsuarioDao();

            //Validamos que se cree un codigo unico para cada usuario y que no se repita
            Usuarios usuarioObtenido = ValidarCodigoDeActivacion(usuario);

            //Save usuario
            Usuarios usuarioGuardado = usuarioDao.Guardar(usuarioObtenido);/***********************/

            if (usuarioGuardado.IdUsuario >= 0)
            {
                return usuarioGuardado.IdUsuario;
            }
            else
            {
                return -1;
            }
            
        }

        public string validoQueExistaEsteUsuario(Usuarios usuario)
        {
            UsuarioDao usuarioDao = new UsuarioDao();

            Usuarios usuarioObtenido = usuarioDao.obtenerUsuarioPorEmail(usuario.Email);
            if (usuarioObtenido == null)
            {
                return null;
            }
            String passwordEncriptada = EncriptarPassword.GetSha256(usuario.Password);
            if (usuarioObtenido.Password == passwordEncriptada)
            {
                return "ok";
            }

            return "incorrecto";
        }

        public bool validacionDelCodigoDeVerificacionJuntoAlEmail(string token)
        {
            UsuarioDao usuarioDao = new UsuarioDao();

            Usuarios usuarioConElToken = usuarioDao.obtenerUsuarioPorCodigoDeActivacion(token);

            //ToDo: Obtener objeto Usuario de la bd por el email ingresado y token ingresados

            if (usuarioConElToken != null)
            {
                usuarioConElToken.Activo = true;
                int resultado = usuarioDao.Actualizar(usuarioConElToken);/***********************/

                if (resultado >= 0)
                {
                    return true;
                }

            }

            return false;
        }

        public bool actualizoDatosDelPerfilDelUsuario(Usuarios usuario)
        {
            UsuarioDao usuarioDao = new UsuarioDao();

            //int resultado = usuarioDao.actualizarDatosDeUsuario(usuario); /***********************/
            int resultado = usuarioDao.Actualizar(usuario);
            if (resultado >= 0)
            {
                return true;
            }

            return false;
        }

        public Usuarios asignoDatosFaltantesAUsuarioDePerfil(Usuarios usuarioPerfil, Usuarios usuarioObtenido)
        {
            UsuarioDao usuarioDao = new UsuarioDao();

            usuarioObtenido.Nombre = usuarioPerfil.Nombre;
            usuarioObtenido.Apellido = usuarioPerfil.Apellido;
            usuarioObtenido.Foto = usuarioPerfil.Foto;

            List<Usuarios> listaUsuarios = usuarioDao.listadoUsuariosActivos();

            string nombreDeUsuario = null;
            int contador = 2;

            foreach (var item in listaUsuarios)
            {
                if (item.UserName == usuarioPerfil.UserName)
                {
                    //Le agrego un numero al nombre, ej: Steven.Gerard.3
                    nombreDeUsuario = usuarioPerfil.UserName + "." + contador;
                    //Se lo asigno a UsuarioPerfil
                    usuarioPerfil.UserName = nombreDeUsuario;
                    contador++;
                }
            }
            usuarioObtenido.UserName = usuarioPerfil.UserName;

            return usuarioObtenido;
        }

        public bool completoDatosDeMiPerfil(Usuarios usuarioPerfil)
        {
            //Obtengo el objeto usuario con los datos anteriores para agregarle los nuevos datos
            Usuarios usuarioObtenido = obtenerUsuarioPorEmail(usuarioPerfil.Email);

            //Agrego los datos faltantes al usuario obtenido de la bd
            Usuarios usuarioActualizado = asignoDatosFaltantesAUsuarioDePerfil(usuarioPerfil, usuarioObtenido);

            bool actualizado = actualizoDatosDelPerfilDelUsuario(usuarioActualizado);

            return actualizado;
        }

        public TipoUsuario tipoDeUsuario(Usuarios usuarioObtenido)
        {
            //Obtengo datos del usuario
            Usuarios usuarioRegistrado = obtenerUsuarioPorEmail(usuarioObtenido.Email);
            
            if(usuarioRegistrado.TipoUsuario == 1)
            {
                return TipoUsuario.Usuario;
            }
            else 
            {
                return TipoUsuario.Administrador;
            }
        }
    }
}
