//	Copyright � 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Styles
{
	/// <summary>
	/// La classe BaseSettings sert de base pour les r�glages, en particulier
	/// LocalSettings et ExtraSettings.
	/// </summary>
	public abstract class BaseSettings : IContentsSignature
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
		
		public int								CountUsers
		{
			get
			{
				return this.user_count;
			}
		}
		
		
		internal void IncrementUserCount()
		{
			Debug.Assert.IsInBounds (this.user_count+1, 1, 1000000-1);
			this.user_count++;
		}
		
		internal void DecrementUserCount()
		{
			Debug.Assert.IsInBounds (this.user_count, 1, 1000000-1);
			this.user_count--;
		}
		
		
		
		
		protected void ClearContentsSignature()
		{
			this.contents_signature = 0;
		}
		
		protected abstract int ComputeContentsSignature();
		
		#region IContentsSignature Members
		public int GetContentsSignature()
		{
			//	Retourne la signature (CRC) correspondant au contenu du style.
			//	La signature exclut les r�glages et l'index.
			
			//	Si la signature n'existe pas, il faut la calculer; on ne fait
			//	cela qu'� la demande, car le calcul de la signature peut �tre
			//	relativement on�reux :
			
			if (this.contents_signature == 0)
			{
				int signature = this.ComputeContentsSignature();
				
				//	La signature calcul�e pourrait �tre nulle; dans ce cas, on
				//	l'ajuste pour �viter d'interpr�ter cela comme une absence
				//	de signature :
				
				this.contents_signature = (signature == 0) ? 1 : signature;
			}
			
			return this.contents_signature;
		}
		#endregion
		
		private byte							settings_index;
		private int								contents_signature;
		private int								user_count;
	}
}
