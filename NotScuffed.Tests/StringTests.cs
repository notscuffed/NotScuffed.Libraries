using System;
using NotScuffed.Linq;
using NUnit.Framework;
using NotScuffed.Strings;

namespace NotScuffed.Tests
{
    public class StringTests
    {
        [Test]
        public void TestValidIsASCIICharacters()
        {
            foreach (var c in "qQxXaAzZ:?!@#$%^&*()_+=-[]\\/.,<>;':\"")
            {
                if (!c.IsASCII())
                    throw new Exception($"Character '{c}' is a valid ASCII character but IsASCII returned false");
            }
        }

        [Test]
        public void TestInvalidIsASCIICharacters()
        {
            foreach (var c in "\0\a\b\t\r\nąóęłć€©☺☻♥")
            {
                if (c.IsASCII())
                    throw new Exception($"Character '{c}' is an invalid ASCII character but IsASCII returned true");
            }
        }

        [Test]
        [TestCase("abcDefGhiJkl", ExpectedResult = "abc_Def_Ghi_Jkl")]
        [TestCase("AbcDefGhiJkL", ExpectedResult = "Abc_Def_Ghi_Jk_L")]
        [TestCase("ABC", ExpectedResult = "A_B_C")]
        [TestCase("ĄĘĆ", ExpectedResult = "Ą_Ę_Ć")]
        [TestCase("$$$", ExpectedResult = "$$$")]
        public string TestSplitCamelCase(string input)
        {
            return input.SplitCamelCase().JoinString("_");
        }
    }
}