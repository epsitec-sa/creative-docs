//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
		
		
		public void Attach(ref ulong code, Styles.CoreSettings coreSettings)
		{
			//	Attache un coreSettings au caractère passé en entrée.

			//	Prend note que le coreSettings a une utilisation de plus (si un
			//	coreSettings identique existe déjà, c'est ce coreSettings là qui
			//	sera réutilisé; on évite ainsi les doublons).
			
			Styles.CoreSettings find = this.FindCore (coreSettings, null);
			
			if (find == null)
			{
				this.Add (coreSettings);
				find = coreSettings;
			}
			
			Debug.Assert.IsFalse (Internal.CharMarker.HasCoreOrSettings (code));
			Debug.Assert.IsTrue (find.CoreIndex > 0);
			
//-			find.IncrementUserCount ();
			
			Internal.CharMarker.SetCoreIndex (ref code, find.CoreIndex);
			Internal.CharMarker.SetLocalIndex (ref code, 0);
			Internal.CharMarker.SetExtraIndex (ref code, 0);
		}
		
		public void Attach(ref ulong code, Styles.CoreSettings coreSettings, Styles.LocalSettings localSettings, Styles.ExtraSettings extraSettings)
		{
			//	Variante de Attach avec réglages spécifiques (voir méthode simple
			//	ci-dessus).
			
			if ((localSettings != null) &&
				(localSettings.IsEmpty))
			{
				localSettings = null;
			}
			
			if ((extraSettings != null) &&
				(extraSettings.IsEmpty))
			{
				extraSettings = null;
			}
			
			if ((localSettings == null) &&
				(extraSettings == null))
			{
				this.Attach (ref code, coreSettings);
			}
			else
			{
				//	Trouve un coreSettings qui abrite à la fois les réglages locaux et
				//	les réglages supplémentaires :
				
				SettingsCoreMatcher matcher = new SettingsCoreMatcher (localSettings, extraSettings);
				Styles.CoreSettings find    = this.FindCore (coreSettings, new CoreMatcher (matcher.FindExactSettings));
				
				if ((find == null) &&
					(matcher.WasCalled))
				{
					//	On avait trouvé un coreSettings, mais il ne faisait pas l'affaire,
					//	car il n'avait pas les réglages demandés. Il faut donc voir
					//	si parmi les candidats il y a un coreSettings avec de la place
					//	pour y placer les réglages demandés :
					
					find = this.FindCore (coreSettings, new CoreMatcher (matcher.FindFreeSettings));
				}
				
				if (find == null)
				{
					//	Aucun coreSettings ne fait l'affaire (soit parce qu'il n'y en a
					//	pas, soit parce que ceux qui existent sont déjà complets
					//	en ce qui concerne les réglages).
					
					//	On en ajoute un neuf :
					
					this.Add (coreSettings);
					find = coreSettings;
				}
				
				Debug.Assert.IsTrue (find.CoreIndex > 0);
				Debug.Assert.IsFalse (Internal.CharMarker.HasCoreOrSettings (code));

				//	Ajoute les réglages au coreSettings s'ils n'en font pas encore partie :
				
				if (localSettings != null)
				{
					localSettings = find.Attach (localSettings);
					Debug.Assert.IsNotNull (localSettings);
					Debug.Assert.IsTrue (localSettings.SettingsIndex > 0);
				}
				if (extraSettings != null)
				{
					extraSettings = find.Attach (extraSettings);
					Debug.Assert.IsNotNull (extraSettings);
					Debug.Assert.IsTrue (extraSettings.SettingsIndex > 0);
				}
				
//-				find.IncrementUserCount ();
				
				Internal.CharMarker.SetCoreIndex (ref code, find.CoreIndex);
				Internal.CharMarker.SetLocalIndex (ref code, localSettings == null ? 0 : localSettings.SettingsIndex);
				Internal.CharMarker.SetExtraIndex (ref code, extraSettings == null ? 0 : extraSettings.SettingsIndex);
			}
		}
		
		
		public void Detach(ref ulong code)
		{
			//	Détache le coreSettings et les réglages associés au caractère 'code'.
			//	Ceci décrémente les divers compteurs d'utilisation.
			
			int coreIndex  = Internal.CharMarker.GetCoreIndex (code);
			int extraIndex = Internal.CharMarker.GetExtraIndex (code);
			int localIndex = Internal.CharMarker.GetLocalIndex (code);
			
			if (coreIndex == 0)
			{
				Debug.Assert.IsTrue (extraIndex == 0);
				Debug.Assert.IsTrue (localIndex == 0);
			}
			else
			{
				Styles.CoreSettings coreSettings = this.cores[coreIndex-1] as Styles.CoreSettings;
				
				Debug.Assert.IsNotNull (coreSettings);
				
				if (localIndex != 0)
				{
//-					coreSettings.GetLocalSettings (code).DecrementUserCount ();
					Internal.CharMarker.SetLocalIndex (ref code, 0);
				}
				if (extraIndex != 0)
				{
//-					coreSettings.GetExtraSettings (code).DecrementUserCount ();
					Internal.CharMarker.SetExtraIndex (ref code, 0);
				}
				
//-				coreSettings.DecrementUserCount ();
				
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
				Styles.CoreSettings coreSettings = this.cores[i] as Styles.CoreSettings;
				
				buffer.Append ("/");
				buffer.Append (coreSettings == null ? "0" : "1");
				
				if (coreSettings != null)
				{
					buffer.Append ("/");
					coreSettings.Serialize (buffer);
				}
			}
			
			buffer.Append ("/");
			buffer.Append ("\n");
		}
		
		public void Deserialize(TextContext context, int version, string[] args, ref int offset)
		{
			int count = SerializerSupport.DeserializeInt (args[offset++]);
			
			Debug.Assert.IsTrue (version <= TextContext.SerializationVersion);
			
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
						Styles.CoreSettings coreSettings = new Styles.CoreSettings ();
						coreSettings.Deserialize (context, version, args, ref offset);
						this.cores.Add (coreSettings);
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
				Styles.CoreSettings coreSettings = this.GetCore (code);
			
				return (coreSettings == null) ? null : coreSettings.GetLocalSettings (code);
			}
			
			return null;
		}
		
		public Styles.ExtraSettings GetExtraSettings(ulong code)
		{
			if (Internal.CharMarker.HasSettings (code))
			{
				Styles.CoreSettings coreSettings = this.GetCore (code);
				
				return (coreSettings == null) ? null : coreSettings.GetExtraSettings (code);
			}
			
			return null;
		}
		
		public void GetCoreAndSettings(ulong code, out Styles.CoreSettings coreSettings, out Styles.LocalSettings localSettings, out Styles.ExtraSettings extraSettings)
		{
			coreSettings  = this.GetCoreFromIndex (Internal.CharMarker.GetCoreIndex (code));
			
			localSettings = (Internal.CharMarker.HasSettings (code) && (coreSettings != null)) ? coreSettings.GetLocalSettings (code) : null;
			extraSettings = (Internal.CharMarker.HasSettings (code) && (coreSettings != null)) ? coreSettings.GetExtraSettings (code) : null;
		}
		
		
		public Styles.CoreSettings FindCore(Styles.CoreSettings coreSettings, CoreMatcher matcher)
		{
			//	Cherche si un coreSettings identique existe déjà. Si oui, retourne la
			//	référence au coreSettings en question; si non, retourne null.
			
			if ((this.cores == null) ||
				(this.cores.Count == 0))
			{
				return null;
			}
			
			foreach (Styles.CoreSettings find in this.cores)
			{
				if (Styles.CoreSettings.CompareEqual (find, coreSettings))
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
		
		
		private void Add(Styles.CoreSettings coreSettings)
		{
			//	Ajoute le coreSettings (qui ne doit pas encore être contenu dans la
			//	liste). Ceci n'affecte nullement le compteur d'utilisations.
			
			Debug.Assert.IsTrue (coreSettings.CoreIndex == 0);
			Debug.Assert.IsTrue (coreSettings.UserCount == 0);
			
			if (this.cores == null)
			{
				this.cores = new System.Collections.ArrayList ();
			}
			
			for (int i = 0; i < this.cores.Count; i++)
			{
				if (this.cores[i] == null)
				{
					this.cores[i] = coreSettings;
					coreSettings.CoreIndex = i+1;
					return;
				}
			}
			
			coreSettings.CoreIndex = this.cores.Add (coreSettings) + 1;
		}
		
		private void Remove(Styles.CoreSettings coreSettings)
		{
			//	Supprime le coreSettings. Le coreSettings doit exister dans la liste.
			//	Ceci n'affecte nullement le compteur d'utilisations.
			
			Debug.Assert.IsTrue (coreSettings.CoreIndex > 0);
			Debug.Assert.IsTrue (this.cores[coreSettings.CoreIndex-1] == coreSettings);
			
			//	Retire de la liste, sans pour autant réorganiser la liste
			//	elle-même :
			
			this.cores[coreSettings.CoreIndex-1] = null;
			
			coreSettings.CoreIndex = 0;
		}
		
		
		#region SettingsCoreMatcher Class
		private class SettingsCoreMatcher
		{
			public SettingsCoreMatcher(Styles.LocalSettings localSettings, Styles.ExtraSettings extraSettings)
			{
				this.localSettings = localSettings;
				this.extraSettings = extraSettings;
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
			
			
			public bool FindExactSettings(Styles.CoreSettings coreSettings)
			{
				this.counter++;
				
				if (this.localSettings != null)
				{
					if (coreSettings.FindSettings (this.localSettings) == 0)
					{
						return false;
					}
				}
				
				if (this.extraSettings != null)
				{
					if (coreSettings.FindSettings (this.extraSettings) == 0)
					{
						return false;
					}
				}
				
				return true;
			}
			
			public bool FindFreeSettings(Styles.CoreSettings coreSettings)
			{
				this.counter++;
				
				if (this.localSettings != null)
				{
					if (coreSettings.FindSettings (this.localSettings) == 0)
					{
						if (coreSettings.CountLocalSettings == Styles.BaseSettings.MaxSettingsCount)
						{
							return false;
						}
					}
				}
				
				if (this.extraSettings != null)
				{
					if (coreSettings.FindSettings (this.extraSettings) == 0)
					{
						if (coreSettings.CountExtraSettings == Styles.BaseSettings.MaxSettingsCount)
						{
							return false;
						}
					}
				}
				
				return true;
			}
			
			
			private Styles.LocalSettings		localSettings;
			private Styles.ExtraSettings		extraSettings;
			private int							counter;
		}
		#endregion
		
		public delegate bool CoreMatcher(Styles.CoreSettings coreSettings);
		
		private System.Collections.ArrayList	cores;
	}
}
