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
    using System.IO;
    using System.Text;
    using System.Xml.Schema;

    public class XPathOrderComparer : Comparer<string>, IComparer<Expression>
    {
        private List<string> _orderedPaths;
        Dictionary<string, XmlSchemaObject> _elements;
        private XmlSchema _schema;
        public XPathOrderComparer(XmlSchema xsd, string rootElementName)
        {
            this._schema = xsd;
            _orderedPaths = new List<string>();
            XmlSchemaSet set = new XmlSchemaSet();
            set.Add(xsd);
            set.Compile();
            _elements = new Dictionary<string, XmlSchemaObject>();
            ExtractBaseElements(xsd, _elements);
            if (_elements.ContainsKey(rootElementName))
            {
                Dispatch(_elements[rootElementName], "");
            }
            else
            {
                throw new Exception(string.Format("The supplied XSD does not define an element {0} to use as a document root element.", rootElementName));
            }
        }
        public XPathOrderComparer(string xsdLocation, string rootElementName)
            : this(XmlSchema.Read(new StreamReader(xsdLocation), null), rootElementName)
        {
        }

        public XmlSchema XmlSchema
        {
            get
            {
                return this._schema;
            }
        }
        /// <summary>
        /// Returns true if the provided XPath expression defines a valid path in an XML document implementing
        /// the schema used for this comparer.
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public bool IsValid(Expression exp)
        {
            return this._elements.ContainsKey(exp.BaseXPath);
        }

        private void RecurseXsdOrder(XmlSchemaElement xsdElement, string path)
        {
            if (!xsdElement.RefName.IsEmpty)
            {
                xsdElement = (XmlSchemaElement)_elements[xsdElement.RefName.Name];
            }
            path = path + "/" + xsdElement.Name;
            _orderedPaths.Add(path);
            Dispatch(xsdElement.ElementSchemaType, path);
        }
        private void RecurseXsdOrder(XmlSchemaComplexType cmpxType, string path)
        {
            RecurseXsdOrder(cmpxType.Attributes, path);
            if (cmpxType.ContentModel != null && cmpxType.ContentModel.Content != null)
            {
                Dispatch(cmpxType.ContentModel.Content, path);
            }
            if (cmpxType.Particle != null)
            {
                Dispatch(cmpxType.Particle, path);
            }
        }
        private void RecurseXsdOrder(XmlSchemaObjectCollection collection, string path)
        {
            foreach (XmlSchemaObject obj in collection)
            {
                Dispatch(obj, path);
            }
        }
        private void RecurseXsdOrder(XmlSchemaAttribute attr, string path)
        {
            if (!attr.RefName.IsEmpty)
            {
                _orderedPaths.Add(path + "/@" + attr.RefName.Name);
            }
            else
            {
                _orderedPaths.Add(path + "/@" + attr.Name);
            }
        }

        private void Dispatch(XmlSchemaObject obj, string path)
        {
            if (obj is XmlSchemaComplexType)
            {
                RecurseXsdOrder((XmlSchemaComplexType)obj, path);
            }
            else if (obj is XmlSchemaSequence)
            {
                RecurseXsdOrder(((XmlSchemaSequence)obj).Items, path);
            }
            else if (obj is XmlSchemaElement)
            {
                RecurseXsdOrder((XmlSchemaElement)obj, path);
            }
            else if (obj is XmlSchemaSimpleContentExtension)
            {
                RecurseXsdOrder(((XmlSchemaSimpleContentExtension)obj).Attributes, path);
            }
            else if (obj is XmlSchemaAttribute)
            {
                RecurseXsdOrder((XmlSchemaAttribute)obj, path);
            }
            else if (obj is XmlSchemaChoice)
            {
                RecurseXsdOrder(((XmlSchemaChoice)obj).Items, path);
            }
            else if (obj is XmlSchemaAttributeGroupRef)
            {
                RecurseXsdOrder(((XmlSchemaAttributeGroup)_elements[((XmlSchemaAttributeGroupRef)obj).RefName.Name]).Attributes, path);
            }
        }
        private void ExtractBaseElements(XmlSchema xsd, Dictionary<string, XmlSchemaObject> schemaElements)
        {
            foreach (XmlSchemaObject obj in xsd.Items)
            {
                if (obj is XmlSchemaComplexType)
                {
                    schemaElements.Add(((XmlSchemaComplexType)obj).Name, obj);
                }
                else if (obj is XmlSchemaElement)
                {
                    schemaElements.Add(((XmlSchemaElement)obj).Name, obj);
                }
                else if (obj is XmlSchemaAttribute)
                {
                    schemaElements.Add(((XmlSchemaAttribute)obj).Name, obj);
                }
                else if (obj is XmlSchemaAttributeGroup)
                {
                    schemaElements.Add(((XmlSchemaAttributeGroup)obj).Name, obj);
                }
            }
        }
        public int Compare(Expression baseExp1, Expression baseExp2)
        {
            int result = 0;
            int branchIndex = baseExp1.FindCommonBranchIndex(baseExp2);
            baseExp1 = (Expression)baseExp1.Clone();
            baseExp2 = (Expression)baseExp2.Clone();
            Expression exp1 = baseExp1.SubExpression(branchIndex + 1);
            Expression exp2 = baseExp2.SubExpression(branchIndex + 1);
            exp1.SetNamespacePrefix(null);
            exp2.SetNamespacePrefix(null);
            if (exp1.IsAttribute ^ exp2.IsAttribute)
            {
                if (exp1.IsAttribute)
                    result = -1;
                else
                    result = 1;
            }
            else if (!exp1.IsAttribute && !exp2.IsAttribute)
            {
                string base1, base2;
                base1 = exp1.BaseXPath;
                base2 = exp2.BaseXPath;
                if (base1.Equals(base2))
                {
                    result = exp1.ToString(true).CompareTo(exp2.ToString(true));
                }
                else if (_orderedPaths.Contains(base1) && _orderedPaths.Contains(base2))
                {
                    result = _orderedPaths.IndexOf(base1) - _orderedPaths.IndexOf(base2);
                }
            }
            return result;
        }
        public override int Compare(string path1, string path2)
        {
            Expression exp1, exp2;
            exp1 = new Expression(path1);
            exp2 = new Expression(path2);
            return this.Compare(exp1, exp2);
        }
    }
}
