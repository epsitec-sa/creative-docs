//	Copyright � 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
		
		
		
		public Styles.LocalSettings GetLocalSettings(ulong code)
		{
			//	Retourne les r�glages locaux pour le caract�re sp�cifi�.
			//	Si aucun r�glage n'est associ�, retourne null.
			
			int index = Internal.CharMarker.GetLocalIndex (code);
			
			if (index == 0)
			{
				return null;
			}
			
			index--;
			
			Debug.Assert.IsTrue (this.local_settings != null);
			Debug.Assert.IsInBounds (index, 0, this.local_settings.Length-1);
			Debug.Assert.IsTrue (this.local_settings[index] != null);
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
			
			index--;
			
			Debug.Assert.IsTrue (this.extra_settings != null);
			Debug.Assert.IsInBounds (index, 0, this.extra_settings.Length-1);
			Debug.Assert.IsTrue (this.extra_settings[index] != null);
			Debug.Assert.IsTrue (this.extra_settings[index].SettingsIndex == index+1);
			
			return this.extra_settings[index];
		}
		
		
		public int Add(Styles.LocalSettings settings)
		{
			//	Ajoute des r�glages locaux. S'il n'y a plus de place dans
			//	ce style, retourne 0, sinon retourne l'index � utiliser.
			
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
			
			settings.SettingsIndex = index+1;
			
			return index+1;
		}
		
		public int Add(Styles.ExtraSettings settings)
		{
			//	Ajoute des r�glages suppl�mentaires. S'il n'y a plus de place dans
			//	ce style, retourne 0, sinon retourne l'index � utiliser.
			
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
			
			settings.SettingsIndex = index+1;
			
			return index+1;
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
			if (a.IsSpecialStyle != b.IsSpecialStyle)
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
		
		
		private int								style_index;
		private int								contents_signature;
		
		private Styles.LocalSettings[]			local_settings;
		private Styles.ExtraSettings[]			extra_settings;
	}
}
