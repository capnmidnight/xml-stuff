using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XmlEdit;

namespace Test.XmlEdit
{
    [TestClass]
    public class XmlDocumentAlterer__test
    {
        [TestMethod]
        public void DocumentPropertyReturnsAValue()
        {
            XmlDocumentAlterer docAlter = new XmlDocumentAlterer();
            Assert.IsNotNull(docAlter.Document);
        }

        [TestMethod]
        public void DocumentPropertyReturnsProvidedDocument()
        {
            XmlDocument doc = new XmlDocument();
            XmlDocumentAlterer docAlter = new XmlDocumentAlterer(doc);
            Assert.AreSame(doc, docAlter.Document);
        }
        [TestMethod]
        public void FillInCreatesNodes()
        {
            XmlDocumentAlterer docAlter = new XmlDocumentAlterer();
            Expression xpath = new Expression("/root");
            docAlter.FillIn(xpath);
            Assert.AreEqual("root", docAlter.Document.DocumentElement.Name);
        }
        [TestMethod]
        public void FillInWorksSeveralLevelsDeep()
        {
            XmlDocumentAlterer docAlter = new XmlDocumentAlterer();
            Expression xpath = new Expression("/root/a/b/c/d");
            docAlter.FillIn(xpath);
            Assert.AreEqual("root", docAlter.Document.DocumentElement.Name);
            Assert.AreEqual("a", docAlter.Document.DocumentElement.FirstChild.Name);
            Assert.AreEqual("b", docAlter.Document.DocumentElement.FirstChild.FirstChild.Name);
            Assert.AreEqual("c", docAlter.Document.DocumentElement.FirstChild.FirstChild.FirstChild.Name);
            Assert.AreEqual("d", docAlter.Document.DocumentElement.FirstChild.FirstChild.FirstChild.FirstChild.Name);
        }
        [TestMethod]
        public void FillInWorksForAttributes()
        {
            XmlDocumentAlterer docAlter = new XmlDocumentAlterer();
            Expression xpath = new Expression("/root/@attr");
            docAlter.FillIn(xpath);
            Assert.AreEqual("root", docAlter.Document.DocumentElement.Name);
            Assert.AreEqual("attr", docAlter.Document.DocumentElement.Attributes[0].Name);
        }
        [TestMethod]
        public void FillInAssumesPredicates()
        {
            XmlDocumentAlterer docAlter = new XmlDocumentAlterer();
            Expression xpath = new Expression("/root/a[@id=3]/b");
            docAlter.FillIn(xpath);
            Assert.AreEqual("root", docAlter.Document.DocumentElement.Name);
            Assert.AreEqual("a", docAlter.Document.DocumentElement.FirstChild.Name);
            Assert.AreEqual("id", docAlter.Document.DocumentElement.FirstChild.Attributes[0].Name);
            Assert.AreEqual("3", docAlter.Document.DocumentElement.FirstChild.Attributes[0].Value);
            Assert.AreEqual("b", docAlter.Document.DocumentElement.FirstChild.FirstChild.Name);
        }
        [TestMethod]
        public void FillInObservesElementIndexing()
        {
            XmlDocumentAlterer docAlter = new XmlDocumentAlterer();
            Expression xpath1 = new Expression("/root/a[1]/b");
            Expression xpath2 = new Expression("/root/a[2]/c");
            docAlter.FillIn(xpath1);
            docAlter.FillIn(xpath2);
            Assert.AreEqual("root", docAlter.Document.DocumentElement.Name);
            Assert.AreEqual(2, docAlter.Document.DocumentElement.ChildNodes.Count);
            Assert.AreEqual("a", docAlter.Document.DocumentElement.ChildNodes[0].Name);
            Assert.AreEqual("b", docAlter.Document.DocumentElement.ChildNodes[0].FirstChild.Name);
            Assert.AreEqual("a", docAlter.Document.DocumentElement.ChildNodes[1].Name);
            Assert.AreEqual("c", docAlter.Document.DocumentElement.ChildNodes[1].FirstChild.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void FillInInvalidNodeFails()
        {
            XmlDocumentAlterer docAlterer = new XmlDocumentAlterer();
            Expression xpath1 = new Expression("/root/a/@b/c");
            docAlterer.FillIn(xpath1);
        }

        [TestMethod]
        public void SelectingNonexistantNodeCreatesIt()
        {
            XmlDocumentAlterer docAlter = new XmlDocumentAlterer();
            XmlNode node = docAlter.SelectNodes(new Expression("/root"))[0];
            Assert.AreEqual("root", node.Name);
            Assert.AreEqual(0, node.ChildNodes.Count);
        }

        [TestMethod]
        public void SelectingNonexistantNodeCreatesIt2()
        {
            XmlDocumentAlterer docAlter = new XmlDocumentAlterer();
            XmlNode node = docAlter.SelectNodes(new Expression("/root/a[@id='3']/b/c[@date='07-07-2007']/d"))[0];
            Assert.AreEqual("d", node.Name);
            Assert.AreEqual(0, node.ChildNodes.Count);
        }

        [TestMethod]
        public void AddingANamespaceAddsItToTheDocument()
        {
            XmlDocumentAlterer docAlterer = new XmlDocumentAlterer();
            docAlterer.AddNamespace("ns0", "http://www.ns0.com");
            docAlterer.FillIn(new Expression("/root/a/b/c"));
            docAlterer.FillIn(new Expression("/root/a/b/d"));
            docAlterer.FillIn(new Expression("/root/a/b/e"));
            docAlterer.FillIn(new Expression("/root/a/f/g"));
            docAlterer.FillIn(new Expression("/root/a/f/h"));
            docAlterer.FillIn(new Expression("/root/a/f/i"));
        }

        [TestMethod]
        public void SetValue_1()
        {
            XmlDocumentAlterer docAlter = new XmlDocumentAlterer();
            Expression xpath = new Expression("/root/a");
            string value = "sup";
            docAlter.SetValue(xpath, value);
            XmlNode node = docAlter.SelectNodes(xpath)[0];
            Assert.AreEqual(value, node.InnerText);
        }

        [TestMethod]
        public void SetValue_2()
        {
            XmlDocumentAlterer docAlter = new XmlDocumentAlterer();
            Expression xpath = new Expression("/root/a[@id='1']/b");
            string value = "sup";
            docAlter.SetValue(xpath, value);
            XmlNode node = docAlter.SelectNodes(xpath)[0];
            Assert.AreEqual(value, node.InnerText);
        }

        [TestMethod]
        public void GetValue_null()
        {
            XmlDocumentAlterer docAlter = new XmlDocumentAlterer();
            Assert.IsNull(docAlter.GetValue(new Expression("/root/a/b"), FailureReaction.Ignore));
        }

        [TestMethod]
        public void Set_GetValue()
        {
            XmlDocumentAlterer docAlter = new XmlDocumentAlterer();
            Expression xpath = new Expression("/root/a");
            string value = "gah";
            docAlter.SetValue(xpath, value);
            Assert.AreEqual(value, docAlter.GetValue(xpath, FailureReaction.Ignore));
        }

        [TestMethod]
        public void EditLoadedDocument()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(
@"<root>
    <a id='1'>
        <b id='9'>asdf</b>
    </a>
    <a id='2'>
        <b id='8'>qwer</b>
    </a>
    <a id='3'>
        <b id='4'>zxcv</b>
    </a>
</root>"
                );
            XmlDocumentAlterer docAlter = new XmlDocumentAlterer(doc);
            docAlter.SetValue(new Expression("/root/a[@id=1]/b/@t"), "6");
            docAlter.SetValue(new Expression("/root/a[2]/@id"), "somtig");
            docAlter.SetValue(new Expression("/root/a[@id='3']/b[@id=4]/c"), "workit");
            Assert.AreEqual("6", docAlter.GetValue(new Expression("/root/a[1]/b/@t"), FailureReaction.Ignore));
            Assert.AreEqual("somtig", docAlter.GetValue(new Expression("/root/a[2]/@id"), FailureReaction.Ignore));
            Assert.AreEqual("workit", docAlter.GetValue(new Expression("/root/a[3]/b/c"), FailureReaction.Ignore));
        }
        [TestMethod]
        public void EditLoadedDocumentWithNamespace()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(
@"<root xmlns=""http://www.icsm.org/icsmxml"">
    <a id='1'>
        <b id='9'>asdf</b>
    </a>
    <a id='2'>
        <b id='8'>qwer</b>
    </a>
    <a id='3'>
        <b id='4'>zxcv</b>
    </a>
</root>"
                );
            XmlDocumentAlterer docAlter = new XmlDocumentAlterer(doc);
            docAlter.AddNamespace("ns0", "http://www.icsm.org/icsmxml");
            docAlter.SetValue(new Expression("/root/a[@id=1]/b/@t", "ns0"), "6");
            docAlter.SetValue(new Expression("/root/a[2]/@id", "ns0"), "somtig");
            docAlter.SetValue(new Expression("/root/a[@id='3']/b[@id=4]/c", "ns0"), "workit");
            Assert.AreEqual("6", docAlter.GetValue(new Expression("/root/a[1]/b/@t", "ns0"), FailureReaction.Ignore));
            Assert.AreEqual("somtig", docAlter.GetValue(new Expression("/root/a[2]/@id", "ns0"), FailureReaction.Ignore));
            Assert.AreEqual("workit", docAlter.GetValue(new Expression("/root/a[3]/b/c", "ns0"), FailureReaction.Ignore));
        }

