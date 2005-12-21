//	Copyright � 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// L'interface IDataItem donne acc�s � un �l�ment dans un IDataGraph en
	/// regroupant IDataFolder et IDataValue sous le m�me toit.
	/// </summary>
	public interface IDataItem : INameCaption, System.ICloneable
	{
		DataItemClasses		Classes	{ get; }
		//	INameCaption Members:
		//
		//	string	Name		{ get; }
		//	string	Caption		{ get; }
		//	string	Description	{ get; }
		//
		//	ICloneable Members:
		//
		//	object Clone();
		
	}
	
	[System.Flags]
	public enum DataItemClasses
	{
		None				= 0x00,
		Value				= 0x01,
		Folder				= 0x02,
	}
}

