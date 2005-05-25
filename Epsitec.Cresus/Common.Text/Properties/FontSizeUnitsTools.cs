//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Properties
{
	/// <summary>
	/// La classe FontSizeUnitsTools offre quelques méthodes de conversion.
	/// </summary>
	public sealed class FontSizeUnitsTools
	{
		private FontSizeUnitsTools()
		{
		}
		
		
		public static void Combine(double a_value, FontSizeUnits a_units, double b_value, FontSizeUnits b_units, out double c_value, out FontSizeUnits c_units)
		{
			switch (b_units)
			{
				case FontSizeUnits.Percent:				//	xxx * Percent --> xxx
					c_units = a_units;
					c_value = a_value * b_value / 100.0;
					break;
				
				case FontSizeUnits.Points:				//	Points --> Points (écrase)
					c_units = b_units;
					c_value = b_value;
					break;
				
				case FontSizeUnits.None:				//	xxx * [rien] --> xxx source
					c_units = a_units;
					c_value = a_value;
					break;
				
				case FontSizeUnits.DeltaPoints:			//	(Delta)Points + DeltaPoints --> (Delta)Points, sinon erreur
					if ((a_units != FontSizeUnits.Points) &&
						(a_units != FontSizeUnits.DeltaPoints))
					{
						throw new System.InvalidOperationException ("Invalid units combination.");
					}
					c_units = a_units;
					c_value = a_value + b_value;
					break;
				
				default:
					throw new System.InvalidOperationException ("Unsupported units.");
			}
		}
	}
}
