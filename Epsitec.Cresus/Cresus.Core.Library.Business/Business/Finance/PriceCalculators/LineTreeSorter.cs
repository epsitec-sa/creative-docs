//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Finance.PriceCalculators
{
	internal sealed class LineTreeSorter
	{
		private LineTreeSorter(IEnumerable<AbstractDocumentItemEntity> lines)
		{
			this.lines = lines;
		}

		public static IEnumerable<AbstractDocumentItemEntity> Sort(IEnumerable<AbstractDocumentItemEntity> lines)
		{
			var sorter = new LineTreeSorter (lines);
			return sorter.Sort ();
		}

		private IEnumerable<AbstractDocumentItemEntity> Sort()
		{
			GroupNode root = new GroupNode (0);

			foreach (var line in this.lines)
			{
				root.Insert (line);
			}

			return root.Lines;
		}

		class GroupNode
		{
			public GroupNode(int groupIndex)
			{
				this.groupIndex = groupIndex;
				this.groupLevel = AbstractDocumentItemEntity.GetGroupLevel (this.groupIndex);
				this.truncIndex = GroupNode.GetTruncatedIndex (this.groupIndex, this.groupLevel);
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
					if (this.items == null)
					{
						this.items = new List<Item> ();
					}

					this.items.Add (new Item (line));
				}
				else
				{
					this.SelectChildGroupNode (groupIndex).Insert (line);
				}
			}

			private GroupNode SelectChildGroupNode(int groupIndex)
			{
				if (this.subGroups == null)
				{
					this.subGroups = new SortedList<int, GroupNode> ();
				}

				int truncIndex = GroupNode.GetTruncatedIndex (groupIndex, this.groupLevel+1);
				GroupNode group;

				if (this.subGroups.TryGetValue (truncIndex, out group))
				{
					//	OK
				}
				else
				{
					group = new GroupNode (truncIndex);
					
					if (this.items == null)
					{
						this.items = new List<Item> ();
					}
					
					this.items.Add (new Item (group));
					this.subGroups.Add (truncIndex, group);
				}

				return group;

			}

			private GroupNode GetLeafGroupNode(int groupIndex, int groupLevel)
			{
				if (this.groupLevel == groupLevel)
				{
					return this;
				}
				else
				{
					return this.SelectChildGroupNode (groupIndex).GetLeafGroupNode (groupIndex, groupLevel);
				}
			}


			static int GetTruncatedIndex(int groupIndex, int groupLevel)
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

			struct Item
			{
				public Item(AbstractDocumentItemEntity line)
				{
					this.line  = line;
					this.group = null;
				}

				public Item(GroupNode group)
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

				public AbstractDocumentItemEntity Line
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
							return Epsitec.Common.Types.Collections.EmptyEnumerable<AbstractDocumentItemEntity>.Instance;
						}
					}
				}

				private readonly AbstractDocumentItemEntity	line;
				private readonly GroupNode					group;
			}

			private readonly int truncIndex;
			private readonly int groupIndex;
			private readonly int groupLevel;

			private SortedList<int, GroupNode> subGroups;
			private List<Item> items;
		}

		private readonly IEnumerable<AbstractDocumentItemEntity> lines;
	}
}
