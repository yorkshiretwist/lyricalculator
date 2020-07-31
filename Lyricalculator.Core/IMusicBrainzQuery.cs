using MetaBrainz.MusicBrainz;
using MetaBrainz.MusicBrainz.Interfaces.Browses;
using MetaBrainz.MusicBrainz.Interfaces.Entities;
using MetaBrainz.MusicBrainz.Interfaces.Searches;
using System;
using System.Threading.Tasks;

namespace Lyricalculator.Core
{
    public interface IMusicBrainzQuery
    {
        Task<ISearchResults<ISearchResult<IArtist>>> FindArtistsAsync(string query, int? limit = null, int? offset = null, bool simple = false);

        Task<IBrowseResults<IReleaseGroup>> BrowseReleaseGroupsAsync(IArtist artist, int? limit = null, int? offset = null, Include inc = Include.None, ReleaseType? type = null);

        Task<IReleaseGroup> LookupReleaseGroupAsync(Guid mbid, Include inc = Include.None, ReleaseStatus? status = null);

        Task<IRelease> LookupReleaseAsync(Guid mbid, Include inc = Include.None);

        Task<IArtist> LookupArtistAsync(Guid mbid, Include inc = Include.None, ReleaseType? type = null, ReleaseStatus? status = null);
    }
}
