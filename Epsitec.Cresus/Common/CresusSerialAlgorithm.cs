//	Copyright © 2005-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

//	Derived from an implementation done by Denis Dumoulin

namespace Epsitec.Serial
{
	public static class CresusSerialAlgorithm
	{
		#region	Algorithmes et détails internes...
		private static int f1(long n)
		{
			string tmp = n.ToString (System.Globalization.CultureInfo.InvariantCulture);
			int check = 0;
			int[] crypt = { 233,123,43,17,2,34,132,175,122,128, 102, 85, 43,77,55,33,22,47 };
			
			int len = tmp.Length;
			
			for (int i = 0; i < len; i++)
			{
				check += ((int)tmp[i]) * crypt[i];
			}
			
			return (check) % 99 ;
		}
		
		private static int f2(long n)
		{
			string tmp = n.ToString (System.Globalization.CultureInfo.InvariantCulture);
			int check = 0;
			int[] crypt = { 123,178,23,55,89,211,222,53,17,67,22,33,87,12,3,4,5,6 };
			
			int len = tmp.Length;
			
			for (int i = 0; i < len-1; i++)
			{
				check += (int)tmp[i] ^ (int)tmp[i+1] * crypt[i] ;
			}
			
			return (check) % 99 ;
		}

		private static int f3(long n)
		{
			int check1 = CresusSerialAlgorithm.f1 (n);
			int check2 = CresusSerialAlgorithm.f2 (n);
			
			return check1 + 100 * check2;
		}
		
		
		private static bool TestSerial0(int a, int b, int c)
		{
			a &= 0xffff;  // il faut tronquer à 16 bits un numéro trop grand, du style '92100'
			return (c == CresusSerialAlgorithm.f3 (1000000 * a + b));
		}

		private static int GetSerialCompl(int a, long b, int c)
		{
			b = 10000000L * (b%100) + 10000L * c + a;
			return CresusSerialAlgorithm.f3 (b) % 1000;
		}

		
		private static bool TestFullSyntax(string snum)
		{
			//	ppppp-nnnnnn-ssss-cccccc
			
			if ((snum == null) ||
				(snum.Length != 24))
			{
				return false;
			}
			
			if ((snum[ 5] != '-') ||
				(snum[12] != '-') ||
				(snum[17] != '-'))
			{
				return false;
			}
			
			string[] chunks = new string[4];
			
			chunks[0] = snum.Substring ( 0, 5);
			chunks[1] = snum.Substring ( 6, 6);
			chunks[2] = snum.Substring (13, 4); 
			chunks[3] = snum.Substring (18, 6);
			
			foreach (string chunk in chunks)
			{
				for (int i = 0; i < chunk.Length; i++)
				{
					if (System.Char.IsDigit (chunk, i) == false)
					{
						return false;
					}
				}
			}
			
			return true;
		}
		
		private static bool InternalTestFullSerial(string snum, out bool updatesAllowed)
		{
			//	ppppp-nnnnnn-ssss-cccccc
			
			updatesAllowed = false;

			if (CresusSerialAlgorithm.TestFullSyntax (snum) == false)
			{
				return false;
			}
			
			string sprodid   = snum.Substring ( 0, 5);
			string snumero   = snum.Substring ( 6, 6);
			string schecksum = snum.Substring (13, 4); 
			string scompl    = snum.Substring (18, 6);
			
			int prodid   = System.Int32.Parse (sprodid, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);
			int numero   = System.Int32.Parse (snumero, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);
			int checksum = System.Int32.Parse (schecksum, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);

			if (!CresusSerialAlgorithm.TestSerial0 (prodid, numero, checksum))
			{
				return false;
			}
			
			int complement = System.Int32.Parse (scompl, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);
			int limite     = (complement/100)%1000;			//	extrait la date limite
			complement     = complement/100000 + 10 * ((complement/10)%10) + 100 * (complement%10);
			
			if (scompl != "000000")
			{
				var now         = System.DateTime.Now;
				int month       = now.Month + (now.Year - 2000) * 12;
				int serialcompl = CresusSerialAlgorithm.GetSerialCompl (checksum, numero, limite);
				
				if (complement == serialcompl)
				{
					updatesAllowed = month <= limite;
					return true;
				}
			}
			
			return false;
		}
		
