//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
			this.font_styles      = null;
			this.special_styles   = null;
		}
		
		
		public void Attach(ref ulong code, Styles.BaseStyle style)
		{
			//	Attache un style au caractère passé en entrée.
			
			//	Prend note que le style a une utilisation de plus (si un style iden-
			//	tique existe déjà, c'est ce style là qui sera utilisé).
			
			Styles.BaseStyle find = this.FindStyle (style, null);
			
			if (find == null)
			{
				this.Add (style);
				find = style;
			}
			
			find.IncrementUserCount ();
			
			Internal.CharMarker.SetStyleIndex (ref code, find.StyleIndex);
			Internal.CharMarker.SetLocalIndex (ref code, 0);
			Internal.CharMarker.SetExtraIndex (ref code, 0);
			Internal.CharMarker.SetSpecialStyleFlag (ref code, find.IsSpecialStyle);
		}
		
		public void Attach(ref ulong code, Styles.BaseStyle style, Styles.LocalSettings local_settings, Styles.ExtraSettings extra_settings)
		{
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
				Styles.BaseStyle     find    = this.FindStyle (style, new StyleMatcher (matcher.FindExactSettings));
				
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
				
				//	Ajoute les réglages au style s'ils n'en font pas encore partie :
				
				if (local_settings != null)
				{
					local_settings = find.Attach (local_settings);
					Debug.Assert.IsNotNull (local_settings);
				}
				if (extra_settings != null)
				{
					extra_settings = find.Attach (extra_settings);
					Debug.Assert.IsNotNull (extra_settings);
				}
				
				find.IncrementUserCount ();
				
				Internal.CharMarker.SetStyleIndex (ref code, find.StyleIndex);
				Internal.CharMarker.SetLocalIndex (ref code, local_settings == null ? 0 : local_settings.SettingsIndex);
				Internal.CharMarker.SetExtraIndex (ref code, extra_settings == null ? 0 : extra_settings.SettingsIndex);
				Internal.CharMarker.SetSpecialStyleFlag (ref code, find.IsSpecialStyle);
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
		
		private Styles.BaseStyle FindStyle(Styles.BaseStyle style, StyleMatcher matcher)
		{
			//	Cherche si un style identique existe déjà. Si oui, retourne la
			//	référence au style en question; si non, retourne null.
			
			if (style.IsSpecialStyle)
			{
				return this.FindSpecialStyle (style, matcher);
			}
			else
			{
				return this.FindFontStyle (style, matcher);
			}
		}
		
		private Styles.FontStyle FindFontStyle(Styles.BaseStyle style, StyleMatcher matcher)
		{
			if ((this.font_styles == null) ||
				(this.font_styles.Count == 0))
			{
				return null;
			}
			
			foreach (Styles.FontStyle find in this.font_styles)
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
		
		private Styles.SpecialStyle FindSpecialStyle(Styles.BaseStyle style, StyleMatcher matcher)
		{
			if ((this.special_styles == null) ||
				(this.special_styles.Count == 0))
			{
				return null;
			}
			
			foreach (Styles.SpecialStyle find in this.special_styles)
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
			//	Ajoute le style (qui ne doit pas encore être contenu dans la
			//	liste). Ceci n'affecte nullement le compteur d'utilisations.
			
			if (style.IsSpecialStyle)
			{
				Debug.Assert.IsTrue (style.StyleIndex == 0);
				Debug.Assert.IsTrue (style.CountUsers == 0);
				
				for (int i = 0; i < special_styles.Count; i++)
				{
					if (this.special_styles[i] == null)
					{
						this.special_styles[i] = style;
						style.StyleIndex = i+1;
						return;
					}
				}
				
				style.StyleIndex = this.special_styles.Add (style) + 1;
			}
			else
			{
				Debug.Assert.IsTrue (style.StyleIndex == 0);
				Debug.Assert.IsTrue (style.CountUsers == 0);
				
				for (int i = 0; i < font_styles.Count; i++)
				{
					if (this.font_styles[i] == null)
					{
						this.font_styles[i] = style;
						style.StyleIndex = i+1;
						return;
					}
				}
				
				style.StyleIndex = this.font_styles.Add (style) + 1;
			}
		}
		
		private void Remove(Styles.BaseStyle style)
		{
			//	Supprime le style. Le style doit exister dans la liste.
			//	Ceci n'affecte nullement le compteur d'utilisations.
			
			if (style.IsSpecialStyle)
			{
				Debug.Assert.IsTrue (style.StyleIndex != 0);
				Debug.Assert.IsTrue (this.special_styles[style.StyleIndex-1] == style);
				
				//	Retire de la liste, sans pour autant réorganiser la liste
				//	elle-même :
				
				this.special_styles[style.StyleIndex-1] = null;
				
				style.StyleIndex = 0;
			}
			else
			{
				Debug.Assert.IsTrue (style.StyleIndex != 0);
				Debug.Assert.IsTrue (this.font_styles[style.StyleIndex-1] == style);

				//	Retire de la liste, sans pour autant réorganiser la liste
				//	elle-même :
				
				this.font_styles[style.StyleIndex-1] = null;
				
				style.StyleIndex = 0;
			}
		}
		
		
		private Styles.BaseStyle FindStyleForLocalSettings(Styles.BaseStyle style, Styles.LocalSettings settings)
		{
			return null;
		}
		
		
		private delegate bool StyleMatcher(Styles.BaseStyle style);
		
		private System.Collections.ArrayList	font_styles;
		private System.Collections.ArrayList	special_styles;
	}
}
