using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Expressions;
using Epsitec.Cresus.DataLayer.Infrastructure;
using Epsitec.Cresus.DataLayer.Loader;
using Epsitec.Cresus.DataLayer.Tests.Vs.Entities;
using Epsitec.Cresus.DataLayer.Tests.Vs.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;


namespace Epsitec.Cresus.DataLayer.Tests.Vs.Loader
{


	[TestClass]
	public sealed class UnitTestRequest
	{


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();

			DatabaseCreator2.ResetPopulatedTestDatabase ();
		}


		[TestMethod]
		public void RequestConstructorTest()
		{
			Request request = new Request ();

			Assert.IsNull (request.RequestedEntity);
			Assert.IsNull (request.RootEntity);
		}


		[TestMethod]
		public void ConditionsTest()
		{
			NaturalPersonEntity person = new NaturalPersonEntity ();
			UriContactEntity uriContact = new UriContactEntity ();
			PersonTitleEntity title = new PersonTitleEntity ();

			person.Title = title;
			person.Contacts.Add (uriContact);

			Request request = new Request ();

			Expression expression1 = new BinaryComparison
			(
				new PublicField (person, new Druid ("[J1AM1]")),
				BinaryComparator.IsEqual,
				new PublicField (person, new Druid ("[J1AL1]"))
			);

			Expression expression2 = new BinaryComparison
			(
				new PublicField (person, new Druid ("[J1AO1]")),
				BinaryComparator.IsEqual,
				new Constant (true)
			);

			Expression expression3 = new UnaryComparison
			(
				new PublicField (uriContact, new Druid ("[J1A62]")),
				UnaryComparator.IsNull
			);

			Expression expression4 = new BinaryOperation
			(
				new UnaryComparison
				(
					new PublicField (title, new Druid ("[J1AP]")),
					UnaryComparator.IsNotNull
				),
				BinaryOperator.Or,
				new UnaryComparison
				(
					new PublicField (title, new Druid ("[J1AO]")),
					UnaryComparator.IsNotNull
				)
			);

			var expected = new List<Expression> ();

			expected.Add (expression1);
			request.Conditions.Add (expression1);

			CollectionAssert.AreEqual (expected, request.Conditions);

			expected.Add (expression2);
			request.Conditions.Add (expression2);

			CollectionAssert.AreEqual (expected, request.Conditions);

			expected.Add (expression3);
			request.Conditions.Add (expression3);

			CollectionAssert.AreEqual (expected, request.Conditions);

			expected.Add (expression4);
			request.Conditions.Add (expression4);

			CollectionAssert.AreEqual (expected, request.Conditions);

			expected.Remove (expression1);
			request.Conditions.Remove (expression1);

			CollectionAssert.AreEqual (expected, request.Conditions);

			expected.Remove (expression2);
			request.Conditions.Remove (expression2);

			CollectionAssert.AreEqual (expected, request.Conditions);

			expected.Remove (expression3);
			request.Conditions.Remove (expression3);

			CollectionAssert.AreEqual (expected, request.Conditions);

			expected.Remove (expression4);
			request.Conditions.Remove (expression4);

			CollectionAssert.AreEqual (expected, request.Conditions);
		}


