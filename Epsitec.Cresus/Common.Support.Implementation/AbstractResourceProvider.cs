//	Copyright � 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Support.Implementation
{
	using System.Globalization;
	
	/// <summary>
	/// La classe AbstractProvider regroupe les m�thodes communes aux divers
	/// providers de ressources et offre une impl�mentation par d�faut de
	/// l'interface IResourceProvider.
	/// </summary>
	public abstract class AbstractResourceProvider : Epsitec.Common.Support.IResourceProvider
	{
		protected AbstractResourceProvider()
		{
		}
		
		#region IResourceProvider Members
		public abstract string Prefix		{ get; }
		
		public virtual void Setup(ResourceManager resource_manager)
		{
			this.manager = resource_manager;
		}

		public abstract bool SetupApplication(string application);
		public virtual void SelectLocale(System.Globalization.CultureInfo culture)
		{
			this.culture = culture;
			
			this.default_suffix = this.manager.MapToSuffix (ResourceLevel.Default, culture);
			this.local_suffix   = this.manager.MapToSuffix (ResourceLevel.Localized, culture);
			this.custom_suffix  = this.manager.MapToSuffix (ResourceLevel.Customized, culture);
			
			System.Diagnostics.Debug.Assert (culture.TwoLetterISOLanguageName == this.local_suffix);
		}
		
		
		public virtual bool ValidateId(string id)
		{
			return (id != null) && (id != "") && (id.Length < 100);
		}
		
		public abstract bool Contains(string id);
		
		public abstract byte[] GetData(string id, Epsitec.Common.Support.ResourceLevel level, System.Globalization.CultureInfo culture);
		public abstract string[] GetIds(string name_filter, string type_filter, ResourceLevel level, System.Globalization.CultureInfo culture);
		
		public abstract bool SetData(string id, Epsitec.Common.Support.ResourceLevel level, System.Globalization.CultureInfo culture, byte[] data, Epsitec.Common.Support.ResourceSetMode mode);
		public abstract bool Remove(string id, Epsitec.Common.Support.ResourceLevel level, System.Globalization.CultureInfo culture);
		#endregion
		
		
		protected CultureInfo				culture;
		protected ResourceManager			manager;
		
		protected string					default_suffix;
		protected string					local_suffix;
		protected string					custom_suffix;
	}
}
