//	Copyright � 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Styles
{
	/// <summary>
	/// Summary description for LocalSettings.
	/// </summary>
	public sealed class LocalSettings : BaseSettings
	{
		public LocalSettings()
		{
		}
		
		
		public static bool CompareEqual(LocalSettings a, LocalSettings b)
		{
			//	D�termine si les deux r�glages ont le m�me contenu. Utilise le
			//	plus d'indices possibles avant de passer � la comparaison.
			
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
			if (a.GetType () != b.GetType ())
			{
				return false;
			}
			if (a.GetContentsSignature () != b.GetContentsSignature ())
			{
				return false;
			}
			
			//	Il y a de fortes chances que les deux objets aient le m�me
			//	contenu. Il faut donc op�rer une comparaison des contenus.
			
			return a.CompareEqualContents (b);
		}
		
		
		public override void UpdateContentsSignature(IO.IChecksum checksum)
		{
		}
		
		public override bool CompareEqualContents(object value)
		{
			return true;
		}
	}
}
