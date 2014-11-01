﻿using System;
using System.Collections.Generic;

namespace Knapcode.RemindMeWhen.Core.Models
{
    public abstract class MovieReleasedEvent : IEvent
    {
        public EventIdentity Identity { get; set; }
        public DateTime? DateTime { get; set; }

        public string Title { get; set; }
        public int? ReleasedYear { get; set; }
        public Uri ImdbUrl { get; set; }
        public Uri RottenTomatoesUrl { get; set; }
        public Uri ImageUrl { get; set; }
        public IEnumerable<string> Actors { get; set; }
    }
}
