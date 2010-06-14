//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer;

using Demo.Demo5juin.Entities;

using NUnit.Framework;

using System.Collections.Generic;

namespace Epsitec.Cresus.DataLayer
{
	[TestFixture]
	public class DataContextTest
	{
		[Test]
		public void Check10CreateSchemas()
		{
			DataContext context = new DataContext (this.infrastructure);

			Assert.IsTrue (context.CreateSchema<ArticleEntity> ());
			Assert.IsTrue (context.CreateSchema<ArticleVisserieEntity> ());
			Assert.IsTrue (context.CreateSchema<PositionEntity> ());

			Assert.IsFalse (context.CreateSchema<PrixEntity> ());
			Assert.IsFalse (context.CreateSchema<RabaisSurArticleEntity> ());
		}


		[Test]
		public void Check11SaveEntity()
		{
			DataContext context = new DataContext (this.infrastructure);

			System.Diagnostics.Debug.WriteLine ("Check11SaveEntity");
			System.Diagnostics.Debug.WriteLine ("------------------------------------------------");
			
			ArticleEntity article = context.CreateEntity<ArticleEntity> ();

			Assert.AreEqual (9, context.CountManagedEntities ());
			Assert.AreEqual (this.prixEntityId, article.PrixVente.GetEntityStructuredTypeId ());

			article.Numéro = "VI-M3-10";
			article.Désignation = "Vis M3 10mm, inox";
			article.PrixVente.Ht = 0.05M;
			article.PrixAchat.Ht = 0.04M;

			System.Diagnostics.Debug.WriteLine ("Saving changes.");
			context.SaveChanges ();
			System.Diagnostics.Debug.WriteLine ("Done.");
			System.Diagnostics.Debug.WriteLine ("------------------------------------------------");
			

			int count = 0;

			System.Diagnostics.Debug.WriteLine ("Adding lots of articles");
			List<ArticleEntity> articles = new List<ArticleEntity> ();

			foreach (ArticleEntity item in this.GetArticles (context.EntityContext))
			{
				articles.Add (item);
				count++;
			}

			System.Diagnostics.Debug.WriteLine ("Serializing");
			context.SerializeChanges ();

			System.Diagnostics.Debug.WriteLine ("Saving");
			context.SaveChanges ();

			System.Diagnostics.Debug.WriteLine ("Saved " + count + " entities");
			System.Diagnostics.Debug.WriteLine ("------------------------------------------------");

			context.Dispose ();

			Assert.AreEqual (480, articles.Count);
			Assert.AreEqual (1000000000001L, context.GetEntityDataMapping (article).RowKey.Id.Value);
			Assert.AreEqual (1000000000002L, context.GetEntityDataMapping (articles[0]).RowKey.Id.Value);
		}

		[Test]
		public void Check12LoadEntity()
		{
			DataContext context = new DataContext (this.infrastructure);

			System.Diagnostics.Debug.WriteLine ("Check12LoadEntity");
			System.Diagnostics.Debug.WriteLine ("------------------------------------------------");
			
			DbTable table1 = context.SchemaEngine.FindTableDefinition (this.articleEntityId);
			DbTable table2 = context.SchemaEngine.FindTableDefinition (this.articleVisserieEntityId);
			DbSelectCondition condition = new DbSelectCondition (DbSelectRevision.LiveActive);

			System.Diagnostics.Debug.WriteLine ("Loading data from database");

			using (DbTransaction transaction = this.infrastructure.BeginTransaction (DbTransactionMode.ReadOnly))
			{
		//		context.RichCommand.ImportTable (transaction, table1, condition);
		//		context.RichCommand.ImportTable (transaction, table2, condition);
				transaction.Commit ();
			}

			System.Diagnostics.Debug.WriteLine ("Done.");

			DbKey key1 = new DbKey (new DbId (1000000000001L));
			DbKey key2 = new DbKey (new DbId (1000000000002L));

			AbstractEntity entity1 = context.DeserializeEntity (key1, this.articleEntityId);
			AbstractEntity entity2 = context.DeserializeEntity (key2, this.articleEntityId);

			System.Diagnostics.Debug.WriteLine ("------------------------------------------------");

			Assert.AreEqual (this.articleEntityId, entity1.GetEntityStructuredTypeId ());
			Assert.AreEqual ("VI-M3-10", entity1.GetField<string> ("[63091]"));

			Assert.AreEqual (this.articleVisserieEntityId, entity2.GetEntityStructuredTypeId ());
			Assert.AreEqual ("VI-M3-10", entity2.GetField<string> ("[63091]"));
			Assert.AreEqual ("M3", entity2.GetField<string> ("[6312]"));

			AbstractEntity prixVente = entity1.GetField<AbstractEntity> ("[630B1]");				//	Article.PrixVente
			AbstractEntity prixAchat = entity1.GetField<AbstractEntity> ("[6313]");					//	Article.PrixAchat

			Assert.AreEqual (0.05M, prixVente.GetField<decimal> ("[630H]"));						//	Prix.HT pour Article.PrixVente
			Assert.AreEqual (0.04M, prixAchat.GetField<decimal> ("[630H]"));						//	Prix.HT pour Article.PrixAchat
			
			context.Dispose ();
		}

