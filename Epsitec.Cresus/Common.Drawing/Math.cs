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
		
		
		public static double ClipAngle(double angle)
		{
			//	Retourne un angle normalisé, c'est-à-dire compris entre 0 et 2*PI.
			
			angle = angle % (System.Math.PI*2.0);
			return (angle < 0.0) ? System.Math.PI*2.0 + angle : angle;
		}
	}
}
