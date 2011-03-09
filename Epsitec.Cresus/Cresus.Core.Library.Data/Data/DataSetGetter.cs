//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Data;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Data
{
	/// <summary>
	/// The <c>DataSetGetter</c> class resolves a data set name to the data
	///	set itself.
	/// </summary>
	public class DataSetGetter : CoreDataComponent
	{
		public DataSetGetter(CoreData data)
			: base (data)
		{
		}

		abstract class DynamicResolver
		{
			public abstract DataSetCollectionGetter Resolve(CoreData data);
		}
		sealed class DynamicResolver<T> : DynamicResolver
			where T : AbstractEntity, new ()
		{
			public override DataSetCollectionGetter Resolve(CoreData data)
			{
				return context => data.GetAllEntities<T> (dataContext: context);
			}
		}

		public DataSetCollectionGetter ResolveDataSet(string dataSetName)
		{
			var types = from type in Infrastructure.GetManagedEntityStructuredTypes ()
						where type.Flags.HasFlag (StructuredTypeFlags.StandaloneDisplay)
						select new
						{ 
							Name = type.Caption.Name.StripSuffix ("Entity"),
							Type = type
						};


			System.Diagnostics.Debug.WriteLine (string.Join ("\n", types.OrderBy (x => x.Name).Select (x => x.Name).ToArray ()));

			foreach (var type in types)
			{
				if ((type.Name == dataSetName) ||
					(StringPluralizer.GuessPluralForms (type.Name).Contains (dataSetName)))
				{
					//	Found the entity type...

					System.Type entityType = EntityClassFactory.FindEntityType (type.Type.CaptionId);
					
					var resolver = System.Activator.CreateInstance (typeof (DynamicResolver<>).MakeGenericType (entityType)) as DynamicResolver;

					return resolver.Resolve (this.Host);
				}
			}

			return null;


#if false
			switch (dataSetName)
			{
				case "Customers":
					return context => data.GetAllEntities<CustomerEntity> (dataContext: context);

				case "Relations":
					return context => data.GetAllEntities<RelationEntity> (dataContext: context);

				case "ArticleDefinitions":
					return context => data.GetAllEntities<ArticleDefinitionEntity> (dataContext: context);

				case "Documents":
					return context => data.GetAllEntities<DocumentMetadataEntity> (dataContext: context);
				
				case "InvoiceDocuments":
					return context => data.GetAllEntities<BusinessDocumentEntity> (dataContext: context);

				case "BusinessSettings":
					return context => data.GetAllEntities<BusinessSettingsEntity> (dataContext: context);

				case "Images":
					return context => data.GetAllEntities<ImageEntity> (dataContext: context);

				case "ImageBlobs":
					return context => data.GetAllEntities<ImageBlobEntity> (dataContext: context);

				case "PriceCalculators":
					return context => data.GetAllEntities<PriceCalculatorEntity> (dataContext: context);

				case "WorkflowDefinitions":
					return context => data.GetAllEntities<WorkflowDefinitionEntity> (dataContext: context);

				case "DocumentCategoryMapping":
					return context => data.GetAllEntities<DocumentCategoryMappingEntity> (dataContext: context);

				case "DocumentCategory":
					return context => data.GetAllEntities<DocumentCategoryEntity> (dataContext: context);

				case "DocumentOptions":
					return context => data.GetAllEntities<DocumentOptionsEntity> (dataContext: context);

				case "DocumentPrintingUnits":
					return context => data.GetAllEntities<DocumentPrintingUnitsEntity> (dataContext: context);
				default:
					return null;
			}
#endif
		}
		
		public Druid GetRootEntityId(string dataSetName)
		{
			var types = from type in Infrastructure.GetManagedEntityStructuredTypes ()
						where type.Flags.HasFlag (StructuredTypeFlags.StandaloneDisplay)
						select new
						{
							Name = type.Caption.Name.StripSuffix ("Entity"),
							Type = type
						};


			foreach (var type in types)
			{
				if ((type.Name == dataSetName) ||
					(StringPluralizer.GuessPluralForms (type.Name).Contains (dataSetName)))
				{
					//	Found the entity type...

					return type.Type.CaptionId;
				}
			}

			return Druid.Empty;

#if false
//			return EntityInfo<NaturalPersonEntity>.GetTypeId ();
			switch (dataSetName)
			{
				case "Customers":
					return EntityInfo<CustomerEntity>.GetTypeId ();
				
				case "Relations":
					return EntityInfo<RelationEntity>.GetTypeId ();

				case "ArticleDefinitions":
					return EntityInfo<ArticleDefinitionEntity>.GetTypeId ();

				case "Documents":
					return EntityInfo<DocumentMetadataEntity>.GetTypeId ();

				case "InvoiceDocuments":
					return EntityInfo<BusinessDocumentEntity>.GetTypeId ();

				case "BusinessSettings":
					return EntityInfo<BusinessSettingsEntity>.GetTypeId ();

				case "Images":
					return EntityInfo<ImageEntity>.GetTypeId ();

				case "ImageBlobs":
					return EntityInfo<ImageBlobEntity>.GetTypeId ();

				case "PriceCalculators":
					return EntityInfo<PriceCalculatorEntity>.GetTypeId ();

				case "WorkflowDefinitions":
					return EntityInfo<WorkflowDefinitionEntity>.GetTypeId ();

				case "DocumentCategoryMapping":
					return EntityInfo<DocumentCategoryMappingEntity>.GetTypeId ();

				case "DocumentCategory":
					return EntityInfo<DocumentCategoryEntity>.GetTypeId ();

				case "DocumentOptions":
					return EntityInfo<DocumentOptionsEntity>.GetTypeId ();

				case "DocumentPrintingUnits":
					return EntityInfo<DocumentPrintingUnitsEntity>.GetTypeId ();
				default:
					return Druid.Empty;
			}
#endif
		}
	}
}
