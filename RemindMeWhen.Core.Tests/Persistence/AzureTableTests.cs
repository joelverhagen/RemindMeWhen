using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Knapcode.RemindMeWhen.Core.Logging;
using Knapcode.RemindMeWhen.Core.Persistence;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Moq;

namespace Knapcode.RemindMeWhen.Core.Tests.Persistence
{
    [TestClass]
    public class AzureTableTests
    {
        [TestMethod, TestCategory("Integration")]
        public async Task SetAsync_Record_Succeeds()
        {
            // ARRANGE
            var ts = await TestState.GetAsync();

            // ACT
            Func<Task> action = () => ts.StringAzureTable.SetAsync(ts.PartitionKey, ts.RowKey, ts.StringValue);

            // ASSERT
            action.ShouldNotThrow();
        }

        [TestMethod, TestCategory("Integration")]
        public async Task SetAsync_Record_EmitsEvent()
        {
            // ARRANGE
            var ts = await TestState.GetAsync();

            // ACT
            await ts.StringAzureTable.SetAsync(ts.PartitionKey, ts.RowKey, ts.StringValue);

            // ASSERT
            ts.EventSourceMock.Verify(e => e.OnSavedRecordToAzure(ts.PartitionKey, ts.RowKey, It.IsAny<TimeSpan>()), Times.Once);
        }

        [TestMethod, TestCategory("Integration")]
        public async Task GetAsync_ExistentRecord_Succeeds()
        {
            // ARRANGE
            var ts = await TestState.GetAsync();
            await ts.StringAzureTable.SetAsync(ts.PartitionKey, ts.RowKey, ts.StringValue);

            // ACT
            string actualValue = await ts.StringAzureTable.GetAsync(ts.PartitionKey, ts.RowKey);

            // ASSERT
            actualValue.ShouldBeEquivalentTo(ts.StringValue);
        }

        [TestMethod, TestCategory("Integration")]
        public async Task GetAsync_ExistentRecord_EmitsEvent()
        {
            // ARRANGE
            var ts = await TestState.GetAsync();
            await ts.StringAzureTable.SetAsync(ts.PartitionKey, ts.RowKey, ts.StringValue);

            // ACT
            await ts.StringAzureTable.GetAsync(ts.PartitionKey, ts.RowKey);

            // ASSERT
            ts.EventSourceMock.Verify(e => e.OnFetchedRecordFromAzure(ts.PartitionKey, ts.RowKey, It.IsAny<TimeSpan>()), Times.Once);
        }

        [TestMethod, TestCategory("Integration")]
        public async Task GetAsync_NonexistentRecord_ReturnsNull()
        {
            // ARRANGE
            var ts = await TestState.GetAsync();

            // ACT
            string actualValue = await ts.StringAzureTable.GetAsync(ts.PartitionKey, ts.RowKey);

            // ASSERT
            actualValue.Should().BeNull();
        }

        [TestMethod, TestCategory("Integration")]
        public async Task GetAsync_NonexistentRecord_EmitsEvents()
        {
            // ARRANGE
            var ts = await TestState.GetAsync();

            // ACT
            await ts.StringAzureTable.GetAsync(ts.PartitionKey, ts.RowKey);

            // ASSERT
            ts.EventSourceMock.Verify(e => e.OnFetchedRecordFromAzure(ts.PartitionKey, ts.RowKey, It.IsAny<TimeSpan>()), Times.Once);
            ts.EventSourceMock.Verify(e => e.OnMissingRecordFromAzure(ts.PartitionKey, ts.RowKey), Times.Once);
        }

        [TestMethod, TestCategory("Integration")]
        public async Task GetAsync_ComplexRecord_FullRecord()
        {
            // ARRANGE
            var ts = await TestState.GetAsync();
            await ts.ComplexRecordAzureTable.SetAsync(ts.PartitionKey, ts.RowKey, ts.ComplexRecordValue);

            // ACT
            ComplexRecord actualValue = await ts.ComplexRecordAzureTable.GetAsync(ts.PartitionKey, ts.RowKey);

            // ASSERT
            actualValue.ShouldBeEquivalentTo(ts.ComplexRecordValue);
        }

        [TestMethod, TestCategory("Integration")]
        public async Task ListAsync_NonexistentPartitionKey_ReturnsZeroRecords()
        {
            // ARRANGE
            var ts = await TestState.GetAsync();
            
            // ACT
            IEnumerable<string> list = await ts.StringAzureTable.ListAsync(ts.PartitionKey, null, null);

            // ASSERT
            list.Should().HaveCount(0);
        }

        [TestMethod, TestCategory("Integration")]
        public async Task ListAsync_NonexistentPartitionKey_EmitsEvent()
        {
            // ARRANGE
            var ts = await TestState.GetAsync();

            // ACT
            await ts.StringAzureTable.ListAsync(ts.PartitionKey, null, null);

            // ASSERT
            ts.EventSourceMock.Verify(e => e.OnFetchedListFromAzure(ts.PartitionKey, 0, It.IsAny<TimeSpan>()), Times.Once);
        }

