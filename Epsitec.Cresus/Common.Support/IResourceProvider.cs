//	Copyright � 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 29/10/2003

namespace Epsitec.Common.Support
{
	/// <summary>
	/// L'interface IResourceProvider offre les services de base permettant
	/// d'obtenir les donn�es � partir d'un identificateur de ressource.
	/// </summary>
	public interface IResourceProvider
	{
		string					Prefix			{ get; }
		
		//	Le gestionnaire de ressources utilise ces m�thodes pour configurer le
		//	fournisseur de ressources :
		
		void Setup(string application);
		void SelectLocale(System.Globalization.CultureInfo culture);
		
		//	M�thodes d'acc�s aux donn�es (en lecture) :
		
		bool ValidateId(string id);
		bool Contains(string id);
		
		byte[] GetData(string id, ResourceLevel level);
		
		string[] GetIds(string filter, ResourceLevel level);
		
		//	M�thodes d'acc�s en �criture/modification :
		
		void Create(string id, ResourceLevel level);
		void Update(string id, ResourceLevel level, byte[] data);
		void Remove(string id, ResourceLevel level);
	}
}
