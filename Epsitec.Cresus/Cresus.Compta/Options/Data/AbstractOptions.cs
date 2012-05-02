//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Helpers;
using Epsitec.Cresus.Compta.Graph;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Options.Data
{
	/// <summary>
	/// Cette classe décrit les options d'affichage génériques de la comptabilité.
	/// </summary>
	public abstract class AbstractOptions : ISettingsData
	{
		public virtual void SetComptaEntity(ComptaEntity compta)
		{
			this.compta = compta;

			this.graphOptions = new GraphOptions ();
		}


		public virtual void Clear()
		{
			this.ViewGraph             = false;
			this.ComparisonEnable      = false;
			this.ComparisonShowed      = ComparisonShowed.None;
			this.ComparisonDisplayMode = ComparisonDisplayMode.Montant;
			this.graphOptions.Clear ();
		}


		public GraphOptions GraphOptions
		{
			get
			{
				return this.graphOptions;
			}
		}


		public bool ViewGraph
		{
			get;
			set;
		}

		public bool Specialist
		{
			get;
			set;
		}

		public int GraphShowLevel
		{
			//	0 = options graphiques cachées
			//	1 = options graphiques communes
			//	2 = options graphiques détaillées
			get;
			set;
		}


		public bool ComparisonEnable
		{
			get;
			set;
		}

		public ComparisonShowed ComparisonShowed
		{
			get;
			set;
		}

		public int ComparisonShowedCount
		{
			get
			{
				int count = 0;

				if (this.ComparisonEnable)
				{
					if ((this.ComparisonShowed & ComparisonShowed.PériodePénultième) != 0)
					{
						count++;
					}

					if ((this.ComparisonShowed & ComparisonShowed.PériodePrécédente) != 0)
					{
						count++;
					}

					if ((this.ComparisonShowed & ComparisonShowed.Budget) != 0)
					{
						count++;
					}

					if ((this.ComparisonShowed & ComparisonShowed.BudgetProrata) != 0)
					{
						count++;
					}

					if ((this.ComparisonShowed & ComparisonShowed.BudgetFutur) != 0)
					{
						count++;
					}

					if ((this.ComparisonShowed & ComparisonShowed.BudgetFuturProrata) != 0)
					{
						count++;
					}
				}

				return count;
			}
		}

		public ComparisonDisplayMode ComparisonDisplayMode
		{
			get;
			set;
		}


		public bool IsEmpty
		{
			get
			{
				if (this.emptyOptions == null)
				{
					this.CreateEmpty ();
				}

				if (this.emptyOptions == null)
				{
					return false;
				}
				else
				{
					return this.CompareTo (this.emptyOptions);
				}
			}
		}

		protected virtual void CreateEmpty()
		{
		}


		public virtual AbstractOptions CopyFrom()
		{
			return null;
		}

		public virtual void CopyTo(AbstractOptions dst)
		{
			dst.ViewGraph             = this.ViewGraph;
			dst.ComparisonEnable      = this.ComparisonEnable;
			dst.ComparisonShowed      = this.ComparisonShowed;
			dst.ComparisonDisplayMode = this.ComparisonDisplayMode;

			this.graphOptions.CopyTo (dst.graphOptions);
		}

		public virtual bool CompareTo(AbstractOptions other)
		{
			return this.ViewGraph             == other.ViewGraph             &&
				   this.ComparisonEnable      == other.ComparisonEnable      &&
				   this.ComparisonShowed      == other.ComparisonShowed      &&
				   this.ComparisonDisplayMode == other.ComparisonDisplayMode &&
				   this.graphOptions.CompareTo (other.graphOptions);
		}


		public virtual FormattedText Summary
		{
			get
			{
				return FormattedText.Empty;
			}
		}


		protected FormattedText ComparisonSummary
		{
			get
			{
				if (this.ComparisonEnable)
				{
					var s = Converters.GetComparisonShowedNiceDescription (this.ComparisonShowed);
					var d = Converters.GetComparisonDisplayModeDescription (this.ComparisonDisplayMode);

					return string.Format ("Comparaison avec \"{0}\" mode \"{1}\"", s, d);
				}
				else
				{
					return FormattedText.Empty;
				}
			}
		}


		protected void StartSummaryBuilder()
		{
			System.Diagnostics.Debug.Assert (this.summaryBuilder == null);

			this.summaryBuilder = new System.Text.StringBuilder ();
			this.firstSummary = true;
		}

		protected void AppendSummaryBuilder(FormattedText text)
		{
			System.Diagnostics.Debug.Assert (this.summaryBuilder != null);

			if (!text.IsNullOrEmpty)
			{
				if (!this.firstSummary)
				{
					this.summaryBuilder.Append (", ");
				}

				this.summaryBuilder.Append (text);
				this.firstSummary = false;
			}
		}

		protected FormattedText StopSummaryBuilder()
		{
			System.Diagnostics.Debug.Assert (this.summaryBuilder != null);

			var result = this.summaryBuilder.ToString ();
			this.summaryBuilder = null;
			return result;
		}


		protected ComptaEntity						compta;
		protected AbstractOptions					emptyOptions;
		protected System.Text.StringBuilder			summaryBuilder;
		protected bool								firstSummary;
		protected GraphOptions						graphOptions;
	}
}
