using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.ImportExport;
using Epsitec.Cresus.DataLayer.Infrastructure;
using Epsitec.Cresus.DataLayer.UnitTests.Entities;
using Epsitec.Cresus.DataLayer.UnitTests.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

using System.Linq;

using System.Xml.Linq;


namespace Epsitec.Cresus.DataLayer.UnitTests.ImportExport
{


	[TestClass]
	public class UnitTestXmlEntitySerializer
	{


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();
		}


		[ClassCleanup]
		public static void ClassCleanup()
		{
			DatabaseHelper.DisconnectFromDatabase ();
		}


		[TestInitialize]
		public void TestInitialize()
		{
			DatabaseHelper.CreateAndConnectToDatabase ();

			Assert.IsTrue (DatabaseHelper.DbInfrastructure.IsConnectionOpen);

			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					DatabaseCreator2.PupulateDatabase (dataContext);
				}
			}
		}


		[TestMethod]
		public void TestAllExported()
		{
			XDocument xDocument;

			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					HashSet<AbstractEntity> entities = new HashSet<AbstractEntity> ();

					entities.Add (dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001))));
					entities.Add (dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1000000001))));
					entities.Add (dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1000000002))));
					entities.Add (dataContext.ResolveEntity<UriSchemeEntity> (new DbKey (new DbId (1000000001))));
					entities.Add (dataContext.ResolveEntity<PersonGenderEntity> (new DbKey (new DbId (1000000001))));
					entities.Add (dataContext.ResolveEntity<LanguageEntity> (new DbKey (new DbId (1000000001))));

					xDocument = XmlEntitySerializer.Serialize (dataContext, entities);
				}
			}

			DatabaseHelper.CreateAndConnectToDatabase ();
			
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					DatabaseCreator2.RegisterSchema (dataContext);
				}

				XmlEntitySerializer.Deserialize (dataInfrastructure, xDocument);

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					NaturalPersonEntity alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));

					Assert.IsTrue (DatabaseCreator2.CheckAlfred (alfred));			
				}
			}
		}


		[TestMethod]
		public void TestNotAllExported()
		{
			XDocument xDocument;

			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					HashSet<AbstractEntity> entities = new HashSet<AbstractEntity> ();

					UriContactEntity uriContact = dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1000000004)));
					
					entities.Add (uriContact);

					xDocument = XmlEntitySerializer.Serialize (dataContext, entities);

					dataContext.DeleteEntity (uriContact);

					dataContext.SaveChanges ();
				}

				XmlEntitySerializer.Deserialize (dataInfrastructure, xDocument);

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					UriContactEntity uriContact = dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1000000005)));
					UriSchemeEntity uriScheme = dataContext.ResolveEntity<UriSchemeEntity> (new DbKey (new DbId (1000000001)));

					Assert.AreEqual (uriScheme, uriContact.UriScheme);
					Assert.AreEqual ("nobody@nowhere.com", uriContact.Uri);
				}
			}
		}


		[TestMethod]
		public void TestInwardBrokenRelations()
		{
			XDocument xDocument;

			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					HashSet<AbstractEntity> entities = new HashSet<AbstractEntity> ();

					UriSchemeEntity uriScheme = dataContext.ResolveEntity<UriSchemeEntity> (new DbKey (new DbId (1000000001)));

					entities.Add (uriScheme);

					xDocument = XmlEntitySerializer.Serialize (dataContext, entities);

					dataContext.DeleteEntity (uriScheme);

					dataContext.SaveChanges ();
				}

				XmlEntitySerializer.Deserialize (dataInfrastructure, xDocument);

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					UriSchemeEntity uriScheme = dataContext.ResolveEntity<UriSchemeEntity> (new DbKey (new DbId (1000000002)));

					Assert.AreEqual ("mailto:", uriScheme.Code);
					Assert.AreEqual ("email", uriScheme.Name);

					List<UriContactEntity> uriContacts = new List<UriContactEntity> ()
					{
						dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1000000001))),
						dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1000000002))),
						dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1000000003))),
						dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1000000004))),
					};

					foreach (UriContactEntity uriContact in uriContacts)
					{
						Assert.IsNull (uriContact.UriScheme);
					}
				}
			}
		}


		[TestMethod]
		public void TestOutwardBrokenReference()
		{
			XDocument xDocument;

			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					HashSet<AbstractEntity> entities = new HashSet<AbstractEntity> ();

					UriContactEntity uriContact = dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1000000004)));
					UriSchemeEntity uriScheme = dataContext.ResolveEntity<UriSchemeEntity> (new DbKey (new DbId (1000000001)));

					entities.Add (uriContact);

					xDocument = XmlEntitySerializer.Serialize (dataContext, entities);

					dataContext.DeleteEntity (uriScheme);

					dataContext.SaveChanges ();
				}

				XmlEntitySerializer.Deserialize (dataInfrastructure, xDocument);

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					UriContactEntity uriContact = dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1000000005)));

					Assert.IsNull (uriContact.UriScheme);
				}
			}
		}


		[TestMethod]
		public void TestOutwardBrokenCollection()
		{
			XDocument xDocument;

			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					HashSet<AbstractEntity> entities = new HashSet<AbstractEntity> ();

					NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
					UriContactEntity uriContact = dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1000000001)));

					entities.Add (person);

					xDocument = XmlEntitySerializer.Serialize (dataContext, entities);

					dataContext.DeleteEntity (uriContact);

					dataContext.SaveChanges ();
				}

				XmlEntitySerializer.Deserialize (dataInfrastructure, xDocument);

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000004)));
					UriContactEntity uriContact = dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1000000002)));

					Assert.AreEqual (1, person.Contacts.Count);
					Assert.AreEqual (uriContact, person.Contacts[0]);
				}
			}
		}


	}


}
