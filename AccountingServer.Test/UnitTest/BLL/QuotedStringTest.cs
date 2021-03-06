using System;
using AccountingServer.BLL.Util;
using Xunit;

namespace AccountingServer.Test.UnitTest.BLL
{
    public class QuotedStringTest
    {
        [Theory]
        [InlineData(null, '\'')]
        [InlineData("", '\'')]
        [InlineData("simple", '\'')]
        [InlineData("'s'i'm'ple'''", '\'')]
        [InlineData("\"\"'s\\'i'm\"'\"ple'\"''\"", '\'')]
        [InlineData("simple", '"')]
        [InlineData("'s'i'm'ple'''", '"')]
        [InlineData("\"\"'s\\'i'm\"'\"ple'\"''\"", '"')]
        public void QuotationTest(string text, char ch) { Assert.Equal(text ?? "", text.Quotation(ch).Dequotation()); }

        [Fact]
        public void DequotationTest()
        {
            Assert.Null(QuotedStringHelper.Dequotation(null));
            Assert.Equal("", "".Dequotation());
            Assert.Throws<ArgumentException>(() => "\'".Dequotation());
            Assert.Throws<ArgumentException>(() => "\'aaerv\"".Dequotation());
        }
    }
}
