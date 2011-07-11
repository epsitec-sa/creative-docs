﻿//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
		/// <param name="context">The business context.</param>
		/// <param name="document">The business document to work on.</param>
		public DocumentPriceCalculator(IBusinessContext context, BusinessDocumentEntity document, DocumentMetadataEntity metadata)
		{
			this.context     = context;
			this.document    = document;
			this.metadata    = metadata;
			this.calculators = new List<AbstractItemPriceCalculator> ();
			this.groups      = new Stack<GroupItemPriceCalculator> ();
			this.suspender   = this.document.DisableEvents ();
		}


		/// <summary>
		/// Updates the prices of the attached business document. This will most likely
		/// change the resulting prices and final prices of all lines in the invoice.
		/// This will also create lines for the taxes (VAT) and the grand total, if
		/// they are not yet present in the document.
		/// </summary>
		public void UpdatePrices()
		{
			this.SortLinesAndFixSubTotals ();
			this.ComputeLinePrices ();

			var group = this.GetLastGroup ();
			var taxes = Tax.Combine (group.TaxDiscountable, group.TaxNotDiscountable) ?? new Tax ();

			this.ReplaceTaxesAndEndTotal (taxes);
			this.ComputeFinalPrices (group, taxes);
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

			this.ReplaceTaxLines (taxReservoir, taxInfos, currency);
			this.ReplaceEndTotalLine (totalReservoir, taxTotals, currency);

			taxReservoir.DeleteUnused ();
			totalReservoir.DeleteUnused ();
		}

		private void ReplaceTaxLines(Reservoir<TaxDocumentItemEntity> reservoir, IOrderedEnumerable<TaxRateAmount> taxInfos, CurrencyCode currency)
		{
			reservoir.Pool.ForEach (line => this.document.Lines.Remove (line));

			int index = this.GetRootGroupInsertionIndex ();
			
			foreach (var taxInfo in taxInfos)
			{
				var taxLine = reservoir.Pull ();

				taxLine.Visibility     = true;
				taxLine.AutoGenerated  = true;
				taxLine.GroupIndex     = 0;

				taxLine.VatCode      = taxInfo.Code;
				taxLine.BaseAmount   = PriceCalculator.ClipPriceValue (taxInfo.Amount, currency);
				taxLine.Rate         = PriceCalculator.ClipTaxRateValue (taxInfo.Rate);
				taxLine.ResultingTax = PriceCalculator.ClipPriceValue (taxInfo.Tax, currency);

				this.document.Lines.Insert (index++, taxLine);
			}
		}

		private void ReplaceEndTotalLine(Reservoir<EndTotalDocumentItemEntity> reservoir, Tax taxTotals, CurrencyCode currency)
		{
			reservoir.Pool.ForEach (line => this.document.Lines.Remove (line));

			int index     = this.GetRootGroupInsertionIndex ();
			var totalLine = reservoir.Pull ();

			totalLine.Visibility     = true;
			totalLine.AutoGenerated  = true;
			totalLine.GroupIndex     = 0;

			totalLine.PriceBeforeTax = PriceCalculator.ClipPriceValue (taxTotals.TotalAmount, currency);
			totalLine.PriceAfterTax  = PriceCalculator.ClipPriceValue (taxTotals.TotalAmount + taxTotals.TotalTax, currency);

			this.document.Lines.Insert (index++, totalLine);
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

		public IBusinessContext					BusinessContext
		{
			get
			{
				return this.context;
			}
		}

		void IDocumentPriceCalculator.Process(ArticleItemPriceCalculator calculator)
		{
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
			this.suspender.Dispose ();
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
		

		private readonly IBusinessContext		context;
		private readonly BusinessDocumentEntity	document;
		private readonly DocumentMetadataEntity	metadata;
		private readonly List<AbstractItemPriceCalculator>	calculators;
		private readonly Stack<GroupItemPriceCalculator>	groups;
		private readonly System.IDisposable		suspender;

		private GroupItemPriceCalculator		currentGroup;
		private State							currentState;
	}
}