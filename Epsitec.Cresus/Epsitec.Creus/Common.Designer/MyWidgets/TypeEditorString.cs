using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

namespace Epsitec.Common.Designer.MyWidgets
{
	/// <summary>
	/// Widget permettant d'éditer un Caption.Type.
	/// </summary>
	public class TypeEditorString : AbstractTypeEditor
	{
		public TypeEditorString(Module module)
		{
			this.module = module;

			this.CreateDecimalLabeled(Res.Strings.Viewers.Types.String.Min, this, out this.groupMin, out this.fieldMin);
			this.groupMin.Dock = DockStyle.StackBegin;
			this.groupMin.Margins = new Margins(0, 0, 0, 2);
			this.groupMin.ResetButton.Clicked += this.HandleResetButtonClicked;
			this.fieldMin.EditionAccepted += this.HandleTextFieldChanged;

			this.CreateDecimalLabeled(Res.Strings.Viewers.Types.String.Max, this, out this.groupMax, out this.fieldMax);
			this.groupMax.Dock = DockStyle.StackBegin;
			this.groupMax.Margins = new Margins(0, 0, 0, 10);
			this.groupMax.ResetButton.Clicked += this.HandleResetButtonClicked;
			this.fieldMax.EditionAccepted += this.HandleTextFieldChanged;

			this.groupMultilingual = new ResetBox(this);
			this.groupMultilingual.IsPatch = this.module.IsPatch;
			this.groupMultilingual.Dock = DockStyle.StackBegin;
			this.groupMultilingual.Margins = new Margins(0, 0, 0, 0);
			this.groupMultilingual.ResetButton.Clicked += this.HandleResetButtonClicked;

			this.checkMultilingual = new CheckButton(this.groupMultilingual.GroupBox);
			this.checkMultilingual.AutoToggle = false;
			this.checkMultilingual.Text = Res.Strings.Viewers.Types.String.Multilingual;
			this.checkMultilingual.Dock = DockStyle.Fill;
			this.checkMultilingual.Clicked += this.HandleCheckClicked;

			this.groupFormatted = new ResetBox(this);
			this.groupFormatted.IsPatch = this.module.IsPatch;
			this.groupFormatted.Dock = DockStyle.StackBegin;
			this.groupFormatted.Margins = new Margins(0, 0, 0, 0);
			this.groupFormatted.ResetButton.Clicked += this.HandleResetButtonClicked;

			this.checkFormatted = new CheckButton(this.groupFormatted.GroupBox);
			this.checkFormatted.AutoToggle = false;
			this.checkFormatted.Text = Res.Strings.Viewers.Types.String.Formatted;
			this.checkFormatted.Dock = DockStyle.Fill;
			this.checkFormatted.Clicked += this.HandleCheckClicked;

			this.CreateDecimalLabeled(Res.Strings.Viewers.Types.String.Default, this, out this.groupDefault, out this.fieldDefault);
			this.groupDefault.Dock = DockStyle.StackBegin;
			this.groupDefault.Margins = new Margins(0, 0, 10, 0);
			this.groupDefault.ResetButton.Clicked += this.HandleResetButtonClicked;
			this.fieldDefault.EditionAccepted += this.HandleTextFieldChanged;
		}

		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.groupMin.ResetButton.Clicked -= this.HandleResetButtonClicked;
				this.groupMax.ResetButton.Clicked -= this.HandleResetButtonClicked;
				this.groupDefault.ResetButton.Clicked -= this.HandleResetButtonClicked;
				this.groupMultilingual.ResetButton.Clicked -= this.HandleResetButtonClicked;
				this.groupFormatted.ResetButton.Clicked -= this.HandleResetButtonClicked;

				this.fieldMin.EditionAccepted -= this.HandleTextFieldChanged;
				this.fieldMax.EditionAccepted -= this.HandleTextFieldChanged;
				this.fieldDefault.EditionAccepted -= this.HandleTextFieldChanged;
				this.checkMultilingual.Clicked -= this.HandleCheckClicked;
				this.checkFormatted.Clicked -= this.HandleCheckClicked;
			}
			
