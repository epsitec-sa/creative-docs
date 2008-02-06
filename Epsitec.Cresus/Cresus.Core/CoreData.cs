//	Copyright © 2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

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

			this.dataBrowser = new DataBrowser (this.infrastructure);
		}

		private void VerifySchemas()
		{
		}

		private void CreateSchemas()
		{
			this.dataContext.CreateSchema<Epsitec.Cresus.AddressBook.Entities.AdresseEntity> ();
			this.dataContext.CreateSchema<Epsitec.Cresus.AddressBook.Entities.AdressePersonneEntity> ();
		}

		private void PopulateDatabase()
		{
			AddressBook.Entities.PaysEntity paysCh = this.dataContext.CreateEntity<AddressBook.Entities.PaysEntity> ();

			paysCh.Code = "CH";
			paysCh.Nom = "Suisse";

			this.dataContext.SaveChanges ();
			System.Diagnostics.Debug.WriteLine ("Created CH-Suisse");

			List<AddressBook.Entities.LocalitéEntity> localités = new List<Epsitec.Cresus.AddressBook.Entities.LocalitéEntity> (this.ReadNuPost (paysCh));
			this.dataContext.SaveChanges ();

			List<AddressBook.Entities.TitrePersonneEntity> titres = new List<Epsitec.Cresus.AddressBook.Entities.TitrePersonneEntity> (this.ReadTitres ());
			this.dataContext.SaveChanges ();

			List<AddressBook.Entities.AdressePersonneEntity> personnes = new List<Epsitec.Cresus.AddressBook.Entities.AdressePersonneEntity> (this.ReadPersonnes (localités, titres));
			this.dataContext.SaveChanges ();
			
			System.Diagnostics.Debug.WriteLine (string.Format ("Created {0} x Localité, {1} x TitrePersonne, {2} x AdressePersonne", localités.Count, titres.Count, personnes.Count));
		}

		public IEnumerable<AddressBook.Entities.LocalitéEntity> ReadNuPost(AddressBook.Entities.PaysEntity paysCh)
		{
			foreach (string line in System.IO.File.ReadAllLines (@"S:\Epsitec.Cresus\External\NUPOST.TXT", System.Text.Encoding.Default))
			{
				string[] values = CoreData.Filter (line.Split ('\t'));

				AddressBook.Entities.LocalitéEntity loc = this.dataContext.CreateEmptyEntity<AddressBook.Entities.LocalitéEntity> ();

				loc.Numéro = values[2];
				loc.Nom = values[5];
				loc.Pays = paysCh;

				yield return loc;
			}
		}

		private static string[] Filter(string[] values)
		{
			for (int i = 0; i < values.Length; i++)
			{
				values[i] = TextLayout.ConvertToTaggedText (values[i].Trim ()); //.Replace ("|", "<br/>");
			}

			return values;
		}

		public IEnumerable<AddressBook.Entities.TitrePersonneEntity> ReadTitres()
		{
			foreach (string line in new string[] { "M.,Monsieur", "Mme,Madame", "Mlle,Mademoiselle", "MM.,Messieurs" })
			{
				string[] values = CoreData.Filter (line.Split (','));
				AddressBook.Entities.TitrePersonneEntity titre = this.dataContext.CreateEmptyEntity<AddressBook.Entities.TitrePersonneEntity> ();

				titre.IntituléCourt = values[0];
				titre.IntituléLong = values[1];

				yield return titre;
			}
		}

		public IEnumerable<AddressBook.Entities.AdressePersonneEntity> ReadPersonnes(List<AddressBook.Entities.LocalitéEntity> localités, List<AddressBook.Entities.TitrePersonneEntity> titres)
		{
			foreach (string line in System.IO.File.ReadAllLines (@"S:\Epsitec.Cresus\External\EPSITEC.CSV", System.Text.Encoding.Default))
			{
				string[] values = CoreData.Filter (line.Split (';'));

				if ((values.Length < 7) ||
					(values[5].Length == 0))
				{
					continue;
				}

				List<AddressBook.Entities.LocalitéEntity> localitéList = localités.FindAll (x => x.Numéro == values[5]);
				AddressBook.Entities.LocalitéEntity localité = localitéList.Find (x => string.Compare (x.Nom, values[6], true) == 0);

				if (localité == null)
				{
//-					System.Diagnostics.Debug.WriteLine ("No exact match found for " + values[5] + " " + values[6]);
					
					if (localitéList.Count > 0)
					{
						localité = localitéList[0];
					}
					else
					{
						System.Diagnostics.Debug.WriteLine ("ERROR: no match found for " + values[5] + " " + values[6]);
						continue;
					}
				}


				AddressBook.Entities.AdressePersonneEntity personne = this.dataContext.CreateEmptyEntity<AddressBook.Entities.AdressePersonneEntity> ();

				personne.Titre = titres.Find (x => string.Compare (x.IntituléLong, values[1], true) == 0) ?? titres.Find (x => x.IntituléCourt == "M.");
				personne.Nom = values[2];
				personne.Prénom = values[3];
				personne.Rue = values[4];
				personne.Localité = localité;

				yield return personne;
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

				if (id == AddressBook.Entities.AdressePersonneEntity.EntityStructuredTypeId)
				{
					AddressBook.Entities.AdressePersonneEntity example = template as AddressBook.Entities.AdressePersonneEntity;

					foreach (AddressBook.Entities.AdressePersonneEntity entity in this.data.DataContext.GetManagedEntities (e => e.GetEntityStructuredTypeId () == id))
					{
						if ((ResolverImplementation.Match (example.Titre, entity.Titre)) &&
							(ResolverImplementation.Match (example.Nom, entity.Nom)) &&
							(ResolverImplementation.Match (example.Prénom, entity.Prénom)) &&
							(ResolverImplementation.Match (example.Rue, entity.Rue)) &&
							(ResolverImplementation.Match (example.Localité, entity.Localité)))
						{
							yield return entity;
						}
					}

//-					this.data.dataBrowser.QueryByExample (transaction, example, query);
				}
				else if (id == AddressBook.Entities.LocalitéEntity.EntityStructuredTypeId)
				{
					AddressBook.Entities.LocalitéEntity example = template as AddressBook.Entities.LocalitéEntity;

					foreach (AddressBook.Entities.LocalitéEntity entity in this.data.DataContext.GetManagedEntities (e => e.GetEntityStructuredTypeId () == id))
					{
						if (entity.SearchValue.Contains (example.Résumé))
						{
							yield return entity;
						}
					}
				}

				yield break;
			}

			#endregion

			#region IDisposable Members

			public void Dispose()
			{
			}

			#endregion

			private static bool Match(string a, string b)
			{
				if (string.IsNullOrEmpty (a))
				{
					return true;
				}
				else if (string.IsNullOrEmpty (b))
				{
					return false;
				}
				else
				{
					return b.Contains (a);
				}
			}

			private static bool Match(AddressBook.Entities.LocalitéEntity a, AddressBook.Entities.LocalitéEntity b)
			{
				if (a == null)
				{
					return true;
				}
				else if (b == null)
				{
					return false;
				}
				else
				{
					return ResolverImplementation.Match (a.Résumé, b.Résumé);
				}
			}

			private static bool Match(AddressBook.Entities.TitrePersonneEntity a, AddressBook.Entities.TitrePersonneEntity b)
			{
				if (a == null)
				{
					return true;
				}
				else if (b == null)
				{
					return false;
				}
				else
				{
					return ResolverImplementation.Match (a.IntituléLong, b.IntituléLong);
				}
			}


			private readonly CoreData data;
		}

		private readonly DbInfrastructure infrastructure;
		private DataContext dataContext;
		private DataBrowser dataBrowser;
		private ResolverImplementation resolver;
	}
}
