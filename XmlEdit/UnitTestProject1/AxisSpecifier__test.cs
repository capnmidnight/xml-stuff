using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XmlEdit;

/*
 * These tests require the NUnit Framework to be installed on the machine.
 * They need to be built into a DLL project, and have the classes that they
 * test visible to them in some way (either in the same project or as a
 * referenced assembly. Once the NUnit Framework is installed and the test
 * suite is built, the NUnit Framework provides a Test Harness application
 * from which one may execute the tests.
 */

namespace Test.XmlEdit
{
    /// <summary>
    /// There are only a fixed set of Axis Specifiers. These tests ensure they come out right
    /// </summary>
    [TestClass]
    public class AxisSpecifier__test
    {
        [TestMethod]
        public void TestChild()
        {
            Assert.AreEqual(PrincipalNodeTypes.Element, AxisSpecifier.Child.NodeType);
            Assert.AreEqual("child::", AxisSpecifier.Child.ExtendedSyntax);
            Assert.AreEqual("", AxisSpecifier.Child.SimpleSyntax);
        }
        [TestMethod]
        public void TestAttribute()
        {
            Assert.AreEqual(PrincipalNodeTypes.Attribute, AxisSpecifier.Attribute.NodeType);
            Assert.AreEqual("attribute::", AxisSpecifier.Attribute.ExtendedSyntax);
            Assert.AreEqual("@", AxisSpecifier.Attribute.SimpleSyntax);
        }
        [TestMethod]
        public void TestDescendant()
        {
            Assert.AreEqual(PrincipalNodeTypes.Element, AxisSpecifier.Descendant.NodeType);
            Assert.AreEqual("descendant::", AxisSpecifier.Descendant.ExtendedSyntax);
            Assert.AreEqual("/", AxisSpecifier.Descendant.SimpleSyntax);
        }
        [TestMethod]
        public void TestDescendantOrSelf()
        {
            Assert.AreEqual(PrincipalNodeTypes.Element, AxisSpecifier.DescendantOrSelf.NodeType);
            Assert.AreEqual("descendant-or-self::", AxisSpecifier.DescendantOrSelf.ExtendedSyntax);
            Assert.IsNull(AxisSpecifier.DescendantOrSelf.SimpleSyntax);
        }
        [TestMethod]
        public void TestParent()
        {
            Assert.AreEqual(PrincipalNodeTypes.Element, AxisSpecifier.Parent.NodeType);
            Assert.AreEqual("parent::", AxisSpecifier.Parent.ExtendedSyntax);
            Assert.AreEqual("..", AxisSpecifier.Parent.SimpleSyntax);
        }
        [TestMethod]
        public void TestAncestor()
        {
            Assert.AreEqual(PrincipalNodeTypes.Element, AxisSpecifier.Ancestor.NodeType);
            Assert.AreEqual("ancestor::", AxisSpecifier.Ancestor.ExtendedSyntax);
            Assert.IsNull(AxisSpecifier.Ancestor.SimpleSyntax);
        }
        [TestMethod]
        public void TestAncestorOrSelf()
        {
            Assert.AreEqual(PrincipalNodeTypes.Element, AxisSpecifier.AncestorOrSelf.NodeType);
            Assert.AreEqual("ancestor-or-self::", AxisSpecifier.AncestorOrSelf.ExtendedSyntax);
            Assert.IsNull(AxisSpecifier.AncestorOrSelf.SimpleSyntax);
        }
        [TestMethod]
        public void TestFollowing()
        {
            Assert.AreEqual(PrincipalNodeTypes.Element, AxisSpecifier.Following.NodeType);
            Assert.AreEqual("following::", AxisSpecifier.Following.ExtendedSyntax);
            Assert.IsNull(AxisSpecifier.Following.SimpleSyntax);
        }
        [TestMethod]
        public void TestPreceding()
        {
            Assert.AreEqual(PrincipalNodeTypes.Element, AxisSpecifier.Preceding.NodeType);
            Assert.AreEqual("preceding::", AxisSpecifier.Preceding.ExtendedSyntax);
            Assert.IsNull(AxisSpecifier.Preceding.SimpleSyntax);
        }
        [TestMethod]
        public void TestFollowingSibling()
        {
            Assert.AreEqual(PrincipalNodeTypes.Element, AxisSpecifier.FollowingSibling.NodeType);
            Assert.AreEqual("following-sibling::", AxisSpecifier.FollowingSibling.ExtendedSyntax);
            Assert.IsNull(AxisSpecifier.FollowingSibling.SimpleSyntax);
        }
        [TestMethod]
        public void TestPrecedingSibling()
        {
            Assert.AreEqual(PrincipalNodeTypes.Element, AxisSpecifier.PrecedingSibling.NodeType);
            Assert.AreEqual("preceding-sibling::", AxisSpecifier.PrecedingSibling.ExtendedSyntax);
            Assert.IsNull(AxisSpecifier.PrecedingSibling.SimpleSyntax);
        }
        [TestMethod]
        public void TestSelf()
        {
            Assert.AreEqual(PrincipalNodeTypes.Element, AxisSpecifier.Self.NodeType);
            Assert.AreEqual("self::", AxisSpecifier.Self.ExtendedSyntax);
            Assert.AreEqual(".", AxisSpecifier.Self.SimpleSyntax);
        }
        [TestMethod]
        public void TestNamespace()
        {
            Assert.AreEqual(PrincipalNodeTypes.Namespace, AxisSpecifier.Namespace.NodeType);
            Assert.AreEqual("namespace::", AxisSpecifier.Namespace.ExtendedSyntax);
            Assert.IsNull(AxisSpecifier.Namespace.SimpleSyntax);
        }
    }
}
