using Android.App;
using Android.Content;
using Android.Widget;
using Android.OS;
using HackAtHome.Entities;
using HackAtHome.SAL;

namespace HackAtHomeClient
{
    [Activity(Label = "@string/ApplicationName", MainLauncher = true, Icon = "@drawable/icon", Theme = "@android:style/Theme.Holo.Light.DarkActionBar")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            
            // Set our view from the "main" layout resource
            SetContentView (Resource.Layout.Main);

            //Variables de UI:
            var etEmail = FindViewById<EditText>(Resource.Id.etEmail);
            var etPassword = FindViewById<EditText>(Resource.Id.etPassword);
            var btnValidar = FindViewById<Button>(Resource.Id.btnValidar);

            //Evento de click al botón:
            btnValidar.Click += async (sender, e) =>
            {
                //Autenticación en TI Capacitacion:
                var client = new ServiceClient();
                var result = await client.AutenticateAsync(etEmail.Text, etPassword.Text);

                //Verificar el estado de la autenticación:
                switch (result.Status)
                {
                    case Status.Error: //Error:
                        CrearDialogo(0);
                        break;

                    case Status.Success: //Success:
                        //Envio de evidencia e intent:
                        var MicrosoftEvidence = new LabItem
                        {
                            Email = etEmail.Text,
                            Lab = "Hack@Home",
                            DeviceId = Android.Provider.Settings.Secure.GetString(
                                ContentResolver, Android.Provider.Settings.Secure.AndroidId)
                        };

                        var MicrosoftClient = new MicrosoftServiceClient();
                        await MicrosoftClient.SendEvidence(MicrosoftEvidence);

                        var intent = new Intent(this, typeof(EvidencesActivity));
                        intent.PutExtra("token", result.Token);
                        intent.PutExtra("name", result.FullName);

                        StartActivity(intent);

                        break;

                    case Status.InvalidUserOrNotInEvent: //InvalidUserOrNotInEvent
                        CrearDialogo(2);
                        break;

                    case Status.OutOfDate:
                        CrearDialogo(3);
                        break;
                }
            };
        }

        private void CrearDialogo(int mensaje)
        {
            var builder = new AlertDialog.Builder(this);
            AlertDialog alert = builder.Create();

            alert.SetTitle(Resources.GetString(Resource.String.ResultDialogTitle));
            
            //Establecer un mensaje según el código de mensaje:
            switch (mensaje)
            {
                case 0:
                    alert.SetMessage(Resources.GetString(Resource.String.Error0));
                    break;

                case 2:
                    alert.SetMessage("InvalidUserOrNotInEvent");
                    break;

                case 3:
                    alert.SetMessage("OutOfDate");
                    break;
            }

            alert.SetButton("Ok", (s, e) => { });
            alert.Show();
        }
    }
}

