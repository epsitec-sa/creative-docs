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
			FrameBox group;

			this.CreateDecimalLabeled(Res.Strings.Viewers.Types.String.Min, this, out group, out this.fieldMin);
			group.Dock = DockStyle.StackBegin;
			group.Margins = new Margins(0, 0, 0, 2);
			this.fieldMin.TextChanged += new EventHandler(this.HandleTextFieldChanged);

			this.CreateDecimalLabeled(Res.Strings.Viewers.Types.String.Max, this, out group, out this.fieldMax);
			group.Dock = DockStyle.StackBegin;
			group.Margins = new Margins(0, 0, 0, 10);
			this.fieldMax.TextChanged += new EventHandler(this.HandleTextFieldChanged);

#if false
			this.CreateStringLabeled(Res.Strings.Viewers.Types.String.Default, this, out group, out this.fieldDefault);
			group.Dock = DockStyle.StackBegin;
			group.Margins = new Margins(0, 0, 0, 2);
			this.fieldDefault.PreferredWidth = 400;
			this.fieldDefault.TextChanged += new EventHandler(this.HandleTextFieldChanged);

			this.CreateStringLabeled(Res.Strings.Viewers.Types.String.Sample, this, out group, out this.fieldSample);
			group.Dock = DockStyle.StackBegin;
			group.Margins = new Margins(0, 0, 0, 10);
			this.fieldSample.PreferredWidth = 400;
			this.fieldSample.TextChanged += new EventHandler(this.HandleTextFieldChanged);
			
			this.checkFixedLength = new CheckButton(this);
			this.checkFixedLength.AutoToggle = false;
			this.checkFixedLength.Text = Res.Strings.Viewers.Types.String.FixedLength;
			this.checkFixedLength.Dock = DockStyle.StackBegin;
			this.checkFixedLength.Margins = new Margins(0, 0, 0, 3);
			this.checkFixedLength.Clicked += new MessageEventHandler(this.HandleCheckClicked);
#endif

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
				this.fieldMin.TextChanged -= new EventHandler(this.HandleTextFieldChanged);
				this.fieldMax.TextChanged -= new EventHandler(this.HandleTextFieldChanged);
#if false

				this.fieldDefault.TextChanged -= new EventHandler(this.HandleTextFieldChanged);
				this.fieldSample.TextChanged -= new EventHandler(this.HandleTextFieldChanged);
				this.checkFixedLength.Clicked -= new MessageEventHandler(this.HandleCheckClicked);
#endif
				this.checkMultilingual.Clicked -= new MessageEventHandler(this.HandleCheckClicked);
			}
			
			base.Dispose(disposing);
		}


		public override string GetSummary()
		{
			//	Retourne le texte du résumé.
			System.Text.StringBuilder builder = new System.Text.StringBuilder();
			this.PutSummaryInitialise();
#if true
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
#else
			StringType type = this.AbstractType as StringType;

			this.PutSummaryValue(builder, Res.Strings.Viewers.Types.Summary.Min, type.MinimumLength.ToString());
			this.PutSummaryValue(builder, Res.Strings.Viewers.Types.Summary.Max, type.MaximumLength.ToString());

			this.PutSummaryDefaultAndSample(builder, type);

			if (type.UseFixedLengthStorage || type.UseMultilingualStorage)
			{
				this.PutSummarySeparator(builder, 2);

				if (type.UseFixedLengthStorage)
				{
					this.PutSummaryValue(builder, Res.Strings.Viewers.Types.Summary.Fix);
				}

				if (type.UseMultilingualStorage)
				{
					this.PutSummaryValue(builder, Res.Strings.Viewers.Types.Summary.Multi);
				}
			}

			return builder.ToString();
#endif
		}


		protected override void UpdateContent()
		{
			//	Met à jour le contenu de l'éditeur.
#if true
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
#else
			StringType type = this.AbstractType as StringType;

			this.ignoreChange = true;
			this.SetDecimal(this.fieldMin, type.MinimumLength);
			this.SetDecimal(this.fieldMax, type.MaximumLength);
			this.SetStringObject(this.fieldDefault, type.DefaultValue);
			this.SetStringObject(this.fieldSample, type.SampleValue);
			this.checkFixedLength.ActiveState = type.UseFixedLengthStorage ? ActiveState.Yes : ActiveState.No;
			this.checkMultilingual.ActiveState = type.UseMultilingualStorage ? ActiveState.Yes : ActiveState.No;
			this.ignoreChange = false;
#endif
		}


		private void HandleTextFieldChanged(object sender)
		{
			if (this.ignoreChange)
			{
				return;
			}

#if true
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
#else
			//	[Note1] On demande le type avec un ResourceAccess.GetField.
			StringType type = this.AbstractType as StringType;

			if (sender == this.fieldMin)
			{
				type.DefineMinimumLength((int) this.GetDecimal(this.fieldMin));
			}

			if (sender == this.fieldMax)
			{
				type.DefineMaximumLength((int) this.GetDecimal(this.fieldMax));
			}

			if (sender == this.fieldDefault)
			{
				type.DefineDefaultValue(this.GetStringObject(this.fieldDefault));
			}

			if (sender == this.fieldSample)
			{
				type.DefineSampleValue(this.GetStringObject(this.fieldSample));
			}

			//	[Note1] Cet appel va provoquer le ResourceAccess.SetField.
			this.OnContentChanged();
#endif
		}

		private void HandleCheckClicked(object sender, MessageEventArgs e)
		{
			if (this.ignoreChange)
			{
				return;
			}

#if true
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
#else
			//	[Note1] On demande le type avec un ResourceAccess.GetField.
			StringType type = this.AbstractType as StringType;

			if (sender == this.checkFixedLength)
			{
				type.DefineUseFixedLengthStorage(!type.UseFixedLengthStorage);
			}

			if (sender == this.checkMultilingual)
			{
				type.DefineUseMultilingualStorage(!type.UseMultilingualStorage);
			}

			//	[Note1] Cet appel va provoquer le ResourceAccess.SetField.
			this.OnContentChanged();
#endif
		}


		protected TextField						fieldMin;
		protected TextField						fieldMax;
		protected TextField						fieldDefault;
		protected TextField						fieldSample;
		protected CheckButton					checkFixedLength;
		protected CheckButton					checkMultilingual;
	}
}
