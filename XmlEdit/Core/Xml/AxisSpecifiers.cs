/*
 * 
 * AUTHOR: Sean T. McBeth, Sean T. McBeth
 * DATE: OCT-02-2007
 * 
 */

/*
 * http://www.w3.org/TR/xpath
 * 2.2 Axes
 *  The following axes are available:
 *      -the 'child' axis contains the children of the context node
 *      -the 'descendant' axis contains the descendants of the context node; a descendant is a
 *          child or a child of a child and so on; thus the descendant axis never contains attribute or namespace nodes
 *      -the 'parent' axis contains the parent of the context node, if there is one
 *      -the 'ancestor' axis contains the ancestors of the context node; the ancestors of the context node consist of 
 *          the parent of context node and the parent's parent and so on; thus, the ancestor axis will always include 
 *          the root node, unless the context node is the root node
 *      -the 'following-sibling' axis contains all the following siblings of the context node; if the context node is 
 *          an attribute node or namespace node, the following-sibling axis is empty
 *      -the 'preceding-sibling' axis contains all the preceding siblings of the context node; if the context node is 
 *          an attribute node or namespace node, the preceding-sibling axis is empty
 *      -the 'following' axis contains all nodes in the same document as the context node that are after the context 
 *          node in document order, excluding any descendants and excluding attribute nodes and namespace nodes
 *      -the 'preceding' axis contains all nodes in the same document as the context node that are before the context 
 *          node in document order, excluding any ancestors and excluding attribute nodes and namespace nodes
 *      -the 'attribute' axis contains the attributes of the context node; the axis will be empty unless the context 
 *          node is an element
 *      -the 'namespace' axis contains the namespace nodes of the context node; the axis will be empty unless the context 
 *          node is an element
 *      -the 'self' axis contains just the context node itself
 *      -the 'descendant-or-self' axis contains the context node and the descendants of the context node
 *      -the 'ancestor-or-self' axis contains the context node and the ancestors of the context node; thus, the ancestor 
 *          axis will always include the root node
 * 
 * NOTE: The ancestor, descendant, following, preceding and self axes partition a document (ignoring attribute and namespace nodes): they do not overlap and together they contain all the nodes in the document.
 * 
 * 
 * 2.3 Node Tests
 *  Every axis has a principal node type. If an axis can contain elements, then the principal node type is element; 
 *      otherwise, it is the type of the nodes that the axis can contain. Thus,
 *      -For the attribute axis, the principal node type is attribute. 
 *      -For the namespace axis, the principal node type is namespace. 
 *      -For other axes, the principal node type is element. 
 */
using System.Collections.Generic;

namespace XmlEdit
{
    /// <summary>
    /// The standard principal node types for nodes referenced by an XPath. Attributes and Namespaces
    /// have their own specific node types. All other AxisSpecifiers use the Element node type.
    /// </summary>
    public enum PrincipalNodeTypes
    {
        Namespace,
        Attribute,
        Element
    }
    /// <summary>
    /// All of the different types of axis specifiers that are available in XPath 1.0
    /// </summary>
    public class AxisSpecifier
    {
        /// <summary>
        /// All of the standard axis specifiers
        /// </summary>
        public static AxisSpecifier Child,
        Attribute,
        Descendant,
        DescendantOrSelf,
        Parent,
        Ancestor,
        AncestorOrSelf,
        Following,
        Preceding,
        FollowingSibling,
        PrecedingSibling,
        Self,
        Namespace;

        /// <summary>
        /// All of the previous axis specifiers, in a convenient list
        /// </summary>
        private static List<AxisSpecifier> AllSpecifiers;

        /// <summary>
        /// Creates a new AxisSpecifer and adds it to the list of all AxisSpecifiers
        /// </summary>
        /// <param name="nodeType"></param>
        /// <param name="extendedSyntax"></param>
        /// <param name="compactSyntax"></param>
        private AxisSpecifier(PrincipalNodeTypes nodeType, string extendedSyntax, string compactSyntax)
        {
            this.NodeType = nodeType;
            this.ExtendedSyntax = extendedSyntax + "::";
            this.SimpleSyntax = compactSyntax;
            AllSpecifiers.Add(this);
        }

        /// <summary>
        /// Creates the standard AxisSpecifiers. This is all done privately and statically because the AxisSpecifiers
        /// are standardized to only certain values (see documentation at top of file).
        /// </summary>
        static AxisSpecifier()
        {
            AllSpecifiers = new List<AxisSpecifier>();
            Child = new AxisSpecifier(PrincipalNodeTypes.Element, "child", "");
            Attribute = new AxisSpecifier(PrincipalNodeTypes.Attribute, "attribute", "@");
            Descendant = new AxisSpecifier(PrincipalNodeTypes.Element, "descendant", "/");
            DescendantOrSelf = new AxisSpecifier(PrincipalNodeTypes.Element, "descendant-or-self", null);
            Parent = new AxisSpecifier(PrincipalNodeTypes.Element, "parent", "..");
            Ancestor = new AxisSpecifier(PrincipalNodeTypes.Element, "ancestor", null);
            AncestorOrSelf = new AxisSpecifier(PrincipalNodeTypes.Element, "ancestor-or-self", null);
            Following = new AxisSpecifier(PrincipalNodeTypes.Element, "following", null);
            Preceding = new AxisSpecifier(PrincipalNodeTypes.Element, "preceding", null);
            FollowingSibling = new AxisSpecifier(PrincipalNodeTypes.Element, "following-sibling", null);
            PrecedingSibling = new AxisSpecifier(PrincipalNodeTypes.Element, "preceding-sibling", null);
            Self = new AxisSpecifier(PrincipalNodeTypes.Element, "self", ".");
            Namespace = new AxisSpecifier(PrincipalNodeTypes.Namespace, "namespace", null);
        }

        /// <summary>
        /// Resolves the string representation of an axis specifier to an AxisSpecifier object that
        /// can be used for XPath manipulations
        /// </summary>
        /// <param name="extendedSyntax">The Extended syntax version of the AxisSpecifier</param>
        /// <returns></returns>
        public static AxisSpecifier Find(string extendedSyntax)
        {
            extendedSyntax = extendedSyntax + "::";
            foreach (AxisSpecifier axis in AllSpecifiers)
            {
                if (axis.ExtendedSyntax.Equals(extendedSyntax))
                {
                    return axis;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the extended version of the string representation of the AxisSpecifier. All AxisSpecifiers have an 
        /// extended syntax version.
        /// </summary>
        public string ExtendedSyntax{get; private set;}

        /// <summary>
        /// Gets the simplified version of the string representation of the AxisSpecifier. Only certain AxisSpecifiers
        /// have simplified syntax versions: Child, Attribute, Descendant, Parent, and Self. All others will return
        /// null.
        /// </summary>
        public string SimpleSyntax{get; private set;}

        /// <summary>
        /// Gest the PrincipalNodeType that is associated with a specific AxisSpecifier. Attributes and Namespaces
        /// have their own specific node types. All other AxisSpecifiers use the Element node type.
        /// </summary>
        public PrincipalNodeTypes NodeType{get; private set;}
    }
}