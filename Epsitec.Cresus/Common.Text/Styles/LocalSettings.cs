//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Styles
{
	/// <summary>
	/// Summary description for LocalSettings.
	/// </summary>
	internal sealed class LocalSettings : BaseSettings
	{
		public LocalSettings()
		{
		}
		
		
		public static bool CompareEqual(LocalSettings a, LocalSettings b)
		{
			//	Détermine si les deux réglages ont le même contenu. Utilise le
			//	plus d'indices possibles avant de passer à la comparaison.
			
			////////////////////////////////////////////////////////////////////
			//  NB: contenu identique n'implique pas que le SettingsIndex est //
			//      identique !                                               //
			////////////////////////////////////////////////////////////////////
			
			if (a == b)
			{
				return true;
			}
			if ((a == null) ||
				(b == null))
			{
				return false;
			}
			if (a.GetContentsSignature () != b.GetContentsSignature ())
			{
				return false;
			}
			
			//	Il y a de fortes chances que les deux objets aient le même
			//	contenu. Il faut donc opérer une comparaison des contenus.
			
			//	TODO: comparer les contenus
			
			return true;
		}
		
		
		protected override int ComputeContentsSignature()
		{
			return 0;
		}
	}
}
