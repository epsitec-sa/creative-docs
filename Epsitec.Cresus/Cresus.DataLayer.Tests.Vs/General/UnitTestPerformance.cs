//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX


using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Infrastructure;
using Epsitec.Cresus.DataLayer.Tests.Vs.Entities;
using Epsitec.Cresus.DataLayer.Tests.Vs.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;
using System.Linq;
using System.IO;
using Epsitec.Common.Support;


namespace Epsitec.Cresus.DataLayer.Tests.Vs.General
{


	[TestClass]
	public sealed class UnitTestPerformance
	{


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();

			File.Delete (UnitTestPerformance.logFile);

			if (UnitTestPerformance.createDatabase)
			{
				TestHelper.WriteStartTest ("Database creation", UnitTestPerformance.logFile);

				DbInfrastructureHelper.ResetTestDatabase ();

				TestHelper.MeasureAndWriteTime (
					"database population",
					UnitTestPerformance.logFile,
					() => DatabaseCreator1.PopulateDatabase (UnitTestPerformance.databaseSize),
					1
				);
			}
			else
			{
				TestHelper.WriteStartTest ("Database connection", UnitTestPerformance.logFile);
			}

			using (DbInfrastructure infrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				using (DbTransaction transaction = infrastructure.BeginTransaction (DbTransactionMode.ReadWrite))
				{
					foreach (DbTable table in infrastructure.FindDbTables (DbElementCat.Any))
					{
						foreach (DbIndex index in table.Indexes)
						{
							infrastructure.ResetIndex (transaction, table, index);
						}
					}
				}
			}
		}


		[TestMethod]
		public void RetrieveAllDataWithWarmup()
		{
			TestHelper.WriteStartTest ("Retrieve all data", UnitTestPerformance.logFile);

			this.RetrieveAllDataNoWarmup<AbstractPersonEntity> ();
			this.RetrieveAllDataNoWarmup<NaturalPersonEntity> ();
			this.RetrieveAllDataNoWarmup<LegalPersonEntity> ();
			this.RetrieveAllDataNoWarmup<LegalPersonTypeEntity> ();
			this.RetrieveAllDataNoWarmup<PersonTitleEntity> ();
			this.RetrieveAllDataNoWarmup<PersonGenderEntity> ();
			this.RetrieveAllDataNoWarmup<AbstractContactEntity> ();
			this.RetrieveAllDataNoWarmup<ContactRoleEntity> ();
			this.RetrieveAllDataNoWarmup<CommentEntity> ();
			this.RetrieveAllDataNoWarmup<MailContactEntity> ();
			this.RetrieveAllDataNoWarmup<AddressEntity> ();
			this.RetrieveAllDataNoWarmup<StreetEntity> ();
			this.RetrieveAllDataNoWarmup<PostBoxEntity> ();
			this.RetrieveAllDataNoWarmup<LocationEntity> ();
			this.RetrieveAllDataNoWarmup<RegionEntity> ();
			this.RetrieveAllDataNoWarmup<CountryEntity> ();
			this.RetrieveAllDataNoWarmup<TelecomContactEntity> ();
			this.RetrieveAllDataNoWarmup<TelecomTypeEntity> ();
			this.RetrieveAllDataNoWarmup<UriContactEntity> ();
			this.RetrieveAllDataWarmup<UriSchemeEntity> ();
			this.RetrieveAllDataWarmup<AbstractPersonEntity> ();
			this.RetrieveAllDataWarmup<NaturalPersonEntity> ();
			this.RetrieveAllDataWarmup<LegalPersonEntity> ();
			this.RetrieveAllDataWarmup<LegalPersonTypeEntity> ();
			this.RetrieveAllDataWarmup<PersonTitleEntity> ();
			this.RetrieveAllDataWarmup<PersonGenderEntity> ();
			this.RetrieveAllDataWarmup<AbstractContactEntity> ();
			this.RetrieveAllDataWarmup<ContactRoleEntity> ();
			this.RetrieveAllDataWarmup<CommentEntity> ();
			this.RetrieveAllDataWarmup<MailContactEntity> ();
			this.RetrieveAllDataWarmup<AddressEntity> ();
			this.RetrieveAllDataWarmup<StreetEntity> ();
			this.RetrieveAllDataWarmup<PostBoxEntity> ();
			this.RetrieveAllDataWarmup<LocationEntity> ();
			this.RetrieveAllDataWarmup<RegionEntity> ();
			this.RetrieveAllDataWarmup<CountryEntity> ();
			this.RetrieveAllDataWarmup<TelecomContactEntity> ();
			this.RetrieveAllDataWarmup<TelecomTypeEntity> ();
			this.RetrieveAllDataWarmup<UriContactEntity> ();
			this.RetrieveAllDataWarmup<UriSchemeEntity> ();
		}


