using FluentAssertions;
using Knapcode.RemindMeWhen.Core.Identities;
using Knapcode.RemindMeWhen.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Knapcode.RemindMeWhen.Core.Tests.Identities
{
    [TestClass]
    public class EventIdentityTests
    {
        [TestMethod]
        public void Constructor_CalledFromJsonDeserializer_Works()
        {
            // ARRANGE
            var expected = new EventIdentity(EventType.MovieReleasedToTheater, "foo", "bar");
            string json = JsonConvert.SerializeObject(expected);
            
            // ACT
            var actual = JsonConvert.DeserializeObject<EventIdentity>(json);

            // ASSERT
            actual.ShouldBeEquivalentTo(expected);
        }
    }
}
