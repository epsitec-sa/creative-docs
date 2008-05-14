//  --------------------------------------------------------------------------- 
//  ATTENTION !
//  Ce fichier a été généré automatiquement. Ne pas l'éditer manuellement, car 
//  toute modification sera perdue. 
//  --------------------------------------------------------------------------- 

[assembly: global::Epsitec.Common.Support.EntityClass ("[9VA]", typeof (Epsitec.Cresus.Mai2008.Entities.FactureEntity))]
#region Epsitec.Cresus.Mai2008.Facture Entity
namespace Epsitec.Cresus.Mai2008.Entities
{
	///	<summary>
	///	The <c>Facture</c> entity.
	///	designer:cap/9VA
	///	</summary>
	public partial class FactureEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>Objet</c> field.
		///	designer:fld/9VA/9VA2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[9VA2]")]
		public global::Epsitec.Common.Types.FormattedText Objet
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.FormattedText> ("[9VA2]");
			}
			set
			{
				global::Epsitec.Common.Types.FormattedText oldValue = this.Objet;
				if (oldValue != value)
				{
					this.OnObjetChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.FormattedText> ("[9VA2]", oldValue, value);
					this.OnObjetChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>AdresseFacturation</c> field.
		///	designer:fld/9VA/9VA1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[9VA1]")]
		public global::Epsitec.Cresus.AddressBook.Entities.AdresseEntity AdresseFacturation
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.AddressBook.Entities.AdresseEntity> ("[9VA1]");
			}
			set
			{
				global::Epsitec.Cresus.AddressBook.Entities.AdresseEntity oldValue = this.AdresseFacturation;
				if (oldValue != value)
				{
					this.OnAdresseFacturationChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.AddressBook.Entities.AdresseEntity> ("[9VA1]", oldValue, value);
					this.OnAdresseFacturationChanged (oldValue, value);
				}
			}
		}
		
		partial void OnObjetChanging(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnObjetChanged(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnAdresseFacturationChanging(global::Epsitec.Cresus.AddressBook.Entities.AdresseEntity oldValue, global::Epsitec.Cresus.AddressBook.Entities.AdresseEntity newValue);
		partial void OnAdresseFacturationChanged(global::Epsitec.Cresus.AddressBook.Entities.AdresseEntity oldValue, global::Epsitec.Cresus.AddressBook.Entities.AdresseEntity newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Mai2008.Entities.FactureEntity.EntityStructuredTypeId;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1001, 10, 0);	// [9VA]
	}
}
#endregion

