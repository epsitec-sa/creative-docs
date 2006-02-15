//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// La classe InvalidValue représente une valeur invalide, à ne pas
	/// confondre avec UndefinedValue.
	/// </summary>
	public sealed class InvalidValue
	{
		private InvalidValue()
		{
		}
		
		
		public static bool IsValueInvalid(object value)
		{
			return (value == InvalidValue.Instance);
		}
		
		
		public static readonly InvalidValue		Instance = new InvalidValue();
	}
}
