//	Copyright � 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Styles
{
	/// <summary>
	/// Summary description for BaseStyle.
	/// </summary>
	internal abstract class BaseStyle : IContentsSignature
	{
		protected BaseStyle()
		{
			this.local_settings = null;
			this.extra_settings = null;
		}
		
		
		public int								StyleIndex
		{
			//	L'index d'un style est compris entre 1 et 100'000. Un index
			//	de z�ro correspond � un style non d�fini.
			
			get
			{
				return this.style_index;
			}
			set
			{
				this.style_index = value;
			}
		}
		
		public abstract bool					IsRichStyle
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
		
		
		public int FindSettings(Styles.LocalSettings settings)
		{
			//	Trouve l'index du r�glage sp�cifi�. Retourne 0 si aucun r�glage
			//	ne peut �tre trouv�.
			
			int n = this.CountLocalSettings;
			
			for (int i = 0; i < n; i++)
			{
				if (Styles.LocalSettings.CompareEqual (this.local_settings[i], settings))
				{
					Debug.Assert.IsTrue (this.local_settings[i].SettingsIndex == i+1);
					return i+1;
				}
			}
			
			return 0;
		}
		
		public int FindSettings(Styles.ExtraSettings settings)
		{
			//	Trouve l'index du r�glage sp�cifi�. Retourne 0 si aucun r�glage
			//	ne peut �tre trouv�.
			
			int n = this.CountExtraSettings;
			
			for (int i = 0; i < n; i++)
			{
				if (Styles.ExtraSettings.CompareEqual (this.extra_settings[i], settings))
				{
					Debug.Assert.IsTrue (this.extra_settings[i].SettingsIndex == i+1);
					return i+1;
				}
			}
			
			return 0;
		}
		
        
		public Styles.LocalSettings GetLocalSettings(ulong code)
		{
			//	Retourne les r�glages locaux pour le caract�re sp�cifi�.
			//	Si aucun r�glage n'est associ�, retourne null.
			
			int index = Internal.CharMarker.GetLocalIndex (code);
			
			if (index == 0)
			{
				return null;
			}
			
			index--;		//	ram�ne � 0..n
			
			Debug.Assert.IsNotNull (this.local_settings);
			Debug.Assert.IsInBounds (index, 0, this.local_settings.Length-1);
			Debug.Assert.IsNotNull (this.local_settings[index]);
			Debug.Assert.IsTrue (this.local_settings[index].SettingsIndex == index+1);
			
			return this.local_settings[index];
		}
		
		public Styles.ExtraSettings GetExtraSettings(ulong code)
		{
			//	Retourne les r�glages suppl�mentaires pour le caract�re
			//	sp�cifi�. Si aucun r�glage n'est associ�, retourne null.
			
			int index = Internal.CharMarker.GetExtraIndex (code);
			
			if (index == 0)
			{
				return null;
			}
			
			index--;		//	ram�ne � 0..n
			
			Debug.Assert.IsNotNull (this.extra_settings);
			Debug.Assert.IsInBounds (index, 0, this.extra_settings.Length-1);
			Debug.Assert.IsNotNull (this.extra_settings[index]);
			Debug.Assert.IsTrue (this.extra_settings[index].SettingsIndex == index+1);
			
			return this.extra_settings[index];
		}
		
		
		public Styles.LocalSettings Attach(Styles.LocalSettings settings)
		{
			//	Ajoute des r�glages locaux. S'il n'y a plus de place dans
			//	ce style, retourne null, sinon retourne les r�glages qui
			//	ont �t� r�ellement utilis�s.
			
			if (this.local_settings == null)
			{
				this.local_settings = new Styles.LocalSettings[0];
			}
			else if (this.local_settings.Length == BaseStyle.MaxSettingsCount)
			{
				return null;
			}
			
			int index;
			int count = this.local_settings.Length;
			
			//	Essaie de r�utiliser des r�glages identiques :
			
			for (index = 0; index < count; index++)
			{
				if (Styles.LocalSettings.CompareEqual (this.local_settings[index], settings))
				{
					Debug.Assert.IsTrue (this.local_settings[index].SettingsIndex == index+1);
					goto assign;
				}
			}
			
			//	Essaie de r�utiliser un emplacement vide de la table des r�glages :
			
			for (index = 0; index < count; index++)
			{
				if (this.local_settings[index] == null)
				{
					this.local_settings[index] = settings;
					goto assign;
				}
			}
			
			//	Agrandit la table des r�glages et ajoute le nouveau r�glage �
			//	la fin :
			
			Debug.Assert.IsTrue (index == count);
			
			Styles.LocalSettings[] old_settings = this.local_settings;
			Styles.LocalSettings[] new_settings = new Styles.LocalSettings[index+1];
			
			System.Array.Copy (old_settings, new_settings, old_settings.Length);
			
			new_settings[index] = settings;
			this.local_settings = new_settings;
			
			//	Trouv� le bon enregistrement pour les r�glages; il faut encore
			//	mettre � jour le compteur d'utilisation :
			
		assign:
			this.local_settings[index].SettingsIndex = index+1;
			this.local_settings[index].IncrementUserCount ();
			
			return this.local_settings[index];
		}
		
		public Styles.ExtraSettings Attach(Styles.ExtraSettings settings)
		{
			//	Ajoute des r�glages suppl�mentaires. S'il n'y a plus de place dans
			//	ce style, retourne null, sinon retourne les r�glages qui
			//	ont �t� r�ellement utilis�s.
			
			if (this.extra_settings == null)
			{
				this.extra_settings = new Styles.ExtraSettings[0];
			}
			else if (this.extra_settings.Length == BaseStyle.MaxSettingsCount)
			{
				return null;
			}
			
			int index;
			int count = this.extra_settings.Length;
			
			//	Essaie de r�utiliser des r�glages identiques :
			
			for (index = 0; index < count; index++)
			{
				if (Styles.ExtraSettings.CompareEqual (this.extra_settings[index], settings))
				{
					Debug.Assert.IsTrue (this.extra_settings[index].SettingsIndex == index+1);
					goto assign;
				}
			}
			
			//	Essaie de r�utiliser un emplacement vide de la table des r�glages :
			
			for (index = 0; index < count; index++)
			{
				if (this.extra_settings[index] == null)
				{
					this.extra_settings[index] = settings;
					goto assign;
				}
			}
			
			//	Agrandit la table des r�glages et ajoute le nouveau r�glage �
			//	la fin :
			
			Debug.Assert.IsTrue (index == count);
			
			Styles.ExtraSettings[] old_settings = this.extra_settings;
			Styles.ExtraSettings[] new_settings = new Styles.ExtraSettings[index+1];
			
			System.Array.Copy (old_settings, new_settings, old_settings.Length);
			
			new_settings[index] = settings;
			this.extra_settings = new_settings;
			
			//	Trouv� le bon enregistrement pour les r�glages; il faut encore
			//	mettre � jour le compteur d'utilisation :
			
		assign:
			this.extra_settings[index].SettingsIndex = index+1;
			this.extra_settings[index].IncrementUserCount ();
			
			return this.extra_settings[index];
		}
		
		
		public void Remove(Styles.LocalSettings settings)
		{
			//	Supprime la r�f�rence aux r�glages sp�cifi�s. Si les r�glages
			//	ne peuvent pas �tre trouv�s, g�n�re une exception.
			
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
			//	Supprime la r�f�rence aux r�glages sp�cifi�s. Si les r�glages
			//	ne peuvent pas �tre trouv�s, g�n�re une exception.
			
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
			//	D�termine si les deux styles ont le m�me contenu. Utilise le
			//	plus d'indices possibles avant de passer � la comparaison.
			
			////////////////////////////////////////////////////////////////////
			//  NB: contenu identique n'implique pas que le StyleIndex ou les //
			//      r�glages sont identiques !                                //
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
			if (a.IsRichStyle != b.IsRichStyle)
			{
				return false;
			}
			if (a.GetContentsSignature () != b.GetContentsSignature ())
			{
				return false;
			}
			
			//	Il y a de fortes chances que les deux objets aient le m�me
			//	contenu. Il faut donc op�rer une comparaison des contenus.
			
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
		

		public const int						MaxSettingsCount = 100;
		public const int						MaxUserCount = 1000*1000;
		
		private int								style_index;
		private int								contents_signature;
		private int								user_count;
		
		private Styles.LocalSettings[]			local_settings;		//	0..n valides; (prendre index-1 pour l'acc�s)
		private Styles.ExtraSettings[]			extra_settings;		//	0..n valides; (prendre index-1 pour l'acc�s)
	}
}
