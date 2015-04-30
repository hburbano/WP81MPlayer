using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading;

namespace MP3Player
{
    public class Track
    {
        [JsonProperty("id")]
        public int UniqueId { get; set; }

        [JsonProperty("album")]
        public String Album { get; set; }

        [JsonProperty("artist")]
        public String Artist { get; set; }

        [JsonProperty("filename")]
        public String Filename { get; set; }

        [JsonProperty("title")]
        public String Title { get; set; }

        [JsonProperty("mp3Uri")]
        public String Mp3Uri { get; set; }

        [JsonProperty("imageUri")]
        public String ImageUri { get; set; }

        public static async Task<Track> GetTrack(int id, CancellationTokenSource cts)
        {
            String queryString = "https://glaring-inferno-8255.firebaseio.com/tracks/"+id+".json";
            Uri idQuery = new Uri(@queryString);
            Track o = new Track();

            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage response = await client.GetAsync(idQuery))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        string content = await response.Content.ReadAsStringAsync();
                        //o = await JsonConvert.DeserializeObjectAsync<Track>(content);

                        o = await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<Track>(content));
                    }
                }
            }
            return o;
        }

        public static async Task<List<Track>> GetAllTracks()
        {
            String queryString = "https://glaring-inferno-8255.firebaseio.com/tracks.json";
            Uri idQuery = new Uri(@queryString);
            List<Track> o = new List<Track>();

            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage response = await client.GetAsync(idQuery))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        string content = await response.Content.ReadAsStringAsync();
                        //o = await JsonConvert.DeserializeObjectAsync<Track>(content);

                        o = await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<List<Track>>(content));
                    }
                }
            }
            return o;
        }
    }
}
