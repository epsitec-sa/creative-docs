//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Styles
{
	/// <summary>
	/// Summary description for BaseStyle.
	/// </summary>
	public abstract class BaseStyle
	{
		protected BaseStyle()
		{
			this.local_settings = null;
			this.extra_settings = null;
		}
		
		
		
		
		public Styles.LocalSettings GetLocalSettings(ulong code)
		{
			//	Retourne les réglages locaux pour le caractère spécifié.
			//	Si aucun réglage n'est associé, retourne null.
			
			int index = Internal.CharMarker.GetLocalIndex (code);
			
			if (index == 0)
			{
				return null;
			}
			
			index--;
			
			Debug.Assert.IsTrue (this.local_settings != null);
			Debug.Assert.IsInBounds (index, 0, this.local_settings.Length-1);
			Debug.Assert.IsTrue (this.local_settings[index] != null);
			
			return this.local_settings[index];
		}
		
		public Styles.ExtraSettings GetExtraSettings(ulong code)
		{
			//	Retourne les réglages supplémentaires pour le caractère
			//	spécifié. Si aucun réglage n'est associé, retourne null.
			
			int index = Internal.CharMarker.GetExtraIndex (code);
			
			if (index == 0)
			{
				return null;
			}
			
			index--;
			
			Debug.Assert.IsTrue (this.extra_settings != null);
			Debug.Assert.IsInBounds (index, 0, this.extra_settings.Length-1);
			Debug.Assert.IsTrue (this.extra_settings[index] != null);
			
			return this.extra_settings[index];
		}
		
		
		public int Add(Styles.LocalSettings settings)
		{
			//	Ajoute des réglages locaux. S'il n'y a plus de place dans
			//	ce style, retourne 0, sinon retourne l'index à utiliser.
			
			if (this.local_settings == null)
			{
				this.local_settings = new Styles.LocalSettings[1];
				this.local_settings[0] = settings;
				return 1;
			}
			
			if (this.local_settings.Length == BaseStyle.MaxSettingsCount)
			{
				return 0;
			}
			
			for (int i = 0; i < this.local_settings.Length; i++)
			{
				if (this.local_settings[i] == null)
				{
					this.local_settings[i] = settings;
					return i+1;
				}
			}
			
			int index = this.local_settings.Length;
			
			Styles.LocalSettings[] old_settings = this.local_settings;
			Styles.LocalSettings[] new_settings = new Styles.LocalSettings[index+1];
			
			System.Array.Copy (old_settings, new_settings, old_settings.Length);
			
			new_settings[index] = settings;
			this.local_settings = new_settings;
			
			return index+1;
		}
		
		public int Add(Styles.ExtraSettings settings)
		{
			//	Ajoute des réglages supplémentaires. S'il n'y a plus de place dans
			//	ce style, retourne 0, sinon retourne l'index à utiliser.
			
			if (this.extra_settings == null)
			{
				this.extra_settings = new Styles.ExtraSettings[1];
				this.extra_settings[0] = settings;
				return 1;
			}
			
			if (this.extra_settings.Length == BaseStyle.MaxSettingsCount)
			{
				return 0;
			}
			
			for (int i = 0; i < this.extra_settings.Length; i++)
			{
				if (this.extra_settings[i] == null)
				{
					this.extra_settings[i] = settings;
					return i+1;
				}
			}
			
			int index = this.extra_settings.Length;
			
			Styles.ExtraSettings[] old_settings = this.extra_settings;
			Styles.ExtraSettings[] new_settings = new Styles.ExtraSettings[index+1];
			
			System.Array.Copy (old_settings, new_settings, old_settings.Length);
			
			new_settings[index] = settings;
			this.extra_settings = new_settings;
			
			return index+1;
		}
		
		
		public void Remove(Styles.LocalSettings settings)
		{
			//	Supprime la référence aux réglages spécifiés. Si les réglages
			//	ne peuvent pas être trouvés, génère une exception.
			
			if (this.local_settings != null)
			{
				for (int i = 0; i < this.local_settings.Length; i++)
				{
					if (this.local_settings[i] == settings)
					{
						this.local_settings[i] = null;
						return;
					}
				}
			}
			
			throw new System.ArgumentException ("No such settings.");
		}
		
		public void Remove(Styles.ExtraSettings settings)
		{
			//	Supprime la référence aux réglages spécifiés. Si les réglages
			//	ne peuvent pas être trouvés, génère une exception.
			
			if (this.extra_settings != null)
			{
				for (int i = 0; i < this.extra_settings.Length; i++)
				{
					if (this.extra_settings[i] == settings)
					{
						this.extra_settings[i] = null;
						return;
					}
				}
			}
			
			throw new System.ArgumentException ("No such settings.");
		}
		
		
		public const int						MaxSettingsCount = 100;
		
		private Styles.LocalSettings[]			local_settings;
		private Styles.ExtraSettings[]			extra_settings;
	}
}
