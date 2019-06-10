using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using app_itsMarts.Models;

namespace app_itsMarts.Funciones
{
    public class General
    {
        public void GuardarXmlUsuario(Usuario usuario)
        {
            Java.IO.File archivo = new Java.IO.File(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "usuario.xml"));

            if (archivo.Exists())
                archivo.Delete();

            var xmlSerializador = new XmlSerializer(typeof(Usuario));
            var streamEscritura = new StreamWriter(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "usuario.xml"));
            xmlSerializador.Serialize(streamEscritura, usuario);
            streamEscritura.Close();
        }

        public Usuario ConsultaXmlUsuario()
        {
            var xmlSerializador = new XmlSerializer(typeof(Usuario));
            var streamLector = new StreamReader(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "usuario.xml"));
            var usuario = (Usuario)xmlSerializador.Deserialize(streamLector);
            return usuario;
        }

        public void MantenerInicioSesion()
        {
            var statusSesion = new StatusSesion();
            statusSesion.status = true;
            var xmlSerializador = new XmlSerializer(typeof(StatusSesion));
            var streamEscritura = new StreamWriter(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "status.xml"));
            xmlSerializador.Serialize(streamEscritura, statusSesion);
            streamEscritura.Close();
        }

        public bool ComprobarInisioSesion()
        {
            Java.IO.File archivo = new Java.IO.File(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "status.xml"));

            if (archivo.Exists())
                return true;
            else
                return false;
        }

        public void CerrarSesesion()
        {
            try
            {
                var pthStatus = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "status.xml");
                var pthUsuario = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "usuario.xml");

                Java.IO.File archivoStatus = new Java.IO.File(pthStatus);
                Java.IO.File archivoUsuario = new Java.IO.File(pthUsuario);

                if (archivoStatus.Exists() && archivoUsuario.Exists())
                {
                    archivoStatus.Delete();
                    archivoUsuario.Delete();
                }                    
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}