		[Test]
		public void Check13LoadEntitySingle()
		{
			DataContext context = new DataContext (this.infrastructure);

			System.Diagnostics.Debug.WriteLine ("Check13LoadEntitySingle");
			System.Diagnostics.Debug.WriteLine ("------------------------------------------------");

			DbTable table1 = context.SchemaEngine.FindTableDefinition (this.articleEntityId);
			DbTable table2 = context.SchemaEngine.FindTableDefinition (this.articleVisserieEntityId);
			DbSelectCondition condition1 = new DbSelectCondition (DbSelectRevision.LiveActive);
			DbSelectCondition condition2 = new DbSelectCondition (DbSelectRevision.LiveActive);
			DbSelectCondition condition3 = new DbSelectCondition (DbSelectRevision.LiveActive);
			condition1.Condition = DbSimpleCondition.CreateCondition (new DbTableColumn (table1.Columns[Tags.ColumnId]), DbSimpleConditionOperator.Equal, 1000000000002L);
			condition2.Condition = DbSimpleCondition.CreateCondition (new DbTableColumn (table2.Columns[Tags.ColumnId]), DbSimpleConditionOperator.Equal, 1000000000002L);
			condition3.Condition = DbSimpleCondition.CreateCondition (new DbTableColumn (table1.Columns[Tags.ColumnId]), DbSimpleConditionOperator.Equal, 1000000000001L);

			System.Diagnostics.Debug.WriteLine ("Loading data from database");

			using (DbTransaction transaction = this.infrastructure.BeginTransaction (DbTransactionMode.ReadOnly))
			{
		//		context.RichCommand.ImportTable (transaction, table1, condition1);
		//		context.RichCommand.ImportTable (transaction, table2, condition2);
		//		context.RichCommand.ImportTable (transaction, table1, condition3);
				transaction.Commit ();
			}

			System.Diagnostics.Debug.WriteLine ("Done.");

			DbKey key1 = new DbKey (new DbId (1000000000001L));
			DbKey key2 = new DbKey (new DbId (1000000000002L));

			AbstractEntity entity1 = context.DeserializeEntity (key1, this.articleEntityId);
			AbstractEntity entity2 = context.DeserializeEntity (key2, this.articleEntityId);

			System.Diagnostics.Debug.WriteLine ("------------------------------------------------");

			Assert.AreEqual (this.articleEntityId, entity1.GetEntityStructuredTypeId ());
			Assert.AreEqual ("VI-M3-10", entity1.GetField<string> ("[63091]"));

			Assert.AreEqual (this.articleVisserieEntityId, entity2.GetEntityStructuredTypeId ());
			Assert.AreEqual ("VI-M3-10", entity2.GetField<string> ("[63091]"));
			Assert.AreEqual ("M3", entity2.GetField<string> ("[6312]"));

			Assert.AreEqual (entity1, context.ResolveEntity (key1, this.articleEntityId));
			Assert.AreEqual (entity2, context.ResolveEntity (key2, this.articleEntityId));
			Assert.AreEqual (entity2, context.ResolveEntity (key2, this.articleVisserieEntityId));

			AbstractEntity prixVente = entity1.GetField<AbstractEntity> ("[630B1]");				//	Article.PrixVente
			AbstractEntity prixAchat = entity1.GetField<AbstractEntity> ("[6313]");					//	Article.PrixAchat

			Assert.AreEqual (0.05M, prixVente.GetField<decimal> ("[630H]"));						//	Prix.HT pour Article.PrixVente
			Assert.AreEqual (0.04M, prixAchat.GetField<decimal> ("[630H]"));						//	Prix.HT pour Article.PrixAchat
			
			context.Dispose ();
		}

