//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD, dérivé du code de DD

namespace Epsitec.Common.Support
{
	/// <summary>
	/// SerialAlgorithm définit les fonctions de base nécessaires pour valider
	/// une clef de logiciel.
	/// </summary>
	public class SerialAlgorithm
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
			int check1 = SerialAlgorithm.f1 (n);
			int check2 = SerialAlgorithm.f2 (n);
			
			return check1 + 100 * check2;
		}
		
		
		private static bool TestSerial0(int a, int b, int c)
		{
			return (c == SerialAlgorithm.f3 (1000000 * a + b)) ;
		}

		private static int GetSerialCompl(int a, long b, int c)
		{
			b = 10000000 * (b%100) + 10000 * c + a;
			return SerialAlgorithm.f3 (b) % 1000;
		}

		
		private static bool TestSyntax(string snum)
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
		
		private static bool TestSerial(string snum)
		{
			//	ppppp-nnnnnn-ssss-cccccc
			
			if (SerialAlgorithm.TestSyntax (snum) == false)
			{
				return false;
			}
			
			string sprodid   = snum.Substring ( 0, 5);
			string snumero   = snum.Substring ( 6, 6);
			string schecksum = snum.Substring (13, 4); 
			string scompl    = snum.Substring (18, 6);
			
			int prodid   = System.Int32.Parse (sprodid);
			int numero   = System.Int32.Parse (snumero);
			int checksum = System.Int32.Parse (schecksum);
			
			if (!SerialAlgorithm.TestSerial0 (prodid, numero, checksum))
			{
				return false;
			}
			
			int complement = System.Int32.Parse (scompl);
			int limite     = (complement/100)%1000;			//	extrait la date limite
			complement     = complement/100000 + 10 * ((complement/10)%10) + 100 * (complement%10);
			
			int product = prodid / 100;
			int prodrev = prodid % 100;
			
			if (product != 40)
			{
				//	Le produit 40, c'est "Crésus Documents". Si l'utilisateur donne un
				//	autre numéro, on échoue ici.
				
				return false;
			}
			
			//	Soit, l'appelant fournit un numéro complémentaire valide par rapport à
			//	la date de compilation du logiciel...
			
			if (scompl != "000000")
			{
				int month       = SerialAlgorithm.BuildDate.Month + (SerialAlgorithm.BuildDate.Year - 2000) * 12;
				int serialcompl = SerialAlgorithm.GetSerialCompl (checksum, numero, limite);
				
				if ((complement == serialcompl) &&
					(month <= limite) &&
					(SerialAlgorithm.BuildDate < System.DateTime.Now))
				{
					return true;
				}
			}
			
			//	...soit l'appelant utilise une révision de produit qui est conforme à la
			//	révision actuelle (en se basant cette fois-ci sur le "build generation" à
			//	la place de la date de "release").
			
			if (prodrev >= (SerialAlgorithm.ProductGeneration - SerialAlgorithm.ProductGracePeriod))
			{
				return true;
			}
			
			return false;
		}
		
		private static System.DateTime GetSerialLimit(string snum)
		{
			//	Si on ne peut pas déterminer de date limite pour la clef, on prétend simplement
			//	que la clef a échu le mois passé. Ca devrait suffire dans la majeure partie des
			//	cas.
			
			System.DateTime past = new System.DateTime (System.DateTime.Now.Year, System.DateTime.Now.Month, 1).Subtract (System.TimeSpan.FromDays (1));
			
			if (SerialAlgorithm.TestSyntax (snum) == false)
			{
				return past;
			}
			
			string sprodid   = snum.Substring ( 0, 5);
			string snumero   = snum.Substring ( 6, 6);
			string schecksum = snum.Substring (13, 4); 
			string scompl    = snum.Substring (18, 6);
			
			int prodid   = System.Int32.Parse (sprodid);
			int numero   = System.Int32.Parse (snumero);
			int checksum = System.Int32.Parse (schecksum);
			
			if (!SerialAlgorithm.TestSerial0 (prodid, numero, checksum))
			{
				return past;
			}
			
			int complement = System.Int32.Parse (scompl);
			int limite     = (complement/100)%1000;			//	extrait la date limite
			
			int year  = 2000+limite/12;
			int month = 1+limite%12;
			
			return new System.DateTime (year, month, 1);
		}
		
		private static string GetAppDataPath()
		{
			string path = System.Windows.Forms.Application.CommonAppDataPath;
			
			int pos = path.LastIndexOf ("\\");
			
			if (pos < 0)
			{
				return null;
			}
			
			return path.Substring (0, pos);
		}
		
		static System.DateTime	BuildDate			= System.DateTime.Now;	// <- doit être mis à jour
		static int				ProductGeneration	= 1;
		static int				ProductGracePeriod	= 0;
		#endregion
		
		public static string ReadSerial()
		{
			string path = SerialAlgorithm.GetAppDataPath ();
			string key  = null;
			
			if (path == null)
			{
				throw new System.InvalidOperationException ("Cannot retrieve AppDataPath");
			}
			
			path = System.IO.Path.Combine (path, "serial.info");
			
			if (System.IO.File.Exists (path))
			{
				using (System.IO.StreamReader reader = System.IO.File.OpenText (path))
				{
					key = reader.ReadLine ();
				}
			}
			
			return key;
		}
		
		public static void WriteSerial(string key)
		{
			string path = SerialAlgorithm.GetAppDataPath ();
			
			if (path == null)
			{
				throw new System.InvalidOperationException ("Cannot retrieve AppDataPath");
			}
			
			path = System.IO.Path.Combine (path, "serial.info");
			
			using (System.IO.StreamWriter writer = new System.IO.StreamWriter (path, false))
			{
				writer.WriteLine (key);
			}
		}
		
		public static bool CheckSerial(string key)
		{
			return SerialAlgorithm.TestSerial (key);
		}

		public static System.DateTime GetExpirationDate(string key)
		{
			return SerialAlgorithm.GetSerialLimit(key);
		}
		
		
		
		public static void SetProductBuildDate(System.DateTime date)
		{
			SerialAlgorithm.BuildDate = date;
		}
		
		public static void SetProductGenerationNumber(int generation, int grace)
		{
			SerialAlgorithm.ProductGeneration  = generation;
			SerialAlgorithm.ProductGracePeriod = grace;
		}
	}
}