		[TestMethod]
		public void SortClausesTest()
		{
			NaturalPersonEntity person = new NaturalPersonEntity ();
			UriContactEntity uriContact = new UriContactEntity ();
			PersonTitleEntity title = new PersonTitleEntity ();

			person.Title = title;
			person.Contacts.Add (uriContact);

			Request request = new Request ();

			SortClause sortClause1 = new SortClause
			(
				new PublicField (person, new Druid ("[J1AM1]")),
				SortOrder.Ascending
			);

			SortClause sortClause2 = new SortClause
			(
				new PublicField (person, new Druid ("[J1AO1]")),
				SortOrder.Descending
			);

			SortClause sortClause3 = new SortClause
			(
				new PublicField (uriContact, new Druid ("[J1A62]")),
				SortOrder.Ascending
			);

			SortClause sortClause4 = new SortClause
			(
				new PublicField (title, new Druid ("[J1AP]")),
				SortOrder.Descending
			);

			var expected = new List<SortClause> ();

			expected.Add (sortClause1);
			request.SortClauses.Add (sortClause1);

			CollectionAssert.AreEqual (expected, request.SortClauses);

			expected.Add (sortClause2);
			request.SortClauses.Add (sortClause2);

			CollectionAssert.AreEqual (expected, request.SortClauses);

			expected.Add (sortClause3);
			request.SortClauses.Add (sortClause3);

			CollectionAssert.AreEqual (expected, request.SortClauses);

			expected.Add (sortClause4);
			request.SortClauses.Add (sortClause4);

			CollectionAssert.AreEqual (expected, request.SortClauses);

			expected.Remove (sortClause1);
			request.SortClauses.Remove (sortClause1);

			CollectionAssert.AreEqual (expected, request.SortClauses);

			expected.Remove (sortClause2);
			request.SortClauses.Remove (sortClause2);

			CollectionAssert.AreEqual (expected, request.SortClauses);

			expected.Remove (sortClause3);
			request.SortClauses.Remove (sortClause3);

			CollectionAssert.AreEqual (expected, request.SortClauses);

			expected.Remove (sortClause4);
			request.SortClauses.Remove (sortClause4);

			CollectionAssert.AreEqual (expected, request.SortClauses);
		}


		[TestMethod]
		public void RequestedEntityTest()
		{
			Request request = new Request ();
			
			AbstractEntity entity1 = new NaturalPersonEntity ();
			AbstractEntity entity2 = new NaturalPersonEntity ();

			Assert.IsNull (request.RequestedEntity);

			request.RequestedEntity = entity1;
			Assert.AreSame (entity1, request.RequestedEntity);

			request.RequestedEntity = null;
			Assert.IsNull (request.RequestedEntity);

			request.RootEntity = entity1;
			Assert.AreSame (entity1, request.RequestedEntity);

			request.RootEntity = null;
			Assert.IsNull (request.RequestedEntity);

			request.RootEntity = entity1;
			request.RequestedEntity = entity2;
			Assert.AreSame (entity2, request.RequestedEntity);
		}


		[TestMethod]
		public void RootEntityTest()
		{
			Request request = new Request ();
			
			AbstractEntity entity = new NaturalPersonEntity ();

			Assert.IsNull (request.RootEntity);

			request.RootEntity = entity;
			Assert.AreSame (entity, request.RootEntity);

			request.RootEntity = null;
			Assert.IsNull (request.RootEntity);
		}


		[TestMethod]
		public void RequestedTakeTest()
		{
			Request request = new Request ();

			for (int i = 0; i < 10; i++)
			{
				Assert.IsNull (request.Take);

				request.Take = i;

				Assert.AreEqual (i, request.Take);

				request.Take = null;
			}
		}


		[TestMethod]
		public void RequestedSkipTest()
		{
			Request request = new Request ();

			for (int i = 0; i < 10; i++)
			{
				Assert.IsNull (request.Skip);

				request.Skip = i;

				Assert.AreEqual (i, request.Skip);

				request.Skip = null;
			}
		}


