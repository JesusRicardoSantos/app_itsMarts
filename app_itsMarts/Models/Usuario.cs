using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace app_itsMarts.Models
{
    public class Usuario
    {
        public string Nombre { get; set; }
        public string A_paterno { get; set; }
        public string A_materno { get; set; }
        public string Telefono { get; set; }
        public int Cliente_sap { get; set; }
        public DateTime Fecha_creacion { get; set; }
        public string Nombre_fiscal { get; set; }
        public string Rfc { get; set; }
        public string Contacto { get; set; }
        public string Cedild { get; set; }
        public double Longitud { get; set; }
        public double Latitud { get; set; }
        public string Contrasena { get; set; }
        public string Referencia { get; set; }
        public string Foto_local { get; set; }
    }

}