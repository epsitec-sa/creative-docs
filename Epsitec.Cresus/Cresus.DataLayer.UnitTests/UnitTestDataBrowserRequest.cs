using Epsitec.Common.Support;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.DataLayer.Browser;
using Epsitec.Cresus.DataLayer.Expressions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Linq;


namespace Epsitec.Cresus.DataLayer
{


	[TestClass]
	public class UnitTestDataBrowserRequest
	{


		[ClassInitialize]
		public void Initialize(TestContext testContext)
		{
			TestHelper.Initialize ();
			Database.CreateAndConnectToDatabase ();
			Database2.PupulateDatabase (false);
		}


		[TestMethod]
		public void ValidEntityContainer()
		{
			TestHelper.PrintStartTest ("Valid Entity Container");

			NaturalPersonEntity person = new NaturalPersonEntity ();
			UriContactEntity uriContact = new UriContactEntity ();
			PersonTitleEntity title = new PersonTitleEntity ();

			person.Title = title;
			person.Contacts.Add (uriContact);

			Request request = new Request ();

			Assert.IsFalse (this.IsExceptionThrown (() =>
				request.AddLocalConstraint (person, new BinaryComparisonFieldWithField (new Field (new Druid ("[L0A01]")), BinaryComparator.IsEqual, new Field (new Druid ("[L0AV]"))))
			));

			Assert.IsFalse (this.IsExceptionThrown (() =>
				request.AddLocalConstraint (person, new BinaryComparisonFieldWithValue (new Field (new Druid ("[L0A61]")), BinaryComparator.IsEqual, new Constant (Type.Boolean, true)))
			));

			Assert.IsFalse (this.IsExceptionThrown (() =>
				request.AddLocalConstraint (uriContact, new UnaryComparison (new Field (new Druid ("[L0AA2]")), UnaryComparator.IsNull))
			));

			Assert.IsFalse (this.IsExceptionThrown (() =>
				request.AddLocalConstraint (title, new BinaryOperation (new UnaryComparison (new Field (new Druid ("[L0AT1]")), UnaryComparator.IsNotNull), BinaryOperator.Or, new UnaryComparison (new Field (new Druid ("[L0AS1]")), UnaryComparator.IsNotNull)))
			));
		}


		[TestMethod]
		public void InvalidEntityContainer()
		{
			TestHelper.PrintStartTest ("Invalid Entity Container");

			NaturalPersonEntity person = new NaturalPersonEntity ();
			UriContactEntity uriContact = new UriContactEntity ();
			PersonTitleEntity title = new PersonTitleEntity ();

			person.Title = title;
			person.Contacts.Add (uriContact);

			Request request = new Request ();

			Assert.IsTrue (this.IsExceptionThrown (() =>
				request.AddLocalConstraint (person, new BinaryComparisonFieldWithField (new Field (new Druid ("[L0AS1]")), BinaryComparator.IsEqual, new Field (new Druid ("[L0AV]"))))
			));

			Assert.IsTrue (this.IsExceptionThrown (() =>
				request.AddLocalConstraint (person, new BinaryComparisonFieldWithValue (new Field (new Druid ("[L0AS]")), BinaryComparator.IsEqual, new Constant (Type.Boolean, true)))
			));

			Assert.IsTrue (this.IsExceptionThrown (() =>
				request.AddLocalConstraint (uriContact, new UnaryComparison (new Field (new Druid ("[L0A92]")), UnaryComparator.IsNull))
			));

			Assert.IsTrue (this.IsExceptionThrown (() =>
				request.AddLocalConstraint (title, new BinaryOperation (new UnaryComparison (new Field (new Druid ("[L0A61]")), UnaryComparator.IsNotNull), BinaryOperator.Or, new UnaryComparison (new Field (new Druid ("[L0AS1]")), UnaryComparator.IsNotNull)))
			));
		}



