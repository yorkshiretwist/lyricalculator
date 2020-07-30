using Lyricalculator.Core.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Lyricalculator.Core
{
    /// <summary>
    /// A simple file-based cache of artists, songs and lyrics
    /// </summary>
    public class CacheManager : ICacheManager
    {
        private readonly string _directoryPath;

        public CacheManager(string directoryPath)
        {
            _directoryPath = directoryPath;
            if (!Directory.Exists(_directoryPath))
            {
                Directory.CreateDirectory(_directoryPath);
            }
        }

        public Artist GetArtist(Guid artistId)
        {
            var filepath = Path.Combine(_directoryPath, $"artist.{artistId}.json");
            if (!File.Exists(filepath))
            {
                return null;
            }
            return JsonConvert.DeserializeObject<Artist>(File.ReadAllText(filepath));
        }

        public IEnumerable<Album> GetArtistAlbums(Guid artistId)
        {
            var filepath = Path.Combine(_directoryPath, $"albums.{artistId}.json");
            if (!File.Exists(filepath))
            {
                return null;
            }
            return JsonConvert.DeserializeObject<IEnumerable<Album>>(File.ReadAllText(filepath));
        }

        public void StoreArtistAlbums(Guid artistId, IEnumerable<Album> albums)
        {
            var filepath = Path.Combine(_directoryPath, $"albums.{artistId}.json");
            File.WriteAllText(filepath, JsonConvert.SerializeObject(albums));
        }

        public void StoreArtist(Artist artist)
        {
            var filepath = Path.Combine(_directoryPath, $"artist.{artist.Id}.json");
            File.WriteAllText(filepath, JsonConvert.SerializeObject(artist));
        }

        public Song GetSong(Guid songId)
        {
            var filepath = Path.Combine(_directoryPath, $"song.{songId}.json");
            if (!File.Exists(filepath))
            {
                return null;
            }
            return JsonConvert.DeserializeObject<Song>(File.ReadAllText(filepath));
        }

        public void StoreSong(Song song)
        {
            var filepath = Path.Combine(_directoryPath, $"song.{song.Id}.json");
            File.WriteAllText(filepath, JsonConvert.SerializeObject(song));
        }
    }
}
