using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XmlEdit;

namespace Test.XmlEdit
{
    /// <summary>
    /// Tests for the XML Path Predicate expression
    /// </summary>
    [TestClass]
    public class Predicate__test
    {
        [TestMethod]
        public void ElementIndex_lotsOfNumbers()
        {
            for (int i = 0; i < 150; i += 11)
            {
                Predicate p = new Predicate(i.ToString());
                Assert.IsTrue(p.IsElementIndex);
                Assert.AreEqual(i, p.ElementIndex);
                Assert.AreEqual("[" + i + "]", p.PredicateExpression);
            }
        }
        [TestMethod]
        public void ElementIndex_stripSquareBrackets()
        {
            for (int i = 0; i < 150; i += 11)
            {
                Predicate p = new Predicate("["+i.ToString()+"]");
                Assert.IsTrue(p.IsElementIndex);
                Assert.AreEqual(i, p.ElementIndex);
                Assert.AreEqual("[" + i + "]", p.PredicateExpression);
            }
        }
        [TestMethod]
        public void ElementIndex_convertToPositionTest()
        {
            for (int i = 0; i < 150; i += 11)
            {
                Predicate p = new Predicate(i.ToString());
                Assert.AreEqual("[position()='" + i + "']", p.ExpandedExpression);
            }
        }
        [TestMethod]
        public void BasicExpression()
        {
            string x = "somethignSomething='asdf'";
            Predicate p = new Predicate(x);
            Assert.IsFalse(p.IsElementIndex);
            Assert.AreEqual("[" + x + "]", p.PredicateExpression);
            Assert.AreEqual(p.PredicateExpression, p.ExpandedExpression);
        }

        [TestMethod]
        public void StripSquareBrackets()
        {
            string x = "[somethignSomething='asdf']";
            Predicate p = new Predicate(x);
            Assert.IsFalse(p.IsElementIndex);
            Assert.AreEqual(x, p.PredicateExpression);
            Assert.AreEqual(p.PredicateExpression, p.ExpandedExpression);
        }

        [TestMethod]
        public void IsAttributeFilter_true()
        {
            Predicate p = new Predicate("@x='s'");
            Assert.IsTrue(p.IsAttributeFilter);
            Assert.AreEqual("@x", p.FilterKey);
            Assert.AreEqual("s", p.FilterValue);
        }
    }
}
