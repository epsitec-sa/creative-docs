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
		
		byte[] GetData(string id, ResourceLevel level, System.Globalization.CultureInfo culture);
		
		string[] GetIds(string filter, ResourceLevel level, System.Globalization.CultureInfo culture);
		
		//	M�thodes d'acc�s en �criture/modification :
		
		bool SetData(string id, ResourceLevel level, System.Globalization.CultureInfo culture, byte[] data, ResourceSetMode mode);
		bool Remove(string id, ResourceLevel level, System.Globalization.CultureInfo culture);
	}
	
	/// <summary>
	/// Le mode d'�criture d�termine comment sont g�r�s les cas o� la ressource existe
	/// d�j� (ou n'existe pas encore).
	/// </summary>
	public enum ResourceSetMode
	{
		None,
		CreateOnly,						//	cr�e si n'existe pas, erreur si existe
		UpdateOnly,						//	met � jour si existe, erreur si n'existe pas
		Write							//	cr�e ou met � jour la ressource
	}
}