		[Test]
		public void Check14LoadAndUpdateEntitySingle()
		{
			DataContext context = new DataContext (this.infrastructure);

			System.Diagnostics.Debug.WriteLine ("Check14LoadAndUpdateEntitySingle");
			System.Diagnostics.Debug.WriteLine ("------------------------------------------------");

			System.Diagnostics.Debug.WriteLine ("Implicit loading of entities");

			DbKey key1 = new DbKey (new DbId (1000000000001L));
			DbKey key2 = new DbKey (new DbId (1000000000002L));

			AbstractEntity entity1 = context.DeserializeEntity (key1, this.articleEntityId);
			AbstractEntity entity2 = context.DeserializeEntity (key2, this.articleEntityId);

			AbstractEntity prixVente = entity1.GetField<AbstractEntity> ("[630B1]");				//	Article.PrixVente
			AbstractEntity prixAchat = entity1.GetField<AbstractEntity> ("[6313]");					//	Article.PrixAchat
			
			System.Diagnostics.Debug.WriteLine ("Done.");

			Assert.AreEqual (this.articleEntityId, entity1.GetEntityStructuredTypeId ());
			Assert.AreEqual ("VI-M3-10", entity1.GetField<string> ("[63091]"));
			Assert.AreEqual (0.05M, prixVente.GetField<decimal> ("[630H]"));						//	Prix.HT pour Article.PrixVente
			Assert.AreEqual (0.04M, prixAchat.GetField<decimal> ("[630H]"));						//	Prix.HT pour Article.PrixAchat

			Assert.AreEqual (this.articleVisserieEntityId, entity2.GetEntityStructuredTypeId ());
			Assert.AreEqual ("VI-M3-10", entity2.GetField<string> ("[63091]"));
			Assert.AreEqual ("M3", entity2.GetField<string> ("[6312]"));

			entity1.SetField<string> ("[630A1]", "Vis M3 10mm (acier inox)");
			entity2.SetField<string> ("[630A1]", "Vis M3 10mm (acier inox)");
			entity2.SetField<string> ("[6312]", "M3.0");

			prixVente.SetField<decimal> ("[630H]", 0.08M);
			prixAchat.SetField<decimal> ("[630H]", 0.12M);

			//	Swap both entities, in order to check the proper UPDATE of the database
			//	in case we change reference values :

			entity1.SetField<AbstractEntity> ("[630B1]", prixAchat);
			entity1.SetField<AbstractEntity> ("[6313]", prixVente);

			System.Diagnostics.Debug.WriteLine ("Serializing");
			Assert.IsTrue (context.SerializeChanges ());
			Assert.IsFalse (context.SerializeChanges ());

			System.Diagnostics.Debug.WriteLine ("Saving");
			context.SaveChanges ();

			System.Diagnostics.Debug.WriteLine ("Done.");
			System.Diagnostics.Debug.WriteLine ("------------------------------------------------");
			
			context.Dispose ();
		}

		[Test]
		public void Check15WriteCollections()
		{
			DataContext context = new DataContext (this.infrastructure);

			context.SchemaEngine.CreateTableDefinition (Druid.Parse ("[63001]"));
//			context.SchemaEngine.CreateTableDefinition (Druid.Parse ("[63011]"));

			AbstractEntity positionEntity = context.EntityContext.CreateEntity (Druid.Parse ("[63001]"));
			AbstractEntity rabais1Entity = context.EntityContext.CreateEntity (Druid.Parse ("[63011]"));
			AbstractEntity rabais2Entity = context.EntityContext.CreateEntity (Druid.Parse ("[63011]"));
			AbstractEntity rabais3Entity = context.EntityContext.CreateEntity (Druid.Parse ("[63011]"));

			IList<AbstractEntity> list = positionEntity.GetFieldCollection<AbstractEntity> ("[63072]");

			positionEntity.SetField<decimal> ("[63052]", 8.0M);
			list.Add (rabais1Entity);
			list.Add (rabais2Entity);

			rabais1Entity.SetField<decimal> ("[63082]", 2.5M);
			rabais1Entity.SetField<int> ("[63092]", 0);
			rabais2Entity.SetField<decimal> ("[63082]", 10.0M);
			rabais2Entity.SetField<int> ("[63092]", 1);
			rabais3Entity.SetField<decimal> ("[63082]", 5.0M);
			rabais3Entity.SetField<int> ("[63092]", 2);

			context.SaveChanges ();

			list.Add (rabais3Entity);

			context.SaveChanges ();

			list.RemoveAt (0);
			list.Add (rabais2Entity);

			context.SaveChanges ();

			this.keyCheck16 = context.GetEntityDataMapping (positionEntity).RowKey;

			context.Dispose ();
		}

