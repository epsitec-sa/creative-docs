//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.ParagraphManagers
{
	/// <summary>
	/// La classe ItemListManager offre les services nécessaires à la réalisation
	/// de listes à puces ou numérotées.
	/// 
	/// Le nom de ce IParagraphManager est "ItemList".
	/// </summary>
	public class ItemListManager : ParagraphManager
	{
		public ItemListManager()
		{
		}
		
		
		public override void AttachToParagraph(TextStory story, ICursor cursor, Properties.ManagedParagraphProperty property)
		{
			TextContext context = story.TextContext;
			
			Parameters p = new Parameters (context, property.ManagerParameters);
			
			//	Le curseur est positionné au début (brut) du paragraphe, juste
			//	après la fin du paragraphe précédent.
			
			System.Diagnostics.Debug.Assert (Internal.Navigator.IsParagraphStart (story, cursor, 0));
			System.Diagnostics.Debug.Assert (context.ContainsProperty (story, cursor, 0, property));
			
			Properties.AutoTextProperty   autoTextProp   = new Properties.AutoTextProperty (this.Name);
			Properties.GeneratorProperty  generatorProp   = new Properties.GeneratorProperty (p.Generator.Name, 0, context.GenerateUniqueId ());
			Properties.TabProperty        tabItemProp    = p.TabItem;
			Properties.TabProperty        tabBodyProp    = p.TabBody;
			Properties.FontProperty       fontProp        = p.Font;
			Properties.FontSizeProperty   fontSizeProp   = p.FontSize;
			Properties.FontOffsetProperty fontOffsetProp = p.FontOffset;
			
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			
			list.Add (autoTextProp);
			list.Add (generatorProp);
			
			if (fontProp != null)
			{
				list.Add (fontProp);
			}
			if (fontSizeProp != null)
			{
				list.Add (fontSizeProp);
			}
			if (fontOffsetProp != null)
			{
				list.Add (fontOffsetProp);
			}
			
			Property[] props;
			
			if (tabItemProp != null)
			{
				props = new Property[] { autoTextProp, tabItemProp };
				Internal.Navigator.Insert (story, cursor, Unicode.Code.HorizontalTab, null, props);
			}
			
			props = (Property[]) list.ToArray (typeof (Property));
			Internal.Navigator.Insert (story, cursor, "X", null, props);
			
			if (tabBodyProp != null)
			{
				props = new Property[] { autoTextProp, tabBodyProp };
				Internal.Navigator.Insert (story, cursor, Unicode.Code.HorizontalTab, null, props);
			}
			
			p.Generator.UpdateAllFields (story, property, context.Culture);
		}
		
		public override void DetachFromParagraph(TextStory story, ICursor cursor, Properties.ManagedParagraphProperty property)
		{
			TextContext context = story.TextContext;
			
			Parameters p = new Parameters (context, property.ManagerParameters);
			
			//	Le curseur est positionné au début (brut) du paragraphe, juste
			//	après la fin du paragraphe précédent; il est donc placé avant le
			//	morceau de texte automatique que nous gérons.
			
			System.Diagnostics.Debug.Assert (Internal.Navigator.IsParagraphStart (story, cursor, 0));
			System.Diagnostics.Debug.Assert (context.ContainsProperty (story, cursor, 0, property) == false);
			
			Property[] flattenedProperties;
			
			Internal.Navigator.GetFlattenedProperties (story, cursor, 0, out flattenedProperties);
			
			System.Diagnostics.Debug.Assert (flattenedProperties != null);
			
			Properties.AutoTextProperty autoTextProp = Properties.AutoTextProperty.Find (flattenedProperties, this.Name);
			
			int length = Internal.Navigator.GetRunEndLength (story, cursor, autoTextProp);
			
			story.DeleteText (cursor, length);
			
			p.Generator.UpdateAllFields (story, property, context.Culture);
		}
		
		public override void RefreshParagraph(TextStory story, ICursor cursor, Properties.ManagedParagraphProperty property)
		{
			TextContext context = story.TextContext;
			
			Parameters p = new Parameters (context, property.ManagerParameters);
			
			//	Le curseur est positionné au début (brut) du paragraphe, juste
			//	après la fin du paragraphe précédent.
			
			System.Diagnostics.Debug.Assert (Internal.Navigator.IsParagraphStart (story, cursor, 0));
			System.Diagnostics.Debug.Assert (context.ContainsProperty (story, cursor, 0, property));
			
			p.Generator.UpdateAllFields (story, property, context.Culture);
		}
		
		
		#region Parameters Class
		public class Parameters
		{
			public Parameters()
			{
			}
			
			public Parameters(TextContext context, params string[] parameters)
			{
				this.Load (context, parameters);
			}
			
			
			public Generator					Generator
			{
				get
				{
					return this.generator;
				}
				set
				{
					this.generator = value;
				}
			}
			
			public Properties.TabProperty		TabItem
			{
				get
				{
					return this.tabItem;
				}
				set
				{
					this.tabItem = value;
				}
			}
			
			public Properties.TabProperty		TabBody
			{
				get
				{
					return this.tabBody;
				}
				set
				{
					this.tabBody = value;
				}
			}
			
			public Properties.FontProperty		Font
			{
				get
				{
					return this.font;
				}
				set
				{
					this.font = value;
				}
			}
			
			public Properties.FontSizeProperty	FontSize
			{
				get
				{
					return this.fontSize;
				}
				set
				{
					this.fontSize = value;
				}
			}
			
			public Properties.FontOffsetProperty FontOffset
			{
				get
				{
					return this.fontOffset;
				}
				set
				{
					this.fontOffset = value;
				}
			}
			
			
			public void Load(TextContext context, string[] parameters)
			{
				System.Diagnostics.Debug.Assert ((parameters.Length == 3) || (parameters.Length == 6));
				
				this.generator = context.GeneratorList[parameters[0]];
				this.tabItem  = null;
				this.tabBody  = null;
				
				if (parameters[1] != null)
				{
					this.tabItem = new Properties.TabProperty ();
					this.tabItem.DeserializeFromText (context, parameters[1]);
				}
				if (parameters[2] != null)
				{
					this.tabBody = new Properties.TabProperty ();
					this.tabBody.DeserializeFromText (context, parameters[2]);
				}
				
				if (parameters.Length == 6)
				{
					if (parameters[3] == null)
					{
						this.font = null;
					}
					else
					{
						this.font = new Properties.FontProperty ();
						this.font.DeserializeFromText (context, parameters[3]);
					}
					
					if (parameters[4] == null)
					{
						this.fontSize = null;
					}
					else
					{
						this.fontSize = new Properties.FontSizeProperty ();
						this.fontSize.DeserializeFromText (context, parameters[4]);
					}
					
					if (parameters[5] == null)
					{
						this.fontOffset = null;
					}
					else
					{
						this.fontOffset = new Properties.FontOffsetProperty ();
						this.fontOffset.DeserializeFromText (context, parameters[5]);
					}
				}
			}
			
			public string[] Save()
			{
				string[] parameters = new string[6];
				
				parameters[0] = this.generator.Name;
				parameters[1] = this.tabItem    == null ? null : this.tabItem.SerializeToText ();
				parameters[2] = this.tabBody    == null ? null : this.tabBody.SerializeToText ();
				parameters[3] = this.font        == null ? null : this.font.SerializeToText ();
				parameters[4] = this.fontSize   == null ? null : this.fontSize.SerializeToText ();
				parameters[5] = this.fontOffset == null ? null : this.fontOffset.SerializeToText ();
				
				return parameters;
			}
			
			
			private Generator						generator;
			private Properties.TabProperty			tabItem;
			private Properties.TabProperty			tabBody;
			private Properties.FontProperty			font;
			private Properties.FontSizeProperty		fontSize;
			private Properties.FontOffsetProperty	fontOffset;
		}
		#endregion
	}
}
