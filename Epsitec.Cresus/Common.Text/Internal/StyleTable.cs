//	Copyright � 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Internal
{
	/// <summary>
	/// Summary description for StyleTable.
	/// </summary>
	internal sealed class StyleTable
	{
		public StyleTable()
		{
			this.simple_styles = null;
			this.rich_styles   = null;
		}
		
		
		public void Attach(ref ulong code, Styles.BaseStyle style)
		{
			//	Attache un style au caract�re pass� en entr�e.
			
			//	Prend note que le style a une utilisation de plus (si un style iden-
			//	tique existe d�j�, c'est ce style l� qui sera r�utilis�; on �vite
			//	ainsi les doublons).
			
			Styles.BaseStyle find = this.FindStyle (style, null);
			
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
			Internal.CharMarker.SetRichStyleFlag (ref code, find.IsRichStyle);
		}
		
		public void Attach(ref ulong code, Styles.BaseStyle style, Styles.LocalSettings local_settings, Styles.ExtraSettings extra_settings)
		{
			//	Variante de Attach avec r�glages sp�cifiques (voir m�thode simple
			//	ci-dessus).
			
			if ((local_settings == null) &&
				(extra_settings == null))
			{
				this.Attach (ref code, style);
			}
			else
			{
				//	Trouve un style qui abrite � la fois les r�glages locaux et
				//	les r�glages suppl�mentaires :
				
				SettingsStyleMatcher matcher = new SettingsStyleMatcher (local_settings, extra_settings);
				Styles.BaseStyle     find    = this.FindStyle (style, new StyleMatcher (matcher.FindExactSettings));
				
				if ((find == null) &&
					(matcher.WasCalled))
				{
					//	On avait trouv� un style, mais il ne faisait pas l'affaire,
					//	car il n'avait pas les r�glages demand�s. Il faut donc voir
					//	si parmi les candidats il y a un style avec de la place
					//	pour y placer les r�glages demand�s :
					
					find = this.FindStyle (style, new StyleMatcher (matcher.FindFreeSettings));
				}
				
				if (find == null)
				{
					//	Aucun style ne fait l'affaire (soit parce qu'il n'y en a
					//	pas, soit parce que ceux qui existent sont d�j� complets
					//	en ce qui concerne les r�glages).
					
					//	On en ajoute un neuf :
					
					this.Add (style);
					find = style;
				}
				
				Debug.Assert.IsTrue (find.StyleIndex > 0);
				Debug.Assert.IsFalse (Internal.CharMarker.HasStyleOrSettings (code));
				
				//	Ajoute les r�glages au style s'ils n'en font pas encore partie :
				
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
				Internal.CharMarker.SetRichStyleFlag (ref code, find.IsRichStyle);
			}
		}
		
		
		public void Detach(ref ulong code)
		{
			//	D�tache le style et les r�glages associ�s au caract�re 'code'.
			//	Ceci d�cr�mente les divers compteurs d'utilisation.
			
			bool rich = Internal.CharMarker.HasRichStyleFlag (code);
			
			int style_index = Internal.CharMarker.GetStyleIndex (code);
			int extra_index = Internal.CharMarker.GetExtraIndex (code);
			int local_index = Internal.CharMarker.GetLocalIndex (code);
			
			if (style_index == 0)
			{
				Debug.Assert.IsTrue (rich == false);
				Debug.Assert.IsTrue (extra_index == 0);
				Debug.Assert.IsTrue (local_index == 0);
			}
			else
			{
				Styles.BaseStyle style = (rich ? this.rich_styles[style_index-1] : this.simple_styles[style_index-1]) as Styles.SimpleStyle;
				
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
				Internal.CharMarker.SetRichStyleFlag (ref code, false);
			}
			
			Debug.Assert.IsFalse (Internal.CharMarker.HasStyleOrSettings (code));
		}
		
		
		public bool UpdateSimpleStyles()
		{
			//	Met � jour tous les styles simples (passe en revue tous les
			//	styles et ne r�g�n�re que le minimum qui a chang�).
			
			bool changed = false;
			
			foreach (Styles.SimpleStyle style in this.simple_styles)
			{
				changed |= style.Update ();
			}
			
			return changed;
		}
		
		
		public Styles.BaseStyle GetStyle(ulong code)
		{
			int index = Internal.CharMarker.GetStyleIndex (code);
			
			if (index == 0)
			{
				return null;
			}
			
			if (Internal.CharMarker.HasRichStyleFlag (code))
			{
				return this.rich_styles[index-1] as Styles.BaseStyle;
			}
			else
			{
				return this.simple_styles[index-1] as Styles.BaseStyle;
			}
		}
		
		public Styles.LocalSettings GetLocalSettings(ulong code)
		{
			if (Internal.CharMarker.HasSettings (code))
			{
				Styles.BaseStyle style = this.GetStyle (code);
			
				return (style == null) ? null : style.GetLocalSettings (code);
			}
			
			return null;
		}
		
		public Styles.ExtraSettings GetExtraSettings(ulong code)
		{
			if (Internal.CharMarker.HasSettings (code))
			{
				Styles.BaseStyle style = this.GetStyle (code);
				
				return (style == null) ? null : style.GetExtraSettings (code);
			}
			
			return null;
		}
		
		
		public Styles.BaseStyle FindStyle(Styles.BaseStyle style, StyleMatcher matcher)
		{
			//	Cherche si un style identique existe d�j�. Si oui, retourne la
			//	r�f�rence au style en question; si non, retourne null.
			
			if (style.IsRichStyle)
			{
				return this.FindRichStyle (style, matcher);
			}
			else
			{
				return this.FindSimpleStyle (style, matcher);
			}
		}
		
		
		private Styles.SimpleStyle FindSimpleStyle(Styles.BaseStyle style, StyleMatcher matcher)
		{
			if ((this.simple_styles == null) ||
				(this.simple_styles.Count == 0))
			{
				return null;
			}
			
			foreach (Styles.SimpleStyle find in this.simple_styles)
			{
				if (Styles.BaseStyle.CompareEqual (find, style))
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
		
		private Styles.RichStyle FindRichStyle(Styles.BaseStyle style, StyleMatcher matcher)
		{
			if ((this.rich_styles == null) ||
				(this.rich_styles.Count == 0))
			{
				return null;
			}
			
			foreach (Styles.RichStyle find in this.rich_styles)
			{
				if (Styles.BaseStyle.CompareEqual (find, style))
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
		
		
		private void Add(Styles.BaseStyle style)
		{
			//	Ajoute le style (qui ne doit pas encore �tre contenu dans la
			//	liste). Ceci n'affecte nullement le compteur d'utilisations.
			
			if (style.IsRichStyle)
			{
				Debug.Assert.IsTrue (style.StyleIndex == 0);
				Debug.Assert.IsTrue (style.CountUsers == 0);
				
				for (int i = 0; i < rich_styles.Count; i++)
				{
					if (this.rich_styles[i] == null)
					{
						this.rich_styles[i] = style;
						style.StyleIndex = i+1;
						return;
					}
				}
				
				style.StyleIndex = this.rich_styles.Add (style) + 1;
			}
			else
			{
				Debug.Assert.IsTrue (style.StyleIndex == 0);
				Debug.Assert.IsTrue (style.CountUsers == 0);
				
				for (int i = 0; i < simple_styles.Count; i++)
				{
					if (this.simple_styles[i] == null)
					{
						this.simple_styles[i] = style;
						style.StyleIndex = i+1;
						return;
					}
				}
				
				style.StyleIndex = this.simple_styles.Add (style) + 1;
			}
		}
		
		private void Remove(Styles.BaseStyle style)
		{
			//	Supprime le style. Le style doit exister dans la liste.
			//	Ceci n'affecte nullement le compteur d'utilisations.
			
			if (style.IsRichStyle)
			{
				Debug.Assert.IsTrue (style.StyleIndex > 0);
				Debug.Assert.IsTrue (this.rich_styles[style.StyleIndex-1] == style);
				
				//	Retire de la liste, sans pour autant r�organiser la liste
				//	elle-m�me :
				
				this.rich_styles[style.StyleIndex-1] = null;
				
				style.StyleIndex = 0;
			}
			else
			{
				Debug.Assert.IsTrue (style.StyleIndex > 0);
				Debug.Assert.IsTrue (this.simple_styles[style.StyleIndex-1] == style);

				//	Retire de la liste, sans pour autant r�organiser la liste
				//	elle-m�me :
				
				this.simple_styles[style.StyleIndex-1] = null;
				
				style.StyleIndex = 0;
			}
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
			
			
			public bool FindExactSettings(Styles.BaseStyle style)
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
			
			public bool FindFreeSettings(Styles.BaseStyle style)
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
		
		public delegate bool StyleMatcher(Styles.BaseStyle style);
		
		private System.Collections.ArrayList	simple_styles;
		private System.Collections.ArrayList	rich_styles;
	}
}
