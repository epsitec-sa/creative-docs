//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Finance
{
	public class DocumentPriceCalculator : IDocumentPriceCalculator
	{
		public DocumentPriceCalculator(DataContext context, BusinessDocumentEntity document)
		{
			this.context  = context;
			this.document = document;
			this.calculators = new List<AbstractPriceCalculator> ();
			this.groups = new List<GroupPriceCalculator> ();
		}

		
		public void Update()
		{
			this.currentState = State.None;
			
			this.calculators.Clear ();
			this.groups.Clear ();

			foreach (var line in this.document.Lines)
			{
				line.Process (this);
			}

			this.RecordCurrentGroup ();
			this.GenerateVatLinesAndGrandTotal ();
		}

		private void GenerateVatLinesAndGrandTotal()
		{
			var taxReservoir   = new Reservoir<TaxDocumentItemEntity> (this.context, this.document.Lines.OfType<TaxDocumentItemEntity> ());
			var totalReservoir = new Reservoir<EndTotalDocumentItemEntity> (this.context, this.document.Lines.OfType<EndTotalDocumentItemEntity> ());

			taxReservoir.Pool.ForEach (line => this.document.Lines.Remove (line));
			totalReservoir.Pool.ForEach (line => this.document.Lines.Remove (line));

			var group = this.GetLastGroup ();

			if (group == null)
			{
				return;
			}
			
			Tax taxTotals = Tax.Combine (group.TaxDiscountable, group.TaxNotDiscountable);

			if (taxTotals == null)
			{
				return;
			}
			
			var taxInfos = from tax in taxTotals.RateAmounts
							orderby tax.CodeRate ascending
							select tax;

			var currency = this.document.BillingCurrencyCode;

			foreach (var taxInfo in taxInfos)
			{
				var taxLine = taxReservoir.Pull ();

				taxLine.VatCode      = taxInfo.Code;
				taxLine.BaseAmount   = PriceCalculator.ClipPriceValue (taxInfo.Amount, currency);
				taxLine.Rate         = PriceCalculator.ClipTaxRateValue (taxInfo.Rate);
				taxLine.ResultingTax = PriceCalculator.ClipPriceValue (taxInfo.Tax, currency);

				this.document.Lines.Add (taxLine);
			}
					
			var grandTotalLine = totalReservoir.Pull ();

			grandTotalLine.PriceBeforeTax = PriceCalculator.ClipPriceValue (taxTotals.TotalAmount, currency);
			grandTotalLine.PriceAfterTax  = PriceCalculator.ClipPriceValue (taxTotals.TotalAmount + taxTotals.TotalTax, currency);

			this.document.Lines.Add (grandTotalLine);
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

		void IDocumentPriceCalculator.Process(ArticlePriceCalculator calculator)
		{
			if (this.currentState != State.Article)
			{
				this.RecordCurrentGroup ();
				this.currentState = State.Article;
				this.currentGroup = new GroupPriceCalculator ();
			}

			this.calculators.Add (calculator);
			
			calculator.ComputePrice ();
			
			this.currentGroup.Add (calculator);
		}

		void IDocumentPriceCalculator.Process(SubTotalPriceCalculator calculator)
		{
			this.calculators.Add (calculator);
			
			calculator.ComputePrice (this.currentGroup ?? new GroupPriceCalculator ());

			this.currentState = State.SubTotal;
			this.currentGroup = new GroupPriceCalculator ();
			this.currentGroup.Add (calculator);
		}

		#endregion

		private void RecordCurrentGroup()
		{
			if (this.currentGroup != null)
			{
				this.groups.Add (this.currentGroup);
				this.currentGroup = null;
			}
		}

		private GroupPriceCalculator GetLastGroup()
		{
			if (this.groups.Count == 0)
			{
				return null;
			}
			else
			{
				return this.groups[this.groups.Count-1];
			}
		}

		private readonly DataContext				context;
		private readonly BusinessDocumentEntity		document;
		private readonly List<AbstractPriceCalculator> calculators;
		private readonly List<GroupPriceCalculator>	groups;


		private GroupPriceCalculator			currentGroup;
		private State							currentState;
	}
}