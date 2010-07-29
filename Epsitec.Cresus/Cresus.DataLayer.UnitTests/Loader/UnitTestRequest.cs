using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Loader;
using Epsitec.Cresus.DataLayer.Expressions;
using Epsitec.Cresus.DataLayer.UnitTests.Entities;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;
using System.Linq;


namespace Epsitec.Cresus.DataLayer.UnitTests
{


	[TestClass]
	public sealed class UnitTestRequest
	{


		[ClassInitialize]
		public static void Initialize(TestContext testContext)
		{
			TestHelper.Initialize ();

			DatabaseHelper.CreateAndConnectToDatabase ();

			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				DatabaseCreator2.PupulateDatabase (dataContext);
			}
		}


		[ClassCleanup]
		public static void Cleanup()
		{
			DatabaseHelper.DisconnectFromDatabase ();
		}


		[TestMethod]
		public void RequestConstructorTest()
		{
			Request request = new Request ();
			Request_Accessor requestAccessor = new Request_Accessor (new PrivateObject (request));

			Assert.IsNull (requestAccessor.RequestedEntity);
			Assert.IsNull (requestAccessor.RootEntity);
			Assert.IsNull (requestAccessor.RootEntityKey);
		}


		[TestMethod]
		public void AddLocalConstraintTest1()
		{
			NaturalPersonEntity person = new NaturalPersonEntity ();
			UriContactEntity uriContact = new UriContactEntity ();
			PersonTitleEntity title = new PersonTitleEntity ();

			person.Title = title;
			person.Contacts.Add (uriContact);

			Request request = new Request ();

			Expression expression1 =
				new ComparisonFieldField (
					new Field (new Druid ("[L0A01]")),
					BinaryComparator.IsEqual,
					new Field (new Druid ("[L0AV]"))
				);

			Expression expression2 =
				new ComparisonFieldValue (
					new Field (new Druid ("[L0A61]")),
					BinaryComparator.IsEqual,
					new Constant (true)
				);

			Expression expression3 =
				new UnaryComparison (
					new Field (new Druid ("[L0AA2]")),
					UnaryComparator.IsNull
				);

			Expression expression4 =
				new BinaryOperation (
					new UnaryComparison (
						new Field (new Druid ("[L0AT1]")),
						UnaryComparator.IsNotNull
					),
					BinaryOperator.Or,
					new UnaryComparison (
						new Field (new Druid ("[L0AS1]")),
						UnaryComparator.IsNotNull
					)
				);
			
			Request_Accessor requestAccessor = new Request_Accessor (new PrivateObject (request));

			Assert.IsFalse (requestAccessor.IsLocalyConstrained (person));
			Assert.IsFalse (requestAccessor.IsLocalyConstrained (uriContact));
			Assert.IsFalse (requestAccessor.IsLocalyConstrained (title));

			request.AddLocalConstraint (person, expression1);
			request.AddLocalConstraint (person, expression2);
			request.AddLocalConstraint (uriContact, expression3);
			request.AddLocalConstraint (title, expression4);

			Assert.IsTrue (requestAccessor.GetLocalConstraints (person).Count () == 2);
			Assert.IsTrue (requestAccessor.GetLocalConstraints (uriContact).Count () == 1);
			Assert.IsTrue (requestAccessor.GetLocalConstraints (title).Count () == 1);

			CollectionAssert.Contains (requestAccessor.GetLocalConstraints (person).ToList (), expression1);
			CollectionAssert.Contains (requestAccessor.GetLocalConstraints (person).ToList (), expression2);
			CollectionAssert.Contains (requestAccessor.GetLocalConstraints (uriContact).ToList (), expression3);
			CollectionAssert.Contains (requestAccessor.GetLocalConstraints (title).ToList (), expression4);

			Assert.IsTrue (requestAccessor.IsLocalyConstrained (person));
			Assert.IsTrue (requestAccessor.IsLocalyConstrained (uriContact));
			Assert.IsTrue (requestAccessor.IsLocalyConstrained (title));
		}


		[TestMethod]
		[ExpectedException (typeof (System.ArgumentException))]
		public void AddLocalConstraintTest2()
		{
			NaturalPersonEntity person = new NaturalPersonEntity ();

			Request request = new Request ();

			Expression expression =
				new ComparisonFieldField (
					new Field (new Druid ("[L0AS1]")),
					BinaryComparator.IsEqual,
					new Field (new Druid ("[L0AV]"))
				);

			request.AddLocalConstraint (person, expression);
		}


		[TestMethod]
		[ExpectedException (typeof (System.ArgumentException))]
		public void AddLocalConstraintTest3()
		{
			NaturalPersonEntity person = new NaturalPersonEntity ();

			Request request = new Request ();

			Expression expression =
				new ComparisonFieldValue (
					new Field (new Druid ("[L0AS]")),
					BinaryComparator.IsEqual,
					new Constant (true)
				);

			request.AddLocalConstraint (person, expression);
		}