		public void RetrieveAllDataNoWarmup<EntityType>() where EntityType : AbstractEntity, new ()
		{
			TestHelper.MeasureAndWriteTime (
				TestHelper.extendString (new EntityType ().GetType ().Name, 30) + "\twarmup: false",
				UnitTestPerformance.logFile,
				() =>
				{
					using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
					using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
					using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
					{
						dataContext.GetByExample<EntityType> (new EntityType ()).Count ();
					}
				},
				UnitTestPerformance.nbRuns
			);
		}


		public void RetrieveAllDataWarmup<EntityType>() where EntityType : AbstractEntity, new ()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				dataContext.GetByExample<EntityType> (new EntityType ()).Count ();

				TestHelper.MeasureAndWriteTime (
					TestHelper.extendString (new EntityType ().GetType ().Name, 30) + "\twarmup: true",
					UnitTestPerformance.logFile,
					() => dataContext.GetByExample<EntityType> (new EntityType ()).Count (),
					UnitTestPerformance.nbRuns
				);
			}
		}


		[TestMethod]
		public void GetUriContactWithGivenUriScheme()
		{
			TestHelper.WriteStartTest ("Retrieve uri contacts with uri scheme", UnitTestPerformance.logFile);

			this.GetUriContactWithGivenUriSchemeReferenceNoWarmup ();
			this.GetUriContactWithGivenUriSchemeValueNoWarmup ();
			this.GetUriContactWithGivenUriSchemeReferenceWarmup ();
			this.GetUriContactWithGivenUriSchemeValueWarmup ();
		}


		public void GetUriContactWithGivenUriSchemeReferenceNoWarmup()
		{
			TestHelper.MeasureAndWriteTime (
				TestHelper.extendString ("mode: reference", 20) + "\twarmup: false",
				UnitTestPerformance.logFile,
				() =>
				{
					using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
					using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
					using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
					{
						UriSchemeEntity uriScheme = dataContext.ResolveEntity<UriSchemeEntity> (new DbKey (new DbId (1000000001)));

						UriContactEntity example = new UriContactEntity ()
						{
							UriScheme = uriScheme,
						};

						dataContext.GetByExample<UriContactEntity> (example).Count ();
					}
				},
				UnitTestPerformance.nbRuns
			);
		}


		public void GetUriContactWithGivenUriSchemeReferenceWarmup()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				UriSchemeEntity uriScheme = dataContext.ResolveEntity<UriSchemeEntity> (new DbKey (new DbId (1000000001)));

				UriContactEntity example = new UriContactEntity ()
				{
					UriScheme = uriScheme,
				};

				dataContext.GetByExample<UriContactEntity> (example).Count ();

				TestHelper.MeasureAndWriteTime (
					TestHelper.extendString ("mode: reference", 20) + "\twarmup: true",
					UnitTestPerformance.logFile,
					() => dataContext.GetByExample<UriContactEntity> (example).Count (),
					UnitTestPerformance.nbRuns
				);
			}
		}


		public void GetUriContactWithGivenUriSchemeValueNoWarmup()
		{
			TestHelper.MeasureAndWriteTime (
				TestHelper.extendString ("mode: value", 20) + "\twarmup: false",
				UnitTestPerformance.logFile,
				() =>
				{
					using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
					using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
					using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
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
				},
				UnitTestPerformance.nbRuns
			);
		}


		public void GetUriContactWithGivenUriSchemeValueWarmup()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				UriContactEntity example = new UriContactEntity ()
				{
					UriScheme = new UriSchemeEntity ()
					{
						Name = "name1",
					},
				};

				dataContext.GetByExample<UriContactEntity> (example).Count ();

				TestHelper.MeasureAndWriteTime (
					TestHelper.extendString ("mode: value", 20) + "\twarmup: true",
					UnitTestPerformance.logFile,
					() => dataContext.GetByExample<UriContactEntity> (example).Count (),
					UnitTestPerformance.nbRuns
				);
			}
		}


		[TestMethod]
		public void GetLocationsGivenCountry()
		{
			TestHelper.WriteStartTest ("Retrieve locations given country", UnitTestPerformance.logFile);

			this.GetLocationsGivenCountryReferenceNoWarmup ();
			this.GetLocationsGivenCountryValueNoWarmup ();
			this.GetLocationsGivenCountryReferenceWarmup ();
			this.GetLocationsGivenCountryValueWarmup ();
		}


		public void GetLocationsGivenCountryReferenceNoWarmup()
		{
			TestHelper.MeasureAndWriteTime (
				TestHelper.extendString ("mode: reference", 20) + "\twarmup: false",
				UnitTestPerformance.logFile,
				() =>
				{
					using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
					using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
					using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
					{
						CountryEntity country = dataContext.ResolveEntity<CountryEntity> (new DbKey (new DbId (1000000001)));

						LocationEntity example = new LocationEntity ()
						{
							Country = country,
						};

						dataContext.GetByExample<LocationEntity> (example).Count ();
					}
				}, UnitTestPerformance.nbRuns
			);
		}


		public void GetLocationsGivenCountryReferenceWarmup()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				CountryEntity country = dataContext.ResolveEntity<CountryEntity> (new DbKey (new DbId (1000000001)));

				LocationEntity example = new LocationEntity ()
				{
					Country = country,
				};

				dataContext.GetByExample<LocationEntity> (example).Count ();

				TestHelper.MeasureAndWriteTime (
					TestHelper.extendString ("mode: reference", 20) + "\twarmup: true",
					UnitTestPerformance.logFile,
					() => dataContext.GetByExample<LocationEntity> (example).Count (),
					UnitTestPerformance.nbRuns
				);
			}
		}


		public void GetLocationsGivenCountryValueNoWarmup()
		{
			TestHelper.MeasureAndWriteTime (
				TestHelper.extendString ("mode: value", 20) + "\twarmup: false",
				UnitTestPerformance.logFile,
				() =>
				{
					using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
					using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
					using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
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
				}, UnitTestPerformance.nbRuns
			);
		}


		public void GetLocationsGivenCountryValueWarmup()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				LocationEntity example = new LocationEntity ()
				{
					Country = new CountryEntity ()
					{
						Name = "name1",
					},
				};

				dataContext.GetByExample<LocationEntity> (example).Count ();

				TestHelper.MeasureAndWriteTime (
					TestHelper.extendString ("mode: value", 20) + "\twarmup: true",
					UnitTestPerformance.logFile,
					() => dataContext.GetByExample<LocationEntity> (example).Count (),
					UnitTestPerformance.nbRuns
				);
			}
		}


		[TestMethod]
		public void GetLegalPersonsGivenType()
		{
			TestHelper.WriteStartTest ("Retrieve legal persons given type", UnitTestPerformance.logFile);

			this.GetLegalPersonsGivenTypeReferenceNoWarmup ();
			this.GetLegalPersonsGivenTypeValueNoWarmup ();
			this.GetLegalPersonsGivenTypeReferenceWarmup ();
			this.GetLegalPersonsGivenTypeValueWarmup ();
		}


		public void GetLegalPersonsGivenTypeReferenceNoWarmup()
		{
			TestHelper.MeasureAndWriteTime (
				TestHelper.extendString ("mode: reference", 20) + "\twarmup: false",
				UnitTestPerformance.logFile,
				() =>
				{
					using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
					using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
					using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
					{
						LegalPersonTypeEntity legalPersonType = dataContext.ResolveEntity<LegalPersonTypeEntity> (new DbKey (new DbId (1000000001)));

						LegalPersonEntity example = new LegalPersonEntity ()
						{
							LegalPersonType = legalPersonType,
						};

						dataContext.GetByExample<LegalPersonEntity> (example).Count ();
					}
				},
				UnitTestPerformance.nbRuns
			);
		}


		public void GetLegalPersonsGivenTypeReferenceWarmup()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				LegalPersonTypeEntity legalPersonType = dataContext.ResolveEntity<LegalPersonTypeEntity> (new DbKey (new DbId (1000000001)));

				LegalPersonEntity example = new LegalPersonEntity ()
				{
					LegalPersonType = legalPersonType,
				};

				dataContext.GetByExample<LegalPersonEntity> (example).Count ();

				TestHelper.MeasureAndWriteTime (
					TestHelper.extendString ("mode: reference", 20) + "\twarmup: true",
					UnitTestPerformance.logFile,
					() => dataContext.GetByExample<LegalPersonEntity> (example).Count (),
					UnitTestPerformance.nbRuns
				);
			}
		}


		public void GetLegalPersonsGivenTypeValueNoWarmup()
		{
			TestHelper.MeasureAndWriteTime (
				TestHelper.extendString ("mode: value", 20) + "\twarmup: false",
				UnitTestPerformance.logFile,
				() =>
				{
					using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
					using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
					using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
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
				},
				UnitTestPerformance.nbRuns
			);
		}


		public void GetLegalPersonsGivenTypeValueWarmup()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				LegalPersonEntity example = new LegalPersonEntity ()
				{
					LegalPersonType = new LegalPersonTypeEntity ()
					{
						Name = "name1",
					},
				};

				dataContext.GetByExample<LegalPersonEntity> (example).Count ();

				TestHelper.MeasureAndWriteTime (
					TestHelper.extendString ("mode: value", 20) + "\twarmup: true",
					UnitTestPerformance.logFile,
					() => dataContext.GetByExample<LegalPersonEntity> (example).Count (),
					UnitTestPerformance.nbRuns
				);
			}
		}


		[TestMethod]
		public void GetContactsGivenPerson()
		{
			TestHelper.WriteStartTest ("Retrieve contacts given person", UnitTestPerformance.logFile);

			this.GetContactsGivenPersonReferenceNoWarmup ();
			this.GetContactsGivenPersonValueNoWarmup ();
			this.GetContactsGivenPersonReferenceWarmup ();
			this.GetContactsGivenPersonValueWarmup ();
		}


		public void GetContactsGivenPersonReferenceNoWarmup()
		{
			TestHelper.MeasureAndWriteTime (
				TestHelper.extendString ("mode: reference", 20) + "\twarmup: false",
				UnitTestPerformance.logFile,
				() =>
				{
					using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
					using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
					using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
					{
						NaturalPersonEntity naturalPerson = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));

						AbstractContactEntity example = new AbstractContactEntity ()
						{
							NaturalPerson = naturalPerson,
						};

						dataContext.GetByExample<AbstractContactEntity> (example).Count ();
					}
				},
				UnitTestPerformance.nbRuns
			);
		}


		public void GetContactsGivenPersonReferenceWarmup()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				NaturalPersonEntity naturalPerson = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));

				AbstractContactEntity example = new AbstractContactEntity ()
				{
					NaturalPerson = naturalPerson,
				};

				dataContext.GetByExample<AbstractContactEntity> (example).Count ();

				TestHelper.MeasureAndWriteTime (
					TestHelper.extendString ("mode: reference", 20) + "\twarmup: true",
					UnitTestPerformance.logFile,
					() => dataContext.GetByExample<AbstractContactEntity> (example).Count (),
					UnitTestPerformance.nbRuns
				);
			}
		}


		public void GetContactsGivenPersonValueNoWarmup()
		{
			TestHelper.MeasureAndWriteTime (
				TestHelper.extendString ("mode: value", 20) + "\twarmup: false",
				UnitTestPerformance.logFile,
				() =>
				{
					using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
					using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
					using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
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
				},
				UnitTestPerformance.nbRuns
			);
		}


		public void GetContactsGivenPersonValueWarmup()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				AbstractContactEntity example = new AbstractContactEntity ()
				{
					NaturalPerson = new NaturalPersonEntity ()
					{
						Lastname = "lastname1",
					}
				};

				dataContext.GetByExample<AbstractContactEntity> (example).Count ();

				TestHelper.MeasureAndWriteTime (
					TestHelper.extendString ("mode: value", 20) + "\twarmup: true",
					UnitTestPerformance.logFile,
					() => dataContext.GetByExample<AbstractContactEntity> (example).Count (),
					UnitTestPerformance.nbRuns
				);
			}
		}


		[TestMethod]
		public void GetPersonGivenLocation()
		{
			TestHelper.WriteStartTest ("Retrieve person given location", UnitTestPerformance.logFile);

			this.GetPersonGivenLocationReferenceNoWarmup ();
			this.GetPersonGivenLocationValueNoWarmup ();
			this.GetPersonGivenLocationReferenceWarmup ();
			this.GetPersonGivenLocationValueWarmup ();
		}


		public void GetPersonGivenLocationReferenceNoWarmup()
		{
			TestHelper.MeasureAndWriteTime (
				TestHelper.extendString ("mode: reference", 20) + "\twarmup: false",
				UnitTestPerformance.logFile,
				() =>
				{
					using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
					using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
					using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
					{
						LocationEntity location = dataContext.ResolveEntity<LocationEntity> (new DbKey (new DbId (1000000001)));

						NaturalPersonEntity example = new NaturalPersonEntity ();
						example.Contacts.Add (new MailContactEntity ()
						{
							Address = new AddressEntity ()
							{
								Location = location,
							}
						}
						);

						dataContext.GetByExample<NaturalPersonEntity> (example).Count ();
					}
				},
				UnitTestPerformance.nbRuns
			);
		}


		public void GetPersonGivenLocationReferenceWarmup()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				LocationEntity location = dataContext.ResolveEntity<LocationEntity> (new DbKey (new DbId (1000000001)));

				NaturalPersonEntity example = new NaturalPersonEntity ();
				example.Contacts.Add (new MailContactEntity ()
				{
					Address = new AddressEntity ()
					{
						Location = location,
					}
				}
				);

				dataContext.GetByExample<NaturalPersonEntity> (example).Count ();

				TestHelper.MeasureAndWriteTime (
					TestHelper.extendString ("mode: reference", 20) + "\twarmup: true",
					UnitTestPerformance.logFile,
					() => dataContext.GetByExample<NaturalPersonEntity> (example).Count (),
					UnitTestPerformance.nbRuns
				);
			}
		}


		public void GetPersonGivenLocationValueNoWarmup()
		{
			TestHelper.MeasureAndWriteTime (
				TestHelper.extendString ("mode: value", 20) + "\twarmup: false",
				UnitTestPerformance.logFile,
				() =>
				{
					using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
					using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
					using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
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
				},
				UnitTestPerformance.nbRuns
			);
		}


		public void GetPersonGivenLocationValueWarmup()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
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

				TestHelper.MeasureAndWriteTime (
					TestHelper.extendString ("mode: value", 20) + "\twarmup: true",
					UnitTestPerformance.logFile,
					() => dataContext.GetByExample<NaturalPersonEntity> (example).Count (),
					UnitTestPerformance.nbRuns
				);
			}
		}


		[TestMethod]
		public void GetAddressGivenLegalPerson()
		{
			TestHelper.WriteStartTest ("Retrieve address given legal person", UnitTestPerformance.logFile);

			this.GetAddressGivenLegalPersonReferenceNoWarmup ();
			this.GetAddressGivenLegalPersonValueNoWarmup ();
			this.GetAddressGivenLegalPersonReferenceWarmup ();
			this.GetAddressGivenLegalPersonValueWarmup ();
		}


		public void GetAddressGivenLegalPersonReferenceNoWarmup()
		{
			TestHelper.MeasureAndWriteTime (
				TestHelper.extendString ("mode: reference", 20) + "\twarmup: false",
				UnitTestPerformance.logFile,
				() =>
				{
					using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
					using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
					using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
					{
						LegalPersonEntity legalPerson = dataContext.ResolveEntity<LegalPersonEntity> (new DbKey (new DbId (1000000000 + UnitTestPerformance.legalPersonId[UnitTestPerformance.databaseSize])));

						MailContactEntity example = new MailContactEntity ()
						{
							LegalPerson = legalPerson,
						};

						dataContext.GetByExample<MailContactEntity> (example).Select (c => c.Address).Count ();
					}
				},
				UnitTestPerformance.nbRuns
			);
		}


		public void GetAddressGivenLegalPersonReferenceWarmup()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				LegalPersonEntity legalPerson = dataContext.ResolveEntity<LegalPersonEntity> (new DbKey (new DbId (1000000000 + UnitTestPerformance.legalPersonId[UnitTestPerformance.databaseSize])));

				MailContactEntity example = new MailContactEntity ()
				{
					LegalPerson = legalPerson,
				};

				dataContext.GetByExample<MailContactEntity> (example).Select (c => c.Address).Count ();

				TestHelper.MeasureAndWriteTime (
					TestHelper.extendString ("mode: reference", 20) + "\twarmup: true",
					UnitTestPerformance.logFile,
					() => dataContext.GetByExample<MailContactEntity> (example).Select (c => c.Address).Count (),
					UnitTestPerformance.nbRuns
				);
			}
		}


		public void GetAddressGivenLegalPersonValueNoWarmup()
		{
			TestHelper.MeasureAndWriteTime (
				TestHelper.extendString ("mode: value", 20) + "\twarmup: false",
				UnitTestPerformance.logFile,
				() =>
				{
					using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
					using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
					using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
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
				},
				UnitTestPerformance.nbRuns
			);
		}


		public void GetAddressGivenLegalPersonValueWarmup()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				MailContactEntity example = new MailContactEntity ()
				{
					LegalPerson = new LegalPersonEntity ()
					{
						Name = "name1",
					}
				};

				dataContext.GetByExample<MailContactEntity> (example).Select (c => c.Address).Count ();

				TestHelper.MeasureAndWriteTime (
					TestHelper.extendString ("mode: value", 20) + "\twarmup: true",
					UnitTestPerformance.logFile,
					() => dataContext.GetByExample<MailContactEntity> (example).Select (c => c.Address).Count (),
					UnitTestPerformance.nbRuns
				);
			}
		}


		[TestMethod]
		public void DeleteEntities()
		{
			TestHelper.WriteStartTest ("DeleteEntities", UnitTestPerformance.logFile);

			if (UnitTestPerformance.runDeleteTests)
			{
				this.DeleteEntity<NaturalPersonEntity> (10);
				this.DeleteEntity<AbstractContactEntity> (10);
				this.DeleteEntity<RegionEntity> (10);
				this.DeleteEntity<ContactRoleEntity> (10);
			}
		}


		public void DeleteEntity<EntityType>(long id) where EntityType : AbstractEntity, new ()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				dataInfrastructure.SchemaEngine.GetReferencingFields (Druid.Parse ("[J1AV]"));

				EntityType entity = dataContext.ResolveEntity<EntityType> (new DbKey (new DbId (1000000000 + id)));

				TestHelper.MeasureAndWriteTime (
					TestHelper.extendString (entity.GetType ().Name, 30),
					UnitTestPerformance.logFile,
					() =>
					{
						dataContext.DeleteEntity (entity);
						dataContext.SaveChanges ();
					},
					1
				);
			}
		}


		private readonly static Dictionary<DatabaseSize, long> legalPersonId = new Dictionary<DatabaseSize, long> ()
		{
			{DatabaseSize.Small,	101},
			{DatabaseSize.Medium,  1001},
			{DatabaseSize.Large,  10001},
			{DatabaseSize.Huge,  100001},
		};


		private readonly static int nbRuns = 5;


		private readonly static bool createDatabase = true;


		private readonly static bool runDeleteTests = true;


		private readonly static DatabaseSize databaseSize = DatabaseSize.Small;


		private readonly static string logFile = @"results.txt";


	}
}