        [TestMethod, TestCategory("Integration")]
        public async Task ListAsync_ExistentPartitionKey_EmitsEvent()
        {
            // ARRANGE
            var ts = await TestState.GetAsync();
            await ts.StringAzureTable.SetAsync(ts.PartitionKey, ts.RowKey, ts.StringValue);

            // ACT
            await ts.StringAzureTable.ListAsync(ts.PartitionKey, null, null);

            // ASSERT
            ts.EventSourceMock.Verify(e => e.OnFetchedListFromAzure(ts.PartitionKey, 1, It.IsAny<TimeSpan>()), Times.Once);
        }

        [TestMethod, TestCategory("Integration")]
        public async Task ListAsync_MultipleRecords_ReturnsAllRecordsInAscendingOrder()
        {
            // ARRANGE
            var ts = await TestState.GetAsync();
            await ts.GenerateSequence();
           
            // ACT
            IEnumerable<string> list = await ts.StringAzureTable.ListAsync(ts.PartitionKey, null, null);

            // ASSERT
            list.Should().BeEquivalentTo(ts.StringValueSequence).And.BeInAscendingOrder();
        }

        [TestMethod, TestCategory("Integration")]
        public async Task ListAsync_LowerBoundSpecified_ReturnsProperRange()
        {
            // ARRANGE
            var ts = await TestState.GetAsync();
            await ts.GenerateSequence();

            // ACT
            IEnumerable<string> list = await ts.StringAzureTable.ListAsync(ts.PartitionKey, "01", null);

            // ASSERT
            list.Should().BeEquivalentTo(ts.StringValueSequence.Skip(1));
        }

        [TestMethod, TestCategory("Integration")]
        public async Task ListAsync_UpperBoundSpecified_ReturnsProperRange()
        {
            // ARRANGE
            var ts = await TestState.GetAsync();
            await ts.GenerateSequence();

            // ACT
            IEnumerable<string> list = await ts.StringAzureTable.ListAsync(ts.PartitionKey, null, "03");

            // ASSERT
            list.Should().BeEquivalentTo(ts.StringValueSequence.Take(4));
        }

        [TestMethod, TestCategory("Integration")]
        public async Task ListAsync_LowerAndUpperBoundSpecified_ReturnsProperRange()
        {
            // ARRANGE
            var ts = await TestState.GetAsync();
            await ts.GenerateSequence();

            // ACT
            IEnumerable<string> list = await ts.StringAzureTable.ListAsync(ts.PartitionKey, "01", "03");

            // ASSERT
            list.Should().BeEquivalentTo(ts.StringValueSequence.Skip(1).Take(3));
        }

        private class TestState
        {
            private TestState()
            {
            }

            public CloudTable CloudTable { get; private set; }

            public Mock<IEventSource> EventSourceMock { get; private set; }

            public AzureTable<string> StringAzureTable { get; private set; }

            public AzureTable<ComplexRecord> ComplexRecordAzureTable { get; private set; }

            public string PartitionKey { get; private set; }

            public string RowKey { get; private set; }

            public string StringValue { get; private set; }

            public ComplexRecord ComplexRecordValue { get; private set; }

            public IEnumerable<string> StringValueSequence { get; set; }

            public static async Task<TestState> GetAsync()
            {
                // get the cloud table
                CloudStorageAccount account = CloudStorageAccount.DevelopmentStorageAccount;
                var client = account.CreateCloudTableClient();
                var cloudTable = client.GetTableReference("testazuretabletests");
                await cloudTable.CreateIfNotExistsAsync();

                // get the mock
                var eventSourceMock = new Mock<IEventSource>();

                return new TestState
                {
                    CloudTable = cloudTable,
                    EventSourceMock = eventSourceMock,
                    StringAzureTable = new AzureTable<string>(eventSourceMock.Object, cloudTable),
                    ComplexRecordAzureTable = new AzureTable<ComplexRecord>(eventSourceMock.Object, cloudTable),
                    PartitionKey = Guid.NewGuid().ToString(),
                    RowKey = Guid.NewGuid().ToString(),
                    StringValue = Guid.NewGuid().ToString(),
                    ComplexRecordValue = new ComplexRecord
                    {
                        EnumProperty = ComplexRecord.TestEnum.ValueB,
                        ObjectProperty = new ComplexRecord
                        {
                            ObjectProperty = null,
                            EnumProperty = ComplexRecord.TestEnum.ValueC,
                            StringProperty = Guid.NewGuid().ToString()
                        },
                        StringProperty = Guid.NewGuid().ToString()
                    },
                    StringValueSequence = Enumerable
                        .Range(0, 5)
                        .Select(i => i.ToString("D2"))
                        .ToArray()
                };
            }

            public async Task GenerateSequence()
            {
                foreach (var value in StringValueSequence)
                {
                    await StringAzureTable.SetAsync(PartitionKey, value, value);
                }
            }
        }

        private class ComplexRecord
        {
            public enum TestEnum
            {
                ValueA,
                ValueB,
                ValueC,
            }

            public string StringProperty { get; set; }
            public TestEnum EnumProperty { get; set; }
            public ComplexRecord ObjectProperty { get; set; }
        }
    }
}