		[TestMethod]
		[ExpectedException (typeof (System.ArgumentException))]
		public void AddLocalConstraintTest4()
		{
			NaturalPersonEntity person = new NaturalPersonEntity ();
			UriContactEntity uriContact = new UriContactEntity ();

			person.Contacts.Add (uriContact);

			Request request = new Request ();

			Expression expression =
				new UnaryComparison (
					new Field (new Druid ("[L0AR]")),
					UnaryComparator.IsNull
				);

			request.AddLocalConstraint (uriContact, expression);
		}


		[TestMethod]
		[ExpectedException (typeof (System.ArgumentException))]
		public void AddLocalConstraintTest5()
		{
			NaturalPersonEntity person = new NaturalPersonEntity ();
			PersonTitleEntity title = new PersonTitleEntity ();

			person.Title = title;

			Request request = new Request ();

			Expression expression =
				new BinaryOperation (
					new UnaryComparison (
						new Field (new Druid ("[L0A61]")),
						UnaryComparator.IsNotNull
					),
					BinaryOperator.Or,
					new UnaryComparison (
						new Field (new Druid ("[L0AS1]")),
						UnaryComparator.IsNotNull
					)
				);

			request.AddLocalConstraint (title, expression);
		}


		[TestMethod]
		[ExpectedException (typeof (System.ArgumentNullException))]
		public void AddLocalConstraintTest6()
		{
			NaturalPersonEntity person = new NaturalPersonEntity ();

			Request request = new Request ();

			Expression expression =
				new UnaryComparison (
					new Field (new Druid ("[L0AV]")),
					UnaryComparator.IsNull
				);

			request.AddLocalConstraint (null, expression);
		}


		[TestMethod]
		[ExpectedException (typeof (System.ArgumentNullException))]
		public void AddLocalConstraintTest7()
		{
			NaturalPersonEntity person = new NaturalPersonEntity ();

			Request request = new Request ();

			request.AddLocalConstraint (person, null);
		}


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		public void IsEntityValueFieldTest()
		{
			Request request = new Request ();
			Request_Accessor requestAccessor = new Request_Accessor (new PrivateObject (request));

			NaturalPersonEntity person = new NaturalPersonEntity ();

			List<string> validIds = new List<string> ()
			{
				"[L0AV]",
				"[L0A01]",
				"[L0A61]"
			};

			List<string> invalidIds = new List<string> ()
			{
				"[L0S]",
				"[L0AD1]",
				"[L0AU]",
				"[L0A11]",
				"[L0A92]",
				"[L0AN]",
				"[L0AM]",
				"[L0AG1]",
				"[L0AA]",
				"[L0AR]",
			};

			foreach (string validId in validIds)
			{
				Assert.IsTrue (requestAccessor.IsEntityValueField (person, validId));
			}

			foreach (string invalidId in invalidIds)
			{
				Assert.IsFalse (requestAccessor.IsEntityValueField (person, invalidId));
			}
		}


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		[ExpectedException (typeof (System.ArgumentNullException))]
		public void IsLocalyConstrainedTest()
		{
			Request request = new Request ();
			Request_Accessor requestAccessor = new Request_Accessor (new PrivateObject (request));

			requestAccessor.IsLocalyConstrained (null);
		}


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		public void RequestedEntityTest()
		{
			Request request = new Request ();
			Request_Accessor requestAccessor = new Request_Accessor (new PrivateObject (request));

			AbstractEntity entity1 = new NaturalPersonEntity ();
			AbstractEntity entity2 = new NaturalPersonEntity ();

			Assert.IsNull (requestAccessor.RequestedEntity);
			
			request.RequestedEntity = entity1;
			Assert.AreSame (entity1, requestAccessor.RequestedEntity);

			request.RequestedEntity = null;
			Assert.IsNull (requestAccessor.RequestedEntity);

			request.RootEntity = entity1;
			Assert.AreSame (entity1, requestAccessor.RequestedEntity);

			request.RootEntity = null;
			Assert.IsNull (requestAccessor.RequestedEntity);

			request.RootEntity = entity1;
			request.RequestedEntity = entity2;
			Assert.AreSame (entity2, requestAccessor.RequestedEntity);
		}


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		public void RootEntityTest()
		{
			Request request = new Request ();
			Request_Accessor requestAccessor = new Request_Accessor (new PrivateObject (request));

			AbstractEntity entity = new NaturalPersonEntity ();

			Assert.IsNull (requestAccessor.RootEntity);

			request.RootEntity = entity;
			Assert.AreSame (entity, requestAccessor.RootEntity);

			request.RootEntity = null;
			Assert.IsNull (requestAccessor.RootEntity);
		}


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		public void RootEntityKeyTest()
		{
			Request request = new Request ();
			Request_Accessor requestAccessor = new Request_Accessor (new PrivateObject (request));

			DbKey dbKey = new DbKey (new DbId (1));

			Assert.IsFalse (requestAccessor.RootEntityKey.HasValue);

			request.RootEntityKey = dbKey;
			Assert.IsTrue (requestAccessor.RootEntityKey.HasValue);
			Assert.AreEqual (dbKey, requestAccessor.RootEntityKey.Value);

			request.RootEntityKey = null;
			Assert.IsFalse (requestAccessor.RootEntityKey.HasValue);
		}


	}


}
