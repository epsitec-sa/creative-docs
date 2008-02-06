//	Copyright © 2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer;

using System.Collections.Generic;

namespace Epsitec.Cresus.Core
{
	public sealed class CoreData : System.IDisposable
	{
		public CoreData()
		{
			this.infrastructure = new DbInfrastructure ();
			this.resolver = new ResolverImplementation (this);
		}

		public DataContext DataContext
		{
			get
			{
				return this.dataContext;
			}
		}

		public IEntityResolver Resolver
		{
			get
			{
				return this.resolver;
			}
		}

		public void SetupDatabase()
		{
			System.Diagnostics.Debug.Assert (this.infrastructure.IsConnectionOpen == false);
			System.Diagnostics.Debug.Assert (this.dataContext == null);

			DbAccess access = DbInfrastructure.CreateDatabaseAccess ("core");
			bool     empty  = false;

			access.IgnoreInitialConnectionErrors = true;
			access.CheckConnection = true;

			if (this.infrastructure.AttachToDatabase (access))
			{
				System.Diagnostics.Debug.WriteLine ("Connected to database");
			}
			else
			{
				System.Diagnostics.Debug.WriteLine ("Cannot connect to database");
				this.infrastructure.CreateDatabase (access);
				System.Diagnostics.Debug.WriteLine ("Created new database");
				empty = true;
			}

			this.dataContext = new DataContext (this.infrastructure);

			System.Diagnostics.Debug.Assert (this.infrastructure.IsConnectionOpen);

			if (empty)
			{
				this.CreateSchemas ();
				this.PopulateDatabase ();
			}
			else
			{
				this.VerifySchemas ();
			}
			
			System.Diagnostics.Debug.WriteLine ("Database ready");
		}

		private void VerifySchemas()
		{
		}

		private void CreateSchemas()
		{
			this.dataContext.CreateSchema<Epsitec.Cresus.AddressBook.Entities.AdressePersonneEntity> ();
		}

		private void PopulateDatabase()
		{
			AddressBook.Entities.PaysEntity paysCh = this.dataContext.CreateEntity<AddressBook.Entities.PaysEntity> ();

			paysCh.Code = "CH";
			paysCh.Nom = "Suisse";

			this.dataContext.SaveChanges ();
			System.Diagnostics.Debug.WriteLine ("Created CH-Suisse");

			int count = 0;

			foreach (AddressBook.Entities.LocalitéEntity localité in this.ReadNuPost (paysCh))
			{
				count++;
			}

			this.dataContext.SaveChanges ();
			System.Diagnostics.Debug.WriteLine (string.Format ("Created {0} entities", count));
			
		}

		public IEnumerable<AddressBook.Entities.LocalitéEntity> ReadNuPost(AddressBook.Entities.PaysEntity paysCh)
		{
			foreach (string line in System.IO.File.ReadAllLines (@"S:\Epsitec.Cresus\External\NUPOST.TXT", System.Text.Encoding.Default))
			{
				string[] values = line.Split ('\t');

				AddressBook.Entities.LocalitéEntity loc = this.dataContext.CreateEntity<AddressBook.Entities.LocalitéEntity> ();

				loc.Numéro = values[2];
				loc.Nom = values[5];
				loc.Pays = paysCh;
				
				yield return loc;
			}
		}



		#region IDisposable Members

		public void Dispose()
		{
			if (this.resolver != null)
			{
				this.resolver.Dispose ();
				this.resolver = null;
			}
			
			if (this.dataContext != null)
			{
				this.dataContext.Dispose ();
				this.dataContext = null;
			}
			
			if (this.infrastructure.IsConnectionOpen)
			{
				this.infrastructure.Dispose ();
			}
		}

		#endregion


		private class ResolverImplementation : IEntityResolver, System.IDisposable
		{
			public ResolverImplementation(CoreData data)
			{
				this.data = data;
			}

			#region IEntityResolver Members

			public IEnumerable<AbstractEntity> Resolve(AbstractEntity template)
			{
				Druid id = template.GetEntityStructuredTypeId ();
				yield break;
			}

			#endregion

			#region IDisposable Members

			public void Dispose()
			{
			}

			#endregion

			private readonly CoreData data;
		}

		private readonly DbInfrastructure infrastructure;
		private DataContext dataContext;
		private ResolverImplementation resolver;
	}
}
