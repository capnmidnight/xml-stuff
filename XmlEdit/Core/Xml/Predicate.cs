/*
 * 
 * AUTHOR: Sean T. McBeth, Sean T. McBeth
 * DATE: OCT-02-2007
 * 
 */
/*
* http://www.w3.org/TR/xpath
* 2.4 Predicates
*  An axis is either a forward axis or a reverse axis. An axis that only ever contains the context node
*      or nodes that are after the context node in document order is a forward axis. An axis that only 
*      ever contains the context node or nodes that are before the context node in document order is a 
*      reverse axis. Thus, the ancestor, ancestor-or-self, preceding, and preceding-sibling axes are 
*      reverse axes; all other axes are forward axes. Since the self axis always contains at most one 
*      node, it makes no difference whether it is a forward or reverse axis. The proximity position of 
*      a member of a node-set with respect to an axis is defined to be the position of the node in the 
*      node-set ordered in document order if the axis is a forward axis and ordered in reverse document 
*      order if the axis is a reverse axis. The first position is 1.
* 
*  A predicate filters a node-set with respect to an axis to produce a new node-set. For each node in the
*      node-set to be filtered, the PredicateExpr is evaluated with that node as the context node, with 
*      the number of nodes in the node-set as the context size, and with the proximity position of the 
*      node in the node-set with respect to the axis as the context position; if PredicateExpr evaluates 
*      to true for that node, the node is included in the new node-set; otherwise, it is not included.
* 
*  A PredicateExpr is evaluated by evaluating the Expr and converting the result to a boolean. If the 
*      result is a number, the result will be converted to true if the number is equal to the context 
*      position and will be converted to false otherwise; if the result is not a number, then the result 
*      will be converted as if by a call to the boolean function. Thus a location path para[3] is equivalent 
*      to para[position()=3].
*/
namespace XmlEdit
{
    /// <summary>
    /// A predicate is a test applied to an XML Node to determine if it matches a certain XPath
    /// </summary>
    public class Predicate
    {
        private string _value1, _value2;
        private bool _isKeyValuePair;
        /// <summary>
        /// Create a new Predicate from a string expression. If that expression includes the square brackets
        /// '[' and ']', the constructor will automatically strip them away.
        /// </summary>
        /// <param name="expression"></param>
        public Predicate(string expression)
        {
            if (expression[0] == '[')
            {
                expression = expression.Substring(1);
            }
            if (expression[expression.Length - 1] == ']')
            {
                expression = expression.Substring(0, expression.Length - 1);
            }

            int mid = expression.IndexOf('=');
            if (mid > 0)
            {
                this._value1 = expression.Substring(0, mid);
                this._value2 = expression.Substring(mid + 1);
                char c = this._value2[0];
                if (c == '\'' || c == '\"')
                {
                    this._value2 = this._value2.Substring(1);
                }
                c = this._value2[this._value2.Length - 1];
                if (c == '\'' || c == '\"')
                {
                    this._value2 = this._value2.Substring(0, this._value2.Length - 1);
                }
            }
            else
            {
                this._value1 = "position()";
                this._value2 = expression;
            }
            this._isKeyValuePair = !this._value1.Equals("position()");
        }
        /// <summary>
        /// Returns true if the Predicate references an element index when element types may be repeated.
        /// </summary>
        public bool IsElementIndex
        {
            get
            {
                if (!this._isKeyValuePair)
                {
                    int iOut = -1;
                    return int.TryParse(this._value2, out iOut);
                }
                return false;
            }
        }
        /// <summary>
        /// If the Predicate references an element index, returns the index it references. Otherwise, returns
        /// -1
        /// </summary>
        public int ElementIndex
        {
            get
            {
                if (this.IsElementIndex)
                {
                    return int.Parse(this._value2);
                }
                else
                {
                    return -1;
                }
            }
        }
        /// <summary>
        /// Gets the full Predicate Expression, including the square brackets
        /// </summary>
        public string PredicateExpression
        {
            get
            {
                if (this._isKeyValuePair)
                {
                    return this.ExpandedExpression;
                }
                return string.Format("[{0}]", this._value2);
            }
        }

        /// <summary>
        /// Gets the fully expanded syntax for an Element index test. If the expression is
        /// not an element index test, then just returns the normal expression
        /// </summary>
        public string ExpandedExpression
        {
            get
            {
                return string.Format("[{0}='{1}']", this._value1, this._value2);
            }
        }

        /// <summary>
        /// Returns the internal Expression value
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.PredicateExpression;
        }

        /// <summary>
        /// Returns true if the predicate is used to filter on an attribute
        /// </summary>
        public bool IsAttributeFilter
        {
            get
            {
                return this._isKeyValuePair && this._value1[0] == '@';
            }
        }

        /// <summary>
        /// Gets the name of the attribute that the predicate is filtering on, if it is an attribute filter
        /// </summary>
        public string FilterKey
        {
            get
            {
                if (this.IsAttributeFilter)
                {
                    return this._value1;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets the value used to filter the filtering attribute, if the predicate is an attribute filter
        /// </summary>
        public string FilterValue
        {
            get
            {
                if (IsAttributeFilter)
                {
                    return this._value2;
                }
                else
                {
                    return null;
                }
            }
        }

    }
}