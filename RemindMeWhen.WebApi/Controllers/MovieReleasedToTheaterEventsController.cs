using Knapcode.RemindMeWhen.Core.Models;
using Knapcode.RemindMeWhen.Core.Repositories;

namespace Knapcode.RemindMeWhen.WebApi.Controllers
{
    public class MovieReleasedToTheaterEventsController : BaseEventSearchController<MovieReleasedToTheaterEvent>
    {
        public MovieReleasedToTheaterEventsController(IEventSearchRepository<MovieReleasedToTheaterEvent> repository) : base(repository)
        {
        }
    }
}