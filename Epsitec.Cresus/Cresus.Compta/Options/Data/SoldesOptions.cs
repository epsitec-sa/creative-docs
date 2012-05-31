//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Graph;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Options.Data
{
	/// <summary>
	/// Cette classe décrit les options d'affichage des soldes de la comptabilité.
	/// </summary>
	public class SoldesOptions : AbstractOptions
	{
		public SoldesOptions()
		{
			this.soldesColumns = new List<SoldesColumn> ();
		}

		public override void Clear()
		{
			base.Clear ();

			this.ViewGraph          = true;
			this.Resolution         = 7;
			this.Count              = 26;
			this.Cumul              = true;
			this.HasStackedGraph    = true;
			this.HasSideBySideGraph = false;

			this.soldesColumns.Clear ();
			this.soldesColumns.Add (new SoldesColumn ());

			this.graphOptions.Mode = GraphMode.Lines;
			this.graphOptions.TitleText = "Soldes";
		}


		public int Resolution
		{
			set;
			get;
		}

		public int Count
		{
			set;
			get;
		}

		public bool Cumul
		{
			get;
			set;
		}

		public bool HasStackedGraph
		{
			get;
			set;
		}

		public bool HasSideBySideGraph
		{
			get;
			set;
		}

		public List<SoldesColumn> SoldesColumns
		{
			get
			{
				return this.soldesColumns;
			}
		}


		protected override void CreateEmpty()
		{
			this.emptyOptions = new SoldesOptions ();
			this.emptyOptions.SetComptaEntity (this.compta);
			this.emptyOptions.Clear ();
		}


		public override AbstractOptions CopyFrom()
		{
			var options = new SoldesOptions ();
			options.SetComptaEntity (this.compta);
			this.CopyTo (options);
			return options;
		}

		public override void CopyTo(AbstractOptions dst)
		{
			if (dst == this)
			{
				return;
			}

			var d = dst as SoldesOptions;
			d.Resolution         = this.Resolution;
			d.Count              = this.Count;
			d.Cumul              = this.Cumul;
			d.HasStackedGraph    = this.HasStackedGraph;
			d.HasSideBySideGraph = this.HasSideBySideGraph;

			d.soldesColumns.Clear ();
			foreach (var c in this.soldesColumns)
			{
				var cc = new SoldesColumn ();
				c.CopyTo (cc);
				d.soldesColumns.Add (cc);
			}

			base.CopyTo (dst);
		}

		public override bool CompareTo(AbstractOptions other)
		{
			if (!base.CompareTo (other))
			{
				return false;
			}

			var o = other as SoldesOptions;

			{
				if (this.soldesColumns.Count != o.soldesColumns.Count)
				{
					return false;
				}

				for (int i = 0; i < this.soldesColumns.Count; i++)
				{
					if (!this.soldesColumns[i].Compare (o.soldesColumns[i]))
					{
						return false;
					}
				}
			}

			return this.Resolution         == o.Resolution        &&
				   this.Count              == o.Count             &&
				   this.Cumul              == o.Cumul             &&
				   this.HasStackedGraph    == o.HasStackedGraph   &&
				   this.HasSideBySideGraph == o.HasSideBySideGraph;
		}


		public override FormattedText Summary
		{
			get
			{
				this.StartSummaryBuilder ();

				this.AppendSummaryBuilder (SoldesOptions.ResolutionToDescription (this.Resolution));
				this.AppendSummaryBuilder (string.Format ("{0}×", this.Count.ToString ()));

				if (this.Cumul)
				{
					this.AppendSummaryBuilder ("Chiffres cumulés");
				}

				if (this.ViewGraph)
				{
					this.AppendSummaryBuilder (this.graphOptions.Summary);
				}
				else
				{
					if (this.HasStackedGraph)
					{
						this.AppendSummaryBuilder ("Graphique cumulé");
					}

					if (this.HasSideBySideGraph)
					{
						this.AppendSummaryBuilder ("Graphique côte à côte");
					}
				}

				foreach (var c in this.soldesColumns)
				{
					this.AppendSummaryBuilder (c.Description);
				}

				return this.StopSummaryBuilder ();
			}
		}

		public static string ResolutionToDescription(int resolution)
		{
			return string.Format ("{0} jour{1}", resolution.ToString (), (resolution <= 1) ? "" : "s");
		}


		private readonly List<SoldesColumn>				soldesColumns;
	}
}
