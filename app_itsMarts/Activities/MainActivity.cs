using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Content;
using System;
using Android.Support.Design.Widget;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using app_itsMarts.Models;
using System.Collections.Generic;
using app_itsMarts.Activities;
using app_itsMarts.Funciones;

namespace app_itsMarts
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        EditText edtIdUsuario, edtContrasena;
        TextView txtIniciarSesion, txtCrearCuenta;
        CheckBox chkRecordarSesion;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState); 
            SetContentView(Resource.Layout.activity_main);

            #region Casamiento de widgets
            edtIdUsuario = FindViewById<EditText>(Resource.Id.edtIdUsuario);
            edtContrasena = FindViewById<EditText>(Resource.Id.edtContrasena);
            txtIniciarSesion = FindViewById<TextView>(Resource.Id.txtIniciarSesion);
            txtCrearCuenta = FindViewById<TextView>(Resource.Id.txtCrearCuenta);
            chkRecordarSesion = FindViewById<CheckBox>(Resource.Id.chkRecordarSesion);
            #endregion

            txtIniciarSesion.Click += TxtIniciarSesion_Click;
            txtCrearCuenta.Click += TxtCrearCuenta_Click;
        }

        private void TxtCrearCuenta_Click(object sender, EventArgs e)
        {
            //Abrimos la pantalla para registrar usuario
            var intent = new Intent(this, typeof(datoCliente_Activity));

            //pasamos la bandera Action para indicar que es un nuevo usuario
            intent.PutExtra("Editar", false);
            StartActivity(intent);
        }

        private void TxtIniciarSesion_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(edtIdUsuario.Text))
                {
                    Toast.MakeText(this, Resource.String.Id_nulo, ToastLength.Short).Show();
                    edtIdUsuario.Focusable = true;
                }
                else if (string.IsNullOrEmpty(edtContrasena.Text))
                {
                    Toast.MakeText(this, Resource.String.Contrasena_nulo, ToastLength.Short).Show();
                    edtIdUsuario.Focusable = true;
                }
                else
                {
                    //Creamos instancia del servicio web
                    Site.Service service = new Site.Service();

                    //Inbocamos metodo del ws y pasamos los parametros de para el metodo de login
                    List<Usuario> lstUsuario = new List<Usuario>();
                    lstUsuario.Add(new Usuario { Cliente_sap = int.Parse(edtIdUsuario.Text), Contrasena = edtContrasena.Text });
                    var jsonBody = JsonConvert.SerializeObject(lstUsuario[0]);

                    service.LoginCompleted += (objsender, eventArgsComplete) =>
                    {
                        if (eventArgsComplete.Result.Equals("[]"))
                            Toast.MakeText(this, Resource.String.Error_validacion, ToastLength.Short).Show();
                        else
                        {
                            //Hacemos instancia de la calse GENERAL para hacer uso de un metodo
                            //para guardar datos de usuario en xml, ademas de validar el checbox
                            //si es que el usuario desea cuardar credenciales de inicio de sesion

                            General general = new General();
                            var JsonUsuario = JsonConvert.DeserializeObject<List<Usuario>>(eventArgsComplete.Result);
                            JsonUsuario[0].Contrasena = edtContrasena.Text;

                            general.GuardarXmlUsuario(JsonUsuario[0]);

                            if (chkRecordarSesion.Checked) //Si esta activo el usuario desea guardar usuario y contrasena
                                general.MantenerInicioSesion();

                            var intent = new Intent(this, typeof(detalleCliente_Activity));
                            StartActivity(intent);
                        }
                    };
                    service.LoginAsync(jsonBody);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                Toast.MakeText(this, Resource.String.Error_catch, ToastLength.Long).Show();
            }
        }

        protected override void OnStart()
        {
            base.OnStart();

            try
            {
                General general = new General();

                if (general.ComprobarInisioSesion())
                {
                    //Iniciamos pantalla de detalleCliente_Activity
                    var intent = new Intent(this, typeof(detalleCliente_Activity));
                    StartActivity(intent);
                    Finish();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
    }
}