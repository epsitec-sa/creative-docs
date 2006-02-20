//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Michael WALZ

namespace Epsitec.Common.Text.Exchange
{
	/// <summary>
	/// La classe Rosetta joue le rôle de plate-forme centrale pour la conversion
	/// de formats de texte (la "pierre de Rosette").
	/// </summary>
	public class Rosetta
	{
		public Rosetta()
		{
		}
		
		public string ConvertCtmlToHtml(string ctml)
		{
			//	Méthode bidon juste pour vérifier si les tests compilent.
			
			return "TODO";
		}

		public static void TestCode(TextStory story, TextNavigator navigator)
		{
			//	TODO: ajouter le code de test; appelé par Crésus Documents
			
			System.Diagnostics.Debug.WriteLine ("Code de test appelé.");
		}
	}
}