		private static System.DateTime InternalGetSerialLimit(string snum)
		{
			//	Si on ne peut pas déterminer de date limite pour la clef, on prétend simplement
			//	que la clef a échu le mois passé. Ca devrait suffire dans la majeure partie des
			//	cas.
			
			System.DateTime past = new System.DateTime (System.DateTime.Now.Year, System.DateTime.Now.Month, 1).Subtract (System.TimeSpan.FromDays (1));

			if (CresusSerialAlgorithm.TestFullSyntax (snum) == false)
			{
				return past;
			}
			
			string sprodid   = snum.Substring ( 0, 5);
			string snumero   = snum.Substring ( 6, 6);
			string schecksum = snum.Substring (13, 4); 
			string scompl    = snum.Substring (18, 6);

			int prodid   = System.Int32.Parse (sprodid, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);
			int numero   = System.Int32.Parse (snumero, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);
			int checksum = System.Int32.Parse (schecksum, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);

			if (!CresusSerialAlgorithm.TestSerial0 (prodid, numero, checksum))
			{
				return past;
			}

			int complement = System.Int32.Parse (scompl, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);
			int limite     = (complement/100)%1000;			//	extrait la date limite
			
			int year  = 2000+limite/12;
			int month = 1+limite%12;
			
			return new System.DateTime (year, month, 1);
		}

		#endregion

		public static System.DateTime GetSerialLimit(string snum)
		{
			return CresusSerialAlgorithm.InternalGetSerialLimit (snum);
		}

		public static bool TestSerialFormat(string snum)
		{
			if (string.IsNullOrEmpty (snum))
			{
				return false;
			}

			if (snum.Length == 17)
			{
				//	Make sure we have a full serial number here.
				snum = snum + "-000000";
			}

			return CresusSerialAlgorithm.TestSerial (snum);
		}

		public static bool TestSerial(string snum)
		{
			bool updatesAllowed;
			return CresusSerialAlgorithm.TestSerial (snum, out updatesAllowed);
		}
		
		public static bool TestSerial(string snum, out bool updatesAllowed)
		{
			return CresusSerialAlgorithm.InternalTestFullSerial (snum, out updatesAllowed);
		}

		public static bool CheckSerial(string key, int productFamily)
		{
			bool updatesAllowed;
			return CresusSerialAlgorithm.CheckSerial (key, productFamily, new System.DateTime (2013, 4, 18), out updatesAllowed);
		}

		public static bool CheckSerial(string key, int productFamily, System.DateTime buildDate, out bool updatesAllowed)
		{
			if (CresusSerialAlgorithm.TestSerial (key, out updatesAllowed))
			{
				if (buildDate >= System.DateTime.Now)
				{
					//	The user is cheating be using a system time which has been set back to
					//	a past date...

					updatesAllowed = false;
				}

				var product = CresusSerialAlgorithm.GetProduct (key) % 100;
				
				if ((product == productFamily) ||
					(productFamily == 0))
				{
					//	C'est le bon produit (par ex. x40 = Crésus Documents)

					return true;
				}
			}

			return false;
		}

		public static System.DateTime GetExpirationDate(string key)
		{
			return CresusSerialAlgorithm.GetSerialLimit (key);
		}

		public static int GetProduct(string key)
		{
			//	ppppp-nnnnnn-ssss-cccccc

			if (CresusSerialAlgorithm.TestFullSyntax (key) == false)
			{
				return 0;
			}

			var sprodid = key.Substring (0, 5);
			int prodid  = System.Int32.Parse (sprodid, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);

			return prodid / 100;
		}

		public static int GetRevision(string key)
		{
			//	ppppp-nnnnnn-ssss-cccccc

			if (CresusSerialAlgorithm.TestFullSyntax (key) == false)
			{
				return 0;
			}

			var sprodid = key.Substring (0, 5);
			int prodid  = System.Int32.Parse (sprodid, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);

			return prodid % 100;
		}
	}
}