		[TestMethod]
		public void CheckRootEntity()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var request = new Request ();

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => request.Check (dataContext)
				);
			}
		}


		[TestMethod]
		public void CheckRequestedEntity()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var request = new Request ()
				{
					RootEntity = new NaturalPersonEntity (),
					RequestedEntity = new NaturalPersonEntity (),
				};

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => request.Check (dataContext)
				);
			}
		}


		[TestMethod]
		public void CheckSkipAndTakeTest()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var request = new Request ()
				{
					RootEntity = new NaturalPersonEntity (),
					Skip = -1,
				};

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => request.Check (dataContext)
				);

				request = new Request ()
				{
					RootEntity = new NaturalPersonEntity (),
					Take = -1,
				};

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => request.Check (dataContext)
				);
			}
		}


		[TestMethod]
		public void CheckConditionsTest()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var request = new Request ()
				{
					RootEntity = new NaturalPersonEntity (),
				};

				request.Conditions.Add
				(
					new UnaryComparison
					(
						new PublicField (new NaturalPersonEntity (), Druid.Parse ("[J1AL1]")),
						UnaryComparator.IsNotNull
					)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => request.Check (dataContext)
				);

				request = new Request ()
				{
					RootEntity = new NaturalPersonEntity (),
				};

				request.Conditions.Add
				(
					new UnaryComparison
					(
						new PublicField (request.RootEntity, Druid.Parse ("[J1A82]")),
						UnaryComparator.IsNotNull
					)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => request.Check (dataContext)
				);

				request = new Request ()
				{
					RootEntity = new NaturalPersonEntity (),
				};

				request.Conditions.Add
				(
					new UnaryComparison
					(
						InternalField.CreateId (new NaturalPersonEntity ()),
						UnaryComparator.IsNotNull
					)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => request.Check (dataContext)
				);
			}
		}


		[TestMethod]
		public void CheckSortClausesTest()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var request = new Request ()
				{
					RootEntity = new NaturalPersonEntity (),
				};

				request.SortClauses.Add
				(
					new SortClause
					(
						new PublicField (new NaturalPersonEntity (), Druid.Parse ("[J1AL1]")),
						SortOrder.Ascending
					)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => request.Check (dataContext)
				);

				request = new Request ()
				{
					RootEntity = new NaturalPersonEntity (),
				};

				request.SortClauses.Add
				(
					new SortClause
					(
						new PublicField (request.RootEntity, Druid.Parse ("[J1A82]")),
						SortOrder.Descending
					)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => request.Check (dataContext)
				);

				request = new Request ()
				{
					RootEntity = new NaturalPersonEntity (),
				};

				request.SortClauses.Add
				(
					new SortClause
					(
						InternalField.CreateId (new NaturalPersonEntity ()),
						SortOrder.Descending
					)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => request.Check (dataContext)
				);
			}
		}


		[TestMethod]
		public void CheckCycleDetectionTest1()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				foreach (var entity in this.GetGraphsWithCycle ())
				{
					Request request = new Request ()
					{
						RootEntity = entity
					};

					ExceptionAssert.Throw<System.ArgumentException>
					(
						() => request.Check (dataContext)
					);
				}
			}
		}


		[TestMethod]
		public void CheckCycleDetectionTest2()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				foreach (var entity in this.GetGraphsWithoutCycle (dataContext))
				{
					Request request = new Request ()
					{
						RootEntity = entity
					};

					request.Check (dataContext);
				}
			}
		}


		private IEnumerable<AbstractEntity> GetGraphsWithCycle()
		{
			{
				NaturalPersonEntity person = new NaturalPersonEntity ();
				MailContactEntity contact = new MailContactEntity ();

				person.Contacts.Add (contact);
				contact.NaturalPerson = person;

				yield return person;
			}
			{
				NaturalPersonEntity person1 = new NaturalPersonEntity ();
				NaturalPersonEntity person2 = new NaturalPersonEntity ();
				MailContactEntity contact1 = new MailContactEntity ();
				MailContactEntity contact2 = new MailContactEntity ();

				person1.Contacts.Add (contact1);
				contact1.NaturalPerson = person2;
				person2.Contacts.Add (contact2);
				contact2.NaturalPerson = person1;
			}
		}


		private IEnumerable<AbstractEntity> GetGraphsWithoutCycle(DataContext dataContext)
		{
			{
				CountryEntity country = new CountryEntity ();
				RegionEntity region = new RegionEntity ();
				LocationEntity location = new LocationEntity ();

				country.Name = "country";

				region.Country = country;
				location.Country = country;
				location.Region = region;

				yield return location;
			}
			{
				PersonGenderEntity gender = new PersonGenderEntity ();
				PersonTitleEntity title = new PersonTitleEntity ();
				NaturalPersonEntity person = new NaturalPersonEntity ();

				gender.Name = "gender";
				title.ComptatibleGenders.Add (gender);
				person.Gender = gender;
				person.Title = title;

				yield return person;
			}
			{
				yield return dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1000000001)));
			}
		}


	}


}
