//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

namespace Epsitec.Common.Support.Implementation
{
	using System.Globalization;
	
	/// <summary>
	/// La classe AbstractProvider regroupe les méthodes communes aux divers
	/// providers de ressources et offre une implémentation par défaut de
	/// l'interface IResourceProvider.
	/// </summary>
	public abstract class AbstractResourceProvider : Epsitec.Common.Support.IResourceProvider
	{
		protected AbstractResourceProvider()
		{
		}
		
		#region IResourceProvider Members
		public abstract string Prefix		{ get; }
		
		public virtual void SelectLocale(System.Globalization.CultureInfo culture)
		{
			this.culture = culture;
			this.suffix  = "." + this.culture.TwoLetterISOLanguageName;
			this.custom  = "." + Resources.CustomisedSuffix;
		}
		
		public virtual bool ValidateId(string id)
		{
			return (id != null) && (id != "") && (id.Length < 100);
		}
		
		public abstract bool Contains(string id);
		public abstract byte[] GetData(string id, Epsitec.Common.Support.ResourceLevel level);
		public abstract void Create(string id, Epsitec.Common.Support.ResourceLevel level);
		public abstract void Update(string id, Epsitec.Common.Support.ResourceLevel level, byte[] data);
		public abstract void Remove(string id, Epsitec.Common.Support.ResourceLevel level);
		#endregion
		
		
		protected CultureInfo			culture;
		protected string				suffix;
		protected string				custom;
	}
}
