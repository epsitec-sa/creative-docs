//	Copyright © 2008-2010, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core
{
	public sealed partial class CoreData
	{
		public IEntityResolver Resolver
		{
			get
			{
				return this.resolver;
			}
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

			List<Mai2008.Entities.ClientEntity> personnes = new List<Epsitec.Cresus.Mai2008.Entities.ClientEntity> (this.ReadPersonnes (localités, titres));
			this.dataContext.SaveChanges ();

			List<Mai2008.Entities.FactureEntity> factures = new List<Epsitec.Cresus.Mai2008.Entities.FactureEntity> ();
			Mai2008.Entities.FactureEntity facture = this.dataContext.CreateEmptyEntity<Mai2008.Entities.FactureEntity> ();
			Mai2008.Entities.LigneFactureEntity ligne1 = this.dataContext.CreateEmptyEntity<Mai2008.Entities.LigneFactureEntity> ();
			Mai2008.Entities.LigneFactureEntity ligne2 = this.dataContext.CreateEmptyEntity<Mai2008.Entities.LigneFactureEntity> ();
			Mai2008.Entities.ArticleEntity article1 = this.dataContext.CreateEmptyEntity<Mai2008.Entities.ArticleEntity> ();
			Mai2008.Entities.ArticleEntity article2 = this.dataContext.CreateEmptyEntity<Mai2008.Entities.ArticleEntity> ();
			Mai2008.Entities.ArticleEntity article3 = this.dataContext.CreateEmptyEntity<Mai2008.Entities.ArticleEntity> ();

			ligne1.Article = article1;
			ligne1.Quantité = 2;

			ligne2.Article = article2;
			ligne2.Quantité = 1;

			article1.Désignation = "Crésus Comptabilité Pro";
			article1.Numéro = "CCPRO";
			article1.Prix = 480.00M;

			article2.Désignation = "Crésus Facturation Largo";
			article2.Numéro = "CFLGO";
			article2.Prix = 960.00M;

			article3.Désignation = "Blupi à la Maison";
			article3.Numéro = "B.HOME";
			article3.Prix = 39.00M;

			facture.Objet = new FormattedText ("Crésus Comptabilité <i>Pro</i>");
			facture.AdresseFacturation = personnes[personnes.Count-1];
			facture.Lignes.Add (ligne1);
			facture.Lignes.Add (ligne2);
			factures.Add (facture);
			
			this.dataContext.SaveChanges ();
			
			System.Diagnostics.Debug.WriteLine (string.Format ("Created {0} x Localité, {1} x TitrePersonne, {2} x AdressePersonne", localités.Count, titres.Count, personnes.Count));
		}

		private void ReloadDatabase()
		{
			DataBrowser browser = new DataBrowser (this.infrastructure);
			KeyValuePair<string, AbstractEntity>[] entities = new KeyValuePair<string, AbstractEntity>[]
			{
				new KeyValuePair<string, AbstractEntity> ("[8V17]", new Mai2008.Entities.ClientEntity ()),
				new KeyValuePair<string, AbstractEntity> ("[9VA2]", new Mai2008.Entities.FactureEntity ()),
				new KeyValuePair<string, AbstractEntity> ("[9VAK]", new Mai2008.Entities.ArticleEntity ()),
//-				new KeyValuePair<string, AbstractEntity> ("[8V19]", new AddressBook.Entities.LocalitéEntity ())
			};

			

			foreach (var item in entities)
			{
				AbstractEntity entity = item.Value;

				System.Diagnostics.Debug.WriteLine ("Loading " + entity.GetType ().Name);
				System.Diagnostics.Debug.WriteLine ("=============================================");

				DataQuery query = new DataQuery ();

				query.Columns.Add (new DataQueryColumn (EntityFieldPath.Parse (item.Key)));

				using (DbTransaction transaction = this.infrastructure.BeginTransaction (DbTransactionMode.ReadOnly))
				{
					foreach (DataBrowserRow row in browser.QueryByExample (transaction, entity, query))
					{
						for (int i = 0; i < row.Keys.Count; i++)
						{
							DbKey key = row.Keys[i];
							Druid entityId = row.Query.EntityIds[i];
							this.dataContext.ResolveEntity (key, entityId);
						}
					}

					transaction.Commit ();
				}

				System.Diagnostics.Debug.WriteLine ("=============================================");
			}
		}


		private IEnumerable<AddressBook.Entities.LocalitéEntity> ReadNuPost(AddressBook.Entities.PaysEntity paysCh)
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

		public IEnumerable<Mai2008.Entities.ClientEntity> ReadPersonnes(List<AddressBook.Entities.LocalitéEntity> localités, List<AddressBook.Entities.TitrePersonneEntity> titres)
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


				Mai2008.Entities.ClientEntity personne = this.dataContext.CreateEmptyEntity<Mai2008.Entities.ClientEntity> ();

				personne.Titre = titres.Find (x => string.Compare (x.IntituléLong, values[1], true) == 0) ?? titres.Find (x => x.IntituléCourt == "M.");
				personne.Nom = values[2];
				personne.Prénom = values[3];
				personne.Rue = values[4];
				personne.Localité = localité;

				yield return personne;
			}
		}

		private class ResolverImplementation : IEntityResolver, System.IDisposable
		{
			public ResolverImplementation(CoreData data)
			{
				this.data = data;
			}

			#region IEntityResolver Members

			public IEnumerable<AbstractEntity> Resolve(Druid entityId, string criteria)
			{
				string[] keywords = criteria.Split (' ');

				foreach (var item in this.GetEntities (entityId))
				{
					string[] set = item.DumpFlatData (field => field.Source == FieldSource.Value).Split (' ', '\n', '\t');
					bool hit = false;

					foreach (string keyword in keywords)
					{
						hit = false;

						foreach (string word in set)
						{
							if (word.Contains (keyword))
							{
								hit = true;
								break;
							}
						}

						if (hit == false)
						{
							break;
						}
					}

					if (hit)
					{
						yield return item;
					}
				}
			}

			public IEnumerable<AbstractEntity> Resolve(AbstractEntity template)
			{
				Druid id = template.GetEntityStructuredTypeId ();

				if (id == Mai2008.Entities.ClientEntity.EntityStructuredTypeId)
				{
					Mai2008.Entities.ClientEntity example = AbstractEntity.Resolve<Mai2008.Entities.ClientEntity> (template);

					DataQuery query = new DataQuery ();
					
					query.Columns.Add (new DataQueryColumn (EntityFieldPath.CreateRelativePath ("[8V16]")));
					query.Columns.Add (new DataQueryColumn (EntityFieldPath.CreateRelativePath ("[8V17]")));

					IFieldPropertyStore propertyStore = template as IFieldPropertyStore;

					if (propertyStore != null)
					{
						propertyStore.SetValue ("[8V16]", StringType.DefaultSearchBehaviorProperty, StringSearchBehavior.MatchAnywhere);
						propertyStore.SetValue ("[8V17]", StringType.DefaultSearchBehaviorProperty, StringSearchBehavior.MatchStart);
					}

#if false
					using (DbTransaction transaction = this.data.infrastructure.BeginTransaction (DbTransactionMode.ReadOnly))
					{
						foreach (DataBrowserRow row in this.data.dataBrowser.QueryByExample (transaction, template, query))
						{
							System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
							
							foreach (object column in row.Values)
							{
								if (buffer.Length > 0)
								{
									buffer.Append (", ");
								}
								buffer.Append (column.ToString ());
							}

							System.Diagnostics.Debug.WriteLine (buffer.ToString ());
						}
						
						transaction.Commit ();
					}
#endif

					if (example == null)
					{
						yield break;
					}

					foreach (Mai2008.Entities.ClientEntity entity in this.GetEntities (id))
					{
						if (/*(ResolverImplementation.Match (example.Titre, entity.Titre)) &&*/
							(ResolverImplementation.Match (example.Nom, entity.Nom)) &&
							(ResolverImplementation.Match (example.Prénom, entity.Prénom)) &&
							(ResolverImplementation.Match (example.Rue, entity.Rue)) /*&&
							(ResolverImplementation.Match (example.Localité, entity.Localité))*/)
						{
							yield return entity;
						}
					}
				}
				else if (id == Mai2008.Entities.ArticleEntity.EntityStructuredTypeId)
				{
					Mai2008.Entities.ArticleEntity example = AbstractEntity.Resolve<Mai2008.Entities.ArticleEntity> (template);

					if (example == null)
					{
						yield break;
					}

					foreach (Mai2008.Entities.ArticleEntity entity in this.GetEntities (id))
					{
						if ((ResolverImplementation.Match (example.Désignation, entity.Désignation)) &&
							(ResolverImplementation.Match (example.Numéro, entity.Numéro)))
						{
							yield return entity;
						}
					}
				}
				else if (id == Mai2008.Entities.FactureEntity.EntityStructuredTypeId)
				{
					Mai2008.Entities.FactureEntity example = AbstractEntity.Resolve<Mai2008.Entities.FactureEntity> (template);

					if (example == null)
					{
						yield break;
					}

					foreach (Mai2008.Entities.FactureEntity entity in this.GetEntities (id))
					{
						if (ResolverImplementation.Match (example.Objet.ToString (), entity.Objet.ToString ()))
						{
							yield return entity;
						}
					}
				}
				else if (id == AddressBook.Entities.LocalitéEntity.EntityStructuredTypeId)
				{
					AddressBook.Entities.LocalitéEntity example = AbstractEntity.Resolve<AddressBook.Entities.LocalitéEntity> (template);

					if (example == null)
					{
						yield break;
					}

					foreach (AddressBook.Entities.LocalitéEntity entity in this.GetEntities (id))
					{
						if (ResolverImplementation.Match (example.Résumé, entity.SearchValue))
						{
							yield return entity;
						}
					}
				}
				else if (id == AddressBook.Entities.TitrePersonneEntity.EntityStructuredTypeId)
				{
					AddressBook.Entities.TitrePersonneEntity example = AbstractEntity.Resolve<AddressBook.Entities.TitrePersonneEntity> (template);

					if (example == null)
					{
						yield break;
					}

					foreach (AddressBook.Entities.TitrePersonneEntity entity in this.GetEntities (id))
					{
						if ((ResolverImplementation.Match (example.IntituléLong, entity.IntituléLong)) ||
							(ResolverImplementation.Match (example.IntituléLong, entity.IntituléCourt)))
						{
							yield return entity;
						}
					}
				}
			}

			#endregion

			#region IDisposable Members

			public void Dispose()
			{
			}

			#endregion

			private IEnumerable<AbstractEntity> GetEntities(Druid id)
			{
				DataContext context = this.data.DataContext;

				return context.GetManagedEntities (
					e => e.GetEntityStructuredTypeId () == id && context.IsPersistent (e));
			}

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

		private ResolverImplementation resolver;
	}
}
