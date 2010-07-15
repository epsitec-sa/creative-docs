using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;

using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Epsitec.Cresus.DataLayer
{


	[TestClass]
	public class UnitTestNullVirtualization
	{


		[ClassInitialize]
		public void Initialize(TestContext testContext)
		{
			TestHelper.Initialize ();
		}


		[TestMethod]
		public void CreateDatabase()
		{
			TestHelper.PrintStartTest ("Create database");

			this.CreateDatabaseHelper ();
		}


		private void CreateDatabaseHelper()
		{
			Database.CreateAndConnectToDatabase ();

			Assert.IsTrue (Database.DbInfrastructure.IsConnectionOpen);

			using (DataContext dataContext = this.CreateDataContext ())
			{
				Database2.PupulateDatabase (dataContext);
			}
		}


		[TestMethod]
		public void ModifyNullReferenceData()
		{
			using (DataContext dataContext = this.CreateDataContext ())
			{
				NaturalPersonEntity gertrude = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (2)));

				System.Diagnostics.Debug.Assert (gertrude != null);
				System.Diagnostics.Debug.Assert (!EntityNullReferenceVirtualizer.IsNullEntity (gertrude));
				System.Diagnostics.Debug.Assert (EntityNullReferenceVirtualizer.IsPatchedEntity (gertrude));
				System.Diagnostics.Debug.Assert (EntityNullReferenceVirtualizer.IsPatchedEntityStillUnchanged (gertrude));

				LanguageEntity language = gertrude.PreferredLanguage;

				System.Diagnostics.Debug.Assert (language != null);
				System.Diagnostics.Debug.Assert (EntityNullReferenceVirtualizer.IsPatchedEntity (language));
				System.Diagnostics.Debug.Assert (EntityNullReferenceVirtualizer.IsPatchedEntityStillUnchanged (language));
				System.Diagnostics.Debug.Assert (EntityNullReferenceVirtualizer.IsNullEntity (language));

				language.Code = "1337";
				language.Name = "1337 5|*34|<";

				System.Diagnostics.Debug.Assert (EntityNullReferenceVirtualizer.IsPatchedEntity (language));
				System.Diagnostics.Debug.Assert (!EntityNullReferenceVirtualizer.IsPatchedEntityStillUnchanged (language));
				System.Diagnostics.Debug.Assert (!EntityNullReferenceVirtualizer.IsNullEntity (language));

				dataContext.SaveChanges ();
			}

			using (DataContext dataContext = this.CreateDataContext ())
			{
				NaturalPersonEntity gertrude = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (2)));
				
				System.Diagnostics.Debug.Assert (gertrude != null);
				System.Diagnostics.Debug.Assert (!EntityNullReferenceVirtualizer.IsNullEntity (gertrude));
				System.Diagnostics.Debug.Assert (EntityNullReferenceVirtualizer.IsPatchedEntity (gertrude));
				System.Diagnostics.Debug.Assert (EntityNullReferenceVirtualizer.IsPatchedEntityStillUnchanged (gertrude));
								
				LanguageEntity language = dataContext.ResolveEntity<LanguageEntity> (new DbKey (new DbId (3)));

				System.Diagnostics.Debug.Assert (language != null);
				System.Diagnostics.Debug.Assert (!EntityNullReferenceVirtualizer.IsNullEntity (language));
				System.Diagnostics.Debug.Assert (EntityNullReferenceVirtualizer.IsPatchedEntity (language));
				System.Diagnostics.Debug.Assert (EntityNullReferenceVirtualizer.IsPatchedEntityStillUnchanged (language));
				System.Diagnostics.Debug.Assert (language.Code == "1337");
				System.Diagnostics.Debug.Assert (language.Name == "1337 5|*34|<");

				System.Diagnostics.Debug.Assert (gertrude.PreferredLanguage == language);
			}

			//this.CreateDatabaseHelper ();
		}


		[TestMethod]
		public void ModifyNullCollectionData()
		{
			//this.CreateDatabaseHelper ();
		}


		[TestMethod]
		public void ReplaceNullReferenceEntity()
		{

			//this.CreateDatabaseHelper ();
		}


		[TestMethod]
		public void ReplaceNullCollectionEntity()
		{

			//this.CreateDatabaseHelper ();
		}

		private DataContext CreateDataContext()
		{
			return new DataContext (Database.DbInfrastructure)
			{
				EnableNullVirtualization = true
			};
		}
                                             

	}


}
