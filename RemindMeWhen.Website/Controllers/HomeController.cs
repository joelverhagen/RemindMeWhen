using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Knapcode.RemindMeWhen.Core.Models;
using Knapcode.RemindMeWhen.Core.Repositories;
using RemindMeWhen.Website.Models;

namespace RemindMeWhen.Website.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEventSearchRepository<MovieReleasedToHomeEvent> _releasedToHomeRepository;
        private readonly IEventSearchRepository<MovieReleasedToTheaterEvent> _releasedToTheaterRepository;

        public HomeController(IEventSearchRepository<MovieReleasedToTheaterEvent> releasedToTheaterRepository, IEventSearchRepository<MovieReleasedToHomeEvent> releasedToHomeRepository)
        {
            _releasedToTheaterRepository = releasedToTheaterRepository;
            _releasedToHomeRepository = releasedToHomeRepository;
        }

        public async Task<ActionResult> Index(string eventType = null, string query = null)
        {
            var viewModel = new SearchViewModel
            {
                Parameters = new SearchParametersViewModel
                {
                    EventType = eventType,
                    Query = query,
                    EventTypes = Enum.GetValues(typeof (EventType)).Cast<EventType>().Where(t => t != EventType.None)
                }
            };

            if (eventType != null && query != null)
            {
                viewModel.Results = await GetSearchResults(eventType, query);
            }

            return View(viewModel);
        }

        private async Task<SearchResultsViewModel> GetSearchResults(string eventType, string query)
        {
            EventType type;
            Enum.TryParse(eventType, out type);

            IEnumerable<MovieReleasedEvent> events;
            switch (type)
            {
                case EventType.MovieReleasedToTheater:
                    events = (await _releasedToTheaterRepository.EventSearchAsync(query, 50, 1)).Entries;
                    break;
                case EventType.MovieReleasedToHome:
                    events = (await _releasedToHomeRepository.EventSearchAsync(query, 50, 1)).Entries;
                    break;
                default:
                    events = Enumerable.Empty<MovieReleasedEvent>();
                    break;
            }

            return new SearchResultsViewModel
            {
                Events = events
            };
        }
    }
}