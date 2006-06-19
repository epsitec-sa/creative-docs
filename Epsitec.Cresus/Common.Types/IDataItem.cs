//	Copyright � 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// L'interface IDataItem donne acc�s � un �l�ment dans un IDataGraph en
	/// regroupant IDataFolder et IDataValue sous le m�me toit.
	/// </summary>
	public interface IDataItem : ICaption, IName, System.ICloneable
	{
		DataItemClasses Classes
		{
			get;
		}
	}
	
	[System.Flags]
	public enum DataItemClasses
	{
		None				= 0x00,
		Value				= 0x01,
		Folder				= 0x02,
	}
}