        [TestMethod]
        public void EditLoadedDocumentWithNamespaceAndPrefix()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(
@"<ns0:root xmlns:ns0=""http://www.icsm.org/icsmxml"">
    <ns0:a id='1'>
        <ns0:b id='9'>asdf</ns0:b>
    </ns0:a>
    <ns0:a id='2'>
        <ns0:b id='8'>qwer</ns0:b>
    </ns0:a>
    <ns0:a id='3'>
        <ns0:b id='4'>zxcv</ns0:b>
    </ns0:a>
</ns0:root>"
                );
            XmlDocumentAlterer docAlter = new XmlDocumentAlterer(doc);
            docAlter.AddNamespace("ns0", "http://www.icsm.org/icsmxml");
            docAlter.SetValue(new Expression("/root/a[@id=1]/b/@t", "ns0"), "6");
            docAlter.SetValue(new Expression("/root/a[2]/@id", "ns0"), "somtig");
            docAlter.SetValue(new Expression("/root/a[@id='3']/b[@id=4]/c", "ns0"), "workit");
            Assert.AreEqual("6", docAlter.GetValue(new Expression("/root/a[1]/b/@t", "ns0"), FailureReaction.Ignore));
            Assert.AreEqual("somtig", docAlter.GetValue(new Expression("/root/a[2]/@id", "ns0"), FailureReaction.Ignore));
            Assert.AreEqual("workit", docAlter.GetValue(new Expression("/root/a[3]/b/c", "ns0"), FailureReaction.Ignore));
        }

