/*
 * 
 * AUTHOR: Sean T. McBeth, Sean T. McBeth
 * DATE: OCT-02-2007
 * 
 */

namespace XmlEdit
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    /// <summary>
    /// Creates a node test for a specified node type. There are only three node types, Node, Text, and Comment.
    /// </summary>
    public class NodeTestNodeType : NodeTest
    {
        private string _nodeType;

        /// <summary>
        /// The standard NodeType node tests
        /// </summary>
        public static NodeTestNodeType Comment;
        public static NodeTestNodeType Text;
        public static NodeTestNodeType Node;

        static NodeTestNodeType()
        {
            Comment = new NodeTestNodeType("comment()");
            Text = new NodeTestNodeType("text()");
            Node = new NodeTestNodeType("node()");
        }

        private NodeTestNodeType(string nodeType)
        {
            this._nodeType = nodeType;
        }

        /// <summary>
        /// Returns the node test expression in string format
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this._nodeType;
        }

    }

    internal class NodeTypeNodeTestFactory : NodeTestFactory
    {
        public bool Matches(string exp)
        {
            return exp.IndexOf("node()") >= 0 || exp.IndexOf("comment()") >= 0 || exp.IndexOf("text()") >= 0;
        }
        public NodeTest Create(string exp)
        {
            if (exp.IndexOf("text()") >= 0)
            {
                return NodeTestNodeType.Text;
            }
            else if (exp.IndexOf("comment()") >= 0)
            {
                return NodeTestNodeType.Comment;
            }
            else
            {
                return NodeTestNodeType.Node;
            }
        }
    }
}
