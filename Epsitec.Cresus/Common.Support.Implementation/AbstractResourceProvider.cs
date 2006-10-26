//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

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
		protected AbstractResourceProvider(ResourceManager manager)
		{
			this.manager = manager;
		}
		
		#region IResourceProvider Members
		public abstract string Prefix
		{
			get;
		}
		
		public abstract bool SelectModule(ref ResourceModuleInfo module);
		
		public virtual bool ValidateId(string id)
		{
			return (id != null) && (id.Length > 0) && (id.Length < 100) && (id[0] != '.');
		}
		
		public abstract bool Contains(string id);
		
		public abstract byte[] GetData(string id, Epsitec.Common.Support.ResourceLevel level, System.Globalization.CultureInfo culture);
		public abstract string[] GetIds(string name_filter, string type_filter, ResourceLevel level, System.Globalization.CultureInfo culture);
		public abstract ResourceModuleInfo[] GetModules();

		public abstract bool SetData(string id, Epsitec.Common.Support.ResourceLevel level, System.Globalization.CultureInfo culture, byte[] data, Epsitec.Common.Support.ResourceSetMode mode);
		public abstract bool Remove(string id, Epsitec.Common.Support.ResourceLevel level, System.Globalization.CultureInfo culture);
		#endregion

		protected virtual void SelectLocale(System.Globalization.CultureInfo culture)
		{
			this.culture = culture;

			this.default_suffix = this.manager.MapToSuffix (ResourceLevel.Default, culture);
			this.local_suffix   = this.manager.MapToSuffix (ResourceLevel.Localized, culture);
			this.custom_suffix  = this.manager.MapToSuffix (ResourceLevel.Customized, culture);
		}
		
		
		protected CultureInfo				culture;
		protected ResourceManager			manager;
		
		protected string					default_suffix;
		protected string					local_suffix;
		protected string					custom_suffix;
	}
}
