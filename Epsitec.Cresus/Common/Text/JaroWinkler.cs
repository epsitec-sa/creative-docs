namespace Epsitec.Common.Text
{


	public static class JaroWinkler
	{


		public static double ComputeJaroDistance(string s1, string s2)
		{
			JaroWinkler.PreProcessArguments (ref s1, ref s2);

			if (s1 == s2)
			{
				return JaroWinkler.MaxValue;
			}
			else
			{
				return JaroWinkler.ComputeJaroDistanceImplementation (s1, s2);
			}
		}


		private static double ComputeJaroDistanceImplementation(string s1, string s2)
		{
			int s1Length = s1.Length;
			int s2Length = s2.Length;

			int maxDistance = s2Length / 2;
			int nbMatches = 0;
			int nbTranspositions = 0;

			int? previousPosition = null;

			for (int i = 0; i < s1Length; i++)
			{
				char c = s1[i];

				int jMin = System.Math.Max (0, i - maxDistance);
				int jMax = System.Math.Min (s2Length, i + maxDistance);

				for (int j = jMin; j < jMax; j++)
				{
					if (c == s2[j])
					{
						nbMatches++;

						if (previousPosition.HasValue && j < previousPosition)
						{
							nbTranspositions++;
						}

						previousPosition = j;

						break;
					}
				}
			}

			if (nbMatches == 0)
			{
				return JaroWinkler.MinValue;
			}
			else
			{
				double t1 = nbMatches / (double) s1Length;
				double t2 = nbMatches / (double) s2Length;
				double t3 = (nbMatches - nbTranspositions) / (double) nbMatches;

				return (t1 + t2 + t3) / 3;
			}
		}


		public static double ComputeJaroWinklerDistance(string s1, string s2)
		{
			var p = JaroWinkler.DefaultScalingFactor;
			var l = JaroWinkler.DefaultCommonPrefixMaxLength;
			
			return JaroWinkler.ComputeJaroWinklerDistance (s1, s2, p, l);
		}


		public static double ComputeJaroWinklerDistance(string s1, string s2, double p, int l)
		{
			JaroWinkler.PreProcessArguments(ref s1, ref s2);

			if (s1 == s2)
			{
				return JaroWinkler.MaxValue;
			}
			else
			{
				return JaroWinkler.ComputeJaroWinklerDistanceImplementation (s1, s2, p, l);
			}
		}


		private static double ComputeJaroWinklerDistanceImplementation(string s1, string s2, double p, int l)
		{
			double jaroDistance = JaroWinkler.ComputeJaroDistanceImplementation (s1, s2);

			int s1Length = s1.Length;
			int commonPrefixLength = 0;
			int commonPrefixMaxLength = System.Math.Min (l, s1Length);

			while (commonPrefixLength < commonPrefixMaxLength && s1[commonPrefixLength] == s2[commonPrefixLength])
			{
				commonPrefixLength++;
			}

			return jaroDistance + (commonPrefixLength * p * (1 - jaroDistance));
		}


		private static void PreProcessArguments(ref string s1, ref string s2)
		{
			if (s1 == null)
			{
				s1 = "";
			}

			if (s2 == null)
			{
				s2 = "";
			}

			if (s1.Length > s2.Length)
			{
				string tmp = s2;

				s2 = s1;
				s1 = tmp;
			}
		}


		public static readonly double MinValue = 0;


		public static readonly double MaxValue = 1;
		
		
		public static readonly double DefaultScalingFactor = 0.1;
		
		
		public static readonly int DefaultCommonPrefixMaxLength = 4;
		


	}


}
