//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Styles
{
	/// <summary>
	/// Summary description for BaseStyle.
	/// </summary>
	public abstract class BaseStyle : IContentsSignature
	{
		protected BaseStyle()
		{
			this.local_settings = null;
			this.extra_settings = null;
		}
		
		
		public int								StyleIndex
		{
			get
			{
				return this.style_index;
			}
			set
			{
				this.style_index = value;
			}
		}
		
		public abstract bool					IsSpecialStyle
		{
			get;
		}
		
		public int								CountUsers
		{
			get
			{
				return this.user_count;
			}
		}
		
		public int								CountLocalSettings
		{
			get
			{
				return this.local_settings == null ? 0 : this.local_settings.Length;
			}
		}

		public int								CountExtraSettings
		{
			get
			{
				return this.extra_settings == null ? 0 : this.extra_settings.Length;
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
		
		
		internal int FindSettings(Styles.LocalSettings settings)
		{
			if (this.local_settings != null)
			{
				for (int i = 0; i < this.local_settings.Length; i++)
				{
					if (Styles.LocalSettings.CompareEqual (this.local_settings[i], settings))
					{
						Debug.Assert.IsTrue (this.local_settings[i].SettingsIndex == i+1);
						return i+1;
					}
				}
			}
			
			return 0;
		}
		
		internal int FindSettings(Styles.ExtraSettings settings)
		{
			if (this.extra_settings != null)
			{
				for (int i = 0; i < this.extra_settings.Length; i++)
				{
					if (Styles.ExtraSettings.CompareEqual (this.extra_settings[i], settings))
					{
						Debug.Assert.IsTrue (this.extra_settings[i].SettingsIndex == i+1);
						return i+1;
					}
				}
			}
			
			return 0;
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
			
			Debug.Assert.IsNotNull (this.local_settings);
			Debug.Assert.IsInBounds (index, 0, this.local_settings.Length-1);
			Debug.Assert.IsNotNull (this.local_settings[index]);
			Debug.Assert.IsTrue (this.local_settings[index].SettingsIndex == index+1);
			
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
			
			Debug.Assert.IsNotNull (this.extra_settings);
			Debug.Assert.IsInBounds (index, 0, this.extra_settings.Length-1);
			Debug.Assert.IsNotNull (this.extra_settings[index]);
			Debug.Assert.IsTrue (this.extra_settings[index].SettingsIndex == index+1);
			
			return this.extra_settings[index];
		}
		
		
		internal Styles.LocalSettings Attach(Styles.LocalSettings settings)
		{
			//	Ajoute des réglages locaux. S'il n'y a plus de place dans
			//	ce style, retourne null, sinon retourne les réglages qui
			//	ont été réellement utilisés.
			
			if (this.local_settings == null)
			{
				this.local_settings = new Styles.LocalSettings[0];
			}
			else if (this.local_settings.Length == BaseStyle.MaxSettingsCount)
			{
				return null;
			}
			
			int index;
			
			//	Essaie de réutiliser des réglages identiques :
			
			for (index = 0; index < this.local_settings.Length; index++)
			{
				if (Styles.LocalSettings.CompareEqual (this.local_settings[index], settings))
				{
					Debug.Assert.IsTrue (this.local_settings[index].SettingsIndex == index+1);
					goto assign;
				}
			}
			
			//	Essaie de réutiliser un emplacement vide de la table des réglages :
			
			for (index = 0; index < this.local_settings.Length; index++)
			{
				if (this.local_settings[index] == null)
				{
					this.local_settings[index] = settings;
					goto assign;
				}
			}
			
			//	Agrandit la table des réglages et ajoute le nouveau réglage à
			//	la fin :
			
			Debug.Assert.IsTrue (index == this.local_settings.Length);
			
			Styles.LocalSettings[] old_settings = this.local_settings;
			Styles.LocalSettings[] new_settings = new Styles.LocalSettings[index+1];
			
			System.Array.Copy (old_settings, new_settings, old_settings.Length);
			
			new_settings[index] = settings;
			this.local_settings = new_settings;
			
			//	Trouvé le bon enregistrement pour les réglages; il faut encore
			//	mettre à jour le compteur d'utilisation :
			
		assign:
			this.local_settings[index].SettingsIndex = index+1;
			this.local_settings[index].IncrementUserCount ();
			
			return this.local_settings[index];
		}
		
		internal Styles.ExtraSettings Attach(Styles.ExtraSettings settings)
		{
			//	Ajoute des réglages supplémentaires. S'il n'y a plus de place dans
			//	ce style, retourne null, sinon retourne les réglages qui
			//	ont été réellement utilisés.
			
			if (this.extra_settings == null)
			{
				this.extra_settings = new Styles.ExtraSettings[0];
			}
			else if (this.extra_settings.Length == BaseStyle.MaxSettingsCount)
			{
				return null;
			}
			
			int index;
			
			//	Essaie de réutiliser des réglages identiques :
			
			for (index = 0; index < this.extra_settings.Length; index++)
			{
				if (Styles.ExtraSettings.CompareEqual (this.extra_settings[index], settings))
				{
					Debug.Assert.IsTrue (this.extra_settings[index].SettingsIndex == index+1);
					goto assign;
				}
			}
			
			//	Essaie de réutiliser un emplacement vide de la table des réglages :
			
			for (index = 0; index < this.extra_settings.Length; index++)
			{
				if (this.extra_settings[index] == null)
				{
					this.extra_settings[index] = settings;
					goto assign;
				}
			}
			
			//	Agrandit la table des réglages et ajoute le nouveau réglage à
			//	la fin :
			
			Debug.Assert.IsTrue (index == this.extra_settings.Length);
			
			Styles.ExtraSettings[] old_settings = this.extra_settings;
			Styles.ExtraSettings[] new_settings = new Styles.ExtraSettings[index+1];
			
			System.Array.Copy (old_settings, new_settings, old_settings.Length);
			
			new_settings[index] = settings;
			this.extra_settings = new_settings;
			
			//	Trouvé le bon enregistrement pour les réglages; il faut encore
			//	mettre à jour le compteur d'utilisation :
			
		assign:
			this.extra_settings[index].SettingsIndex = index+1;
			this.extra_settings[index].IncrementUserCount ();
			
			return this.extra_settings[index];
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
						settings.SettingsIndex = 0;
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
						settings.SettingsIndex = 0;
						return;
					}
				}
			}
			
			throw new System.ArgumentException ("No such settings.");
		}
		
		
		public static bool CompareEqual(BaseStyle a, BaseStyle b)
		{
			//	Détermine si les deux styles ont le même contenu. Utilise le
			//	plus d'indices possibles avant de passer à la comparaison.
			
			////////////////////////////////////////////////////////////////////
			//  NB: contenu identique n'implique pas que le StyleIndex ou les //
			//      réglages sont identiques !                                //
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
			if (a.IsSpecialStyle != b.IsSpecialStyle)
			{
				return false;
			}
			if (a.GetContentsSignature () != b.GetContentsSignature ())
			{
				return false;
			}
			
			//	Il y a de fortes chances que les deux objets aient le même
			//	contenu. Il faut donc opérer une comparaison des contenus.
			
			//	TODO: comparer les contenus
			
			return true;
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
			//	La signature exclut les réglages et l'index.
			
			//	Si la signature n'existe pas, il faut la calculer; on ne fait
			//	cela qu'à la demande, car le calcul de la signature peut être
			//	relativement onéreux :
			
			if (this.contents_signature == 0)
			{
				int signature = this.ComputeContentsSignature();
				
				//	La signature calculée pourrait être nulle; dans ce cas, on
				//	l'ajuste pour éviter d'interpréter cela comme une absence
				//	de signature :
				
				this.contents_signature = (signature == 0) ? 1 : signature;
			}
			
			return this.contents_signature;
		}
		#endregion
		

		public const int						MaxSettingsCount = 100;
		
		
		private int								style_index;
		private int								contents_signature;
		private int								user_count;
		
		private Styles.LocalSettings[]			local_settings;
		private Styles.ExtraSettings[]			extra_settings;
	}
}
