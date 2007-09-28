using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

namespace Epsitec.Common.Designer.MyWidgets
{
	/// <summary>
	/// Widget permettant d'�diter un Caption.Type.
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


		public static AbstractTypeEditor Create(TypeCode typeCode)
		{
			//	Cr�e le bon widget AbstractTypeEditor pour �diter un type.
			switch (typeCode)
			{
				case TypeCode.Integer:      return new TypeEditorNumeric();
				case TypeCode.LongInteger:  return new TypeEditorNumeric();
				case TypeCode.Double:       return new TypeEditorNumeric();
				case TypeCode.Decimal:      return new TypeEditorNumeric();
				case TypeCode.String:       return new TypeEditorString();
				case TypeCode.Enum:         return new TypeEditorEnum();
				case TypeCode.Date:         return new TypeEditorDateTime();
				case TypeCode.Time:         return new TypeEditorDateTime();
				case TypeCode.DateTime:     return new TypeEditorDateTime();
				case TypeCode.Binary:       return new TypeEditorBinary();
				case TypeCode.Other:        return new TypeEditorNative();
			}

			return null;
		}


		public TypeCode TypeCode
		{
			get
			{
				return this.typeCode;
			}
			set
			{
				this.typeCode = value;
			}
		}

		public CultureMap CultureMap
		{
			get
			{
				return this.cultureMap;
			}
			set
			{
				StructuredData data = (value == null) ? null : value.GetCultureData(Resources.DefaultTwoLetterISOLanguageName);

				if (this.cultureMap != value || this.structuredData != data)
				{
					this.cultureMap = value;
					this.structuredData = data;
					this.UpdateContent();
				}
			}
		}

