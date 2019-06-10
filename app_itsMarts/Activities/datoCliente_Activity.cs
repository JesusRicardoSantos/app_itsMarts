using System;
using System.Collections.Generic;
using System.IO;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Widget;
using app_itsMarts.Funciones;
using Plugin.Geolocator;
using Refractored.Controls;
using Newtonsoft.Json;
using app_itsMarts.Models;
using app_itsMarts.Activities;

namespace app_itsMarts
{
    [Activity(Label = "datoCliente_Activity", Theme = "@style/AppTheme")]
    public class datoCliente_Activity : AppCompatActivity
    {
        //Pantalla para crear/editar cliente
        Site.Service service = new Site.Service();        

        #region Parametros para camara
        Intent intentImage;
        #endregion

        #region Widgets
        CircleImageView cimgTomarFoto;
        EditText edtNombre, edtApaterno, edtAmaterno, edtTelefono, edtCliente_sap, edtNombre_fiscal, edtRFC, edtContacto, edtReferencia, edtContrasena, edtContrasenaConfirmar;
        Button btnGuardar;
        Spinner spnCedId;
        LinearLayout lnPrincipal;

        ArrayAdapter adapterSpinCedId;
        #endregion Widgets

        #region mapa
        bool blnEditar = false;
        double dbllatitud, dbllongitud;
        #endregion;

        string strEdo = "";
        string strEncodingImage = "";
        bool blnCargarDatos = false;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.datoCliente_layout);

            #region Cazamiento de widgest
            edtNombre = FindViewById<EditText>(Resource.Id.edtNombre);
            edtApaterno = FindViewById<EditText>(Resource.Id.edtApaterno);
            edtAmaterno = FindViewById<EditText>(Resource.Id.edtAmaterno);
            edtTelefono = FindViewById<EditText>(Resource.Id.edtTelefono);
            edtCliente_sap = FindViewById<EditText>(Resource.Id.edtCliente_sap);
            edtNombre_fiscal = FindViewById<EditText>(Resource.Id.edtNombre_fiscal);
            edtRFC = FindViewById<EditText>(Resource.Id.edtRFC);
            edtContacto = FindViewById<EditText>(Resource.Id.edtContacto);
            edtReferencia = FindViewById<EditText>(Resource.Id.edtReferencia);
            edtContrasena = FindViewById<EditText>(Resource.Id.edtContrasena);
            edtContrasenaConfirmar = FindViewById<EditText>(Resource.Id.edtContrasenaConfirmar);
            cimgTomarFoto = FindViewById<CircleImageView>(Resource.Id.cimgTomarFoto);
            btnGuardar = FindViewById<Button>(Resource.Id.btnGuardar);
            lnPrincipal = FindViewById<LinearLayout>(Resource.Id.lnPrincipal);
            spnCedId = FindViewById<Spinner>(Resource.Id.spinEdo);
            #endregion

            #region Adaptador spinner
            spnCedId.SetPromptId(Resource.String.Seleccion_edo);

