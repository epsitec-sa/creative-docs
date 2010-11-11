//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Finance
{
	public class DocumentPriceCalculator : IDocumentPriceCalculator
	{
		public DocumentPriceCalculator(BusinessDocumentEntity document)
		{
			this.document = document;
			this.calculators = new List<AbstractPriceCalculator> ();
		}

		
		public void Update()
		{
			this.currentState = State.Article;

			foreach (var line in this.document.Lines)
			{
				line.Process (this);
			}
		}

		enum State
		{
			Article,
			Total,
		}
		

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
			this.calculators.Add (calculator);
			calculator.ComputePrice ();
			
			if (this.currentGroup == null)
			{
				this.currentGroup = new GroupPriceCalculator ();
			}

			this.currentGroup.Add (calculator);

			var item  = calculator.ArticleItem;
			var value = item.ResultingLinePriceBeforeTax.Value;
			var tax   = PriceCalculator.Sum (item.ResultingLineTax1, item.ResultingLineTax2);

			this.currentGroup.Accumulate (value, tax, item.NeverApplyDiscount);
		}

		void IDocumentPriceCalculator.Process(SubTotalPriceCalculator calculator)
		{
			this.calculators.Add (calculator);
			calculator.ComputePrice (this.currentGroup);

			this.currentGroup = new GroupPriceCalculator ();
			this.currentGroup.Add (calculator);
		}

		#endregion
		
		private readonly BusinessDocumentEntity		document;
		private readonly List<AbstractPriceCalculator> calculators;


		private GroupPriceCalculator currentGroup;
		private SubTotalPriceCalculator lastPriceCalculator;
		
		private State currentState;
	}

	public interface IDocumentPriceCalculator
	{
		BusinessDocumentEntity Document
		{
			get;
		}
		void Process(ArticlePriceCalculator calculator);
		void Process(SubTotalPriceCalculator calculator);
	}
}
