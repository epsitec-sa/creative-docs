//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// L'interface IDataItem donne accès à un élément dans un IDataGraph en
	/// regroupant IDataFolder et IDataValue sous le même toit.
	/// </summary>
	public interface IDataItem : ICaption, System.ICloneable
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

