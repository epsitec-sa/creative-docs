using Epsitec.Common.Types;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Infrastructure;
using Epsitec.Cresus.DataLayer.Tests.Vs.Entities;
using Epsitec.Cresus.DataLayer.Tests.Vs.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Tests.Vs.General
{


	[TestClass]
	public sealed class UnitTestGetEntitiesByExample
	{


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();
		}


		[TestInitialize]
		public void TestInitialize()
		{
			DatabaseCreator2.ResetPopulatedTestDatabase ();
		}


		[TestMethod]
		public void GetObjects()
		{			
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				NaturalPersonEntity[] persons = dataContext.GetByExample<NaturalPersonEntity> (new NaturalPersonEntity ()).ToArray ();

				Assert.IsTrue (persons.Length == 3);

				Assert.IsTrue (persons.Any (p => DatabaseCreator2.CheckAlfred (p)));
				Assert.IsTrue (persons.Any (p => DatabaseCreator2.CheckGertrude (p)));
				Assert.IsTrue (persons.Any (p => DatabaseCreator2.CheckHans (p)));

			}
		}


		[TestMethod]
		public void GetObjectsWithCast()
		{		
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				NaturalPersonEntity[] persons = dataContext.GetByExample<AbstractPersonEntity> (new AbstractPersonEntity ()).Cast<NaturalPersonEntity> ().ToArray ();

				Assert.IsTrue (persons.Length == 3);

				Assert.IsTrue (persons.Any (p => DatabaseCreator2.CheckAlfred (p)));
				Assert.IsTrue (persons.Any (p => DatabaseCreator2.CheckGertrude (p)));
				Assert.IsTrue (persons.Any (p => DatabaseCreator2.CheckHans (p)));
			}
		}


		[TestMethod]
		public void GetObjectsWithCorrectValueExample()
		{		
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				NaturalPersonEntity example = DatabaseCreator2.GetCorrectExample1 ();

				NaturalPersonEntity[] persons = dataContext.GetByExample<NaturalPersonEntity> (example).ToArray ();

				Assert.IsTrue (persons.Count () == 1);

				Assert.IsTrue (persons.Any (p => DatabaseCreator2.CheckAlfred (p)));
			}
		}


		[TestMethod]
		public void GetObjectsWithCorrectReferenceExample()
		{		
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				NaturalPersonEntity example = DatabaseCreator2.GetCorrectExample2 ();

				NaturalPersonEntity[] persons = dataContext.GetByExample<NaturalPersonEntity> (example).ToArray ();

				Assert.IsTrue (persons.Count () == 1);

				Assert.IsTrue (persons.Any (p => DatabaseCreator2.CheckAlfred (p)));
			}
		}


		[TestMethod]
		public void GetObjectsWithCorrectCollectionExample1()
		{		
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				NaturalPersonEntity example = DatabaseCreator2.GetCorrectExample3 ();

				NaturalPersonEntity[] persons = dataContext.GetByExample<NaturalPersonEntity> (example).ToArray ();

				Assert.IsTrue (persons.Length == 2);

				Assert.IsTrue (persons.Any (p => DatabaseCreator2.CheckAlfred (p)));
				Assert.IsTrue (persons.Any (p => DatabaseCreator2.CheckGertrude (p)));
			}
		}


		[TestMethod]
		public void GetObjectsWithCorrectCollectionExample2()
		{		
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				NaturalPersonEntity example = DatabaseCreator2.GetCorrectExample4 ();

				NaturalPersonEntity[] persons = dataContext.GetByExample<NaturalPersonEntity> (example).ToArray ();

				Assert.IsTrue (persons.Count () == 1);

				Assert.IsTrue (persons.Any (p => DatabaseCreator2.CheckAlfred (p)));
			}
		}


		[TestMethod]
		public void GetObjectsWithCorrectCollectionExample3()
		{			
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				NaturalPersonEntity example = DatabaseCreator2.GetCorrectExample5 ();

				NaturalPersonEntity[] persons = dataContext.GetByExample<NaturalPersonEntity> (example).ToArray ();

				Assert.IsTrue (persons.Count () == 1);

				Assert.IsTrue (persons.Any (p => DatabaseCreator2.CheckAlfred (p)));
			}
		}


		[TestMethod]
		public void GetObjectsWithIncorrectValueExample()
		{			
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				NaturalPersonEntity example = DatabaseCreator2.GetIncorrectExample1 ();

				NaturalPersonEntity[] persons = dataContext.GetByExample<NaturalPersonEntity> (example).ToArray ();

				Assert.IsTrue (persons.Count () == 0);
			}
		}


		[TestMethod]
		public void GetObjectsWithIncorrectReferenceExample()
		{			
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				NaturalPersonEntity example = DatabaseCreator2.GetIncorrectExample2 ();

				NaturalPersonEntity[] persons = dataContext.GetByExample<NaturalPersonEntity> (example).ToArray ();

				Assert.IsTrue (persons.Count () == 0);
			}
		}


		[TestMethod]
		public void GetObjectsWithIncorrectCollectionExample1()
		{			
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				NaturalPersonEntity example = DatabaseCreator2.GetIncorrectExample3 ();

				NaturalPersonEntity[] persons = dataContext.GetByExample<NaturalPersonEntity> (example).ToArray ();

				Assert.IsTrue (persons.Count () == 0);
			}
		}


		[TestMethod]
		public void GetObjectsWithIncorrectCollectionExample2()
		{			
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				NaturalPersonEntity example = DatabaseCreator2.GetIncorrectExample4 ();

				NaturalPersonEntity[] persons = dataContext.GetByExample<NaturalPersonEntity> (example).ToArray ();

				Assert.IsTrue (persons.Count () == 0);
			}
		}


		[TestMethod]
		public void GetObjectsWithEntityEquality()
		{			
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				NaturalPersonEntity example = DatabaseCreator2.GetCorrectExample1 ();

				NaturalPersonEntity[] persons1 = dataContext.GetByExample<NaturalPersonEntity> (example).ToArray ();
				NaturalPersonEntity[] persons2 = dataContext.GetByExample<NaturalPersonEntity> (example).ToArray ();

				Assert.IsTrue (persons1.Count () == 1);
				Assert.IsTrue (persons2.Count () == 1);

				Assert.IsTrue (persons1.Any (p => DatabaseCreator2.CheckAlfred (p)));
				Assert.IsTrue (persons2.Any (p => DatabaseCreator2.CheckAlfred (p)));

				Assert.IsTrue (object.ReferenceEquals (persons1[0], persons2[0]));
			}
		}


		[TestMethod]
		public void GetFreshObject()
		{			
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				NaturalPersonEntity freshPerson1 = dataContext.CreateEntity<NaturalPersonEntity> ();

				freshPerson1.Firstname = "Albert";
				freshPerson1.Lastname = "Levert";

				dataContext.SaveChanges ();

				NaturalPersonEntity example = new NaturalPersonEntity ()
				{
					Firstname = "Albert",
					Lastname = "Levert",
				};

				NaturalPersonEntity freshPerson2 = dataContext.GetByExample<NaturalPersonEntity> (example).First ();

				Assert.IsTrue (freshPerson1 == freshPerson2);

				dataContext.SaveChanges ();
			}
		}


		[TestMethod]
		public void GetObjectByReferenceReference()
		{			
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				NaturalPersonEntity alfredExample = DatabaseCreator2.GetCorrectExample1 ();
				NaturalPersonEntity alfred = dataContext.GetByExample<NaturalPersonEntity> (alfredExample).FirstOrDefault ();

				UriContactEntity contactExample = new UriContactEntity ()
				{
					NaturalPerson = alfred,
				};

				UriContactEntity[] contacts = dataContext.GetByExample<UriContactEntity> (contactExample).ToArray ();

				Assert.IsTrue (contacts.Length == 2);

				Assert.IsTrue (contacts.Any (c => DatabaseCreator2.CheckUriContact (c, "alfred@coucou.com", "Alfred")));
				Assert.IsTrue (contacts.Any (c => DatabaseCreator2.CheckUriContact (c, "alfred@blabla.com", "Alfred")));

				Assert.IsTrue (contacts.All (c => c.NaturalPerson == alfred));
			}
		}


		[TestMethod]
		public void GetObjectByCollectionReference()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				UriContactEntity contactExample = new UriContactEntity ()
					{
						Uri = "alfred@coucou.com",
					};

				UriContactEntity contact = dataContext.GetByExample (contactExample).FirstOrDefault ();

				NaturalPersonEntity alfredExample = new NaturalPersonEntity ();
				alfredExample.Contacts.Add (contact);

				NaturalPersonEntity alfred = dataContext.GetByExample (alfredExample).FirstOrDefault ();

				Assert.IsTrue (DatabaseCreator2.CheckAlfred (alfred));
			}
		}


		[TestMethod]
		public void GetObjectsWithDeletedEntity()
		{		
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			{
				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				{
					NaturalPersonEntity example = DatabaseCreator2.GetCorrectExample1 ();

					NaturalPersonEntity[] persons = dataContext.GetByExample<NaturalPersonEntity> (example).ToArray ();

					Assert.IsTrue (persons.Count () == 1);
					Assert.IsTrue (persons.Any (p => DatabaseCreator2.CheckAlfred (p)));

					dataContext.DeleteEntity (persons[0]);

					dataContext.SaveChanges ();

					NaturalPersonEntity[] persons2 = dataContext.GetByExample<NaturalPersonEntity> (example).ToArray ();

					Assert.IsTrue (persons2.Count () == 0);
				}

				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				{
					NaturalPersonEntity example = DatabaseCreator2.GetCorrectExample1 ();

					NaturalPersonEntity[] persons = dataContext.GetByExample<NaturalPersonEntity> (example).ToArray ();

					Assert.IsTrue (persons.Count () == 0);
				}
			}
		}


		[TestMethod]
		public void GetObjectsWithDeletedRelation()
		{		
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			{
				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				{
					NaturalPersonEntity example = DatabaseCreator2.GetCorrectExample1 ();

					NaturalPersonEntity[] persons = dataContext.GetByExample<NaturalPersonEntity> (example).ToArray ();

					Assert.IsTrue (persons.Count () == 1);
					Assert.IsTrue (persons.Any (p => DatabaseCreator2.CheckAlfred (p)));

					persons[0].Contacts.RemoveAt (0);
					persons[0].Contacts.RemoveAt (0);

					persons[0].Gender = null;

					dataContext.SaveChanges ();

					NaturalPersonEntity[] persons2 = dataContext.GetByExample<NaturalPersonEntity> (example).ToArray ();

					Assert.IsTrue (persons2.Count () == 1);
					Assert.IsTrue (persons2[0].Contacts.Count == 0);
					Assert.IsTrue (persons2[0].Gender == null);
				}

				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				{
					NaturalPersonEntity example = DatabaseCreator2.GetCorrectExample1 ();

					NaturalPersonEntity[] persons = dataContext.GetByExample<NaturalPersonEntity> (example).ToArray ();

					Assert.IsTrue (persons.Count () == 1);
					Assert.IsTrue (persons[0].Contacts.Count == 0);
					Assert.IsTrue (persons[0].Gender == null);
				}
			}
		}


		[TestMethod]
		public void GetObjectBasedOnBooleanField()
		{		
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				ValueDataEntity example = new ValueDataEntity ()
				{
					BooleanValue = false,
				};

				ValueDataEntity[] valueData = dataContext.GetByExample<ValueDataEntity> (example).ToArray ();

				Assert.IsTrue (valueData.Count () == 1);

				Assert.IsTrue (valueData.Any (vd => DatabaseCreator2.CheckValueData2 (vd)));
			}
		}


		[TestMethod]
		public void GetObjectBasedOnByteArrayField()
		{			
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				ValueDataEntity example = new ValueDataEntity ()
				{
					ByteArrayValue = new byte[] { 0x0F, 0xF0 },
				};

				ValueDataEntity[] valueData = dataContext.GetByExample<ValueDataEntity> (example).ToArray ();

				Assert.IsTrue (valueData.Count () == 1);

				Assert.IsTrue (valueData.Any (vd => DatabaseCreator2.CheckValueData1 (vd)));
			}
		}


		[TestMethod]
		public void GetObjectBasedOnDateTimeField()
		{			
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				ValueDataEntity example = new ValueDataEntity ()
				{
					DateTimeValue = new System.DateTime (1969, 7, 21, 4, 17, 0),
				};

				ValueDataEntity[] valueData = dataContext.GetByExample<ValueDataEntity> (example).ToArray ();

				Assert.IsTrue (valueData.Count () == 1);

				Assert.IsTrue (valueData.Any (vd => DatabaseCreator2.CheckValueData1 (vd)));
			}
		}


		[TestMethod]
		public void GetObjectBasedOnDateField()
		{			
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				ValueDataEntity example = new ValueDataEntity ()
				{
					DateValue = new Date (1291, 8, 1),
				};

				ValueDataEntity[] valueData = dataContext.GetByExample<ValueDataEntity> (example).ToArray ();

				Assert.IsTrue (valueData.Count () == 1);

				Assert.IsTrue (valueData.Any (vd => DatabaseCreator2.CheckValueData1 (vd)));
			}
		}


		[TestMethod]
		public void GetObjectBasedOnDecimalField()
		{			
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				ValueDataEntity example = new ValueDataEntity ()
				{
					DecimalValue = 123.456m,
				};

				ValueDataEntity[] valueData = dataContext.GetByExample<ValueDataEntity> (example).ToArray ();

				Assert.IsTrue (valueData.Count () == 1);

				Assert.IsTrue (valueData.Any (vd => DatabaseCreator2.CheckValueData1 (vd)));
			}
		}


		[TestMethod]
		public void GetObjectBasedOnEnumField()
		{		
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				ValueDataEntity example = new ValueDataEntity ()
				{
					EnumValue = SimpleEnum.Value2,
				};

				ValueDataEntity[] valueData = dataContext.GetByExample<ValueDataEntity> (example).ToArray ();

				Assert.IsTrue (valueData.Count () == 1);

				Assert.IsTrue (valueData.Any (vd => DatabaseCreator2.CheckValueData2 (vd)));
			}
		}


		[TestMethod]
		public void GetObjectBasedOnIntegerField()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				ValueDataEntity example = new ValueDataEntity ()
				{
					IntegerValue = 42,
				};

				ValueDataEntity[] valueData = dataContext.GetByExample<ValueDataEntity> (example).ToArray ();

				Assert.IsTrue (valueData.Count () == 1);

				Assert.IsTrue (valueData.Any (vd => DatabaseCreator2.CheckValueData1 (vd)));
			}
		}


		[TestMethod]
		public void GetObjectBasedOnLongIntegerField()
		{			
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				ValueDataEntity example = new ValueDataEntity ()
				{
					LongIntegerValue = 4242,
				};

				ValueDataEntity[] valueData = dataContext.GetByExample<ValueDataEntity> (example).ToArray ();

				Assert.IsTrue (valueData.Count () == 1);

				Assert.IsTrue (valueData.Any (vd => DatabaseCreator2.CheckValueData1 (vd)));
			}
		}


		[TestMethod]
		public void GetObjectBasedOnStringField()
		{			
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				ValueDataEntity example = new ValueDataEntity ()
				{
					StringValue = "blupi",
				};

				ValueDataEntity[] valueData = dataContext.GetByExample<ValueDataEntity> (example).ToArray ();

				Assert.IsTrue (valueData.Count () == 1);

				Assert.IsTrue (valueData.Any (vd => DatabaseCreator2.CheckValueData1 (vd)));
			}
		}


		[TestMethod]
		public void GetObjectBasedOnTimeField()
		{			
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				ValueDataEntity example = new ValueDataEntity ()
				{
					TimeValue = new Time(12, 12, 12),
				};

				ValueDataEntity[] valueData = dataContext.GetByExample<ValueDataEntity> (example).ToArray ();

				Assert.IsTrue (valueData.Count () == 1);

				Assert.IsTrue (valueData.Any (vd => DatabaseCreator2.CheckValueData1 (vd)));
			}
		}


	}


}
