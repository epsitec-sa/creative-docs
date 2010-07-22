﻿using Epsitec.Common.Support;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Loader;
using Epsitec.Cresus.DataLayer.Expressions;
using Epsitec.Cresus.DataLayer.UnitTests.Entities;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.UnitTests
{


	[TestClass]
	public sealed class UnitTestGetEntitiesByRequest
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
		public void UnaryComparisonTest()
		{
			using (DataContext dataContext = new DataContext(DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity example = new NaturalPersonEntity ();

				Request request = new Request ()
				{
					RootEntity = example,
					RequestedEntity = example,
				};

				request.AddLocalConstraint (example,
					new UnaryComparison (
						new Field (new Druid ("[L0A01]")),
						UnaryComparator.IsNotNull
					)
				);

				NaturalPersonEntity[] persons = dataContext.GetByRequest<NaturalPersonEntity> (request).ToArray ();

				Assert.IsTrue (persons.Count () == 3);
				Assert.IsTrue (persons.Any (p => DatabaseCreator2.CheckAlfred (p)));
				Assert.IsTrue (persons.Any (p => DatabaseCreator2.CheckGertrude (p)));
				Assert.IsTrue (persons.Any (p => DatabaseCreator2.CheckHans (p)));
			}
		}


		[TestMethod]
		public void BinaryComparisonFieldWithValueTest()
		{
			using (DataContext dataContext = new DataContext(DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity example = new NaturalPersonEntity ();

				Request request = new Request ()
				{
					RootEntity = example,
					RequestedEntity = example,
				};

				request.AddLocalConstraint (example,
					new BinaryComparisonFieldWithValue (
						new Field (new Druid ("[L0AV]")),
						BinaryComparator.IsEqual,
						new Constant ("Alfred")
					)
				);

				NaturalPersonEntity[] persons = dataContext.GetByRequest<NaturalPersonEntity> (request).ToArray ();

				Assert.IsTrue (persons.Count () == 1);
				Assert.IsTrue (persons.Any (p => DatabaseCreator2.CheckAlfred (p)));
			}
		}


		[TestMethod]
		public void BinaryComparisonFieldWithFieldTest()
		{
			using (DataContext dataContext = new DataContext(DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity example = new NaturalPersonEntity ();

				Request request = new Request ()
				{
					RootEntity = example,
					RequestedEntity = example,
				};

				request.AddLocalConstraint (example,
					new BinaryComparisonFieldWithField (
						new Field (new Druid ("[L0AV]")),
						BinaryComparator.IsEqual,
						new Field (new Druid ("[L0A01]"))
					)
				);

				NaturalPersonEntity[] persons = dataContext.GetByRequest<NaturalPersonEntity> (request).ToArray ();

				Assert.IsTrue (persons.Count () == 0);
			}
		}


		[TestMethod]
		public void UnaryOperationTest()
		{
			using (DataContext dataContext = new DataContext(DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity example = new NaturalPersonEntity ();

				Request request = new Request ()
				{
					RootEntity = example,
					RequestedEntity = example,
				};

				request.AddLocalConstraint (example,
					new UnaryOperation (
						UnaryOperator.Not,
						new BinaryComparisonFieldWithValue (
							new Field (new Druid ("[L0AV]")),
							BinaryComparator.IsEqual,
							new Constant ("Hans")
						)
					)
				);

				NaturalPersonEntity[] persons = dataContext.GetByRequest<NaturalPersonEntity> (request).ToArray ();

				Assert.IsTrue (persons.Count () == 2);
				Assert.IsTrue (persons.Any (p => DatabaseCreator2.CheckAlfred (p)));
				Assert.IsTrue (persons.Any (p => DatabaseCreator2.CheckGertrude (p)));
			}
		}


		[TestMethod]
		public void BinaryOperationTest()
		{
			using (DataContext dataContext = new DataContext(DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity example = new NaturalPersonEntity ();

				Request request = new Request ()
				{
					RootEntity = example,
					RequestedEntity = example,
				};

				request.AddLocalConstraint (example,
					new BinaryOperation (
						new BinaryComparisonFieldWithValue (
							new Field (new Druid ("[L0AV]")),
							BinaryComparator.IsNotEqual,
							new Constant ("Hans")
						),
						BinaryOperator.And,
						new BinaryComparisonFieldWithValue (
							new Field (new Druid ("[L0AV]")),
							BinaryComparator.IsNotEqual,
							new Constant ("Gertrude")
						)
					)
				);

				NaturalPersonEntity[] persons = dataContext.GetByRequest<NaturalPersonEntity> (request).ToArray ();

				Assert.IsTrue (persons.Count () == 1);
				Assert.IsTrue (persons.Any (p => DatabaseCreator2.CheckAlfred (p)));
			}
		}


		[TestMethod]
		public void DoubleRequest1()
		{
			using (DataContext dataContext = new DataContext(DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity example = new NaturalPersonEntity ();

				Request request = new Request ()
				{
					RootEntity = example,
					RequestedEntity = example,
				};

				request.AddLocalConstraint (example,
					new BinaryComparisonFieldWithValue (
						new Field (new Druid ("[L0AV]")),
						BinaryComparator.IsEqual,
						new Constant ("Alfred")
					)
				);

				request.AddLocalConstraint (example,
					new BinaryComparisonFieldWithValue (
						new Field (new Druid ("[L0A01]")),
						BinaryComparator.IsEqual,
						new Constant ("Dupond")
					)
				);

				NaturalPersonEntity[] persons = dataContext.GetByRequest<NaturalPersonEntity> (request).ToArray ();

				Assert.IsTrue (persons.Count () == 1);
				Assert.IsTrue (persons.Any (p => DatabaseCreator2.CheckAlfred (p)));
			}
		}

		[TestMethod]
		public void DoubleRequest2()
		{
			using (DataContext dataContext = new DataContext(DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity example = new NaturalPersonEntity ()
				{
					Gender = new PersonGenderEntity (),
				};

				Request request = new Request ()
				{
					RootEntity = example,
					RequestedEntity = example,
				};

				request.AddLocalConstraint (example,
					new BinaryComparisonFieldWithValue (
						new Field (new Druid ("[L0AV]")),
						BinaryComparator.IsEqual,
						new Constant ("Alfred")
					)
				);

				request.AddLocalConstraint (example.Gender,
					new BinaryComparisonFieldWithValue (
						new Field (new Druid ("[L0AC1]")),
						BinaryComparator.IsEqual,
						new Constant ("Male")
					)
				);

				NaturalPersonEntity[] persons = dataContext.GetByRequest<NaturalPersonEntity> (request).ToArray ();

				Assert.IsTrue (persons.Count () == 1);
				Assert.IsTrue (persons.Any (p => DatabaseCreator2.CheckAlfred (p)));
			}
		}


		[TestMethod]
		public void InnerRequest()
		{
			using (DataContext dataContext = new DataContext(DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity example = new NaturalPersonEntity ()
				{
					Gender = new PersonGenderEntity (),
				};

				Request request = new Request ()
				{
					RootEntity = example,
					RequestedEntity = example,
				};

				request.AddLocalConstraint (example.Gender,
					new BinaryComparisonFieldWithValue (
						new Field (new Druid ("[L0AC1]")),
						BinaryComparator.IsEqual,
						new Constant ("Male")
					)
				);

				NaturalPersonEntity[] persons = dataContext.GetByRequest<NaturalPersonEntity> (request).ToArray ();

				Assert.IsTrue (persons.Count () == 1);
				Assert.IsTrue (persons.Any (p => DatabaseCreator2.CheckAlfred (p)));
			}
		}


		[TestMethod]
		public void LikeRequest()
		{
			using (DataContext dataContext = new DataContext(DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity example = new NaturalPersonEntity ()
				{
					Gender = new PersonGenderEntity (),
				};

				Request request = new Request ()
				{
					RootEntity = example,
					RequestedEntity = example,
				};

				request.AddLocalConstraint (example.Gender,
					new BinaryComparisonFieldWithValue (
						new Field (new Druid ("[L0AC1]")),
						BinaryComparator.IsLike,
						new Constant ("%ale")
					)
				);

				NaturalPersonEntity[] persons = dataContext.GetByRequest<NaturalPersonEntity> (request).ToArray ();

				Assert.IsTrue (persons.Count () == 2);
				Assert.IsTrue (persons.Any (p => DatabaseCreator2.CheckAlfred (p)));
				Assert.IsTrue (persons.Any (p => DatabaseCreator2.CheckGertrude (p)));
			}
		}


		[TestMethod]
		public void LikeEscapeRequest()
		{
			using (DataContext dataContext = new DataContext(DatabaseHelper.DbInfrastructure))
			{
				CountryEntity country1 = DatabaseHelper.CreateCountry (dataContext, "c1", "test%test");
				CountryEntity country2 = DatabaseHelper.CreateCountry (dataContext, "c2", "test_test");
				CountryEntity country3 = DatabaseHelper.CreateCountry (dataContext, "c2", "test#test");
				CountryEntity country4 = DatabaseHelper.CreateCountry (dataContext, "c3", "testxxtest");

				dataContext.SaveChanges ();
			}

			using (DataContext dataContext = new DataContext(DatabaseHelper.DbInfrastructure))
			{
				CountryEntity example = new CountryEntity ();

				Request request = new Request ()
				{
					RootEntity = example,
					RequestedEntity = example,
				};

				request.AddLocalConstraint (example,
					new BinaryComparisonFieldWithValue (
						new Field (new Druid ("[L0A3]")),
						BinaryComparator.IsLike,
						new Constant ("test%test")
					)
				);

				CountryEntity[] countries = dataContext.GetByRequest<CountryEntity> (request).ToArray ();

				Assert.IsTrue (countries.Count () == 4);
				Assert.IsTrue (countries.Any (c => c.Name == "test%test"));
				Assert.IsTrue (countries.Any (c => c.Name == "test_test"));
				Assert.IsTrue (countries.Any (c => c.Name == "test#test"));
				Assert.IsTrue (countries.Any (c => c.Name == "testxxtest"));
			}

			using (DataContext dataContext = new DataContext(DatabaseHelper.DbInfrastructure))
			{
				CountryEntity example = new CountryEntity ();

				Request request = new Request ()
				{
					RootEntity = example,
					RequestedEntity = example,
				};

				request.AddLocalConstraint (example,
					new BinaryComparisonFieldWithValue (
						new Field (new Druid ("[L0A3]")),
						BinaryComparator.IsLike,
						new Constant ("test_test")
					)
				);

				CountryEntity[] countries = dataContext.GetByRequest<CountryEntity> (request).ToArray ();

				Assert.IsTrue (countries.Count () == 3);
				Assert.IsTrue (countries.Any (c => c.Name == "test%test"));
				Assert.IsTrue (countries.Any (c => c.Name == "test_test"));
				Assert.IsTrue (countries.Any (c => c.Name == "test#test"));
			}

			using (DataContext dataContext = new DataContext(DatabaseHelper.DbInfrastructure))
			{
				CountryEntity example = new CountryEntity ();

				Request request = new Request ()
				{
					RootEntity = example,
					RequestedEntity = example,
				};

				string value = BinaryComparisonFieldWithValue.Escape ("test%test");

				request.AddLocalConstraint (example,
					new BinaryComparisonFieldWithValue (
						new Field (new Druid ("[L0A3]")),
						BinaryComparator.IsLikeEscape,
						new Constant (value)
					)
				);

				CountryEntity[] countries = dataContext.GetByRequest<CountryEntity> (request).ToArray ();

				Assert.IsTrue (countries.Count () == 1);
				Assert.IsTrue (countries.Any (c => c.Name == "test%test"));
			}

			using (DataContext dataContext = new DataContext(DatabaseHelper.DbInfrastructure))
			{
				CountryEntity example = new CountryEntity ();

				Request request = new Request ()
				{
					RootEntity = example,
					RequestedEntity = example,
				};

				string value = BinaryComparisonFieldWithValue.Escape ("test_test");

				request.AddLocalConstraint (example,
					new BinaryComparisonFieldWithValue (
						new Field (new Druid ("[L0A3]")),
						BinaryComparator.IsLikeEscape,
						new Constant (value)
					)
				);

				CountryEntity[] countries = dataContext.GetByRequest<CountryEntity> (request).ToArray ();

				Assert.IsTrue (countries.Count () == 1);
				Assert.IsTrue (countries.Any (c => c.Name == "test_test"));
			}

			using (DataContext dataContext = new DataContext(DatabaseHelper.DbInfrastructure))
			{
				CountryEntity example = new CountryEntity ();

				Request request = new Request ()
				{
					RootEntity = example,
					RequestedEntity = example,
				};

				string value = BinaryComparisonFieldWithValue.Escape ("test#test");

				request.AddLocalConstraint (example,
					new BinaryComparisonFieldWithValue (
						new Field (new Druid ("[L0A3]")),
						BinaryComparator.IsLikeEscape,
						new Constant (value)
					)
				);

				CountryEntity[] countries = dataContext.GetByRequest<CountryEntity> (request).ToArray ();

				Assert.IsTrue (countries.Count () == 1);
				Assert.IsTrue (countries.Any (c => c.Name == "test#test"));
			}
		}


		[TestMethod]
		public void RequestedEntityRequest1()
		{
			using (DataContext dataContext = new DataContext(DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity example = DatabaseCreator2.GetCorrectExample3 ();

				Request request = new Request ()
				{
					RootEntity = example,
				};

				NaturalPersonEntity[] persons = dataContext.GetByRequest<NaturalPersonEntity> (request).ToArray ();

				Assert.IsTrue (persons.Length == 2);

				Assert.IsTrue (persons.Any (p => DatabaseCreator2.CheckAlfred (p)));
				Assert.IsTrue (persons.Any (p => DatabaseCreator2.CheckGertrude (p)));
			}
		}


		[TestMethod]
		public void RequestedEntityRequest2()
		{
			using (DataContext dataContext = new DataContext(DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity example = DatabaseCreator2.GetCorrectExample3 ();

				Request request = new Request ()
				{
					RootEntity = example,
					RequestedEntity = example.Contacts[0],
				};

				UriContactEntity[] contacts = dataContext.GetByRequest<UriContactEntity> (request).ToArray ();

				Assert.IsTrue (contacts.Length == 3);

				Assert.IsTrue (contacts.Any (c => DatabaseCreator2.CheckUriContact (c, "alfred@coucou.com", "Alfred")));
				Assert.IsTrue (contacts.Any (c => DatabaseCreator2.CheckUriContact (c, "alfred@blabla.com", "Alfred")));
				Assert.IsTrue (contacts.Any (c => DatabaseCreator2.CheckUriContact (c, "gertrude@coucou.com", "Gertrude")));
			}
		}


		[TestMethod]
		public void RequestedEntityRequest3()
		{
			using (DataContext dataContext = new DataContext(DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity example = DatabaseCreator2.GetCorrectExample3 ();

				Request request = new Request ()
				{
					RootEntity = example,
					RequestedEntity = (example.Contacts[0] as UriContactEntity).UriScheme,
				};

				UriSchemeEntity[] uriSchemes = dataContext.GetByRequest<UriSchemeEntity> (request).ToArray ();

				Assert.IsTrue (uriSchemes.Length == 1);

				Assert.IsTrue (uriSchemes.Any (s => s.Code == "mailto:" && s.Name == "email"));
			}
		}


		[TestMethod]
		public void RootEntityReferenceRequest1()
		{
			using (DataContext dataContext = new DataContext(DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity example = DatabaseCreator2.GetCorrectExample1();
				NaturalPersonEntity alfred = dataContext.GetByExample<NaturalPersonEntity> (example).First ();

				Request request = new Request ()
				{
					RootEntity = new NaturalPersonEntity(),
					RootEntityKey = dataContext.GetEntityKey(alfred).Value.RowKey,
				};

				NaturalPersonEntity[] persons = dataContext.GetByRequest<NaturalPersonEntity> (request).ToArray ();

				Assert.IsTrue (persons.Length == 1);

				Assert.IsTrue (persons.Any (p => DatabaseCreator2.CheckAlfred (p)));
			}
		}


		[TestMethod]
		public void RootEntityReferenceRequest2()
		{
			using (DataContext dataContext = new DataContext(DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity example1 = DatabaseCreator2.GetCorrectExample1 ();
				NaturalPersonEntity alfred1 = dataContext.GetByExample<NaturalPersonEntity> (example1).First ();

				NaturalPersonEntity example2 = new NaturalPersonEntity ();
				example2.Contacts.Add (new UriContactEntity ()
				{
					UriScheme = new UriSchemeEntity ()
				});

				Request request = new Request ()
				{
					RootEntity = example2,
					RootEntityKey = dataContext.GetEntityKey(alfred1).Value.RowKey,
					RequestedEntity = (example2.Contacts[0] as UriContactEntity).UriScheme,
				};

				UriSchemeEntity[] uriSchemes = dataContext.GetByRequest<UriSchemeEntity> (request).ToArray ();

				Assert.IsTrue (uriSchemes.Length == 1);

				Assert.IsTrue (uriSchemes.Any (s => s.Code == "mailto:" && s.Name == "email"));
			}
		}


		[TestMethod]
		public void RootEntityReferenceRequest3()
		{
			using (DataContext dataContext = new DataContext(DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity example1 = DatabaseCreator2.GetCorrectExample1 ();
				NaturalPersonEntity alfred1 = dataContext.GetByExample<NaturalPersonEntity> (example1).First ();

				NaturalPersonEntity example2 = new NaturalPersonEntity ();
				example2.Contacts.Add (new AbstractContactEntity ());

				Request request = new Request ()
				{
					RootEntity = example2,
					RootEntityKey = dataContext.GetEntityKey (alfred1).Value.RowKey,
					RequestedEntity = example2.Contacts[0],
				};

				UriContactEntity[] contacts = dataContext.GetByRequest<UriContactEntity> (request).ToArray ();

				Assert.IsTrue (contacts.Length == 2);

				Assert.IsTrue (contacts.Any (c => DatabaseCreator2.CheckUriContact (c, "alfred@coucou.com", "Alfred")));
				Assert.IsTrue (contacts.Any (c => DatabaseCreator2.CheckUriContact (c, "alfred@blabla.com", "Alfred")));
			}
		}


		private bool IsExceptionThrown(System.Action action)
		{
			try
			{
				action ();
				return false;
			}
			catch (System.Exception)
			{
				return true;
			}
		}


	}


}
