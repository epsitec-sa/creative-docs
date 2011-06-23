//  --------------------------------------------------------------------------- 
//  ATTENTION !
//  Ce fichier a été généré automatiquement. Ne pas l'éditer manuellement, car 
//  toute modification sera perdue. 
//  --------------------------------------------------------------------------- 

[assembly: global::Epsitec.Common.Support.EntityClass ("[JVA]", typeof (Epsitec.Cresus.Core.Library.Entities.ProductFeatureEntity))]
#region Epsitec.Cresus.Core.Library.ProductFeature Entity
namespace Epsitec.Cresus.Core.Library.Entities
{
	///	<summary>
	///	The <c>ProductFeature</c> entity.
	///	designer:cap/JVA
	///	</summary>
	public partial class ProductFeatureEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Library.Entities.ProductFeatureEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Library.Entities.ProductFeatureEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1011, 10, 0);	// [JVA]
		public static readonly string EntityStructuredTypeKey = "[JVA]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<ProductFeatureEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Immutable)
			{
			}
		}
		#endregion
	}
}
#endregion

