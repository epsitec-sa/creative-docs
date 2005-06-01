//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Properties
{
	/// <summary>
	/// La classe UnitsTools offre quelques méthodes de conversion.
	/// </summary>
	public sealed class UnitsTools
	{
		private UnitsTools()
		{
		}
		
		
		public static void Combine(double a_value, SizeUnits a_units, double b_value, SizeUnits b_units, out double c_value, out SizeUnits c_units)
		{
			switch (b_units)
			{
				case SizeUnits.Percent:				//	xxx * Percent --> xxx
					c_units = a_units;
					c_value = a_value * b_value / 100.0;
					break;
				
				case SizeUnits.Points:				//	[pt] --> [pt] (écrase)
				case SizeUnits.Millimeters:			//	[mm] --> [mm] (écrase)
				case SizeUnits.Inches:				//	[in] --> [in] (écrase)
					c_units = b_units;
					c_value = b_value;
					break;
				
				case SizeUnits.None:				//	xxx * [rien] --> xxx source
					c_units = a_units;
					c_value = a_value;
					break;
				
				case SizeUnits.DeltaPoints:			//	ajoute des deltas, conserve l'unité de départ
				case SizeUnits.DeltaMillimeters:
				case SizeUnits.DeltaInches:
					if (a_units == SizeUnits.None)
					{
						c_units = b_units;
						c_value = b_value;
					}
					else
					{
						c_units = a_units;
						c_value = a_value + UnitsTools.ConvertToSizeUnits (b_value, b_units, a_units);
					}
					break;
				
				default:
					throw new System.NotSupportedException (string.Format ("Unsupported units: {0}.", b_units));
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
			}
			
			throw new System.NotSupportedException (string.Format ("Unsupported units: {0}.", units));
		}
		
		public static SizeUnits DeserializeSizeUnits(string units)
		{
			switch (units)
			{
				case "":	return SizeUnits.None;
				case "pt":	return SizeUnits.Points;
				case "mm":	return SizeUnits.Millimeters;
				case "in":	return SizeUnits.Inches;
				case "+pt":	return SizeUnits.DeltaPoints;
				case "+mm":	return SizeUnits.DeltaMillimeters;
				case "+in":	return SizeUnits.DeltaInches;
				case "%":	return SizeUnits.Percent;
			}
			
			throw new System.NotSupportedException (string.Format ("Unsupported units: {0}.", units));
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
		
		public static double ConvertToSizeUnits(double value, SizeUnits from_units, SizeUnits to_units)
		{
			if (from_units == to_units)
			{
				return value;
			}
			
			switch (to_units)
			{
				case SizeUnits.Points:
				case SizeUnits.DeltaPoints:
					return UnitsTools.ConvertToPoints (value, from_units);
				
				case SizeUnits.Millimeters:
				case SizeUnits.DeltaMillimeters:
					return UnitsTools.ConvertToMillimeters (value, from_units);
				
				case SizeUnits.Inches:
				case SizeUnits.DeltaInches:
					return UnitsTools.ConvertToInches (value, from_units);
			}
			
			throw new System.InvalidOperationException (string.Format ("Cannot convert from {0} to {1}.", from_units, to_units));
		}
	}
}
