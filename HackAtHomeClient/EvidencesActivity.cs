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
using HackAtHome.CustomAdapters;
using HackAtHome.Entities;
using HackAtHome.SAL;

namespace HackAtHomeClient
{
    [Activity(Label = "@string/ApplicationName", Icon = "@drawable/icon", Theme = "@android:style/Theme.Holo.Light.DarkActionBar")]
    public class EvidencesActivity : Activity
    {
        private Data data;
        private List<Evidence> evidences;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.ActivityEvidences);

            //Variables de UI:
            var lvEvidences = FindViewById<ListView>(Resource.Id.lvEvidences);
            var tvName = FindViewById<TextView>(Resource.Id.tvName);

            //Adquirimos nombre y token:
            var token = Intent.GetStringExtra("token");
            var name = Intent.GetStringExtra("name");

            //Revisamos si ya hay datos guardados en un fragment:
            data = (Data) this.FragmentManager.FindFragmentByTag("Data");
            if (data == null) //Si no hay fragment recuperamos la información y lo creamos:
            {
                //Guardamos el fragment:
                data = new Data();
                var transaction = this.FragmentManager.BeginTransaction();
                transaction.Add(data, "Data");
                transaction.Commit();

                //Adquirimos los datos:
                ServiceClient client = new ServiceClient();
                var evidencias = await client.GetEvidencesAsync(token);

                //Establecemos los datos adquiridos al fragment:
                data.Evidences = evidencias;
                data.Name = name;
                evidences = data.Evidences;
            }

            //Ponemos el nombre del alumno:
            tvName.Text = data.Name;

            //Ponemos adaptador a la lista, en base a la orientación ponemos su plantilla:
            if (!IsLandScape()) //Si está en modo vertical
            {
                lvEvidences.Adapter = new EvidencesAdapter(this, data.Evidences,
                    Resource.Layout.lvTemplatePort, Resource.Id.tvEvidenceNamePort,
                    Resource.Id.tvEvidenceStatusPort);
            }
            else //Modo horizontal:
            {
                {
                    lvEvidences.Adapter = new EvidencesAdapter(this, data.Evidences,
                        Resource.Layout.lvTemplateLand, Resource.Id.tvEvidenceNameLand,
                        Resource.Id.tvEvidenceStatusLand);
                }
            }

            //Evento para click en un item de la lista:
            lvEvidences.ItemClick += (sender, args) =>
            {
                    var item = data.Evidences.ElementAt(args.Position);

                    var intent = new Intent(this, typeof(EvidenceDetailActivity));
                    intent.PutExtra("name", name);
                    intent.PutExtra("title", item.Title);
                    intent.PutExtra("token", token);
                    intent.PutExtra("status", item.Status);
                    intent.PutExtra("id", item.EvidenceID);

                    StartActivity(intent);
            };
        }

        private bool IsLandScape()
        {
            var orientation = this.WindowManager.DefaultDisplay.Rotation;
            return orientation == SurfaceOrientation.Rotation90 ||
                   orientation == SurfaceOrientation.Rotation270;
        }
    }
}