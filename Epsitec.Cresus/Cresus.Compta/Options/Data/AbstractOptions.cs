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
		public AbstractOptions()
		{
			this.graphOptions = new GraphOptions ();
		}

		public void SetComptaEntity(ComptaEntity compta)
		{
			this.compta = compta;
		}


		public virtual void Clear()
		{
			this.ViewGraph             = false;
			this.ZeroFiltered          = true;
			this.ZeroDisplayedInWhite  = true;
			this.HasGraphicColumn      = false;
			this.DeepFrom              = 1;
			this.DeepTo                = int.MaxValue;
			this.Catégories            = CatégorieDeCompte.Tous;
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


		public bool Specialist
		{
			get;
			set;
		}


		public bool ViewGraph
		{
			//	Affiche le graphe à la place du tableau ?
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


		public bool ZeroFiltered
		{
			//	Filtre les montants nuls (qui ne sont alors plus affichés) ?
			get;
			set;
		}

		public bool ZeroDisplayedInWhite
		{
			//	Affiche en blanc les montants nuls ?
			get;
			set;
		}

		public bool HasGraphicColumn
		{
			//	Affiche une colonne avec un graphe simplifié ?
			get;
			set;
		}

		public int DeepFrom
		{
			//	Affiche les comptes depuis cette profondeur.
			get;
			set;
		}

		public int DeepTo
		{
			//	Affiche les comptes jusqu'à cette profondeur.
			get;
			set;
		}

		public CatégorieDeCompte Catégories
		{
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
			if (dst == this)
			{
				return;
			}

			dst.ViewGraph             = this.ViewGraph;
			dst.ZeroFiltered          = this.ZeroFiltered;
			dst.ZeroDisplayedInWhite  = this.ZeroDisplayedInWhite;
			dst.HasGraphicColumn      = this.HasGraphicColumn;
			dst.DeepFrom              = this.DeepFrom;
			dst.DeepTo                = this.DeepTo;
			dst.Catégories            = this.Catégories;
			dst.ComparisonEnable      = this.ComparisonEnable;
			dst.ComparisonShowed      = this.ComparisonShowed;
			dst.ComparisonDisplayMode = this.ComparisonDisplayMode;

			this.graphOptions.CopyTo (dst.graphOptions);
		}

		public virtual bool CompareTo(AbstractOptions other)
		{
			if (this.ViewGraph            != other.ViewGraph            ||
				this.ZeroFiltered         != other.ZeroFiltered         ||
				this.ZeroDisplayedInWhite != other.ZeroDisplayedInWhite ||
				this.HasGraphicColumn     != other.HasGraphicColumn     ||
				this.DeepFrom             != other.DeepFrom             ||
				this.DeepTo               != other.DeepTo               ||
				this.Catégories           != other.Catégories           ||
				this.ComparisonEnable != other.ComparisonEnable         ||
				!this.graphOptions.CompareTo (other.graphOptions))
			{
				return false;
			}

			if (this.ComparisonEnable)
			{
				if (this.ComparisonShowed      != other.ComparisonShowed      ||
					this.ComparisonDisplayMode != other.ComparisonDisplayMode)
				{
					return false;
				}
			}

			return true;
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
