using System;
using FluentAssertions;
using Xunit;

namespace SwissDwellings.Tests
{
    public class WktValidationTests
    {
        [Theory]
        [InlineData("POINT (10 20)", true)]
        [InlineData("LINESTRING (0 0, 1 1)", true)]
        [InlineData("POLYGON ((0 0, 1 0, 1 1, 0 1, 0 0))", true)]
        [InlineData("MULTIPOINT ((10 10), (20 20))", true)]
        [InlineData("MULTILINESTRING ((0 0, 1 1), (2 2, 3 3))", true)]
        [InlineData("MULTIPOLYGON (((0 0, 1 0, 1 1, 0 1, 0 0)))", true)]
        [InlineData("GEOMETRYCOLLECTION (POINT(4 6), LINESTRING(4 6,7 10))", true)]
        [InlineData("point (10 20)", true)] // Case insensitive
        [InlineData("  POINT (10 20)  ", true)] // Whitespace
        [InlineData("SRID=4326;POINT (10 20)", true)] // EWKT
        [InlineData("SRID=4326; POINT (10 20)", true)] // EWKT with space
        [InlineData("SRID=4326;POINT(10 20)", true)] // EWKT compact
        [InlineData("", false)]
        [InlineData("   ", false)]
        [InlineData("INVALID", false)]
        [InlineData("NOT A GEOMETRY", false)]
        [InlineData("SRID=4326", false)] // Missing geometry
        [InlineData("SRID=;POINT(10 20)", true)] // Empty SRID might be technically valid structure for parser to try? Or let's assume valid.
                                                // Actually MsdLoader.IsPotentialWkt will just check structure.
        [InlineData("UNKNOWN (10 20)", false)]
        public void IsPotentialWkt_ValidatesCorrectly(string wkt, bool expected)
        {
            // Act
            bool result = MsdLoader.IsPotentialWkt(wkt.AsSpan());

            // Assert
            result.Should().Be(expected);
        }
    }
}
