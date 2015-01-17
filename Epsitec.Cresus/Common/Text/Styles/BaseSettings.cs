//	Copyright � 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Styles
{
	/// <summary>
	/// La classe BaseSettings sert de base � CoreSettings.
	/// </summary>
	public abstract class BaseSettings : PropertyContainer, IContentsComparer
	{
		protected BaseSettings()
		{
		}
		
		protected BaseSettings(System.Collections.ICollection properties) : base (properties)
		{
		}
		
		
		public int								CoreIndex
		{
			//	L'index d'un coreSettings est compris entre 1 et 100'000. Un index
			//	de z�ro correspond � un coreSettings non d�fini.
			
			get
			{
				return this.coreIndex;
			}
			set
			{
				this.coreIndex = value;
			}
		}
		
		public int								CountLocalSettings
		{
			get
			{
				return this.localSettings == null ? 0 : this.localSettings.Length;
			}
		}

		public int								CountExtraSettings
		{
			get
			{
				return this.extraSettings == null ? 0 : this.extraSettings.Length;
			}
		}
		
		
		
		public bool Contains(ulong code, Property property)
		{
			PropertyContainer search = null;
			
			switch (property.PropertyType)
			{
				case Properties.PropertyType.CoreSetting:
					search = this;
					break;
						
				case Properties.PropertyType.LocalSetting:
					search = this.GetLocalSettings (code);
					break;
					
				case Properties.PropertyType.ExtraSetting:
					search = this.GetExtraSettings (code);
					break;
			}
			
			return (search != null) && search.Contains (property);
		}
		
		public bool Contains(ulong code, Properties.WellKnownType wellKnownType, Properties.PropertyType propertyType)
		{
			PropertyContainer search = null;
			
			switch (propertyType)
			{
				case Properties.PropertyType.CoreSetting:
					search = this;
					break;
						
				case Properties.PropertyType.LocalSetting:
					search = this.GetLocalSettings (code);
					break;
					
				case Properties.PropertyType.ExtraSetting:
					search = this.GetExtraSettings (code);
					break;
			}
			
			return (search != null) && search.Contains (wellKnownType);
		}
		
		
		public int FindSettings(LocalSettings settings)
		{
			//	Trouve l'index du r�glage sp�cifi�. Retourne 0 si aucun r�glage
			//	ne peut �tre trouv�.
			
			int n = this.CountLocalSettings;
			
			for (int i = 0; i < n; i++)
			{
				if (LocalSettings.CompareEqual (this.localSettings[i], settings))
				{
					Debug.Assert.IsTrue (this.localSettings[i].SettingsIndex == i+1);
					return i+1;
				}
			}
			
			return 0;
		}
		
		public int FindSettings(ExtraSettings settings)
		{
			//	Trouve l'index du r�glage sp�cifi�. Retourne 0 si aucun r�glage
			//	ne peut �tre trouv�.
			
			int n = this.CountExtraSettings;
			
			for (int i = 0; i < n; i++)
			{
				if (ExtraSettings.CompareEqual (this.extraSettings[i], settings))
				{
					Debug.Assert.IsTrue (this.extraSettings[i].SettingsIndex == i+1);
					return i+1;
				}
			}
			
			return 0;
		}
		
        
		public LocalSettings GetLocalSettings(ulong code)
		{
			//	Retourne les r�glages locaux pour le caract�re sp�cifi�.
			//	Si aucun r�glage n'est associ�, retourne null.
			
			int index = Internal.CharMarker.GetLocalIndex (code);
			
			if (index == 0)
			{
				return null;
			}
			
			index--;		//	ram�ne � 0..n
			
			Debug.Assert.IsNotNull (this.localSettings);
			Debug.Assert.IsInBounds (index, 0, this.localSettings.Length-1);
			Debug.Assert.IsNotNull (this.localSettings[index]);
			Debug.Assert.IsTrue (this.localSettings[index].SettingsIndex == index+1);
			
			return this.localSettings[index];
		}
		
		public ExtraSettings GetExtraSettings(ulong code)
		{
			//	Retourne les r�glages suppl�mentaires pour le caract�re
			//	sp�cifi�. Si aucun r�glage n'est associ�, retourne null.
			
			int index = Internal.CharMarker.GetExtraIndex (code);
			
			if (index == 0)
			{
				return null;
			}
			
			index--;		//	ram�ne � 0..n
			
			Debug.Assert.IsNotNull (this.extraSettings);
			Debug.Assert.IsInBounds (index, 0, this.extraSettings.Length-1);
			Debug.Assert.IsNotNull (this.extraSettings[index]);
			Debug.Assert.IsTrue (this.extraSettings[index].SettingsIndex == index+1);
			
			return this.extraSettings[index];
		}
		
		
		public LocalSettings Attach(LocalSettings settings)
		{
			//	Ajoute des r�glages locaux. S'il n'y a plus de place dans
			//	ce coreSettings, retourne null, sinon retourne les r�glages qui
			//	ont �t� r�ellement utilis�s.
			
			if (this.localSettings == null)
			{
				this.localSettings = new LocalSettings[0];
			}
			
			int index;
			int count = this.localSettings.Length;
			
			//	Essaie de r�utiliser des r�glages identiques :
			
			for (index = 0; index < count; index++)
			{
				if (LocalSettings.CompareEqual (this.localSettings[index], settings))
				{
					Debug.Assert.IsTrue (this.localSettings[index].SettingsIndex == index+1);
					goto assign;
				}
			}
			
			//	Essaie de r�utiliser un emplacement vide de la table des r�glages :
			
			for (index = 0; index < count; index++)
			{
				if (this.localSettings[index] == null)
				{
					this.localSettings[index] = settings;
					goto assign;
				}
			}
			
			//	Agrandit la table des r�glages et ajoute le nouveau r�glage �
			//	la fin :
			
			Debug.Assert.IsTrue (index == count);
			
			if (this.localSettings.Length == BaseSettings.MaxSettingsCount)
			{
				//	La table est pleine. On s'arr�te ici !
				
				return null;
			}
			
			LocalSettings[] oldSettings = this.localSettings;
			LocalSettings[] newSettings = new LocalSettings[index+1];
			
			System.Array.Copy (oldSettings, newSettings, oldSettings.Length);
			
			newSettings[index] = settings;
			this.localSettings = newSettings;
			
			//	Trouv� le bon enregistrement pour les r�glages; il faut encore
			//	mettre � jour le compteur d'utilisation :
			
		assign:
			this.localSettings[index].SettingsIndex = index+1;
//-			this.localSettings[index].IncrementUserCount ();
			
			return this.localSettings[index];
		}
		
		public ExtraSettings Attach(ExtraSettings settings)
		{
			//	Ajoute des r�glages suppl�mentaires. S'il n'y a plus de place dans
			//	ce coreSettings, retourne null, sinon retourne les r�glages qui
			//	ont �t� r�ellement utilis�s.
			
			if (this.extraSettings == null)
			{
				this.extraSettings = new ExtraSettings[0];
			}
			
			int index;
			int count = this.extraSettings.Length;
			
			//	Essaie de r�utiliser des r�glages identiques :
			
			for (index = 0; index < count; index++)
			{
				if (ExtraSettings.CompareEqual (this.extraSettings[index], settings))
				{
					Debug.Assert.IsTrue (this.extraSettings[index].SettingsIndex == index+1);
					goto assign;
				}
			}
			
			//	Essaie de r�utiliser un emplacement vide de la table des r�glages :
			
			for (index = 0; index < count; index++)
			{
				if (this.extraSettings[index] == null)
				{
					this.extraSettings[index] = settings;
					goto assign;
				}
			}
			
			//	Agrandit la table des r�glages et ajoute le nouveau r�glage �
			//	la fin :
			
			Debug.Assert.IsTrue (index == count);
			
			if (this.extraSettings.Length == BaseSettings.MaxSettingsCount)
			{
				//	La table est pleine. On s'arr�te ici !
				
				return null;
			}
			
			ExtraSettings[] oldSettings = this.extraSettings;
			ExtraSettings[] newSettings = new ExtraSettings[index+1];
			
			System.Array.Copy (oldSettings, newSettings, oldSettings.Length);
			
			newSettings[index] = settings;
			this.extraSettings = newSettings;
			
			//	Trouv� le bon enregistrement pour les r�glages; il faut encore
			//	mettre � jour le compteur d'utilisation :
			
		assign:
			this.extraSettings[index].SettingsIndex = index+1;
//-			this.extraSettings[index].IncrementUserCount ();
			
			return this.extraSettings[index];
		}
		
		
		public void Remove(LocalSettings settings)
		{
			//	Supprime la r�f�rence aux r�glages sp�cifi�s. Si les r�glages
			//	ne peuvent pas �tre trouv�s, g�n�re une exception.
			
			if (this.localSettings != null)
			{
				for (int i = 0; i < this.localSettings.Length; i++)
				{
					if (this.localSettings[i] == settings)
					{
						this.localSettings[i] = null;
						settings.SettingsIndex = 0;
						return;
					}
				}
			}
			
			throw new System.ArgumentException ("No such settings.");
		}
		
		public void Remove(ExtraSettings settings)
		{
			//	Supprime la r�f�rence aux r�glages sp�cifi�s. Si les r�glages
			//	ne peuvent pas �tre trouv�s, g�n�re une exception.
			
			if (this.extraSettings != null)
			{
				for (int i = 0; i < this.extraSettings.Length; i++)
				{
					if (this.extraSettings[i] == settings)
					{
						this.extraSettings[i] = null;
						settings.SettingsIndex = 0;
						return;
					}
				}
			}
			
			throw new System.ArgumentException ("No such settings.");
		}
		
		
		public Property[] Flatten(ulong code)
		{
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			
			ExtraSettings extra = this.GetExtraSettings (code);
			LocalSettings local = this.GetLocalSettings (code);
			
			this.Flatten (list);
			
			if (extra != null) extra.Flatten (list);
			if (local != null) local.Flatten (list);
			
			Property[] props = new Property[list.Count];
			list.CopyTo (props);
			
			return props;
		}
		
		
		public static bool CompareEqual(BaseSettings a, BaseSettings b)
		{
			//	D�termine si les deux cores ont le m�me contenu. Utilise le
			//	plus d'indices possibles avant de passer � la comparaison.
			
			///////////////////////////////////////////////////////////////////
			//	NB: contenu identique n'implique pas que le CoreIndex ou les //
			//	r�glages sont identiques !                               //
			///////////////////////////////////////////////////////////////////
			
			if (a == b)
			{
				return true;
			}
			if ((a == null) ||
				(b == null))
			{
				return false;
			}
			if (a.GetType () != b.GetType ())
			{
				return false;
			}
			if (a.GetContentsSignature () != b.GetContentsSignature ())
			{
				return false;
			}
			
			//	Il y a de fortes chances que les deux objets aient le m�me
			//	contenu. Il faut donc op�rer une comparaison des contenus.
			
			return a.CompareEqualContents (b);
		}
		
		
		internal void Serialize(System.Text.StringBuilder buffer)
		{
			buffer.Append (SerializerSupport.SerializeInt (this.coreIndex));
			buffer.Append ("/");
			buffer.Append (SerializerSupport.SerializeInt (this.CountLocalSettings));
			buffer.Append ("/");
			buffer.Append (SerializerSupport.SerializeInt (this.CountExtraSettings));
			buffer.Append ("/");
			
			this.SerializeProperties (buffer);
			
			//	S�rialise maintenant les r�glages "local", s'il y en a :
			
			if ((this.localSettings != null) &&
				(this.localSettings.Length > 0))
			{
				this.SerializeSettings (this.localSettings, buffer);
			}
			
			//	S�rialise maintenant les r�glages "extra", s'il y en a :
			
			if ((this.extraSettings != null) &&
				(this.extraSettings.Length > 0))
			{
				this.SerializeSettings (this.extraSettings, buffer);
			}
		}
		
		internal void Deserialize(TextContext context, int version, string[] args, ref int offset)
		{
			int index   = SerializerSupport.DeserializeInt (args[offset++]);
			int nLocal  = SerializerSupport.DeserializeInt (args[offset++]);
			int nExtra  = SerializerSupport.DeserializeInt (args[offset++]);
			
			this.DeserializeProperties (context, version, args, ref offset);
			
			if (nLocal > 0)
			{
				this.localSettings = new LocalSettings[nLocal];
				
				for (int i = 0; i < nLocal; i++)
				{
					this.localSettings[i] = new LocalSettings ();
				}
				
				this.DeserializeSettings (this.localSettings, context, version, args, ref offset);
			}
			
			if (nExtra > 0)
			{
				this.extraSettings = new ExtraSettings[nExtra];
				
				for (int i = 0; i < nExtra; i++)
				{
					this.extraSettings[i] = new ExtraSettings ();
				}
				
				this.DeserializeSettings (this.extraSettings, context, version, args, ref offset);
			}
			
			this.coreIndex = index;
		}
		
		
		private void SerializeSettings(AdditionalSettings[] settings, System.Text.StringBuilder buffer)
		{
			for (int i = 0; i < settings.Length; i++)
			{
				buffer.Append ("/");
				buffer.Append (SerializerSupport.SerializeInt (settings[i].SettingsIndex));
				buffer.Append ("/");
				settings[i].SerializeProperties (buffer);
			}
		}
		
		private void DeserializeSettings(AdditionalSettings[] settings, TextContext context, int version, string[] source, ref int offset)
		{
			for (int i = 0; i < settings.Length; i++)
			{
				settings[i].SettingsIndex = SerializerSupport.DeserializeInt (source[offset++]);
				settings[i].DeserializeProperties (context, version, source, ref offset);
			}
		}
		
		
		#region IContentsComparer Members
		public abstract bool CompareEqualContents(object value);
		#endregion
		

		public const int						MaxSettingsCount = 100;
		public const int						MaxUserCount = Internal.CursorIdArray.MaxPosition + 1;
		
		private int								coreIndex;
		
		private LocalSettings[]					localSettings;		//	0..n valides; (prendre index-1 pour l'acc�s)
		private ExtraSettings[]					extraSettings;		//	0..n valides; (prendre index-1 pour l'acc�s)
	}
}
