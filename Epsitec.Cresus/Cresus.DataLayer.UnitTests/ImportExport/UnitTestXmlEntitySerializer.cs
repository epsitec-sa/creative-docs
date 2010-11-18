﻿using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.ImportExport;
using Epsitec.Cresus.DataLayer.Infrastructure;
using Epsitec.Cresus.DataLayer.UnitTests.Entities;
using Epsitec.Cresus.DataLayer.UnitTests.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

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

			DatabaseCreator2.PupulateDatabase ();
		}


		[TestMethod]
		public void TestAllExported()
		{
			XDocument xDocument;

			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				dataInfrastructure.OpenConnection ("id");

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					HashSet<AbstractEntity> exportableEntities = new HashSet<AbstractEntity> ()
					{
						dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001))),
						dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1000000001))),
						dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1000000002))),
						dataContext.ResolveEntity<UriSchemeEntity> (new DbKey (new DbId (1000000001))),
						dataContext.ResolveEntity<PersonGenderEntity> (new DbKey (new DbId (1000000001))),
						dataContext.ResolveEntity<LanguageEntity> (new DbKey (new DbId (1000000001))),
					};

					HashSet<AbstractEntity> externalEntities = new HashSet<AbstractEntity> ();

					xDocument = XmlEntitySerializer.Serialize (dataContext, exportableEntities, externalEntities);
				}
			}

			DatabaseHelper.CreateAndConnectToDatabase ();

			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				dataInfrastructure.OpenConnection ("id");

				DatabaseCreator2.RegisterSchema (dataInfrastructure);

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
				dataInfrastructure.OpenConnection ("id");

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					UriContactEntity uriContact = dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1000000004)));
					UriSchemeEntity uriScheme = dataContext.ResolveEntity<UriSchemeEntity> (new DbKey (new DbId (1000000001)));
					
					HashSet<AbstractEntity> exportableEntities = new HashSet<AbstractEntity> ()
					{
						uriContact,
					};

					HashSet<AbstractEntity> externalEntities = new HashSet<AbstractEntity> ()
					{
						uriScheme,
					};

					xDocument = XmlEntitySerializer.Serialize (dataContext, exportableEntities, externalEntities);

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
				dataInfrastructure.OpenConnection ("id");

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					UriSchemeEntity uriScheme = dataContext.ResolveEntity<UriSchemeEntity> (new DbKey (new DbId (1000000001)));

					HashSet<AbstractEntity> exportableEntities = new HashSet<AbstractEntity> ()
					{
						uriScheme,
					};

					HashSet<AbstractEntity> externalEntities = new HashSet<AbstractEntity> ();

					xDocument = XmlEntitySerializer.Serialize (dataContext, exportableEntities, externalEntities);

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
				dataInfrastructure.OpenConnection ("id");

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					UriContactEntity uriContact = dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1000000004)));
					UriSchemeEntity uriScheme = dataContext.ResolveEntity<UriSchemeEntity> (new DbKey (new DbId (1000000001)));

					HashSet<AbstractEntity> exportableEntities = new HashSet<AbstractEntity> ()
					{
						uriContact,
					};

					HashSet<AbstractEntity> externalEntities = new HashSet<AbstractEntity> ()
					{
						uriScheme,
					};

					xDocument = XmlEntitySerializer.Serialize (dataContext, exportableEntities, externalEntities);

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
				dataInfrastructure.OpenConnection ("id");

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
					UriContactEntity uriContact1 = dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1000000001)));
					UriContactEntity uriContact2 = dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1000000002)));
					LanguageEntity language = dataContext.ResolveEntity<LanguageEntity> (new DbKey (new DbId (1000000001)));
					PersonGenderEntity gender = dataContext.ResolveEntity<PersonGenderEntity> (new DbKey (new DbId (1000000001)));

					HashSet<AbstractEntity> exportableEntities = new HashSet<AbstractEntity> ()
					{
						person,
					};

					HashSet<AbstractEntity> externalEntities = new HashSet<AbstractEntity> ()
					{
						uriContact1,
						uriContact2,
						language,
						gender,
					};

					xDocument = XmlEntitySerializer.Serialize (dataContext, exportableEntities, externalEntities);

					dataContext.DeleteEntity (uriContact1);

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
