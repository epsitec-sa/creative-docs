﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;

using Epsitec.Cresus.Core.Business.Finance;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Finance.PriceCalculators
{
	/// <summary>
	/// The <c>DocumentPriceCalculator</c> class implements the price calculation
	/// algorithm used to compute the total of an invoice, for instance.
	/// </summary>
	public sealed class DocumentPriceCalculator : IDocumentPriceCalculator, System.IDisposable
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DocumentPriceCalculator"/> class.
		/// </summary>
		/// <param name="context">The business context.</param>
		/// <param name="document">The business document to work on.</param>
		public DocumentPriceCalculator(IBusinessContext context, BusinessDocumentEntity document)
		{
			this.context     = context;
			this.document    = document;
			this.calculators = new List<AbstractPriceCalculator> ();
			this.groups      = new Stack<GroupPriceCalculator> ();
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
			this.SortLines ();
			this.ComputeLinePrices ();

			var group = this.GetLastGroup ();
			var taxes = Tax.Combine (group.TaxDiscountable, group.TaxNotDiscountable) ?? new Tax ();

			this.ComputeTaxesAndEndTotal (group, taxes);
			this.ComputeFinalPrices (group, taxes);
		}

		
		private void SortLines()
		{
			List<AbstractDocumentItemEntity> lines = new List<AbstractDocumentItemEntity> ();

			lines.AddRange (this.document.Lines.Where (x => x.GroupLevel > 0));
			lines.AddRange (this.document.Lines.Where (x => x.GroupLevel == 0 && !x.AutoGenerated));
			lines.AddRange (this.document.Lines.Where (x => x.GroupLevel == 0 && x.AutoGenerated));

			if (!Comparer.EqualObjects (lines, this.document.Lines))
			{
				this.document.Lines.Clear ();
				this.document.Lines.AddRange (lines);
			}
		}

		private void ComputeLinePrices()
		{
			this.currentState = State.None;

			this.calculators.Clear ();
			this.groups.Clear ();

			foreach (var line in this.document.Lines)
			{
				line.Process (this);
			}

			this.RecordCurrentGroup ();
		}

		private void ComputeTaxesAndEndTotal(GroupPriceCalculator group, Tax taxTotals)
		{
			DataContext context = this.context.DataContext;
			
			var taxReservoir   = new Reservoir<TaxDocumentItemEntity> (context, this.document.Lines.OfType<TaxDocumentItemEntity> ());
			var totalReservoir = new Reservoir<EndTotalDocumentItemEntity> (context, this.document.Lines.OfType<EndTotalDocumentItemEntity> ());

			var taxInfos = from tax in taxTotals.RateAmounts
						   orderby tax.CodeRate ascending
						   select tax;

			var currency = this.document.CurrencyCode;

			this.GenerateVatLines (taxReservoir, taxInfos, currency);
			this.GenerateEndTotalLine (totalReservoir, taxTotals, currency);
		}

		private void ComputeFinalPrices(GroupPriceCalculator group, Tax taxTotals)
		{
			group.AdjustFinalPrices (taxTotals.TotalAmount);
		}

		private void GenerateVatLines(Reservoir<TaxDocumentItemEntity> reservoir, IOrderedEnumerable<TaxRateAmount> taxInfos, CurrencyCode currency)
		{
			reservoir.Pool.ForEach (line => this.document.Lines.Remove (line));
			
			foreach (var taxInfo in taxInfos)
			{
				var taxLine = reservoir.Pull ();

				taxLine.Visibility     = true;
				taxLine.AutoGenerated  = true;
//				taxLine.LayoutSettings = null;
				taxLine.GroupLevel     = 0;
				taxLine.GroupIndex     = 0;

				taxLine.VatCode      = taxInfo.Code;
				taxLine.BaseAmount   = PriceCalculator.ClipPriceValue (taxInfo.Amount, currency);
				taxLine.Rate         = PriceCalculator.ClipTaxRateValue (taxInfo.Rate);
				taxLine.ResultingTax = PriceCalculator.ClipPriceValue (taxInfo.Tax, currency);

				this.document.Lines.Add (taxLine);
			}
		}

		private void GenerateEndTotalLine(Reservoir<EndTotalDocumentItemEntity> reservoir, Tax taxTotals, CurrencyCode currency)
		{
			reservoir.Pool.ForEach (line => this.document.Lines.Remove (line));
			
			var totalLine = reservoir.Pull ();

			totalLine.Visibility     = true;
			totalLine.AutoGenerated  = true;
//			totalLine.LayoutSettings = null;
			totalLine.GroupLevel     = 0;
			totalLine.GroupIndex     = 0;

			totalLine.PriceBeforeTax = PriceCalculator.ClipPriceValue (taxTotals.TotalAmount, currency);
			totalLine.PriceAfterTax  = PriceCalculator.ClipPriceValue (taxTotals.TotalAmount + taxTotals.TotalTax, currency);

			this.document.Lines.Add (totalLine);
		}

		#region Reservoir Class

		class Reservoir<T>
			where T : AbstractEntity, new ()
		{
			public Reservoir(DataContext context, IEnumerable<T> source)
			{
				this.context = context;
				this.pool = new Queue<T> (source);
			}

			public IEnumerable<T> Pool
			{
				get
				{
					return this.pool;
				}
			}

			public T Pull()
			{
				if (this.pool.Count > 0)
				{
					return this.pool.Dequeue ();
				}
				else
				{
					return this.context.CreateEntity<T> ();
				}
			}


			private readonly Queue<T> pool;
			private readonly DataContext context;
		}

		#endregion

		#region State Enum
		
		enum State
		{
			None,
			Article,
			SubTotal,
		}

#endregion

		#region IDocumentPriceCalculator Members

		public BusinessDocumentEntity Document
		{
			get
			{
				return this.document;
			}
		}

		public CoreData Data
		{
			get
			{
				return this.context.Data;
			}
		}

		void IDocumentPriceCalculator.Process(ArticlePriceCalculator calculator)
		{
			int groupLevel = calculator.ArticleItem.GroupLevel;

			if (this.currentState != State.Article)
			{
				//	We reached the first article line of a new group. Record the current
				//	group and create a new one for the current level.

				this.RecordCurrentGroup ();
				this.currentState = State.Article;
				this.currentGroup = new GroupPriceCalculator (groupLevel);
			}

			System.Diagnostics.Debug.Assert (this.currentGroup != null);
			System.Diagnostics.Debug.Assert (this.currentGroup.GroupLevel == groupLevel);

			if (this.currentGroup.GroupLevel != groupLevel)
			{
				//	TODO: implement multi-level articles ? or add a foolproof system in the UI to avoid that the user creates such invalid constructs...
				throw new System.NotImplementedException ("Found articles of different group levels without summarizing sub-totals");
			}

			this.calculators.Add (calculator);
			
			calculator.ComputePrice ();
			
			this.currentGroup.Add (calculator);
			this.IncludeSubgroups (this.currentGroup, groupLevel);
		}

		void IDocumentPriceCalculator.Process(SubTotalPriceCalculator calculator)
		{
			this.calculators.Add (calculator);

			int groupLevel = calculator.Item.GroupLevel;
			var localGroup = this.currentGroup;

			if (localGroup == null)
			{
				localGroup = new GroupPriceCalculator (groupLevel);
			}
			else if (localGroup.GroupLevel > groupLevel)
			{
				//	Summing groups of higher level into a new lower-level group. Add the
				//	groups of the higher level into the new local group, as if they were
				//	simple article lines.

				this.groups.Push (localGroup);

				localGroup = new GroupPriceCalculator (groupLevel);

				this.IncludeSubgroups (localGroup, groupLevel);
			}

			calculator.ComputePrice (localGroup);

			this.currentState = State.SubTotal;
			this.currentGroup = new GroupPriceCalculator (groupLevel);
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


		private GroupPriceCalculator GetLastGroup()
		{
			if (this.groups.Count == 0)
			{
				return new GroupPriceCalculator (0);
			}
			else
			{
				return this.groups.Peek ();
			}
		}

		private void IncludeSubgroups(GroupPriceCalculator localGroup, int groupLevel)
		{
			while (this.groups.Count > 0 && this.groups.Peek ().GroupLevel > groupLevel)
			{
				localGroup.Add (this.groups.Pop ());
			}
		}
		
		private readonly IBusinessContext				context;
		private readonly BusinessDocumentEntity			document;
		private readonly List<AbstractPriceCalculator>	calculators;
		private readonly Stack<GroupPriceCalculator>	groups;
		private readonly System.IDisposable				suspender;

		private GroupPriceCalculator			currentGroup;
		private State							currentState;
	}
}