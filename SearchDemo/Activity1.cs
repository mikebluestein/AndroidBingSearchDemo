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
using System.Net;
using System.IO;
using System.Json;
using SearchDemo.Core;

namespace CallRestService
{
    [Activity (Label = "CallRestService", MainLauncher = true)]
    public class Activity1 : ListActivity
    {
        List<SearchItem> results;

        protected override void OnCreate (Bundle bundle)
        {
            base.OnCreate (bundle);

            Search();
        }

        void Search()
        {
            var bing = new Bing (SyncToMain);
            bing.Search("Xamarin");
        }

        void SyncToMain (List<SearchItem> results)
        {
            RunOnUiThread(() =>
            {
                this.results = results;

				try{
                ListAdapter = new ArrayAdapter<SearchItem>(
                    this,
                    Resource.Layout.SearchItemView,
                    results
                );
				}
				catch(Java.Lang.NullPointerException){
					Console.WriteLine ("Did you add your Azure key to Bing.cs?");
				}
            });   
        }

        protected override void OnListItemClick (Android.Widget.ListView l, View v, int position, long id)
        {
            base.OnListItemClick (l, v, position, id);
            string url = results [position].Url;
            if (!string.IsNullOrEmpty (url)) {
                var uri = Android.Net.Uri.Parse (url);
                var intent = new Intent (Intent.ActionView, uri); 
                StartActivity (intent); 
            } else {
                Toast.MakeText (this, "No url available", ToastLength.Short);
            }
        }
    }
}


