using Epsitec.Common.Support;

using Epsitec.Common.Types;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Infrastructure;
using Epsitec.Cresus.DataLayer.Loader;
using Epsitec.Cresus.DataLayer.Expressions;
using Epsitec.Cresus.DataLayer.UnitTests.Entities;
using Epsitec.Cresus.DataLayer.UnitTests.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.UnitTests.General
{


	[TestClass]
	public sealed class UnitTestGetEntitiesByRequest
	{


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();

			DatabaseHelper.CreateAndConnectToDatabase ();

			DatabaseCreator2.PupulateDatabase ();

			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				dataInfrastructure.OpenConnection ("id");

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					for (int i = 0; i < 5; i++)
					{
						int? rank = (i % 2 == 0) ? (int?) null : i;

						DatabaseHelper.CreateContactRole (dataContext, "role" + i, rank);
					}

					dataContext.SaveChanges ();
				}
			}
		}


		[ClassCleanup]
		public static void ClassCleanup()
		{
			DatabaseHelper.DisconnectFromDatabase ();
		}


		[TestMethod]
		public void UnaryComparisonTest()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				dataInfrastructure.OpenConnection ("id");

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
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
		}


		[TestMethod]
		public void BinaryComparisonFieldWithValueTest()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				dataInfrastructure.OpenConnection ("id");

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					NaturalPersonEntity example = new NaturalPersonEntity ();

					Request request = new Request ()
					{
						RootEntity = example,
						RequestedEntity = example,
					};

					request.AddLocalConstraint (example,
						new ComparisonFieldValue (
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
		}


		[TestMethod]
		public void BinaryComparisonFieldWithFieldTest()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				dataInfrastructure.OpenConnection ("id");

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					NaturalPersonEntity example = new NaturalPersonEntity ();

					Request request = new Request ()
					{
						RootEntity = example,
						RequestedEntity = example,
					};

					request.AddLocalConstraint (example,
						new ComparisonFieldField (
							new Field (new Druid ("[L0AV]")),
							BinaryComparator.IsEqual,
							new Field (new Druid ("[L0A01]"))
						)
					);

					NaturalPersonEntity[] persons = dataContext.GetByRequest<NaturalPersonEntity> (request).ToArray ();

					Assert.IsTrue (persons.Count () == 0);
				}
			}
		}


		[TestMethod]
		public void UnaryOperationTest()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				dataInfrastructure.OpenConnection ("id");

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
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
							new ComparisonFieldValue (
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
		}


		[TestMethod]
		public void BinaryOperationTest()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				dataInfrastructure.OpenConnection ("id");

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					NaturalPersonEntity example = new NaturalPersonEntity ();

					Request request = new Request ()
					{
						RootEntity = example,
						RequestedEntity = example,
					};

					request.AddLocalConstraint (example,
						new BinaryOperation (
							new ComparisonFieldValue (
								new Field (new Druid ("[L0AV]")),
								BinaryComparator.IsNotEqual,
								new Constant ("Hans")
							),
							BinaryOperator.And,
							new ComparisonFieldValue (
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
		}


		[TestMethod]
		public void DoubleRequest1()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				dataInfrastructure.OpenConnection ("id");

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					NaturalPersonEntity example = new NaturalPersonEntity ();

					Request request = new Request ()
					{
						RootEntity = example,
						RequestedEntity = example,
					};

					request.AddLocalConstraint (example,
						new ComparisonFieldValue (
							new Field (new Druid ("[L0AV]")),
							BinaryComparator.IsEqual,
							new Constant ("Alfred")
						)
					);

					request.AddLocalConstraint (example,
						new ComparisonFieldValue (
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
		}

		[TestMethod]
		public void DoubleRequest2()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				dataInfrastructure.OpenConnection ("id");

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
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
						new ComparisonFieldValue (
							new Field (new Druid ("[L0AV]")),
							BinaryComparator.IsEqual,
							new Constant ("Alfred")
						)
					);

					request.AddLocalConstraint (example.Gender,
						new ComparisonFieldValue (
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
		}


		[TestMethod]
		public void InnerRequest()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				dataInfrastructure.OpenConnection ("id");

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
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
						new ComparisonFieldValue (
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
		}


		[TestMethod]
		public void LikeRequest()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				dataInfrastructure.OpenConnection ("id");

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
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
						new ComparisonFieldValue (
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
		}


		[TestMethod]
		public void LikeEscapeRequest()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				dataInfrastructure.OpenConnection ("id");

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					CountryEntity country1 = DatabaseHelper.CreateCountry (dataContext, "c1", "test%test");
					CountryEntity country2 = DatabaseHelper.CreateCountry (dataContext, "c2", "test_test");
					CountryEntity country3 = DatabaseHelper.CreateCountry (dataContext, "c2", "test#test");
					CountryEntity country4 = DatabaseHelper.CreateCountry (dataContext, "c3", "testxxtest");

					dataContext.SaveChanges ();
				}

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					CountryEntity example = new CountryEntity ();

					Request request = new Request ()
					{
						RootEntity = example,
						RequestedEntity = example,
					};

					request.AddLocalConstraint (example,
						new ComparisonFieldValue (
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

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					CountryEntity example = new CountryEntity ();

					Request request = new Request ()
					{
						RootEntity = example,
						RequestedEntity = example,
					};

					request.AddLocalConstraint (example,
						new ComparisonFieldValue (
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

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					CountryEntity example = new CountryEntity ();

					Request request = new Request ()
					{
						RootEntity = example,
						RequestedEntity = example,
					};

					string value = Constant.Escape ("test%test");

					request.AddLocalConstraint (example,
						new ComparisonFieldValue (
							new Field (new Druid ("[L0A3]")),
							BinaryComparator.IsLikeEscape,
							new Constant (value)
						)
					);

					CountryEntity[] countries = dataContext.GetByRequest<CountryEntity> (request).ToArray ();

					Assert.IsTrue (countries.Count () == 1);
					Assert.IsTrue (countries.Any (c => c.Name == "test%test"));
				}

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					CountryEntity example = new CountryEntity ();

					Request request = new Request ()
					{
						RootEntity = example,
						RequestedEntity = example,
					};

					string value = Constant.Escape ("test_test");

					request.AddLocalConstraint (example,
						new ComparisonFieldValue (
							new Field (new Druid ("[L0A3]")),
							BinaryComparator.IsLikeEscape,
							new Constant (value)
						)
					);

					CountryEntity[] countries = dataContext.GetByRequest<CountryEntity> (request).ToArray ();

					Assert.IsTrue (countries.Count () == 1);
					Assert.IsTrue (countries.Any (c => c.Name == "test_test"));
				}

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					CountryEntity example = new CountryEntity ();

					Request request = new Request ()
					{
						RootEntity = example,
						RequestedEntity = example,
					};

					string value = Constant.Escape ("test#test");

					request.AddLocalConstraint (example,
						new ComparisonFieldValue (
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
		}


		[TestMethod]
		public void RequestedEntityRequest1()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				dataInfrastructure.OpenConnection ("id");

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
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
		}


		[TestMethod]
		public void RequestedEntityRequest2()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				dataInfrastructure.OpenConnection ("id");

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
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
		}


		[TestMethod]
		public void RequestedEntityRequest3()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				dataInfrastructure.OpenConnection ("id");

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
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
		}


		[TestMethod]
		public void RootEntityReferenceRequest1()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				dataInfrastructure.OpenConnection ("id");

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					NaturalPersonEntity example = DatabaseCreator2.GetCorrectExample1 ();
					NaturalPersonEntity alfred = dataContext.GetByExample<NaturalPersonEntity> (example).First ();

					Request request = new Request ()
					{
						RootEntity = new NaturalPersonEntity (),
						RootEntityKey = dataContext.GetNormalizedEntityKey (alfred).Value.RowKey,
					};

					NaturalPersonEntity[] persons = dataContext.GetByRequest<NaturalPersonEntity> (request).ToArray ();

					Assert.IsTrue (persons.Length == 1);

					Assert.IsTrue (persons.Any (p => DatabaseCreator2.CheckAlfred (p)));
				}
			}
		}


		[TestMethod]
		public void RootEntityReferenceRequest2()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				dataInfrastructure.OpenConnection ("id");

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
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
						RootEntityKey = dataContext.GetNormalizedEntityKey (alfred1).Value.RowKey,
						RequestedEntity = (example2.Contacts[0] as UriContactEntity).UriScheme,
					};

					UriSchemeEntity[] uriSchemes = dataContext.GetByRequest<UriSchemeEntity> (request).ToArray ();

					Assert.IsTrue (uriSchemes.Length == 1);

					Assert.IsTrue (uriSchemes.Any (s => s.Code == "mailto:" && s.Name == "email"));
				}
			}
		}


		[TestMethod]
		public void RootEntityReferenceRequest3()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				dataInfrastructure.OpenConnection ("id");

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					NaturalPersonEntity example1 = DatabaseCreator2.GetCorrectExample1 ();
					NaturalPersonEntity alfred1 = dataContext.GetByExample<NaturalPersonEntity> (example1).First ();

					NaturalPersonEntity example2 = new NaturalPersonEntity ();
					example2.Contacts.Add (new AbstractContactEntity ());

					Request request = new Request ()
					{
						RootEntity = example2,
						RootEntityKey = dataContext.GetNormalizedEntityKey (alfred1).Value.RowKey,
						RequestedEntity = example2.Contacts[0],
					};

					UriContactEntity[] contacts = dataContext.GetByRequest<UriContactEntity> (request).ToArray ();

					Assert.IsTrue (contacts.Length == 2);

					Assert.IsTrue (contacts.Any (c => DatabaseCreator2.CheckUriContact (c, "alfred@coucou.com", "Alfred")));
					Assert.IsTrue (contacts.Any (c => DatabaseCreator2.CheckUriContact (c, "alfred@blabla.com", "Alfred")));
				}
			}
		}


		[TestMethod]
		public void IntRequest1()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				dataInfrastructure.OpenConnection ("id");

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					ContactRoleEntity example = new ContactRoleEntity ();

					Request request = new Request ()
					{
						RootEntity = example,
						RequestedEntity = example,
					};

					request.AddLocalConstraint (example,
						new ComparisonFieldValue (
							new Field (new Druid ("[L0A03]")),
							BinaryComparator.IsEqual,
							new Constant (1)
						)
					);

					var roles = dataContext.GetByRequest<ContactRoleEntity> (request).ToList ();

					Assert.IsTrue (roles.Count == 1);
					Assert.AreEqual (1, roles.First ().Rank);
				}
			}
		}


		[TestMethod]
		public void IntRequest2()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				dataInfrastructure.OpenConnection ("id");

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					ContactRoleEntity example = new ContactRoleEntity ();

					Request request = new Request ()
					{
						RootEntity = example,
						RequestedEntity = example,
					};

					request.AddLocalConstraint (example,
						new BinaryOperation (
							new ComparisonFieldValue (
								new Field (new Druid ("[L0A03]")),
								BinaryComparator.IsLower,
								new Constant (4)
							),
							BinaryOperator.And,
							new ComparisonFieldValue (
								new Field (new Druid ("[L0A03]")),
								BinaryComparator.IsGreaterOrEqual,
								new Constant (3)
							)
						)
					);

					var roles = dataContext.GetByRequest<ContactRoleEntity> (request).ToList ();

					Assert.IsTrue (roles.Count == 1);
					Assert.AreEqual (3, roles.First ().Rank);
				}
			}
		}


		[TestMethod]
		public void IntRequest3()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				dataInfrastructure.OpenConnection ("id");

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					ContactRoleEntity example = new ContactRoleEntity ();

					Request request = new Request ()
					{
						RootEntity = example,
						RequestedEntity = example,
					};

					request.AddLocalConstraint (example,
						new UnaryComparison (
							new Field (new Druid ("[L0A03]")),
							UnaryComparator.IsNull
						)
					);

					var roles = dataContext.GetByRequest<ContactRoleEntity> (request).ToList ();

					Assert.IsTrue (roles.Count == 3);
					Assert.IsTrue (roles.All (r => r.Rank == null));
				}
			}
		}


		[TestMethod]
		public void DateRequest1()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				dataInfrastructure.OpenConnection ("id");

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					NaturalPersonEntity example = new NaturalPersonEntity ();

					Request request = new Request ()
					{
						RootEntity = example,
						RequestedEntity = example,
					};

					request.AddLocalConstraint (example,
						new UnaryComparison (
							new Field (new Druid ("[L0A61]")),
							UnaryComparator.IsNotNull
						)
					);

					var persons = dataContext.GetByRequest<NaturalPersonEntity> (request).ToList ();

					Assert.IsTrue (persons.Count == 3);
					Assert.IsTrue (persons.Any (p => DatabaseCreator2.CheckAlfred (p)));
					Assert.IsTrue (persons.Any (p => DatabaseCreator2.CheckGertrude (p)));
					Assert.IsTrue (persons.Any (p => DatabaseCreator2.CheckHans (p)));
				}
			}
		}


		[TestMethod]
		public void DateRequest2()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				dataInfrastructure.OpenConnection ("id");

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					NaturalPersonEntity example = new NaturalPersonEntity ();

					Request request = new Request ()
					{
						RootEntity = example,
						RequestedEntity = example,
					};

					request.AddLocalConstraint (example,
						new ComparisonFieldValue (
							new Field (new Druid ("[L0A61]")),
							BinaryComparator.IsEqual,
							new Constant (new Date (1965, 5, 3))
						)
					);

					var persons = dataContext.GetByRequest<NaturalPersonEntity> (request).ToList ();

					Assert.IsTrue (persons.Count == 1);
					Assert.IsTrue (persons.Any (p => DatabaseCreator2.CheckGertrude (p)));
				}
			}
		}


	}


}
