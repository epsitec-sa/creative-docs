//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX


using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.PerformanceTests;

using System.Linq;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Cresus.PerformanceTests.Entities;


namespace Epsitec.Cresus.PerformanceTests.Entity
{


	internal sealed class TestPerformance : System.IDisposable
	{


		public TestPerformance(bool createAndPopulateDatabase)
		{
			TestHelper.Initialize ();

			if (createAndPopulateDatabase)
			{
				DatabaseHelper.CreateAndConnectToDatabase ();
				DatabaseCreator.PopulateDatabase (DatabaseSize.Small);
			}
			else
			{
				DatabaseHelper.ConnectToDatabase ();
			}

			this.dbInfrastructure = DatabaseHelper.DbInfrastructure;
		}


		public DbInfrastructure DbInfrastructure
		{
			get
			{
				return this.dbInfrastructure;
			}
		}


		public void RetrieveNaturalPerson()
		{
			using (DataContext context =  new DataContext(this.dbInfrastructure))
			{
				System.Diagnostics.Trace.WriteLine ("About to retrieve a natural person entity");
				System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch ();

				watch.Start ();

				var key    = new DbKey (new DbId (1000000000040));
				var person = context.ResolveEntity<NaturalPersonEntity> (key);

				string.Concat (
					person.Firstname,
					person.Lastname);

				watch.Stop ();
				System.Diagnostics.Trace.WriteLine ("Operation took " + watch.ElapsedMilliseconds + " ms");
			}
		}


		public void RetrieveLocation()
		{
			using (DataContext context =  new DataContext(this.dbInfrastructure))
			{
				System.Diagnostics.Trace.WriteLine ("About to retrieve a location entity");
				System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch ();

				watch.Start ();

				var key    = new DbKey (new DbId (1000000000040));
				var person = context.ResolveEntity<LocationEntity> (key);

				watch.Stop ();
				System.Diagnostics.Trace.WriteLine ("Operation took " + watch.ElapsedMilliseconds + " ms");
			}
		}


		public void RetrieveAllData()
		{
			this.RetrieveAllData<AbstractPersonEntity> ();
			this.RetrieveAllData<NaturalPersonEntity> ();
			this.RetrieveAllData<LegalPersonEntity> ();
			this.RetrieveAllData<LegalPersonTypeEntity> ();
			this.RetrieveAllData<PersonTitleEntity> ();
			this.RetrieveAllData<PersonGenderEntity> ();
			this.RetrieveAllData<AbstractContactEntity> ();
			this.RetrieveAllData<ContactRoleEntity> ();
			this.RetrieveAllData<CommentEntity> ();
			this.RetrieveAllData<MailContactEntity> ();
			this.RetrieveAllData<AddressEntity> ();
			this.RetrieveAllData<StreetEntity> ();
			this.RetrieveAllData<PostBoxEntity> ();
			this.RetrieveAllData<LocationEntity> ();
			this.RetrieveAllData<RegionEntity> ();
			this.RetrieveAllData<CountryEntity> ();
			this.RetrieveAllData<TelecomContactEntity> ();
			this.RetrieveAllData<TelecomTypeEntity> ();
			this.RetrieveAllData<UriContactEntity> ();
			this.RetrieveAllData<UriSchemeEntity> ();
		}


