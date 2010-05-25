//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.SummaryControllers;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers
{
	public class TileContainerController
	{
		public TileContainerController(CoreViewController controller, Widget container, SummaryDataItems dataItems)
		{
			this.controller  = controller;
			this.container   = container;
			this.dataItems   = dataItems;
			this.activeItems = new List<SummaryData> ();
		}

		
		public void MapDataToTiles()
		{
			this.RefreshCollectionItems ();
		}

		private void QueueTasklet<T>(string name, T source, System.Action<T> action, params SimpleCallback[] andThen)
		{
			andThen.ForEach (callback => Application.QueueAsyncCallback (callback));

			//	TODO: execute following code asynchronously :
			
			action (source);
			andThen.ForEach (callback => Application.QueueAsyncCallback (callback));
		}

		
		private void RefreshCollectionItems()
		{
			this.QueueTasklet ("RefreshCollectionItems", this.dataItems, items => items.RefreshCollectionItems (),
				this.RefreshActiveItems);
		}
		
		private void RefreshActiveItems()
		{
			var currentItems  = new List<SummaryData> (this.dataItems);
			var obsoleteItems = new List<SummaryData> (this.activeItems.Except (currentItems));

			this.activeItems.Clear ();
			this.activeItems.AddRange (currentItems);

			this.DisposeDataItems (obsoleteItems);

			this.QueueTasklet ("ExecuteAccessors", this.activeItems, items => items.ForEach (x => x.ExecuteAccessors ()),
				this.RefreshDataTiles);
		}

		private void DisposeDataItems(IEnumerable<SummaryData> collection)
		{
			//	TODO: dispose data items
		}


		private void RefreshDataTiles()
		{
			this.SortActiveItems ();
			this.CreateMissingDataTiles ();
			this.ResetDataTiles ();

			if (!this.container.IsActualGeometryValid)
			{
				Common.Widgets.Layouts.LayoutContext.SyncArrange (this.container);
			}

			double maxHeight = this.container.ActualHeight;

			this.LayoutTiles (maxHeight);
			this.SetDataTilesParent (this.container);
		}


		public void LayoutTiles(double maxHeight)
		{
			while (true)
			{
				double height = this.RefreshDataTilesContent ();

				if (height <= maxHeight)
				{
					break;
				}

				var lastItem = this.activeItems.LastOrDefault (item => item.AutoGroup == false && !item.SummaryTile.IsCompact);

				if (lastItem == null)
				{
					break;
				}

				lastItem.SummaryTile.IsCompact = true;
			}
		}
		public static FormattedText FormatText(params object[] values)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();

			bool emptyItem = true;

			foreach (var value in values.Select (item => UIBuilder.ConvertToText (item)))
			{
				string text = value.Replace ("\n", FormattedText.HtmlBreak).Trim ();

				if (text.Length == 0)
				{
					emptyItem = true;
					continue;
				}

				if (text[0] == '~')
				{
					if (emptyItem)
					{
						continue;
					}

					text = text.Substring (1);
				}

				if (!emptyItem && buffer[buffer.Length-1] != '(' && !Misc.IsPunctuationMark (text[0]))
				{
					buffer.Append (" ");
				}

				buffer.Append (text);

				emptyItem = text.EndsWith (FormattedText.HtmlBreak);
			}

			return new FormattedText (string.Join (FormattedText.HtmlBreak, buffer.ToString ().Split (new string[] { FormattedText.HtmlBreak }, System.StringSplitOptions.RemoveEmptyEntries)).Replace ("()", ""));
		}

		public static string ConvertToText(object value)
		{
			if (value == null)
			{
				return "";
			}

			string text = value as string;

			if (text != null)
			{
				return text;
			}

			if (value is Date)
			{
				return ((Date) value).ToDateTime ().ToShortDateString ();
			}

			return value.ToString ();
		}


		private void SortActiveItems()
		{
			this.activeItems.Sort ();
		}

		private void CreateMissingDataTiles()
		{
			foreach (var item in this.activeItems)
			{
				if (item.SummaryTile == null)
				{
					this.CreateSummaryTile (item);
					item.SummaryTile.Controller = item;

					UIBuilder.CreateTileHandler (item.SummaryTile, this.controller);
				}

				if (item.TitleTile == null)
				{
					item.TitleTile = new TitleTile ();
					item.TitleTile.Items.Add (item.SummaryTile);
				}
			}
		}

		private void CreateSummaryTile(SummaryData item)
		{
			if (item.DataType == SummaryDataType.CollectionItem)
			{
				var tile = new CollectionItemTile ();

				tile.AddClicked    += sender =>
				{
					item.AddNewItem ();
					this.MapDataToTiles ();
				};
				tile.RemoveClicked += sender =>
				{
					item.DeleteItem ();
					this.MapDataToTiles ();
				};

				item.SummaryTile = tile;
			}
			else
			{
				var tile = new SummaryTile ();
				item.SummaryTile = tile;
			}
		}

		private void ResetDataTiles()
		{
			HashSet<long> visualIds = new HashSet<long> ();
			Dictionary<string, TitleTile> tiles = new Dictionary<string, TitleTile> ();

			foreach (var item in this.activeItems)
			{
				System.Diagnostics.Debug.Assert (item.TitleTile != null);
				System.Diagnostics.Debug.Assert (item.SummaryTile != null);

				item.TitleTile.Parent = null;

				if (item.AutoGroup)
				{
					string prefix = SummaryData.GetNamePrefix (item.Name);
					TitleTile other;

					if (tiles.TryGetValue (prefix, out other))
					{
						if (item.TitleTile != other)
						{
							item.TitleTile.Items.Remove (item.SummaryTile);
							item.TitleTile = other;
							item.TitleTile.Items.Add (item.SummaryTile);
						}
					}
					else
					{
						tiles.Add (prefix, item.TitleTile);
					}
				}
				else
				{
					long visualId = item.TitleTile.GetVisualSerialId ();

					if (visualIds.Contains (visualId))
					{
						item.TitleTile.Items.Remove (item.SummaryTile);
						item.TitleTile = new TitleTile ();
						item.TitleTile.Items.Add (item.SummaryTile);
					}
					else
					{
						visualIds.Add (visualId);
					}
				}

				item.SummaryTile.IsCompact = false;
			}
		}

		private double RefreshDataTilesContent()
		{
			var visualIds = new HashSet<long> ();
			var titleTiles = new List<TitleTile> ();

			foreach (var item in this.activeItems)
			{
				if (item.SummaryTile.IsCompact)
				{
					item.SummaryTile.Summary = item.CompactText.ToString ();
					item.TitleTile.Title     = item.CompactTitle.ToString ();
					item.TitleTile.IconUri   = item.IconUri;
				}
				else
				{
					item.SummaryTile.Summary = item.Text.ToString ();
					item.TitleTile.Title     = item.DefaultTitle.ToString ();
					item.TitleTile.IconUri   = item.IconUri;
				}

				long visualId = item.TitleTile.GetVisualSerialId ();

				if (!visualIds.Contains (visualId))
				{
					visualIds.Add (visualId);
					titleTiles.Add (item.TitleTile);
				}
			}

			double height = 0;

			foreach (var tile in titleTiles)
			{
				if (height > 0)
				{
					height += 5;
				}

				height += tile.GetFullHeight ();
			}

			return height;
		}

		private void SetDataTilesParent(Widget parent)
		{
			var visualIds = new HashSet<long> ();

			foreach (var item in this.activeItems)
			{
				var titleTile = item.TitleTile;
				long visualId = titleTile.GetVisualSerialId ();

				if (!visualIds.Contains (visualId))
				{
					visualIds.Add (visualId);
					titleTile.Parent = parent;
					titleTile.Dock = DockStyle.Top;
					titleTile.Margins = new Margins (0, 0, 0, 5);
					titleTile.ArrowDirection = Direction.Right;
					titleTile.IsReadOnly = true;
				}
			}
		}

		private readonly Widget					container;
		private readonly CoreViewController		controller;
		private readonly SummaryDataItems		dataItems;
		private readonly List<SummaryData>		activeItems;

		private int currentGeneration;
	}
}
