using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using HackAtHome.SAL;

namespace HackAtHomeClient
{
    [Activity(Label = "@string/ApplicationName", Icon = "@drawable/icon", Theme = "@android:style/Theme.Holo.Light.DarkActionBar")]
    public class EvidenceDetailActivity : Activity
    {
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.ActivityEvidenceDetail);

            //Variables de UI:
            var tvNameDetail = FindViewById<TextView>(Resource.Id.tvNameDetail);
            var tvEvidenceName = FindViewById<TextView>(Resource.Id.tvEvidenceName);
            var tvEvidenceStatus = FindViewById<TextView>(Resource.Id.tvEvidenceStatus);
            var wvDescription = FindViewById<WebView>(Resource.Id.wvDescription);
            var ivEvidence = FindViewById<ImageView>(Resource.Id.ivEvidencia);

            //Obtenemos nombre, título, token, status y id:
            var name = Intent.GetStringExtra("name");
            var title = Intent.GetStringExtra("title");
            var token = Intent.GetStringExtra("token");
            var status = Intent.GetStringExtra("status");
            var evidenceId = Intent.GetIntExtra("id", 0);

            //Ponemos los datos que tenemos:
            tvNameDetail.Text = name;
            tvEvidenceName.Text = title;
            tvEvidenceStatus.Text = status;

            //Obtenemos la descripción y la URL:
            var client = new ServiceClient();
            var evidenceDetails = await client.GetEvidenceByIDAsync(token, evidenceId);

            //Establecemos el contenido de la descripción en el WebView:
            wvDescription.LoadDataWithBaseURL(null, evidenceDetails.Description, "text/html", "utf-8", null);

            //Establecemos la imagen a partir de la URL:
            Koush.UrlImageViewHelper.SetUrlDrawable(ivEvidence, evidenceDetails.Url);
        }
    }
}