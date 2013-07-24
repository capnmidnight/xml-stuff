/*
 * 
 * AUTHOR: Sean T. McBeth, Sean T. McBeth
 * DATE: OCT-02-2007
 * 
 */
/*
* From the XPath 1.0 Spec http://www.w3.org/TR/xpath
* 
* 2.1 Location Steps 
* A location step has three parts:
*      -an axis, which specifies the tree relationship between the nodes selected by the location step and the context node,
*      -a node test, which specifies the node type and expanded-name of the nodes selected by the location step, and
*      -zero or more predicates, which use arbitrary expressions to further refine the set of nodes selected by the location step.
*     
* The syntax for a location step is the axis name and node test separated by a double colon, followed by zero or more expressions
* each in square brackets. For example, in child::para[position()=1], child is the name of the axis, para is the node test and 
* [position()=1] is a predicate.
* 
* The node-set selected by the location step is the node-set that results from generating an initial node-set from the axis and 
* node-test, and then filtering that node-set by each of the predicates in turn.
* 
* The initial node-set consists of the nodes having the relationship to the context node specified by the axis, and having the 
* node type and expanded-name specified by the node test. For example, a location step descendant::para selects the para element 
* descendants of the context node: descendant specifies that each node in the initial node-set must be a descendant of the context; 
* para specifies that each node in the initial node-set must be an element named para. The available axes are described in [2.2 Axes]. 
* The available node tests are described in [2.3 Node Tests]. The meaning of some node tests is dependent on the axis.
* 
* The initial node-set is filtered by the first predicate to generate a new node-set; this new node-set is then filtered using the 
* second predicate, and so on. The final node-set is the node-set selected by the location step. The axis affects how the expression 
* in each predicate is evaluated and so the semantics of a predicate is defined with respect to an axis. See [2.4 Predicates].
*/

namespace XmlEdit
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// A single component of an XPath expression
    /// </summary>
    public class LocationStep : ICloneable
    {
        private static Regex predicateMatcher;
        static LocationStep()
        {
            predicateMatcher = new Regex(@"\[[a-zA-Z0-9`~!@#$%^&*()_+\-={}|\\:"";'<>?,./]*\]");
        }

        private AxisSpecifier _axis;
        private NodeTest _nodeTest;
        private List<Predicate> _predicates;
        /// <summary>
        /// Creates a new Location Step for a provided axis type and node test
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="nodeTest"></param>
        public LocationStep(AxisSpecifier axis, NodeTest nodeTest)
        {
            this._axis = axis;
            this._nodeTest = nodeTest;
            this._predicates = new List<Predicate>();
        }

        public LocationStep(string exp)
        {
            if (exp.IndexOf("::") > 0)
            {
                int start = exp.IndexOf("::");
                string axisSpec = exp.Substring(0, start);
                exp = exp.Substring(start + 2);
                this._axis = AxisSpecifier.Find(axisSpec);
            }
            else
            {
                if (exp[0] == '@')
                {
                    this._axis = AxisSpecifier.Attribute;
                    exp = exp.Substring(1);
                }
                else if (exp.Length >= 2 && exp.Substring(0, 2).Equals(".."))
                {
                    this._axis = AxisSpecifier.Parent;
                }
                else if (exp[0] == '.')
                {
                    this._axis = AxisSpecifier.Self;
                }
                else
                {
                    this._axis = AxisSpecifier.Child;
                }
            }
            this._predicates = new List<Predicate>();
            MatchCollection matches = predicateMatcher.Matches(exp);
            if (matches.Count > 0)
            {
                foreach (Match match in matches)
                {
                    this._predicates.Add(new Predicate(match.Value));
                }
                exp = exp.Substring(0, matches[0].Index);
            }
            if (this._axis == AxisSpecifier.Parent)
            {
                this._nodeTest = NodeTest.Create("*");
            }
            else
            {
                this._nodeTest = NodeTest.Create(exp);
            }
        }

        /// <summary>
        /// Appends the Predicate parameter to the current step, and returns itself
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public LocationStep AppendPredicate(Predicate predicate)
        {
            this._predicates.Add(predicate);
            return this;
        }
        public AxisSpecifier AxisSpecifier
        {
            get
            {
                return this._axis;
            }
        }
        public NodeTest NodeTest
        {
            get
            {
                return this._nodeTest;
            }
        }
        public List<Predicate> Predicates
        {
            get
            {
                return this._predicates;
            }
        }
        /// <summary>
        /// Gets the full syntax expression for this Location Step
        /// </summary>
        public string ToString(bool useExtendedSyntax)
        {
            StringBuilder builder = new StringBuilder();
            if (useExtendedSyntax)
            {
                builder.Append(_axis.ExtendedSyntax);
            }
            else if (_axis.SimpleSyntax != null)
            {
                builder.Append(_axis.SimpleSyntax);
            }
            else
            {
                throw new Exception("Axis Specifier does not have a simplified form.");
            }


            if (this.IsAttribute)
            {
                builder.Append(_nodeTest.Test);
            }
            else if (!(this._axis == AxisSpecifier.Parent && !useExtendedSyntax))
            {
                builder.Append(_nodeTest.ToString());
            }
            foreach (Predicate predicate in _predicates)
            {
                if (useExtendedSyntax)
                {
                    builder.AppendFormat(predicate.ExpandedExpression);
                }
                else
                {
                    builder.AppendFormat(predicate.PredicateExpression);
                }
            }
            return builder.ToString();
        }

        public override string ToString()
        {
            return this.ToString(false);
        }
        /// <summary>
        /// gets the basic form of this location step portion of an XPath, sans the predicate statements
        /// </summary>
        public string BaseXPath
        {
            get
            {
                return this._axis.SimpleSyntax + this._nodeTest.ToString();
            }
        }
        /// <summary>
        /// Compares two location steps for equality. They must represent the same Axis Specifier, same Node Test
        /// and equivalent sets of Predicates.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is LocationStep)
            {
                LocationStep step = (LocationStep)obj;
                return this.ToString(true).Equals(step.ToString(true));
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return this.ToString(true).GetHashCode();
        }

        /// <summary>
        /// Returns true if this location step represents an attribute
        /// </summary>
        public bool IsAttribute
        {
            get
            {
                return this._axis == AxisSpecifier.Attribute;
            }
        }

        /// <summary>
        /// Creates a new instance of a LocationStep with the same value as this LocationStep
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return new LocationStep(this.ToString(true));
        }

        public void SetIndexPredicate(int nodeIndex)
        {
            this._predicates.Clear();
            this.AppendPredicate(new Predicate(nodeIndex.ToString()));
        }
    }
}