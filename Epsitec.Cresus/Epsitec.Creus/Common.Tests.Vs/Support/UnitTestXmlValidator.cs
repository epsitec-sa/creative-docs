using Epsitec.Common.Support;

using Epsitec.Common.UnitTesting;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;

using System.Collections.Generic;

using System.IO;

using System.Linq;

using System.Reflection;

using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;


namespace Epsitec.Common.Tests.Vs.Support
{


	[TestClass]
	public class UnitTestXmlValidator
	{


		[TestMethod]
		public void ValidTest1()
		{
			var xmlValidator = this.GetXmlValidatorFromFiles ();
			var xmlDocuments = this.GetValidXmlDocuments ();
			Action<XmlValidator, XmlDocument> checkAction = (v, x) => v.Validate (x);

			this.ExecuteValidTest (xmlValidator, xmlDocuments, checkAction);
		}


		[TestMethod]
		public void ValidTest2()
		{
			var xmlValidator = this.GetXmlValidatorFromResources ();
			var xmlDocuments = this.GetValidXmlDocuments ();
			Action<XmlValidator, XmlDocument> checkAction = (v, x) => v.Validate (x);

			this.ExecuteValidTest (xmlValidator, xmlDocuments, checkAction);
		}


		[TestMethod]
		public void ValidTest3()
		{
			var xmlValidator = this.GetXmlValidatorFromFiles ();
			var xmlDocuments = this.GetValidXDocuments ();
			Action<XmlValidator, XDocument> checkAction = (v, x) => v.Validate (x);

			this.ExecuteValidTest (xmlValidator, xmlDocuments, checkAction);
		}


		[TestMethod]
		public void ValidTest4()
		{
			var xmlValidator = this.GetXmlValidatorFromResources ();
			var xmlDocuments = this.GetValidXDocuments ();
			Action<XmlValidator, XDocument> checkAction = (v, x) => v.Validate (x);

			this.ExecuteValidTest (xmlValidator, xmlDocuments, checkAction);
		}


		private void ExecuteValidTest<T>(XmlValidator xmlValidator, IEnumerable<T> xmlDocuments, Action<XmlValidator, T> checkAction)
		{
			foreach (var xmlDocument in xmlDocuments)
			{
				checkAction (xmlValidator, xmlDocument);
			}
		}


		[TestMethod]
		public void InvalidTest1()
		{
			var xmlValidator = this.GetXmlValidatorFromFiles ();
			var xmlDocuments = this.GetInvalidXmlDocuments ();
			Action<XmlValidator, XmlDocument> checkAction = (v, x) => v.Validate (x);

			this.ExecuteInvalidTest (xmlValidator, xmlDocuments, checkAction);
		}


		[TestMethod]
		public void InvalidTest2()
		{
			var xmlValidator = this.GetXmlValidatorFromResources ();
			var xmlDocuments = this.GetInvalidXmlDocuments ();
			Action<XmlValidator, XmlDocument> checkAction = (v, x) => v.Validate (x);

			this.ExecuteInvalidTest (xmlValidator, xmlDocuments, checkAction);
		}


		[TestMethod]
		public void InvalidTest3()
		{
			var xmlValidator = this.GetXmlValidatorFromFiles ();
			var xmlDocuments = this.GetInvalidXDocuments ();
			Action<XmlValidator, XDocument> checkAction = (v, x) => v.Validate (x);

			this.ExecuteInvalidTest (xmlValidator, xmlDocuments, checkAction);
		}


		[TestMethod]
		public void InvalidTest4()
		{
			var xmlValidator = this.GetXmlValidatorFromResources ();
			var xmlDocuments = this.GetInvalidXDocuments ();
			Action<XmlValidator, XDocument> checkAction = (v, x) => v.Validate (x);

			this.ExecuteInvalidTest (xmlValidator, xmlDocuments, checkAction);
		}


		private void ExecuteInvalidTest<T>(XmlValidator xmlValidator, IEnumerable<T> xmlDocuments, Action<XmlValidator, T> checkAction)
		{
			foreach (var xmlDocument in xmlDocuments)
			{
				ExceptionAssert.Throw<XmlSchemaValidationException>
				(
					() => checkAction (xmlValidator, xmlDocument)
				);
			}
		}


		private XmlValidator GetXmlValidatorFromFiles()
		{
			var xsdFiles = new List<FileInfo> ()
			{
				new FileInfo ("Resources\\SalaryDeclaration.xsd"),
				new FileInfo ("Resources\\SalaryDeclarationContainer.xsd"),
				new FileInfo ("Resources\\SalaryDeclarationServiceTypes.xsd"),
			};

			return XmlValidator.Create (xsdFiles);
		}


		private XmlValidator GetXmlValidatorFromResources()
		{
			var assembly = Assembly.GetExecutingAssembly ();
			var xsdFiles = new List<string> ()
			{
				"Epsitec.Common.Tests.Vs.Resources.SalaryDeclaration.xsd",
				"Epsitec.Common.Tests.Vs.Resources.SalaryDeclarationContainer.xsd",
				"Epsitec.Common.Tests.Vs.Resources.SalaryDeclarationServiceTypes.xsd",
			};

			return XmlValidator.Create (assembly, xsdFiles);
		}


		private IEnumerable<XmlDocument> GetValidXmlDocuments()
		{
			return this.GetXmlDocuments (this.GetValidXmlFiles ());
		}


		private IEnumerable<XmlDocument> GetInvalidXmlDocuments()
		{
			return this.GetXmlDocuments (this.GetInvalidXmlFiles ());
		}


		private IEnumerable<XmlDocument> GetXmlDocuments(IEnumerable<FileInfo> xmlFiles)
		{
			foreach (var xmlFile in xmlFiles)
			{
				var xmlDocument = new XmlDocument ();

				xmlDocument.Load (xmlFile.FullName);

				yield return xmlDocument;
			}
		}


		private IEnumerable<XDocument> GetValidXDocuments()
		{
			return this.GetXDocuments (this.GetValidXmlFiles ());	
		}


		private IEnumerable<XDocument> GetInvalidXDocuments()
		{
			return this.GetXDocuments (this.GetInvalidXmlFiles ());	
		}


		private IEnumerable<XDocument> GetXDocuments(IEnumerable<FileInfo> xmlFiles)
		{
			return xmlFiles.Select (f => XDocument.Load (f.FullName));
		}


		private IEnumerable<FileInfo> GetValidXmlFiles()
		{
			yield return new FileInfo ("Resources\\ValidSalaryDeclarationRequest1.xml");
			yield return new FileInfo ("Resources\\ValidSalaryDeclarationRequest2.xml");
			yield return new FileInfo ("Resources\\ValidSalaryDeclarationRequest3.xml");
		}


		private IEnumerable<FileInfo> GetInvalidXmlFiles()
		{
			yield return new FileInfo ("Resources\\InvalidSalaryDeclarationRequest1.xml");
			yield return new FileInfo ("Resources\\InvalidSalaryDeclarationRequest2.xml");
			yield return new FileInfo ("Resources\\InvalidSalaryDeclarationRequest3.xml");
			yield return new FileInfo ("Resources\\InvalidSalaryDeclarationRequest4.xml");
		}


	}


}