            spnCedId.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spin_ItemSelected);
            adapterSpinCedId = ArrayAdapter.CreateFromResource(this, Resource.Array.Estados, Android.Resource.Layout.SimpleSpinnerDropDownItem);

            adapterSpinCedId.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spnCedId.Adapter = adapterSpinCedId;
            #endregion

            #region Geo-Localizacion
            var location = CrossGeolocator.Current;
            location.DesiredAccuracy = 50;//50 metros
            var position = await location.GetPositionAsync(TimeSpan.FromSeconds(10), null, true);
            dbllatitud = position.Latitude;
            dbllongitud = position.Longitude;
            #endregion
            
            cimgTomarFoto.Click += delegate {
                intentImage = new Intent(MediaStore.ActionImageCapture);
                StartActivityForResult(intentImage, 0);
            };

            btnGuardar.Click += async delegate {

                try
                {
                    if (string.IsNullOrEmpty(edtNombre.Text))
                    {
                        Toast.MakeText(this, Resource.String.Nombre_nulo, ToastLength.Short).Show();
                        edtNombre.Focusable = true;
                    }
                    else if (string.IsNullOrEmpty(edtApaterno.Text))
                    {
                        Toast.MakeText(this, Resource.String.Apaterno_nulo, ToastLength.Short).Show();
                        edtAmaterno.Focusable = true;
                    }
                    else if (string.IsNullOrEmpty(edtAmaterno.Text))
                    {
                        Toast.MakeText(this, Resource.String.Amaterno_nulo, ToastLength.Short).Show();
                        edtAmaterno.Focusable = true;
                    }
                    else if (string.IsNullOrEmpty(edtCliente_sap.Text))
                    {
                        Toast.MakeText(this, Resource.String.Cliente_sap_nulo, ToastLength.Short).Show();
                        edtCliente_sap.Focusable = true;
                    }
                    else if (string.IsNullOrEmpty(edtNombre_fiscal.Text))
                    {
                        Toast.MakeText(this, Resource.String.Nombre_fiscal_nulo, ToastLength.Short).Show();
                        edtNombre_fiscal.Focusable = true;
                    }
                    else if (string.IsNullOrEmpty(edtRFC.Text))
                    {
                        Toast.MakeText(this, Resource.String.RFC_nulo, ToastLength.Short).Show();
                        edtRFC.Focusable = true;
                    }
                    else if (string.IsNullOrEmpty(edtContrasena.Text))
                    {
                        Toast.MakeText(this, Resource.String.Contrasena_nulo, ToastLength.Short).Show();
                        edtContrasena.Focusable = true;
                    }
                    else if (string.IsNullOrEmpty(edtContrasenaConfirmar.Text))
                    {
                        Toast.MakeText(this, Resource.String.Contrasena_nulo, ToastLength.Short).Show();
                        edtContrasenaConfirmar.Focusable = true;
                    }
                    else if (edtRFC.Text.Length != 13)
                    {
                        Toast.MakeText(this, Resource.String.Extencion_RFC, ToastLength.Short).Show();
                        edtRFC.Text = "";
                        edtRFC.Focusable = true;
                    }
                    else if (edtTelefono.Text.Length != 10)
                    {
                        Toast.MakeText(this, Resource.String.Extencion_Telefono, ToastLength.Short).Show();
                        edtTelefono.Text = "";
                        edtTelefono.Focusable = true;
                    }
                    else if (edtContrasena.Text != edtContrasenaConfirmar.Text)
                    {
                        Toast.MakeText(this, Resource.String.Error_contrasenas_iguales, ToastLength.Short).Show();
                    }
                    else
                    {
                        List<Usuario> lstUsuario = new List<Usuario>();
                        lstUsuario.Add(new Usuario
                        {
                            Nombre = edtNombre.Text,
                            A_paterno = edtApaterno.Text,
                            A_materno = edtAmaterno.Text,
                            Telefono = "+(52)" + edtTelefono.Text,
                            Cliente_sap = int.Parse(edtCliente_sap.Text),
                            Fecha_creacion = DateTime.Today,
                            Nombre_fiscal = edtNombre_fiscal.Text,
                            Rfc = edtRFC.Text,
                            Contacto = edtContacto.Text,
                            Cedild = strEdo,
                            Longitud = dbllongitud,
                            Latitud = dbllatitud,
                            Contrasena = edtContrasena.Text,
                            Referencia = edtReferencia.Text,
                            Foto_local = strEncodingImage
                        });

                        var jsonBody = JsonConvert.SerializeObject(lstUsuario[0]);

                        if (blnEditar)
                        {
                            //Actualizar usuario
                            service.ActualizarCuentaCompleted += (s, e) => {

                                var intent = new Intent(this, typeof(detalleCliente_Activity));
                                RespuestasDelWebService(e.Result, intent);

                                General general = new General();
                                general.GuardarXmlUsuario(lstUsuario[0]);

                            };

                            service.ActualizarCuentaAsync(jsonBody);
                        }
                        else
                        {
                            //Crear usuario
                            service.CrearCuentaCompleted += (s, e) => {

                                var intent = new Intent(this, typeof(MainActivity));
                                RespuestasDelWebService(e.Result, intent);

                            };
                            service.CrearCuentaAsync(jsonBody);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Toast.MakeText(this, Resource.String.Error_catch, ToastLength.Short).Show();
                    Console.WriteLine("Error: " + ex.Message);
                }

            };
        }

        protected override void OnStart()
        {
            base.OnStart();
            blnEditar = Intent.Extras.GetBoolean("Editar");

            //si bln es true, es editar usuario, sino es crear uno nuevo
            if (blnEditar && !blnCargarDatos)
                CargarDatos();
        }

        private void RespuestasDelWebService(int intResult, Intent intent)
        {
            switch (intResult)
            {
                case 0: //->OK
                    {
                        Toast.MakeText(this, Resource.String.Operacion_exitosa, ToastLength.Short).Show();
                        StartActivity(intent);
                    }
                    break;
                case 1: //-> Id cliente repetido
                    {
                        Toast.MakeText(this, Resource.String.IdCliente_repetido, ToastLength.Short).Show();
                        edtCliente_sap.Focusable = true;
                        edtCliente_sap.Text = "";
                    }
                    break;
                case 2: //Nombre fiscal repetido
                    {
                        Toast.MakeText(this, Resource.String.NombreFiscal_repetido, ToastLength.Short).Show();
                        edtNombre_fiscal.Focusable = true;
                        edtNombre_fiscal.Text = "";
                    }
                    break;
                case 3: // RFC repetido
                    {
                        Toast.MakeText(this, Resource.String.RFC_repetido, ToastLength.Short).Show();
                        edtRFC.Focusable = true;
                        edtRFC.Text = "";
                    }
                    break;
            }
        }

        private void CargarDatos()
        {
            General general = new General();
            var datosUsuario = general.ConsultaXmlUsuario();

            //Bloque foto
            strEncodingImage = datosUsuario.Foto_local;
            if (!string.IsNullOrEmpty(datosUsuario.Foto_local))
            {
                byte[] bytArray = Android.Util.Base64.Decode(datosUsuario.Foto_local, Android.Util.Base64Flags.Default);
                var bitmapdecodeByte = BitmapFactory.DecodeByteArray(bytArray, 0, bytArray.Length);

                cimgTomarFoto.SetImageBitmap(bitmapdecodeByte);
            }

            //Bloque Info Basica
            edtNombre.Text = datosUsuario.Nombre;
            edtApaterno.Text = datosUsuario.A_paterno;
            edtAmaterno.Text = datosUsuario.A_materno;
            edtContrasena.Text = datosUsuario.Contrasena;
            edtContrasenaConfirmar.Text = datosUsuario.Contrasena;

            //Contacto
            edtTelefono.Text = datosUsuario.Telefono.Substring(5, 10);
            edtContacto.Text = datosUsuario.Contacto;
            edtReferencia.Text = datosUsuario.Referencia;

            //Datos del negocio
            edtCliente_sap.Text = datosUsuario.Cliente_sap.ToString();
            edtNombre_fiscal.Text = datosUsuario.Nombre_fiscal;
            edtRFC.Text = datosUsuario.Rfc;

            for (int i = 0; i < adapterSpinCedId.Count; i++)
            {
                if (adapterSpinCedId.GetItem(i).ToString().Contains(datosUsuario.Cedild))
                {
                    spnCedId.SetSelection(i);
                    break;
                }
            }
            blnCargarDatos = true;
            // - - - Mapa - - -
            //datosUsuario.Longitud;
            //datosUsuario.Latitud;
        }

        private void spin_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            var spinItemSelected = sender as Spinner;
            strEdo = spinItemSelected.GetItemAtPosition(e.Position).ToString();
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            try
            {
                base.OnActivityResult(requestCode, resultCode, data);
                
                var bitmap = (Bitmap)data.Extras.Get("data");
                cimgTomarFoto.SetImageBitmap(bitmap);
                
                var uriRutaArchivo = Android.Net.Uri.Parse(System.IO.Path.Combine
                                (System.Environment.GetFolderPath
                                    (System.Environment.SpecialFolder.Personal), "capture.jpg"));

                if (File.Exists(uriRutaArchivo.ToString()))
                {
                    var fileDelete = new Java.IO.File(uriRutaArchivo.ToString());
                    fileDelete.Delete();
                }

                var memoryStream = new MemoryStream();
                bitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, memoryStream);
                byte[] bytArray = memoryStream.ToArray();
                strEncodingImage = Android.Util.Base64.EncodeToString(bytArray, Android.Util.Base64Flags.Default);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Toast.MakeText(this, Resource.String.Error_catch, ToastLength.Short).Show();
            }
            
        }
    }
}