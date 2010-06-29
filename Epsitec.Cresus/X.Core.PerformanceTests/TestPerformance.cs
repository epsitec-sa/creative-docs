//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Cresus.Core.Data;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer;
using System.Linq;
using Epsitec.Common.Support.EntityEngine;

namespace Epsitec.Cresus.Core
{
	public class TestPerformance : System.IDisposable
	{
		public TestPerformance(bool createAndPopulateDatabase)
		{
			TestHelper.Initialize ();

			if (createAndPopulateDatabase)
			{
				Epsitec.Cresus.DataLayer.Database.CreateAndConnectToDatabase ();
				Database1.PopulateDatabase (DatabaseSize.Large);
			}
			else
			{
				Epsitec.Cresus.DataLayer.Database.ConnectToDatabase ();
			}

			this.dbInfrastructure = Epsitec.Cresus.DataLayer.Database.DbInfrastructure;
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
			using (DataContext context =  new DataContext (this.dbInfrastructure))
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
			using (DataContext context =  new DataContext (this.dbInfrastructure))
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
			this.RetrieveAllData<AbstractPersonEntity> (false);
			this.RetrieveAllData<NaturalPersonEntity> (false);
			this.RetrieveAllData<LegalPersonEntity> (false);
			this.RetrieveAllData<LegalPersonTypeEntity> (false);
			this.RetrieveAllData<PersonTitleEntity> (false);
			this.RetrieveAllData<PersonGenderEntity> (false);
			this.RetrieveAllData<AbstractContactEntity> (false);
			this.RetrieveAllData<ContactRoleEntity> (false);
			this.RetrieveAllData<CommentEntity> (false);
			this.RetrieveAllData<MailContactEntity> (false);
			this.RetrieveAllData<AddressEntity> (false);
			this.RetrieveAllData<StreetEntity> (false);
			this.RetrieveAllData<PostBoxEntity> (false);
			this.RetrieveAllData<LocationEntity> (false);
			this.RetrieveAllData<RegionEntity> (false);
			this.RetrieveAllData<CountryEntity> (false);
			this.RetrieveAllData<TelecomContactEntity> (false);
			this.RetrieveAllData<TelecomTypeEntity> (false);
			this.RetrieveAllData<UriContactEntity> (false);
			this.RetrieveAllData<UriSchemeEntity> (false);
		}


		public void RetrieveAllData<EntityType>(bool bulkMode) where EntityType : AbstractEntity, new ()
		{
			using (DataContext dataContext = new DataContext (Epsitec.Cresus.DataLayer.Database.DbInfrastructure, bulkMode))
			{
				DataBrowser repository = new DataBrowser (dataContext);
				repository.GetByExample<EntityType> (new EntityType ()).Count ();
			};
		}


		public void RetrieveRequestedData()
		{
			this.GetUriContactWithGivenUriSchemeReference (false);
			this.GetUriContactWithGivenUriSchemeValue (false);

			this.GetLocationsGivenCountryReference (false);
			this.GetLocationsGivenCountryValue (false);

			this.GetLegalPersonsGivenTypeReference (false);
			this.GetLegalPersonsGivenTypeValue (false);

			this.GetContactsGivenPersonReference (false);
			this.GetContactsGivenPersonValue (false);

			this.GetPersonGivenLocationReference (false);
			this.GetPersonGivenLocationValue (false);

			this.GetAddressGivenLegalPersonReference (false);
			this.GetAddressGivenLegalPersonValue (false);

			this.GetAddressReferencers (false);
			
			this.GetLegalPersonReferencers (false);
		}


		public void GetUriContactWithGivenUriSchemeReference(bool bulkMode)
		{
			using (DataContext dataContext = new DataContext (Epsitec.Cresus.DataLayer.Database.DbInfrastructure, bulkMode))
			{
				DataBrowser repository = new DataBrowser (dataContext);

				UriContactEntity example = new UriContactEntity ()
				{
					UriScheme = dataContext.ResolveEntity<UriSchemeEntity> (new DbKey (new DbId (1000000000001))),
				};

				repository.GetByExample<UriContactEntity> (example).Count ();
			}
		}


		public void GetUriContactWithGivenUriSchemeValue(bool bulkMode)
		{
			using (DataContext dataContext = new DataContext (Epsitec.Cresus.DataLayer.Database.DbInfrastructure, bulkMode))
			{
				DataBrowser repository = new DataBrowser (dataContext);

				UriContactEntity example = new UriContactEntity ()
				{
					UriScheme = new UriSchemeEntity ()
					{
						Name = "name1",
					},
				};

				repository.GetByExample<UriContactEntity> (example).Count ();
			}
		}


		public void GetLocationsGivenCountryReference(bool bulkMode)
		{
			using (DataContext dataContext = new DataContext (Epsitec.Cresus.DataLayer.Database.DbInfrastructure, bulkMode))
			{
				DataBrowser repository = new DataBrowser (dataContext);

				LocationEntity example = new LocationEntity ()
				{
					Country = dataContext.ResolveEntity<CountryEntity> (new DbKey (new DbId (1000000000001))),
				};

				repository.GetByExample<LocationEntity> (example).Count ();
			}
		}


		public void GetLocationsGivenCountryValue(bool bulkMode)
		{
			using (DataContext dataContext = new DataContext (Epsitec.Cresus.DataLayer.Database.DbInfrastructure, bulkMode))
			{
				DataBrowser repository = new DataBrowser (dataContext);

				LocationEntity example = new LocationEntity ()
				{
					Country = new CountryEntity ()
					{
						Name = "name1",
					},
				};

				repository.GetByExample<LocationEntity> (example).Count ();
			}
		}


