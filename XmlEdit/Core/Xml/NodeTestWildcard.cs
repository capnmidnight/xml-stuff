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
    /// Creates a basic NodeTestNamedNode specifically for Wildcard access. A wild card node is the only type of standard
    /// node test.
    /// </summary>
    public class NodeTestWildcard : NodeTestNamedNode
    {
        public static NodeTestWildcard NoNamespace;
        static NodeTestWildcard()
        {
            NoNamespace = new NodeTestWildcard();
        }
        private NodeTestWildcard()
            : base("*")
        {
        }
        public NodeTestWildcard(string xmlNamespace)
            : base(xmlNamespace, "*")
        {
        }
    }
    /// <summary>
    /// Automated factory for determining if an expression matches a Node Test type, and creating
    /// those Node Test objects.
    /// </summary>
    internal class WildcardNodeTestFactory : NodeTestFactory
    {
        public bool Matches(string exp)
        {
            return exp.IndexOf("*") >= 0;
        }
        public NodeTest Create(string exp)
        {
            int split = exp.IndexOf(":");
            if (split > 0)
            {
                string nmspc = exp.Substring(0, split);
                return new NodeTestWildcard(nmspc);
            }
            else
            {
                return NodeTestWildcard.NoNamespace;
            }
        }
    } 
}
