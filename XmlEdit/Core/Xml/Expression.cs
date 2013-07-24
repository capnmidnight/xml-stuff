/*
 * 
 * AUTHOR: Sean T. McBeth, Sean T. McBeth
 * DATE: OCT-02-2007
 * 
 */
/*
* From the XPath 1.0 Spec http://www.w3.org/TR/xpath
* 
* 1 Introduction
* XPath models an XML document as a tree of nodes. There are different types of nodes, including element nodes, attribute nodes and text nodes.
* XPath defines a way to compute a string-value for each type of node. Some types of nodes also have names. XPath fully supports XML Namespaces
* [XML Names]. Thus, the name of a node is modeled as a pair consisting of a local part and a possibly null namespace URI; this is called an 
* expanded-name. The data model is described in detail in [5 Data Model].
*/
namespace XmlEdit
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// An XPath expression parser. Enables advanced control over XPath expression that would otherwise
    /// be extremely tedious using only strings.
    /// </summary>
    public class Expression : ICloneable, IEnumerable
    {
        private List<LocationStep> _components;
        private bool _isFromRoot;
        private string _standardPrefix;
        /// <summary>
        /// Used internally for constructing XPaths on the fly
        /// </summary>
        /// <param name="isFromRoot"></param>
        private Expression()
        {
            _components = new List<LocationStep>();
        }
        /// <summary>
        /// Tokenizes a provided XPath expression
        /// </summary>
        /// <param name="exp"></param>
        public Expression(string exp)
            : this()
        {
            try
            {
                exp = exp.Replace("//", "/descendant::");
                if (exp[0] == '/')
                {
                    this._isFromRoot = true;
                    exp = exp.Substring(1);
                }
                else
                {
                    this._isFromRoot = false;
                }
                string[] parts = exp.Split('/');
                string namespacePrefix = null;
                bool allHavePrefix = true;
                foreach (string part in parts)
                {
                    LocationStep step = new LocationStep(part);
                    this._components.Add(step);
                    if (step.NodeTest.Namespace == null || (namespacePrefix != null && !step.NodeTest.Namespace.Equals(namespacePrefix)))
                    {
                        allHavePrefix = false;
                    }
                    else
                    {
                        namespacePrefix = step.NodeTest.Namespace;
                    }
                }
                if (allHavePrefix)
                {
                    this._standardPrefix = namespacePrefix;
                }
            }
            catch (Exception exception)
            {
                throw new Exception(string.Format("Could not construct XPath expression from input string(\"{0}\")", exp), exception);
            }
        }

        /// <summary>
        /// Creates an XPath expression and provides each location step with a namespace prefix
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="prefix"></param>
        public Expression(string exp, string prefix)
            : this(exp)
        {
            this.SetNamespacePrefix(prefix);
        }
        /// <summary>
        /// Gets a specific location step in the expression
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public LocationStep this[int index]
        {
            get
            {
                return _components[index];
            }
        }
        /// <summary>
        /// Gets the last location step in this Expression
        /// </summary>
        public LocationStep LastStep
        {
            get
            {
                if (this._components.Count == 0)
                {
                    return null;
                }
                return this._components[this._components.Count - 1];
            }
        }
        /// <summary>
        /// Gets the number of location steps in this Expression
        /// </summary>
        public int StepCount
        {
            get
            {
                return _components.Count;
            }
        }
        /// <summary>
        /// Returns true if the Expression starts by referencing the root element of the document,
        /// i.e., the XPath begins with a forward-slash '/'
        /// </summary>
        public bool IsFromRoot
        {
            get
            {
                return this._isFromRoot;
            }
        }
        /// <summary>
        /// Returns true if this Expression references a path to an attribute
        /// </summary>
        public bool IsAttribute
        {
            get
            {
                if (this.LastStep == null)
                {
                    return false;
                }
                return this.LastStep.IsAttribute;
            }
        }

        /// <summary>
        /// Creates a full string representation of the XPath
        /// </summary>
        /// <param name="useExtendedSyntax">true if the XPath uses advanced XPath features that do
        /// not have simplified syntax equivalents</param>
        /// <returns></returns>
        public string ToString(bool useExtendedSyntax)
        {
            StringBuilder builder = new StringBuilder();
            if (this._isFromRoot)
            {
                builder.Append("/");
            }
            for (int index = 0; index < _components.Count; ++index)
            {
                builder.Append(_components[index].ToString(useExtendedSyntax));
                if (index < _components.Count - 1)
                {
                    builder.Append("/");
                }
            }
            return builder.ToString();
        }

        /// <summary>
        /// Creates a full XPath expression using the Extended XPath Syntax
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.ToString(true);
        }

        /// <summary>
        /// Gets the expression that points to the parent element of the element referenced by this expression
        /// </summary>
        public Expression Parent
        {
            get
            {
                Expression parent = new Expression();
                for (int index = 0; index < this._components.Count - 1; ++index)
                {
                    parent._components.Add(this._components[index]);
                }
                return parent;
            }
        }
        /// <summary>
        /// Compares two expressions for equality
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is Expression)
            {
                Expression exp = (Expression)obj;
                //The Extended Syntax for the an expression is always accurate and complete.
                //This provides a simple means for comparing XPath expressions.
                return this.ToString(true).Equals(exp.ToString(true));
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Gets the basic form of the XPath in simplified format that this Expression represents, sans Predicate specifiers in
        /// each location step
        /// </summary>
        public string BaseXPath
        {
            get
            {
                StringBuilder builder = new StringBuilder("/");
                for (int index = 0; index < this._components.Count; ++index)
                {
                    builder.Append(this._components[index].BaseXPath);
                    if (index < this._components.Count - 1)
                    {
                        builder.Append("/");
                    }
                }
                return builder.ToString();
            }
        }

        public override int GetHashCode()
        {
            return this.ToString(true).GetHashCode();
        }

        /// <summary>
        /// Builds an Expression object that represents the node that is in common
        /// between the current expression and the supplied expression.
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public int FindCommonBranchIndex(Expression exp)
        {
            int length = Math.Min(this.StepCount, exp.StepCount);
            int index;
            for (index = 0; index < length; ++index)
            {
                if (!this[index].Equals(exp[index]))
                {
                    break;
                }
            }
            return index;
        }
        
        /// <summary>
        /// Builds an expression that runs from the root to the provide step index in the current
        /// expressions step path.
        /// </summary>
        /// <param name="stepIndex"></param>
        /// <returns></returns>
        public Expression SubExpression(int length)
        {
            return this.SubExpression(0, length);
        }

        /// <summary>
        /// Generates a sub expression that begins with the start index location step and fills the full length
        /// </summary>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public Expression SubExpression(int start, int length)
        {
            Expression exp = new Expression();
            exp._isFromRoot = (start == 0 && this._isFromRoot);
            for (int index = 0; index < length && index + start < this.StepCount; ++index)
            {
                exp._components.Add(this[index + start]);
            }
            return exp;
        }

        /// <summary>
        /// Salt the entire XPath expression with a namespace prefix
        /// </summary>
        /// <param name="prefix"></param>
        public void SetNamespacePrefix(string prefix)
        {
            this._standardPrefix = prefix;
            foreach (LocationStep step in this._components)
            {
                step.NodeTest.Namespace = prefix;
            }
        }

        /// <summary>
        /// Adds a location step to the end of the xpath, if the current xpath does not reference an Attribute
        /// </summary>
        /// <param name="locationStep"></param>
        public void AddStep(LocationStep locationStep)
        {
            if (!this.IsAttribute)
            {
                this._components.Add(locationStep);
            }
            if (this._standardPrefix != null)
            {
                locationStep.NodeTest.Namespace = this._standardPrefix;
            }
        }

        /// <summary>
        /// Adds a location step to the end of the xpath
        /// </summary>
        /// <param name="exp"></param>
        public void AddStep(string exp)
        {
            this.AddStep(new LocationStep(exp));
        }

        /// <summary>
        /// Creates a new instance of the Expression with the same value as this expression
        /// </summary>
        public object Clone()
        {
            return new Expression(this.ToString(true));
        }

        /// <summary>
        /// Returns an enumerator over the list of location steps
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            return this._components.GetEnumerator();
        }

        /// <summary>
        /// Returns true if the named node test exists in the XPath expression
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool ContainsNodeTest(string p)
        {
            foreach (LocationStep step in this._components)
            {
                if (step.NodeTest.Test.Equals(p))
                {
                    return true;
                }
            }
            return false;
        }

        public LocationStep GetLocationStepByNodeTest(string p)
        {
            LocationStep locationStep = null;
            foreach (LocationStep step in this._components)
            {
                if (step.NodeTest.Test.Equals(p))
                {
                    locationStep = step;
                }
            }
            return locationStep;
        }

        public void RemoveLastStep()
        {
            if (this._components.Count > 0)
            {
                this._components.RemoveAt(this._components.Count - 1);
            }
        }

        public Expression DuplicateAndAppend(string stub)
        {
            Expression xpath = new Expression(string.Format("{0}/{1}", this.ToString(true), stub));
            return xpath;
        }
    }
}