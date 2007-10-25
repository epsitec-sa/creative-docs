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
		public TypeEditorString()
		{
			ResetBox group;

			this.CreateDecimalLabeled(Res.Strings.Viewers.Types.String.Min, this, out group, out this.fieldMin);
			group.Dock = DockStyle.StackBegin;
			group.Margins = new Margins(0, 0, 0, 2);
			this.fieldMin.EditionAccepted += new EventHandler(this.HandleTextFieldChanged);

			this.CreateDecimalLabeled(Res.Strings.Viewers.Types.String.Max, this, out group, out this.fieldMax);
			group.Dock = DockStyle.StackBegin;
			group.Margins = new Margins(0, 0, 0, 10);
			this.fieldMax.EditionAccepted += new EventHandler(this.HandleTextFieldChanged);

			this.checkMultilingual = new CheckButton(this);
			this.checkMultilingual.AutoToggle = false;
			this.checkMultilingual.Text = Res.Strings.Viewers.Types.String.Multilingual;
			this.checkMultilingual.Dock = DockStyle.StackBegin;
			this.checkMultilingual.Margins = new Margins(0, 0, 0, 0);
			this.checkMultilingual.Clicked += new MessageEventHandler(this.HandleCheckClicked);
		}

		public TypeEditorString(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.fieldMin.EditionAccepted -= new EventHandler(this.HandleTextFieldChanged);
				this.fieldMax.EditionAccepted -= new EventHandler(this.HandleTextFieldChanged);
				this.checkMultilingual.Clicked -= new MessageEventHandler(this.HandleCheckClicked);
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
			//	Met à jour le contenu de l'éditeur.
			this.ignoreChange = true;
			object value;

			value = this.structuredData.GetValue(Support.Res.Fields.ResourceStringType.MinimumLength);
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
			
			value = this.structuredData.GetValue(Support.Res.Fields.ResourceStringType.MaximumLength);
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
			
			value = this.structuredData.GetValue(Support.Res.Fields.ResourceStringType.UseMultilingualStorage);
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


		protected TextFieldEx					fieldMin;
		protected TextFieldEx					fieldMax;
		protected CheckButton					checkMultilingual;
	}
}
