//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Styles
{
	/// <summary>
	/// La classe BaseSettings sert de base pour les réglages, en particulier
	/// LocalSettings et ExtraSettings.
	/// </summary>
	public abstract class BaseSettings
	{
		protected BaseSettings()
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
				Debug.Assert.IsInBounds (value, 0, BaseStyle.MaxSettingsCount);
				this.settings_index = (byte) value;
			}
		}
		
		
		
		private byte							settings_index;
	}
}
