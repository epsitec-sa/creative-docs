//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Finance.PriceCalculators
{
	/// <summary>
	/// The <c>LineTreeNode</c> class is used to build a tree of lines and groups, used
	/// by <see cref="LineTreeSorter"/> to sort the items.
	/// </summary>
	internal sealed class LineTreeNode
	{
		public LineTreeNode(int groupIndex)
		{
			this.groupIndex = groupIndex;
			this.groupLevel = AbstractDocumentItemEntity.GetGroupLevel (this.groupIndex);
		}

		
		public IEnumerable<AbstractDocumentItemEntity> Lines
		{
			get
			{
				if (this.items != null)
				{
					if (this.groupIndex == 0)
					{
						//	Root group: first emit all groups 01..nn, then emit the articles, sub-total
						//	taxes and end total :

						foreach (var line in this.items.Where (x => x.IsGroup).SelectMany (x => x.Lines))
						{
							yield return line;
						}

						var lines = this.items.Where (x => x.IsLine).Select (x => x.Line).ToList ();

						foreach (var line in lines.OfType<ArticleDocumentItemEntity> ())
						{
							yield return line;
						}
						foreach (var line in lines.OfType<SubTotalDocumentItemEntity> ())
						{
							yield return line;
						}
						foreach (var line in lines.OfType<TaxDocumentItemEntity> ())
						{
							yield return line;
						}
						foreach (var line in lines.OfType<EndTotalDocumentItemEntity> ())
						{
							yield return line;
						}
						foreach (var line in lines.OfType<TextDocumentItemEntity> ())
						{
							yield return line;
						}
					}
					else
					{
						bool emitSubTotals = false;

						foreach (var item in this.items)
						{
							if (item.IsLine)
							{
								var line = item.Line;

								if (line is SubTotalDocumentItemEntity)
								{
									emitSubTotals = true;
								}
								else
								{
									yield return line;
								}
							}
							if (item.IsGroup)
							{
								foreach (var line in item.Lines)
								{
									yield return line;
								}
							}
						}

						if (emitSubTotals)
						{
							foreach (var item in this.items)
							{
								if (item.IsLine)
								{
									var line = item.Line;

									if (line is SubTotalDocumentItemEntity)
									{
										yield return line;
									}
								}
							}
						}
					}
				}
			}
		}

		
		public void Insert(AbstractDocumentItemEntity line)
		{
			int groupIndex = line.GroupIndex;

			if (this.groupIndex == groupIndex)
			{
				this.EnsureItems ();
				this.items.Add (new Item (line));
			}
			else
			{
				this.SelectChildGroupNode (groupIndex).Insert (line);
			}
		}

		
		private LineTreeNode SelectChildGroupNode(int groupIndex)
		{
			if (this.subGroups == null)
			{
				this.subGroups = new SortedList<int, LineTreeNode> ();
			}

			int truncIndex = LineTreeNode.GetTruncatedIndex (groupIndex, this.groupLevel+1);
			LineTreeNode group;

			if (this.subGroups.TryGetValue (truncIndex, out group))
			{
				//	OK
			}
			else
			{
				group = new LineTreeNode (truncIndex);

				this.EnsureItems ();
				this.items.Insert (this.GetGroupItemInsertionIndex (truncIndex), new Item (group));

				this.subGroups.Add (truncIndex, group);
			}

			return group;

		}

		private void EnsureItems()
		{
			if (this.items == null)
			{
				this.items = new List<Item> ();
			}
		}
		
		private int GetGroupItemInsertionIndex(int truncIndex)
		{
			//	Find the insertion point in the items list, so that the group will be
			//	at the end of the list, if possible, or else just before the next group
			//	based on their indexes.

			int indexOfFollowingGroupItem = this.items.Count;
			int bestMatch = System.Int32.MaxValue;

			for (int i = indexOfFollowingGroupItem-1; i >= 0; i--)
			{
				var item = this.items[i];

				if ((item.IsGroup) &&
					(item.Group.groupIndex > truncIndex) &&
					(item.Group.groupIndex < bestMatch))
				{
					indexOfFollowingGroupItem = i;
					bestMatch = item.Group.groupIndex;
				}
			}
			
			return indexOfFollowingGroupItem;
		}
		
		private static int GetTruncatedIndex(int groupIndex, int groupLevel)
		{
			switch (groupLevel)
			{
				case 0:
					return 0;
				case 1:
					return groupIndex % 100;
				case 2:
					return groupIndex % 10000;
				case 3:
					return groupIndex % 1000000;
				case 4:
					return groupIndex % 100000000;
				default:
					throw new System.NotSupportedException ("Group level too high");
			}
		}

		#region Item Structure

		private struct Item
		{
			public Item(AbstractDocumentItemEntity line)
			{
				this.line  = line;
				this.group = null;
			}

			public Item(LineTreeNode group)
			{
				this.line  = null;
				this.group = group;
			}

			public bool IsGroup
			{
				get
				{
					return this.group != null;
				}
			}

			public bool IsLine
			{
				get
				{
					return this.line != null;
				}
			}

			public LineTreeNode					Group
			{
				get
				{
					return this.group;
				}
			}
			
			public AbstractDocumentItemEntity	Line
			{
				get
				{
					return this.line;
				}
			}

			public IEnumerable<AbstractDocumentItemEntity> Lines
			{
				get
				{
					if (this.line != null)
					{
						return new AbstractDocumentItemEntity[] { this.line };
					}
					else if (this.group != null)
					{
						return this.group.Lines;
					}
					else
					{
						return Enumerable.Empty<AbstractDocumentItemEntity> ();
					}
				}
			}

			private readonly AbstractDocumentItemEntity	line;
			private readonly LineTreeNode		group;
		}

		#endregion

		private readonly int					groupIndex;
		private readonly int					groupLevel;

		private SortedList<int, LineTreeNode>	subGroups;
		private List<Item>						items;
	}
}
