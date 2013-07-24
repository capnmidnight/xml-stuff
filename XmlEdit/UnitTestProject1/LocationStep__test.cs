using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XmlEdit;

namespace Test.XmlEdit
{
    [TestClass]
    public class LocationStep__test
    {
        [TestMethod]
        public void CreateLocationStep_1()
        {
            LocationStep step = new LocationStep("child::a:b[@c=d]");
            Assert.AreEqual(1, step.Predicates.Count);
            Assert.AreEqual("[@c='d']", step.Predicates[0].ExpandedExpression);
            Assert.AreEqual("a:b", step.NodeTest.ToString());
            Assert.AreEqual(AxisSpecifier.Child, step.AxisSpecifier);
            Assert.AreEqual(PrincipalNodeTypes.Element, step.AxisSpecifier.NodeType);
            Assert.IsFalse(step.IsAttribute);
            Assert.AreEqual("a:b", step.BaseXPath);
            Assert.AreEqual("a:b[@c='d']", step.ToString(false));
        }
        [TestMethod]
        public void CreateLocationStep_2()
        {
            LocationStep step = new LocationStep("root");
            Assert.AreEqual(0, step.Predicates.Count);
            Assert.AreEqual("root", step.NodeTest.ToString());
            Assert.AreEqual(PrincipalNodeTypes.Element, step.AxisSpecifier.NodeType);
            Assert.AreEqual(AxisSpecifier.Child, step.AxisSpecifier);
            Assert.IsFalse(step.IsAttribute);
            Assert.AreEqual("root", step.BaseXPath);
            Assert.AreEqual("child::root", step.ToString(true));
        }

        [TestMethod]
        public void CreateLocationStep_3()
        {
            LocationStep step = new LocationStep("@okay");
            Assert.AreEqual(0, step.Predicates.Count);
            Assert.AreEqual("okay", step.NodeTest.ToString());
            Assert.AreEqual(PrincipalNodeTypes.Attribute, step.AxisSpecifier.NodeType);
            Assert.AreEqual(AxisSpecifier.Attribute, step.AxisSpecifier);
            Assert.IsTrue(step.IsAttribute);
        }

        [TestMethod]
        public void StepForAttributeDoesntIncludeNamespace()
        {
            LocationStep step = new LocationStep("@attr");
            step.NodeTest.Namespace = "ns0";
            Assert.AreEqual("@attr", step.ToString(false));
        }

        [TestMethod]
        public void StepForElementIncludesNamespace()
        {
            LocationStep step = new LocationStep("Element");
            step.NodeTest.Namespace = "ns0";
            Assert.AreEqual("ns0:Element", step.ToString(false));
        }

        [TestMethod]
        public void CreateLocationStep_4()
        {
            LocationStep step = new LocationStep("A[@b='c']");
            Assert.AreEqual(1, step.Predicates.Count);
            Assert.AreEqual("A", step.NodeTest.ToString());
            Assert.AreEqual(PrincipalNodeTypes.Element, step.AxisSpecifier.NodeType);
            Assert.IsFalse(step.IsAttribute);
        }

        [TestMethod]
        public void Clone()
        {
            LocationStep step1 = new LocationStep("a");
            LocationStep step2 = (LocationStep)step1.Clone();
            Assert.AreNotSame(step1, step2);
            Assert.AreEqual(step1.ToString(true), step2.ToString(true));
        }
    }
}
