using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.OS;
using HackAtHome.Entities;

namespace HackAtHomeClient
{
    class Data : Fragment
    {
        public List<Evidence> Evidences { get; set; }
        public string Name { get; set; }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RetainInstance = true;
        }
    }
}