using System.Collections.Generic;


namespace Epsitec.Common.Text
{


	public static class JaroWinkler
	{

		// It is hard to find good documentation on the Jaro-Winkler distance. There is always
		// something missing. Here is what I found:
		// 
		// http://en.wikipedia.org/wiki/Jaro%E2%80%93Winkler_distance
		// http://alias-i.com/lingpipe/docs/api/com/aliasi/spell/JaroWinklerDistance.html
		// http://richardminerich.com/2011/09/record-linkage-algorithms-in-f-jaro-winkler-distance-part-1/
		// http://richardminerich.com/2011/09/record-linkage-algorithms-in-f-%E2%80%93-jaro-winkler-distance-part-2/
		// http://richardminerich.com/2011/09/record-linkage-algorithms-in-f-extensions-to-jaro-winkler-distance-part-3/
		// http://text-analysis.sourceforge.net/workspace/Text-Analysis/reports/junit/eu/kostia/textanalysis/similarity/22_ComparisonAllMetricsTest-out.txt
		// 
		// The biggest problem is that there is not always a precise definition of every small
		// detail on the Jaro distance algorithm. I had to put back the pieces together from the
		// links above to finaly get it right (I think).


		public static double ComputeJaroDistance(string s1, string s2)
		{
			// We swap the arguments if necessary, to ensure that the shorter string comes before
			// the other.
			JaroWinkler.PreProcessArguments (ref s1, ref s2);

			// If the string are equal, we skip to the final result.
			if (s1 == s2)
			{
				return JaroWinkler.MaxValue;
			}
			
			return JaroWinkler.ComputeJaroDistanceImplementation (s1, s2);
		}


		private static double ComputeJaroDistanceImplementation(string s1, string s2)
		{
			// This method is the most complex one. The formula for the Jaro distance is:
			// if m = 0 => 0
			// else     => 1/3 * (m/length(s1) + m/length(s2) + (m-t)/m)
			//
			// m is the number of matching characters:
			// A character from s1 matches a character from s2 only if there are the same and they
			// are not farther than a maximum distance which is defined as:
			// floor(max(length(s1), length(s2)) / 2) - 1
			// Moreover, a character cannot be matched twice. So to find the matching characters,
			// we must iterate over each character in s1 to check if there is an equal character in
			// s2 within the maximum distance, that as not already been matched by another
			// character from s1.
			// The matching sequence is the sequence of matching characters from one string to the
			// other.
			//
			// t is the number of transpositions.
			// The matching sequence from s1 to s2 and the matching sequence from s2 to s1 have the
			// same size and the same elements, but their elements might be ordered differently. The
			// number of transpositions represents this difference in ordering. Its formula is:
			// floor(ht / 2)
			// where ht is the number of half transpositions. This number is the the number of
			// indexes from those sequence where the character is not the same.

			int s1Length = s1.Length;
			int s2Length = s2.Length;

			// Here we compute the maximum distance used to check if two characters are matching or
			// not.
			int maxDistance = (s2Length / 2) - 1;

			// We will put in this variable the matching sequence from s1 to s2.
			List<char> s1Matches = new List<char> ();

			// We will put in this variable a bool for each char in s2 that indicates whether it has
			// been matched or not. This will be usefull to avoid matching a character twice and to
			// build the matching sequence from s2 to s1 more efficiently later on.
			bool[] s2Matched = new bool[s2Length];

			// We look over all the characters from s1 to get the matching sequence from s1 to s2.
			for (int i = 0; i < s1Length; i++)
			{
				char c = s1[i];

				// We compute the range of characters in s2 where matches are allowed according to
				// the definition.
				int jMin = System.Math.Max (0, i - maxDistance);
				int jMax = System.Math.Min (s2.Length - 1, i + maxDistance);

				// Here we iterage over that range to find a matching character.
				for (int j = jMin; j <= jMax; j++)
				{
					// If the characters are the same and if the character from s2 has not already
					// been matched, we have found a matching charater. We add it to the matching
					// sequence from s1 to s2 and set the bool to indicate that the characted in s2
					// has been matched and cannot be matched again.
					if (c == s2[j] && !s2Matched[j])
					{
						s1Matches.Add (c);
						s2Matched[j] = true;

						break;
					}
				}
			}

			// The number of match is the length of the matching sequence. If that number is zero,
			// we skip to the final result.
			int nbMatches = s1Matches.Count;

			if (nbMatches == 0)
			{
				return JaroWinkler.MinValue;
			}

			// Here we reconstruct the matching sequence from s2 to s1 based on the booleans that we
			// stored to indicate whether a character from s2 has been matched or not.
			List<char> s2Matches = new List<char> ();

			for (int i = 0; i < s2Length; i++)
			{
				if (s2Matched[i])
				{
					s2Matches.Add (s2[i]);
				}
			}

			// Now that we have both sequences, we count the number of positions where they differ
			// and that gives us the number of half transpositions.
			double nbHalfTranspositions = 0;

			for (int i = 0; i < s1Matches.Count; i++)
			{
				if (s1Matches[i] != s2Matches[i])
				{
					nbHalfTranspositions++;
				}
			}

			// We get the number of transposition by dividing by two and rounding down. Note the
			// cast to int whose purpose is the rounding down.
			int nbTranspositions = (int) nbHalfTranspositions / 2;

			// At last, we apply the formula for the Jaro distance and we are done.
			double t1 = nbMatches / (double) s1Length;
			double t2 = nbMatches / (double) s2Length;
			double t3 = (nbMatches - nbTranspositions) / (double) nbMatches;

			return (t1 + t2 + t3) / 3;
		}
		
		
		public static double ComputeJaroWinklerDistance(string s1, string s2)
		{
			var p = JaroWinkler.DefaultScalingFactor;
			var l = JaroWinkler.DefaultCommonPrefixMaxLength;
			
			return JaroWinkler.ComputeJaroWinklerDistance (s1, s2, p, l);
		}


		public static double ComputeJaroWinklerDistance(string s1, string s2, double p, int l)
		{
			// We swap the arguments if necessary, to ensure that the shorter string comes before
			// the other.
			JaroWinkler.PreProcessArguments(ref s1, ref s2);

			// If the string are equal, we skip to the final result.
			if (s1 == s2)
			{
				return JaroWinkler.MaxValue;
			}

			return JaroWinkler.ComputeJaroWinklerDistanceImplementation (s1, s2, p, l);
		}


		private static double ComputeJaroWinklerDistanceImplementation(string s1, string s2, double p, int l)
		{
			// Here we simply apply the Jaro-Winkler formula, which is:
			// dw = dj + (cpl * p * (1 - dj))

			double jaroDistance = JaroWinkler.ComputeJaroDistanceImplementation (s1, s2);
			int commonPrefixLength = JaroWinkler.ComputeCommonPrefixLength (s1, s2, l);

			return jaroDistance + (commonPrefixLength * p * (1 - jaroDistance));
		}


		private static int ComputeCommonPrefixLength(string s1, string s2, int l)
		{
			// Here we simply get the length of the common prefix of both strings.

			int cpl = 0;

			int max = System.Math.Min (l, s1.Length);

			while (cpl < max && s1[cpl] == s2[cpl])
			{
				cpl++;
			}

			return cpl;
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
