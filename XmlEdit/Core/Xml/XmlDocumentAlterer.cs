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
    using System.Xml;
    using System.Xml.Schema;
    /// <summary>
    /// Values for indicating a type of action to take during failure when attempting to retrieve a value from the document.
    /// </summary>
    public enum FailureReaction
    {
        Ignore,
        GenerateErrorOnNull,
        GenerateErrorOnEmpty
    }
    /// <summary>
    /// Utility methods for altering XML documents. Can iteratively fill in missing elements based off of XPath expressions.
    /// </summary>
    public class XmlDocumentAlterer
    {
        private XmlDocument _document;
        private XmlNamespaceManager _namespaces;
        private XmlSchema _schema;
        private XPathOrderComparer _comparer;
        private XmlSchemaValidationException _error;
        /// <summary>
        /// Creates a new document alterer around a new document
        /// </summary>
        public XmlDocumentAlterer()
            : this(new XmlDocument())
        {
        }

        /// <summary>
        /// Creates a new document alterer, enforcing adherence to a specified XSD
        /// </summary>
        /// <param name="schema"></param>
        public XmlDocumentAlterer(XmlSchema schema)
            : this(new XmlDocument(), schema)
        {
        }

        /// <summary>
        /// Creates a new document alterer around a preexisting document
        /// </summary>
        /// <param name="document"></param>
        public XmlDocumentAlterer(XmlDocument document)
            : this(document, null)
        {
        }

        /// <summary>
        /// Creates a new document alterer around a preexisting document, enforcing adherence to a specified XSD
        /// </summary>
        /// <param name="document"></param>
        /// <param name="schema"></param>
        public XmlDocumentAlterer(XmlDocument document, XmlSchema schema)
        {
            this._document = document;
            this._namespaces = new XmlNamespaceManager(this._document.NameTable);
            this._schema = schema;
        }

        /// <summary>
        /// Creates the XPathComparer object used to enforce the document constraints when all the necessary information is available
        /// </summary>
        private void ResolveSchemaComparer()
        {
            if (this._schema != null && this._document.DocumentElement != null && this._comparer == null)
            {
                this._comparer = new XPathOrderComparer(this._schema, this._document.DocumentElement.LocalName);
            }
        }

        /// <summary>
        /// Add a namespace to the namespace manager
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="url"></param>
        public void AddNamespace(string prefix, string url)
        {
            this._namespaces.AddNamespace(prefix, url);
        }
        /// <summary>
        /// Selects a node from the underlying document. If that node does not exist, it will be filled in
        /// </summary>
        /// <param name="xpath"></param>
        /// <returns></returns>
        public XmlNodeList SelectNodes(Expression xpath)
        {
            string pathStr = xpath.ToString(true);
            XmlNodeList nodes = this._document.SelectNodes(pathStr, this._namespaces);
            if (nodes.Count == 0)
            {
                FillIn(xpath);
                nodes = this._document.SelectNodes(pathStr, this._namespaces);
            }
            return nodes;
        }
        /// <summary>
        /// Gets the XML document that is being altered
        /// </summary>
        public XmlDocument Document
        {
            get
            {
                return this._document;
            }
        }
        /// <summary>
        /// Creates the necessary nodes to make the XPath expression return a result
        /// </summary>
        /// <param name="xpath"></param>
        public void FillIn(Expression xpath)
        {
            FillIn(xpath, this._document);
        }
        private void FillIn(Expression xpath, XmlNode topContext)
        {
            //initialize the context node
            XmlNode current = topContext;
            //start looking down the document tree
            for (int length = 1; length <= xpath.StepCount; ++length)
            {
                Expression sub = xpath.SubExpression(length);

                //check to see if the node already exists...
                XmlNodeList nodes = topContext.SelectNodes(sub.ToString(true), this._namespaces);

                if (nodes.Count == 0)
                {
                    //... and create it if it doesn't
                    LocationStep step = sub.LastStep;
                    if (step.IsAttribute)
                    {
                        XmlAttribute attribute = this._document.CreateAttribute(step.NodeTest.Test);
                        current.Attributes.Append(attribute);
                        current = attribute;
                    }
                    else
                    {
                        string namespaceUri = this._namespaces.LookupNamespace(step.NodeTest.Namespace);
                        XmlNode element = this._document.CreateElement(step.NodeTest.Namespace, step.NodeTest.Test, namespaceUri);
                        current.AppendChild(element);
                        current = element;
                    }

                    //if there were attribute filters applied to the xpath, then apply them here
                    if (step.Predicates.Count > 0)
                    {
                        foreach (Predicate predicate in step.Predicates)
                        {
                            if (predicate.IsAttributeFilter)
                            {
                                XmlAttribute attribute = this._document.CreateAttribute(predicate.FilterKey.Substring(1));
                                attribute.Value = predicate.FilterValue;
                                current.Attributes.Append(attribute);
                            }
                        }
                    }
                }
                else if (nodes.Count == 1) //...otherwise, advance the context
                {
                    current = nodes[0];
                }
                else //in the case of multiples, do it to all of them
                {
                    foreach (XmlNode child in nodes)
                    {
                        FillIn(xpath.SubExpression(length, xpath.StepCount - length), child);
                    }
                }
            }
        }

        /// <summary>
        /// Sets the value of a node at a given XPath, filling in the necessary middle nodes
        /// </summary>
        public void SetValue(Expression xpath, string value)
        {
            if (this._comparer != null && !this._comparer.IsValid(xpath))
            {
                throw new Exception(string.Format("The provided XPath does not match a valid path as defined by the XML Schema: {0}", xpath));
            }
            else
            {
                XmlNodeList nodes = this.SelectNodes(xpath);
                foreach (XmlNode node in nodes)
                {
                    if (xpath.IsAttribute)
                    {
                        node.Value = value;
                    }
                    else
                    {
                        node.InnerText = value;
                    }
                }
            }
        }

        /// <summary>
        /// Sets the value of a node at a given XPath, filling in the necessary middling nodes
        /// </summary>
        public void SetValue(string xpath, string value)
        {
            this.SetValue(new Expression(xpath), value);
        }

        /// <summary>
        /// Get a string value from the document
        /// </summary>
        /// <param name="xpath"></param>
        /// <returns></returns>
        public string GetValue(Expression xpath, FailureReaction reaction)
        {
            XmlNode node = this._document.SelectSingleNode(xpath.ToString(true), this._namespaces);
            string returnValue = null;
            if (node != null)
            {
                if (xpath.IsAttribute)
                {
                    returnValue = node.Value;
                }
                else
                {
                    returnValue = node.InnerText;
                }
            }
            ValidateValue(xpath, reaction, returnValue);
            return returnValue;
        }
        /// <summary>
        /// Get all string values that match the given xpath from the document
        /// </summary>
        /// <param name="xpath"></param>
        /// <returns></returns>
        public string[] GetValues(Expression xpath, FailureReaction reaction)
        {
            XmlNodeList nodeList = this._document.SelectNodes(xpath.ToString(true), this._namespaces);
            List<string> returnValues = new List<string>();
            if (nodeList != null)
            {
                foreach (XmlNode node in nodeList)
                {
                    if (xpath.IsAttribute)
                    {
                        returnValues.Add(node.Value);
                    }
                    else
                    {
                        returnValues.Add(node.InnerText);
                    }
                }
            }
            ValidateValues(xpath, reaction, returnValues);
            return returnValues.ToArray();
        }
        private static void ValidateValue(Expression xpath, FailureReaction reaction, string returnValue)
        {
            if (reaction == FailureReaction.GenerateErrorOnNull && returnValue == null ||
                (reaction == FailureReaction.GenerateErrorOnEmpty && (returnValue == null || returnValue.Length == 0)))
            {
                throw new Exception(string.Format("Could not find a value at the XPath({0})", xpath.ToString(true)));
            }
        }
        private static void ValidateValues(Expression xpath, FailureReaction reaction, List<string> returnValues)
        {
            foreach (string value in returnValues)
            {
                ValidateValue(xpath, reaction, value);
            }
        }

        /// <summary>
        /// Get a string value from the document
        /// </summary>
        /// <param name="xpath"></param>
        /// <returns></returns>
        public string GetValue(string xpath, FailureReaction reaction)
        {
            return this.GetValue(new Expression(xpath), reaction);
        }

        /// <summary>
        /// Get the number of nodes that the XPath returns
        /// </summary>
        /// <param name="xpath"></param>
        /// <returns></returns>
        public int CountNodes(Expression xpath)
        {
            XmlNodeList nodes = this._document.SelectNodes(xpath.ToString(true), this._namespaces);
            return nodes.Count;
        }

        /// <summary>
        /// Gets or sets the XML Schema (XSD) used to constrain the document
        /// </summary>
        public XmlSchema Schema
        {
            get
            {
                return this._schema;
            }
            set
            {
                if (value == null || !value.Equals(this._schema))
                {
                    this._comparer = null;
                    this._schema = value;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating if the underlying XML document passes the validation
        /// of the XML Scheme set in the Schema property.
        /// </summary>
        public bool IsValid
        {
            get
            {
                bool isValid = true;
                if (this._schema != null)
                {
                    this._document.Schemas.Add(this._schema);
                    using (System.IO.MemoryStream buffer = new System.IO.MemoryStream())
                    {
                        this._document.Save(buffer);
                        buffer.Flush();
                        buffer.Position = 0;
                        XmlReaderSettings settings = new XmlReaderSettings();
                        settings.CheckCharacters = true;
                        settings.CloseInput = true;
                        settings.ConformanceLevel = ConformanceLevel.Document;
                        settings.IgnoreComments = true;
                        settings.IgnoreProcessingInstructions = true;
                        settings.IgnoreWhitespace = true;
                        settings.Schemas.Add(this._schema);
                        settings.ValidationType = ValidationType.Schema;
                        try
                        {
                            using (XmlReader reader = XmlReader.Create(buffer, settings))
                            {
                                while (reader.Read())
                                    ;
                            }
                        }
                        catch (XmlSchemaValidationException exception)
                        {
                            isValid = false;
                            this._error = exception;
                        }
                    }
                }
                return isValid;
            }
        }

        /// <summary>
        /// Gets the XmlSchemaValidationException that occured if the Document did not pass validation
        /// </summary>
        public XmlSchemaValidationException SchemaValidationError
        {
            get
            {
                return this._error;
            }
        }

        /// <summary>
        /// Sorts the elements of the XML document according to an XML Schema defined in the Schema property
        /// </summary>
        public void SortElements()
        {
            this.ResolveSchemaComparer();
            if (this._comparer != null)
            {
                Expression start = new Expression(string.Format("/" + this._document.DocumentElement.Name), "ns0");
                SortElements(start);
            }
        }

        private void SortElements(Expression contextXPath)
        {
            XmlNode context = this._document.SelectSingleNode(contextXPath.ToString(true), this._namespaces);
            if (context.NodeType == XmlNodeType.Element)
            {
                Dictionary<string, int> elementCounts = new Dictionary<string, int>();
                List<Expression> keys = new List<Expression>();
                Dictionary<Expression, XmlNode> children = new Dictionary<Expression, XmlNode>();
                for (int index = 0; index < context.ChildNodes.Count; ++index)
                {
                    XmlNode child = context.ChildNodes[index];
                    if (child.NodeType == XmlNodeType.Element)
                    {
                        string name = child.LocalName;
                        if (!elementCounts.ContainsKey(name))
                        {
                            elementCounts.Add(name, 0);
                        }
                        elementCounts[name]++;

                        Expression element = (Expression)contextXPath.Clone();
                        element.AddStep(string.Format("{0}[{1}]", name, elementCounts[name]));

                        keys.Add(element);
                        children.Add(element, child);
                    }
                }
                foreach (XmlNode child in children.Values)
                {
                    context.RemoveChild(child);
                }
                keys.Sort(this._comparer);
                foreach (Expression key in keys)
                {
                    context.AppendChild(children[key]);
                }
                foreach (Expression key in keys)
                {
                    this.SortElements(key);
                }
            }
        }

        public Expression CreateExpressionFor(XmlNode node)
        {
            List<LocationStep> steps = new List<LocationStep>();
            XmlNode current = node;
            while (current != null)
            {
                steps.Insert(0, new LocationStep(AxisSpecifier.Descendant, NodeTest.Create("")));
                current = current.ParentNode;
            }
            return null;
        }
    }
}
