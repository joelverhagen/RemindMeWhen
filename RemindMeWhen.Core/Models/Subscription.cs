﻿using System;
using Knapcode.RemindMeWhen.Core.Persistence;

namespace Knapcode.RemindMeWhen.Core.Models
{
    public class Subscription
    {
        public SubscriptionId Id { get; set; }
        public DateTime Created { get; set; }
    }
}