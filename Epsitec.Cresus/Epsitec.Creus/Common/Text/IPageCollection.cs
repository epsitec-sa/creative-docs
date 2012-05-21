//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// L'interface IPageCollection permet de d'obtenir des informations sur
	/// des pages d'un document.
	/// </summary>
	public interface IPageCollection
	{
		int GetPageCount();
		
		string GetPageLabel(int page);
		PageFlags GetPageFlags(int page);
		
		void SetPageProperty(int page, string key, string value);
		string GetPageProperty(int page, string key);
		void ClearPageProperties(int page);
	}
	
	/// <summary>
	/// Le bitset PageFlags permet d'indiquer si une page est paire/impaire et
	/// s'il s'agit de la première page d'un document/d'une section.
	/// </summary>
	
	[System.Flags]
	public enum PageFlags
	{
		None			= 0,
		
		Even			= 1,
		Odd				= 2,
		
		First			= 0x10,
	}
}
