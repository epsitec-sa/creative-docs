//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Styles
{
	/// <summary>
	/// La classe BaseSettings sert de base pour les réglages, en particulier
	/// LocalSettings et ExtraSettings.
	/// </summary>
	public abstract class BaseSettings : IContentsSignature, IContentsSignatureUpdater, IContentsComparer
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
		
		
		public void IncrementUserCount()
		{
			Debug.Assert.IsInBounds (this.user_count+1, 1, BaseStyle.MaxUserCount-1);
			this.user_count++;
		}
		
		public void DecrementUserCount()
		{
			Debug.Assert.IsInBounds (this.user_count, 1, BaseStyle.MaxUserCount-1);
			this.user_count--;
		}
		
		
		protected void ClearContentsSignature()
		{
			this.contents_signature = 0;
		}
		
		
		#region IContentsSignatureUpdater Members
		public abstract void UpdateContentsSignature(IO.IChecksum checksum);
		#endregion
		
		#region IContentsComparer Members
		public abstract bool CompareEqualContents(object value);
		#endregion
		
		#region IContentsSignature Members
		public int GetContentsSignature()
		{
			//	Retourne la signature (CRC) correspondant au contenu du style.
			//	La signature exclut les réglages et l'index.
			
			//	Si la signature n'existe pas, il faut la calculer; on ne fait
			//	cela qu'à la demande, car le calcul de la signature peut être
			//	relativement onéreux :
			
			if (this.contents_signature == 0)
			{
				IO.IChecksum checksum = IO.Checksum.CreateAdler32 ();
				
				this.UpdateContentsSignature (checksum);
				
				int signature = (int) checksum.Value;
				
				//	La signature calculée pourrait être nulle; dans ce cas, on
				//	l'ajuste pour éviter d'interpréter cela comme une absence
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
