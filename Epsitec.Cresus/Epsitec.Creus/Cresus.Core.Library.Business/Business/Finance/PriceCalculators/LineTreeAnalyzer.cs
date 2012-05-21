//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Finance.PriceCalculators
{
	/// <summary>
	/// The <c>LineTreeAnalyzer</c> class is used to analyze the lines of a business document
	/// and possibily add or remove sub-total lines, as required.
	/// </summary>
	internal sealed class LineTreeAnalyzer
	{
		private LineTreeAnalyzer(IBusinessContext context, IList<AbstractDocumentItemEntity> lines)
		{
			this.context = context;
			this.lines = lines;
			this.stack = new Stack<State> ();
		}


		/// <summary>
		/// Fixes the sub-totals, by either adding or removing them on a group by group
		/// base.
		/// </summary>
		/// <param name="context">The business context.</param>
		/// <param name="lines">The collection of lines which has to be fixed.</param>
		public static void FixSubTotals(IBusinessContext context, IList<AbstractDocumentItemEntity> lines)
		{
			var analyzer = new LineTreeAnalyzer (context, lines);
			
			analyzer.FixSubTotals ();
			analyzer.FixFooterSection ();
		}


		/// <summary>
		/// Fixes the sub-totals found in the various line groups, but don't touch the
		/// footer (group zero) which needs some special consideration.
		/// </summary>
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

				if (this.currentGroupIndex == 0)
				{
					break;
				}

				this.UpdateStateAndRemoveUselessSubTotals (line);
			}

			System.Diagnostics.Debug.Assert (this.activeGroupLevel == 0);
			System.Diagnostics.Debug.Assert (this.activeGroupIndex == 0);
			System.Diagnostics.Debug.Assert (this.stack.Count == 0);
		}

		/// <summary>
		/// Fixes the footer section by adding/removing sub-totals in group zero.
		/// </summary>
		private void FixFooterSection()
		{
			bool foundArticle  = false;
			bool foundSubTotal = false;

			for (this.lineIndex = 0; this.lineIndex < this.lines.Count; this.lineIndex++)
			{
				var line = lines[this.lineIndex];

				foundArticle = foundArticle || LineTreeAnalyzer.IsArticle (line);
				
				if (line.GroupIndex == 0)
				{
					if (LineTreeAnalyzer.IsSubTotal (line))
					{
						//	If there are any articles in the document, then we will have to
						//	make sure that there is at least on terminal sub-total; if there
						//	are no articles at all, remove any sub-total...

						if (foundArticle)
						{
							foundSubTotal = true;
						}
						else
						{
							this.lines.RemoveAt (this.lineIndex--);
						}
					}
					else if (LineTreeAnalyzer.IsTax (line))
					{
						//	If we reach the VAT lines without having found a sub-total, add
						//	one, or else we won't have a properly formatted invoice.

						if ((foundSubTotal == false) &&
							(foundArticle))
						{
							this.AddFooterSubTotal ();
						}

						break;
					}
				}
			}
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

		private void AddFooterSubTotal()
		{
			var line = this.context.CreateEntity<SubTotalDocumentItemEntity> ();

			line.GroupIndex = 0;
			line.TextForPrimaryPrice   = "Total avant rabais";
			line.TextForResultingPrice = "Total";

			this.lines.Insert (this.lineIndex++, line);
		}

		private void UpdateStateAndRemoveUselessSubTotals(AbstractDocumentItemEntity line)
		{
			if (LineTreeAnalyzer.IsArticle (line))
			{
				this.state = State.Article;
			}
			else if (LineTreeAnalyzer.IsSubTotal (line))
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

		private static bool IsSubTotal(AbstractDocumentItemEntity line)
		{
			return (line is SubTotalDocumentItemEntity);
		}

		private static bool IsTax(AbstractDocumentItemEntity line)
		{
			return (line is TaxDocumentItemEntity);
		}

		private static bool IsArticle(AbstractDocumentItemEntity line)
		{
			return (line is ArticleDocumentItemEntity)
				&& (line.Attributes.HasFlag (DocumentItemAttributes.ProFormaOnly) == false);
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

		
		private readonly IBusinessContext		context;
		private readonly IList<AbstractDocumentItemEntity> lines;
		private readonly Stack<State>			stack;

		private State							state;
		private int								lineIndex;
		private int								activeGroupIndex;
		private int								activeGroupLevel;
		private int								currentGroupIndex;
		private int								currentGroupLevel;
	}
}
