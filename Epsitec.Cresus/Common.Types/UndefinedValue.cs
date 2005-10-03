//	Copyright � 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// La classe UndefinedValue repr�sente la valeur d'une propri�t� non initialis�e.
	/// </summary>
	public sealed class UndefinedValue
	{
		private UndefinedValue()
		{
		}
		
		
		public static readonly UndefinedValue	Instance = new UndefinedValue();
	}
}
