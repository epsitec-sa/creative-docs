//	Copyright � 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 26/04/2004

namespace Epsitec.Common.Types
{
	/// <summary>
	/// L'interface IDataItem donne acc�s � un �l�ment dans un IDataGraph en
	/// regroupant IDataFolder et IDataValue sous le m�me toit.
	/// </summary>
	public interface IDataItem : INameCaption
	{
		//	INameCaption Members:
		//
		//	string	Name		{ get; }
		//	string	Caption		{ get; }
		//	string	Description	{ get; }
		
		DataItemClasses		Classes	{ get; }
	}
	
	[System.Flags]
	public enum DataItemClasses
	{
		None				= 0x00,
		Value				= 0x01,
		Folder				= 0x02,
	}
}

