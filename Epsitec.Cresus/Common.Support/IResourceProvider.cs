//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 29/10/2003

namespace Epsitec.Common.Support
{
	/// <summary>
	/// L'interface IResourceProvider offre les services de base permettant
	/// d'obtenir les données à partir d'un identificateur de ressource.
	/// </summary>
	public interface IResourceProvider
	{
		string					Prefix			{ get; }
		
		//	Le gestionnaire de ressources utilise ces méthodes pour configurer le
		//	fournisseur de ressources :
		
		void Setup(string application);
		void SelectLocale(System.Globalization.CultureInfo culture);
		
		//	Méthodes d'accès aux données (en lecture) :
		
		bool ValidateId(string id);
		bool Contains(string id);
		
		byte[] GetData(string id, ResourceLevel level, System.Globalization.CultureInfo culture);
		
		string[] GetIds(string filter, ResourceLevel level, System.Globalization.CultureInfo culture);
		
		//	Méthodes d'accès en écriture/modification :
		
		bool Create(string id, ResourceLevel level, System.Globalization.CultureInfo culture);
		bool Update(string id, ResourceLevel level, System.Globalization.CultureInfo culture, byte[] data);
		bool Remove(string id, ResourceLevel level, System.Globalization.CultureInfo culture);
	}
}
