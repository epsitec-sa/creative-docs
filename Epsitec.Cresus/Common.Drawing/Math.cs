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
	}
}