		public void GetLegalPersonsGivenTypeReference(bool bulkMode)
		{
			using (DataContext dataContext = new DataContext (Epsitec.Cresus.DataLayer.Database.DbInfrastructure, bulkMode))
			{
				DataBrowser repository = new DataBrowser (dataContext);

				LegalPersonEntity example = new LegalPersonEntity ()
				{
					LegalPersonType = dataContext.ResolveEntity<LegalPersonTypeEntity> (new DbKey (new DbId (1000000000001))),
				};

				repository.GetByExample<LegalPersonEntity> (example).Count ();
			}
		}


		public void GetLegalPersonsGivenTypeValue(bool bulkMode)
		{
			using (DataContext dataContext = new DataContext (Epsitec.Cresus.DataLayer.Database.DbInfrastructure, bulkMode))
			{
				DataBrowser repository = new DataBrowser (dataContext);

				LegalPersonEntity example = new LegalPersonEntity ()
				{
					LegalPersonType = new LegalPersonTypeEntity ()
					{
						Name = "name1",
					},
				};

				repository.GetByExample<LegalPersonEntity> (example).Count ();
			}
		}


		public void GetContactsGivenPersonReference(bool bulkMode)
		{
			using (DataContext dataContext = new DataContext (Epsitec.Cresus.DataLayer.Database.DbInfrastructure, bulkMode))
			{
				DataBrowser repository = new DataBrowser (dataContext);

				AbstractContactEntity example = new AbstractContactEntity ()
				{
					NaturalPerson = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000000001))),
				};

				repository.GetByExample<AbstractContactEntity> (example).Count ();
			}
		}


		public void GetContactsGivenPersonValue(bool bulkMode)
		{
			using (DataContext dataContext = new DataContext (Epsitec.Cresus.DataLayer.Database.DbInfrastructure, bulkMode))
			{
				DataBrowser repository = new DataBrowser (dataContext);

				AbstractContactEntity example = new AbstractContactEntity ()
				{
					NaturalPerson = new NaturalPersonEntity ()
					{
						Lastname = "lastname1",
					}
				};

				repository.GetByExample<AbstractContactEntity> (example).Count ();
			}
		}


		public void GetPersonGivenLocationReference(bool bulkMode)
		{
			using (DataContext dataContext = new DataContext (Epsitec.Cresus.DataLayer.Database.DbInfrastructure, bulkMode))
			{
				DataBrowser repository = new DataBrowser (dataContext);

				NaturalPersonEntity example = new NaturalPersonEntity ();
				example.Contacts.Add (new MailContactEntity ()
				{
					Address = new AddressEntity ()
					{
						Location = dataContext.ResolveEntity<LocationEntity> (new DbKey (new DbId (1000000000001))),
					}
				}
				);

				repository.GetByExample<NaturalPersonEntity> (example).Count ();
			}
		}


		public void GetPersonGivenLocationValue(bool bulkMode)
		{
			using (DataContext dataContext = new DataContext (Epsitec.Cresus.DataLayer.Database.DbInfrastructure, bulkMode))
			{
				DataBrowser repository = new DataBrowser (dataContext);

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

				repository.GetByExample<NaturalPersonEntity> (example).Count ();
			}
		}


		public void GetAddressGivenLegalPersonReference(bool bulkMode)
		{
			using (DataContext dataContext = new DataContext (Epsitec.Cresus.DataLayer.Database.DbInfrastructure, bulkMode))
			{
				DataBrowser repository = new DataBrowser (dataContext);

				MailContactEntity example = new MailContactEntity ()
				{
					LegalPerson = dataContext.ResolveEntity<LegalPersonEntity> (new DbKey (new DbId (1000000010001))),
				};

				repository.GetByExample<MailContactEntity> (example).Select (c => c.Address).Count ();
			}
		}


		public void GetAddressGivenLegalPersonValue(bool bulkMode)
		{
			using (DataContext dataContext = new DataContext (Epsitec.Cresus.DataLayer.Database.DbInfrastructure, bulkMode))
			{
				DataBrowser repository = new DataBrowser (dataContext);

				MailContactEntity example = new MailContactEntity ()
				{
					LegalPerson = new LegalPersonEntity ()
					{
						Name = "name1",
					}
				};

				repository.GetByExample<MailContactEntity> (example).Select (c => c.Address).Count ();
			}
		}


		public void GetAddressReferencers(bool bulkMode)
		{
			using (DataContext dataContext = new DataContext (Epsitec.Cresus.DataLayer.Database.DbInfrastructure, bulkMode))
			{
				DataBrowser repository = new DataBrowser (dataContext);

				AddressEntity address = dataContext.ResolveEntity<AddressEntity> (new DbKey (new DbId (1000000000001)));

				repository.GetReferencers (address).Count ();
			}
		}


		public void GetLegalPersonReferencers(bool bulkMode)
		{
			using (DataContext dataContext = new DataContext (Epsitec.Cresus.DataLayer.Database.DbInfrastructure, bulkMode))
			{
				DataBrowser repository = new DataBrowser (dataContext);

				LegalPersonEntity person = dataContext.ResolveEntity<LegalPersonEntity> (new DbKey (new DbId (1000000010001)));

				repository.GetReferencers (person).Count ();
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
