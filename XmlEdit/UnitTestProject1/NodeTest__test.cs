using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XmlEdit;

namespace Test.XmlEdit
{
    [TestClass]
    public class NodeTest__test
    {
        [TestMethod]
        public void Create_Node()
        {
            NodeTest test = NodeTest.Create("node()");
            Assert.AreEqual("node()", test.ToString());
            Assert.AreEqual(NodeTestNodeType.Node, test);
        }
        [TestMethod]
        public void Create_Comment()
        {
            NodeTest test = NodeTest.Create("comment()");
            Assert.AreEqual("comment()", test.ToString());
            Assert.AreEqual(NodeTestNodeType.Comment, test);
        }
        [TestMethod]
        public void Create_Text()
        {
            NodeTest test = NodeTest.Create("text()");
            Assert.AreEqual("text()", test.ToString());
            Assert.AreEqual(NodeTestNodeType.Text, test);
        }

        [TestMethod]
        public void Create_ProcessingInstruction_Php()
        {
            NodeTest test = NodeTest.Create("processing-instruction('php')");
            Assert.AreEqual(typeof(NodeTestProcessingInstruction), test.GetType());
            Assert.AreEqual("processing-instruction('php')", test.ToString());
        }


        [TestMethod]
        public void Create_ProcessingInstruction_NoParam()
        {
            NodeTest test = NodeTest.Create("processing-instruction()");
            Assert.AreEqual(typeof(NodeTestProcessingInstruction), test.GetType());
            Assert.AreEqual("processing-instruction()", test.ToString());
        }
        [TestMethod]
        public void Create_Wildcard_NoNamespace()
        {
            NodeTest test = NodeTest.Create("*");
            Assert.AreEqual(typeof(NodeTestWildcard), test.GetType());
        }

        [TestMethod]
        public void Create_Wildcard_Ns0()
        {
            NodeTest test = NodeTest.Create("ns0:*");
            Assert.AreEqual(typeof(NodeTestWildcard), test.GetType());
            Assert.AreEqual("ns0:*", test.ToString());
        }

        [TestMethod]
        public void Create_NamedNode_NoNamespace()
        {
            NodeTest test = NodeTest.Create("ICSMXML");
            Assert.AreEqual(typeof(NodeTestNamedNode), test.GetType());
            Assert.AreEqual("ICSMXML", test.ToString());
        }

        [TestMethod]
        public void Create_NamedNode_Ns0()
        {
            NodeTest test = NodeTest.Create("ns0:ICSMXML");
            Assert.AreEqual(typeof(NodeTestNamedNode), test.GetType());
            Assert.AreEqual("ns0:ICSMXML", test.ToString());
        }
    }
}
