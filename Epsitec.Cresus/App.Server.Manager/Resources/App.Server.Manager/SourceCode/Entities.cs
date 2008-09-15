//  --------------------------------------------------------------------------- 
//  ATTENTION !
//  Ce fichier a été généré automatiquement. Ne pas l'éditer manuellement, car 
//  toute modification sera perdue. 
//  --------------------------------------------------------------------------- 

[assembly: global::Epsitec.Common.Support.EntityClass ("[AVA]", typeof (Epsitec.Cresus.ServerManager.Entities.ServiceSettingsEntity))]
#region Epsitec.Cresus.ServerManager.ServiceSettings Entity
namespace Epsitec.Cresus.ServerManager.Entities
{
	///	<summary>
	///	The <c>ServiceSettings</c> entity.
	///	designer:cap/AVA
	///	</summary>
	public partial class ServiceSettingsEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>IsServiceRunning</c> field.
		///	designer:fld/AVA/AVA1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[AVA1]")]
		public bool IsServiceRunning
		{
			get
			{
				return this.GetField<bool> ("[AVA1]");
			}
			set
			{
				bool oldValue = this.IsServiceRunning;
				if (oldValue != value)
				{
					this.OnIsServiceRunningChanging (oldValue, value);
					this.SetField<bool> ("[AVA1]", oldValue, value);
					this.OnIsServiceRunningChanged (oldValue, value);
				}
			}
		}
		
		partial void OnIsServiceRunningChanging(bool oldValue, bool newValue);
		partial void OnIsServiceRunningChanged(bool oldValue, bool newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.ServerManager.Entities.ServiceSettingsEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.ServerManager.Entities.ServiceSettingsEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1002, 10, 0);	// [AVA]
		public static readonly new string EntityStructuredTypeKey = "[AVA]";
	}
}
#endregion

