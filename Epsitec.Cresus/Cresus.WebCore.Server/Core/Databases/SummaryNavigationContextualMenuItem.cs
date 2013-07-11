using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Metadata;

using Epsitec.Cresus.DataLayer.Context;

using Epsitec.Cresus.WebCore.Server.Core.IO;

using System.Collections.Generic;

using System.Linq.Expressions;


namespace Epsitec.Cresus.WebCore.Server.Core.Databases
{


	internal sealed class SummaryNavigationContextualMenuItem : AbstractContextualMenuItem
	{


		public SummaryNavigationContextualMenuItem(DataSetMenuItem menuItem)
			: base (menuItem.Title)
		{
			this.menuItem = menuItem;
		}


		public Druid DataSetId
		{
			get
			{
				return this.menuItem.DataSetId;
			}
		}


		public string ColumnName
		{
			get
			{
				return this.menuItem.Path.FieldPath;
			}
		}


		public LambdaExpression LambdaExpression
		{
			get
			{
				return this.menuItem.Expression;
			}
		}


		public override Dictionary<string, object> GetDataDictionary(Caches caches)
		{
			var data = base.GetDataDictionary (caches);

			data["databaseName"] = DataIO.DruidToString (this.DataSetId);
			data["columnName"] = this.GetId (caches);

			return data;
		}


		protected override string GetDataType()
		{
			return "summarynavigation";
		}


		public string GetId(Caches caches)
		{
			return caches.ColumnIdCache.GetId (this.ColumnName);
		}


		public object GetEntityId(DataContext dataContext, Caches caches, AbstractEntity entity)
		{
			var propertyAccessor = caches.PropertyAccessorCache.Get (this.LambdaExpression);
			var value = (AbstractEntity) propertyAccessor.GetValue (entity);

			return EntityIO.GetEntityId (dataContext, value);
		}


		private readonly DataSetMenuItem menuItem;


	}


}
