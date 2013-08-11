using System;
using System.Collections.Generic;
using System.Threading;
using System.Net;
using System.Xml;
using System.Xml.Linq;
using System.Linq;
using System.Json;
using System.IO;
using System.Text;

#if iOS
using MonoTouch.Foundation;
using MonoTouch.AVFoundation;
#endif

namespace SearchDemo.Core
{
    public delegate void SynchronizerDelegate (List<SearchItem> results);

    public class Bing
    {
        #region API Keys
        // You can obtain these API keys from the Windows Azure Marketplace
        // https://datamarket.azure.com/browse/data
		const string MS_TRANSLATOR_API_ID = "ENTER_YOUR_KEY";
		const string AZURE_KEY = "ENTER_YOUR_KEY";
        #endregion

        static SynchronizerDelegate sync;
#if iOS
        AVAudioPlayer audioPlayer;
#endif

        public Bing ()
        {
        }
        
        public Bing (SynchronizerDelegate sync)
        {
            Bing.sync = sync;
        }

        public void Search (string text)
        {
            var t = new Thread (Search);
            t.Start (text);
        }

        void Search (object text)
        {
            string bingSearch = String.Format ("https://api.datamarket.azure.com/Data.ashx/Bing/Search/v1/Web?Query=%27{0}%27&$top=10&$format=Json", text);

            var httpReq = (HttpWebRequest)HttpWebRequest.Create (new Uri (bingSearch));

            httpReq.Credentials = new NetworkCredential (AZURE_KEY, AZURE_KEY);

            try {
                using (HttpWebResponse httpRes = (HttpWebResponse)httpReq.GetResponse ()) {

                    var response = httpRes.GetResponseStream ();
                    var json = (JsonObject)JsonObject.Load (response);
            
                    var results = (from result in (JsonArray)json ["d"] ["results"]
                                let jResult = result as JsonObject 
                                select new SearchItem { Title = jResult["Title"], Url = jResult["Url"], Description = jResult["Description"] }).ToList ();
               
                    if (sync != null)
                        sync (results);
                }
            } catch (Exception) {
                if (sync != null)
                    sync (null);
            }
        }
#if iOS
        public void Speak (string text, string lang)
        {
            HttpWebRequest request;
            HttpWebResponse response = null;

            try {
                string uri = String.Format (
                    "http://api.microsofttranslator.com/v2/Http.svc/Speak?appId={0}&text={1}&language={2}",
                    MS_TRANSLATOR_API_ID,
                    text,
                    lang
                );
                
                request = (HttpWebRequest)WebRequest.Create (uri);
                response = (HttpWebResponse)request.GetResponse ();

                Stream s = response.GetResponseStream ();
                NSData d = NSData.FromStream (s);

                audioPlayer = AVAudioPlayer.FromData (d);
                audioPlayer.PrepareToPlay ();
                audioPlayer.Play ();
                
            } catch (Exception) {
                
                if (response != null)
                    response.Close ();
            }
        }
#endif
    }
}