		public StructuredData StructuredData
		{
			get
			{
				return this.structuredData;
			}
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

		public DesignerApplication DesignerApplication
		{
			get
			{
				return this.designerApplication;
			}
			set
			{
				this.designerApplication = value;
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
			//	Retourne le texte du r�sum�.
			return "";
		}

		protected void PutSummaryInitialise()
		{
			this.summaryEmpty = true;
			this.summarySeparator = false;
		}

		protected void PutSummaryValue(System.Text.StringBuilder builder, string value)
		{
			this.PutSummarySeparator(builder, 1);
			builder.Append(value);

			this.summaryEmpty = false;
			this.summarySeparator = false;
		}

		protected void PutSummaryValue(System.Text.StringBuilder builder, string label, string value)
		{
			this.PutSummarySeparator(builder, 1);
			builder.Append(label);
			builder.Append(" = ");
			builder.Append(value);

			this.summaryEmpty = false;
			this.summarySeparator = false;
		}

		protected void PutSummarySeparator(System.Text.StringBuilder builder, int level)
		{
			if (this.summaryEmpty || this.summarySeparator)
			{
				return;
			}

			if (level == 1)
			{
				builder.Append(", ");
			}

			if (level == 2)
			{
				builder.Append("   �   ");
			}

			this.summaryEmpty = false;
			this.summarySeparator = true;
		}

		protected virtual string TypeToString(object value)
		{
			return value.ToString();
		}


		public void ClearCache()
		{
			//	Force une nouvelle mise � jour lors du prochain Update.
			this.resourceSelected = -1;
		}

		public virtual void UpdateContent()
		{
			//	Met � jour le contenu de l'�diteur.
		}

		protected string SelectedName
		{
			//	Retourne le nom de la ressource s�lectionn�e.
			get
			{
				if (this.resourceSelected != -1)
				{
					ResourceAccess.Field field = this.resourceAccess.GetField(this.resourceSelected, null, ResourceAccess.FieldType.Name);
					if (field != null)
					{
						return field.String;
					}
				}

				return null;
			}
		}


		#region Super widgets
		protected void CreateStringLabeled(string label, Widget parent, out FrameBox group, out TextFieldEx field)
		{
			//	Cr�e un super-widget permettant d'�diter une cha�ne, avec une �tiquette � gauche.
			group = new FrameBox(parent);
			group.TabIndex = this.tabIndex++;

			StaticText text = new StaticText(group);
			text.Text = label;
			text.ContentAlignment = ContentAlignment.MiddleRight;
			text.PreferredWidth = 160;
			text.Dock = DockStyle.Left;
			text.Margins = new Margins(0, 8, 0, 0);

			field = new TextFieldEx(group);
			field.ButtonShowCondition = ShowCondition.WhenModified;
			field.DefocusAction = DefocusAction.AutoAcceptOrRejectEdition;
			field.PreferredWidth = 130;
			field.Dock = DockStyle.Left;
			field.TabIndex = 1;
			field.TabNavigationMode = TabNavigationMode.ActivateOnTab;
		}

		protected void CreateComboLabeled(string label, Widget parent, out FrameBox group, out TextFieldCombo field)
		{
			//	Cr�e un super-widget permettant d'�diter une cha�ne, avec une �tiquette � gauche.
			group = new FrameBox(parent);
			group.TabIndex = this.tabIndex++;

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
			field.TabIndex = 1;
			field.TabNavigationMode = TabNavigationMode.ActivateOnTab;
		}

		protected void CreateDecimalLabeled(string label, Widget parent, out FrameBox group, out TextFieldEx field)
		{
			//	Cr�e un super-widget permettant d'�diter une valeur d�cimale, avec une �tiquette � gauche.
			group = new FrameBox(parent);
			group.TabIndex = this.tabIndex++;

			StaticText text = new StaticText(group);
			text.Text = label;
			text.ContentAlignment = ContentAlignment.MiddleRight;
			text.PreferredWidth = 160;
			text.Dock = DockStyle.Left;
			text.Margins = new Margins(0, 8, 0, 0);

			field = new TextFieldEx(group);
			field.PreferredWidth = 130;
			field.Dock = DockStyle.Left;
			field.TabIndex = 1;
			field.TabNavigationMode = TabNavigationMode.ActivateOnTab;
		}

		protected object GetStringObject(TextFieldEx field)
		{
			if (string.IsNullOrEmpty(field.Text))
			{
				return null;
			}
			else
			{
				return field.Text;
			}
		}

		protected void SetStringObject(TextFieldEx field, object value)
		{
			if (value == null)
			{
				field.Text = "";
			}
			else
			{
				field.Text = (string) value;
			}
		}

		protected object GetDecimalObject(TextFieldEx field, System.Type type)
		{
			if (string.IsNullOrEmpty(field.Text))
			{
				return null;
			}
			else
			{
				return Types.Converters.AutomaticValueConverter.Instance.Convert(field.Text, type, null, System.Globalization.CultureInfo.InvariantCulture);
			}
		}

		protected void SetDecimalObject(TextFieldEx field, object value)
		{
			if (value == null)
			{
				field.Text = "";
			}
			else
			{
				string s = (string) Types.Converters.AutomaticValueConverter.Instance.Convert(value, typeof(string), null, System.Globalization.CultureInfo.InvariantCulture);
				field.Text = s;
			}
		}

		protected decimal GetDecimal(TextFieldEx field)
		{
			decimal value;
			if (!decimal.TryParse(field.Text, out value))
			{
				return 0;
			}
			return value;
		}

		protected void SetDecimal(TextFieldEx field, decimal value)
		{
			field.Text = value.ToString();
		}

		protected IconButton CreateLocatorGotoButton(Widget parent)
		{
			//	Cr�e un bouton pour la commande "LocatorGoto".
			IconButton button = new IconButton(parent);

			button.CaptionId = Res.Captions.Editor.LocatorGoto.Id;
			button.AutoFocus = false;

			return button;
		}

		protected IconButton CreateIconButton()
		{
			IconButton button = new IconButton();

			return button;
		}
		#endregion


		#region Events
		public virtual void OnContentChanged()
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


		protected TypeCode						typeCode;
		protected CultureMap					cultureMap;
		protected StructuredData				structuredData;
		protected ResourceAccess				resourceAccess;
		protected int							resourceSelected = -1;
		protected Module						module;
		protected DesignerApplication			designerApplication;
		protected int							tabIndex = 1;
		protected bool							ignoreChange = false;
		protected bool							summaryEmpty;
		protected bool							summarySeparator;
	}
}
