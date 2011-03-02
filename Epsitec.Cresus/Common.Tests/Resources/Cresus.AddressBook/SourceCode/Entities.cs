//  --------------------------------------------------------------------------- 
//  ATTENTION !
//  Ce fichier a été généré automatiquement. Ne pas l'éditer manuellement, car 
//  toute modification sera perdue. 
//  --------------------------------------------------------------------------- 

[assembly: global::Epsitec.Common.Support.EntityClass ("[8V1]", typeof (Epsitec.Cresus.AddressBook.Entities.AdresseEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[8V11]", typeof (Epsitec.Cresus.AddressBook.Entities.AdressePersonneEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[8V12]", typeof (Epsitec.Cresus.AddressBook.Entities.AdresseEntrepriseEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[8V18]", typeof (Epsitec.Cresus.AddressBook.Entities.LocalitéEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[8V1C]", typeof (Epsitec.Cresus.AddressBook.Entities.PaysEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[8V1G]", typeof (Epsitec.Cresus.AddressBook.Entities.TitrePersonneEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[8VA]", typeof (Epsitec.Cresus.AddressBook.Entities.Localité1Entity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[8VA1]", typeof (Epsitec.Cresus.AddressBook.Entities.Localité2Entity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[8VA4]", typeof (Epsitec.Cresus.AddressBook.Entities.TestEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[8VA5]", typeof (Epsitec.Cresus.AddressBook.Entities.TexteTestEntity))]
#region Epsitec.Cresus.AddressBook.Adresse Entity
namespace Epsitec.Cresus.AddressBook.Entities
{
	///	<summary>
	///	The <c>Adresse</c> entity.
	///	designer:cap/8V1
	///	</summary>
	public partial class AdresseEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Common.Dialogs.Entities.ISearchable
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
		#region ISearchable Members
		///	<summary>
		///	The <c>SearchValue</c> field.
		///	designer:fld/8V1/6016
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[6016]")]
		public virtual string SearchValue
		{
			get
			{
				return global::Epsitec.Common.Support.EntityEngine.AbstractEntity.GetCalculation<global::Epsitec.Cresus.AddressBook.Entities.AdresseEntity, string> (this, "[6016]", global::Epsitec.Cresus.AddressBook.Entities.AdresseEntity.FuncSearchValue, global::Epsitec.Cresus.AddressBook.Entities.AdresseEntity.ExprSearchValue);
			}
			set
			{
				global::Epsitec.Common.Support.EntityEngine.AbstractEntity.SetCalculation<global::Epsitec.Cresus.AddressBook.Entities.AdresseEntity, string> (this, "[6016]", value);
			}
		}
		private static readonly global::System.Func<global::Epsitec.Cresus.AddressBook.Entities.AdresseEntity, string> FuncSearchValue = x => string.Concat (x.Rue, ", ", x.Localité.Résumé); // λ [8V1] [6016]
		private static readonly global::System.Linq.Expressions.Expression<global::System.Func<global::Epsitec.Cresus.AddressBook.Entities.AdresseEntity, string>> ExprSearchValue = x => string.Concat (x.Rue, ", ", x.Localité.Résumé); // λ [8V1] [6016]
		#endregion
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
			return global::Epsitec.Cresus.AddressBook.Entities.AdresseEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.AddressBook.Entities.AdresseEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1000, 1, 0);	// [8V1]
		public static readonly new string EntityStructuredTypeKey = "[8V1]";
	}
}
#endregion

#region Epsitec.Cresus.AddressBook.AdressePersonne Entity
namespace Epsitec.Cresus.AddressBook.Entities
{
	///	<summary>
	///	The <c>AdressePersonne</c> entity.
	///	designer:cap/8V11
	///	</summary>
	public partial class AdressePersonneEntity : global::Epsitec.Cresus.AddressBook.Entities.AdresseEntity
	{
		#region AdresseEntity Members
		///	<summary>
		///	The <c>SearchValue</c> field.
		///	designer:fld/8V11/6016
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[6016]")]
		public override string SearchValue
		{
			get
			{
				return global::Epsitec.Common.Support.EntityEngine.AbstractEntity.GetCalculation<global::Epsitec.Cresus.AddressBook.Entities.AdressePersonneEntity, string> (this, "[6016]", global::Epsitec.Cresus.AddressBook.Entities.AdressePersonneEntity.FuncSearchValue, global::Epsitec.Cresus.AddressBook.Entities.AdressePersonneEntity.ExprSearchValue);
			}
			set
			{
				global::Epsitec.Common.Support.EntityEngine.AbstractEntity.SetCalculation<global::Epsitec.Cresus.AddressBook.Entities.AdressePersonneEntity, string> (this, "[6016]", value);
			}
		}
		private static readonly global::System.Func<global::Epsitec.Cresus.AddressBook.Entities.AdressePersonneEntity, string> FuncSearchValue = x => string.Concat (x.Titre.IntituléCourt, " ", x.Prénom, " ", x.Nom, ", ", x.Rue, ", ", x.Localité.Résumé); // λ [8V11] [6016]
		private static readonly global::System.Linq.Expressions.Expression<global::System.Func<global::Epsitec.Cresus.AddressBook.Entities.AdressePersonneEntity, string>> ExprSearchValue = x => string.Concat (x.Titre.IntituléCourt, " ", x.Prénom, " ", x.Nom, ", ", x.Rue, ", ", x.Localité.Résumé); // λ [8V11] [6016]
		#endregion
		///	<summary>
		///	The <c>Titre</c> field.
		///	designer:fld/8V11/8V1J
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8V1J]")]
		public global::Epsitec.Cresus.AddressBook.Entities.TitrePersonneEntity Titre
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.AddressBook.Entities.TitrePersonneEntity> ("[8V1J]");
			}
			set
			{
				global::Epsitec.Cresus.AddressBook.Entities.TitrePersonneEntity oldValue = this.Titre;
				if (oldValue != value)
				{
					this.OnTitreChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.AddressBook.Entities.TitrePersonneEntity> ("[8V1J]", oldValue, value);
					this.OnTitreChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Prénom</c> field.
		///	designer:fld/8V11/8V16
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8V16]")]
		public string Prénom
		{
			get
			{
				return this.GetField<string> ("[8V16]");
			}
			set
			{
				string oldValue = this.Prénom;
				if (oldValue != value)
				{
					this.OnPrénomChanging (oldValue, value);
					this.SetField<string> ("[8V16]", oldValue, value);
					this.OnPrénomChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Nom</c> field.
		///	designer:fld/8V11/8V17
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8V17]")]
		public string Nom
		{
			get
			{
				return this.GetField<string> ("[8V17]");
			}
			set
			{
				string oldValue = this.Nom;
				if (oldValue != value)
				{
					this.OnNomChanging (oldValue, value);
					this.SetField<string> ("[8V17]", oldValue, value);
					this.OnNomChanged (oldValue, value);
				}
			}
		}
		
		partial void OnTitreChanging(global::Epsitec.Cresus.AddressBook.Entities.TitrePersonneEntity oldValue, global::Epsitec.Cresus.AddressBook.Entities.TitrePersonneEntity newValue);
		partial void OnTitreChanged(global::Epsitec.Cresus.AddressBook.Entities.TitrePersonneEntity oldValue, global::Epsitec.Cresus.AddressBook.Entities.TitrePersonneEntity newValue);
		partial void OnPrénomChanging(string oldValue, string newValue);
		partial void OnPrénomChanged(string oldValue, string newValue);
		partial void OnNomChanging(string oldValue, string newValue);
		partial void OnNomChanged(string oldValue, string newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.AddressBook.Entities.AdressePersonneEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.AddressBook.Entities.AdressePersonneEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1000, 1, 1);	// [8V11]
		public static readonly new string EntityStructuredTypeKey = "[8V11]";
	}
}
#endregion

#region Epsitec.Cresus.AddressBook.AdresseEntreprise Entity
namespace Epsitec.Cresus.AddressBook.Entities
{
	///	<summary>
	///	The <c>AdresseEntreprise</c> entity.
	///	designer:cap/8V12
	///	</summary>
	public partial class AdresseEntrepriseEntity : global::Epsitec.Cresus.AddressBook.Entities.AdresseEntity
	{
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.AddressBook.Entities.AdresseEntrepriseEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.AddressBook.Entities.AdresseEntrepriseEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1000, 1, 2);	// [8V12]
		public static readonly new string EntityStructuredTypeKey = "[8V12]";
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
		public virtual string SearchValue
		{
			get
			{
				return global::Epsitec.Common.Support.EntityEngine.AbstractEntity.GetCalculation<global::Epsitec.Cresus.AddressBook.Entities.LocalitéEntity, string> (this, "[6016]", global::Epsitec.Cresus.AddressBook.Entities.LocalitéEntity.FuncSearchValue, global::Epsitec.Cresus.AddressBook.Entities.LocalitéEntity.ExprSearchValue);
			}
			set
			{
				global::Epsitec.Common.Support.EntityEngine.AbstractEntity.SetCalculation<global::Epsitec.Cresus.AddressBook.Entities.LocalitéEntity, string> (this, "[6016]", value);
			}
		}
		private static readonly global::System.Func<global::Epsitec.Cresus.AddressBook.Entities.LocalitéEntity, string> FuncSearchValue = loc => loc.Résumé; // λ [8V18] [6016]
		private static readonly global::System.Linq.Expressions.Expression<global::System.Func<global::Epsitec.Cresus.AddressBook.Entities.LocalitéEntity, string>> ExprSearchValue = loc => loc.Résumé; // λ [8V18] [6016]
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
		public virtual string Résumé
		{
			get
			{
				return global::Epsitec.Common.Support.EntityEngine.AbstractEntity.GetCalculation<global::Epsitec.Cresus.AddressBook.Entities.LocalitéEntity, string> (this, "[8V1F]", global::Epsitec.Cresus.AddressBook.Entities.LocalitéEntity.FuncRésumé, global::Epsitec.Cresus.AddressBook.Entities.LocalitéEntity.ExprRésumé);
			}
			set
			{
				global::Epsitec.Common.Support.EntityEngine.AbstractEntity.SetCalculation<global::Epsitec.Cresus.AddressBook.Entities.LocalitéEntity, string> (this, "[8V1F]", value);
			}
		}
		private static readonly global::System.Func<global::Epsitec.Cresus.AddressBook.Entities.LocalitéEntity, string> FuncRésumé = loc => string.Concat (loc.Pays.Code, " ", loc.Numéro, " ", loc.Nom).Trim (); // λ [8V18] [8V1F]
		private static readonly global::System.Linq.Expressions.Expression<global::System.Func<global::Epsitec.Cresus.AddressBook.Entities.LocalitéEntity, string>> ExprRésumé = loc => string.Concat (loc.Pays.Code, " ", loc.Numéro, " ", loc.Nom).Trim (); // λ [8V18] [8V1F]
		
		partial void OnNuméroChanging(string oldValue, string newValue);
		partial void OnNuméroChanged(string oldValue, string newValue);
		partial void OnNomChanging(string oldValue, string newValue);
		partial void OnNomChanged(string oldValue, string newValue);
		partial void OnPaysChanging(global::Epsitec.Cresus.AddressBook.Entities.PaysEntity oldValue, global::Epsitec.Cresus.AddressBook.Entities.PaysEntity newValue);
		partial void OnPaysChanged(global::Epsitec.Cresus.AddressBook.Entities.PaysEntity oldValue, global::Epsitec.Cresus.AddressBook.Entities.PaysEntity newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.AddressBook.Entities.LocalitéEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.AddressBook.Entities.LocalitéEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1000, 1, 8);	// [8V18]
		public static readonly new string EntityStructuredTypeKey = "[8V18]";
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
	public partial class PaysEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Common.Dialogs.Entities.ISearchable
	{
		#region ISearchable Members
		///	<summary>
		///	The <c>SearchValue</c> field.
		///	designer:fld/8V1C/6016
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[6016]")]
		public string SearchValue
		{
			get
			{
				return global::Epsitec.Common.Dialogs.Entities.ISearchableInterfaceImplementation.GetSearchValue (this);
			}
			set
			{
				global::Epsitec.Common.Dialogs.Entities.ISearchableInterfaceImplementation.SetSearchValue (this, value);
			}
		}
		#endregion
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
			return global::Epsitec.Cresus.AddressBook.Entities.PaysEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.AddressBook.Entities.PaysEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1000, 1, 12);	// [8V1C]
		public static readonly new string EntityStructuredTypeKey = "[8V1C]";
	}
}
#endregion

#region Epsitec.Cresus.AddressBook.TitrePersonne Entity
namespace Epsitec.Cresus.AddressBook.Entities
{
	///	<summary>
	///	The <c>TitrePersonne</c> entity.
	///	designer:cap/8V1G
	///	</summary>
	public partial class TitrePersonneEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Common.Dialogs.Entities.ISearchable
	{
		#region ISearchable Members
		///	<summary>
		///	The <c>SearchValue</c> field.
		///	designer:fld/8V1G/6016
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[6016]")]
		public virtual string SearchValue
		{
			get
			{
				return global::Epsitec.Common.Support.EntityEngine.AbstractEntity.GetCalculation<global::Epsitec.Cresus.AddressBook.Entities.TitrePersonneEntity, string> (this, "[6016]", global::Epsitec.Cresus.AddressBook.Entities.TitrePersonneEntity.FuncSearchValue, global::Epsitec.Cresus.AddressBook.Entities.TitrePersonneEntity.ExprSearchValue);
			}
			set
			{
				global::Epsitec.Common.Support.EntityEngine.AbstractEntity.SetCalculation<global::Epsitec.Cresus.AddressBook.Entities.TitrePersonneEntity, string> (this, "[6016]", value);
			}
		}
		private static readonly global::System.Func<global::Epsitec.Cresus.AddressBook.Entities.TitrePersonneEntity, string> FuncSearchValue = titre => string.IsNullOrEmpty (titre.IntituléLong) ? titre.IntituléCourt : titre.IntituléLong; // λ [8V1G] [6016]
		private static readonly global::System.Linq.Expressions.Expression<global::System.Func<global::Epsitec.Cresus.AddressBook.Entities.TitrePersonneEntity, string>> ExprSearchValue = titre => string.IsNullOrEmpty (titre.IntituléLong) ? titre.IntituléCourt : titre.IntituléLong; // λ [8V1G] [6016]
		#endregion
		///	<summary>
		///	The <c>IntituléCourt</c> field.
		///	designer:fld/8V1G/8V1H
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8V1H]")]
		public string IntituléCourt
		{
			get
			{
				return this.GetField<string> ("[8V1H]");
			}
			set
			{
				string oldValue = this.IntituléCourt;
				if (oldValue != value)
				{
					this.OnIntituléCourtChanging (oldValue, value);
					this.SetField<string> ("[8V1H]", oldValue, value);
					this.OnIntituléCourtChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>IntituléLong</c> field.
		///	designer:fld/8V1G/8V1I
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8V1I]")]
		public string IntituléLong
		{
			get
			{
				return this.GetField<string> ("[8V1I]");
			}
			set
			{
				string oldValue = this.IntituléLong;
				if (oldValue != value)
				{
					this.OnIntituléLongChanging (oldValue, value);
					this.SetField<string> ("[8V1I]", oldValue, value);
					this.OnIntituléLongChanged (oldValue, value);
				}
			}
		}
		
		partial void OnIntituléCourtChanging(string oldValue, string newValue);
		partial void OnIntituléCourtChanged(string oldValue, string newValue);
		partial void OnIntituléLongChanging(string oldValue, string newValue);
		partial void OnIntituléLongChanged(string oldValue, string newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.AddressBook.Entities.TitrePersonneEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.AddressBook.Entities.TitrePersonneEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1000, 1, 16);	// [8V1G]
		public static readonly new string EntityStructuredTypeKey = "[8V1G]";
	}
}
#endregion

#region Epsitec.Cresus.AddressBook.Localité1 Entity
namespace Epsitec.Cresus.AddressBook.Entities
{
	///	<summary>
	///	The <c>Localité1</c> entity.
	///	designer:cap/8VA
	///	</summary>
	public partial class Localité1Entity : global::Epsitec.Cresus.AddressBook.Entities.LocalitéEntity
	{
		#region LocalitéEntity Members
		///	<summary>
		///	The <c>SearchValue</c> field.
		///	designer:fld/8VA/6016
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[6016]")]
		public override string SearchValue
		{
			get
			{
				return global::Epsitec.Common.Support.EntityEngine.AbstractEntity.GetCalculation<global::Epsitec.Cresus.AddressBook.Entities.Localité1Entity, string> (this, "[6016]", global::Epsitec.Cresus.AddressBook.Entities.Localité1Entity.FuncSearchValue, global::Epsitec.Cresus.AddressBook.Entities.Localité1Entity.ExprSearchValue);
			}
			set
			{
				global::Epsitec.Common.Support.EntityEngine.AbstractEntity.SetCalculation<global::Epsitec.Cresus.AddressBook.Entities.Localité1Entity, string> (this, "[6016]", value);
			}
		}
		private static readonly global::System.Func<global::Epsitec.Cresus.AddressBook.Entities.Localité1Entity, string> FuncSearchValue = loc => loc.Résumé.ToUpper (); // λ [8VA] [6016]
		private static readonly global::System.Linq.Expressions.Expression<global::System.Func<global::Epsitec.Cresus.AddressBook.Entities.Localité1Entity, string>> ExprSearchValue = loc => loc.Résumé.ToUpper (); // λ [8VA] [6016]
		#endregion
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.AddressBook.Entities.Localité1Entity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.AddressBook.Entities.Localité1Entity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1000, 10, 0);	// [8VA]
		public static readonly new string EntityStructuredTypeKey = "[8VA]";
	}
}
#endregion

#region Epsitec.Cresus.AddressBook.Localité2 Entity
namespace Epsitec.Cresus.AddressBook.Entities
{
	///	<summary>
	///	The <c>Localité2</c> entity.
	///	designer:cap/8VA1
	///	</summary>
	public partial class Localité2Entity : global::Epsitec.Cresus.AddressBook.Entities.Localité1Entity
	{
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.AddressBook.Entities.Localité2Entity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.AddressBook.Entities.Localité2Entity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1000, 10, 1);	// [8VA1]
		public static readonly new string EntityStructuredTypeKey = "[8VA1]";
	}
}
#endregion

#region Epsitec.Cresus.AddressBook.ITest Interface
namespace Epsitec.Cresus.AddressBook.Entities
{
	///	<summary>
	///	The <c>ITest</c> entity.
	///	designer:cap/8VA2
	///	</summary>
	public interface ITest
	{
		///	<summary>
		///	The <c>Champ</c> field.
		///	designer:fld/8VA2/8VA3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VA3]")]
		global::System.DateTime Champ
		{
			get;
			set;
		}
	}
	public static partial class ITestInterfaceImplementation
	{
		public static global::System.DateTime GetChamp(global::Epsitec.Cresus.AddressBook.Entities.ITest obj)
		{
			return global::Epsitec.Common.Support.EntityEngine.AbstractEntity.GetCalculation<global::Epsitec.Cresus.AddressBook.Entities.ITest, global::System.DateTime> (obj, "[8VA3]", ITestInterfaceImplementation.FuncChamp, ITestInterfaceImplementation.ExprChamp);
		}
		public static void SetChamp(global::Epsitec.Cresus.AddressBook.Entities.ITest obj, global::System.DateTime value)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity.SetCalculation<global::Epsitec.Cresus.AddressBook.Entities.ITest, global::System.DateTime> (obj, "[8VA3]", value);
		}
		internal static readonly global::System.Func<global::Epsitec.Cresus.AddressBook.Entities.ITest, global::System.DateTime> FuncChamp = x => System.DateTime.Now; // λ [8VA2] [8VA3]
		internal static readonly global::System.Linq.Expressions.Expression<global::System.Func<global::Epsitec.Cresus.AddressBook.Entities.ITest, global::System.DateTime>> ExprChamp = x => System.DateTime.Now; // λ [8VA2] [8VA3]
	}
}
#endregion

#region Epsitec.Cresus.AddressBook.Test Entity
namespace Epsitec.Cresus.AddressBook.Entities
{
	///	<summary>
	///	The <c>Test</c> entity.
	///	designer:cap/8VA4
	///	</summary>
	public partial class TestEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.AddressBook.Entities.ITest
	{
		#region ITest Members
		///	<summary>
		///	The <c>Champ</c> field.
		///	designer:fld/8VA4/8VA3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VA3]")]
		public virtual global::System.DateTime Champ
		{
			get
			{
				return global::Epsitec.Cresus.AddressBook.Entities.ITestInterfaceImplementation.GetChamp (this);
			}
			set
			{
				global::Epsitec.Cresus.AddressBook.Entities.ITestInterfaceImplementation.SetChamp (this, value);
			}
		}
		#endregion
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.AddressBook.Entities.TestEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.AddressBook.Entities.TestEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1000, 10, 4);	// [8VA4]
		public static readonly new string EntityStructuredTypeKey = "[8VA4]";
	}
}
#endregion

#region Epsitec.Cresus.AddressBook.TexteTest Entity
namespace Epsitec.Cresus.AddressBook.Entities
{
	///	<summary>
	///	The <c>TexteTest</c> entity.
	///	designer:cap/8VA5
	///	</summary>
	public partial class TexteTestEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>TexteSimple</c> field.
		///	designer:fld/8VA5/8VA6
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VA6]")]
		public string TexteSimple
		{
			get
			{
				return this.GetField<string> ("[8VA6]");
			}
			set
			{
				string oldValue = this.TexteSimple;
				if (oldValue != value)
				{
					this.OnTexteSimpleChanging (oldValue, value);
					this.SetField<string> ("[8VA6]", oldValue, value);
					this.OnTexteSimpleChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>TexteFormatté</c> field.
		///	designer:fld/8VA5/8VA7
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VA7]")]
		public global::Epsitec.Common.Types.FormattedText TexteFormatté
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.FormattedText> ("[8VA7]");
			}
			set
			{
				global::Epsitec.Common.Types.FormattedText oldValue = this.TexteFormatté;
				if (oldValue != value)
				{
					this.OnTexteFormattéChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.FormattedText> ("[8VA7]", oldValue, value);
					this.OnTexteFormattéChanged (oldValue, value);
				}
			}
		}
		
		partial void OnTexteSimpleChanging(string oldValue, string newValue);
		partial void OnTexteSimpleChanged(string oldValue, string newValue);
		partial void OnTexteFormattéChanging(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnTexteFormattéChanged(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.AddressBook.Entities.TexteTestEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.AddressBook.Entities.TexteTestEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1000, 10, 5);	// [8VA5]
		public static readonly new string EntityStructuredTypeKey = "[8VA5]";
	}
}
#endregion

