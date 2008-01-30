//  --------------------------------------------------------------------------- 
//  ATTENTION !
//  Ce fichier a été généré automatiquement. Ne pas l'éditer manuellement, car 
//  toute modification sera perdue. 
//  --------------------------------------------------------------------------- 

[assembly: global::Epsitec.Common.Support.EntityClass ("[8V1]", typeof (Epsitec.Cresus.AddressBook.Entities.AdresseEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[8V11]", typeof (Epsitec.Cresus.AddressBook.Entities.PersonAddressEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[8V12]", typeof (Epsitec.Cresus.AddressBook.Entities.EnterpriseAddressEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[8V18]", typeof (Epsitec.Cresus.AddressBook.Entities.LocalitéEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[8V1C]", typeof (Epsitec.Cresus.AddressBook.Entities.PaysEntity))]
#region Epsitec.Cresus.AddressBook.Adresse Entity
namespace Epsitec.Cresus.AddressBook.Entities
{
	///	<summary>
	///	The <c>Adresse</c> entity.
	///	designer:cap/8V1
	///	</summary>
	public partial class AdresseEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>Rue</c> field.
		///	designer:fld/8V1/8V13
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8V13]")]
		public string Rue
		{
			get
			{
				return this.GetField<string> ("[8V13]");
			}
			set
			{
				string oldValue = this.Rue;
				if (oldValue != value)
				{
					this.OnRueChanging (oldValue, value);
					this.SetField<string> ("[8V13]", oldValue, value);
					this.OnRueChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>CasePostale</c> field.
		///	designer:fld/8V1/8V14
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8V14]")]
		public string CasePostale
		{
			get
			{
				return this.GetField<string> ("[8V14]");
			}
			set
			{
				string oldValue = this.CasePostale;
				if (oldValue != value)
				{
					this.OnCasePostaleChanging (oldValue, value);
					this.SetField<string> ("[8V14]", oldValue, value);
					this.OnCasePostaleChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Localité</c> field.
		///	designer:fld/8V1/8V15
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8V15]")]
		public global::Epsitec.Cresus.AddressBook.Entities.LocalitéEntity Localité
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.AddressBook.Entities.LocalitéEntity> ("[8V15]");
			}
			set
			{
				global::Epsitec.Cresus.AddressBook.Entities.LocalitéEntity oldValue = this.Localité;
				if (oldValue != value)
				{
					this.OnLocalitéChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.AddressBook.Entities.LocalitéEntity> ("[8V15]", oldValue, value);
					this.OnLocalitéChanged (oldValue, value);
				}
			}
		}
		
		partial void OnRueChanging(string oldValue, string newValue);
		partial void OnRueChanged(string oldValue, string newValue);
		partial void OnCasePostaleChanging(string oldValue, string newValue);
		partial void OnCasePostaleChanged(string oldValue, string newValue);
		partial void OnLocalitéChanging(global::Epsitec.Cresus.AddressBook.Entities.LocalitéEntity oldValue, global::Epsitec.Cresus.AddressBook.Entities.LocalitéEntity newValue);
		partial void OnLocalitéChanged(global::Epsitec.Cresus.AddressBook.Entities.LocalitéEntity oldValue, global::Epsitec.Cresus.AddressBook.Entities.LocalitéEntity newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return new global::Epsitec.Common.Support.Druid (1000, 1, 0);	// [8V1]
		}
	}
}
#endregion

#region Epsitec.Cresus.AddressBook.PersonAddress Entity
namespace Epsitec.Cresus.AddressBook.Entities
{
	///	<summary>
	///	The <c>PersonAddress</c> entity.
	///	designer:cap/8V11
	///	</summary>
	public partial class PersonAddressEntity : global::Epsitec.Cresus.AddressBook.Entities.AdresseEntity
	{
		///	<summary>
		///	The <c>FirstName</c> field.
		///	designer:fld/8V11/8V16
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8V16]")]
		public string FirstName
		{
			get
			{
				return this.GetField<string> ("[8V16]");
			}
			set
			{
				string oldValue = this.FirstName;
				if (oldValue != value)
				{
					this.OnFirstNameChanging (oldValue, value);
					this.SetField<string> ("[8V16]", oldValue, value);
					this.OnFirstNameChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>LastName</c> field.
		///	designer:fld/8V11/8V17
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8V17]")]
		public string LastName
		{
			get
			{
				return this.GetField<string> ("[8V17]");
			}
			set
			{
				string oldValue = this.LastName;
				if (oldValue != value)
				{
					this.OnLastNameChanging (oldValue, value);
					this.SetField<string> ("[8V17]", oldValue, value);
					this.OnLastNameChanged (oldValue, value);
				}
			}
		}
		
		partial void OnFirstNameChanging(string oldValue, string newValue);
		partial void OnFirstNameChanged(string oldValue, string newValue);
		partial void OnLastNameChanging(string oldValue, string newValue);
		partial void OnLastNameChanged(string oldValue, string newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return new global::Epsitec.Common.Support.Druid (1000, 1, 1);	// [8V11]
		}
	}
}
#endregion

#region Epsitec.Cresus.AddressBook.EnterpriseAddress Entity
namespace Epsitec.Cresus.AddressBook.Entities
{
	///	<summary>
	///	The <c>EnterpriseAddress</c> entity.
	///	designer:cap/8V12
	///	</summary>
	public partial class EnterpriseAddressEntity : global::Epsitec.Cresus.AddressBook.Entities.AdresseEntity
	{
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return new global::Epsitec.Common.Support.Druid (1000, 1, 2);	// [8V12]
		}
	}
}
#endregion

#region Epsitec.Cresus.AddressBook.Localité Entity
namespace Epsitec.Cresus.AddressBook.Entities
{
	///	<summary>
	///	The <c>Localité</c> entity.
	///	designer:cap/8V18
	///	</summary>
	public partial class LocalitéEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Common.Dialogs.Entities.ISearchable
	{
		#region ISearchable Members
		///	<summary>
		///	The <c>SearchValue</c> field.
		///	designer:fld/8V18/6016
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[6016]")]
		public string SearchValue
		{
			get
			{
				return global::Epsitec.Common.Support.EntityEngine.AbstractEntity.GetCalculation<global::Epsitec.Cresus.AddressBook.Entities.LocalitéEntity, string> (this, "[6016]", global::Epsitec.Cresus.AddressBook.Entities.LocalitéEntity.FuncSearchValue);
			}
			set
			{
				global::Epsitec.Common.Support.EntityEngine.AbstractEntity.SetCalculation<global::Epsitec.Cresus.AddressBook.Entities.LocalitéEntity, string> (this, "[6016]", value);
			}
		}
		internal static readonly global::System.Func<global::Epsitec.Cresus.AddressBook.Entities.LocalitéEntity, string> FuncSearchValue = loc => loc.Résumé; // λ [8V18] [6016]
		internal static readonly global::System.Linq.Expressions.Expression<global::System.Func<global::Epsitec.Cresus.AddressBook.Entities.LocalitéEntity, string>> ExprSearchValue = loc => loc.Résumé; // λ [8V18] [6016]
		#endregion
		///	<summary>
		///	The <c>Numéro</c> field.
		///	designer:fld/8V18/8V19
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8V19]")]
		public string Numéro
		{
			get
			{
				return this.GetField<string> ("[8V19]");
			}
			set
			{
				string oldValue = this.Numéro;
				if (oldValue != value)
				{
					this.OnNuméroChanging (oldValue, value);
					this.SetField<string> ("[8V19]", oldValue, value);
					this.OnNuméroChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Nom</c> field.
		///	designer:fld/8V18/8V1A
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8V1A]")]
		public string Nom
		{
			get
			{
				return this.GetField<string> ("[8V1A]");
			}
			set
			{
				string oldValue = this.Nom;
				if (oldValue != value)
				{
					this.OnNomChanging (oldValue, value);
					this.SetField<string> ("[8V1A]", oldValue, value);
					this.OnNomChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Pays</c> field.
		///	designer:fld/8V18/8V1B
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8V1B]")]
		public global::Epsitec.Cresus.AddressBook.Entities.PaysEntity Pays
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.AddressBook.Entities.PaysEntity> ("[8V1B]");
			}
			set
			{
				global::Epsitec.Cresus.AddressBook.Entities.PaysEntity oldValue = this.Pays;
				if (oldValue != value)
				{
					this.OnPaysChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.AddressBook.Entities.PaysEntity> ("[8V1B]", oldValue, value);
					this.OnPaysChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Résumé</c> field.
		///	designer:fld/8V18/8V1F
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8V1F]")]
		public string Résumé
		{
			get
			{
				return global::Epsitec.Common.Support.EntityEngine.AbstractEntity.GetCalculation<global::Epsitec.Cresus.AddressBook.Entities.LocalitéEntity, string> (this, "[8V1F]", global::Epsitec.Cresus.AddressBook.Entities.LocalitéEntity.FuncRésumé);
			}
			set
			{
				global::Epsitec.Common.Support.EntityEngine.AbstractEntity.SetCalculation<global::Epsitec.Cresus.AddressBook.Entities.LocalitéEntity, string> (this, "[8V1F]", value);
			}
		}
		internal static readonly global::System.Func<global::Epsitec.Cresus.AddressBook.Entities.LocalitéEntity, string> FuncRésumé = loc => string.Concat (loc.Pays.Code, "-", loc.Numéro, " ", loc.Nom); // λ [8V18] [8V1F]
		internal static readonly global::System.Linq.Expressions.Expression<global::System.Func<global::Epsitec.Cresus.AddressBook.Entities.LocalitéEntity, string>> ExprRésumé = loc => string.Concat (loc.Pays.Code, "-", loc.Numéro, " ", loc.Nom); // λ [8V18] [8V1F]
		
		partial void OnNuméroChanging(string oldValue, string newValue);
		partial void OnNuméroChanged(string oldValue, string newValue);
		partial void OnNomChanging(string oldValue, string newValue);
		partial void OnNomChanged(string oldValue, string newValue);
		partial void OnPaysChanging(global::Epsitec.Cresus.AddressBook.Entities.PaysEntity oldValue, global::Epsitec.Cresus.AddressBook.Entities.PaysEntity newValue);
		partial void OnPaysChanged(global::Epsitec.Cresus.AddressBook.Entities.PaysEntity oldValue, global::Epsitec.Cresus.AddressBook.Entities.PaysEntity newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return new global::Epsitec.Common.Support.Druid (1000, 1, 8);	// [8V18]
		}
	}
}
#endregion

#region Epsitec.Cresus.AddressBook.Pays Entity
namespace Epsitec.Cresus.AddressBook.Entities
{
	///	<summary>
	///	The <c>Pays</c> entity.
	///	designer:cap/8V1C
	///	</summary>
	public partial class PaysEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>Code</c> field.
		///	designer:fld/8V1C/8V1D
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8V1D]")]
		public string Code
		{
			get
			{
				return this.GetField<string> ("[8V1D]");
			}
			set
			{
				string oldValue = this.Code;
				if (oldValue != value)
				{
					this.OnCodeChanging (oldValue, value);
					this.SetField<string> ("[8V1D]", oldValue, value);
					this.OnCodeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Nom</c> field.
		///	designer:fld/8V1C/8V1E
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8V1E]")]
		public string Nom
		{
			get
			{
				return this.GetField<string> ("[8V1E]");
			}
			set
			{
				string oldValue = this.Nom;
				if (oldValue != value)
				{
					this.OnNomChanging (oldValue, value);
					this.SetField<string> ("[8V1E]", oldValue, value);
					this.OnNomChanged (oldValue, value);
				}
			}
		}
		
		partial void OnCodeChanging(string oldValue, string newValue);
		partial void OnCodeChanged(string oldValue, string newValue);
		partial void OnNomChanging(string oldValue, string newValue);
		partial void OnNomChanged(string oldValue, string newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return new global::Epsitec.Common.Support.Druid (1000, 1, 12);	// [8V1C]
		}
	}
}
#endregion

