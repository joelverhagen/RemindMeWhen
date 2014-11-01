﻿using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Knapcode.RemindMeWhen.Core.Models;
using Knapcode.RemindMeWhen.Core.Repositories;
using Knapcode.RemindMeWhen.WebApi.Models;

namespace Knapcode.RemindMeWhen.WebApi.Controllers
{
    public abstract class BaseEventSearchController<T> : ApiController where T : IEvent
    {
        private readonly IEventSearchRepository<T> _repository;

        protected BaseEventSearchController(IEventSearchRepository<T> repository)
        {
            _repository = repository;
        }

        public async Task<PageViewModel<T>> GetEvents(string query, int pageLimit = 10, int pageNumber = 1)
        {
            Page<T> page = await _repository.EventSearchAsync(query, pageLimit, pageNumber);

            Uri self = GetUrl(query, pageLimit, pageNumber);
            Uri next = page.HasNextPage ? GetUrl(query, pageLimit, pageNumber + 1) : null;

            return new PageViewModel<T>
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

        private Uri GetUrl(string query, int pageLimit, int pageNumber)
        {
            var builder = new UriBuilder(Request.RequestUri);

            var queryString = HttpUtility.ParseQueryString(string.Empty);
            queryString["query"] = query;
            queryString["pageLimit"] = pageLimit.ToString(CultureInfo.InvariantCulture);
            queryString["pageNumber"] = pageNumber.ToString(CultureInfo.InvariantCulture);
            builder.Query = queryString.ToString();

            return builder.Uri;
        }
    }
}