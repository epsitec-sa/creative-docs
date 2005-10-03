//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// La classe UndefinedValue représente la valeur d'une propriété non initialisée.
	/// </summary>
	public sealed class UndefinedValue
	{
		private UndefinedValue()
		{
		}
		
		
		public static readonly UndefinedValue	Instance = new UndefinedValue();
	}
}
