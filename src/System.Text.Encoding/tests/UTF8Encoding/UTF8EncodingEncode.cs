// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Text.Tests
{
    public class UTF8EncodingEncode
    {
        public static IEnumerable<object[]> Encode_TestData()
        {
            // All ASCII chars
            for (char c = char.MinValue; c <= 0x7F; c++)
            {
                yield return new object[] { c.ToString(), 0, 1, new byte[] { (byte)c } };
                yield return new object[] { "a" + c.ToString() + "b", 1, 1, new byte[] { (byte)c } };
                yield return new object[] { "a" + c.ToString() + "b", 2, 1, new byte[] { 98 } };
                yield return new object[] { "a" + c.ToString() + "b", 0, 3, new byte[] { 97, (byte)c, 98 } };
            }

            // Misc ASCII and Unicode strings
            yield return new object[] { "FooBA\u0400R", 0, 7, new byte[] { 70, 111, 111, 66, 65, 208, 128, 82 } };
            yield return new object[] { "\u00C0nima\u0300l", 0, 7, new byte[] { 195, 128, 110, 105, 109, 97, 204, 128, 108 } };
            yield return new object[] { "Test\uD803\uDD75Test", 0, 10, new byte[] { 84, 101, 115, 116, 240, 144, 181, 181, 84, 101, 115, 116 } };
            yield return new object[] { "\u0130", 0, 1, new byte[] { 196, 176 } };

            yield return new object[] { "\uD803\uDD75\uD803\uDD75\uD803\uDD75", 0, 6, new byte[] { 240, 144, 181, 181, 240, 144, 181, 181, 240, 144, 181, 181 } };
            yield return new object[] { "za\u0306\u01FD\u03B2\uD8FF\uDCFF", 0, 7, new byte[] { 122, 97, 204, 134, 199, 189, 206, 178, 241, 143, 179, 191 } };
            yield return new object[] { "za\u0306\u01FD\u03B2\uD8FF\uDCFF", 4, 3, new byte[] { 206, 178, 241, 143, 179, 191 } };

            yield return new object[] { "\u0023\u0025\u03a0\u03a3", 1, 2, new byte[] { 37, 206, 160 } };
            yield return new object[] { "\u00C5", 0, 1, new byte[] { 0xC3, 0x85 } };

            yield return new object[] { "\u0065\u0065\u00E1\u0065\u0065\u8000\u00E1\u0065\uD800\uDC00\u8000\u00E1\u0065\u0065\u0065", 0, 15, new byte[] { 0x65, 0x65, 0xC3, 0xA1, 0x65, 0x65, 0xE8, 0x80, 0x80, 0xC3, 0xA1, 0x65, 0xF0, 0x90, 0x80, 0x80, 0xE8, 0x80, 0x80, 0xC3, 0xA1, 0x65, 0x65, 0x65 } };

            yield return new object[] { "\u00A4\u00D0aR|{AnGe\u00A3\u00A4", 0, 12, new byte[] { 0xC2, 0xA4, 0xC3, 0x90, 0x61, 0x52, 0x7C, 0x7B, 0x41, 0x6E, 0x47, 0x65, 0xC2, 0xA3, 0xC2, 0xA4 } };

            // Control codes
            yield return new object[] { "\u001F\u0010\u0000\u0009", 0, 4, new byte[] { 0x1F, 0x10, 0x00, 0x09 } };

            // Long ASCII strings
            yield return new object[] { "eeeee", 0, 5, new byte[] { 0x65, 0x65, 0x65, 0x65, 0x65 } };
            yield return new object[] { "e\u00E1eee", 0, 5, new byte[] { 0x65, 0xC3, 0xA1, 0x65, 0x65, 0x65 } };
            yield return new object[] { "\u0065\u8000\u0065\u0065\u0065", 0, 5, new byte[] { 0x65, 0xE8, 0x80, 0x80, 0x65, 0x65, 0x65 } };
            yield return new object[] { "\u0065\uD800\uDC00\u0065\u0065\u0065", 0, 6, new byte[] { 0x65, 0xF0, 0x90, 0x80, 0x80, 0x65, 0x65, 0x65 } };

            yield return new object[] { "eeeeeeeeeeeeeee", 0, 15, new byte[] { 0x65, 0x65, 0x65, 0x65, 0x65, 0x65, 0x65, 0x65, 0x65, 0x65, 0x65, 0x65, 0x65, 0x65, 0x65 } };
            yield return new object[] { "eeeeee\u00E1eeeeeeee", 0, 15, new byte[] { 0x65, 0x65, 0x65, 0x65, 0x65, 0x65, 0xC3, 0xA1, 0x65, 0x65, 0x65, 0x65, 0x65, 0x65, 0x65, 0x65 } };

            yield return new object[] { "\u0065\u0065\u0065\u0065\u0065\u0065\u8000\u0065\u0065\u0065\u0065\u0065\u0065\u0065\u0065", 0, 15, new byte[] { 0x65, 0x65, 0x65, 0x65, 0x65, 0x65, 0xE8, 0x80, 0x80, 0x65, 0x65, 0x65, 0x65, 0x65, 0x65, 0x65, 0x65 } };
            yield return new object[] { "\u0065\u0065\u0065\u0065\u0065\u0065\uD800\uDC00\u0065\u0065\u0065\u0065\u0065\u0065\u0065\u0065", 0, 16, new byte[] { 0x65, 0x65, 0x65, 0x65, 0x65, 0x65, 0xF0, 0x90, 0x80, 0x80, 0x65, 0x65, 0x65, 0x65, 0x65, 0x65, 0x65, 0x65 } };

            // 2 bytes
            yield return new object[] { "\u00E1", 0, 1, new byte[] { 0xC3, 0xA1 } };
            yield return new object[] { "\u00E1\u00E1\u00E1\u00E1\u00E1", 0, 5, new byte[] { 0xC3, 0xA1, 0xC3, 0xA1, 0xC3, 0xA1, 0xC3, 0xA1, 0xC3, 0xA1 } };
            yield return new object[] { "\u00E1e\u00E1\u00E1\u00E1", 0, 5, new byte[] { 0xC3, 0xA1, 0x65, 0xC3, 0xA1, 0xC3, 0xA1, 0xC3, 0xA1 } };
            yield return new object[] { "\u00E1\u8000\u00E1\u00E1\u00E1", 0, 5,  new byte[] { 0xC3, 0xA1, 0xE8, 0x80, 0x80, 0xC3, 0xA1, 0xC3, 0xA1, 0xC3, 0xA1 } };
            yield return new object[] { "\u00E1\uD800\uDC00\u00E1\u00E1\u00E1", 0, 6, new byte[] { 0xC3, 0xA1, 0xF0, 0x90, 0x80, 0x80, 0xC3, 0xA1, 0xC3, 0xA1, 0xC3, 0xA1 } };

            yield return new object[] { "\u00E1\u00E1\u00E1\u00E1\u00E1\u00E1\u00E1\u00E1\u00E1\u00E1\u00E1\u00E1\u00E1\u00E1\u00E1", 0, 15, new byte[] { 0xC3, 0xA1, 0xC3, 0xA1, 0xC3, 0xA1, 0xC3, 0xA1, 0xC3, 0xA1, 0xC3, 0xA1, 0xC3, 0xA1, 0xC3, 0xA1, 0xC3, 0xA1, 0xC3, 0xA1, 0xC3, 0xA1, 0xC3, 0xA1, 0xC3, 0xA1, 0xC3, 0xA1, 0xC3, 0xA1 } };
            yield return new object[] { "\u00E1\u00E1e\u00E1\u00E1\u00E1\u00E1\u00E1\u00E1\u00E1\u00E1\u00E1\u00E1\u00E1\u00E1", 0, 15, new byte[] { 0xC3, 0xA1, 0xC3, 0xA1, 0x65, 0xC3, 0xA1, 0xC3, 0xA1, 0xC3, 0xA1, 0xC3, 0xA1, 0xC3, 0xA1, 0xC3, 0xA1, 0xC3, 0xA1, 0xC3, 0xA1, 0xC3, 0xA1, 0xC3, 0xA1, 0xC3, 0xA1, 0xC3, 0xA1 } };
            yield return new object[] { "\u00E1\u00E1\u8000\u00E1\u00E1\u00E1\u00E1\u00E1\u00E1\u00E1\u00E1\u00E1\u00E1\u00E1\u00E1", 0, 15, new byte[] { 0xC3, 0xA1, 0xC3, 0xA1, 0xE8, 0x80, 0x80, 0xC3, 0xA1, 0xC3, 0xA1, 0xC3, 0xA1, 0xC3, 0xA1, 0xC3, 0xA1, 0xC3, 0xA1, 0xC3, 0xA1, 0xC3, 0xA1, 0xC3, 0xA1, 0xC3, 0xA1, 0xC3, 0xA1, 0xC3, 0xA1 } };
            yield return new object[] { "\u00E1\u00E1\uD800\uDC00\u00E1\u00E1\u00E1\u00E1\u00E1\u00E1\u00E1\u00E1\u00E1\u00E1\u00E1\u00E1", 0, 16, new byte[] { 0xC3, 0xA1, 0xC3, 0xA1, 0xF0, 0x90, 0x80, 0x80, 0xC3, 0xA1, 0xC3, 0xA1, 0xC3, 0xA1, 0xC3, 0xA1, 0xC3, 0xA1, 0xC3, 0xA1, 0xC3, 0xA1, 0xC3, 0xA1, 0xC3, 0xA1, 0xC3, 0xA1, 0xC3, 0xA1, 0xC3, 0xA1 } };

            // 3 bytes
            yield return new object[] { "\u8000", 0, 1, new byte[] { 0xE8, 0x80, 0x80 } };
            yield return new object[] { "\u8000\u8000\u8000\u8000", 0, 4, new byte[] { 0xE8, 0x80, 0x80, 0xE8, 0x80, 0x80, 0xE8, 0x80, 0x80, 0xE8, 0x80, 0x80 } };
            yield return new object[] { "\u8000\u0065\u8000\u8000", 0, 4, new byte[] { 0xE8, 0x80, 0x80, 0x65, 0xE8, 0x80, 0x80, 0xE8, 0x80, 0x80 } };
            yield return new object[] { "\u8000\u00E1\u8000\u8000", 0, 4, new byte[] { 0xE8, 0x80, 0x80, 0xC3, 0xA1, 0xE8, 0x80, 0x80, 0xE8, 0x80, 0x80 } };
            yield return new object[] { "\u8000\uD800\uDC00\u8000\u8000", 0, 5, new byte[] { 0xE8, 0x80, 0x80, 0xF0, 0x90, 0x80, 0x80, 0xE8, 0x80, 0x80, 0xE8, 0x80, 0x80 } };

            yield return new object[] { "\u8000\u8000\u8000\u8000\u8000\u8000\u8000\u8000\u8000\u8000\u8000\u8000\u8000\u8000\u8000", 0, 15, new byte[] { 0xE8, 0x80, 0x80, 0xE8, 0x80, 0x80, 0xE8, 0x80, 0x80, 0xE8, 0x80, 0x80, 0xE8, 0x80, 0x80, 0xE8, 0x80, 0x80, 0xE8, 0x80, 0x80, 0xE8, 0x80, 0x80, 0xE8, 0x80, 0x80, 0xE8, 0x80, 0x80, 0xE8, 0x80, 0x80, 0xE8, 0x80, 0x80, 0xE8, 0x80, 0x80, 0xE8, 0x80, 0x80, 0xE8, 0x80, 0x80 } };
            yield return new object[] { "\u8000\u8000\u8000\u0065\u8000\u8000\u8000\u8000\u8000\u8000\u8000\u8000\u8000\u8000\u8000", 0, 15, new byte[] { 0xE8, 0x80, 0x80, 0xE8, 0x80, 0x80, 0xE8, 0x80, 0x80, 0x65, 0xE8, 0x80, 0x80, 0xE8, 0x80, 0x80, 0xE8, 0x80, 0x80, 0xE8, 0x80, 0x80, 0xE8, 0x80, 0x80, 0xE8, 0x80, 0x80, 0xE8, 0x80, 0x80, 0xE8, 0x80, 0x80, 0xE8, 0x80, 0x80, 0xE8, 0x80, 0x80, 0xE8, 0x80, 0x80 } };
            yield return new object[] { "\u8000\u8000\u8000\u00E1\u8000\u8000\u8000\u8000\u8000\u8000\u8000\u8000\u8000\u8000\u8000", 0, 15, new byte[] { 0xE8, 0x80, 0x80, 0xE8, 0x80, 0x80, 0xE8, 0x80, 0x80, 0xC3, 0xA1, 0xE8, 0x80, 0x80, 0xE8, 0x80, 0x80, 0xE8, 0x80, 0x80, 0xE8, 0x80, 0x80, 0xE8, 0x80, 0x80, 0xE8, 0x80, 0x80, 0xE8, 0x80, 0x80, 0xE8, 0x80, 0x80, 0xE8, 0x80, 0x80, 0xE8, 0x80, 0x80, 0xE8, 0x80, 0x80 } };
            yield return new object[] { "\u8000\u8000\u8000\uD800\uDC00\u8000\u8000\u8000\u8000\u8000\u8000\u8000\u8000\u8000\u8000\u8000", 0, 16, new byte[] { 0xE8, 0x80, 0x80, 0xE8, 0x80, 0x80, 0xE8, 0x80, 0x80, 0xF0, 0x90, 0x80, 0x80, 0xE8, 0x80, 0x80, 0xE8, 0x80, 0x80, 0xE8, 0x80, 0x80, 0xE8, 0x80, 0x80, 0xE8, 0x80, 0x80, 0xE8, 0x80, 0x80, 0xE8, 0x80, 0x80, 0xE8, 0x80, 0x80, 0xE8, 0x80, 0x80, 0xE8, 0x80, 0x80, 0xE8, 0x80, 0x80 } };

            // Surrogate pairs
            yield return new object[] { "\uD800\uDC00", 0, 2, new byte[] { 240, 144, 128, 128 } };
            yield return new object[] { "a\uD800\uDC00b", 0, 4, new byte[] { 97, 240, 144, 128, 128, 98 } };

            yield return new object[] { "\uDB80\uDC00", 0, 2, new byte[] { 0xF3, 0xB0, 0x80, 0x80 } };
            yield return new object[] { "\uD800\uDFFF", 0, 2, new byte[] { 0xF0, 0x90, 0x8F, 0xBF } };
            yield return new object[] { "\uDBFF\uDC00", 0, 2, new byte[] { 0xF4, 0x8F, 0xB0, 0x80 } };
            yield return new object[] { "\uDBFF\uDFFF", 0, 2, new byte[] { 0xF4, 0x8F, 0xBF, 0xBF } };

            yield return new object[] { "\uD800\uDC00\uD800\uDC00\uD800\uDC00", 0, 6, new byte[] { 0xF0, 0x90, 0x80, 0x80, 0xF0, 0x90, 0x80, 0x80, 0xF0, 0x90, 0x80, 0x80 } };
            yield return new object[] { "\uD800\uDC00\u0065\uD800\uDC00", 0, 5, new byte[] { 0xF0, 0x90, 0x80, 0x80, 0x65, 0xF0, 0x90, 0x80, 0x80 } };

            yield return new object[] { "\uD800\uDC00\u00E1\uD800\uDC00", 0, 5, new byte[] { 0xF0, 0x90, 0x80, 0x80, 0xC3, 0xA1, 0xF0, 0x90, 0x80, 0x80 } };
            yield return new object[] { "\uD800\uDC00\u8000\uD800\uDC00", 0, 5, new byte[] { 0xF0, 0x90, 0x80, 0x80, 0xE8, 0x80, 0x80, 0xF0, 0x90, 0x80, 0x80 } };

            yield return new object[] { "\uD800\uDC00\uD800\uDC00\uD800\uDC00\uD800\uDC00\uD800\uDC00\uD800\uDC00\uD800\uDC00\uD800\uDC00", 0, 16, new byte[] { 0xF0, 0x90, 0x80, 0x80, 0xF0, 0x90, 0x80, 0x80, 0xF0, 0x90, 0x80, 0x80, 0xF0, 0x90, 0x80, 0x80, 0xF0, 0x90, 0x80, 0x80, 0xF0, 0x90, 0x80, 0x80, 0xF0, 0x90, 0x80, 0x80, 0xF0, 0x90, 0x80, 0x80 } };
            yield return new object[] { "\uD800\uDC00\u0065\uD800\uDC00\uD800\uDC00\uD800\uDC00\uD800\uDC00\uD800\uDC00\uD800\uDC00", 0, 15, new byte[] { 0xF0, 0x90, 0x80, 0x80, 0x65, 0xF0, 0x90, 0x80, 0x80, 0xF0, 0x90, 0x80, 0x80, 0xF0, 0x90, 0x80, 0x80, 0xF0, 0x90, 0x80, 0x80, 0xF0, 0x90, 0x80, 0x80, 0xF0, 0x90, 0x80, 0x80 } };
            yield return new object[] { "\uD800\uDC00\u00E1\uD800\uDC00\uD800\uDC00\uD800\uDC00\uD800\uDC00\uD800\uDC00\uD800\uDC00", 0, 15, new byte[] { 0xF0, 0x90, 0x80, 0x80, 0xC3, 0xA1, 0xF0, 0x90, 0x80, 0x80, 0xF0, 0x90, 0x80, 0x80, 0xF0, 0x90, 0x80, 0x80, 0xF0, 0x90, 0x80, 0x80, 0xF0, 0x90, 0x80, 0x80, 0xF0, 0x90, 0x80, 0x80 } };
            yield return new object[] { "\uD800\uDC00\u8000\uD800\uDC00\uD800\uDC00\uD800\uDC00\uD800\uDC00\uD800\uDC00\uD800\uDC00", 0, 15, new byte[] { 0xF0, 0x90, 0x80, 0x80, 0xE8, 0x80, 0x80, 0xF0, 0x90, 0x80, 0x80, 0xF0, 0x90, 0x80, 0x80, 0xF0, 0x90, 0x80, 0x80, 0xF0, 0x90, 0x80, 0x80, 0xF0, 0x90, 0x80, 0x80, 0xF0, 0x90, 0x80, 0x80 } };

            // Empty strings
            yield return new object[] { string.Empty, 0, 0, new byte[0] };
            yield return new object[] { "abc", 3, 0, new byte[0] };
            yield return new object[] { "abc", 0, 0, new byte[0] };
        }

        [Theory]
        [MemberData(nameof(Encode_TestData))]
        public void Encode(string chars, int index, int count, byte[] expected)
        {
            EncodingHelpers.Encode(new UTF8Encoding(true, false), chars, index, count, expected);
            EncodingHelpers.Encode(new UTF8Encoding(false, false), chars, index, count, expected);

            EncodingHelpers.Encode(new UTF8Encoding(false, true), chars, index, count, expected);
            EncodingHelpers.Encode(new UTF8Encoding(true, true), chars, index, count, expected);
        }

        public void Encode_InvalidChars(string chars, int index, int count, byte[] expected)
        {
            EncodingHelpers.Encode(new UTF8Encoding(true, false), chars, index, count, expected);
            EncodingHelpers.Encode(new UTF8Encoding(false, false), chars, index, count, expected);

            NegativeEncodingTests.Encode_Invalid(new UTF8Encoding(false, true), chars, index, count);
            NegativeEncodingTests.Encode_Invalid(new UTF8Encoding(true, true), chars, index, count);
        }

        [Fact]
        public void Encode_InvalidChars()
        {
            // TODO: add into Encode_TestData or Encode_InvalidChars_TestData once #7166 is fixed
            byte[] unicodeReplacementBytes1 = new byte[] { 239, 191, 189 };

            // Lone high surrogate
            Encode_InvalidChars("\uD800", 0, 1, unicodeReplacementBytes1);
            Encode_InvalidChars("\uDD75", 0, 1, unicodeReplacementBytes1);
            Encode_InvalidChars("\uDBFF", 0, 1, unicodeReplacementBytes1);

            // Lone low surrogate
            Encode_InvalidChars("\uDC00", 0, 1, unicodeReplacementBytes1);
            Encode_InvalidChars("\uDC00", 0, 1, unicodeReplacementBytes1);

            // Surrogate pair out of range
            Encode_InvalidChars("\uD800\uDC00", 0, 1, unicodeReplacementBytes1);
            Encode_InvalidChars("\uD800\uDC00", 1, 1, unicodeReplacementBytes1);

            // Invalid surrogate pair
            Encode_InvalidChars("\u0041\uD800\uE000", 0, 3, new byte[] { 0x41, 0xEF, 0xBF, 0xBD, 0xEE, 0x80, 0x80 });
            Encode_InvalidChars("\uD800\u0041\uDC00", 0, 3, new byte[] { 0xEF, 0xBF, 0xBD, 0x41, 0xEF, 0xBF, 0xBD });
            Encode_InvalidChars("\uD800\u0041\u0042\u07FF\u0043\uDC00", 0, 6, new byte[] { 0xEF, 0xBF, 0xBD, 0x41, 0x42, 0xDF, 0xBF, 0x43, 0xEF, 0xBF, 0xBD });

            // Mixture of ASCII, valid Unicode and invalid unicode
            Encode_InvalidChars("\uDD75\uDD75\uD803\uDD75\uDD75\uDD75\uDD75\uD803\uD803\uD803\uDD75\uDD75\uDD75\uDD75", 0, 14, new byte[] { 239, 191, 189, 239, 191, 189, 240, 144, 181, 181, 239, 191, 189, 239, 191, 189, 239, 191, 189, 239, 191, 189, 239, 191, 189, 240, 144, 181, 181, 239, 191, 189, 239, 191, 189, 239, 191, 189 });
            Encode_InvalidChars("Test\uD803Test", 0, 9, new byte[] { 84, 101, 115, 116, 239, 191, 189, 84, 101, 115, 116 });
            Encode_InvalidChars("Test\uDD75Test", 0, 9, new byte[] { 84, 101, 115, 116, 239, 191, 189, 84, 101, 115, 116 });
            Encode_InvalidChars("TestTest\uDD75", 0, 9, new byte[] { 84, 101, 115, 116, 84, 101, 115, 116, 239, 191, 189 });
            Encode_InvalidChars("TestTest\uD803", 0, 9, new byte[] { 84, 101, 115, 116, 84, 101, 115, 116, 239, 191, 189 });
            
            byte[] unicodeReplacementBytes2 = new byte[] { 239, 191, 189, 239, 191, 189 };
            Encode_InvalidChars("\uD800\uD800", 0, 2, unicodeReplacementBytes2); // High, high
            Encode_InvalidChars("\uDC00\uD800", 0, 2, unicodeReplacementBytes2); // Low, high
            Encode_InvalidChars("\uDC00\uDC00", 0, 2, unicodeReplacementBytes2); // Low, low

            // U+FDD0 - U+FDEF
            Encode("\uFDD0\uFDEF", 0, 2, new byte[] { 0xEF, 0xB7, 0x90, 0xEF, 0xB7, 0xAF });

            // BOM
            Encode("\uFEFF\u0041", 0, 2, new byte[] { 0xEF, 0xBB, 0xBF, 0x41 });

            // High BMP non-chars
            Encode("\uFFFD", 0, 1, unicodeReplacementBytes1);
            Encode("\uFFFE", 0, 1, new byte[] { 239, 191, 190 });
            Encode("\uFFFF", 0, 1, new byte[] { 239, 191, 191 });
        }
    }
}
