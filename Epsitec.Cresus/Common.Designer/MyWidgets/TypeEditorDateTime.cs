using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

namespace Epsitec.Common.Designer.MyWidgets
{
	/// <summary>
	/// Widget permettant d'éditer un Caption.Type.
	/// </summary>
	public class TypeEditorDateTime : AbstractTypeEditor
	{
		public TypeEditorDateTime()
		{
			Widget group;

			this.CreateStringLabeled("Date minimale", this, out group, out this.fieldMinDate);
			group.Dock = DockStyle.StackBegin;
			group.Margins = new Margins(0, 0, 0, 2);
			this.fieldMinDate.TextChanged += new EventHandler(this.HandleTextFieldChanged);

			this.CreateStringLabeled("Date maximale", this, out group, out this.fieldMaxDate);
			group.Dock = DockStyle.StackBegin;
			group.Margins = new Margins(0, 0, 0, 10);
			this.fieldMaxDate.TextChanged += new EventHandler(this.HandleTextFieldChanged);

			this.CreateStringLabeled("Heure minimale", this, out group, out this.fieldMinTime);
			group.Dock = DockStyle.StackBegin;
			group.Margins = new Margins(0, 0, 0, 2);
			this.fieldMinTime.TextChanged += new EventHandler(this.HandleTextFieldChanged);

			this.CreateStringLabeled("Heure maximale", this, out group, out this.fieldMaxTime);
			group.Dock = DockStyle.StackBegin;
			group.Margins = new Margins(0, 0, 0, 10);
			this.fieldMaxTime.TextChanged += new EventHandler(this.HandleTextFieldChanged);

			this.CreateStringLabeled("Pas pour la date", this, out group, out this.fieldDateStep);
			group.Dock = DockStyle.StackBegin;
			group.Margins = new Margins(0, 0, 0, 2);
			this.fieldDateStep.TextChanged += new EventHandler(this.HandleTextFieldChanged);

			this.CreateStringLabeled("Pas pour l'heure", this, out group, out this.fieldTimeStep);
			group.Dock = DockStyle.StackBegin;
			group.Margins = new Margins(0, 0, 0, 2);
			this.fieldTimeStep.TextChanged += new EventHandler(this.HandleTextFieldChanged);
		}

		public TypeEditorDateTime(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.fieldMinDate.TextChanged -= new EventHandler(this.HandleTextFieldChanged);
				this.fieldMaxDate.TextChanged -= new EventHandler(this.HandleTextFieldChanged);
				this.fieldMinTime.TextChanged -= new EventHandler(this.HandleTextFieldChanged);
				this.fieldMaxTime.TextChanged -= new EventHandler(this.HandleTextFieldChanged);
				this.fieldDateStep.TextChanged -= new EventHandler(this.HandleTextFieldChanged);
				this.fieldTimeStep.TextChanged -= new EventHandler(this.HandleTextFieldChanged);
			}
			
			base.Dispose(disposing);
		}


		public override string GetSummary()
		{
			//	Retourne le texte du résumé.
			System.Text.StringBuilder builder = new System.Text.StringBuilder();

			AbstractDateTimeType type = this.AbstractType as AbstractDateTimeType;

			if (type.MinimumDate == Date.Null)
			{
				this.PutSummaryLegend(builder, "Date min = ");
				builder.Append(type.MinimumDate.ToString());
			}

			if (type.MaximumDate == Date.Null)
			{
				this.PutSummaryLegend(builder, "Date max = ");
				builder.Append(type.MaximumDate.ToString());
			}

			if (type.MinimumTime == Time.Null)
			{
				this.PutSummaryLegend(builder, "Heure min = ");
				builder.Append(type.MinimumTime.ToString());
			}

			if (type.MaximumTime == Time.Null)
			{
				this.PutSummaryLegend(builder, "Heure max = ");
				builder.Append(type.MaximumTime.ToString());
			}

			this.PutSummaryLegend(builder, "Pas date = ");
			builder.Append(type.DateStep.ToString());

			this.PutSummaryLegend(builder, "Pas heure = ");
			builder.Append(type.TimeStep.ToString());

			return builder.ToString();
		}

		protected void PutSummaryLegend(System.Text.StringBuilder builder, string legend)
		{
			if (builder.Length > 0)
			{
				builder.Append(", ");
			}

			builder.Append(legend);
		}


		protected override void UpdateContent()
		{
			//	Met à jour le contenu de l'éditeur.
			AbstractDateTimeType type = this.AbstractType as AbstractDateTimeType;

			this.ignoreChange = true;
			this.ignoreChange = false;
		}


		private void HandleTextFieldChanged(object sender)
		{
			if (this.ignoreChange)
			{
				return;
			}

			//	[Note1] On demande le type avec un ResourceAccess.GetField.
			AbstractDateTimeType type = this.AbstractType as AbstractDateTimeType;

			if (sender == this.fieldMinDate)
			{
			}

			if (sender == this.fieldMaxDate)
			{
			}

			//	[Note1] Cet appel va provoquer le ResourceAccess.SetField.
			this.OnContentChanged();
		}


		protected TextFieldCombo				fieldType;
		protected TextField						fieldMinDate;
		protected TextField						fieldMaxDate;
		protected TextField						fieldMinTime;
		protected TextField						fieldMaxTime;
		protected TextField						fieldDateStep;
		protected TextField						fieldTimeStep;
	}
}