		public void RetrieveAllData<EntityType>() where EntityType : AbstractEntity, new ()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				dataContext.GetByExample<EntityType> (new EntityType ()).Count ();
			};
		}


		public void RetrieveRequestedData()
		{
			this.GetUriContactWithGivenUriSchemeReference ();
			this.GetUriContactWithGivenUriSchemeValue ();

			this.GetLocationsGivenCountryReference ();
			this.GetLocationsGivenCountryValue ();

			this.GetLegalPersonsGivenTypeReference ();
			this.GetLegalPersonsGivenTypeValue ();

			this.GetContactsGivenPersonReference ();
			this.GetContactsGivenPersonValue ();

			this.GetPersonGivenLocationReference ();
			this.GetPersonGivenLocationValue ();

			this.GetAddressGivenLegalPersonReference ();
			this.GetAddressGivenLegalPersonValue ();
		}


		public void GetUriContactWithGivenUriSchemeReference()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				UriContactEntity example = new UriContactEntity ()
				{
					UriScheme = dataContext.ResolveEntity<UriSchemeEntity> (new DbKey (new DbId (1000000000001))),
				};

				dataContext.GetByExample<UriContactEntity> (example).Count ();
			}
		}


		public void GetUriContactWithGivenUriSchemeValue()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				UriContactEntity example = new UriContactEntity ()
				{
					UriScheme = new UriSchemeEntity ()
					{
						Name = "name1",
					},
				};

				dataContext.GetByExample<UriContactEntity> (example).Count ();
			}
		}


		public void GetLocationsGivenCountryReference()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				LocationEntity example = new LocationEntity ()
				{
					Country = dataContext.ResolveEntity<CountryEntity> (new DbKey (new DbId (1000000000001))),
				};

				dataContext.GetByExample<LocationEntity> (example).Count ();
			}
		}


		public void GetLocationsGivenCountryValue()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				LocationEntity example = new LocationEntity ()
				{
					Country = new CountryEntity ()
					{
						Name = "name1",
					},
				};

				dataContext.GetByExample<LocationEntity> (example).Count ();
			}
		}


		public void GetLegalPersonsGivenTypeReference()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				LegalPersonEntity example = new LegalPersonEntity ()
				{
					LegalPersonType = dataContext.ResolveEntity<LegalPersonTypeEntity> (new DbKey (new DbId (1000000000001))),
				};

				dataContext.GetByExample<LegalPersonEntity> (example).Count ();
			}
		}


		public void GetLegalPersonsGivenTypeValue()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				LegalPersonEntity example = new LegalPersonEntity ()
				{
					LegalPersonType = new LegalPersonTypeEntity ()
					{
						Name = "name1",
					},
				};

				dataContext.GetByExample<LegalPersonEntity> (example).Count ();
			}
		}


		public void GetContactsGivenPersonReference()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				AbstractContactEntity example = new AbstractContactEntity ()
				{
					NaturalPerson = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000000001))),
				};

				dataContext.GetByExample<AbstractContactEntity> (example).Count ();
			}
		}


		public void GetContactsGivenPersonValue()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				AbstractContactEntity example = new AbstractContactEntity ()
				{
					NaturalPerson = new NaturalPersonEntity ()
					{
						Lastname = "lastname1",
					}
				};

				dataContext.GetByExample<AbstractContactEntity> (example).Count ();
			}
		}


		public void GetPersonGivenLocationReference()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity example = new NaturalPersonEntity ();
				example.Contacts.Add (new MailContactEntity ()
				{
					Address = new AddressEntity ()
					{
						Location = dataContext.ResolveEntity<LocationEntity> (new DbKey (new DbId (1000000000001))),
					}
				}
				);

				dataContext.GetByExample<NaturalPersonEntity> (example).Count ();
			}
		}


		public void GetPersonGivenLocationValue()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity example = new NaturalPersonEntity ();
				example.Contacts.Add (new MailContactEntity ()
				{
					Address = new AddressEntity ()
					{
						Location = new LocationEntity ()
						{
							Name = "name1",
						}
					}
				}
				);

				dataContext.GetByExample<NaturalPersonEntity> (example).Count ();
			}
		}


		public void GetAddressGivenLegalPersonReference()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				MailContactEntity example = new MailContactEntity ()
				{
					LegalPerson = dataContext.ResolveEntity<LegalPersonEntity> (new DbKey (new DbId (1000000010001))),
				};

				dataContext.GetByExample<MailContactEntity> (example).Select (c => c.Address).Count ();
			}
		}


		public void GetAddressGivenLegalPersonValue()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				MailContactEntity example = new MailContactEntity ()
				{
					LegalPerson = new LegalPersonEntity ()
					{
						Name = "name1",
					}
				};

				dataContext.GetByExample<MailContactEntity> (example).Select (c => c.Address).Count ();
			}
		}


		#region IDisposable Members


		public void Dispose()
		{
			if (this.dbInfrastructure != null)
			{
				this.dbInfrastructure.Dispose ();
				this.dbInfrastructure = null;
			}
		}


		#endregion



		private DbInfrastructure dbInfrastructure;
	}


}
