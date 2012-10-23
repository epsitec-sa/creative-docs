//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business.Finance;
using Epsitec.Cresus.Core.Business.Finance.PriceCalculators.ItemPriceCalculators;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Finance.PriceCalculators
{
	/// <summary>
	/// The <c>DocumentPriceCalculator</c> class implements the price calculation
	/// algorithm used to compute the total of an invoice, for instance.
	/// </summary>
	public sealed class DocumentPriceCalculator : IPriceCalculator, IDocumentPriceCalculator, System.IDisposable
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DocumentPriceCalculator"/> class.
		/// This must be used in a <c>using</c> block, or else the events on the document
		/// entity won't ever be re-enabled.
		/// </summary>
		/// <param name="businessContext">The business context.</param>
		/// <param name="document">The business document to work on.</param>
		/// <param name="metadata">The associated document metadata.</param>
		private DocumentPriceCalculator(BusinessContext businessContext, BusinessDocumentEntity document, DocumentMetadataEntity metadata)
		{
			ExceptionThrower.ThrowIfNull (businessContext, "businessContext");
			ExceptionThrower.ThrowIfNull (document, "document");
			ExceptionThrower.ThrowIfNull (metadata, "metadata");

			this.context       = businessContext;
			this.document      = document;
			this.metadata      = metadata;
			this.calculators   = new List<AbstractItemPriceCalculator> ();
			this.groups        = new Stack<GroupItemPriceCalculator> ();
			this.suspender     = this.document.DisableEvents ();
			this.roundingModes = new Dictionary<RoundingPolicy, PriceRoundingModeEntity> ();
		}


		/// <summary>
		/// Calculates the prices for the specified business document.
		/// </summary>
		/// <param name="context">The business context.</param>
		/// <param name="document">The business document to work on.</param>
		/// <param name="metadata">The metadata.</param>
		public static void Calculate(BusinessContext context, BusinessDocumentEntity document, DocumentMetadataEntity metadata)
		{
			using (var calculator = new DocumentPriceCalculator (context, document, metadata))
			{
				PriceCalculator.UpdatePrices (calculator);
			}
		}

		public static void Reset(BusinessContext context, BusinessDocumentEntity document, DocumentMetadataEntity metadata)
		{
			using (context.SuspendUpdates ())
			{
				foreach (var item in document.Lines.OfType<ArticleDocumentItemEntity> ())
				{
					item.Reset ();
				}
			}
		}

		#region IPriceCalculator Members

		/// <summary>
		/// Updates the prices of the attached business document. This will most likely
		/// change the resulting prices and final prices of all lines in the invoice.
		/// This will also create lines for the taxes (VAT) and the grand total, if
		/// they are not yet present in the document.
		/// </summary>
		public void UpdatePrices()
		{
			this.EnsureRoundingModes ();
			this.EnsureEndTotal ();
			this.SortLinesAndFixSubTotals ();
			this.ComputeLinePrices ();

			var group = this.GetLastGroup ();
			var taxes = Tax.Combine (group.TaxDiscountable, group.TaxNotDiscountable) ?? new Tax ();

			this.ReplaceTaxesAndEndTotal (taxes);
			this.ComputeFinalPrices (group, taxes);
		}

		/// <summary>
		/// Rounds the specified value according to the attached business document
		/// rounding rules.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="policy">The rounding policy.</param>
		/// <returns>
		/// The rounded value.
		/// </returns>
		public decimal Round(decimal value, RoundingPolicy policy)
		{
			PriceRoundingModeEntity rounding;

			if (this.roundingModes.TryGetValue (policy, out rounding))
			{
				return rounding.Round (value);
			}

			return value;
		}

		/// <summary>
		/// Gets the billing mode in use in the attached business document.
		/// </summary>
		/// <returns>
		/// The billing mode.
		/// </returns>
		public BillingMode GetBillingMode()
		{
			if ((this.document.IsNotNull ()) &&
				(this.document.PriceGroup.IsNotNull ()))
			{
				return this.document.PriceGroup.BillingMode;
			}
			else
			{
				return BillingMode.None;
			}
		}

		#endregion

		private void EnsureRoundingModes()
		{
			var priceGroup = this.document.PriceGroup;

			if ((priceGroup.IsNotNull ()) &&
				(priceGroup.DefaultRoundingModes.Count > 0))
			{
				priceGroup.DefaultRoundingModes.ForEach (x => this.AddRoundingMode (x));
			}
			
			var settings = this.context.GetCached<BusinessSettingsEntity> ();

			if ((settings.IsNotNull ()) &&
				(settings.Finance.IsNotNull ()) &&
				(settings.Finance.DefaultPriceGroup.IsNotNull ()))
			{
				settings.Finance.DefaultPriceGroup.DefaultRoundingModes.ForEach (x => this.AddRoundingMode (x));
			}

			var fallback1 = new PriceRoundingModeEntity ()
			{
				Modulo          = 0.05M,
				AddBeforeModulo = 0.00M,
				RoundingPolicy  = RoundingPolicy.OnLinePrice | RoundingPolicy.OnTotalPrice | RoundingPolicy.OnTotalRounding,
			};

			var fallback2 = new PriceRoundingModeEntity ()
			{
				Modulo          = 1.00M,
				AddBeforeModulo = 0.00M,
				RoundingPolicy  = RoundingPolicy.OnEndTotal,
			};

			var fallbackVat = new PriceRoundingModeEntity ()
			{
				Modulo          = 0.05M,
				AddBeforeModulo = 0.025M,
				RoundingPolicy  = RoundingPolicy.OnTotalVat,
			};

			this.AddRoundingMode (fallback1);
			this.AddRoundingMode (fallback2);
			this.AddRoundingMode (fallbackVat);
		}

		private void AddRoundingMode(PriceRoundingModeEntity rounding)
		{
			foreach (var flag in rounding.RoundingPolicy.GetFlags ())
			{
				if (this.roundingModes.ContainsKey (flag))
				{
					continue;
				}

				this.roundingModes[flag] = rounding;
			}
		}

		/// <summary>
		/// Ensures that there is an end total in the document. This is a required element for
		/// the price calculator to work correctly.
		/// </summary>
		private void EnsureEndTotal()
		{
			if (this.document.Lines.Any (x => x is EndTotalDocumentItemEntity))
			{
				return;
			}

			var endTotal = this.BusinessContext.CreateEntity<EndTotalDocumentItemEntity> ();

			endTotal.Attributes = DocumentItemAttributes.AutoGenerated;
			endTotal.GroupIndex = 0;

			this.document.Lines.Add (endTotal);
		}

		private void SortLinesAndFixSubTotals()
		{
			var lines = new List<AbstractDocumentItemEntity> ();
			
			LineTreeSorter.Sort (this.document.Lines, lines);
			LineTreeAnalyzer.FixSubTotals (this.context, lines);

			if (!Comparer.EqualObjects (lines, this.document.Lines))
			{
				this.ReplaceDocumentLines (lines);
			}
		}

		private void ReplaceDocumentLines(IEnumerable<AbstractDocumentItemEntity> lines)
		{
			this.document.Lines.Clear ();
			this.document.Lines.AddRange (lines);
		}

		private void ComputeLinePrices()
		{
			this.Reset ();

			foreach (var line in this.document.Lines)
			{
				//	Indirectly call into our implementation of IDocumentPriceCalculator.Process,
				//	based on the type of the line :

				line.Process (this);
			}

			this.RecordCurrentGroup ();
		}

		private void ComputeFinalPrices(GroupItemPriceCalculator group, Tax taxTotals)
		{
			group.AdjustFinalPrices (taxTotals.TotalAmount);
		}

		private void Reset()
		{
			this.currentState = State.None;
			this.calculators.Clear ();
			this.groups.Clear ();
		}

		private void ReplaceTaxesAndEndTotal(Tax taxTotals)
		{
			var dataContext    = this.context.DataContext;
			var taxReservoir   = new Reservoir<TaxDocumentItemEntity> (dataContext, this.document.Lines.OfType<TaxDocumentItemEntity> ());
			var totalReservoir = new Reservoir<EndTotalDocumentItemEntity> (dataContext, this.document.Lines.OfType<EndTotalDocumentItemEntity> ());

			var taxInfos = taxTotals.RateAmounts.OrderBy (x => x.CodeRate);
			var currency = this.document.CurrencyCode;

			var vatRounding = this.ReplaceTaxLines (taxReservoir, taxInfos, currency);
			
			this.ReplaceEndTotalLine (totalReservoir, taxTotals, vatRounding, currency);

			taxReservoir.DeleteUnused ();
			totalReservoir.DeleteUnused ();
		}

		private decimal ReplaceTaxLines(Reservoir<TaxDocumentItemEntity> reservoir, IOrderedEnumerable<TaxRateAmount> taxInfos, CurrencyCode currency)
		{
			decimal roundingTotal = 0;

			reservoir.Pool.ForEach (line => this.document.Lines.Remove (line));

			int index = this.GetRootGroupInsertionIndex ();
			
			foreach (var taxInfo in taxInfos)
			{
				var taxLine = reservoir.Pull ();

				taxLine.Attributes   = DocumentItemAttributes.AutoGenerated;
				taxLine.GroupIndex   = 0;
				taxLine.VatRateType  = taxInfo.VatRateType;
				taxLine.TotalRevenue = PriceCalculator.ClipPriceValue (taxInfo.Amount, currency);
				taxLine.VatRate      = PriceCalculator.ClipTaxRateValue (taxInfo.Rate);
				taxLine.ResultingTax = PriceCalculator.ClipPriceValue (taxInfo.Tax, currency);

				roundingTotal += this.RoundTaxLine (taxLine);

				this.document.Lines.Insert (index++, taxLine);
			}

			return roundingTotal;
		}

		private void ReplaceEndTotalLine(Reservoir<EndTotalDocumentItemEntity> reservoir, Tax taxTotals, decimal vatRounding, CurrencyCode currency)
		{
			reservoir.Pool.ForEach (line => this.document.Lines.Remove (line));

			int index     = this.GetRootGroupInsertionIndex ();
			var totalLine = reservoir.Pull ();

			totalLine.Attributes = DocumentItemAttributes.AutoGenerated;
			totalLine.GroupIndex = 0;

			switch (this.GetBillingMode ())
			{
				case BillingMode.ExcludingTax:
					totalLine.PriceBeforeTax = PriceCalculator.ClipPriceValue (taxTotals.TotalAmount, currency);
					totalLine.PriceAfterTax  = PriceCalculator.ClipPriceValue (taxTotals.TotalAmount + taxTotals.TotalTax + vatRounding, currency);
					break;

				case BillingMode.IncludingTax:
					totalLine.PriceBeforeTax = PriceCalculator.ClipPriceValue (taxTotals.TotalAmount, currency);
					totalLine.PriceAfterTax  = PriceCalculator.ClipPriceValue (taxTotals.TotalAmount + taxTotals.TotalTax, currency);
					break;
			}

			this.RoundEndTotal (totalLine);

			this.document.Lines.Insert (index++, totalLine);
		}

		private decimal RoundTaxLine(TaxDocumentItemEntity taxLine)
		{
			var beforeRounding = taxLine.ResultingTax;
			var afterRounding  = this.Round (beforeRounding, RoundingPolicy.OnTotalVat);
			var totalRounding  = afterRounding - beforeRounding;

			if (totalRounding == 0)
			{
				return 0;
			}

			taxLine.ResultingTax = afterRounding;

			if (this.GetBillingMode () == BillingMode.IncludingTax)
			{
				taxLine.TotalRevenue -= totalRounding;
			}

			return totalRounding;
		}

		private void RoundEndTotal(EndTotalDocumentItemEntity totalLine)
		{
			var beforeRounding = totalLine.PriceAfterTax.GetValueOrDefault ();
			var afterRounding  = this.Round (beforeRounding, RoundingPolicy.OnEndTotal);

			if (afterRounding == beforeRounding)
			{
				totalLine.TotalRounding = null;
				return;
			}

			totalLine.PriceAfterTax = afterRounding;
			totalLine.TotalRounding = afterRounding - beforeRounding;
		}

		private int GetRootGroupInsertionIndex()
		{
			int index = this.document.Lines.Count ();

			while (index > 0)
			{
				var line = this.document.Lines[index-1];

				if (line.GroupIndex > 0)
				{
					return index;
				}

				if (line is TextDocumentItemEntity)
				{
					//	Continue
				}
				else
				{
					return index;
				}

				index--;
			}

			return index;
		}


		#region IDocumentPriceCalculator Members

		public BusinessDocumentEntity			Document
		{
			get
			{
				return this.document;
			}
		}

		public DocumentMetadataEntity			Metadata
		{
			get
			{
				return this.metadata;
			}
		}

		public CoreData							Data
		{
			get
			{
				return this.context.Data;
			}
		}

		public BusinessContext					BusinessContext
		{
			get
			{
				return this.context;
			}
		}

		void IDocumentPriceCalculator.Process(ArticleItemPriceCalculator calculator)
		{
			if (calculator.ProFormaOnly)
			{
				calculator.ComputePrice ();
				return;
			}

			int groupLevel = calculator.ArticleItem.GroupLevel;

			if (this.currentState != State.Article)
			{
				//	We reached the first article line of a new group. Record the current
				//	group and create a new one for the current level.

				this.RecordCurrentGroup ();

				this.currentState = State.Article;
				this.currentGroup = new GroupItemPriceCalculator (groupLevel);
			}

			System.Diagnostics.Debug.Assert (this.currentGroup != null);
			// TODO: [DR] Pour une raison que j'ignore, cet Assert arrive fréquemment !
			System.Diagnostics.Debug.Assert (this.currentGroup.GroupLevel == groupLevel);

			this.calculators.Add (calculator);
			
			calculator.ComputePrice ();
			
			this.currentGroup.IncludeSubGroups (this, groupLevel);
			this.currentGroup.Add (calculator);
		}

		void IDocumentPriceCalculator.Process(SubTotalItemPriceCalculator calculator)
		{
			this.calculators.Add (calculator);

			int groupLevel = calculator.Item.GroupLevel;
			
			System.Diagnostics.Debug.Assert (this.currentGroup != null);

			if (this.currentGroup.GroupLevel > groupLevel)
			{
				//	Summing groups of higher level into a new lower-level group. Add the
				//	groups of the higher level into the new local group, as if they were
				//	simple article lines.

				this.RecordCurrentGroup ();

				this.currentGroup = new GroupItemPriceCalculator (groupLevel);
				this.currentGroup.IncludeSubGroups (this, groupLevel);
			}

			calculator.ComputePrice (this.currentGroup);

			this.currentState = State.SubTotal;
			this.currentGroup = new GroupItemPriceCalculator (groupLevel);
			this.currentGroup.Add (calculator);
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			if (this.suspender != null)
			{
				this.suspender.Dispose ();
			}
		}

		#endregion


		private void RecordCurrentGroup()
		{
			if (this.currentGroup != null)
			{
				this.groups.Push (this.currentGroup);
				this.currentGroup = null;
			}
		}

		private GroupItemPriceCalculator GetLastGroup()
		{
			if (this.groups.Count == 0)
			{
				return new GroupItemPriceCalculator (0);
			}
			else
			{
				return this.groups.Peek ();
			}
		}

		internal void IncludeSubGroups(GroupItemPriceCalculator localGroup, int groupLevel)
		{
			while (this.groups.Count > 0 && this.groups.Peek ().GroupLevel > groupLevel)
			{
				localGroup.Add (this.groups.Pop ());
			}
		}
		

		private readonly BusinessContext										context;
		private readonly BusinessDocumentEntity									document;
		private readonly DocumentMetadataEntity									metadata;
		private readonly List<AbstractItemPriceCalculator>						calculators;
		private readonly Stack<GroupItemPriceCalculator>						groups;
		private readonly System.IDisposable										suspender;
		private readonly Dictionary<RoundingPolicy, PriceRoundingModeEntity>	roundingModes;
		private GroupItemPriceCalculator										currentGroup;
		private State															currentState;
	}
}