using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Infrastructure;
using Epsitec.Cresus.DataLayer.Loader;
using Epsitec.Cresus.DataLayer.UnitTests.Entities;
using Epsitec.Cresus.DataLayer.UnitTests.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.UnitTests.General
{


	[TestClass]
	public sealed class UnitTestForeignEntities
	{


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();

			DatabaseHelper.CreateAndConnectToDatabase ();

			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{

					DatabaseCreator2.PupulateDatabase (dataContext);
				}
			}

			DatabaseHelper.DisconnectFromDatabase ();
		}


		[TestMethod]
		public void EntityReference1()
		{
			using (DbInfrastructure infrastructure = this.GetDbInfrastructure ())
			{
				using (DataInfrastructure dataInfrastructure = new DataInfrastructure (infrastructure))
				{
					using (DataContext dataContext1 = dataInfrastructure.CreateDataContext ())
					using (DataContext dataContext2 = dataInfrastructure.CreateDataContext ())
					{
						NaturalPersonEntity person1 = dataContext1.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
						PersonGenderEntity gender2 = dataContext2.ResolveEntity<PersonGenderEntity> (new DbKey (new DbId (2)));

						person1.Gender = gender2;

						ExceptionAssert.Throw<System.InvalidOperationException>
						(
							() => dataContext1.SaveChanges ()
						);
					}
				}
			}
		}


		[TestMethod]
		public void EntityReference2()
		{
			using (DbInfrastructure infrastructure = this.GetDbInfrastructure ())
			{
				using (DataInfrastructure dataInfrastructure = new DataInfrastructure (infrastructure))
				{
					using (DataContext dataContext1 = dataInfrastructure.CreateDataContext ())
					using (DataContext dataContext2 = dataInfrastructure.CreateDataContext ())
					{
						NaturalPersonEntity person1 = dataContext1.CreateEntity<NaturalPersonEntity> ();
						PersonGenderEntity gender2 = dataContext2.ResolveEntity<PersonGenderEntity> (new DbKey (new DbId (2)));

						person1.Gender = gender2;

						ExceptionAssert.Throw<System.InvalidOperationException>
						(
							() => dataContext1.SaveChanges ()
						);
					}
				}
			}
		}


		[TestMethod]
		public void EntityCollection1()
		{
			using (DbInfrastructure infrastructure = this.GetDbInfrastructure ())
			{
				using (DataInfrastructure dataInfrastructure = new DataInfrastructure (infrastructure))
				{
					using (DataContext dataContext1 = dataInfrastructure.CreateDataContext ())
					using (DataContext dataContext2 = dataInfrastructure.CreateDataContext ())
					{
						NaturalPersonEntity person1 = dataContext1.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
						UriContactEntity contact2 = dataContext2.ResolveEntity<UriContactEntity> (new DbKey (new DbId (4)));

						person1.Contacts.Add (contact2);

						ExceptionAssert.Throw<System.InvalidOperationException>
						(
							() => dataContext1.SaveChanges ()
						);
					}
				}
			}
		}


		[TestMethod]
		public void EntityCollection2()
		{
			using (DbInfrastructure infrastructure = this.GetDbInfrastructure ())
			{
				using (DataInfrastructure dataInfrastructure = new DataInfrastructure (infrastructure))
				{
					using (DataContext dataContext1 = dataInfrastructure.CreateDataContext ())
					using (DataContext dataContext2 = dataInfrastructure.CreateDataContext ())
					{
						NaturalPersonEntity person1 = dataContext1.CreateEntity<NaturalPersonEntity> ();
						UriContactEntity contact2 = dataContext2.ResolveEntity<UriContactEntity> (new DbKey (new DbId (4)));

						person1.Contacts.Add (contact2);

						ExceptionAssert.Throw<System.InvalidOperationException>
						(
							() => dataContext1.SaveChanges ()
						);
					}
				}
			}
		}

		
		[TestMethod]
		public void GetByRequest1()
		{
			using (DbInfrastructure infrastructure = this.GetDbInfrastructure ())
			{
				using (DataInfrastructure dataInfrastructure = new DataInfrastructure (infrastructure))
				{
					using (DataContext dataContext1 = dataInfrastructure.CreateDataContext ())
					using (DataContext dataContext2 = dataInfrastructure.CreateDataContext ())
					{
						NaturalPersonEntity person = new NaturalPersonEntity ();

						person.Contacts.Add (dataContext1.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1))));

						Request request = new Request ()
						{
							RootEntity = person,
						};

						ExceptionAssert.Throw<System.InvalidOperationException>
						(
							() => dataContext2.GetByRequest<NaturalPersonEntity> (request).ToList ()
						);
					}
				}
			}
		}


		[TestMethod]
		public void GetByRequest2()
		{
			using (DbInfrastructure infrastructure = this.GetDbInfrastructure ())
			{
				using (DataInfrastructure dataInfrastructure = new DataInfrastructure (infrastructure))
				{
					using (DataContext dataContext1 = dataInfrastructure.CreateDataContext ())
					using (DataContext dataContext2 = dataInfrastructure.CreateDataContext ())
					{
						AbstractContactEntity contact1 = dataContext1.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1)));

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
		}


		[TestMethod]
		public void GetByRequest3()
		{
			using (DbInfrastructure infrastructure = this.GetDbInfrastructure ())
			{
				using (DataInfrastructure dataInfrastructure = new DataInfrastructure (infrastructure))
				{
					using (DataContext dataContext1 = dataInfrastructure.CreateDataContext ())
					using (DataContext dataContext2 = dataInfrastructure.CreateDataContext ())
					{
						AbstractContactEntity contact1 = dataContext1.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1)));
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
		}


		[TestMethod]
		public void GetByExample1()
		{
			using (DbInfrastructure infrastructure = this.GetDbInfrastructure ())
			{
				using (DataInfrastructure dataInfrastructure = new DataInfrastructure (infrastructure))
				{
					using (DataContext dataContext1 = dataInfrastructure.CreateDataContext ())
					using (DataContext dataContext2 = dataInfrastructure.CreateDataContext ())
					{
						AbstractContactEntity contact = new AbstractContactEntity ()
						{
							NaturalPerson = dataContext1.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1))),
						};

						ExceptionAssert.Throw<System.InvalidOperationException>
						(
							() => dataContext2.GetByExample (contact).ToList ()
						);
					}
				}
			}
		}


		[TestMethod]
		public void GetByExample2()
		{
			using (DbInfrastructure infrastructure = this.GetDbInfrastructure ())
			{
				using (DataInfrastructure dataInfrastructure = new DataInfrastructure (infrastructure))
				{
					using (DataContext dataContext1 = dataInfrastructure.CreateDataContext ())
					using (DataContext dataContext2 = dataInfrastructure.CreateDataContext ())
					{
						NaturalPersonEntity	person1 = dataContext1.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));

						ExceptionAssert.Throw<System.ArgumentException>
						(
							() => dataContext2.GetByExample (person1)
						);
					}
				}
			}
		}


		[TestMethod]
		public void DeleteEntity()
		{
			using (DbInfrastructure infrastructure = this.GetDbInfrastructure ())
			{
				using (DataInfrastructure dataInfrastructure = new DataInfrastructure (infrastructure))
				{
					using (DataContext dataContext1 = dataInfrastructure.CreateDataContext ())
					using (DataContext dataContext2 = dataInfrastructure.CreateDataContext ())
					{
						NaturalPersonEntity person1 = dataContext1.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));

						ExceptionAssert.Throw<System.ArgumentException>
						(
							() => dataContext2.DeleteEntity (person1)
						);
					}
				}
			}
		}


		[TestMethod]
		public void IsDeleted()
		{
			using (DbInfrastructure infrastructure = this.GetDbInfrastructure ())
			{
				using (DataInfrastructure dataInfrastructure = new DataInfrastructure (infrastructure))
				{
					using (DataContext dataContext1 = dataInfrastructure.CreateDataContext ())
					using (DataContext dataContext2 = dataInfrastructure.CreateDataContext ())
					{
						NaturalPersonEntity person1 = dataContext1.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));

						ExceptionAssert.Throw<System.ArgumentException>
						(
							() => dataContext2.IsDeleted (person1)
						);
					}
				}
			}
		}


		[TestMethod]
		public void GetLeafEntityKey()
		{
			using (DbInfrastructure infrastructure = this.GetDbInfrastructure ())
			{
				using (DataInfrastructure dataInfrastructure = new DataInfrastructure (infrastructure))
				{
					using (DataContext dataContext1 = dataInfrastructure.CreateDataContext ())
					using (DataContext dataContext2 = dataInfrastructure.CreateDataContext ())
					{
						NaturalPersonEntity person1 = dataContext1.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));

						ExceptionAssert.Throw<System.ArgumentException>
						(
							() => dataContext2.GetLeafEntityKey (person1)
						);
					}
				}
			}
		}


		[TestMethod]
		public void GetNormalizedEntityKey()
		{
			using (DbInfrastructure infrastructure = this.GetDbInfrastructure ())
			{
				using (DataInfrastructure dataInfrastructure = new DataInfrastructure (infrastructure))
				{
					using (DataContext dataContext1 = dataInfrastructure.CreateDataContext ())
					using (DataContext dataContext2 = dataInfrastructure.CreateDataContext ())
					{
						NaturalPersonEntity person1 = dataContext1.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));

						ExceptionAssert.Throw<System.ArgumentException>
						(
							() => dataContext2.GetNormalizedEntityKey (person1)
						);
					}
				}
			}
		}


		[TestMethod]
		public void GetPersistedId()
		{
			using (DbInfrastructure infrastructure = this.GetDbInfrastructure ())
			{
				using (DataInfrastructure dataInfrastructure = new DataInfrastructure (infrastructure))
				{
					using (DataContext dataContext1 = dataInfrastructure.CreateDataContext ())
					using (DataContext dataContext2 = dataInfrastructure.CreateDataContext ())
					{
						NaturalPersonEntity person1 = dataContext1.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));

						ExceptionAssert.Throw<System.ArgumentException>
						(
							() => dataContext2.GetPersistedId (person1)
						);
					}
				}
			}
		}


		[TestMethod]
		public void IsPersistent()
		{
			using (DbInfrastructure infrastructure = this.GetDbInfrastructure ())
			{
				using (DataInfrastructure dataInfrastructure = new DataInfrastructure (infrastructure))
				{
					using (DataContext dataContext1 = dataInfrastructure.CreateDataContext ())
					using (DataContext dataContext2 = dataInfrastructure.CreateDataContext ())
					{
						NaturalPersonEntity person1 = dataContext1.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));

						ExceptionAssert.Throw<System.ArgumentException>
						(
							() => dataContext2.IsPersistent (person1)
						);
					}
				}
			}
		}


		[TestMethod]
		public void IsRegisteredAsEmptyEntity()
		{
			using (DbInfrastructure infrastructure = this.GetDbInfrastructure ())
			{
				using (DataInfrastructure dataInfrastructure = new DataInfrastructure (infrastructure))
				{
					using (DataContext dataContext1 = dataInfrastructure.CreateDataContext ())
					using (DataContext dataContext2 = dataInfrastructure.CreateDataContext ())
					{
						NaturalPersonEntity person1 = dataContext1.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));

						ExceptionAssert.Throw<System.ArgumentException>
						(
							() => dataContext2.IsRegisteredAsEmptyEntity (person1)
						);
					}
				}
			}
		}


		[TestMethod]
		public void RegisterEmptyEntity()
		{
			using (DbInfrastructure infrastructure = this.GetDbInfrastructure ())
			{
				using (DataInfrastructure dataInfrastructure = new DataInfrastructure (infrastructure))
				{
					using (DataContext dataContext1 = dataInfrastructure.CreateDataContext ())
					using (DataContext dataContext2 = dataInfrastructure.CreateDataContext ())
					{
						NaturalPersonEntity person1 = dataContext1.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));

						ExceptionAssert.Throw<System.ArgumentException>
						(
							() => dataContext2.RegisterEmptyEntity (person1)
						);
					}
				}
			}
		}


		[TestMethod]
		public void UnregisterEmptyEntity()
		{
			using (DbInfrastructure infrastructure = this.GetDbInfrastructure ())
			{
				using (DataInfrastructure dataInfrastructure = new DataInfrastructure (infrastructure))
				{
					using (DataContext dataContext1 = dataInfrastructure.CreateDataContext ())
					using (DataContext dataContext2 = dataInfrastructure.CreateDataContext ())
					{
						NaturalPersonEntity person1 = dataContext1.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));

						ExceptionAssert.Throw<System.ArgumentException>
						(
							() => dataContext2.UnregisterEmptyEntity (person1)
						);
					}
				}
			}
		}


		[TestMethod]
		public void UpdateEmptyEntityStatus()
		{
			using (DbInfrastructure infrastructure = this.GetDbInfrastructure ())
			{
				using (DataInfrastructure dataInfrastructure = new DataInfrastructure (infrastructure))
				{
					using (DataContext dataContext1 = dataInfrastructure.CreateDataContext ())
					using (DataContext dataContext2 = dataInfrastructure.CreateDataContext ())
					{
						NaturalPersonEntity person1 = dataContext1.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));

						ExceptionAssert.Throw<System.ArgumentException>
						(
							() => dataContext2.UpdateEmptyEntityStatus (person1, false)
						);
					}
				}
			}
		}


		private DbInfrastructure GetDbInfrastructure()
		{
			DbInfrastructure dbInfrastructure = new DbInfrastructure ();

			DbAccess access = TestHelper.CreateDbAccess ();
			dbInfrastructure.AttachToDatabase (access);

			return dbInfrastructure;
		}


	}


}
