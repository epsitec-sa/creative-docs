//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common
{
	public class Math
	{
		private Math()
		{
		}
		
		
		public static float Clip(float value)
		{
			if (value < 0) return 0;
			if (value > 1) return 1;
			return value;
		}
		
		public static double Clip(double value)
		{
			if (value < 0.0) return 0.0;
			if (value > 1.0) return 1.0;
			return value;
		}
		
		
		public static double ClipAngleRad(double angle)
		{
			//	Retourne un angle normalisé, c'est-à-dire compris entre 0 et 2*PI.
			
			angle = angle % (System.Math.PI*2.0);
			return (angle < 0.0) ? System.Math.PI*2.0 + angle : angle;
		}
		
		public static double ClipAngleDeg(double angle)
		{
			//	Retourne un angle normalisé, c'est-à-dire compris entre 0 et 360°.
			
			angle = angle % 360.0;
			return (angle < 0.0) ? 360.0 + angle : angle;
		}
		
		public static double DegToRad(double angle)
		{
			return angle * System.Math.PI / 180.0;
		}
		
		public static double RadToDeg(double angle)
		{
			return angle * 180.0 / System.Math.PI;
		}
		
		
		public static bool Equal(double a, double b, double δ)
		{
			//	Compare deux nombres avec une certaine marge d'erreur.
			
			if (a == b)
			{
				return true;
			}
			
			if (double.IsNaN (a) && double.IsNaN (b))
			{
				return true;
			}
			
			if (double.IsNegativeInfinity (a) && double.IsNegativeInfinity (b))
			{
				return true;
			}
			
			if (double.IsPositiveInfinity (a) && double.IsPositiveInfinity (b))
			{
				return true;
			}
			
			double diff = a - b;
			
			if (diff < 0)
			{
				return -diff < δ;
			}
			else
			{
				return diff < δ;
			}
		}
	}
}
