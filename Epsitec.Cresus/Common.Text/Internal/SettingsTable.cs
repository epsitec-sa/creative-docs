//	Copyright © 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Internal
{
	/// <summary>
	/// La classe SettingsTable permet d'accéder aux réglages d'après le code d'un
	/// caractère. De manière interne, SettingsTable gère une liste avec les réglages
	/// fondamentaux (core settings).
	/// </summary>
	internal sealed class SettingsTable
	{
		public SettingsTable()
		{
			this.cores = null;
		}
		
		
		public void Attach(ref ulong code, Styles.CoreSettings core_settings)
		{
			//	Attache un core_settings au caractère passé en entrée.
			
			//	Prend note que le core_settings a une utilisation de plus (si un
			//	core_settings identique existe déjà, c'est ce core_settings là qui
			//	sera réutilisé; on évite ainsi les doublons).
			
			Styles.CoreSettings find = this.FindCore (core_settings, null);
			
			if (find == null)
			{
				this.Add (core_settings);
				find = core_settings;
			}
			
			Debug.Assert.IsFalse (Internal.CharMarker.HasCoreOrSettings (code));
			Debug.Assert.IsTrue (find.CoreIndex > 0);
			
//-			find.IncrementUserCount ();
			
			Internal.CharMarker.SetCoreIndex (ref code, find.CoreIndex);
			Internal.CharMarker.SetLocalIndex (ref code, 0);
			Internal.CharMarker.SetExtraIndex (ref code, 0);
		}
		
		public void Attach(ref ulong code, Styles.CoreSettings core_settings, Styles.LocalSettings local_settings, Styles.ExtraSettings extra_settings)
		{
			//	Variante de Attach avec réglages spécifiques (voir méthode simple
			//	ci-dessus).
			
			if ((local_settings != null) &&
				(local_settings.IsEmpty))
			{
				local_settings = null;
			}
			
			if ((extra_settings != null) &&
				(extra_settings.IsEmpty))
			{
				extra_settings = null;
			}
			
			if ((local_settings == null) &&
				(extra_settings == null))
			{
				this.Attach (ref code, core_settings);
			}
			else
			{
				//	Trouve un core_settings qui abrite à la fois les réglages locaux et
				//	les réglages supplémentaires :
				
				SettingsCoreMatcher matcher = new SettingsCoreMatcher (local_settings, extra_settings);
				Styles.CoreSettings  find    = this.FindCore (core_settings, new CoreMatcher (matcher.FindExactSettings));
				
				if ((find == null) &&
					(matcher.WasCalled))
				{
					//	On avait trouvé un core_settings, mais il ne faisait pas l'affaire,
					//	car il n'avait pas les réglages demandés. Il faut donc voir
					//	si parmi les candidats il y a un core_settings avec de la place
					//	pour y placer les réglages demandés :
					
					find = this.FindCore (core_settings, new CoreMatcher (matcher.FindFreeSettings));
				}
				
				if (find == null)
				{
					//	Aucun core_settings ne fait l'affaire (soit parce qu'il n'y en a
					//	pas, soit parce que ceux qui existent sont déjà complets
					//	en ce qui concerne les réglages).
					
					//	On en ajoute un neuf :
					
					this.Add (core_settings);
					find = core_settings;
				}
				
				Debug.Assert.IsTrue (find.CoreIndex > 0);
				Debug.Assert.IsFalse (Internal.CharMarker.HasCoreOrSettings (code));
				
				//	Ajoute les réglages au core_settings s'ils n'en font pas encore partie :
				
				if (local_settings != null)
				{
					local_settings = find.Attach (local_settings);
					Debug.Assert.IsNotNull (local_settings);
					Debug.Assert.IsTrue (local_settings.SettingsIndex > 0);
				}
				if (extra_settings != null)
				{
					extra_settings = find.Attach (extra_settings);
					Debug.Assert.IsNotNull (extra_settings);
					Debug.Assert.IsTrue (extra_settings.SettingsIndex > 0);
				}
				
//-				find.IncrementUserCount ();
				
				Internal.CharMarker.SetCoreIndex (ref code, find.CoreIndex);
				Internal.CharMarker.SetLocalIndex (ref code, local_settings == null ? 0 : local_settings.SettingsIndex);
				Internal.CharMarker.SetExtraIndex (ref code, extra_settings == null ? 0 : extra_settings.SettingsIndex);
			}
		}
		
		
		public void Detach(ref ulong code)
		{
			//	Détache le core_settings et les réglages associés au caractère 'code'.
			//	Ceci décrémente les divers compteurs d'utilisation.
			
			int core_index  = Internal.CharMarker.GetCoreIndex (code);
			int extra_index = Internal.CharMarker.GetExtraIndex (code);
			int local_index = Internal.CharMarker.GetLocalIndex (code);
			
			if (core_index == 0)
			{
				Debug.Assert.IsTrue (extra_index == 0);
				Debug.Assert.IsTrue (local_index == 0);
			}
			else
			{
				Styles.CoreSettings core_settings = this.cores[core_index-1] as Styles.CoreSettings;
				
				Debug.Assert.IsNotNull (core_settings);
				
				if (local_index != 0)
				{
//-					core_settings.GetLocalSettings (code).DecrementUserCount ();
					Internal.CharMarker.SetLocalIndex (ref code, 0);
				}
				if (extra_index != 0)
				{
//-					core_settings.GetExtraSettings (code).DecrementUserCount ();
					Internal.CharMarker.SetExtraIndex (ref code, 0);
				}
				
//-				core_settings.DecrementUserCount ();
				
				Internal.CharMarker.SetCoreIndex (ref code, 0);
			}
			
			Debug.Assert.IsFalse (Internal.CharMarker.HasCoreOrSettings (code));
		}
		
		
		public void Serialize(System.Text.StringBuilder buffer)
		{
			int n = this.cores == null ? 0 : this.cores.Count;
			
			buffer.Append (SerializerSupport.SerializeInt (n));
			
			for (int i = 0; i < n; i++)
			{
				Styles.CoreSettings core_settings = this.cores[i] as Styles.CoreSettings;
				
				buffer.Append ("/");
				buffer.Append (core_settings == null ? "0" : "1");
				
				if (core_settings != null)
				{
					buffer.Append ("/");
					core_settings.Serialize (buffer);
				}
			}
			
			buffer.Append ("/");
			buffer.Append ("\n");
		}
		
		public void Deserialize(TextContext context, int version, string[] args, ref int offset)
		{
			int count = SerializerSupport.DeserializeInt (args[offset++]);
			
			Debug.Assert.IsTrue (version == TextContext.SerializationVersion);
			
			if (count > 0)
			{
				this.cores = new System.Collections.ArrayList ();
				
				for (int i = 0; i < count; i++)
				{
					string test = args[offset++];
					
					if (test == "0")
					{
						this.cores.Add (null);
					}
					else if (test == "1")
					{
						Styles.CoreSettings core_settings = new Styles.CoreSettings ();
						core_settings.Deserialize (context, version, args, ref offset);
						this.cores.Add (core_settings);
					}
					else
					{
						throw new System.InvalidOperationException ("Deserialization problem");
					}
				}
			}
			
			System.Diagnostics.Debug.Assert (args[offset] == "\n");
			
			offset++;
		}
		
		
		public Styles.CoreSettings GetCoreFromIndex(int index)
		{
			if (index == 0)
			{
				return null;
			}
			
			return this.cores[index-1] as Styles.CoreSettings;
		}
		
		public Styles.CoreSettings GetCore(ulong code)
		{
			return this.GetCoreFromIndex (Internal.CharMarker.GetCoreIndex (code));
		}
		
		public Styles.LocalSettings GetLocalSettings(ulong code)
		{
			if (Internal.CharMarker.HasSettings (code))
			{
				Styles.CoreSettings core_settings = this.GetCore (code);
			
				return (core_settings == null) ? null : core_settings.GetLocalSettings (code);
			}
			
			return null;
		}
		
		public Styles.ExtraSettings GetExtraSettings(ulong code)
		{
			if (Internal.CharMarker.HasSettings (code))
			{
				Styles.CoreSettings core_settings = this.GetCore (code);
				
				return (core_settings == null) ? null : core_settings.GetExtraSettings (code);
			}
			
			return null;
		}
		
		public void GetCoreAndSettings(ulong code, out Styles.CoreSettings core_settings, out Styles.LocalSettings local_settings, out Styles.ExtraSettings extra_settings)
		{
			core_settings  = this.GetCoreFromIndex (Internal.CharMarker.GetCoreIndex (code));
			
			local_settings = (Internal.CharMarker.HasSettings (code) && (core_settings != null)) ? core_settings.GetLocalSettings (code) : null;
			extra_settings = (Internal.CharMarker.HasSettings (code) && (core_settings != null)) ? core_settings.GetExtraSettings (code) : null;
		}
		
		
		public Styles.CoreSettings FindCore(Styles.CoreSettings core_settings, CoreMatcher matcher)
		{
			//	Cherche si un core_settings identique existe déjà. Si oui, retourne la
			//	référence au core_settings en question; si non, retourne null.
			
			if ((this.cores == null) ||
				(this.cores.Count == 0))
			{
				return null;
			}
			
			foreach (Styles.CoreSettings find in this.cores)
			{
				if (Styles.CoreSettings.CompareEqual (find, core_settings))
				{
					if ((matcher == null) ||
						(matcher (find)))
					{
						return find;
					}
				}
			}
			
			return null;
		}
		
		
		private void Add(Styles.CoreSettings core_settings)
		{
			//	Ajoute le core_settings (qui ne doit pas encore être contenu dans la
			//	liste). Ceci n'affecte nullement le compteur d'utilisations.
			
			Debug.Assert.IsTrue (core_settings.CoreIndex == 0);
			Debug.Assert.IsTrue (core_settings.UserCount == 0);
			
			if (this.cores == null)
			{
				this.cores = new System.Collections.ArrayList ();
			}
			
			for (int i = 0; i < this.cores.Count; i++)
			{
				if (this.cores[i] == null)
				{
					this.cores[i] = core_settings;
					core_settings.CoreIndex = i+1;
					return;
				}
			}
			
			core_settings.CoreIndex = this.cores.Add (core_settings) + 1;
		}
		
		private void Remove(Styles.CoreSettings core_settings)
		{
			//	Supprime le core_settings. Le core_settings doit exister dans la liste.
			//	Ceci n'affecte nullement le compteur d'utilisations.
			
			Debug.Assert.IsTrue (core_settings.CoreIndex > 0);
			Debug.Assert.IsTrue (this.cores[core_settings.CoreIndex-1] == core_settings);
			
			//	Retire de la liste, sans pour autant réorganiser la liste
			//	elle-même :
			
			this.cores[core_settings.CoreIndex-1] = null;
			
			core_settings.CoreIndex = 0;
		}
		
		
		#region SettingsCoreMatcher Class
		private class SettingsCoreMatcher
		{
			public SettingsCoreMatcher(Styles.LocalSettings local_settings, Styles.ExtraSettings extra_settings)
			{
				this.local_settings = local_settings;
				this.extra_settings = extra_settings;
			}
			
			
			public bool							WasCalled
			{
				get
				{
					return this.counter > 0;
				}
			}
			
			
			public void ResetCounter()
			{
				this.counter = 0;
			}
			
			
			public bool FindExactSettings(Styles.CoreSettings core_settings)
			{
				this.counter++;
				
				if (this.local_settings != null)
				{
					if (core_settings.FindSettings (this.local_settings) == 0)
					{
						return false;
					}
				}
				
				if (this.extra_settings != null)
				{
					if (core_settings.FindSettings (this.extra_settings) == 0)
					{
						return false;
					}
				}
				
				return true;
			}
			
			public bool FindFreeSettings(Styles.CoreSettings core_settings)
			{
				this.counter++;
				
				if (this.local_settings != null)
				{
					if (core_settings.FindSettings (this.local_settings) == 0)
					{
						return core_settings.CountLocalSettings < Styles.BaseSettings.MaxSettingsCount;
					}
				}
				
				if (this.extra_settings != null)
				{
					if (core_settings.FindSettings (this.extra_settings) == 0)
					{
						return core_settings.CountExtraSettings < Styles.BaseSettings.MaxSettingsCount;
					}
				}
				
				return true;
			}
			
			
			private Styles.LocalSettings		local_settings;
			private Styles.ExtraSettings		extra_settings;
			private int							counter;
		}
		#endregion
		
		public delegate bool CoreMatcher(Styles.CoreSettings core_settings);
		
		private System.Collections.ArrayList	cores;
	}
}
