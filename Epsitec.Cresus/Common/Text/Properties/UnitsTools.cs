//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Properties
{
	/// <summary>
	/// La classe UnitsTools offre quelques méthodes de conversion pour des
	/// valeurs numériques associées à des unités de mesure.
	/// </summary>
	public sealed class UnitsTools
	{
		private UnitsTools()
		{
		}
		
		
		public static void Combine(double aValue, SizeUnits aUnits, double bValue, SizeUnits bUnits, out double cValue, out SizeUnits cUnits)
		{
			switch (bUnits)
			{
				case SizeUnits.Percent:				//	xxx * Percent --> xxx
					cUnits = aUnits;
					cValue = aValue * bValue;
					break;
				
				case SizeUnits.PercentNotCombining:	//	[%] --> [%] (écrase)
					cUnits = bUnits;
					cValue = bValue;
					break;
				
				case SizeUnits.Points:				//	[pt] --> [pt] (écrase)
				case SizeUnits.Millimeters:			//	[mm] --> [mm] (écrase)
				case SizeUnits.Inches:				//	[in] --> [in] (écrase)
					cUnits = bUnits;
					cValue = bValue;
					break;
				
				case SizeUnits.None:				//	xxx * [rien] --> xxx source
					cUnits = aUnits;
					cValue = aValue;
					break;
				
				case SizeUnits.DeltaPoints:			//	ajoute des deltas, conserve l'unité de départ
				case SizeUnits.DeltaMillimeters:
				case SizeUnits.DeltaInches:
					if (aUnits == SizeUnits.None)
					{
						cUnits = bUnits;
						cValue = bValue;
					}
					else
					{
						cUnits = aUnits;
						cValue = aValue + UnitsTools.ConvertToSizeUnits (bValue, bUnits, aUnits);
					}
					break;
				
				default:
					throw new System.NotSupportedException (string.Format ("Unsupported units: {0}.", bUnits));
			}
		}
		
		
		public static string SerializeSizeUnits(SizeUnits units)
		{
			switch (units)
			{
				case SizeUnits.None:				return "";
				case SizeUnits.Points:				return "pt";
				case SizeUnits.Millimeters:			return "mm";
				case SizeUnits.Inches:				return "in";
				case SizeUnits.DeltaPoints:			return "+pt";
				case SizeUnits.DeltaMillimeters:	return "+mm";
				case SizeUnits.DeltaInches:			return "+in";
				case SizeUnits.Percent:				return "%";
				case SizeUnits.PercentNotCombining:	return "!%";
			}
			
			throw new System.NotSupportedException (string.Format ("Unsupported units: {0}.", units));
		}
		
		public static string SerializeSizeUnits(double value, SizeUnits units)
		{
			if ((units == SizeUnits.Percent) ||
				(units == SizeUnits.PercentNotCombining))
			{
				value *= 100;
			}
			
			return string.Format (System.Globalization.CultureInfo.InvariantCulture, "{0} {1}", value, UnitsTools.SerializeSizeUnits (units));
		}
		
		
		public static SizeUnits DeserializeSizeUnits(string text)
		{
			switch (text)
			{
				case "":	return SizeUnits.None;
				case "pt":	return SizeUnits.Points;
				case "mm":	return SizeUnits.Millimeters;
				case "in":	return SizeUnits.Inches;
				case "+pt":	return SizeUnits.DeltaPoints;
				case "+mm":	return SizeUnits.DeltaMillimeters;
				case "+in":	return SizeUnits.DeltaInches;
				case "%":	return SizeUnits.Percent;
				case "!%":	return SizeUnits.PercentNotCombining;
			}
			
			throw new System.NotSupportedException (string.Format ("Unsupported units: {0}.", text));
		}
		
		public static void DeserializeSizeUnits(string text, out double value, out SizeUnits units)
		{
			string[] args = text.Split (' ');
			
			System.Diagnostics.Debug.Assert (args.Length == 2);
			
			value = System.Double.Parse (args[0], System.Globalization.CultureInfo.InvariantCulture);
			units = UnitsTools.DeserializeSizeUnits (args[1]);
			
			if ((units == SizeUnits.Percent) ||
				(units == SizeUnits.PercentNotCombining))
			{
				value /= 100;
			}
		}
		
		
		public static bool IsAbsoluteSize(SizeUnits units)
		{
			switch (units)
			{
				case SizeUnits.Points:
				case SizeUnits.Millimeters:
				case SizeUnits.Inches:
					return true;
				
				default:
					return false;
			}
		}
		
		public static bool IsRelativeSize(SizeUnits units)
		{
			switch (units)
			{
				case SizeUnits.DeltaPoints:
				case SizeUnits.DeltaMillimeters:
				case SizeUnits.DeltaInches:
					return true;
				
				default:
					return false;
			}
		}
		
		public static bool IsScale(SizeUnits units)
		{
			switch (units)
			{
				case SizeUnits.Percent:
				case SizeUnits.PercentNotCombining:
					return true;
				
				default:
					return false;
			}
		}
		
		
		public static string ConvertToName(SizeUnits units)
		{
			switch (units)
			{
				case SizeUnits.None:				return "";
				case SizeUnits.Points:				return "pt";
				case SizeUnits.Millimeters:			return "mm";
				case SizeUnits.Inches:				return "in";
				case SizeUnits.DeltaPoints:			return "pt";
				case SizeUnits.DeltaMillimeters:	return "mm";
				case SizeUnits.DeltaInches:			return "in";
				case SizeUnits.Percent:				return "%";
				case SizeUnits.PercentNotCombining:	return "!%";
			}
			
			throw new System.NotSupportedException (string.Format ("Unsupported units: {0}.", units));
		}
		
		public static double ConvertToPoints(double value, SizeUnits units)
		{
			switch (units)
			{
				case SizeUnits.Points:
				case SizeUnits.DeltaPoints:
					return value;
				
				case SizeUnits.Millimeters:
				case SizeUnits.DeltaMillimeters:
					return value * 72.0 / 25.4;
				
				case SizeUnits.Inches:
				case SizeUnits.DeltaInches:
					return value * 72.0;
				
				default:
					throw new System.InvalidOperationException (string.Format ("Cannot convert from {0} to {1}.", units, SizeUnits.Points));
			}
		}
		
		public static double ConvertToMillimeters(double value, SizeUnits units)
		{
			switch (units)
			{
				case SizeUnits.Points:
				case SizeUnits.DeltaPoints:
					return value / 72.0 * 25.4;
				
				case SizeUnits.Millimeters:
				case SizeUnits.DeltaMillimeters:
					return value;
				
				case SizeUnits.Inches:
				case SizeUnits.DeltaInches:
					return value * 25.4;
				
				default:
					throw new System.InvalidOperationException (string.Format ("Cannot convert from {0} to {1}.", units, SizeUnits.Millimeters));
			}
		}
		
		public static double ConvertToInches(double value, SizeUnits units)
		{
			switch (units)
			{
				case SizeUnits.Points:
				case SizeUnits.DeltaPoints:
					return value / 72.0;
				
				case SizeUnits.Millimeters:
				case SizeUnits.DeltaMillimeters:
					return value / 25.4;
				
				case SizeUnits.Inches:
				case SizeUnits.DeltaInches:
					return value;
				
				default:
					throw new System.InvalidOperationException (string.Format ("Cannot convert from {0} to {1}.", units, SizeUnits.Inches));
			}
		}
		
		public static double ConvertToScale(double value, SizeUnits units)
		{
			switch (units)
			{
				case SizeUnits.Percent:
				case SizeUnits.PercentNotCombining:
					return value;
				
				default:
					throw new System.InvalidOperationException (string.Format ("Cannot convert from {0} to {1}.", units, SizeUnits.Percent));
			}
		}
		
		public static double ConvertToSizeUnits(double value, SizeUnits fromUnits, SizeUnits toUnits)
		{
			if (fromUnits == toUnits)
			{
				return value;
			}
			if (double.IsNaN (value))
			{
				return double.NaN;
			}
			
			switch (toUnits)
			{
				case SizeUnits.Points:
				case SizeUnits.DeltaPoints:
					return UnitsTools.ConvertToPoints (value, fromUnits);
				
				case SizeUnits.Millimeters:
				case SizeUnits.DeltaMillimeters:
					return UnitsTools.ConvertToMillimeters (value, fromUnits);
				
				case SizeUnits.Inches:
				case SizeUnits.DeltaInches:
					return UnitsTools.ConvertToInches (value, fromUnits);
			}
			
			throw new System.InvalidOperationException (string.Format ("Cannot convert from {0} to {1}.", fromUnits, toUnits));
		}
	}
}
