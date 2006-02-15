//	Copyright © 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// La classe UndefinedValue représente la valeur d'une propriété non
	/// initialisée. Voir aussi InvalidValue.
	/// </summary>
	public sealed class UndefinedValue
	{
		private UndefinedValue()
		{
		}
		
		
		public static bool IsValueUndefined(object value)
		{
			return (value == UndefinedValue.Instance);
		}
		
		
		public static readonly UndefinedValue	Instance = new UndefinedValue();
	}
}
