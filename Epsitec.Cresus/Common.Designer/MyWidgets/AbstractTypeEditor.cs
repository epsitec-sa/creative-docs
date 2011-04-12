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
		
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
			}
			
			base.Dispose(disposing);
		}


		public static AbstractTypeEditor Create(TypeCode typeCode, Module module)
		{
			//	Crée le bon widget AbstractTypeEditor pour éditer un type.
			switch (typeCode)
			{
				case TypeCode.Boolean:      return new TypeEditorBoolean(module);
				case TypeCode.Integer:      return new TypeEditorNumeric(module);
				case TypeCode.LongInteger:  return new TypeEditorNumeric(module);
				case TypeCode.Double:       return new TypeEditorNumeric(module);
				case TypeCode.Decimal:      return new TypeEditorNumeric(module);
				case TypeCode.String:       return new TypeEditorString(module);
				case TypeCode.Enum:         return new TypeEditorEnum(module);
				case TypeCode.Date:         return new TypeEditorDateTime(module);
				case TypeCode.Time:         return new TypeEditorDateTime(module);
				case TypeCode.DateTime:     return new TypeEditorDateTime(module);
				case TypeCode.Binary:       return new TypeEditorBinary(module);
				case TypeCode.Other:        return new TypeEditorNative(module);
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
			//	Retourne le texte du résumé.
			return "";
		}

		public void ResetToOriginalValue(Druid id)
		{
			//	Force la ressource à revenir à la version "par défaut" du module de référence.
			this.module.AccessTypes.Accessor.ResetToOriginalValue(this.cultureMap, this.structuredData, id);
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
				builder.Append("   —   ");
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
			//	Force une nouvelle mise à jour lors du prochain Update.
			this.resourceSelected = -1;
		}

		public virtual void UpdateContent()
		{
			//	Met à jour le contenu de l'éditeur.
		}

		protected string SelectedName
		{
			//	Retourne le nom de la ressource sélectionnée.
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


		public void ColorizeResetBox(MyWidgets.ResetBox box, CultureMapSource source, bool usesOriginalData)
		{
			//	Colore la boîte si on est dans un module de patch avec redéfinition de la donnée.
			if (!box.IsPatch || source != CultureMapSource.DynamicMerge || usesOriginalData)
			{
				box.BackColor = Color.Empty;
				box.ResetButton.Enable = false;
			}
			else
			{
				box.BackColor = Misc.SourceColor(CultureMapSource.DynamicMerge);
				box.ResetButton.Enable = !this.designerApplication.IsReadonly;
			}
		}


		#region Super widgets
		protected void CreateStringLabeled(string label, Widget parent, out ResetBox group, out TextFieldEx field)
		{
			//	Crée un super-widget permettant d'éditer une chaîne, avec une étiquette à gauche.
			group = new ResetBox(parent);
			System.Diagnostics.Debug.Assert(this.module != null);
			group.IsPatch = this.module.IsPatch;
			group.TabIndex = this.tabIndex++;

			StaticText text = new StaticText(group.GroupBox);
			text.Text = label;
			text.ContentAlignment = ContentAlignment.MiddleRight;
			text.PreferredWidth = 100;
			text.Dock = DockStyle.Left;
			text.Margins = new Margins(0, 8, 0, 0);

			field = new TextFieldEx(group.GroupBox);
			field.ButtonShowCondition = ButtonShowCondition.WhenModified;
			field.DefocusAction = DefocusAction.AutoAcceptOrRejectEdition;
			field.MinWidth = 120;
			field.Dock = DockStyle.Fill;
			field.TabIndex = 1;
			field.TabNavigationMode = TabNavigationMode.ActivateOnTab;
		}

		protected void CreateComboLabeled(string label, Widget parent, out ResetBox group, out TextFieldCombo field)
		{
			//	Crée un super-widget permettant d'éditer une chaîne, avec une étiquette à gauche.
			group = new ResetBox(parent);
			System.Diagnostics.Debug.Assert(this.module != null);
			group.IsPatch = this.module.IsPatch;
			group.TabIndex = this.tabIndex++;

			StaticText text = new StaticText(group.GroupBox);
			text.Text = label;
			text.ContentAlignment = ContentAlignment.MiddleRight;
			text.PreferredWidth = 100;
			text.Dock = DockStyle.Left;
			text.Margins = new Margins(0, 8, 0, 0);

			field = new TextFieldCombo(group.GroupBox);
			field.IsReadOnly = true;
			field.MinWidth = 120;
			field.Dock = DockStyle.Fill;
			field.TabIndex = 1;
			field.TabNavigationMode = TabNavigationMode.ActivateOnTab;
		}

		protected void CreateDecimalLabeled(string label, Widget parent, out ResetBox group, out TextFieldEx field)
		{
			//	Crée un super-widget permettant d'éditer une valeur décimale, avec une étiquette à gauche.
			group = new ResetBox(parent);
			System.Diagnostics.Debug.Assert(this.module != null);
			group.IsPatch = this.module.IsPatch;
			group.TabIndex = this.tabIndex++;
			
			StaticText text = new StaticText(group.GroupBox);
			text.Text = label;
			text.ContentAlignment = ContentAlignment.MiddleRight;
			text.PreferredWidth = 100;
			text.Dock = DockStyle.Left;
			text.Margins = new Margins(0, 8, 0, 0);

			field = new TextFieldEx(group.GroupBox);
			field.ButtonShowCondition = ButtonShowCondition.WhenModified;
			field.DefocusAction = DefocusAction.AutoAcceptOrRejectEdition;
			field.MinWidth = 120;
			field.Dock = DockStyle.Fill;
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

		protected decimal? GetDecimalOrNull(TextFieldEx field)
		{
			decimal value;
			if (!decimal.TryParse(field.Text, out value))
			{
				return null;
			}
			return value;
		}

		protected void SetDecimal(TextFieldEx field, decimal value)
		{
			field.Text = value.ToString();
		}

		protected IconButton CreateLocatorGotoButton(Widget parent)
		{
			//	Crée un bouton pour la commande "LocatorGoto".
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
			var handler = this.GetUserEventHandler("ContentChanged");
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
