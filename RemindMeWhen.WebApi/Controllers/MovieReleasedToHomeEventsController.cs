using Knapcode.RemindMeWhen.Core.Models;
using Knapcode.RemindMeWhen.Core.Repositories;

namespace Knapcode.RemindMeWhen.WebApi.Controllers
{
    public class MovieReleasedToHomeEventsController : BaseEventSearchController<MovieReleasedToHomeEvent>
    {
        public MovieReleasedToHomeEventsController(IEventSearchRepository<MovieReleasedToHomeEvent> repository) : base(repository)
        {
        }
    }
}