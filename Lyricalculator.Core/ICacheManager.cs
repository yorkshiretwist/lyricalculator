using Lyricalculator.Core.Models;
using System;
using System.Collections.Generic;

namespace Lyricalculator.Core
{
    public interface ICacheManager
    {
        Artist GetArtist(Guid artistId);

        void StoreArtist(Artist artist);

        IEnumerable<Album> GetArtistAlbums(Guid artistId);

        void StoreArtistAlbums(Guid artistId, IEnumerable<Album> albums);

        Song GetSong(Guid songId);

        void StoreSong(Song song);
    }
}