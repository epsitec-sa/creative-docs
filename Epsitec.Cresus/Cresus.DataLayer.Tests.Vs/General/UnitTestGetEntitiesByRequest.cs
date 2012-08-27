//	Copyright © 2010-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Common.Support;

using Epsitec.Common.Types;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Expressions;
using Epsitec.Cresus.DataLayer.Infrastructure;
using Epsitec.Cresus.DataLayer.Loader;
using Epsitec.Cresus.DataLayer.Tests.Vs.Entities;
using Epsitec.Cresus.DataLayer.Tests.Vs.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Tests.Vs.General
{


	[TestClass]
	public sealed class UnitTestGetEntitiesByRequest
	{


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();

			DatabaseCreator2.ResetPopulatedTestDatabase ();

			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				for (int i = 0; i < 5; i++)
				{
					int? rank = (i % 2 == 0) ? (int?) null : i;

					DataContextHelper.CreateContactRole (dataContext, "role" + i, rank);
				}

				dataContext.SaveChanges ();
			}
		}


		[TestMethod]
		public void UnaryComparisonTest()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				NaturalPersonEntity example = new NaturalPersonEntity ();

				Request request = new Request ()
				{
					RootEntity = example,
				};

				request.Conditions.Add (
					new UnaryComparison (
						new ValueField (example, new Druid ("[J1AM1]")),
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
		public void BinaryComparisonTest1()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				NaturalPersonEntity example = new NaturalPersonEntity ();

				Request request = new Request ()
				{
					RootEntity = example,
				};

				request.Conditions.Add (
					new BinaryComparison (
						new ValueField (example, new Druid ("[J1AL1]")),
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
		public void BinaryComparisonTest2()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				NaturalPersonEntity example = new NaturalPersonEntity ();

				Request request = new Request ()
				{
					RootEntity = example,
				};

				request.Conditions.Add (
					new BinaryComparison (
						new ValueField (example, new Druid ("[J1AL1]")),
						BinaryComparator.IsEqual,
						new ValueField (example, new Druid ("[J1AM1]"))
					)
				);

				NaturalPersonEntity[] persons = dataContext.GetByRequest<NaturalPersonEntity> (request).ToArray ();

				Assert.IsTrue (persons.Count () == 0);
			}
		}


		[TestMethod]
		public void BinaryComparisonTest3()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				NaturalPersonEntity example = new NaturalPersonEntity ()
				{
					Gender = new PersonGenderEntity (),
				};

				Request request = new Request ()
				{
					RootEntity = example,
				};

				request.Conditions.Add (
					new BinaryComparison (
						new ValueField (example, new Druid ("[J1AL1]")),
						BinaryComparator.IsLowerOrEqual,
						new ValueField (example.Gender, new Druid ("[J1AR]"))
					)
				);

				NaturalPersonEntity[] persons = dataContext.GetByRequest<NaturalPersonEntity> (request).ToArray ();

				Assert.IsTrue (persons.Count () == 1);
				Assert.IsTrue (persons.Any (p => DatabaseCreator2.CheckAlfred (p)));
			}
		}


		[TestMethod]
		public void BinaryComparisonTest4()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				ValueDataEntity example = new ValueDataEntity ();

				Request request = new Request ()
				{
					RootEntity = example,
				};

				request.Conditions.Add (
					new BinaryComparison (
						new ValueField (example, new Druid ("[J1AD2]")),
						BinaryComparator.IsLowerOrEqual,
						InternalField.CreateId (example)
					)
				);

				ValueDataEntity[] data = dataContext.GetByRequest<ValueDataEntity> (request).ToArray ();

				Assert.IsTrue (data.Count () == 3);
				Assert.IsTrue (data.Any (d => DatabaseCreator2.CheckValueData1 (d)));
				Assert.IsTrue (data.Any (d => DatabaseCreator2.CheckValueData2 (d)));
				Assert.IsTrue (data.Any (d => DatabaseCreator2.CheckValueData3 (d)));
			}
		}


		[TestMethod]
		public void SetComparisonTest1()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				ValueDataEntity example = new ValueDataEntity ();

				Request request = new Request ()
				{
					RootEntity = example,
				};

				request.Conditions.Add (
					new SetComparison (
						new ValueField (example, new Druid ("[J1AD2]")),
						SetComparator.In,
						new List<Constant> ()
		                {
		                    new Constant (0),
		                    new Constant (42)
		                }
					)
				);

				ValueDataEntity[] data = dataContext.GetByRequest<ValueDataEntity> (request).ToArray ();

				Assert.IsTrue (data.Count () == 2);
				Assert.IsTrue (data.Any (d => DatabaseCreator2.CheckValueData1 (d)));
				Assert.IsTrue (data.Any (d => DatabaseCreator2.CheckValueData3 (d)));
			}
		}


		[TestMethod]
		public void SetComparisonTest2()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				ValueDataEntity example = new ValueDataEntity ();

				Request request = new Request ()
				{
					RootEntity = example,
				};

				request.Conditions.Add (
					new SetComparison (
						new ValueField (example, new Druid ("[J1AD2]")),
						SetComparator.NotIn,
						new List<Constant> ()
		                {
		                    new Constant (0),
		                    new Constant (42)
		                }
					)
				);

				ValueDataEntity[] data = dataContext.GetByRequest<ValueDataEntity> (request).ToArray ();

				Assert.IsTrue (data.Count () == 1);
				Assert.IsTrue (data.Any (d => DatabaseCreator2.CheckValueData2 (d)));
			}
		}


		[TestMethod]
		public void UnaryOperationTest()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				NaturalPersonEntity example = new NaturalPersonEntity ();

				Request request = new Request ()
				{
					RootEntity = example,
				};

				request.Conditions.Add (
					new UnaryOperation (
						UnaryOperator.Not,
						new BinaryComparison (
							new ValueField (example, new Druid ("[J1AL1]")),
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
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				NaturalPersonEntity example = new NaturalPersonEntity ();

				Request request = new Request ()
				{
					RootEntity = example,
				};

				request.Conditions.Add (
					new BinaryOperation (
						new BinaryComparison (
							new ValueField (example, new Druid ("[J1AL1]")),
							BinaryComparator.IsNotEqual,
							new Constant ("Hans")
						),
						BinaryOperator.And,
						new BinaryComparison (
							new ValueField (example, new Druid ("[J1AL1]")),
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
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				NaturalPersonEntity example = new NaturalPersonEntity ();

				Request request = new Request ()
				{
					RootEntity = example,
				};

				request.Conditions.Add (
					new BinaryComparison (
						new ValueField (example, new Druid ("[J1AL1]")),
						BinaryComparator.IsEqual,
						new Constant ("Alfred")
					)
				);

				request.Conditions.Add (
					new BinaryComparison (
						new ValueField (example, new Druid ("[J1AM1]")),
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
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				NaturalPersonEntity example = new NaturalPersonEntity ()
				{
					Gender = new PersonGenderEntity (),
				};

				Request request = new Request ()
				{
					RootEntity = example,
				};

				request.Conditions.Add (
					new BinaryComparison (
						new ValueField (example, new Druid ("[J1AL1]")),
						BinaryComparator.IsEqual,
						new Constant ("Alfred")
					)
				);

				request.Conditions.Add (
					new BinaryComparison (
						new ValueField (example.Gender, new Druid ("[J1AR]")),
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
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				NaturalPersonEntity example = new NaturalPersonEntity ()
				{
					Gender = new PersonGenderEntity (),
				};

				Request request = new Request ()
				{
					RootEntity = example,
				};

				request.Conditions.Add (
					new BinaryComparison (
						new ValueField (example.Gender, new Druid ("[J1AR]")),
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
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				NaturalPersonEntity example = new NaturalPersonEntity ()
				{
					Gender = new PersonGenderEntity (),
				};

				Request request = new Request ()
				{
					RootEntity = example,
				};

				request.Conditions.Add (
					new BinaryComparison (
						new ValueField (example.Gender, new Druid ("[J1AR]")),
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
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			{
				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				{
					CountryEntity country1 = DataContextHelper.CreateCountry (dataContext, "c1", "test%test");
					CountryEntity country2 = DataContextHelper.CreateCountry (dataContext, "c2", "test_test");
					CountryEntity country3 = DataContextHelper.CreateCountry (dataContext, "c2", "test#test");
					CountryEntity country4 = DataContextHelper.CreateCountry (dataContext, "c3", "testxxtest");

					dataContext.SaveChanges ();
				}

				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				{
					CountryEntity example = new CountryEntity ();

					Request request = new Request ()
					{
						RootEntity = example,
					};

					request.Conditions.Add (
						new BinaryComparison (
							new ValueField (example, new Druid ("[J1A5]")),
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

				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				{
					CountryEntity example = new CountryEntity ();

					Request request = new Request ()
					{
						RootEntity = example,
					};

					request.Conditions.Add (
						new BinaryComparison (
							new ValueField (example, new Druid ("[J1A5]")),
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

				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				{
					CountryEntity example = new CountryEntity ();

					Request request = new Request ()
					{
						RootEntity = example,
					};

					string value = Constant.Escape ("test%test");

					request.Conditions.Add (
						new BinaryComparison (
							new ValueField (example, new Druid ("[J1A5]")),
							BinaryComparator.IsLikeEscape,
							new Constant (value)
						)
					);

					CountryEntity[] countries = dataContext.GetByRequest<CountryEntity> (request).ToArray ();

					Assert.IsTrue (countries.Count () == 1);
					Assert.IsTrue (countries.Any (c => c.Name == "test%test"));
				}

				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				{
					CountryEntity example = new CountryEntity ();

					Request request = new Request ()
					{
						RootEntity = example,	
					};

					string value = Constant.Escape ("test_test");

					request.Conditions.Add (
						new BinaryComparison (
							new ValueField (example, new Druid ("[J1A5]")),
							BinaryComparator.IsLikeEscape,
							new Constant (value)
						)
					);

					CountryEntity[] countries = dataContext.GetByRequest<CountryEntity> (request).ToArray ();

					Assert.IsTrue (countries.Count () == 1);
					Assert.IsTrue (countries.Any (c => c.Name == "test_test"));
				}

				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				{
					CountryEntity example = new CountryEntity ();

					Request request = new Request ()
					{
						RootEntity = example,
					};

					string value = Constant.Escape ("test#test");

					request.Conditions.Add (
						new BinaryComparison (
							new ValueField (example, new Druid ("[J1A5]")),
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
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
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
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
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
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
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
		public void GetObjectBasedOnBooleanField1()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				ValueDataEntity example = new ValueDataEntity ();

				Request request = new Request ()
				{
					RootEntity = example,
				};

				request.Conditions.Add (
					new BinaryComparison (
						new ValueField (example, new Druid ("[J1A82]")),
						BinaryComparator.IsEqual,
						new Constant (true)
					)
				);

				var valueData = dataContext.GetByRequest<ValueDataEntity> (request).ToList ();

				Assert.IsTrue (valueData.Count () == 2);

				Assert.IsTrue (valueData.Any (vd => DatabaseCreator2.CheckValueData1 (vd)));
				Assert.IsTrue (valueData.Any (vd => DatabaseCreator2.CheckValueData3 (vd)));
			}
		}


		[TestMethod]
		public void GetObjectBasedOnBooleanField2()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				ValueDataEntity example = new ValueDataEntity ();

				Request request = new Request ()
				{
					RootEntity = example,
				};

				request.Conditions.Add (
					new BinaryComparison (
						new ValueField (example, new Druid ("[J1A82]")),
						BinaryComparator.IsNotEqual,
						new Constant (false)
					)
				);

				var valueData = dataContext.GetByRequest<ValueDataEntity> (request).ToList ();

				Assert.IsTrue (valueData.Count () == 2);

				Assert.IsTrue (valueData.Any (vd => DatabaseCreator2.CheckValueData1 (vd)));
				Assert.IsTrue (valueData.Any (vd => DatabaseCreator2.CheckValueData3 (vd)));
			}
		}


		[TestMethod]
		public void GetObjectBasedOnByteArrayField1()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				ValueDataEntity example = new ValueDataEntity ();

				Request request = new Request ()
				{
					RootEntity = example,
				};

				request.Conditions.Add (
					new BinaryComparison (
						new ValueField (example, new Druid ("[J1A92]")),
						BinaryComparator.IsEqual,
						new Constant (new byte[] { 0x0F, 0xF0 })
					)
				);

				var valueData = dataContext.GetByRequest<ValueDataEntity> (request).ToList ();

				Assert.IsTrue (valueData.Count () == 1);

				Assert.IsTrue (valueData.Any (vd => DatabaseCreator2.CheckValueData1 (vd)));
			}
		}


		[TestMethod]
		public void GetObjectBasedOnByteArrayField2()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				ValueDataEntity example = new ValueDataEntity ();

				Request request = new Request ()
				{
					RootEntity = example,
				};

				request.Conditions.Add (
					new BinaryComparison (
						new ValueField (example, new Druid ("[J1A92]")),
						BinaryComparator.IsNotEqual,
						new Constant (new byte[] { 0x0F, 0xF0 })
					)
				);

				var valueData = dataContext.GetByRequest<ValueDataEntity> (request).ToList ();

				Assert.IsTrue (valueData.Count () == 2);

				Assert.IsTrue (valueData.Any (vd => DatabaseCreator2.CheckValueData2 (vd)));
				Assert.IsTrue (valueData.Any (vd => DatabaseCreator2.CheckValueData3 (vd)));
			}
		}


		[TestMethod]
		public void GetObjectBasedOnDateTimeField1()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				ValueDataEntity example = new ValueDataEntity ();

				Request request = new Request ()
				{
					RootEntity = example,
				};

				request.Conditions.Add (
					new BinaryComparison (
						new ValueField (example, new Druid ("[J1AA2]")),
						BinaryComparator.IsEqual,
						new Constant (new System.DateTime (1969, 7, 21, 4, 17, 0))
					)
				);

				var valueData = dataContext.GetByRequest<ValueDataEntity> (request).ToList ();

				Assert.IsTrue (valueData.Count () == 1);

				Assert.IsTrue (valueData.Any (vd => DatabaseCreator2.CheckValueData1 (vd)));
			}
		}


		[TestMethod]
		public void GetObjectBasedOnDateTimeField2()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				ValueDataEntity example = new ValueDataEntity ();

				Request request = new Request ()
				{
					RootEntity = example,
				};

				request.Conditions.Add (
					new BinaryComparison (
						new ValueField (example, new Druid ("[J1AA2]")),
						BinaryComparator.IsNotEqual,
						new Constant (new System.DateTime (1969, 7, 21, 4, 17, 0))
					)
				);

				var valueData = dataContext.GetByRequest<ValueDataEntity> (request).ToList ();

				Assert.IsTrue (valueData.Count () == 2);

				Assert.IsTrue (valueData.Any (vd => DatabaseCreator2.CheckValueData2 (vd)));
				Assert.IsTrue (valueData.Any (vd => DatabaseCreator2.CheckValueData3 (vd)));
			}
		}


		[TestMethod]
		public void GetObjectBasedOnDateTimeField3()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				ValueDataEntity example = new ValueDataEntity ();

				Request request = new Request ()
				{
					RootEntity = example,
				};

				request.Conditions.Add (
					new BinaryComparison (
						new ValueField (example, new Druid ("[J1AA2]")),
						BinaryComparator.IsGreater,
						new Constant (new System.DateTime (1969, 7, 21, 4, 17, 0))
					)
				);

				var valueData = dataContext.GetByRequest<ValueDataEntity> (request).ToList ();

				Assert.IsTrue (valueData.Count () == 2);

				Assert.IsTrue (valueData.Any (vd => DatabaseCreator2.CheckValueData2 (vd)));
				Assert.IsTrue (valueData.Any (vd => DatabaseCreator2.CheckValueData3 (vd)));
			}
		}


		[TestMethod]
		public void GetObjectBasedOnDateField1()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				ValueDataEntity example = new ValueDataEntity ();

				Request request = new Request ()
				{
					RootEntity = example,
				};

				request.Conditions.Add (
					new BinaryComparison (
						new ValueField (example, new Druid ("[J1AB2]")),
						BinaryComparator.IsEqual,
						new Constant (new Date (1291, 8, 1))
					)
				);

				var valueData = dataContext.GetByRequest<ValueDataEntity> (request).ToList ();

				Assert.IsTrue (valueData.Count () == 1);

				Assert.IsTrue (valueData.Any (vd => DatabaseCreator2.CheckValueData1 (vd)));
			}
		}


		[TestMethod]
		public void GetObjectBasedOnDateField2()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				ValueDataEntity example = new ValueDataEntity ();

				Request request = new Request ()
				{
					RootEntity = example,
				};

				request.Conditions.Add (
					new BinaryComparison (
						new ValueField (example, new Druid ("[J1AB2]")),
						BinaryComparator.IsNotEqual,
						new Constant (new Date (1291, 8, 1))
					)
				);

				var valueData = dataContext.GetByRequest<ValueDataEntity> (request).ToList ();

				Assert.IsTrue (valueData.Count () == 2);

				Assert.IsTrue (valueData.Any (vd => DatabaseCreator2.CheckValueData2 (vd)));
				Assert.IsTrue (valueData.Any (vd => DatabaseCreator2.CheckValueData3 (vd)));
			}
		}


		[TestMethod]
		public void GetObjectBasedOnDateField3()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				ValueDataEntity example = new ValueDataEntity ();

				Request request = new Request ()
				{
					RootEntity = example,
				};

				request.Conditions.Add (
					new BinaryComparison (
						new ValueField (example, new Druid ("[J1AB2]")),
						BinaryComparator.IsGreater,
						new Constant (new Date (1291, 8, 1))
					)
				);

				var valueData = dataContext.GetByRequest<ValueDataEntity> (request).ToList ();

				Assert.IsTrue (valueData.Count () == 2);

				Assert.IsTrue (valueData.Any (vd => DatabaseCreator2.CheckValueData2 (vd)));
				Assert.IsTrue (valueData.Any (vd => DatabaseCreator2.CheckValueData3 (vd)));
			}
		}


		[TestMethod]
		public void GetObjectBasedOnDecimalField1()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				ValueDataEntity example = new ValueDataEntity ();

				Request request = new Request ()
				{
					RootEntity = example,
				};

				request.Conditions.Add (
					new BinaryComparison (
						new ValueField (example, new Druid ("[J1AC2]")),
						BinaryComparator.IsEqual,
						new Constant (123.456m)
					)
				);

				var valueData = dataContext.GetByRequest<ValueDataEntity> (request).ToList ();

				Assert.IsTrue (valueData.Count () == 1);

				Assert.IsTrue (valueData.Any (vd => DatabaseCreator2.CheckValueData1 (vd)));
			}
		}


		[TestMethod]
		public void GetObjectBasedOnDecimalField2()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				ValueDataEntity example = new ValueDataEntity ();

				Request request = new Request ()
				{
					RootEntity = example,
				};

				request.Conditions.Add (
					new BinaryComparison (
						new ValueField (example, new Druid ("[J1AC2]")),
						BinaryComparator.IsNotEqual,
						new Constant (123.456m)
					)
				);

				var valueData = dataContext.GetByRequest<ValueDataEntity> (request).ToList ();

				Assert.IsTrue (valueData.Count () == 2);

				Assert.IsTrue (valueData.Any (vd => DatabaseCreator2.CheckValueData2 (vd)));
				Assert.IsTrue (valueData.Any (vd => DatabaseCreator2.CheckValueData3 (vd)));
			}
		}


		[TestMethod]
		public void GetObjectBasedOnDecimalField3()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				ValueDataEntity example = new ValueDataEntity ();

				Request request = new Request ()
				{
					RootEntity = example,
				};

				request.Conditions.Add (
					new BinaryComparison (
						new ValueField (example, new Druid ("[J1AC2]")),
						BinaryComparator.IsGreater,
						new Constant (123.456m)
					)
				);

				var valueData = dataContext.GetByRequest<ValueDataEntity> (request).ToList ();

				Assert.IsTrue (valueData.Count () == 1);

				Assert.IsTrue (valueData.Any (vd => DatabaseCreator2.CheckValueData2 (vd)));
			}
		}


		[TestMethod]
		public void GetObjectBasedOnEnumField1()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				ValueDataEntity example = new ValueDataEntity ();

				Request request = new Request ()
				{
					RootEntity = example,
				};

				request.Conditions.Add (
					new BinaryComparison (
						new ValueField (example, new Druid ("[J1AL2]")),
						BinaryComparator.IsEqual,
						new Constant (SimpleEnum.Value2)
					)
				);

				var valueData = dataContext.GetByRequest<ValueDataEntity> (request).ToList ();

				Assert.IsTrue (valueData.Count () == 1);

				Assert.IsTrue (valueData.Any (vd => DatabaseCreator2.CheckValueData2 (vd)));
			}
		}


		[TestMethod]
		public void GetObjectBasedOnEnumField2()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				ValueDataEntity example = new ValueDataEntity ();

				Request request = new Request ()
				{
					RootEntity = example,
				};

				request.Conditions.Add (
					new BinaryComparison (
						new ValueField (example, new Druid ("[J1AL2]")),
						BinaryComparator.IsNotEqual,
						new Constant (SimpleEnum.Value2)
					)
				);

				var valueData = dataContext.GetByRequest<ValueDataEntity> (request).ToList ();

				Assert.IsTrue (valueData.Count () == 2);

				Assert.IsTrue (valueData.Any (vd => DatabaseCreator2.CheckValueData1 (vd)));
				Assert.IsTrue (valueData.Any (vd => DatabaseCreator2.CheckValueData3 (vd)));
			}
		}


		[TestMethod]
		public void GetObjectBasedOnIntegerField1()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				ValueDataEntity example = new ValueDataEntity ();

				Request request = new Request ()
				{
					RootEntity = example,
				};

				request.Conditions.Add (
					new BinaryComparison (
						new ValueField (example, new Druid ("[J1AD2]")),
						BinaryComparator.IsEqual,
						new Constant (42)
					)
				);

				var valueData = dataContext.GetByRequest<ValueDataEntity> (request).ToList ();

				Assert.IsTrue (valueData.Count () == 1);

				Assert.IsTrue (valueData.Any (vd => DatabaseCreator2.CheckValueData1 (vd)));
			}
		}


		[TestMethod]
		public void GetObjectBasedOnIntegerField2()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				ValueDataEntity example = new ValueDataEntity ();

				Request request = new Request ()
				{
					RootEntity = example,
				};

				request.Conditions.Add (
					new BinaryComparison (
						new ValueField (example, new Druid ("[J1AD2]")),
						BinaryComparator.IsNotEqual,
						new Constant (42)
					)
				);

				var valueData = dataContext.GetByRequest<ValueDataEntity> (request).ToList ();

				Assert.IsTrue (valueData.Count () == 2);

				Assert.IsTrue (valueData.Any (vd => DatabaseCreator2.CheckValueData2 (vd)));
				Assert.IsTrue (valueData.Any (vd => DatabaseCreator2.CheckValueData3 (vd)));
			}
		}


		[TestMethod]
		public void GetObjectBasedOnIntegerField3()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				ValueDataEntity example = new ValueDataEntity ();

				Request request = new Request ()
				{
					RootEntity = example,
				};

				request.Conditions.Add (
					new BinaryComparison (
						new ValueField (example, new Druid ("[J1AD2]")),
						BinaryComparator.IsLower,
						new Constant (42)
					)
				);

				var valueData = dataContext.GetByRequest<ValueDataEntity> (request).ToList ();

				Assert.IsTrue (valueData.Count () == 1);

				Assert.IsTrue (valueData.Any (vd => DatabaseCreator2.CheckValueData3 (vd)));
			}
		}


		[TestMethod]
		public void GetObjectBasedOnLongIntegerField1()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				ValueDataEntity example = new ValueDataEntity ();

				Request request = new Request ()
				{
					RootEntity = example,
				};

				request.Conditions.Add (
					new BinaryComparison (
						new ValueField (example, new Druid ("[J1AE2]")),
						BinaryComparator.IsEqual,
						new Constant (4242)
					)
				);

				var valueData = dataContext.GetByRequest<ValueDataEntity> (request).ToList ();

				Assert.IsTrue (valueData.Count () == 1);

				Assert.IsTrue (valueData.Any (vd => DatabaseCreator2.CheckValueData1 (vd)));
			}
		}


		[TestMethod]
		public void GetObjectBasedOnLongIntegerField2()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				ValueDataEntity example = new ValueDataEntity ();

				Request request = new Request ()
				{
					RootEntity = example,
				};

				request.Conditions.Add (
					new BinaryComparison (
						new ValueField (example, new Druid ("[J1AE2]")),
						BinaryComparator.IsNotEqual,
						new Constant (4242)
					)
				);

				var valueData = dataContext.GetByRequest<ValueDataEntity> (request).ToList ();

				Assert.IsTrue (valueData.Count () == 2);

				Assert.IsTrue (valueData.Any (vd => DatabaseCreator2.CheckValueData2 (vd)));
				Assert.IsTrue (valueData.Any (vd => DatabaseCreator2.CheckValueData3 (vd)));
			}
		}


		[TestMethod]
		public void GetObjectBasedOnLongIntegerField3()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				ValueDataEntity example = new ValueDataEntity ();

				Request request = new Request ()
				{
					RootEntity = example,
				};

				request.Conditions.Add (
					new BinaryComparison (
						new ValueField (example, new Druid ("[J1AE2]")),
						BinaryComparator.IsLower,
						new Constant (4242)
					)
				);

				var valueData = dataContext.GetByRequest<ValueDataEntity> (request).ToList ();

				Assert.IsTrue (valueData.Count () == 2);

				Assert.IsTrue (valueData.Any (vd => DatabaseCreator2.CheckValueData2 (vd)));
				Assert.IsTrue (valueData.Any (vd => DatabaseCreator2.CheckValueData3 (vd)));
			}
		}


		[TestMethod]
		public void GetObjectBasedOnStringField1()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				ValueDataEntity example = new ValueDataEntity ();

				Request request = new Request ()
				{
					RootEntity = example,
				};

				request.Conditions.Add (
					new BinaryComparison (
						new ValueField (example, new Druid ("[J1AF2]")),
						BinaryComparator.IsEqual,
						new Constant ("blupi")
					)
				);

				var valueData = dataContext.GetByRequest<ValueDataEntity> (request).ToList ();

				Assert.IsTrue (valueData.Count () == 1);

				Assert.IsTrue (valueData.Any (vd => DatabaseCreator2.CheckValueData1 (vd)));
			}
		}


		[TestMethod]
		public void GetObjectBasedOnStringField2()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				ValueDataEntity example = new ValueDataEntity ();

				Request request = new Request ()
				{
					RootEntity = example,
				};

				request.Conditions.Add (
					new BinaryComparison (
						new ValueField (example, new Druid ("[J1AF2]")),
						BinaryComparator.IsNotEqual,
						new Constant ("blupi")
					)
				);

				var valueData = dataContext.GetByRequest<ValueDataEntity> (request).ToList ();

				Assert.IsTrue (valueData.Count () == 2);

				Assert.IsTrue (valueData.Any (vd => DatabaseCreator2.CheckValueData2 (vd)));
				Assert.IsTrue (valueData.Any (vd => DatabaseCreator2.CheckValueData3 (vd)));
			}
		}


		[TestMethod]
		public void GetObjectBasedOnStringField3()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				ValueDataEntity example = new ValueDataEntity ();

				Request request = new Request ()
				{
					RootEntity = example,
				};

				request.Conditions.Add (
					new BinaryComparison (
						new ValueField (example, new Druid ("[J1AF2]")),
						BinaryComparator.IsLower,
						new Constant ("blupi")
					)
				);

				var valueData = dataContext.GetByRequest<ValueDataEntity> (request).ToList ();

				Assert.IsTrue (valueData.Count () == 1);

				Assert.IsTrue (valueData.Any (vd => DatabaseCreator2.CheckValueData3 (vd)));
			}
		}


		[TestMethod]
		public void GetObjectBasedOnTimeField1()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				ValueDataEntity example = new ValueDataEntity ();

				Request request = new Request ()
				{
					RootEntity = example,
				};

				request.Conditions.Add (
					new BinaryComparison (
						new ValueField (example, new Druid ("[J1AG2]")),
						BinaryComparator.IsEqual,
						new Constant (new Time (12, 12, 12))
					)
				);

				var valueData = dataContext.GetByRequest<ValueDataEntity> (request).ToList ();

				Assert.IsTrue (valueData.Count () == 1);

				Assert.IsTrue (valueData.Any (vd => DatabaseCreator2.CheckValueData1 (vd)));
			}
		}


		[TestMethod]
		public void GetObjectBasedOnTimeField2()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				ValueDataEntity example = new ValueDataEntity ();

				Request request = new Request ()
				{
					RootEntity = example,
				};

				request.Conditions.Add (
					new BinaryComparison (
						new ValueField (example, new Druid ("[J1AG2]")),
						BinaryComparator.IsNotEqual,
						new Constant (new Time (12, 12, 12))
					)
				);

				var valueData = dataContext.GetByRequest<ValueDataEntity> (request).ToList ();

				Assert.IsTrue (valueData.Count () == 2);

				Assert.IsTrue (valueData.Any (vd => DatabaseCreator2.CheckValueData2 (vd)));
				Assert.IsTrue (valueData.Any (vd => DatabaseCreator2.CheckValueData3 (vd)));
			}
		}


		[TestMethod]
		public void GetObjectBasedOnTimeField3()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				ValueDataEntity example = new ValueDataEntity ();

				Request request = new Request ()
				{
					RootEntity = example,
				};

				request.Conditions.Add (
					new BinaryComparison (
						new ValueField (example, new Druid ("[J1AG2]")),
						BinaryComparator.IsLower,
						new Constant (new Time (12, 12, 12))
					)
				);

				var valueData = dataContext.GetByRequest<ValueDataEntity> (request).ToList ();

				Assert.IsTrue (valueData.Count () == 2);

				Assert.IsTrue (valueData.Any (vd => DatabaseCreator2.CheckValueData2 (vd)));
				Assert.IsTrue (valueData.Any (vd => DatabaseCreator2.CheckValueData3 (vd)));
			}
		}


		[TestMethod]
		public void GetSortedObjects1()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				NaturalPersonEntity example = new NaturalPersonEntity ();

				Request request = new Request ()
				{
					RootEntity = example,
				};

				request.SortClauses.Add (
					new SortClause (
						new ValueField (example, new Druid ("[J1AL1]")),
						SortOrder.Ascending
					)
				);

				var result = dataContext.GetByRequest<NaturalPersonEntity> (request).ToList ();

				Assert.IsTrue (result.Count () == 3);

				Assert.IsTrue (DatabaseCreator2.CheckAlfred (result[0]));
				Assert.IsTrue (DatabaseCreator2.CheckGertrude (result[1]));
				Assert.IsTrue (DatabaseCreator2.CheckHans (result[2]));
			}
		}


		[TestMethod]
		public void GetSortedObjects2()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				NaturalPersonEntity example = new NaturalPersonEntity ();

				Request request = new Request ()
				{
					RootEntity = example,
				};

				request.SortClauses.Add (
					new SortClause (
						new ValueField (example, new Druid ("[J1AL1]")),
						SortOrder.Descending
					)
				);

				var result = dataContext.GetByRequest<NaturalPersonEntity> (request).ToList ();

				Assert.IsTrue (result.Count () == 3);

				Assert.IsTrue (DatabaseCreator2.CheckHans (result[0]));
				Assert.IsTrue (DatabaseCreator2.CheckGertrude (result[1]));
				Assert.IsTrue (DatabaseCreator2.CheckAlfred (result[2]));
			}
		}


		[TestMethod]
		public void GetSortedObjects3()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				NaturalPersonEntity example = new NaturalPersonEntity ()
				{
					PreferredLanguage = new LanguageEntity (),
				};

				Request request = new Request ()
				{
					RootEntity = example,
				};

				request.SortClauses.Add (
					new SortClause (
						new ValueField (example.PreferredLanguage, new Druid ("[J1AU]")),
						SortOrder.Descending
					)
				);

				var result = dataContext.GetByRequest<NaturalPersonEntity> (request).ToList ();

				Assert.IsTrue (result.Count () == 3);

				Assert.IsTrue (DatabaseCreator2.CheckHans (result[0]));
				Assert.IsTrue (DatabaseCreator2.CheckAlfred (result[1]));
				Assert.IsTrue (DatabaseCreator2.CheckGertrude (result[2]));
			}
		}


		[TestMethod]
		public void GetSortedObjects4()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				NaturalPersonEntity example = new NaturalPersonEntity ()
				{
					PreferredLanguage = new LanguageEntity (),
				};

				Request request = new Request ()
				{
					RootEntity = example,
				};

				request.SortClauses.Add (
					new SortClause (
						new ValueField (example, new Druid ("[J1AL1]")),
						SortOrder.Ascending
					)
				);

				request.SortClauses.Add (
					new SortClause (
						new ValueField (example.PreferredLanguage, new Druid ("[J1AU]")),
						SortOrder.Descending
					)
				);

				var result = dataContext.GetByRequest<NaturalPersonEntity> (request).ToList ();

				Assert.IsTrue (result.Count () == 3);

				Assert.IsTrue (DatabaseCreator2.CheckAlfred (result[0]));
				Assert.IsTrue (DatabaseCreator2.CheckGertrude (result[1]));
				Assert.IsTrue (DatabaseCreator2.CheckHans (result[2]));
			}
		}


		[TestMethod]
		public void GetSortedObjects5()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				NaturalPersonEntity example = new NaturalPersonEntity ()
				{
					Gender = new PersonGenderEntity (),
				};

				Request request = new Request ()
				{
					RootEntity = example,
				};

				request.Conditions.Add (
					new UnaryOperation (
						UnaryOperator.Not,
						new BinaryComparison (
							ValueField.Create (example, x => x.Firstname),
							BinaryComparator.IsEqual,
							new Constant ("Hans")
						)
					)
				);

				request.SortClauses.Add (
					new SortClause (
						ValueField.Create (example, x => x.BirthDate),
						SortOrder.Descending
					)
				);

				var result = dataContext.GetByRequest<NaturalPersonEntity> (request).ToList ();

				Assert.IsTrue (result.Count () == 2);

				Assert.IsTrue (DatabaseCreator2.CheckGertrude (result[0]));
				Assert.IsTrue (DatabaseCreator2.CheckAlfred (result[1]));
			}
		}


		[TestMethod]
		public void GetSortedObjects6()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				NaturalPersonEntity example = new NaturalPersonEntity ();

				var request = new Request ()
				{
					RootEntity = example
				};

				request.SortClauses.Add (
					new SortClause (
						InternalField.CreateId (example),
						SortOrder.Descending
					)
				);

				var result = dataContext.GetByRequest<NaturalPersonEntity> (request).ToList ();

				Assert.IsTrue (result.Count () == 3);

				Assert.IsTrue (DatabaseCreator2.CheckHans (result[0]));
				Assert.IsTrue (DatabaseCreator2.CheckGertrude (result[1]));
				Assert.IsTrue (DatabaseCreator2.CheckAlfred (result[2]));
			}
		}


		[TestMethod]
		public void GetObjectsWithSkipAndTake1()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				NaturalPersonEntity example = new NaturalPersonEntity ();

				Request request = new Request ()
				{
					RootEntity = example,
				};

				request.SortClauses.Add (
					new SortClause (
						new ValueField (example, new Druid ("[J1AL1]")),
						SortOrder.Descending
					)
				);

				var result1 = dataContext
					.GetByRequest<NaturalPersonEntity> (request)
					.ToList ();

				var data = Enumerable.Range (0, 4).Cast<int?> ().Concat (new List<int?> () { null });

				foreach (var skip in data)
				{
					foreach (var take in data)
					{
						request.Skip = skip;
						request.Take = take;

						var result2 = dataContext
							.GetByRequest<NaturalPersonEntity> (request)
							.ToList ();

						var expected = result1.Skip (skip ?? 0).Take (take ?? 4).ToList ();

						CollectionAssert.AreEqual (expected, result2);
					}
				}
			}
		}


		[TestMethod]
		public void GetObjectsWithOrderBySkipAndTake()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				NaturalPersonEntity example = new NaturalPersonEntity ()
				{
					Gender = new PersonGenderEntity (),
				};

				Request request = new Request ()
				{
					RootEntity = example,
					Skip = 1,
					Take = 1,
				};

				request.SortClauses.Add (
					new SortClause (
						new ValueField (example.Gender, new Druid ("[J1AR]")),
						SortOrder.Ascending
					)
				);

				var result = dataContext
					.GetByRequest<NaturalPersonEntity> (request)
					.ToList ();

				Assert.AreEqual (1, result.Count);
				Assert.IsTrue (DatabaseCreator2.CheckGertrude (result[0]));
			}
		}


		[TestMethod]
		public void ValueFieldTest()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				NaturalPersonEntity example = new NaturalPersonEntity ();

				Request request = new Request ()
				{
					RootEntity = example,
				};

				request.Conditions.Add (
					new BinaryComparison (
						ValueField.Create (example, p => p.Firstname),
						BinaryComparator.IsEqual,
						new Constant ("Alfred")
					)
				);

				NaturalPersonEntity[] persons = dataContext.GetByRequest<NaturalPersonEntity> (request).ToArray ();

				Assert.IsTrue (persons.Count () == 1);
				Assert.IsTrue (DatabaseCreator2.CheckAlfred (persons[0]));
			}
		}


		[TestMethod]
		public void ReferenceFieldTest()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				NaturalPersonEntity example = new NaturalPersonEntity ();

				Request request = new Request ()
				{
					RootEntity = example,
				};

				request.Conditions.Add (
					new BinaryComparison (
						ReferenceField.Create (example, p => p.Gender),
						BinaryComparator.IsEqual,
						new Constant ((long) 1000000001)
					)
				);

				NaturalPersonEntity[] persons = dataContext.GetByRequest<NaturalPersonEntity> (request).ToArray ();

				Assert.IsTrue (persons.Count () == 1);
				Assert.IsTrue (DatabaseCreator2.CheckAlfred (persons[0]));
			}
		}


		[TestMethod]
		public void CollectionFieldTest()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				NaturalPersonEntity example = new NaturalPersonEntity ();
				UriContactEntity contact = new UriContactEntity ();
				example.Contacts.Add (contact);

				Request request = new Request ()
				{
					RootEntity = example,
					RequestedEntity = contact,
				};

				request.Conditions.Add (
					new BinaryComparison (
						CollectionField.CreateRank (example, Druid.Parse ("[J1AC1]"), contact),
						BinaryComparator.IsEqual,
						new Constant ((long) 1)
					)
				);

				UriContactEntity[] contacts = dataContext.GetByRequest<UriContactEntity> (request).ToArray ();

				Assert.IsTrue (contacts.Count () == 1);
				Assert.IsTrue (DatabaseCreator2.CheckUriContact (contacts[0], "alfred@blabla.com", "Alfred"));
			}
		}


		[TestMethod]
		public void InternalFieldTest()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				NaturalPersonEntity example = new NaturalPersonEntity ();

				Request request = new Request ()
				{
					RootEntity = example,
				};

				request.Conditions.Add (
					new BinaryComparison (
						InternalField.CreateId (example),
						BinaryComparator.IsEqual,
						new Constant ((long) 1000000001)
					)
				);

				NaturalPersonEntity[] persons = dataContext.GetByRequest<NaturalPersonEntity> (request).ToArray ();

				Assert.IsTrue (persons.Count () == 1);
				Assert.IsTrue (DatabaseCreator2.CheckAlfred (persons[0]));
			}
		}


		[TestMethod]
		public void SignificantFieldTest()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				NaturalPersonEntity example = new NaturalPersonEntity ();
				UriContactEntity contact = new UriContactEntity ()
				{
					UriScheme = new UriSchemeEntity ()
					{
						Name = "email"
					}
				};

				example.Contacts.Add (contact);

				Request request = new Request ()
				{
					RootEntity = example,
				};

				request.SignificantFields.Add (CollectionField.CreateRank (example, Druid.Parse ("[J1AC1]"), contact));

				NaturalPersonEntity[] persons = dataContext.GetByRequest<NaturalPersonEntity> (request).ToArray ();

				Assert.IsTrue (persons.Count () == 3);
				Assert.AreEqual (1, persons.Count (p => DatabaseCreator2.CheckGertrude (p)));
				Assert.AreEqual (2, persons.Count (p => DatabaseCreator2.CheckAlfred (p)));
			}
		}


	}


}
