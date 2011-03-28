using Epsitec.Common.Support;
using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Infrastructure;
using Epsitec.Cresus.DataLayer.Loader;
using Epsitec.Cresus.DataLayer.Schema;
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
			
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			{
				using (DataContext dataContext1 = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				using (DataContext dataContext2 = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
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
			
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			{
				using (DataContext dataContext1 = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				using (DataContext dataContext2 = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
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
			
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			{
				using (DataContext dataContext1 = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				using (DataContext dataContext2 = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
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
			
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			{
				using (DataContext dataContext1 = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				using (DataContext dataContext2 = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
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
			
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			{
				using (DataContext dataContext1 = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				using (DataContext dataContext2 = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				{
					NaturalPersonEntity person = new NaturalPersonEntity ();

					person.Contacts.Add (dataContext1.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1000000001))));

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


		[TestMethod]
		public void GetByRequest2()
		{
			
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			{
				using (DataContext dataContext1 = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				using (DataContext dataContext2 = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
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
		public void GetByRequest3()
		{
			
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			{
				using (DataContext dataContext1 = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				using (DataContext dataContext2 = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
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
			
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			{
				using (DataContext dataContext1 = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				using (DataContext dataContext2 = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				{
					AbstractContactEntity contact = new AbstractContactEntity ()
					{
						NaturalPerson = dataContext1.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001))),
					};

					ExceptionAssert.Throw<System.InvalidOperationException>
					(
						() => dataContext2.GetByExample (contact).ToList ()
					);
				}
			}
		}


		[TestMethod]
		public void GetByExample2()
		{
			
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			{
				using (DataContext dataContext1 = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				using (DataContext dataContext2 = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
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
		public void DeleteEntity()
		{
			
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			{
				using (DataContext dataContext1 = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				using (DataContext dataContext2 = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
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
			
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			{
				using (DataContext dataContext1 = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				using (DataContext dataContext2 = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
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
		public void GetLeafEntityKey()
		{
			
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			{
				using (DataContext dataContext1 = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				using (DataContext dataContext2 = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				{
					NaturalPersonEntity person1 = dataContext1.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));

					ExceptionAssert.Throw<System.ArgumentException>
					(
						() => dataContext2.GetLeafEntityKey (person1)
					);
				}
			}
		}


		[TestMethod]
		public void GetNormalizedEntityKey()
		{
			
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			{
				using (DataContext dataContext1 = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				using (DataContext dataContext2 = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
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
			
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			{
				using (DataContext dataContext1 = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				using (DataContext dataContext2 = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
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
			
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			{
				using (DataContext dataContext1 = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				using (DataContext dataContext2 = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
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
			
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			{
				using (DataContext dataContext1 = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				using (DataContext dataContext2 = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
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
			
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			{
				using (DataContext dataContext1 = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				using (DataContext dataContext2 = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
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
			
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			{
				using (DataContext dataContext1 = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				using (DataContext dataContext2 = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
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
			
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			{
				using (DataContext dataContext1 = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				using (DataContext dataContext2 = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
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