		[Test]
		public void Check16ReadCollections()
		{
			DataContext context = new DataContext (this.infrastructure);

			AbstractEntity entity = context.DeserializeEntity (this.keyCheck16, Druid.Parse ("[63001]"));

			IList<AbstractEntity> list = entity.GetFieldCollection<AbstractEntity> ("[63072]");

			Assert.AreEqual (3, list.Count);
			Assert.AreEqual (list[0], list[2]);

			context.Dispose ();
		}

		[Test]
		public void Check17ReadCollections()
		{
			DataContext context = new DataContext (this.infrastructure);

			PositionEntity position = context.ResolveEntity<PositionEntity> (this.keyCheck16);

			IList<RabaisSurArticleEntity> list = position.Rabais;

			Assert.AreEqual (3, list.Count);
			Assert.AreEqual (list[0], list[2]);

			Assert.AreEqual ( 8.0M, position.Quantité);
			Assert.AreEqual (10.0M, list[0].Pourcent);
			Assert.AreEqual ( 5.0M, list[1].Pourcent);

			RabaisSurArticleEntity rabais = context.EntityContext.CreateEntity<RabaisSurArticleEntity> ();

			Assert.AreEqual (0, Collection.Count (context.GetModifiedEntities ()));

			rabais.CodeRaison = 76;
			rabais.Pourcent = 0.6M;

			Assert.AreEqual (1, Collection.Count (context.GetModifiedEntities ()));

			list.Insert (1, rabais);

			Assert.AreEqual (2, Collection.Count (context.GetModifiedEntities ()));

			context.SaveChanges ();

			Assert.AreEqual (0, Collection.Count (context.GetModifiedEntities ()));

			context.Dispose ();
		}


		private IEnumerable<ArticleEntity> GetArticles(EntityContext context)
		{
			string[] materials = new string[] { "Inox", "Cuivre", "Galvanisé" /*"Teflon", "POM", "Acier"*/ };
			string[] categories = new string[] { "Vis", "Ecrou", "Rondelle", "Boulon" };
			string[] sizes = new string[] { "M3", "M4", "M5", "M6", "M8" /* "M10", "M12", "M14", "M16", "M20" */ };
			string[] lengths = new string[] { "10mm", "12mm", "15mm", "20mm", "25mm", "30mm", "40mm", "50mm" };

			foreach (string mat in materials)
			{
				foreach (string cat in categories)
				{
					foreach (string size in sizes)
					{
						foreach (string len in lengths)
						{
							string itemKey = string.Concat (cat.Substring (0, 1), mat.Substring (0, 1), "-", size, "-", len.Substring (0, 2));
							string itemValue = string.Concat (cat, " ", size, " ", len, ", ", mat);

							ArticleVisserieEntity article = context.CreateEntity<ArticleVisserieEntity> ();

							article.Numéro = itemKey;
							article.Désignation = itemValue;
							article.Longueur = int.Parse (len.Substring (0, 2));
							article.Dimension = size;

							yield return article;
						}
					}
				}
			}
		}

		private readonly DbInfrastructure infrastructure = TestSupport.Database.NewInfrastructure ();
		private readonly Druid articleEntityId = Druid.Parse ("[630Q]");
		private readonly Druid articleVisserieEntityId = Druid.Parse ("[631]");
		private readonly Druid adresseEntityId = Druid.Parse ("[63081]");
		private readonly Druid prixEntityId = Druid.Parse ("[6308]");

		private DbKey keyCheck16;
	}
}
