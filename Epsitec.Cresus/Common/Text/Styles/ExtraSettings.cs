//	Copyright � 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Styles
{
	/// <summary>
	/// Summary description for ExtraSettings.
	/// </summary>
	public sealed class ExtraSettings : AdditionalSettings
	{
		public ExtraSettings()
		{
		}
		
		public ExtraSettings(System.Collections.ICollection properties) : base (properties)
		{
		}
		
		
		public static bool CompareEqual(ExtraSettings a, ExtraSettings b)
		{
			//	D�termine si les deux r�glages ont le m�me contenu. Utilise le
			//	plus d'indices possibles avant de passer � la comparaison.
			
			////////////////////////////////////////////////////////////////////
			//	NB: contenu identique n'implique pas que le SettingsIndex est //
			//	identique !                                               //
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
			base.UpdateContentsSignature (checksum);
		}
		
		public override bool CompareEqualContents(object value)
		{
			return Styles.PropertyContainer.CompareEqualContents (this, value as Styles.ExtraSettings);
		}
	}
}
