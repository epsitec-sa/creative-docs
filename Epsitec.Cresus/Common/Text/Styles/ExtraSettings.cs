//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
			//	Détermine si les deux réglages ont le même contenu. Utilise le
			//	plus d'indices possibles avant de passer à la comparaison.
			
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
			
			//	Il y a de fortes chances que les deux objets aient le même
			//	contenu. Il faut donc opérer une comparaison des contenus.
			
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
