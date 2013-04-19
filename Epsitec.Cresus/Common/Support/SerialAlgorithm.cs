//	Copyright © 2005-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Serial;

namespace Epsitec.Common.Support
{
	/// <summary>
	/// SerialAlgorithm définit les fonctions de base nécessaires pour valider
	/// une clef de logiciel.
	/// </summary>
	public static class SerialAlgorithm
	{
		public static int						ProductGeneration
		{
			get;
			private set;
		}

		public static int						ProductGracePeriod
		{
			get;
			private set;
		}

		public static System.DateTime			BuildDate
		{
			get;
			private set;
		}
		
		
		public static string ReadCrDocSerial()
		{
			string key = null;
			return (string) Microsoft.Win32.Registry.GetValue (SerialAlgorithm.RegistrySerialPath, "ID", key);
		}
		
		
		public static bool CheckSerial(string key, int productFamily)
		{
			return CresusSerialAlgorithm.CheckSerial (key, productFamily);
		}

		public static bool CheckSerial(string key, int productFamily, out bool updatesAllowed)
		{
			return CresusSerialAlgorithm.CheckSerial (key, productFamily, SerialAlgorithm.BuildDate, out updatesAllowed);
		}

		public static System.DateTime GetExpirationDate(string key)
		{
			return CresusSerialAlgorithm.GetSerialLimit (key);
		}

		public static int GetProduct(string key)
		{
			return CresusSerialAlgorithm.GetProduct (key);
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
		
		
		private const string RegistrySerialPath = @"HKEY_LOCAL_MACHINE\SOFTWARE\Epsitec\Cresus Documents\Setup";
	}
}