        [TestMethod]
        public void EditDocument_2()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(
@"<?xml version=""1.0"" encoding=""utf-8""?>
<ICSMXML  xmlns:xsd=""http://www.w3.org/2001/XMLSchema""
		 xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
		 xmlns=""http://www.icsm.com/icsmxml"">
    <Request deploymentMode=""test"">
        <OrderRequest>
            <OrderRequestHeader orderDate=""2007-05-24""
								dueDate=""2007-05-27""
								orderID=""1203074043""
								type=""new""
								orderType=""regular""
								shippingType=""header""
								billCustomer=""no"">
                <CustomerIdentification>
					<CustomerInfo name=""AccountNumber"">0</CustomerInfo>
					<CustomerInfo name=""CustomerPONumber"">1</CustomerInfo>
					<CorporateInfo name=""AccountNumber"">254490</CorporateInfo>
					<CorporateInfo name=""PurchaseOrder""></CorporateInfo>
					<CorporateInfo name=""StoreNumber"">9999</CorporateInfo>
				</CustomerIdentification>
            </OrderRequestHeader>
        </OrderRequest>
    </Request>
</ICSMXML>"
                );
            XmlDocumentAlterer docAlter = new XmlDocumentAlterer(doc);
            docAlter.AddNamespace("ns0", "http://www.icsm.com/icsmxml");
            Expression xpath = new Expression("/ICSMXML/Request/OrderRequest/OrderRequestHeader/CustomerIdentification/CorporateInfor[@name=SourceCode]", "ns0");
            docAlter.SetValue(xpath, "XYZ");
            Assert.AreEqual("XYZ", docAlter.GetValue(xpath, FailureReaction.Ignore));
        }

        [TestMethod]
        public void SetValueFromStringExpression()
        {
            XmlDocumentAlterer docAlter = new XmlDocumentAlterer();
            docAlter.SetValue("/root/a", "abc");
            Assert.AreEqual("abc", docAlter.GetValue(new Expression("/root/a"), FailureReaction.Ignore));
        }

        [TestMethod]
        public void GetValueFromStingExpression()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<root><a>asdf</a></root>");
            XmlDocumentAlterer docAlter = new XmlDocumentAlterer(doc);
            Assert.AreEqual("asdf", docAlter.GetValue("/root/a", FailureReaction.Ignore));
        }

        [TestMethod]
        public void ConstructWithSchemaObject()
        {
            XmlSchema schema = new XmlSchema();
            XmlDocumentAlterer docAlter = new XmlDocumentAlterer(schema);
            Assert.AreSame(schema, docAlter.Schema);
        }

        [TestMethod]
        public void RearrangeWithSchema()
        {
            XmlDocument schemaDoc = new XmlDocument();
            schemaDoc.LoadXml(
@"<?xml version=""1.0"" encoding=""utf-8""?>
<xsd:schema xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
    <xsd:element name=""root"">
        <xsd:complexType>
            <xsd:sequence>
                <xsd:element ref=""a""/>
                <xsd:element ref=""b""/>
            </xsd:sequence>
        </xsd:complexType>
    </xsd:element>

    <xsd:element name=""a"">
        <xsd:complexType>
            <xsd:sequence>
                <xsd:element ref=""c""/>
                <xsd:element ref=""d""/>
            </xsd:sequence>
        </xsd:complexType>
    </xsd:element>
    <xsd:element name=""b"" type=""xsd:string""/>
    <xsd:element name=""c"" type=""xsd:string""/>
    <xsd:element name=""d"" type=""xsd:string""/>

</xsd:schema>");
            XmlSchema schema = null;
            using (MemoryStream memBuf = new MemoryStream())
            {
                schemaDoc.Save(memBuf);
                memBuf.Flush();
                memBuf.Position = 0;
                schema = XmlSchema.Read(memBuf, null);
                memBuf.Close();
            }
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(
@"<?xml version=""1.0"" encoding=""utf-8""?>
<root xmlns=""http://www.what.com"">
    <b>test1</b>
    <a>
        <d>test2</d>
        <c>test3</c>
    </a>
</root>");

            XmlDocumentAlterer docAlter = new XmlDocumentAlterer(doc, schema);
            docAlter.AddNamespace("ns0", "http://www.what.com");
            docAlter.SortElements();
            Assert.AreEqual("a", doc.DocumentElement.ChildNodes[0].Name);
            Assert.AreEqual("c", doc.DocumentElement.ChildNodes[0].ChildNodes[0].Name);
            Assert.AreEqual("test3", doc.DocumentElement.ChildNodes[0].ChildNodes[0].InnerText);
            Assert.AreEqual("d", doc.DocumentElement.ChildNodes[0].ChildNodes[1].Name);
            Assert.AreEqual("test2", doc.DocumentElement.ChildNodes[0].ChildNodes[1].InnerText);
            Assert.AreEqual("b", doc.DocumentElement.ChildNodes[1].Name);
            Assert.AreEqual("test1", doc.DocumentElement.ChildNodes[1].InnerText);
        }

        [TestMethod]
        public void SetValueInMultipleNodes()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<root><a></a><a></a></root>");
            XmlDocumentAlterer docAlter = new XmlDocumentAlterer(doc);
            Expression xpath = new Expression("/root/a");
            docAlter.SetValue(xpath, "x");
            XmlNodeList nodes = doc.SelectNodes(xpath.ToString());
            Assert.AreEqual(2, nodes.Count);
            Assert.AreEqual("x", nodes[0].InnerText, "first node");
            Assert.AreEqual("x", nodes[1].InnerText, "second node");
        }

        [TestMethod]
        public void SetValueInMultipleNodesWithFillIn()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<root><a></a><a></a></root>");
            XmlDocumentAlterer docAlter = new XmlDocumentAlterer(doc);
            Expression xpath = new Expression("/root/a/b");
            docAlter.SetValue(xpath, "x");
            XmlNodeList nodes = doc.SelectNodes(xpath.ToString());
            Assert.AreEqual(2, nodes.Count);
            Assert.AreEqual("x", nodes[0].InnerText, "first node");
            Assert.AreEqual("x", nodes[1].InnerText, "second node");
        }

        [TestMethod]
        public void SetValueInMultipleNodesWithFillInDeep()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<root><a></a><a></a></root>");
            XmlDocumentAlterer docAlter = new XmlDocumentAlterer(doc);
            Expression xpath = new Expression("/root/a/b/c/e/@d");
            docAlter.SetValue(xpath, "x");
            XmlNodeList nodes = doc.SelectNodes(xpath.ToString());
            Assert.AreEqual(2, nodes.Count);
            Assert.AreEqual("x", nodes[0].InnerText, "first node");
            Assert.AreEqual("x", nodes[1].InnerText, "second node");
        }
        [TestMethod]
        public void SetValueInMultipleNodesWithMultipleFillIn()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<root><a><b></b><b></b></a><a><b></b></a></root>");
            XmlDocumentAlterer docAlter = new XmlDocumentAlterer(doc);
            Expression xpath = new Expression("/root/a/b/c/e/@d");
            docAlter.SetValue(xpath, "x");
            XmlNodeList nodes = doc.SelectNodes(xpath.ToString());
            Assert.AreEqual(3, nodes.Count);
            Assert.AreEqual("x", nodes[0].InnerText, "first node");
            Assert.AreEqual("x", nodes[1].InnerText, "second node");
            Assert.AreEqual("x", nodes[2].InnerText, "second node");
        }

        [TestMethod]
        public void IsNotValidBeforeSorting()
        {
            XmlDocument schemaDoc = new XmlDocument();
            schemaDoc.LoadXml(
@"<?xml version=""1.0"" encoding=""utf-8""?>
<xsd:schema xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns=""http://www.what.com"" targetNamespace=""http://www.what.com"">
    <xsd:element name=""root"">
        <xsd:complexType>
            <xsd:sequence>
                <xsd:element ref=""a""/>
                <xsd:element ref=""b""/>
            </xsd:sequence>
        </xsd:complexType>
    </xsd:element>

    <xsd:element name=""a"">
        <xsd:complexType>
            <xsd:sequence>
                <xsd:element ref=""c""/>
                <xsd:element ref=""d""/>
            </xsd:sequence>
        </xsd:complexType>
    </xsd:element>
    <xsd:element name=""b"" type=""xsd:string""/>
    <xsd:element name=""c"" type=""xsd:string""/>
    <xsd:element name=""d"" type=""xsd:string""/>

</xsd:schema>");
            XmlSchema schema = null;
            using (MemoryStream memBuf = new MemoryStream())
            {
                schemaDoc.Save(memBuf);
                memBuf.Flush();
                memBuf.Position = 0;
                schema = XmlSchema.Read(memBuf, null);
                memBuf.Close();
            }
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(
@"<?xml version=""1.0"" encoding=""utf-8""?>
<root xmlns=""http://www.what.com"">
    <b>test1</b>
    <a>
        <d>test2</d>
        <c>test3</c>
    </a>
</root>");

            XmlDocumentAlterer docAlter = new XmlDocumentAlterer(doc, schema);
            docAlter.AddNamespace("ns0", "http://www.what.com");
            Assert.IsFalse(docAlter.IsValid);
        }
        [TestMethod]
        public void IsValidAfterSorting()
        {
            XmlDocument schemaDoc = new XmlDocument();
            schemaDoc.LoadXml(
@"<?xml version=""1.0"" encoding=""utf-8""?>
<xsd:schema xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns=""http://www.what.com"" targetNamespace=""http://www.what.com"">
    <xsd:element name=""root"">
        <xsd:complexType>
            <xsd:sequence>
                <xsd:element ref=""a""/>
                <xsd:element ref=""b""/>
            </xsd:sequence>
        </xsd:complexType>
    </xsd:element>

    <xsd:element name=""a"">
        <xsd:complexType>
            <xsd:sequence>
                <xsd:element ref=""c""/>
                <xsd:element ref=""d""/>
            </xsd:sequence>
        </xsd:complexType>
    </xsd:element>
    <xsd:element name=""b"" type=""xsd:string""/>
    <xsd:element name=""c"" type=""xsd:string""/>
    <xsd:element name=""d"" type=""xsd:string""/>

</xsd:schema>");
            XmlSchema schema = null;
            using (MemoryStream memBuf = new MemoryStream())
            {
                schemaDoc.Save(memBuf);
                memBuf.Flush();
                memBuf.Position = 0;
                schema = XmlSchema.Read(memBuf, null);
                memBuf.Close();
            }
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(
@"<?xml version=""1.0"" encoding=""utf-8""?>
<root xmlns=""http://www.what.com"">
    <b>test1</b>
    <a>
        <d>test2</d>
        <c>test3</c>
    </a>
</root>");

            XmlDocumentAlterer docAlter = new XmlDocumentAlterer(doc, schema);
            docAlter.AddNamespace("ns0", "http://www.what.com");
            docAlter.SortElements();
            Assert.IsTrue(docAlter.IsValid, "not valid");
        }

        [TestMethod]
        public void SelectParentNode1()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(
@"<root>
    <a type='3'/>
    <a>
        <b>d</b>
    </a>
    <a>
        <c type='z_val'/>
    </a>
    <a>
        <b>x</b>
        <c>
            <d type='x_val'/>
        </c>
    </a>
    <a>
        <b>y</b>
        <c>
            <d type='y_val'/>
        </c>
    </a>
</root>");
            XmlDocumentAlterer docAlter = new XmlDocumentAlterer(doc);
            Expression xpath = new Expression("//c[2]/../b");
            Assert.AreEqual("x", docAlter.GetValue(xpath, FailureReaction.Ignore));
        }

        [TestMethod]
        public void SelectParentNode2()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(
@"<root>
    <a type='3'/>
    <a>
        <b>d</b>
    </a>
    <a>
        <c type='z_val'/>
    </a>
    <a>
        <b>x</b>
        <c>
            <d type='x_val'/>
        </c>
    </a>
    <a>
        <b>y</b>
        <c>
            <d type='y_val'/>
        </c>
    </a>
</root>");
            XmlDocumentAlterer docAlter = new XmlDocumentAlterer(doc);
            Expression exp = new Expression("/root//d[2]/@type");
            Expression xpath = new Expression("/root//d[2]/parent::*/parent::*/b");
            Assert.AreEqual("y_val", docAlter.GetValue(exp, FailureReaction.Ignore));
            Assert.AreEqual("y", docAlter.GetValue(xpath, FailureReaction.Ignore));
        }

        [TestMethod]
        public void CreateElementWithRelativePath()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(@"<root>
    <a type='a_1'>
        <b>zxcv</b>
        <b>yuiop</b>
    </a>
    <a type='a_2'>
        <b>hjkl</b>
        <b>qwert</b>
    </a>
</root>");
            Assert.AreEqual(4, doc.SelectNodes("/root//b").Count);
            XmlDocumentAlterer alter = new XmlDocumentAlterer(doc);
            Assert.AreEqual(4, alter.CountNodes(new Expression("/root//b")));
            Assert.AreEqual("hjkl", alter.GetValue("/root//b[3]", FailureReaction.Ignore));

            Expression xpath = new Expression("/root//b[3]/../c");
            alter.SetValue(xpath, "qqq");

            Expression xpath2 = new Expression("/root/a[@type='a_2']/c");
            Assert.AreEqual("qqq", alter.GetValue(xpath2, FailureReaction.Ignore));
        }
        [TestMethod]
        public void RetrieveNullWithIgnore()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<root></root>");
            XmlDocumentAlterer docAlter = new XmlDocumentAlterer(doc);
            Assert.IsNull(docAlter.GetValue("/root/a", FailureReaction.Ignore));
        }
        [TestMethod]
        public void RetrieveEmptyWithIgnore()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<root><a/></root>");
            XmlDocumentAlterer docAlter = new XmlDocumentAlterer(doc);
            Assert.AreEqual(0, docAlter.GetValue("/root/a", FailureReaction.Ignore).Length);
        }
        [TestMethod]
        public void RetrieveValueWithIgnore()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<root><a>asdf</a></root>");
            XmlDocumentAlterer docAlter = new XmlDocumentAlterer(doc);
            Assert.AreEqual("asdf", docAlter.GetValue("/root/a", FailureReaction.Ignore));
        }
        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void RetrieveNullWithNull()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<root></root>");
            XmlDocumentAlterer docAlter = new XmlDocumentAlterer(doc);
            Assert.IsNull(docAlter.GetValue("/root/a", FailureReaction.GenerateErrorOnNull));
        }
        [TestMethod]
        public void RetrieveEmptyWithNull()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<root><a/></root>");
            XmlDocumentAlterer docAlter = new XmlDocumentAlterer(doc);
            Assert.AreEqual(0, docAlter.GetValue("/root/a", FailureReaction.GenerateErrorOnNull).Length);
        }
        [TestMethod]
        public void RetrieveValueWithNull()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<root><a>asdf</a></root>");
            XmlDocumentAlterer docAlter = new XmlDocumentAlterer(doc);
            Assert.AreEqual("asdf", docAlter.GetValue("/root/a", FailureReaction.GenerateErrorOnNull));
        }
        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void RetrieveNullWithEmpty()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<root></root>");
            XmlDocumentAlterer docAlter = new XmlDocumentAlterer(doc);
            Assert.IsNull(docAlter.GetValue("/root/a", FailureReaction.GenerateErrorOnEmpty));
        }
        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void RetrieveEmptyWithEmpty()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<root><a/></root>");
            XmlDocumentAlterer docAlter = new XmlDocumentAlterer(doc);
            Assert.AreEqual(0, docAlter.GetValue("/root/a", FailureReaction.GenerateErrorOnEmpty).Length);
        }
        [TestMethod]
        public void RetrieveValueWithEmpty()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<root><a>asdf</a></root>");
            XmlDocumentAlterer docAlter = new XmlDocumentAlterer(doc);
            Assert.AreEqual("asdf", docAlter.GetValue("/root/a", FailureReaction.GenerateErrorOnEmpty));
        }
    }


}
