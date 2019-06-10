using System;
using Android.App;
using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using app_itsMarts.Funciones;
using Refractored.Controls;

namespace app_itsMarts.Activities
{
    [Activity(Theme = "@style/AppTheme")]
    public class detalleCliente_Activity : AppCompatActivity, IOnMapReadyCallback
    {
        //Pantalla para ver datos del cliente
        GoogleMap map;
        LocationManager locationManager;
        String provider;

        CircleImageView cimgVisualizarFoto;
        TextView txtNombre, txtAPaterno, txtAMaterno, txtContrasena, txtTelefono, txtContacto, txtReferencia, txtCliente_sap, txtNombre_fiscal, txtRFC, txtEstado, txtFechaCreacion;
        double dbllat, dbllon;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.detalleCliente_layout);
            #region Casamiento de widgets

            txtNombre = FindViewById<TextView>(Resource.Id.txtNombre);
            txtAPaterno = FindViewById<TextView>(Resource.Id.txtAPaterno);
            txtAMaterno = FindViewById<TextView>(Resource.Id.txtAMaterno);
            txtContrasena = FindViewById<TextView>(Resource.Id.txtContrasena);
            txtTelefono = FindViewById<TextView>(Resource.Id.txtTelefono);
            txtContacto = FindViewById<TextView>(Resource.Id.txtContacto);
            txtReferencia = FindViewById<TextView>(Resource.Id.txtReferencia);
            txtCliente_sap = FindViewById<TextView>(Resource.Id.txtCliente_sap);
            txtNombre_fiscal = FindViewById<TextView>(Resource.Id.txtNombre_fiscal);
            txtRFC = FindViewById<TextView>(Resource.Id.txtRFC);
            txtEstado = FindViewById<TextView>(Resource.Id.txtEstado);
            txtFechaCreacion = FindViewById<TextView>(Resource.Id.txtFechaCreacion);
            cimgVisualizarFoto = FindViewById<CircleImageView>(Resource.Id.cimgVisualizarFoto);
            
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            toolbar.SetTitleTextColor(Android.Graphics.Color.White);
            toolbar.SetTitle(Resource.String.Usuario);
            SetSupportActionBar(toolbar);

            MapFragment mapFragment = (MapFragment)FragmentManager.FindFragmentById(Resource.Id.mapVer);
            mapFragment.GetMapAsync(this);

            locationManager = (LocationManager)GetSystemService(Context.LocationService);
            provider = locationManager.GetBestProvider(new Criteria(), false);

            Location location = locationManager.GetLastKnownLocation(provider);
            if (location == null)
                System.Diagnostics.Debug.WriteLine("No es posible obtener la localizacion");
            #endregion
            
            CargarDatos();
        }
        
        private void CargarDatos()
        {
            General general = new General();
            var datosUsuario = general.ConsultaXmlUsuario();

            //Bloque foto
            var strEncodingImage = datosUsuario.Foto_local;
            if (!string.IsNullOrEmpty(datosUsuario.Foto_local))
            {
                byte[] bytArray = Android.Util.Base64.Decode(datosUsuario.Foto_local, Android.Util.Base64Flags.Default);
                var bitmapdecodeByte = BitmapFactory.DecodeByteArray(bytArray, 0, bytArray.Length);

                cimgVisualizarFoto.SetImageBitmap(bitmapdecodeByte);
            }

            //Bloque Info Basica
            txtNombre.Text = datosUsuario.Nombre;

            txtAPaterno.Text = datosUsuario.A_paterno;
            txtAMaterno.Text = datosUsuario.A_materno;
            txtContrasena.Text = datosUsuario.Contrasena;

            //Contacto
            txtTelefono.Text = datosUsuario.Telefono;
            txtContacto.Text = datosUsuario.Contacto;
            txtReferencia.Text = datosUsuario.Referencia;

            //Datos del negocio
            txtCliente_sap.Text = datosUsuario.Cliente_sap.ToString();
            txtNombre_fiscal.Text = datosUsuario.Nombre_fiscal;
            txtRFC.Text = datosUsuario.Rfc;
            txtEstado.Text = datosUsuario.Cedild;
            txtFechaCreacion.Text = datosUsuario.Fecha_creacion.ToString();

            // - - - Mapa - - -
            dbllat = datosUsuario.Latitud;
            dbllon = datosUsuario.Longitud;

        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            var inflate = MenuInflater;
            inflate.Inflate(Resource.Menu.toolbar, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.itEditar)
            {
                var intent = new Intent(this, typeof(datoCliente_Activity));
                
                //True indica a datoCliente que quiere editar, si no, significa que va a dar de alta algun cliente
                intent.PutExtra("Editar", true);
                StartActivity(intent);
            }
            else
            {
                General general = new General();
                general.CerrarSesesion();
                Toast.MakeText(this, "bye", ToastLength.Short).Show();

                var intent = new Intent(this, typeof(MainActivity));
                StartActivity(intent);
                Finish();
            }

            return base.OnOptionsItemSelected(item);
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            map = googleMap;

            MarkerOptions markerOptions = new MarkerOptions();
            markerOptions.SetPosition(new LatLng(dbllat, dbllon));
            map.AddMarker(markerOptions);

            //Mover camara
            CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
            builder.Target(new LatLng(dbllat, dbllon));
            builder.Zoom(15);
            CameraPosition cameraPosition = builder.Build();
            CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);
            map.AnimateCamera(cameraUpdate);
            map.MapType = GoogleMap.MapTypeTerrain;

            googleMap.UiSettings.ZoomControlsEnabled = false;
            googleMap.UiSettings.CompassEnabled = true;
            googleMap.UiSettings.ScrollGesturesEnabled = true;
        }

        public override void OnBackPressed()
        {
            
        }
    }
}