//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
		
		void Add(string key, string value);
		void Remove(string key);
		void Clear();
		bool Contains(string key);
	}
}
