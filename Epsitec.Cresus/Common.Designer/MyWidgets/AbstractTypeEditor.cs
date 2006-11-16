using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

namespace Epsitec.Common.Designer.MyWidgets
{
	/// <summary>
	/// Widget permettant d'éditer un Caption.Type.
	/// </summary>
	public abstract class AbstractTypeEditor : AbstractGroup
	{
		public AbstractTypeEditor()
		{
		}
		
		public AbstractTypeEditor(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
			}
			
			base.Dispose(disposing);
		}


		public static AbstractTypeEditor Create(ResourceAccess.TypeType typeType)
		{
			//	Crée le bon widget AbstractTypeEditor pour éditer un type.
			switch (typeType)
			{
				case ResourceAccess.TypeType.Integer:      return new TypeEditorNumeric();
				case ResourceAccess.TypeType.LongInteger:  return new TypeEditorNumeric();
				case ResourceAccess.TypeType.Double:       return new TypeEditorNumeric();
				case ResourceAccess.TypeType.Decimal:      return new TypeEditorNumeric();
				case ResourceAccess.TypeType.String:       return new TypeEditorString();
				case ResourceAccess.TypeType.Enum:         return new TypeEditorEnum();
				case ResourceAccess.TypeType.Structured:   return new TypeEditorStructured();
				//?case ResourceAccess.TypeType.Collection:   return new TypeEditorCollection();
				case ResourceAccess.TypeType.Date:         return new TypeEditorDateTime();
				case ResourceAccess.TypeType.Time:         return new TypeEditorDateTime();
				case ResourceAccess.TypeType.DateTime:     return new TypeEditorDateTime();
				case ResourceAccess.TypeType.Binary:       return new TypeEditorBinary();
			}

			return null;
		}


		public Module Module
		{
			get
			{
				return this.module;
			}
			set
			{
				this.module = value;
			}
		}

		public MainWindow MainWindow
		{
			get
			{
				return this.mainWindow;
			}
			set
			{
				this.mainWindow = value;
			}
		}

		public ResourceAccess ResourceAccess
		{
			get
			{
				return this.resourceAccess;
			}
			set
			{
				this.resourceAccess = value;
			}
		}

		public int ResourceSelected
		{
			get
			{
				return this.resourceSelected;
			}
			set
			{
				if (this.resourceSelected != value)
				{
					this.resourceSelected = value;
					this.UpdateContent();
				}
			}
		}

		public virtual string GetSummary()
		{
			//	Retourne le texte du résumé.
			return "";
		}


		public void ClearCache()
		{
			//	Force une nouvelle mise à jour lors du prochain Update.
			this.resourceSelected = -1;
		}

		protected virtual void UpdateContent()
		{
			//	Met à jour le contenu de l'éditeur.
		}

		protected AbstractType AbstractType
		{
			//	Retourne les définitions du type, en fonction de l'index de la ressource
			//	sélectionnée.
			get
			{
				if (this.resourceSelected != -1)
				{
					ResourceAccess.Field field = this.resourceAccess.GetField(this.resourceSelected, null, ResourceAccess.FieldType.AbstractType);
					if (field != null)
					{
						return field.AbstractType;
					}
				}

				return null;
			}
		}


		#region Super widgets
		protected void CreateStringLabeled(string label, Widget parent, out Widget group, out TextField field)
		{
			//	Crée un super-widget permettant d'éditer une chaîne, avec une étiquette à gauche.
			group = new Widget(parent);
			group.TabIndex = this.tabIndex++;
			group.TabNavigation = TabNavigationMode.ForwardTabPassive;

			StaticText text = new StaticText(group);
			text.Text = label;
			text.ContentAlignment = ContentAlignment.MiddleRight;
			text.PreferredWidth = 160;
			text.Dock = DockStyle.Left;
			text.Margins = new Margins(0, 8, 0, 0);

			field = new TextField(group);
			field.PreferredWidth = 130;
			field.Dock = DockStyle.Left;
			field.TabIndex = 0;
			field.TabNavigation = TabNavigationMode.ActivateOnTab;
		}

		protected void CreateComboLabeled(string label, Widget parent, out Widget group, out TextFieldCombo field)
		{
			//	Crée un super-widget permettant d'éditer une chaîne, avec une étiquette à gauche.
			group = new Widget(parent);
			group.TabIndex = this.tabIndex++;
			group.TabNavigation = TabNavigationMode.ForwardTabPassive;

			StaticText text = new StaticText(group);
			text.Text = label;
			text.ContentAlignment = ContentAlignment.MiddleRight;
			text.PreferredWidth = 160;
			text.Dock = DockStyle.Left;
			text.Margins = new Margins(0, 8, 0, 0);

			field = new TextFieldCombo(group);
			field.IsReadOnly = true;
			field.PreferredWidth = 130;
			field.Dock = DockStyle.Left;
			field.TabIndex = 0;
			field.TabNavigation = TabNavigationMode.ActivateOnTab;
		}

		protected void CreateDecimalLabeled(string label, Widget parent, out Widget group, out TextField field)
		{
			//	Crée un super-widget permettant d'éditer une valeur décimale, avec une étiquette à gauche.
			group = new Widget(parent);
			group.TabIndex = this.tabIndex++;
			group.TabNavigation = TabNavigationMode.ForwardTabPassive;

			StaticText text = new StaticText(group);
			text.Text = label;
			text.ContentAlignment = ContentAlignment.MiddleRight;
			text.PreferredWidth = 160;
			text.Dock = DockStyle.Left;
			text.Margins = new Margins(0, 8, 0, 0);

			field = new TextField(group);
			field.PreferredWidth = 130;
			field.Dock = DockStyle.Left;
			field.TabIndex = 0;
			field.TabNavigation = TabNavigationMode.ActivateOnTab;
		}

		protected decimal GetDecimal(TextField field)
		{
			decimal value;
			if (!decimal.TryParse(field.Text, out value))
			{
				return 0;
			}
			return value;
		}

		protected void SetDecimal(TextField field, decimal value)
		{
			field.Text = value.ToString();
		}

		protected IconButton CreateIconButton()
		{
			IconButton button = new IconButton();

			return button;
		}
		#endregion


		#region Events
		protected virtual void OnContentChanged()
		{
			EventHandler handler = (EventHandler) this.GetUserEventHandler("ContentChanged");
			if (handler != null)
			{
				handler(this);
			}
		}

		public event EventHandler ContentChanged
		{
			add
			{
				this.AddUserEventHandler("ContentChanged", value);
			}
			remove
			{
				this.RemoveUserEventHandler("ContentChanged", value);
			}
		}
		#endregion


		protected ResourceAccess				resourceAccess;
		protected int							resourceSelected = -1;
		protected Module						module;
		protected MainWindow					mainWindow;
		protected int							tabIndex = 0;
		protected bool							ignoreChange = false;
	}
}
