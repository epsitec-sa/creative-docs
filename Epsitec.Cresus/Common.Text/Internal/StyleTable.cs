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
			this.simple_styles = null;
			this.rich_styles   = null;
		}
		
		
		public void Attach(ref ulong code, Styles.BaseStyle style)
		{
			//	Attache un style au caractère passé en entrée.
			
			//	Prend note que le style a une utilisation de plus (si un style iden-
			//	tique existe déjà, c'est ce style là qui sera réutilisé; on évite
			//	ainsi les doublons).
			
			Styles.BaseStyle find = this.FindStyle (style, null);
			
			if (find == null)
			{
				this.Add (style);
				find = style;
			}
			
			Debug.Assert.IsTrue (find.StyleIndex > 0);
			
			find.IncrementUserCount ();
			
			Internal.CharMarker.SetStyleIndex (ref code, find.StyleIndex);
			Internal.CharMarker.SetLocalIndex (ref code, 0);
			Internal.CharMarker.SetExtraIndex (ref code, 0);
			Internal.CharMarker.SetRichStyleFlag (ref code, find.IsRichStyle);
		}
		
		public void Attach(ref ulong code, Styles.BaseStyle style, Styles.LocalSettings local_settings, Styles.ExtraSettings extra_settings)
		{
			//	Variante avec réglages de Attach (voir méthode simple ci-dessus).
			
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
				
				Debug.Assert.IsTrue (find.StyleIndex > 0);
				
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
				Internal.CharMarker.SetRichStyleFlag (ref code, find.IsRichStyle);
			}
		}
		
		
		#region SettingsStyleMatcher Class
		public class SettingsStyleMatcher
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
		
		public Styles.BaseStyle FindStyle(Styles.BaseStyle style, StyleMatcher matcher)
		{
			//	Cherche si un style identique existe déjà. Si oui, retourne la
			//	référence au style en question; si non, retourne null.
			
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
			//	Ajoute le style (qui ne doit pas encore être contenu dans la
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
				
				//	Retire de la liste, sans pour autant réorganiser la liste
				//	elle-même :
				
				this.rich_styles[style.StyleIndex-1] = null;
				
				style.StyleIndex = 0;
			}
			else
			{
				Debug.Assert.IsTrue (style.StyleIndex > 0);
				Debug.Assert.IsTrue (this.simple_styles[style.StyleIndex-1] == style);

				//	Retire de la liste, sans pour autant réorganiser la liste
				//	elle-même :
				
				this.simple_styles[style.StyleIndex-1] = null;
				
				style.StyleIndex = 0;
			}
		}
		
		
		public delegate bool StyleMatcher(Styles.BaseStyle style);
		
		private System.Collections.ArrayList	simple_styles;
		private System.Collections.ArrayList	rich_styles;
	}
}
