using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Routing;
using Knapcode.RemindMeWhen.Core.Models;
using Knapcode.RemindMeWhen.Core.Repositories;
using Knapcode.RemindMeWhen.WebApi.Models;

namespace Knapcode.RemindMeWhen.WebApi.Controllers
{
    public class MovieReleasesController : ApiController
    {
        private readonly IMovieReleaseRepository _repository;

        public MovieReleasesController(IMovieReleaseRepository repository)
        {
            _repository = repository;
        }

        public async Task<PageViewModel<MovieRelease>> Get(string id, int pageLimit = 10, int pageNumber = 1)
        {
            Page<MovieRelease> page = await _repository.GetMovieReleasesPage(id, pageLimit, pageNumber);

            string self = Url.Link("DefaultApi", new {controller = "MovieReleases", id, pageLimit, pageNumber});
            string next = page.HasNextPage ? Url.Link("DefaultApi", new { controller = "MovieReleases", id, pageLimit, pageNumber = pageNumber + 1 }) : null;

            return new PageViewModel<MovieRelease>
            {
                Count = page.Entries.Count(),
                Entries = page.Entries,
                Links = new PageLinks
                {
                    Self = self,
                    Next = next
                }
            };
        }
    }
}
