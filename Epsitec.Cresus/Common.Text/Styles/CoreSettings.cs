//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Styles
{
	/// <summary>
	/// La classe CoreSettings permet de décrire des propriétés fondamentales telles
	/// que défition de fonte et de paragraphe, plus quelques autres détails.
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
