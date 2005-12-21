//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Styles
{
	/// <summary>
	/// La classe BaseSettings sert de base à CoreSettings.
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
			//	L'index d'un core_settings est compris entre 1 et 100'000. Un index
			//	de zéro correspond à un core_settings non défini.
			
			get
			{
				return this.core_index;
			}
			set
			{
				this.core_index = value;
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
		
		public bool Contains(ulong code, Properties.WellKnownType well_known_type, Properties.PropertyType property_type)
		{
			PropertyContainer search = null;
			
			switch (property_type)
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
			
			return (search != null) && search.Contains (well_known_type);
		}
		
		
		public int FindSettings(LocalSettings settings)
		{
			//	Trouve l'index du réglage spécifié. Retourne 0 si aucun réglage
			//	ne peut être trouvé.
			
			int n = this.CountLocalSettings;
			
			for (int i = 0; i < n; i++)
			{
				if (LocalSettings.CompareEqual (this.local_settings[i], settings))
				{
					Debug.Assert.IsTrue (this.local_settings[i].SettingsIndex == i+1);
					return i+1;
				}
			}
			
			return 0;
		}
		
		public int FindSettings(ExtraSettings settings)
		{
			//	Trouve l'index du réglage spécifié. Retourne 0 si aucun réglage
			//	ne peut être trouvé.
			
			int n = this.CountExtraSettings;
			
			for (int i = 0; i < n; i++)
			{
				if (ExtraSettings.CompareEqual (this.extra_settings[i], settings))
				{
					Debug.Assert.IsTrue (this.extra_settings[i].SettingsIndex == i+1);
					return i+1;
				}
			}
			
			return 0;
		}
		
        
		public LocalSettings GetLocalSettings(ulong code)
		{
			//	Retourne les réglages locaux pour le caractère spécifié.
			//	Si aucun réglage n'est associé, retourne null.
			
			int index = Internal.CharMarker.GetLocalIndex (code);
			
			if (index == 0)
			{
				return null;
			}
			
			index--;		//	ramène à 0..n
			
			Debug.Assert.IsNotNull (this.local_settings);
			Debug.Assert.IsInBounds (index, 0, this.local_settings.Length-1);
			Debug.Assert.IsNotNull (this.local_settings[index]);
			Debug.Assert.IsTrue (this.local_settings[index].SettingsIndex == index+1);
			
			return this.local_settings[index];
		}
		
		public ExtraSettings GetExtraSettings(ulong code)
		{
			//	Retourne les réglages supplémentaires pour le caractère
			//	spécifié. Si aucun réglage n'est associé, retourne null.
			
			int index = Internal.CharMarker.GetExtraIndex (code);
			
			if (index == 0)
			{
				return null;
			}
			
			index--;		//	ramène à 0..n
			
			Debug.Assert.IsNotNull (this.extra_settings);
			Debug.Assert.IsInBounds (index, 0, this.extra_settings.Length-1);
			Debug.Assert.IsNotNull (this.extra_settings[index]);
			Debug.Assert.IsTrue (this.extra_settings[index].SettingsIndex == index+1);
			
			return this.extra_settings[index];
		}
		
		
		public LocalSettings Attach(LocalSettings settings)
		{
			//	Ajoute des réglages locaux. S'il n'y a plus de place dans
			//	ce core_settings, retourne null, sinon retourne les réglages qui
			//	ont été réellement utilisés.
			
			if (this.local_settings == null)
			{
				this.local_settings = new LocalSettings[0];
			}
			
			int index;
			int count = this.local_settings.Length;
			
			//	Essaie de réutiliser des réglages identiques :
			
			for (index = 0; index < count; index++)
			{
				if (LocalSettings.CompareEqual (this.local_settings[index], settings))
				{
					Debug.Assert.IsTrue (this.local_settings[index].SettingsIndex == index+1);
					goto assign;
				}
			}
			
			//	Essaie de réutiliser un emplacement vide de la table des réglages :
			
			for (index = 0; index < count; index++)
			{
				if (this.local_settings[index] == null)
				{
					this.local_settings[index] = settings;
					goto assign;
				}
			}
			
			//	Agrandit la table des réglages et ajoute le nouveau réglage à
			//	la fin :
			
			Debug.Assert.IsTrue (index == count);
			
			if (this.local_settings.Length == BaseSettings.MaxSettingsCount)
			{
				//	La table est pleine. On s'arrête ici !
				
				return null;
			}
			
			LocalSettings[] old_settings = this.local_settings;
			LocalSettings[] new_settings = new LocalSettings[index+1];
			
			System.Array.Copy (old_settings, new_settings, old_settings.Length);
			
			new_settings[index] = settings;
			this.local_settings = new_settings;
			
			//	Trouvé le bon enregistrement pour les réglages; il faut encore
			//	mettre à jour le compteur d'utilisation :
			
		assign:
			this.local_settings[index].SettingsIndex = index+1;
//-			this.local_settings[index].IncrementUserCount ();
			
			return this.local_settings[index];
		}
		
		public ExtraSettings Attach(ExtraSettings settings)
		{
			//	Ajoute des réglages supplémentaires. S'il n'y a plus de place dans
			//	ce core_settings, retourne null, sinon retourne les réglages qui
			//	ont été réellement utilisés.
			
			if (this.extra_settings == null)
			{
				this.extra_settings = new ExtraSettings[0];
			}
			
			int index;
			int count = this.extra_settings.Length;
			
			//	Essaie de réutiliser des réglages identiques :
			
			for (index = 0; index < count; index++)
			{
				if (ExtraSettings.CompareEqual (this.extra_settings[index], settings))
				{
					Debug.Assert.IsTrue (this.extra_settings[index].SettingsIndex == index+1);
					goto assign;
				}
			}
			
			//	Essaie de réutiliser un emplacement vide de la table des réglages :
			
			for (index = 0; index < count; index++)
			{
				if (this.extra_settings[index] == null)
				{
					this.extra_settings[index] = settings;
					goto assign;
				}
			}
			
			//	Agrandit la table des réglages et ajoute le nouveau réglage à
			//	la fin :
			
			Debug.Assert.IsTrue (index == count);
			
			if (this.extra_settings.Length == BaseSettings.MaxSettingsCount)
			{
				//	La table est pleine. On s'arrête ici !
				
				return null;
			}
			
			ExtraSettings[] old_settings = this.extra_settings;
			ExtraSettings[] new_settings = new ExtraSettings[index+1];
			
			System.Array.Copy (old_settings, new_settings, old_settings.Length);
			
			new_settings[index] = settings;
			this.extra_settings = new_settings;
			
			//	Trouvé le bon enregistrement pour les réglages; il faut encore
			//	mettre à jour le compteur d'utilisation :
			
		assign:
			this.extra_settings[index].SettingsIndex = index+1;
//-			this.extra_settings[index].IncrementUserCount ();
			
			return this.extra_settings[index];
		}
		
		
		public void Remove(LocalSettings settings)
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
		
		public void Remove(ExtraSettings settings)
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
			//	Détermine si les deux cores ont le même contenu. Utilise le
			//	plus d'indices possibles avant de passer à la comparaison.
			
			///////////////////////////////////////////////////////////////////
			//	NB: contenu identique n'implique pas que le CoreIndex ou les //
			//	réglages sont identiques !                               //
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
			
			//	Il y a de fortes chances que les deux objets aient le même
			//	contenu. Il faut donc opérer une comparaison des contenus.
			
			return a.CompareEqualContents (b);
		}
		
		
		internal void Serialize(System.Text.StringBuilder buffer)
		{
			buffer.Append (SerializerSupport.SerializeInt (this.core_index));
			buffer.Append ("/");
			buffer.Append (SerializerSupport.SerializeInt (this.CountLocalSettings));
			buffer.Append ("/");
			buffer.Append (SerializerSupport.SerializeInt (this.CountExtraSettings));
			buffer.Append ("/");
			
			this.SerializeProperties (buffer);
			
			//	Sérialise maintenant les réglages "local", s'il y en a :
			
			if ((this.local_settings != null) &&
				(this.local_settings.Length > 0))
			{
				this.SerializeSettings (this.local_settings, buffer);
			}
			
			//	Sérialise maintenant les réglages "extra", s'il y en a :
			
			if ((this.extra_settings != null) &&
				(this.extra_settings.Length > 0))
			{
				this.SerializeSettings (this.extra_settings, buffer);
			}
		}
		
		internal void Deserialize(TextContext context, int version, string[] args, ref int offset)
		{
			int index   = SerializerSupport.DeserializeInt (args[offset++]);
			int n_local = SerializerSupport.DeserializeInt (args[offset++]);
			int n_extra = SerializerSupport.DeserializeInt (args[offset++]);
			
			this.DeserializeProperties (context, version, args, ref offset);
			
			if (n_local > 0)
			{
				this.local_settings = new LocalSettings[n_local];
				
				for (int i = 0; i < n_local; i++)
				{
					this.local_settings[i] = new LocalSettings ();
				}
				
				this.DeserializeSettings (this.local_settings, context, version, args, ref offset);
			}
			
			if (n_extra > 0)
			{
				this.extra_settings = new ExtraSettings[n_extra];
				
				for (int i = 0; i < n_extra; i++)
				{
					this.extra_settings[i] = new ExtraSettings ();
				}
				
				this.DeserializeSettings (this.extra_settings, context, version, args, ref offset);
			}
			
			this.core_index = index;
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
		
		private int								core_index;
		
		private LocalSettings[]					local_settings;		//	0..n valides; (prendre index-1 pour l'accès)
		private ExtraSettings[]					extra_settings;		//	0..n valides; (prendre index-1 pour l'accès)
	}
}
