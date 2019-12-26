using LIN.MSA.Infrastructure;
using NUnit.Framework;
using System;

namespace LIN.MSA.NUnit
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }

        [Test]
        public void hello()
        {
            for(int i = 0; i < 1000; i++)
            {
                Console.WriteLine(Snowflake.GetId());
            }
            
        }
    }
}