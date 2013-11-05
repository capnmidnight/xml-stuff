xml-stuff
=========

a handy bit of code for comprehending, authoring, and repairing XML documents


Comparison to LINQ To XML
=========================

As a quick example for XML document authoring, here is a simple example taken from the MSDN article on LINQ to XML
(http://msdn.microsoft.com/en-us/library/bb308960.aspx#xlinqoverview_topic2e)

Linq Version:
```C#
XElement contacts =
   new XElement("contacts",
      new XElement("contact",
         new XElement("name", "Patrick Hines"),
         new XElement("phone", "206-555-0144"),
         new XElement("address",
            new XElement("street1", "123 Main St"),
            new XElement("city", "Mercer Island"),
            new XElement("state", "WA"),
            new XElement("postal", "68042")
         )
      )
   );
```

XmlEdit version:
```C#
XmlDocumentAlterer contacts = new XmlDocumentAlterer();
x.SetValue("/contacts/contact/name", "Patrick Hines");
x.SetValue("/contacts/contact/phone", "206-555-0144");
x.SetValue("/contacts/contact/address/street1", "123 Main St");
x.SetValue("/contacts/contact/address/city", "Mercer Island");
x.SetValue("/contacts/contact/address/state", "WA");
x.SetValue("/contacts/contact/address/postal", "68042");
```

If we wanted to do it as attributes instead, however, it would be a little more amenable to the style of XmlEdit
```C#
XmlDocumentAlterer contacts = new XmlDocumentAlterer();
x.FillIn("/contacts/contact/[@name='Patrick Hines',@phone='206-555-0144']/address/[@street1='123 Main St',@city='Mercer Island',@state='WA',@postal='68042']");
```

As XmlEdit has the power to make your XPath expressions Just Work.
