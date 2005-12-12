//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Styles
{
	/// <summary>
	/// La classe AdditionalSettings sert de base pour les réglages aditionnels,
	/// en particulier LocalSettings et ExtraSettings.
	/// </summary>
	public abstract class AdditionalSettings : PropertyContainer, IContentsComparer
	{
		protected AdditionalSettings()
		{
		}
		
		protected AdditionalSettings(System.Collections.ICollection properties) : base (properties)
		{
		}
		
		
		public int								SettingsIndex
		{
			get
			{
				return this.settings_index;
			}
			set
			{
				Debug.Assert.IsInBounds (value, 0, BaseSettings.MaxSettingsCount);
				this.settings_index = (byte) value;
			}
		}
		
		
		#region IContentsComparer Members
		public abstract bool CompareEqualContents(object value);
		#endregion
		
		
		private byte							settings_index;
	}
}
