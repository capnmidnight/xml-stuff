/*
 * 
 * AUTHOR: Sean T. McBeth, Sean T. McBeth
 * DATE: OCT-02-2007
 * 
 */
/*
* From the XPath 1.0 Spec http://www.w3.org/TR/xpath
* 
* 2.3 Node Tests
* Every axis has a principal node type. If an axis can contain elements, then the principal node type is element; otherwise, it is
* the type of the nodes that the axis can contain. Thus,
* 
*      -For the attribute axis, the principal node type is attribute. 
*      -For the namespace axis, the principal node type is namespace. 
*      -For other axes, the principal node type is element. 
* 
* A node test that is a QName is true if and only if the type of the node (see [5 Data Model]) is the principal node type and has
* an expanded-name equal to the expanded-name specified by the QName. For example, child::para selects the para element children
* of the context node; if the context node has no para children, it will select an empty set of nodes. attribute::href selects the
* href attribute of the context node; if the context node has no href attribute, it will select an empty set of nodes.
* 
* A QName in the node test is expanded into an expanded-name using the namespace declarations from the expression context. This is
* the same way expansion is done for element type names in start and end-tags except that the default namespace declared with xmlns
* is not used: if the QName does not have a prefix, then the namespace URI is null (this is the same way attribute names are expanded).
* It is an error if the QName has a prefix for which there is no namespace declaration in the expression context.
* 
* A node test * is true for any node of the principal node type. For example, child::* will select all element children of the context
* node, and attribute::* will select all attributes of the context node.
* 
* A node test can have the form NCName:*. In this case, the prefix is expanded in the same way as with a QName, using the context
* namespace declarations. It is an error if there is no namespace declaration for the prefix in the expression context. The node
* test will be true for any node of the principal type whose expanded-name has the namespace URI to which the prefix expands, regardless
* of the local part of the name.
* 
* The node test text() is true for any text node. For example, child::text() will select the text node children of the context node.
* Similarly, the node test comment() is true for any comment node, and the node test processing-instruction() is true for any processing
* instruction. The processing-instruction() test may have an argument that is Literal; in this case, it is true for any processing
* instruction that has a name equal to the value of the Literal.
* 
* A node test node() is true for any node of any type whatsoever.
*/
namespace XmlEdit
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    public interface NodeTestFactory
    {
        bool Matches(string expression);
        NodeTest Create(string expression);
    }

    /// <summary>
    /// Base NodeTest type. Concrete subclasses create specific NodeTest types
    /// </summary>
    public class NodeTest
    {
        internal static List<NodeTestFactory> AllFactories;
        static NodeTest()
        {
            AllFactories = new List<NodeTestFactory>();
            AllFactories.Add(new ProcessingInstructionNodeTestFactory());
            AllFactories.Add(new NodeTypeNodeTestFactory());
            AllFactories.Add(new WildcardNodeTestFactory());
            AllFactories.Add(new NamedNodeNodeTestFactory());
        }
        /// <summary>
        /// Parses the string expression and returns the correct type of NodeTest needed.
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public static NodeTest Create(string exp)
        {
            foreach (NodeTestFactory factory in AllFactories)
            {
                if (factory.Matches(exp))
                {
                    return factory.Create(exp);
                }
            }
            return null;
        }

        protected string _xmlNamespace, _nodeName;

        public string Namespace
        {
            get
            {
                return this._xmlNamespace;
            }
            set
            {
                this._xmlNamespace = value;
            }
        }
        public string Test
        {
            get
            {
                return this._nodeName;
            }
        }
        public override string ToString()
        {
            if (this._xmlNamespace != null && this._xmlNamespace.Length > 0)
            {
                return this._xmlNamespace + ":" + this._nodeName;
            }
            else
            {
                return this._nodeName;
            }
        }
    }
}