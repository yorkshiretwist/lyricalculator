using Lyricalculator.Core.Models;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace Lyricalculator.Core
{
    public class LyricsApiQuery : ILyricsApiQuery
    {
        private readonly HttpClient _httpClient;

        public LyricsApiQuery(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<LyricsResponse> FetchLyrics(Artist artist, Song song)
        {
            using (var response = await _httpClient.GetAsync($"{HttpUtility.UrlEncode(artist.Name)}/{HttpUtility.UrlEncode(song.Title)}"))
            {
                if (response.IsSuccessStatusCode)
                {
                    var lyricResponse = await response.Content.ReadAsStringAsync();
                    dynamic lyricJson = JObject.Parse(lyricResponse);

                    return new LyricsResponse
                    {
                        Status = LyricsStatus.Found,
                        Lyrics = lyricJson.lyrics
                    };
                }

                return new LyricsResponse
                {
                    Status = LyricsStatus.NotFound
                };
            }
        }
    }
}