			base.Dispose(disposing);
		}


		public override string GetSummary()
		{
			//	Retourne le texte du résumé.
			System.Text.StringBuilder builder = new System.Text.StringBuilder();
			this.PutSummaryInitialise();
			object value;

			value = this.structuredData.GetValue(Support.Res.Fields.ResourceStringType.MinimumLength);
			if (!UndefinedValue.IsUndefinedValue(value))
			{
				int min = (int) value;
				if (min != 0)
				{
					this.PutSummaryValue(builder, Res.Strings.Viewers.Types.Summary.Min, min.ToString());
				}
			}

			value = this.structuredData.GetValue(Support.Res.Fields.ResourceStringType.MaximumLength);
			if (!UndefinedValue.IsUndefinedValue(value))
			{
				int max = (int) value;
				if (max != 0)
				{
					this.PutSummaryValue(builder, Res.Strings.Viewers.Types.Summary.Max, max.ToString());
				}
			}

			value = this.structuredData.GetValue(Support.Res.Fields.ResourceBaseType.DefaultValue);
			if (!UndefinedValue.IsUndefinedValue(value))
			{
				string def = value as string;
				if (def != null)
				{
					this.PutSummaryValue(builder, Res.Strings.Viewers.Types.Summary.Default, def);
				}
			}

			value = this.structuredData.GetValue(Support.Res.Fields.ResourceStringType.UseMultilingualStorage);
			if (!UndefinedValue.IsUndefinedValue(value))
			{
				bool multi = (bool) value;
				if (multi)
				{
					this.PutSummaryValue(builder, Res.Strings.Viewers.Types.Summary.Multi);
				}
			}

			value = this.structuredData.GetValue(Support.Res.Fields.ResourceStringType.UseFormattedText);
			if (!UndefinedValue.IsUndefinedValue(value))
			{
				bool formatted = (bool) value;
				if (formatted)
				{
					this.PutSummaryValue(builder, Res.Strings.Viewers.Types.Summary.Formatted);
				}
			}

			return builder.ToString();
		}


		public override void UpdateContent()
		{
			//	Met à jour le contenu de l'éditeur.
			this.ignoreChange = true;
			object value;
			bool usesOriginalData;

			CultureMapSource source = this.module.AccessTypes.GetCultureMapSource(this.cultureMap);

			value = this.structuredData.GetValue(Support.Res.Fields.ResourceStringType.MinimumLength, out usesOriginalData);
			this.ColorizeResetBox(this.groupMin, source, usesOriginalData);
			if (UndefinedValue.IsUndefinedValue(value))
			{
				this.fieldMin.Text = "";
			}
			else
			{
				int min = (int) value;
				if (min == 0)
				{
					this.fieldMin.Text = "";
				}
				else
				{
					this.SetDecimal(this.fieldMin, min);
				}
			}
			
			value = this.structuredData.GetValue(Support.Res.Fields.ResourceStringType.MaximumLength, out usesOriginalData);
			this.ColorizeResetBox(this.groupMax, source, usesOriginalData);
			if (UndefinedValue.IsUndefinedValue(value))
			{
				this.fieldMax.Text = "";
			}
			else
			{
				int max = (int) value;
				if (max == 0)
				{
					this.fieldMax.Text = "";
				}
				else
				{
					this.SetDecimal(this.fieldMax, max);
				}
			}
			
			value = this.structuredData.GetValue(Support.Res.Fields.ResourceBaseType.DefaultValue, out usesOriginalData);
			this.ColorizeResetBox(this.groupDefault, source, usesOriginalData);
			if (UndefinedValue.IsUndefinedValue(value))
			{
				this.fieldDefault.Text = ResourceBundle.Field.Null;
			}
			else
			{
				string def = value as string;
				if (def == null)
				{
					this.fieldDefault.Text = ResourceBundle.Field.Null;
				}
				else
				{
					this.fieldDefault.Text = def;
				}
			}
			
			value = this.structuredData.GetValue(Support.Res.Fields.ResourceStringType.UseMultilingualStorage, out usesOriginalData);
			this.ColorizeResetBox(this.groupMultilingual, source, usesOriginalData);
			if (UndefinedValue.IsUndefinedValue(value))
			{
				this.checkMultilingual.ActiveState = ActiveState.No;
			}
			else
			{
				bool multi = (bool) value;
				this.checkMultilingual.ActiveState = multi ? ActiveState.Yes : ActiveState.No;
			}

			value = this.structuredData.GetValue(Support.Res.Fields.ResourceStringType.UseFormattedText, out usesOriginalData);
			this.ColorizeResetBox(this.groupFormatted, source, usesOriginalData);
			if (UndefinedValue.IsUndefinedValue(value))
			{
				this.checkFormatted.ActiveState = ActiveState.No;
			}
			else
			{
				bool formatted = (bool) value;
				this.checkFormatted.ActiveState = formatted ? ActiveState.Yes : ActiveState.No;
			}

			this.ignoreChange = false;
		}


		private void HandleTextFieldChanged(object sender)
		{
			if (this.ignoreChange)
			{
				return;
			}

			if (sender == this.fieldMin)
			{
				int min = (int) this.GetDecimal(this.fieldMin);
				this.structuredData.SetValue(Support.Res.Fields.ResourceStringType.MinimumLength, min);
			}

			if (sender == this.fieldMax)
			{
				int max = (int) this.GetDecimal(this.fieldMax);
				this.structuredData.SetValue(Support.Res.Fields.ResourceStringType.MaximumLength, max);
			}

			if (sender == this.fieldDefault)
			{
				string def = this.fieldDefault.Text;
				if (ResourceBundle.Field.IsNullString (def))
				{
					this.structuredData.SetValue (Support.Res.Fields.ResourceBaseType.DefaultValue, UndefinedValue.Value);
				}
				else
				{
					this.structuredData.SetValue (Support.Res.Fields.ResourceBaseType.DefaultValue, def);
				}
			}

			this.OnContentChanged();
			this.UpdateContent();
			this.module.AccessTypes.SetLocalDirty();
		}

		private void HandleCheckClicked(object sender, MessageEventArgs e)
		{
			if (this.ignoreChange)
			{
				return;
			}

			if (sender == this.checkMultilingual)
			{
				bool multi = false;
				object value = this.structuredData.GetValue(Support.Res.Fields.ResourceStringType.UseMultilingualStorage);
				if (!UndefinedValue.IsUndefinedValue(value))
				{
					multi = (bool) value;
				}

				this.structuredData.SetValue(Support.Res.Fields.ResourceStringType.UseMultilingualStorage, !multi);
			}

			if (sender == this.checkFormatted)
			{
				bool formatted = false;
				object value = this.structuredData.GetValue(Support.Res.Fields.ResourceStringType.UseFormattedText);
				if (!UndefinedValue.IsUndefinedValue(value))
				{
					formatted = (bool) value;
				}

				this.structuredData.SetValue(Support.Res.Fields.ResourceStringType.UseFormattedText, !formatted);
			}

			this.OnContentChanged();
			this.UpdateContent();
			this.module.AccessTypes.SetLocalDirty();
		}

		private void HandleResetButtonClicked(object sender, MessageEventArgs e)
		{
			AbstractButton button = sender as AbstractButton;

			if (button == this.groupMin.ResetButton)
			{
				this.ResetToOriginalValue(Support.Res.Fields.ResourceStringType.MinimumLength);
			}

			if (button == this.groupMax.ResetButton)
			{
				this.ResetToOriginalValue(Support.Res.Fields.ResourceStringType.MaximumLength);
			}

			if (button == this.groupDefault.ResetButton)
			{
				this.ResetToOriginalValue(Support.Res.Fields.ResourceBaseType.DefaultValue);
			}

			if (button == this.groupMultilingual.ResetButton)
			{
				this.ResetToOriginalValue(Support.Res.Fields.ResourceStringType.UseMultilingualStorage);
			}

			if (button == this.groupFormatted.ResetButton)
			{
				this.ResetToOriginalValue(Support.Res.Fields.ResourceStringType.UseFormattedText);
			}

			this.OnContentChanged();
			this.UpdateContent();
			this.module.AccessTypes.SetLocalDirty();
		}


		protected ResetBox						groupMin;
		protected TextFieldEx					fieldMin;
		protected ResetBox						groupMax;
		protected TextFieldEx					fieldMax;
		protected ResetBox						groupMultilingual;
		protected CheckButton					checkMultilingual;
		protected ResetBox						groupFormatted;
		protected CheckButton					checkFormatted;
		protected ResetBox						groupDefault;
		protected TextFieldEx					fieldDefault;
	}
}
