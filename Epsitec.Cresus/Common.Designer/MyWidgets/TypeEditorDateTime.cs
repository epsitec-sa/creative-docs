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

			this.CreateComboLabeled("Résolution", this, out group, out this.fieldResol);
			group.Dock = DockStyle.StackBegin;
			group.Margins = new Margins(0, 0, 0, 10);
			this.fieldResol.TextChanged += new EventHandler(this.HandleTextFieldChanged);
			this.fieldResol.Items.Add("Milliseconds");
			this.fieldResol.Items.Add("Seconds");
			this.fieldResol.Items.Add("Minutes");
			this.fieldResol.Items.Add("Hours");
			this.fieldResol.Items.Add("Days");
			this.fieldResol.Items.Add("Weeks");
			this.fieldResol.Items.Add("Months");
			this.fieldResol.Items.Add("Years");

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
				this.fieldResol.TextChanged -= new EventHandler(this.HandleTextFieldChanged);
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

			if (type.Resolution != TimeResolution.Default)
			{
				builder.Append("Résolution = ");
				builder.Append(TypeEditorDateTime.Convert(type.Resolution));
			}

			if (type.MinimumDate != Date.Null)
			{
				this.PutSummaryLegend(builder, "Date min = ");
				builder.Append(type.MinimumDate.ToString());
			}

			if (type.MaximumDate != Date.Null)
			{
				this.PutSummaryLegend(builder, "Date max = ");
				builder.Append(type.MaximumDate.ToString());
			}

			if (type.MinimumTime != Time.Null)
			{
				this.PutSummaryLegend(builder, "Heure min = ");
				builder.Append(type.MinimumTime.ToString());
			}

			if (type.MaximumTime != Time.Null)
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
			this.fieldResol.Text = TypeEditorDateTime.Convert(type.Resolution);
			this.ignoreChange = false;
		}


		protected static TimeResolution Convert(string text)
		{
			switch (text)
			{
				case "Milliseconds":  return TimeResolution.Milliseconds;
				case "Seconds":       return TimeResolution.Seconds;
				case "Minutes":       return TimeResolution.Minutes;
				case "Hours":         return TimeResolution.Hours;
				case "Days":          return TimeResolution.Days;
				case "Weeks":         return TimeResolution.Weeks;
				case "Months":        return TimeResolution.Months;
				case "Years":         return TimeResolution.Years;
				default:              return TimeResolution.Default;
			}
		}

		protected static string Convert(TimeResolution resol)
		{
			switch (resol)
			{
				case TimeResolution.Milliseconds:  return "Milliseconds";
				case TimeResolution.Seconds:       return "Seconds";
				case TimeResolution.Minutes:       return "Minutes";
				case TimeResolution.Hours:         return "Hours";
				case TimeResolution.Days:          return "Days";
				case TimeResolution.Weeks:         return "Weeks";
				case TimeResolution.Months:        return "Months";
				case TimeResolution.Years:         return "Years";
				default:                           return "";
			}
		}


		private void HandleTextFieldChanged(object sender)
		{
			if (this.ignoreChange)
			{
				return;
			}

			//	[Note1] On demande le type avec un ResourceAccess.GetField.
			AbstractDateTimeType type = this.AbstractType as AbstractDateTimeType;

			if (sender == this.fieldResol)
			{
				type.DefineResolution(TypeEditorDateTime.Convert(this.fieldResol.Text));
			}

			if (sender == this.fieldMinDate)
			{
			}

			if (sender == this.fieldMaxDate)
			{
			}

			//	[Note1] Cet appel va provoquer le ResourceAccess.SetField.
			this.OnContentChanged();
		}


		protected TextFieldCombo				fieldResol;
		protected TextField						fieldMinDate;
		protected TextField						fieldMaxDate;
		protected TextField						fieldMinTime;
		protected TextField						fieldMaxTime;
		protected TextField						fieldDateStep;
		protected TextField						fieldTimeStep;
	}
}
