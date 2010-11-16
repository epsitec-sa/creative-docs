//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.DataAccessors
{
	public static class DataSetGetter
	{
		public static DataSetCollectionGetter ResolveDataSet(CoreData data, string dataSetName)
		{
			switch (dataSetName)
			{
				case "Customers":
					return context => data.GetAllEntities<RelationEntity> (dataContext: context);

				case "ArticleDefinitions":
					return context => data.GetAllEntities<ArticleDefinitionEntity> (dataContext: context);

				case "Documents":
				case "InvoiceDocuments":
					return context => data.GetAllEntities<BusinessDocumentEntity> (dataContext: context);

				case "BusinessSettings":
					return context => data.GetAllEntities<BusinessSettingsEntity> (dataContext: context);

				case "WorkflowDefinitions":
					return context => data.GetAllEntities<WorkflowDefinitionEntity> (dataContext: context);

				default:
					return null;
			}
		}
	}
}
