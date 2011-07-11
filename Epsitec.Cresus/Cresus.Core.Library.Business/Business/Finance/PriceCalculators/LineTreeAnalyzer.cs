//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Finance.PriceCalculators
{
	internal sealed class LineTreeAnalyzer
	{
		private LineTreeAnalyzer(IBusinessContext context, IList<AbstractDocumentItemEntity> lines)
		{
			this.context = context;
			this.lines = lines;
			this.stack = new Stack<State> ();
		}

		public static void FixSubTotals(IBusinessContext context, IList<AbstractDocumentItemEntity> lines)
		{
			var analyzer = new LineTreeAnalyzer (context, lines);
			analyzer.FixSubTotals ();
		}

		private void FixSubTotals()
		{
			this.state = State.None;
			
			for (this.lineIndex = 0; this.lineIndex < this.lines.Count; this.lineIndex++)
			{
				var line = lines[this.lineIndex];

				this.currentGroupIndex = line.GroupIndex;
				this.currentGroupLevel = line.GroupLevel;

				if (this.activeGroupIndex != this.currentGroupIndex)
				{
					this.SwitchGroups ();
				}

				this.UpdateStateAndRemoveUselessSubTotals (line);
			}

			System.Diagnostics.Debug.Assert (this.activeGroupLevel == 0);
			System.Diagnostics.Debug.Assert (this.activeGroupIndex == 0);
			System.Diagnostics.Debug.Assert (this.stack.Count == 0);
		}

		private void SwitchGroups()
		{
			//	First, make sure we are operating in the same parent as before

			int commonLevel = LineTreeAnalyzer.GetCommonLevel (this.activeGroupIndex, this.currentGroupIndex);

			this.RewindToGroupLevel (commonLevel);
			this.EnterCurrentGroup ();
			
			System.Diagnostics.Debug.Assert (this.activeGroupIndex == this.currentGroupIndex);
			System.Diagnostics.Debug.Assert (this.activeGroupLevel == this.currentGroupLevel);
		}

		private void RewindToGroupLevel(int level)
		{
			while (this.activeGroupLevel > level)
			{
				switch (this.state)
				{
					case State.Article:
						this.AddMissingSubTotal (this.activeGroupIndex);
						this.stack.Pop ();
						this.state = State.Article;
						break;

					case State.SubTotal:
						this.stack.Pop ();
						this.state = State.Article;
						break;

					case State.None:
						this.state = this.stack.Pop ();
						break;
				}

				this.activeGroupLevel--;
				this.activeGroupIndex = this.activeGroupIndex / 100;
			}
		}

		private void EnterCurrentGroup()
		{
			while (this.activeGroupLevel < this.currentGroupLevel)
			{
				this.activeGroupLevel++;

				this.stack.Push (this.state);
				this.state = State.None;
			}

			this.activeGroupIndex = this.currentGroupIndex;
		}
		
		private void AddMissingSubTotal(int groupIndex)
		{
			var line = this.context.CreateEntity<SubTotalDocumentItemEntity> ();

			line.GroupIndex = groupIndex;
			line.TextForPrimaryPrice   = "Sous-total avant rabais";
			line.TextForResultingPrice = "Sous-total";

			this.lines.Insert (this.lineIndex++, line);
		}

		private void UpdateStateAndRemoveUselessSubTotals(AbstractDocumentItemEntity line)
		{
			if (line is ArticleDocumentItemEntity)
			{
				this.state = State.Article;
			}
			else if (line is SubTotalDocumentItemEntity)
			{
				if (this.state == State.None)
				{
					//	Reached a subtotal which does not sum any items. It must be removed
					//	from the collection of lines.

					this.lines.RemoveAt (this.lineIndex--);
				}
				else
				{
					this.state = State.SubTotal;
				}
			}
		}

		private static int GetCommonLevel(int groupA, int groupB)
		{
			int level = 0;
			int mask  = 100;

			while ((groupA % mask) == (groupB % mask))
			{
				mask  = mask * 100;
				level = level + 1;
			}

			return level;
		}

		private readonly IBusinessContext context;
		private readonly IList<AbstractDocumentItemEntity> lines;
		private readonly Stack<State> stack;

		private State state;
		private int lineIndex;
		private int activeGroupIndex;
		private int activeGroupLevel;
		private int currentGroupIndex;
		private int currentGroupLevel;
	}
}
