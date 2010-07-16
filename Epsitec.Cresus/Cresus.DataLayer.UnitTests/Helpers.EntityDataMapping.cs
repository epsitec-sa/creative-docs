//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer.Context;

using NUnit.Framework;

using System.Collections.Generic;

namespace Epsitec.Cresus.DataLayer.Helpers
{
	[TestFixture]
	public class EntityDataMappingTest
	{
		[Test]
		public void CheckBasic()
		{
			EntityContext context = EntityContext.Current;
			AbstractEntity entity = context.CreateEmptyEntity (Druid.Parse ("[630Q]"));

			EntityDataMapping mapping = new EntityDataMapping (entity, entity.GetEntityStructuredTypeId (), context.GetRootEntityId (entity.GetEntityStructuredTypeId ()));

			Assert.AreEqual (entity, mapping.Entity);
			Assert.AreEqual (Druid.Parse ("[630Q]"), mapping.LeafEntityId);
			Assert.AreEqual (DbKey.Empty, mapping.RowKey);

			Assert.IsFalse (mapping.RowKey.IsDefinitive);
			mapping.RowKey = DbKey.Empty;
			Assert.IsFalse (mapping.RowKey.IsDefinitive);
			mapping.RowKey = new DbKey (DbKey.CreateTemporaryId ());
			Assert.IsFalse (mapping.RowKey.IsDefinitive);
			mapping.RowKey = new DbKey (new DbId (1000000000001L));
			Assert.IsTrue (mapping.RowKey.IsDefinitive);
		}

		[Test]
		[ExpectedException (typeof (System.InvalidOperationException))]
		public void CheckRowKeyEx1()
		{
			EntityContext context = EntityContext.Current;
			AbstractEntity entity = context.CreateEmptyEntity (Druid.Parse ("[630Q]"));

			EntityDataMapping mapping = new EntityDataMapping (entity, entity.GetEntityStructuredTypeId (), context.GetRootEntityId (entity.GetEntityStructuredTypeId ()));

			mapping.RowKey = new DbKey (new DbId (1000000000001L));
			mapping.RowKey = new DbKey (new DbId (1000000000001L));
		}

		[Test]
		[ExpectedException (typeof (System.InvalidOperationException))]
		public void CheckRowKeyEx2()
		{
			EntityContext context = EntityContext.Current;
			AbstractEntity entity = context.CreateEmptyEntity (Druid.Parse ("[630Q]"));

			EntityDataMapping mapping = new EntityDataMapping (entity, entity.GetEntityStructuredTypeId (), context.GetRootEntityId (entity.GetEntityStructuredTypeId ()));

			mapping.RowKey = new DbKey (new DbId (1000000000001L));
			mapping.RowKey = new DbKey (new DbId (1000000000002L));
		}

		[Test]
		[ExpectedException (typeof (System.InvalidOperationException))]
		public void CheckRowKeyEx3()
		{
			EntityContext context = EntityContext.Current;
			AbstractEntity entity = context.CreateEmptyEntity (Druid.Parse ("[630Q]"));

			EntityDataMapping mapping = new EntityDataMapping (entity, entity.GetEntityStructuredTypeId (), context.GetRootEntityId (entity.GetEntityStructuredTypeId ()));

			mapping.RowKey = new DbKey (new DbId (1000000000001L));
			mapping.RowKey = DbKey.Empty;
		}

		[Test]
		[ExpectedException (typeof (System.InvalidOperationException))]
		public void CheckRowKeyEx4()
		{
			EntityContext context = EntityContext.Current;
			AbstractEntity entity = context.CreateEmptyEntity (Druid.Parse ("[630Q]"));

			EntityDataMapping mapping = new EntityDataMapping (entity, entity.GetEntityStructuredTypeId (), context.GetRootEntityId (entity.GetEntityStructuredTypeId ()));

			mapping.RowKey = new DbKey (DbKey.CreateTemporaryId ());
			mapping.RowKey = new DbKey (DbKey.CreateTemporaryId ());
		}
	}
}
