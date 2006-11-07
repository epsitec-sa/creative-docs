//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// L'interface IStringDict donne accès à un dictionnaire de clefs/valeurs
	/// de type string.
	/// </summary>
	public interface IStringDict
	{
		string[]	Keys				{ get; }
		string		this[string key]	{ get; set; }
		int			Count				{ get; }
		
		void Add(string key, string value);
		bool Remove(string key);
		void Clear();
		bool ContainsKey(string key);
	}
}
