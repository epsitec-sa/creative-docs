//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Internal
{
	/// <summary>
	/// La classe StyleTable permet d'accéder aux styles et aux réglages d'après
	/// le code d'un caractère. De manière interne, StyleTable gère deux listes
	/// avec les styles simples et les styles riches.
	/// </summary>
	internal sealed class StyleTable
	{
		public StyleTable()
		{
			this.styles = null;
		}
		
		
		public void Attach(ref ulong code, Styles.SimpleStyle style)
		{
			//	Attache un style au caractère passé en entrée.
			
			//	Prend note que le style a une utilisation de plus (si un style iden-
			//	tique existe déjà, c'est ce style là qui sera réutilisé; on évite
			//	ainsi les doublons).
			
			Styles.SimpleStyle find = this.FindStyle (style, null);
			
			if (find == null)
			{
				this.Add (style);
				find = style;
			}
			
			Debug.Assert.IsFalse (Internal.CharMarker.HasStyleOrSettings (code));
			Debug.Assert.IsTrue (find.StyleIndex > 0);
			
			find.IncrementUserCount ();
			
			Internal.CharMarker.SetStyleIndex (ref code, find.StyleIndex);
			Internal.CharMarker.SetLocalIndex (ref code, 0);
			Internal.CharMarker.SetExtraIndex (ref code, 0);
		}
		
		public void Attach(ref ulong code, Styles.SimpleStyle style, Styles.LocalSettings local_settings, Styles.ExtraSettings extra_settings)
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
				this.Attach (ref code, style);
			}
			else
			{
				//	Trouve un style qui abrite à la fois les réglages locaux et
				//	les réglages supplémentaires :
				
				SettingsStyleMatcher matcher = new SettingsStyleMatcher (local_settings, extra_settings);
				Styles.SimpleStyle   find    = this.FindStyle (style, new StyleMatcher (matcher.FindExactSettings));
				
				if ((find == null) &&
					(matcher.WasCalled))
				{
					//	On avait trouvé un style, mais il ne faisait pas l'affaire,
					//	car il n'avait pas les réglages demandés. Il faut donc voir
					//	si parmi les candidats il y a un style avec de la place
					//	pour y placer les réglages demandés :
					
					find = this.FindStyle (style, new StyleMatcher (matcher.FindFreeSettings));
				}
				
				if (find == null)
				{
					//	Aucun style ne fait l'affaire (soit parce qu'il n'y en a
					//	pas, soit parce que ceux qui existent sont déjà complets
					//	en ce qui concerne les réglages).
					
					//	On en ajoute un neuf :
					
					this.Add (style);
					find = style;
				}
				
				Debug.Assert.IsTrue (find.StyleIndex > 0);
				Debug.Assert.IsFalse (Internal.CharMarker.HasStyleOrSettings (code));
				
				//	Ajoute les réglages au style s'ils n'en font pas encore partie :
				
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
				
				find.IncrementUserCount ();
				
				Internal.CharMarker.SetStyleIndex (ref code, find.StyleIndex);
				Internal.CharMarker.SetLocalIndex (ref code, local_settings == null ? 0 : local_settings.SettingsIndex);
				Internal.CharMarker.SetExtraIndex (ref code, extra_settings == null ? 0 : extra_settings.SettingsIndex);
			}
		}
		
		
		public void Detach(ref ulong code)
		{
			//	Détache le style et les réglages associés au caractère 'code'.
			//	Ceci décrémente les divers compteurs d'utilisation.
			
			int style_index = Internal.CharMarker.GetStyleIndex (code);
			int extra_index = Internal.CharMarker.GetExtraIndex (code);
			int local_index = Internal.CharMarker.GetLocalIndex (code);
			
			if (style_index == 0)
			{
				Debug.Assert.IsTrue (extra_index == 0);
				Debug.Assert.IsTrue (local_index == 0);
			}
			else
			{
				Styles.SimpleStyle style = this.styles[style_index-1] as Styles.SimpleStyle;
				
				Debug.Assert.IsNotNull (style);
				
				if (local_index != 0)
				{
					style.GetLocalSettings (code).DecrementUserCount ();
					Internal.CharMarker.SetLocalIndex (ref code, 0);
				}
				if (extra_index != 0)
				{
					style.GetExtraSettings (code).DecrementUserCount ();
					Internal.CharMarker.SetExtraIndex (ref code, 0);
				}
				
				style.DecrementUserCount ();
				
				Internal.CharMarker.SetStyleIndex (ref code, 0);
			}
			
			Debug.Assert.IsFalse (Internal.CharMarker.HasStyleOrSettings (code));
		}
		
		
		public bool UpdateSimpleStyles()
		{
			//	Met à jour tous les styles simples (passe en revue tous les
			//	styles et ne régénère que le minimum qui a changé).
			
			bool changed = false;
			
			foreach (Styles.SimpleStyle style in this.styles)
			{
				if (style != null)
				{
					if (style.Update ())
					{
						changed = true;
					}
				}
			}
			
			return changed;
		}
		
		
		public void Serialize(System.Text.StringBuilder buffer)
		{
			int n = this.styles == null ? 0 : this.styles.Count;
			
			buffer.Append (SerializerSupport.SerializeInt (n));
			
			for (int i = 0; i < n; i++)
			{
				Styles.SimpleStyle style = this.styles[i] as Styles.SimpleStyle;
				
				buffer.Append ("/");
				buffer.Append (style == null ? "0" : "1");
				
				if (style != null)
				{
					buffer.Append ("/");
					style.Serialize (buffer);
				}
			}
			
			buffer.Append ("/");
			buffer.Append ("\n");
		}
		
		public void Deserialize(TextContext context, int version, string[] args, ref int offset)
		{
			int count = SerializerSupport.DeserializeInt (args[offset++]);
			
			Debug.Assert.IsTrue (version == 1);
			
			if (count > 0)
			{
				this.styles = new System.Collections.ArrayList ();
				
				for (int i = 0; i < count; i++)
				{
					string test = args[offset++];
					
					if (test == "0")
					{
						this.styles.Add (null);
					}
					else if (test == "1")
					{
						Styles.SimpleStyle style = new Styles.SimpleStyle ();
						style.Deserialize (context, version, args, ref offset);
						this.styles.Add (style);
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
		
		
		public Styles.SimpleStyle GetStyleFromIndex(int index)
		{
			if (index == 0)
			{
				return null;
			}
			
			return this.styles[index-1] as Styles.SimpleStyle;
		}
		
		public Styles.SimpleStyle GetStyle(ulong code)
		{
			return this.GetStyleFromIndex (Internal.CharMarker.GetStyleIndex (code));
		}
		
		public Styles.LocalSettings GetLocalSettings(ulong code)
		{
			if (Internal.CharMarker.HasSettings (code))
			{
				Styles.SimpleStyle style = this.GetStyle (code);
			
				return (style == null) ? null : style.GetLocalSettings (code);
			}
			
			return null;
		}
		
		public Styles.ExtraSettings GetExtraSettings(ulong code)
		{
			if (Internal.CharMarker.HasSettings (code))
			{
				Styles.SimpleStyle style = this.GetStyle (code);
				
				return (style == null) ? null : style.GetExtraSettings (code);
			}
			
			return null;
		}
		
		
		public Styles.SimpleStyle FindStyle(Styles.SimpleStyle style, StyleMatcher matcher)
		{
			//	Cherche si un style identique existe déjà. Si oui, retourne la
			//	référence au style en question; si non, retourne null.
			
			if ((this.styles == null) ||
				(this.styles.Count == 0))
			{
				return null;
			}
			
			foreach (Styles.SimpleStyle find in this.styles)
			{
				if (Styles.SimpleStyle.CompareEqual (find, style))
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
		
		
		private void Add(Styles.SimpleStyle style)
		{
			//	Ajoute le style (qui ne doit pas encore être contenu dans la
			//	liste). Ceci n'affecte nullement le compteur d'utilisations.
			
			Debug.Assert.IsTrue (style.StyleIndex == 0);
			Debug.Assert.IsTrue (style.CountUsers == 0);
			
			if (this.styles == null)
			{
				this.styles = new System.Collections.ArrayList ();
			}
			
			StyleVersion.Default.Change ();
			
			for (int i = 0; i < this.styles.Count; i++)
			{
				if (this.styles[i] == null)
				{
					this.styles[i] = style;
					style.StyleIndex = i+1;
					return;
				}
			}
			
			style.StyleIndex = this.styles.Add (style) + 1;
		}
		
		private void Remove(Styles.SimpleStyle style)
		{
			//	Supprime le style. Le style doit exister dans la liste.
			//	Ceci n'affecte nullement le compteur d'utilisations.
			
			Debug.Assert.IsTrue (style.StyleIndex > 0);
			Debug.Assert.IsTrue (this.styles[style.StyleIndex-1] == style);
			
			StyleVersion.Default.Change ();
			
			//	Retire de la liste, sans pour autant réorganiser la liste
			//	elle-même :
			
			this.styles[style.StyleIndex-1] = null;
			
			style.StyleIndex = 0;
		}
		
		
		#region SettingsStyleMatcher Class
		private class SettingsStyleMatcher
		{
			public SettingsStyleMatcher(Styles.LocalSettings local_settings, Styles.ExtraSettings extra_settings)
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
			
			
			public bool FindExactSettings(Styles.SimpleStyle style)
			{
				this.counter++;
				
				if (this.local_settings != null)
				{
					if (style.FindSettings (this.local_settings) == 0)
					{
						return false;
					}
				}
				
				if (this.extra_settings != null)
				{
					if (style.FindSettings (this.extra_settings) == 0)
					{
						return false;
					}
				}
				
				return true;
			}
			
			public bool FindFreeSettings(Styles.SimpleStyle style)
			{
				this.counter++;
				
				if (this.local_settings != null)
				{
					if (style.FindSettings (this.local_settings) == 0)
					{
						return style.CountLocalSettings < Styles.BaseStyle.MaxSettingsCount;
					}
				}
				
				if (this.extra_settings != null)
				{
					if (style.FindSettings (this.extra_settings) == 0)
					{
						return style.CountExtraSettings < Styles.BaseStyle.MaxSettingsCount;
					}
				}
				
				return true;
			}
			
			
			private Styles.LocalSettings		local_settings;
			private Styles.ExtraSettings		extra_settings;
			private int							counter;
		}
		#endregion
		
		public delegate bool StyleMatcher(Styles.SimpleStyle style);
		
		private System.Collections.ArrayList	styles;
	}
}
