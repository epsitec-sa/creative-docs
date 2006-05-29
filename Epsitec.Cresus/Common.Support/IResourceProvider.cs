//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Support
{
	/// <summary>
	/// L'interface IResourceProvider offre les services de base permettant
	/// d'obtenir les données à partir d'un identificateur de ressource.
	/// </summary>
	public interface IResourceProvider
	{
		string					Prefix			{ get; }
		
		void Setup(ResourceManager resource_manager);
		bool SetupApplication(string application);
		void SelectLocale(System.Globalization.CultureInfo culture);
		bool ValidateId(string id);
		bool Contains(string id);
		byte[] GetData(string id, ResourceLevel level, System.Globalization.CultureInfo culture);
		ResourceModuleInfo[] GetModules();
		string[] GetIds(string name_filter, string type_filter, ResourceLevel level, System.Globalization.CultureInfo culture);
		bool SetData(string id, ResourceLevel level, System.Globalization.CultureInfo culture, byte[] data, ResourceSetMode mode);
		bool Remove(string id, ResourceLevel level, System.Globalization.CultureInfo culture);
		//	Le gestionnaire de ressources utilise ces méthodes pour configurer le
		//	fournisseur de ressources :
		
		
		//	Méthodes d'accès aux données (en lecture) :
		
		
		
		
		//	Méthodes d'accès en écriture/modification :
		
	}
	
	/// <summary>
	/// Le mode d'écriture détermine comment sont gérés les cas où la ressource existe
	/// déjà (ou n'existe pas encore).
	/// </summary>
	public enum ResourceSetMode
	{
		None,
		CreateOnly,						//	crée si n'existe pas, erreur si existe
		UpdateOnly,						//	met à jour si existe, erreur si n'existe pas
		Write							//	crée ou met à jour la ressource
	}
}
