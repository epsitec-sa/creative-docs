using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

namespace Epsitec.Common.Designer.MyWidgets
{
	/// <summary>
	/// Widget permettant d'�diter un Caption.Type.
	/// </summary>
	public class TypeEditorString : AbstractTypeEditor
	{
		public TypeEditorString(Module module)
		{
			this.module = module;

			this.CreateDecimalLabeled(Res.Strings.Viewers.Types.String.Min, this, out this.groupMin, out this.fieldMin);
			this.groupMin.Dock = DockStyle.StackBegin;
			this.groupMin.Margins = new Margins(0, 0, 0, 2);
			this.groupMin.ResetButton.Clicked += new MessageEventHandler(this.HandleResetButtonClicked);
			this.fieldMin.EditionAccepted += new EventHandler(this.HandleTextFieldChanged);

			this.CreateDecimalLabeled(Res.Strings.Viewers.Types.String.Max, this, out this.groupMax, out this.fieldMax);
			this.groupMax.Dock = DockStyle.StackBegin;
			this.groupMax.Margins = new Margins(0, 0, 0, 10);
			this.groupMax.ResetButton.Clicked += new MessageEventHandler(this.HandleResetButtonClicked);
			this.fieldMax.EditionAccepted += new EventHandler(this.HandleTextFieldChanged);

			this.groupMultilingual = new ResetBox(this);
			this.groupMultilingual.IsPatch = this.module.IsPatch;
			this.groupMultilingual.Dock = DockStyle.StackBegin;
			this.groupMultilingual.Margins = new Margins(0, 0, 0, 0);
			this.groupMultilingual.ResetButton.Clicked += new MessageEventHandler(this.HandleResetButtonClicked);

			this.checkMultilingual = new CheckButton(this.groupMultilingual.GroupBox);
			this.checkMultilingual.AutoToggle = false;
			this.checkMultilingual.Text = Res.Strings.Viewers.Types.String.Multilingual;
			this.checkMultilingual.Dock = DockStyle.Fill;
			this.checkMultilingual.Clicked += new MessageEventHandler(this.HandleCheckClicked);
		}

		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.groupMin.ResetButton.Clicked -= new MessageEventHandler(this.HandleResetButtonClicked);
				this.groupMax.ResetButton.Clicked -= new MessageEventHandler(this.HandleResetButtonClicked);

				this.fieldMin.EditionAccepted -= new EventHandler(this.HandleTextFieldChanged);
				this.fieldMax.EditionAccepted -= new EventHandler(this.HandleTextFieldChanged);
				this.checkMultilingual.Clicked -= new MessageEventHandler(this.HandleCheckClicked);
			}
			
			base.Dispose(disposing);
		}


		public override string GetSummary()
		{
			//	Retourne le texte du r�sum�.
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

			value = this.structuredData.GetValue(Support.Res.Fields.ResourceStringType.UseMultilingualStorage);
			if (!UndefinedValue.IsUndefinedValue(value))
			{
				bool multi = (bool) value;
				if (multi)
				{
					this.PutSummaryValue(builder, Res.Strings.Viewers.Types.Summary.Multi);
				}
			}

			return builder.ToString();
		}


		public override void UpdateContent()
		{
			//	Met � jour le contenu de l'�diteur.
			this.ignoreChange = true;
			object value;
			bool usesOriginalData;

			CultureMapSource source = this.module.AccessTypes.GetCultureMapSource(this.cultureMap);

			value = this.structuredData.GetValue(Support.Res.Fields.ResourceStringType.MinimumLength, out usesOriginalData);
			Viewers.Abstract.ColorizeResetBox(this.groupMin, source, usesOriginalData);
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
			Viewers.Abstract.ColorizeResetBox(this.groupMax, source, usesOriginalData);
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
			
			value = this.structuredData.GetValue(Support.Res.Fields.ResourceStringType.UseMultilingualStorage, out usesOriginalData);
			Viewers.Abstract.ColorizeResetBox(this.groupMultilingual, source, usesOriginalData);
			if (UndefinedValue.IsUndefinedValue(value))
			{
				this.checkMultilingual.ActiveState = ActiveState.No;
			}
			else
			{
				bool multi = (bool) value;
				this.checkMultilingual.ActiveState = multi ? ActiveState.Yes : ActiveState.No;
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

			if (button == this.groupMultilingual.ResetButton)
			{
				this.ResetToOriginalValue(Support.Res.Fields.ResourceStringType.UseMultilingualStorage);
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
	}
}
