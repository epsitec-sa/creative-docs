//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD, dérivé du code de DD

namespace Epsitec.App.DocumentEditor.Installer
{
	/// <summary>
	/// Summary description for SerialAlgorithm.
	/// </summary>
	public class SerialAlgorithm
	{
		#region	Algorithmes
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

		
		internal static bool TestSerial(string snum)
		{
			//	ppppp-nnnnnn-ssss-cccccc
			
			if (snum.Length != 24)
			{
				return false;
			}
			
			if ((snum[ 5] != '-') ||
				(snum[12] != '-') ||
				(snum[17] != '-'))
			{
				return false;
			}

			try
			{
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
				//	la date courante pour faire la mise à jour...
				
				if (scompl != "000000")
				{
					int month       = System.DateTime.Now.Month + (System.DateTime.Now.Year - 2000) * 12;
					int serialcompl = SerialAlgorithm.GetSerialCompl (checksum, numero, limite);
					
					if ((complement == serialcompl) &&
						(month <= limite) &&
						(month > 60))
					{
						return true;
					}
				}
				
				//	...soit l'appelant utilise une révision de produit qui est conforme à la
				//	révision actuelle (une nouvelle clef permet d'installer un soft plus vieux
				//	ou équivalent).
				
				if (prodrev >= SerialAlgorithm.ProductRevision)
				{
					return true;
				}
			}
			catch
			{
			}
			
			return false;
		}
		#endregion
		
		//	Lors de chaque nouvelle version majeure du programme, il faut incrémenter le
		//	numéro ci-dessous (attention, nous sommes limités dans la durée de vie d'un
		//	produit à la plage 01..99).
		//
		//	Si on veut générer une clef qui va pour les version 01..08 (par exemple), il
		//	faut une clef de type 04008-nnnnnn-ssss
		//
		//	Si on veut générer une clef avec une date de mise à jour ultime, il faut une
		//	clef de type 04000-nnnnnn-ssss-cccccc (le numéro complémentaire code la date
		//	d'expiration; cccccc=*072** signifie décembre 2005 - on compte le nombre de
		//	mois depuis janvier 2000).
		
		private const int	ProductRevision = 1;
		
		//	Voici les dates des diverses versions :
		//
		//	01 --> 20/01/2005
	}
}
