using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Knapcode.RemindMeWhen.Core.Models;
using Knapcode.RemindMeWhen.Core.Repositories;
using Knapcode.RemindMeWhen.Website.Models;

namespace Knapcode.RemindMeWhen.Website.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRottenTomatoesRepository<MovieReleasedToTheaterEvent> _movieReleasedToTheaterEventRepository;
        private readonly IRottenTomatoesRepository<MovieReleasedToHomeEvent> _movieReleasedToHomeEventRepository;

        public HomeController(IRottenTomatoesRepository<MovieReleasedToTheaterEvent> movieReleasedToTheaterEventRepository, IRottenTomatoesRepository<MovieReleasedToHomeEvent> movieReleasedToHomeEventRepository)
        {
            _movieReleasedToTheaterEventRepository = movieReleasedToTheaterEventRepository;
            _movieReleasedToHomeEventRepository = movieReleasedToHomeEventRepository;
        }

        public async Task<ActionResult> Index(string eventType = null, string query = null)
        {
            var viewModel = new SearchViewModel
            {
                Parameters = new SearchParametersViewModel
                {
                    EventType = eventType,
                    Query = query,
                    EventTypes = Enum.GetValues(typeof (EventType)).Cast<EventType>()
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
                    events = (await _movieReleasedToTheaterEventRepository.SearchMovieReleaseEventsAsync(query, new PageOffset(0, 25))).Entries;
                    break;
                case EventType.MovieReleasedToHome:
                    events = (await _movieReleasedToHomeEventRepository.SearchMovieReleaseEventsAsync(query, new PageOffset(0, 25))).Entries;
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