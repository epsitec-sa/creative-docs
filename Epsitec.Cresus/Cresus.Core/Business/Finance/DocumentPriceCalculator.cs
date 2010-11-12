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
		}

		enum State
		{
			None,
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
			if (this.currentState != State.Article)
			{
				if (this.currentGroup != null)
				{
					this.groups.Add (this.currentGroup);
				}

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

			this.currentState = State.Total;
			this.currentGroup = new GroupPriceCalculator ();
			this.currentGroup.Add (calculator);
		}

		#endregion
		
		private readonly BusinessDocumentEntity		document;
		private readonly List<AbstractPriceCalculator> calculators;
		private readonly List<GroupPriceCalculator>	groups;


		private GroupPriceCalculator			currentGroup;
		private State							currentState;
	}
}