//  --------------------------------------------------------------------------- 
//  ATTENTION !
//  Ce fichier a été généré automatiquement. Ne pas l'éditer manuellement, car 
//  toute modification sera perdue. 
//  --------------------------------------------------------------------------- 

[assembly: global::Epsitec.Common.Support.EntityClass ("[CVA]", typeof (Epsitec.Cresus.Assets.Entities.AssetObjectEntity))]
#region Epsitec.Cresus.Assets.AssetObject Entity
namespace Epsitec.Cresus.Assets.Entities
{
	///	<summary>
	///	The <c>AssetObject</c> entity.
	///	designer:cap/CVA
	///	</summary>
	public partial class AssetObjectEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.ILifetime, global::Epsitec.Cresus.Core.Entities.INameDescription
	{
		#region ILifetime Members
		///	<summary>
		///	The <c>IsArchive</c> field.
		///	designer:fld/CVA/L0AB5
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0AB5]")]
		public bool IsArchive
		{
			get
			{
				return global::Epsitec.Cresus.Core.Entities.ILifetimeInterfaceImplementation.GetIsArchive (this);
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.ILifetimeInterfaceImplementation.SetIsArchive (this, value);
			}
		}
		#endregion
		#region INameDescription Members
		///	<summary>
		///	The <c>Name</c> field.
		///	designer:fld/CVA/L0AUN
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0AUN]")]
		public global::Epsitec.Common.Types.FormattedText Name
		{
			get
			{
				return global::Epsitec.Cresus.Core.Entities.INameDescriptionInterfaceImplementation.GetName (this);
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.INameDescriptionInterfaceImplementation.SetName (this, value);
			}
		}
		///	<summary>
		///	The <c>Description</c> field.
		///	designer:fld/CVA/L0AVN
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0AVN]")]
		public global::Epsitec.Common.Types.FormattedText Description
		{
			get
			{
				return global::Epsitec.Cresus.Core.Entities.INameDescriptionInterfaceImplementation.GetDescription (this);
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.INameDescriptionInterfaceImplementation.SetDescription (this, value);
			}
		}
		#endregion
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Assets.Entities.AssetObjectEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Assets.Entities.AssetObjectEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1004, 10, 0);	// [CVA]
		public static readonly new string EntityStructuredTypeKey = "[CVA]";
	}
}
#endregion

