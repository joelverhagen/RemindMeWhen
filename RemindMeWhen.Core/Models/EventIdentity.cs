using System;
using System.Collections.Generic;
using System.Linq;

namespace Knapcode.RemindMeWhen.Core.Models
{
    public struct EventIdentity : IEquatable<EventIdentity>, IComparable<EventIdentity>
    {
        private static readonly IDictionary<string, EventType> EventTypeDictionary;

        private readonly string _source;
        private readonly string _sourceIdentity;
        private readonly EventType _type;

        static EventIdentity()
        {
            EventTypeDictionary = new Dictionary<string, EventType>(StringComparer.InvariantCultureIgnoreCase);

            foreach (EventType eventType in Enum.GetValues(typeof (EventType)).Cast<EventType>())
            {
                EventType existingEventType;
                if (EventTypeDictionary.TryGetValue(eventType.ToString(), out existingEventType))
                {
                    string message = string.Format("Event type '{0}' and '{1}' collide when using case-insensitive comparison.", eventType, existingEventType);
                    throw new InvalidOperationException(message);
                }

                EventTypeDictionary[eventType.ToString()] = eventType;
            }
        }

        public EventIdentity(string eventIdentity)
        {
            if (eventIdentity == null)
            {
                throw new ArgumentNullException("eventIdentity");
            }

            string[] pieces = eventIdentity.Split(new[] {'-'}, 3);
            if (pieces.Length != 3)
            {
                throw new ArgumentException("The event identity string must have at least two hyphens.");
            }

            EventType type;
            if (!EventTypeDictionary.TryGetValue(pieces[0], out type))
            {
                string message = string.Format("The first piece of event identity string '{0}' did not match a valid event type.", pieces[0]);
                throw new ArgumentException(message);
            }

            _type = type;
            _source = pieces[1];
            _sourceIdentity = pieces[2];
        }

        public EventIdentity(EventType type, string source, string sourceIdentity)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (source.Contains("-"))
            {
                string message = string.Format("The source '{0}' cannot contain a hyphen.", source);
                throw new ArgumentException(message, "source");
            }

            if (sourceIdentity == null)
            {
                throw new ArgumentNullException("sourceIdentity");
            }

            _type = type;
            _source = source;
            _sourceIdentity = sourceIdentity;
        }

        public EventType Type
        {
            get { return _type; }
        }

        public string Source
        {
            get { return _source; }
        }

        public string SourceIdentity
        {
            get { return _sourceIdentity; }
        }

        public int CompareTo(EventIdentity other)
        {
            return String.CompareOrdinal(ToString(), other.ToString());
        }

        public bool Equals(EventIdentity other)
        {
            return ToString().Equals(other.ToString());
        }

        public override string ToString()
        {
            return string.Join("-", new[] {Type.ToString(), Source, SourceIdentity});
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is EventIdentity && Equals((EventIdentity) obj);
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
    }
}