		[TestMethod]
		public void UnaryComparisonTest()
		{
			TestHelper.PrintStartTest ("Unary comparison");

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure, false))
			{
				DataBrowser dataBrowser = new DataBrowser (dataContext);

				NaturalPersonEntity example = new NaturalPersonEntity ();

				Request request = new Request ();
				request.AddLocalConstraint (example,
					new UnaryComparison (
						new Field (new Druid ("[L0A01]")),
						UnaryComparator.IsNotNull
					)
				);

				NaturalPersonEntity[] persons = dataBrowser.GetByExample (example, request).ToArray ();

				Assert.IsTrue (persons.Count () == 3);
				Assert.IsTrue (persons.Any (p => Database2.CheckAlfred (p)));
				Assert.IsTrue (persons.Any (p => Database2.CheckGertrude (p)));
				Assert.IsTrue (persons.Any (p => Database2.CheckHans (p)));
			}
		}


		[TestMethod]
		public void BinaryComparisonFieldWithValueTest()
		{
			TestHelper.PrintStartTest ("Binary comparison field with value");

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure, false))
			{
				DataBrowser dataBrowser = new DataBrowser (dataContext);

				NaturalPersonEntity example = new NaturalPersonEntity ();

				Request request = new Request ();
				request.AddLocalConstraint (example,
					new BinaryComparisonFieldWithValue (
						new Field (new Druid ("[L0AV]")),
						BinaryComparator.IsEqual,
						new Constant (Type.String, "Alfred")
					)
				);

				NaturalPersonEntity[] persons = dataBrowser.GetByExample (example, request).ToArray ();

				Assert.IsTrue (persons.Count () == 1);
				Assert.IsTrue (persons.Any (p => Database2.CheckAlfred (p)));
			}
		}


		[TestMethod]
		public void BinaryComparisonFieldWithFieldTest()
		{
			TestHelper.PrintStartTest ("Binary comparison field with field");

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure, false))
			{
				DataBrowser dataBrowser = new DataBrowser (dataContext);

				NaturalPersonEntity example = new NaturalPersonEntity ();

				Request request = new Request ();
				request.AddLocalConstraint (example,
					new BinaryComparisonFieldWithField (
						new Field (new Druid ("[L0AV]")),
						BinaryComparator.IsEqual,
						new Field (new Druid ("[L0A01]"))
					)
				);

				NaturalPersonEntity[] persons = dataBrowser.GetByExample (example, request).ToArray ();

				Assert.IsTrue (persons.Count () == 0);
			}
		}


		[TestMethod]
		public void UnaryOperationTest()
		{
			TestHelper.PrintStartTest ("Unary operation");

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure, false))
			{
				DataBrowser dataBrowser = new DataBrowser (dataContext);

				NaturalPersonEntity example = new NaturalPersonEntity ();

				Request request = new Request ();
				request.AddLocalConstraint (example,
					new UnaryOperation (
						UnaryOperator.Not,
						new BinaryComparisonFieldWithValue (
							new Field (new Druid ("[L0AV]")),
							BinaryComparator.IsEqual,
							new Constant (Type.String, "Hans")
						)
					)
				);

				NaturalPersonEntity[] persons = dataBrowser.GetByExample (example, request).ToArray ();

				Assert.IsTrue (persons.Count () == 2);
				Assert.IsTrue (persons.Any (p => Database2.CheckAlfred (p)));
				Assert.IsTrue (persons.Any (p => Database2.CheckGertrude (p)));
			}
		}


		[TestMethod]
		public void BinaryOperationTest()
		{
			TestHelper.PrintStartTest ("Binary operation");

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure, false))
			{
				DataBrowser dataBrowser = new DataBrowser (dataContext);

				NaturalPersonEntity example = new NaturalPersonEntity ();

				Request request = new Request ();
				request.AddLocalConstraint (example,
					new BinaryOperation (
						new BinaryComparisonFieldWithValue (
							new Field (new Druid ("[L0AV]")),
							BinaryComparator.IsNotEqual,
							new Constant (Type.String, "Hans")
						),
						BinaryOperator.And,
						new BinaryComparisonFieldWithValue (
							new Field (new Druid ("[L0AV]")),
							BinaryComparator.IsNotEqual,
							new Constant (Type.String, "Gertrude")
						)
					)
				);

				NaturalPersonEntity[] persons = dataBrowser.GetByExample (example, request).ToArray ();

				Assert.IsTrue (persons.Count () == 1);
				Assert.IsTrue (persons.Any (p => Database2.CheckAlfred (p)));
			}
		}


		[TestMethod]
		public void DoubleRequest1()
		{
			TestHelper.PrintStartTest ("Double request 1");

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure, false))
			{
				DataBrowser dataBrowser = new DataBrowser (dataContext);

				NaturalPersonEntity example = new NaturalPersonEntity ();

				Request request = new Request ();

				request.AddLocalConstraint (example,
					new BinaryComparisonFieldWithValue (
						new Field (new Druid ("[L0AV]")),
						BinaryComparator.IsEqual,
						new Constant (Type.String, "Alfred")
					)
				);

				request.AddLocalConstraint (example,
					new BinaryComparisonFieldWithValue (
						new Field (new Druid ("[L0A01]")),
						BinaryComparator.IsEqual,
						new Constant (Type.String, "Dupond")
					)
				);

				NaturalPersonEntity[] persons = dataBrowser.GetByExample (example, request).ToArray ();

				Assert.IsTrue (persons.Count () == 1);
				Assert.IsTrue (persons.Any (p => Database2.CheckAlfred (p)));
			}
		}

		[TestMethod]
		public void DoubleRequest2()
		{
			TestHelper.PrintStartTest ("Double request 2");

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure, false))
			{
				DataBrowser dataBrowser = new DataBrowser (dataContext);

				NaturalPersonEntity example = new NaturalPersonEntity ()
				{
					Gender = new PersonGenderEntity (),
				};

				Request request = new Request ();

				request.AddLocalConstraint (example,
					new BinaryComparisonFieldWithValue (
						new Field (new Druid ("[L0AV]")),
						BinaryComparator.IsEqual,
						new Constant (Type.String, "Alfred")
					)
				);

				request.AddLocalConstraint (example.Gender,
					new BinaryComparisonFieldWithValue (
						new Field (new Druid ("[L0AC1]")),
						BinaryComparator.IsEqual,
						new Constant (Type.String, "Male")
					)
				);

				NaturalPersonEntity[] persons = dataBrowser.GetByExample (example, request).ToArray ();

				Assert.IsTrue (persons.Count () == 1);
				Assert.IsTrue (persons.Any (p => Database2.CheckAlfred (p)));
			}
		}


		[TestMethod]
		public void InnerRequest()
		{
			TestHelper.PrintStartTest ("Inner request");

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure, false))
			{
				DataBrowser dataBrowser = new DataBrowser (dataContext);

				NaturalPersonEntity example = new NaturalPersonEntity ()
				{
					Gender = new PersonGenderEntity (),
				};

				Request request = new Request ();

				request.AddLocalConstraint (example.Gender,
					new BinaryComparisonFieldWithValue (
						new Field (new Druid ("[L0AC1]")),
						BinaryComparator.IsEqual,
						new Constant (Type.String, "Male")
					)
				);

				NaturalPersonEntity[] persons = dataBrowser.GetByExample (example, request).ToArray ();

				Assert.IsTrue (persons.Count () == 1);
				Assert.IsTrue (persons.Any (p => Database2.CheckAlfred (p)));
			}
		}


		[TestMethod]
		public void LikeRequest()
		{
			TestHelper.PrintStartTest ("Like request");

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure, false))
			{
				DataBrowser dataBrowser = new DataBrowser (dataContext);

				NaturalPersonEntity example = new NaturalPersonEntity ()
				{
					Gender = new PersonGenderEntity (),
				};

				Request request = new Request ();

				request.AddLocalConstraint (example.Gender,
					new BinaryComparisonFieldWithValue (
						new Field (new Druid ("[L0AC1]")),
						BinaryComparator.IsLike,
						new Constant (Type.String, "%ale")
					)
				);

				NaturalPersonEntity[] persons = dataBrowser.GetByExample (example, request).ToArray ();

				Assert.IsTrue (persons.Count () == 2);
				Assert.IsTrue (persons.Any (p => Database2.CheckAlfred (p)));
				Assert.IsTrue (persons.Any (p => Database2.CheckGertrude (p)));
			}
		}


		[TestMethod]
		public void LikeEscapeRequest()
		{
			TestHelper.PrintStartTest ("Like escape request");

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure, false))
			{
				CountryEntity country1 = Database.CreateCountry (dataContext, "c1", "test%test");
				CountryEntity country2 = Database.CreateCountry (dataContext, "c2", "test_test");
				CountryEntity country3 = Database.CreateCountry (dataContext, "c2", "test#test");
				CountryEntity country4 = Database.CreateCountry (dataContext, "c3", "testxxtest");

				dataContext.SaveChanges ();
			}

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure, false))
			{
				DataBrowser dataBrowser = new DataBrowser (dataContext);

				CountryEntity example = new CountryEntity ();
				Request request = new Request ();

				request.AddLocalConstraint (example,
					new BinaryComparisonFieldWithValue (
						new Field (new Druid ("[L0A3]")),
						BinaryComparator.IsLike,
						new Constant (Type.String, "test%test")
					)
				);

				CountryEntity[] countries = dataBrowser.GetByExample (example, request).ToArray ();

				Assert.IsTrue (countries.Count () == 4);
				Assert.IsTrue (countries.Any (c => c.Name == "test%test"));
				Assert.IsTrue (countries.Any (c => c.Name == "test_test"));
				Assert.IsTrue (countries.Any (c => c.Name == "test#test"));
				Assert.IsTrue (countries.Any (c => c.Name == "testxxtest"));
			}

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure, false))
			{
				DataBrowser dataBrowser = new DataBrowser (dataContext);

				CountryEntity example = new CountryEntity ();
				Request request = new Request ();

				request.AddLocalConstraint (example,
					new BinaryComparisonFieldWithValue (
						new Field (new Druid ("[L0A3]")),
						BinaryComparator.IsLike,
						new Constant (Type.String, "test_test")
					)
				);

				CountryEntity[] countries = dataBrowser.GetByExample (example, request).ToArray ();

				Assert.IsTrue (countries.Count () == 3);
				Assert.IsTrue (countries.Any (c => c.Name == "test%test"));
				Assert.IsTrue (countries.Any (c => c.Name == "test_test"));
				Assert.IsTrue (countries.Any (c => c.Name == "test#test"));
			}

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure, false))
			{
				DataBrowser dataBrowser = new DataBrowser (dataContext);

				CountryEntity example = new CountryEntity ();
				Request request = new Request ();

				string value = BinaryComparisonFieldWithValue.Escape ("test%test");

				request.AddLocalConstraint (example,
					new BinaryComparisonFieldWithValue (
						new Field (new Druid ("[L0A3]")),
						BinaryComparator.IsLikeEscape,
						new Constant (Type.String, value)
					)
				);

				CountryEntity[] countries = dataBrowser.GetByExample (example, request).ToArray ();

				Assert.IsTrue (countries.Count () == 1);
				Assert.IsTrue (countries.Any (c => c.Name == "test%test"));
			}

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure, false))
			{
				DataBrowser dataBrowser = new DataBrowser (dataContext);

				CountryEntity example = new CountryEntity ();
				Request request = new Request ();

				string value = BinaryComparisonFieldWithValue.Escape ("test_test");

				request.AddLocalConstraint (example,
					new BinaryComparisonFieldWithValue (
						new Field (new Druid ("[L0A3]")),
						BinaryComparator.IsLikeEscape,
						new Constant (Type.String, value)
					)
				);

				CountryEntity[] countries = dataBrowser.GetByExample (example, request).ToArray ();

				Assert.IsTrue (countries.Count () == 1);
				Assert.IsTrue (countries.Any (c => c.Name == "test_test"));
			}

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure, false))
			{
				DataBrowser dataBrowser = new DataBrowser (dataContext);

				CountryEntity example = new CountryEntity ();
				Request request = new Request ();

				string value = BinaryComparisonFieldWithValue.Escape ("test#test");

				request.AddLocalConstraint (example,
					new BinaryComparisonFieldWithValue (
						new Field (new Druid ("[L0A3]")),
						BinaryComparator.IsLikeEscape,
						new Constant (Type.String, value)
					)
				);

				CountryEntity[] countries = dataBrowser.GetByExample (example, request).ToArray ();

				Assert.IsTrue (countries.Count () == 1);
				Assert.IsTrue (countries.Any (c => c.Name == "test#test"));
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
