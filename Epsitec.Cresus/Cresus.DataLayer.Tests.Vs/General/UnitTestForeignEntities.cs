using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Infrastructure;
using Epsitec.Cresus.DataLayer.Loader;
using Epsitec.Cresus.DataLayer.Tests.Vs.Entities;
using Epsitec.Cresus.DataLayer.Tests.Vs.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Tests.Vs.General
{


	[TestClass]
	public sealed class UnitTestForeignEntities
	{


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();

			DatabaseCreator2.ResetPopulatedTestDatabase ();
		}


		[TestMethod]
		public void EntityReference1()
		{
			using (DB db = DB.ConnectToTestDatabase ())
			{
				using (DataContext dataContext1 = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure))
				using (DataContext dataContext2 = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure))
				{
					NaturalPersonEntity person1 = dataContext1.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
					PersonGenderEntity gender2 = dataContext2.ResolveEntity<PersonGenderEntity> (new DbKey (new DbId (1000000002)));

					person1.Gender = gender2;

					ExceptionAssert.Throw<System.InvalidOperationException>
					(
						() => dataContext1.SaveChanges ()
					);
				}
			}
		}


		[TestMethod]
		public void EntityReference2()
		{
			using (DB db = DB.ConnectToTestDatabase ())
			{
				using (DataContext dataContext1 = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure))
				using (DataContext dataContext2 = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure))
				{
					NaturalPersonEntity person1 = dataContext1.CreateEntity<NaturalPersonEntity> ();
					PersonGenderEntity gender2 = dataContext2.ResolveEntity<PersonGenderEntity> (new DbKey (new DbId (1000000002)));

					person1.Gender = gender2;

					ExceptionAssert.Throw<System.InvalidOperationException>
					(
						() => dataContext1.SaveChanges ()
					);
				}
			}
		}


		[TestMethod]
		public void EntityCollection1()
		{
			using (DB db = DB.ConnectToTestDatabase ())
			{
				using (DataContext dataContext1 = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure))
				using (DataContext dataContext2 = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure))
				{
					NaturalPersonEntity person1 = dataContext1.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
					UriContactEntity contact2 = dataContext2.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1000000004)));

					person1.Contacts.Add (contact2);

					ExceptionAssert.Throw<System.InvalidOperationException>
					(
						() => dataContext1.SaveChanges ()
					);
				}
			}
		}


		[TestMethod]
		public void EntityCollection2()
		{
			using (DB db = DB.ConnectToTestDatabase ())
			{
				using (DataContext dataContext1 = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure))
				using (DataContext dataContext2 = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure))
				{
					NaturalPersonEntity person1 = dataContext1.CreateEntity<NaturalPersonEntity> ();
					UriContactEntity contact2 = dataContext2.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1000000004)));

					person1.Contacts.Add (contact2);

					ExceptionAssert.Throw<System.InvalidOperationException>
					(
						() => dataContext1.SaveChanges ()
					);
				}
			}
		}


		[TestMethod]
		public void GetByRequest1()
		{
			using (DB db = DB.ConnectToTestDatabase ())
			{
				using (DataContext dataContext1 = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure))
				using (DataContext dataContext2 = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure))
				{
					NaturalPersonEntity person = new NaturalPersonEntity ();

					person.Contacts.Add (dataContext1.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1000000001))));

					Request request = new Request ()
					{
						RootEntity = person,
					};

					ExceptionAssert.Throw<System.ArgumentException>
					(
						() => dataContext2.GetByRequest<NaturalPersonEntity> (request).ToList ()
					);
				}
			}
		}


		[TestMethod]
		public void GetByRequest2()
		{
			using (DB db = DB.ConnectToTestDatabase ())
			{
				using (DataContext dataContext1 = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure))
				using (DataContext dataContext2 = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure))
				{
					Request request = new Request ()
					{
						RootEntity = new NaturalPersonEntity
						{
							Gender = dataContext1.ResolveEntity<PersonGenderEntity> (new DbKey (new DbId (1000000001)))
						},
					};

					ExceptionAssert.Throw<System.ArgumentException>
					(
						() => dataContext2.GetByRequest<NaturalPersonEntity> (request).ToList ()
					);
				}
			}
		}


		[TestMethod]
		public void GetByRequest3()
		{
			using (DB db = DB.ConnectToTestDatabase ())
			{
				using (DataContext dataContext1 = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure))
				using (DataContext dataContext2 = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure))
				{
					AbstractContactEntity contact1 = dataContext1.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1000000001)));

					Request request = new Request ()
					{
						RequestedEntity = contact1,
					};

					ExceptionAssert.Throw<System.ArgumentException>
					(
						() => dataContext2.GetByRequest<NaturalPersonEntity> (request)
					);
				}
			}
		}


		[TestMethod]
		public void GetByRequest4()
		{
			using (DB db = DB.ConnectToTestDatabase ())
			{
				using (DataContext dataContext1 = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure))
				using (DataContext dataContext2 = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure))
				{
					AbstractContactEntity contact1 = dataContext1.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1000000001)));
					Request request = new Request ()
					{
						RootEntity = contact1,
					};

					ExceptionAssert.Throw<System.ArgumentException>
					(
						() => dataContext2.GetByRequest<NaturalPersonEntity> (request)
					);
				}
			}
		}


		[TestMethod]
		public void GetByExample1()
		{
			using (DB db = DB.ConnectToTestDatabase ())
			{
				using (DataContext dataContext1 = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure))
				using (DataContext dataContext2 = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure))
				{
					AbstractContactEntity contact = new AbstractContactEntity ()
					{
						NaturalPerson = dataContext1.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001))),
					};

					ExceptionAssert.Throw<System.ArgumentException>
					(
						() => dataContext2.GetByExample (contact).ToList ()
					);
				}
			}
		}


		[TestMethod]
		public void GetByExample2()
		{
			using (DB db = DB.ConnectToTestDatabase ())
			{
				using (DataContext dataContext1 = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure))
				using (DataContext dataContext2 = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure))
				{
					NaturalPersonEntity	person1 = dataContext1.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));

					ExceptionAssert.Throw<System.ArgumentException>
					(
						() => dataContext2.GetByExample (person1)
					);
				}
			}
		}


		[TestMethod]
		public void GetCount1()
		{
			using (DB db = DB.ConnectToTestDatabase ())
			{
				using (DataContext dataContext1 = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure))
				using (DataContext dataContext2 = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure))
				{
					AbstractContactEntity contact = new AbstractContactEntity ()
					{
						NaturalPerson = dataContext1.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001))),
					};

					ExceptionAssert.Throw<System.ArgumentException>
					(
						() => dataContext2.GetCount (contact)
					);
				}
			}
		}


		[TestMethod]
		public void GetCount2()
		{
			using (DB db = DB.ConnectToTestDatabase ())
			{
				using (DataContext dataContext1 = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure))
				using (DataContext dataContext2 = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure))
				{
					NaturalPersonEntity	person1 = dataContext1.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));

					ExceptionAssert.Throw<System.ArgumentException>
					(
						() => dataContext2.GetCount (person1)
					);
				}
			}
		}


		[TestMethod]
		public void GetCount3()
		{
			using (DB db = DB.ConnectToTestDatabase ())
			{
				using (DataContext dataContext1 = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure))
				using (DataContext dataContext2 = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure))
				{
					NaturalPersonEntity person = new NaturalPersonEntity ();

					person.Contacts.Add (dataContext1.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1000000001))));

					Request request = new Request ()
					{
						RootEntity = person,
					};

					ExceptionAssert.Throw<System.ArgumentException>
					(
						() => dataContext2.GetCount (request)
					);
				}
			}
		}


		[TestMethod]
		public void GetCount4()
		{
			using (DB db = DB.ConnectToTestDatabase ())
			{
				using (DataContext dataContext1 = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure))
				using (DataContext dataContext2 = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure))
				{
					Request request = new Request ()
					{
						RootEntity = new NaturalPersonEntity
						{
							Gender = dataContext1.ResolveEntity<PersonGenderEntity> (new DbKey (new DbId (1000000001)))
						},
					};

					ExceptionAssert.Throw<System.ArgumentException>
					(
						() => dataContext2.GetCount (request)
					);
				}
			}
		}


		[TestMethod]
		public void GetCount5()
		{
			using (DB db = DB.ConnectToTestDatabase ())
			{
				using (DataContext dataContext1 = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure))
				using (DataContext dataContext2 = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure))
				{
					AbstractContactEntity contact1 = dataContext1.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1000000001)));

					Request request = new Request ()
					{
						RequestedEntity = contact1,
					};

					ExceptionAssert.Throw<System.ArgumentException>
					(
						() => dataContext2.GetCount (request)
					);
				}
			}
		}


		[TestMethod]
		public void GetCount6()
		{
			using (DB db = DB.ConnectToTestDatabase ())
			{
				using (DataContext dataContext1 = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure))
				using (DataContext dataContext2 = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure))
				{
					AbstractContactEntity contact1 = dataContext1.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1000000001)));
					Request request = new Request ()
					{
						RootEntity = contact1,
					};

					ExceptionAssert.Throw<System.ArgumentException>
					(
						() => dataContext2.GetCount (request)
					);
				}
			}
		}


		[TestMethod]
		public void DeleteEntity()
		{
			using (DB db = DB.ConnectToTestDatabase ())
			{
				using (DataContext dataContext1 = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure))
				using (DataContext dataContext2 = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure))
				{
					NaturalPersonEntity person1 = dataContext1.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));

					ExceptionAssert.Throw<System.ArgumentException>
					(
						() => dataContext2.DeleteEntity (person1)
					);
				}
			}
		}


		[TestMethod]
		public void IsDeleted()
		{
			using (DB db = DB.ConnectToTestDatabase ())
			{
				using (DataContext dataContext1 = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure))
				using (DataContext dataContext2 = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure))
				{
					NaturalPersonEntity person1 = dataContext1.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));

					ExceptionAssert.Throw<System.ArgumentException>
					(
						() => dataContext2.IsDeleted (person1)
					);
				}
			}
		}


		[TestMethod]
		public void GetNormalizedEntityKey()
		{
			using (DB db = DB.ConnectToTestDatabase ())
			{
				using (DataContext dataContext1 = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure))
				using (DataContext dataContext2 = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure))
				{
					NaturalPersonEntity person1 = dataContext1.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));

					ExceptionAssert.Throw<System.ArgumentException>
					(
						() => dataContext2.GetNormalizedEntityKey (person1)
					);
				}
			}
		}


		[TestMethod]
		public void GetPersistedId()
		{
			using (DB db = DB.ConnectToTestDatabase ())
			{
				using (DataContext dataContext1 = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure))
				using (DataContext dataContext2 = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure))
				{
					NaturalPersonEntity person1 = dataContext1.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));

					ExceptionAssert.Throw<System.ArgumentException>
					(
						() => dataContext2.GetPersistedId (person1)
					);
				}
			}
		}


		[TestMethod]
		public void IsPersistent()
		{
			using (DB db = DB.ConnectToTestDatabase ())
			{
				using (DataContext dataContext1 = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure))
				using (DataContext dataContext2 = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure))
				{
					NaturalPersonEntity person1 = dataContext1.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));

					ExceptionAssert.Throw<System.ArgumentException>
					(
						() => dataContext2.IsPersistent (person1)
					);
				}
			}
		}


		[TestMethod]
		public void IsRegisteredAsEmptyEntity()
		{
			using (DB db = DB.ConnectToTestDatabase ())
			{
				using (DataContext dataContext1 = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure))
				using (DataContext dataContext2 = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure))
				{
					NaturalPersonEntity person1 = dataContext1.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));

					ExceptionAssert.Throw<System.ArgumentException>
					(
						() => dataContext2.IsRegisteredAsEmptyEntity (person1)
					);
				}
			}
		}


		[TestMethod]
		public void RegisterEmptyEntity()
		{
			using (DB db = DB.ConnectToTestDatabase ())
			{
				using (DataContext dataContext1 = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure))
				using (DataContext dataContext2 = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure))
				{
					NaturalPersonEntity person1 = dataContext1.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));

					ExceptionAssert.Throw<System.ArgumentException>
					(
						() => dataContext2.RegisterEmptyEntity (person1)
					);
				}
			}
		}


		[TestMethod]
		public void UnregisterEmptyEntity()
		{
			using (DB db = DB.ConnectToTestDatabase ())
			{
				using (DataContext dataContext1 = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure))
				using (DataContext dataContext2 = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure))
				{
					NaturalPersonEntity person1 = dataContext1.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));

					ExceptionAssert.Throw<System.ArgumentException>
					(
						() => dataContext2.UnregisterEmptyEntity (person1)
					);
				}
			}
		}


		[TestMethod]
		public void UpdateEmptyEntityStatus()
		{
			using (DB db = DB.ConnectToTestDatabase ())
			{
				using (DataContext dataContext1 = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure))
				using (DataContext dataContext2 = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure))
				{
					NaturalPersonEntity person1 = dataContext1.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));

					ExceptionAssert.Throw<System.ArgumentException>
					(
						() => dataContext2.UpdateEmptyEntityStatus (person1, false)
					);
				}
			}
		}


	}


}
