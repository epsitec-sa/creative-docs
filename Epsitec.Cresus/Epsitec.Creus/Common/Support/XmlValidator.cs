using Epsitec.Common.Support.Extensions;

using System;

using System.Collections.Generic;

using System.IO;

using System.Linq;

using System.Reflection;

using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;


namespace Epsitec.Common.Support
{


	public sealed class XmlValidator
	{
		

		private XmlValidator(IEnumerable<string> xsdContents)
		{
			this.xmlSchemaSet = XmlValidator.BuildXmlSchemaSet (xsdContents);
		}


		public void Validate(XmlDocument document)
		{
			var rootNamespace = document.DocumentElement.NamespaceURI;

			this.CheckRootNamespace (rootNamespace);

			XmlReaderSettings settings = new XmlReaderSettings ()
			{
				ValidationType = ValidationType.Schema
			};

			settings.Schemas.Add (this.xmlSchemaSet);

			using (XmlNodeReader xmlNodeReader = new XmlNodeReader (document))
			{
				using (XmlReader r = XmlReader.Create (xmlNodeReader, settings))
				{
					while (r.Read ())
					{
					}
				}
			}
		}


		public void Validate(XDocument document)
		{
			var rootNamespace = document.Root.Name.Namespace.NamespaceName;

			this.CheckRootNamespace (rootNamespace);

			document.Validate (this.xmlSchemaSet, null);
		}



		private void CheckRootNamespace(string rootNamespace)
		{
			if (!this.xmlSchemaSet.Contains (rootNamespace))
			{
				var message = "Invalid namespace for root element";

				throw new XmlSchemaValidationException (message);
			}
		}


		private static XmlSchemaSet BuildXmlSchemaSet(IEnumerable<string> xsdContents)
		{
			var xmlSchemaSet = new XmlSchemaSet ()
			{
				XmlResolver = null,
			};

			foreach (var xsdContent in xsdContents)
			{
				var stringReader = new StringReader (xsdContent);
				var schema = XmlSchema.Read (stringReader, null);

				xmlSchemaSet.Add (schema);
			}

			xmlSchemaSet.Compile ();

			return xmlSchemaSet;
		}


		public static XmlValidator Create(FileInfo xsdFile)
		{
			var xsdFiles = new List<FileInfo> () { xsdFile };

			return XmlValidator.Create (xsdFiles);
		}


		public static XmlValidator Create(IEnumerable<FileInfo> xsdFiles)
		{
			var xsdContents = from xsdFile in xsdFiles
							  select File.ReadAllText (xsdFile.FullName);

			return new XmlValidator (xsdContents);
		}


		public static XmlValidator Create(Assembly assembly, string xsdResourceName)
		{
			var xsdResourceNames = new List<string> () { xsdResourceName };

			return XmlValidator.Create (assembly, xsdResourceNames);
		}


		public static XmlValidator Create(Assembly assembly, IEnumerable<string> xsdResourceNames)
		{
			var xsdContents = from xsdResourceName in xsdResourceNames
							  select assembly.GetResourceText (xsdResourceName);

			return new XmlValidator (xsdContents);
		}


		private readonly XmlSchemaSet xmlSchemaSet;


	}


}