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
    /// A node test for a specifically named element. Also used as the baseclass for the Wildcard node test.
    /// </summary>
    public class NodeTestNamedNode : NodeTest
    {
        public NodeTestNamedNode(string nodeName)
            : this(null, nodeName)
        {
        }

        public NodeTestNamedNode(string xmlNamespace, string nodeName)
        {
            this.Namespace = xmlNamespace;
            this.Test = nodeName;
        }
    }
    internal class NamedNodeNodeTestFactory : NodeTestFactory
    {
        public bool Matches(string exp)
        {
            foreach (NodeTestFactory factory in NodeTest.AllFactories)
            {
                if (!factory.Equals(this))
                {
                    if (factory.Matches(exp))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public NodeTest Create(string exp)
        {
            int split = exp.IndexOf(":");
            if (split > 0)
            {
                string nmsp = exp.Substring(0, split);
                exp = exp.Substring(split + 1, exp.Length - split - 1);
                return new NodeTestNamedNode(nmsp, exp);
            }
            return new NodeTestNamedNode(exp);
        }
    }
}
