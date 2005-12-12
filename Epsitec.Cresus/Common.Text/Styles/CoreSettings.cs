//	Copyright � 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Styles
{
	/// <summary>
	/// La classe CoreSettings permet de d�crire des propri�t�s fondamentales telles
	/// que d�fition de fonte et de paragraphe, plus quelques autres d�tails.
	/// </summary>
	public sealed class CoreSettings : BaseSettings
	{
		public CoreSettings()
		{
		}
		
		public CoreSettings(System.Collections.ICollection properties) : base (properties)
		{
		}
		
		
		public override void UpdateContentsSignature(IO.IChecksum checksum)
		{
			base.UpdateContentsSignature (checksum);
		}
		
		public override bool CompareEqualContents(object value)
		{
			return Styles.PropertyContainer.CompareEqualContents (this, value as Styles.CoreSettings);
		}
		
	}
}
