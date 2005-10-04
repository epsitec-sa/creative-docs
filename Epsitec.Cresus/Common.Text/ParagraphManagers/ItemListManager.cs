//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
			
			Properties.AutoTextProperty  auto_text_prop = new Properties.AutoTextProperty (this.Name);
			Properties.GeneratorProperty generator_prop = new Properties.GeneratorProperty (p.Generator.Name, 0);
			Properties.TabProperty       tab_item_prop  = p.TabItem;
			Properties.TabProperty       tab_body_prop  = p.TabBody;
			
			Property[] props_1 = new Property[] { auto_text_prop, tab_item_prop };
			Property[] props_2 = new Property[] { auto_text_prop, generator_prop };
			Property[] props_3 = new Property[] { auto_text_prop, tab_body_prop };
			
			Internal.Navigator.Insert (story, cursor, Unicode.Code.HorizontalTab, null, props_1);
			Internal.Navigator.Insert (story, cursor, "X", null, props_2);
			Internal.Navigator.Insert (story, cursor, Unicode.Code.HorizontalTab, null, props_3);
			
			p.Generator.UpdateAllFields (story, context.Culture);
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
			
			Property[] flattened_properties;
			
			Internal.Navigator.GetFlattenedProperties (story, cursor, 0, out flattened_properties);
			
			System.Diagnostics.Debug.Assert (flattened_properties != null);
			
			Properties.AutoTextProperty auto_text_prop = Properties.AutoTextProperty.Find (flattened_properties, this.Name);
			
			int length = Internal.Navigator.GetRunEndLength (story, cursor, auto_text_prop);
			
			story.DeleteText (cursor, length);
			
			p.Generator.UpdateAllFields (story, context.Culture);
		}
		
		
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
					return this.tab_item;
				}
				set
				{
					this.tab_item = value;
				}
			}
			
			public Properties.TabProperty		TabBody
			{
				get
				{
					return this.tab_body;
				}
				set
				{
					this.tab_body = value;
				}
			}
			
			
			public void Load(TextContext context, string[] parameters)
			{
				System.Diagnostics.Debug.Assert (parameters.Length == 3);
				
				this.generator = context.GeneratorList[parameters[0]];
				this.tab_item  = new Properties.TabProperty ();
				this.tab_body  = new Properties.TabProperty ();
				
				this.tab_item.DeserializeFromText (context, parameters[1]);
				this.tab_body.DeserializeFromText (context, parameters[2]);
			}
			
			public string[] Save()
			{
				string[] parameters = new string[3];
				
				parameters[0] = this.generator.Name;
				parameters[1] = this.tab_item.SerializeToText ();
				parameters[2] = this.tab_body.SerializeToText ();
				
				return parameters;
			}
			
			
			private Generator					generator;
			private Properties.TabProperty		tab_item;
			private Properties.TabProperty		tab_body;
		}
	}
}
