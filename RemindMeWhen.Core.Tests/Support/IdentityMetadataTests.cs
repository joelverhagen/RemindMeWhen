using System;
using System.IO;
using FluentAssertions;
using Knapcode.RemindMeWhen.Core.Identities;
using Knapcode.RemindMeWhen.Core.Support;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Knapcode.RemindMeWhen.Core.Tests.Support
{
    [TestClass]
    public class IdentityMetadataTests
    {
        private const string ArbitraryFieldNameA = "FieldA";
        private const string ArbitraryFieldNameB = "FieldB";
        private const string ArbitraryFieldNameC = "FieldC";
        private const string ArbitraryFieldSeperator = "-";
        private static readonly string[] ArbitraryFieldNames = {"StringField", "EnumField"};
        private static readonly Type[] ArbitraryFieldTypes = {typeof (string), typeof (TestEnum)};
        private static readonly IdentityMetadata ArbitraryIdentityMetadata = new IdentityMetadata(ArbitraryFieldNames, ArbitraryFieldTypes, ArbitraryFieldSeperator);
        private static readonly object[] ArbitraryFieldsA = {"a", TestEnum.SomeValue};
        private static readonly object[] ArbitraryFieldsB = {"b", TestEnum.SomeValue};

        [TestMethod]
        public void Constructor_NullFieldNames_Throws()
        {
            // ACT
            Action a = () => new IdentityMetadata(null, ArbitraryFieldTypes, ArbitraryFieldSeperator);

            // ASSERT
            a.ShouldThrow<ArgumentNullException>().And.ParamName.ShouldBeEquivalentTo("fieldNames");
        }

        [TestMethod]
        public void Constructor_NoFieldNames_Throws()
        {
            // ACT
            Action a = () => new IdentityMetadata(new string[0], ArbitraryFieldTypes, ArbitraryFieldSeperator);

            // ASSERT
            a.ShouldThrow<ArgumentException>().And.ParamName.ShouldBeEquivalentTo("fieldNames");
        }

        [TestMethod]
        public void Constructor_NullFieldTypes_Throws()
        {
            // ACT
            Action a = () => new IdentityMetadata(ArbitraryFieldNames, null, ArbitraryFieldSeperator);

            // ASSERT
            a.ShouldThrow<ArgumentNullException>().And.ParamName.ShouldBeEquivalentTo("fieldTypes");
        }

        [TestMethod]
        public void Constructor_DifferentNumberOfFieldTypesThanNames_Throws()
        {
            // ACT
            Action a = () => new IdentityMetadata(new[] {ArbitraryFieldNameA}, new Type[0], ArbitraryFieldSeperator);

            // ASSERT
            a.ShouldThrow<ArgumentException>().And.ParamName.ShouldBeEquivalentTo("fieldTypes");
        }

        [TestMethod]
        public void Constructor_NullFieldType_Throws()
        {
            // ACT
            Action a = () => new IdentityMetadata(new[] {ArbitraryFieldNameA}, new Type[] {null}, ArbitraryFieldSeperator);

            // ASSERT
            a.ShouldThrow<ArgumentException>().And.ParamName.ShouldBeEquivalentTo("fieldTypes");
        }

        [TestMethod]
        public void Constructor_UnsupportedFieldType_Throws()
        {
            // ACT
            Action a = () => new IdentityMetadata(new[] {ArbitraryFieldNameA}, new[] {typeof (MemoryStream)}, ArbitraryFieldSeperator);

            // ASSERT
            a.ShouldThrow<ArgumentException>().And.ParamName.ShouldBeEquivalentTo("fieldTypes");
        }

        [TestMethod]
        public void Constructor_ValidFieldNames_Succeeds()
        {
            // ACT
            var metadata = new IdentityMetadata(new[] {"foo"}, new[] {typeof (string)}, ArbitraryFieldSeperator);

            // ASSERT
            metadata.FieldNames.Should().HaveCount(1).And.OnlyContain(name => name == "foo");
        }

        [TestMethod]
        public void Constructor_StringFieldType_Succeeds()
        {
            // ACT
            var metadata = new IdentityMetadata(new[] {ArbitraryFieldNameA}, new[] {typeof (string)}, ArbitraryFieldSeperator);

            // ASSERT
            metadata.FieldTypes.Should().HaveCount(1).And.OnlyContain(type => type == typeof (string));
        }

        [TestMethod]
        public void Constructor_EnumFieldType_Succeeds()
        {
            // ACT
            var metadata = new IdentityMetadata(new[] {ArbitraryFieldNameA}, new[] {typeof (TestEnum)}, ArbitraryFieldSeperator);

            // ASSERT
            metadata.FieldTypes.Should().HaveCount(1).And.OnlyContain(type => type == typeof (TestEnum));
        }

        [TestMethod]
        public void Constructor_NullFieldSeperator_Throws()
        {
            // ACT
            Action a = () => new IdentityMetadata(ArbitraryFieldNames, ArbitraryFieldTypes, null);

            // ASSERT
            a.ShouldThrow<ArgumentNullException>().And.ParamName.ShouldBeEquivalentTo("fieldSeperator");
        }

        [TestMethod]
        public void Constructor_EmptyFieldSeperator_Throws()
        {
            // ACT
            Action a = () => new IdentityMetadata(ArbitraryFieldNames, ArbitraryFieldTypes, string.Empty);

            // ASSERT
            a.ShouldThrow<ArgumentException>().And.ParamName.ShouldBeEquivalentTo("fieldSeperator");
        }

        [TestMethod]
        public void Constructor_ValidFieldSeperator_Succeeds()
        {
            // ACT
            var metadata = new IdentityMetadata(ArbitraryFieldNames, ArbitraryFieldTypes, "-");

            // ASSERT
            metadata.FieldSeperator.ShouldBeEquivalentTo(ArbitraryFieldSeperator);
        }

        [TestMethod]
        public void ToString_NullFieldArray_Throws()
        {
            // ACT
            Action a = () => ArbitraryIdentityMetadata.ToString(null);

            // ASSERT
            a.ShouldThrow<ArgumentNullException>().And.ParamName.ShouldBeEquivalentTo("fields");
        }

        [TestMethod]
        public void ToString_WrongNumberOfFields_Throws()
        {
            // ARRANGE
            var metadata = new IdentityMetadata(new[] {ArbitraryFieldNameA}, new[] {typeof (string)}, ArbitraryFieldSeperator);

            // ACT
            Action a = () => metadata.ToString(new object[] {"foo", "bar"});

            // ASSERT
            a.ShouldThrow<ArgumentException>().And.ParamName.ShouldBeEquivalentTo("fields");
        }

        [TestMethod]
        public void ToString_WrongTypeOfFields_Throws()
        {
            // ARRANGE
            var metadata = new IdentityMetadata(new[] {ArbitraryFieldNameA}, new[] {typeof (string)}, ArbitraryFieldSeperator);

            // ACT
            Action a = () => metadata.ToString(new object[] {TestEnum.SomeValue});

            // ASSERT
            a.ShouldThrow<ArgumentException>().And.ParamName.ShouldBeEquivalentTo("fields");
        }

        [TestMethod]
        public void ToString_NullField_Throws()
        {
            // ARRANGE
            var metadata = new IdentityMetadata(new[] {ArbitraryFieldNameA}, new[] {typeof (string)}, ArbitraryFieldSeperator);

            // ACT
            Action a = () => metadata.ToString(new object[] {null});

            // ASSERT
            a.ShouldThrow<ArgumentException>().And.ParamName.ShouldBeEquivalentTo("fields");
        }

        [TestMethod]
        public void ToString_ValidIdentity_Succeeds()
        {
            // ARRANGE
            var metadata = new IdentityMetadata(new[] {ArbitraryFieldNameA, ArbitraryFieldNameB, ArbitraryFieldNameC}, new[] {typeof (string), typeof (TestEnum), typeof (string)}, "-");
            var fields = new object[] {"foo", TestEnum.SomeValue, "bar"};
            const string expected = "foo-SomeValue-bar";

            // ACT
            string actual = metadata.ToString(fields);

            // ASSERT
            actual.ShouldBeEquivalentTo(expected);
        }

        [TestMethod]
        public void TryParse_NullIdentity_Throws()
        {
            // ARRANGE
            object[] fields;

            // ACT
            bool success = ArbitraryIdentityMetadata.TryParse(null, out fields);

            // ASSERT
            success.Should().BeFalse();
            fields.Should().BeNull();
        }

        [TestMethod]
        public void TryParse_TooFewFields_ReturnsFalse()
        {
            // ARRANGE
            var metadata = new IdentityMetadata(new[] {ArbitraryFieldNameA, ArbitraryFieldNameB}, new[] {typeof (string), typeof (string)}, "-");
            object[] fields;

            // ACT
            bool success = metadata.TryParse("invalid", out fields);

            // ASSERT
            success.Should().BeFalse();
            fields.Should().BeNull();
        }

        [TestMethod]
        public void TryParse_InvalidField_ReturnsFalse()
        {
            // ARRANGE
            var metadata = new IdentityMetadata(new[] {ArbitraryFieldNameA}, new[] {typeof (TestEnum)}, ArbitraryFieldSeperator);
            object[] fields;

            // ACT
            bool success = metadata.TryParse("invalid", out fields);

            // ASSERT
            success.Should().BeFalse();
            fields.Should().BeNull();
        }

        [TestMethod]
        public void TryParse_EmptyField_ReturnsTrue()
        {
            // ARRANGE
            var metadata = new IdentityMetadata(new[] {ArbitraryFieldNameA, ArbitraryFieldNameB, ArbitraryFieldNameC}, new[] {typeof (string), typeof (string), typeof (string)}, "-");
            const string input = "foo--bar";
            object[] fields;

            // ACT
            bool success = metadata.TryParse(input, out fields);

            // ASSERT
            success.Should().BeTrue();
            fields.ShouldAllBeEquivalentTo(new object[] {"foo", string.Empty, "bar"});
        }

        [TestMethod]
        public void TryParse_ExtraFields_ReturnsTrue()
        {
            // ARRANGE
            var metadata = new IdentityMetadata(new[] {ArbitraryFieldNameA, ArbitraryFieldNameB}, new[] {typeof (string), typeof (string)}, "-");
            const string input = "foo-bar-baz";
            object[] fields;

            // ACT
            bool success = metadata.TryParse(input, out fields);

            // ASSERT
            success.Should().BeTrue();
            fields.ShouldAllBeEquivalentTo(new object[] {"foo", "bar-baz"});
        }

        [TestMethod]
        public void TryParse_ValidFields_ReturnsTrue()
        {
            // ARRANGE
            var metadata = new IdentityMetadata(new[] {ArbitraryFieldNameA, ArbitraryFieldNameB, ArbitraryFieldNameC}, new[] {typeof (string), typeof (TestEnum), typeof (string)}, "-");
            const string input = "foo-SomeValue-bar";
            object[] fields;

            // ACT
            bool success = metadata.TryParse(input, out fields);

            // ASSERT
            success.Should().BeTrue();
            fields.ShouldAllBeEquivalentTo(new object[] {"foo", TestEnum.SomeValue, "bar"});
        }

        [TestMethod]
        public void TryParse_EnumWithWrongCase_ReturnsFalse()
        {
            // ARRANGE
            var metadata = new IdentityMetadata(new[] {ArbitraryFieldNameA}, new[] {typeof (TestEnum)}, ArbitraryFieldSeperator);
            const string input = "someValue";
            object[] fields;

            // ACT
            bool success = metadata.TryParse(input, out fields);

            // ASSERT
            success.Should().BeFalse();
            fields.Should().BeNull();
        }

        [TestMethod]
        public void CompareTo_NullFirstArgument_Throws()
        {
            // ACT
            Action a = () => ArbitraryIdentityMetadata.CompareTo(null, ArbitraryFieldsA);

            // ASSERT
            a.ShouldThrow<ArgumentNullException>().And.ParamName.ShouldBeEquivalentTo("fieldsA");
        }

        [TestMethod]
        public void CompareTo_NullSecondArgument_Throws()
        {
            // ACT
            Action a = () => ArbitraryIdentityMetadata.CompareTo(ArbitraryFieldsA, null);

            // ASSERT
            a.ShouldThrow<ArgumentNullException>().And.ParamName.ShouldBeEquivalentTo("fieldsB");
        }

        [TestMethod]
        public void CompareTo_LessThan_ReturnsNegative()
        {
            // ACT
            int output = ArbitraryIdentityMetadata.CompareTo(ArbitraryFieldsA, ArbitraryFieldsB);

            // ASSERT
            output.Should().BeLessThan(0);
        }

        [TestMethod]
        public void CompareTo_GreaterThan_ReturnsPositive()
        {
            // ACT
            int output = ArbitraryIdentityMetadata.CompareTo(ArbitraryFieldsB, ArbitraryFieldsA);

            // ASSERT
            output.Should().BeGreaterThan(0);
        }

        [TestMethod]
        public void CompareTo_Equal_ReturnsZero()
        {
            // ACT
            int output = ArbitraryIdentityMetadata.CompareTo(ArbitraryFieldsA, ArbitraryFieldsA);

            // ASSERT
            output.Should().Be(0);
        }

        [TestMethod]
        public void Equals_NullFirstArgument_Throws()
        {
            // ACT
            Action a = () => ArbitraryIdentityMetadata.Equals(null, ArbitraryFieldsA);

            // ASSERT
            a.ShouldThrow<ArgumentNullException>().And.ParamName.ShouldBeEquivalentTo("fieldsA");
        }

        [TestMethod]
        public void Equals_NullSecondArgument_Throws()
        {
            // ACT
            Action a = () => ArbitraryIdentityMetadata.Equals(ArbitraryFieldsA, null);

            // ASSERT
            a.ShouldThrow<ArgumentNullException>().And.ParamName.ShouldBeEquivalentTo("fieldsB");
        }

        [TestMethod]
        public void Equals_Equal_ReturnsTrue()
        {
            // ACT
            bool equal = ArbitraryIdentityMetadata.Equals(ArbitraryFieldsA, ArbitraryFieldsA);

            // ASSERT
            equal.Should().BeTrue();
        }

        [TestMethod]
        public void Equals_NotEqual_ReturnsFalse()
        {
            // ACT
            bool equal = ArbitraryIdentityMetadata.Equals(ArbitraryFieldsA, ArbitraryFieldsB);

            // ASSERT
            equal.Should().BeFalse();
        }

        [TestMethod]
        public void GetHashCode_NullArgument_Throws()
        {
            // ACT
            Action a = () => ArbitraryIdentityMetadata.GetHashCode(null);

            // ASSERT
            a.ShouldThrow<ArgumentNullException>().And.ParamName.ShouldBeEquivalentTo("fields");
        }

        [TestMethod]
        public void GetHashCode_DifferentInput_ReturnsDifferentHashCode()
        {
            // ACT
            int hashCodeA = ArbitraryIdentityMetadata.GetHashCode(ArbitraryFieldsA);
            int hashCodeB = ArbitraryIdentityMetadata.GetHashCode(ArbitraryFieldsB);

            // ASSERT
            hashCodeA.Should().NotBe(hashCodeB);
        }

        [TestMethod]
        public void ValidateFieldsAreParsable_NullFields_Throws()
        {
            // ACT
            Action a = () => ArbitraryIdentityMetadata.ValidateFieldsAreParsable(null);

            // ASSERT
            a.ShouldThrow<ArgumentNullException>().And.ParamName.ShouldBeEquivalentTo("fields");
        }

        [TestMethod]
        public void ValidateFieldsAreParsable_ValidFields_Succeeds()
        {
            // ACT
            Action a = () => ArbitraryIdentityMetadata.ValidateFieldsAreParsable(ArbitraryFieldsA);

            // ASSERT
            a.ShouldNotThrow();
        }

        [TestMethod]
        public void ValidateFieldsAreParsable_UnparsableInput_Throws()
        {
            // ARRANGE
            var metadata = new IdentityMetadata(new[] {ArbitraryFieldNameA, ArbitraryFieldNameB}, new[] {typeof (TestEnum), typeof (TestEnum)}, TestEnum.SomeValue.ToString());
            var fields = new object[] {TestEnum.SomeValue, TestEnum.SomeValue};

            // ACT
            Action a = () => metadata.ValidateFieldsAreParsable(fields);

            // ASSERT
            a.ShouldThrow<ArgumentException>().And.ParamName.ShouldBeEquivalentTo("fields");
        }

        [TestMethod]
        public void ValidateFieldsAreParsable_AmbiguousInput_Throws()
        {
            // ARRANGE
            var metadata = new IdentityMetadata(new[] {ArbitraryFieldNameA, ArbitraryFieldNameB}, new[] {typeof (string), typeof (string)}, "-");
            var fields = new object[] {"foo-bar", "baz"};

            // ACT
            Action a = () => metadata.ValidateFieldsAreParsable(fields);

            // ASSERT
            a.ShouldThrow<ArgumentException>().And.ParamName.ShouldBeEquivalentTo("fields");
        }

        private enum TestEnum
        {
            SomeValue
        };
    }
}