//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core.Factories;
using Epsitec.Cresus.Core.Orchestrators.Navigation;
using Epsitec.Cresus.Core.Widgets.Tiles;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.DataAccessors
{
	/// <summary>
	/// The <c>TileDataItem</c> provides the texts used to set up the
	/// <see cref="TitleTile"/> and <see cref="Tile"/>. It is
	/// built to support asynchronous tile initialization.
	/// </summary>
	public class TileDataItem : System.IComparable<TileDataItem>, ITileController, IGroupedItem
	{
		public TileDataItem()
		{
			this.bindings = new HashSet<AccessorBinding> ();
			this.DefaultMode = ViewControllerMode.Edition;
			this.InitialVisibility = true;
		}

		public TileDataItem(TileDataItem template)
			: this ()
		{
			if (template != null)
			{
				this.Name                     = template.Name;
				this.Rank                     = template.Rank;
				this.IconUri                  = template.IconUri;
				this.AutoGroup                = template.AutoGroup;
				this.HideAddButton            = template.HideAddButton;
				this.HideRemoveButton         = template.HideRemoveButton;
				this.Frameless                = template.Frameless;
				this.InitialVisibility        = template.InitialVisibility;
				this.DataType                 = template.DataType;
				this.Title                    = template.Title;
				this.CompactTitle             = template.CompactTitle;
				this.DefaultMode              = template.DefaultMode;
				this.EntityMarshalerConverter = template.EntityMarshalerConverter;
				this.CreateEditionUI          = template.CreateEditionUI;
				this.CreateCustomizedUI       = template.CreateCustomizedUI;
			}
		}

		
		public string							Name
		{
			get;
			set;
		}

		public int								Rank
		{
			get;
			set;
		}

		public int								GroupingRank
		{
			get
			{
				return TileDataItem.GetGroupingRank (this.Rank);
			}
		}

		public int								LocalRank
		{
			get
			{
				return TileDataItem.GetLocalRank (this.Rank);
			}
		}

		public string							IconUri
		{
			get;
			set;
		}

		public GroupedItemController			GroupController
		{
			get;
			set;
		}
		
		public bool								AutoGroup
		{
			get;
			set;
		}

		public bool								IsCompact
		{
			get;
			set;
		}

		public bool								Frameless
		{
			get;
			set;
		}

		public bool								HideAddButton
		{
			get
			{
				return this.hideAddButton || this.AddNewItem == null;
			}
			set
			{
				this.hideAddButton = value;
			}
		}

		public bool								HideRemoveButton
		{
			get
			{
				return this.hideRemoveButton || this.DeleteItem == null;
			}
			set
			{
				this.hideRemoveButton = value;
			}
		}

		public bool								InitialVisibility
		{
			get;
			set;
		}

		public TileDataType						DataType
		{
			get;
			set;
		}

		public FormattedText					Title
		{
			get;
			set;
		}

		public FormattedText					Text
		{
			get;
			set;
		}

		public FormattedText					CompactTitle
		{
			get;
			set;
		}

		public FormattedText					CompactText
		{
			get;
			set;
		}

		public FormattedText					DisplayedTitle
		{
			get
			{
				if (this.displayedTitle.IsNullOrEmpty)
				{
					return this.Title;
				}
				else
				{
					return this.displayedTitle;
				}
			}
		}

		public FormattedText					DisplayedText
		{
			get
			{
				if (this.displayedText.IsNullOrEmpty)
				{
					return this.Text;
				}
				else
				{
					return this.displayedText;
				}
			}
		}

		public FormattedText					DisplayedCompactText
		{
			get
			{
				if (this.displayedCompactText.IsNullOrEmpty)
				{
					return this.CompactText;
				}
				else
				{
					return this.displayedCompactText;
				}
			}
		}

		public FormattedText					DisplayedCompactTitle
		{
			get
			{
				if (this.displayedCompactTitle.IsNullOrEmpty)
				{
					return this.CompactTitle;
				}
				else
				{
					return this.displayedCompactTitle;
				}
			}
		}


		public ViewControllerMode				DefaultMode
		{
			get;
			set;
		}


		public System.Action<EditionTile, UIBuilder> CreateEditionUI
		{
			//	Constructeur de l'interface utilisateur pour une tuile d'édition.
			get;
			set;
		}

		public System.Action<EditionTile, UIBuilder> CreateCustomizedUI
		{
			//	Constructeur de l'interface utilisateur pour une tuile personnalisée.
			get;
			set;
		}


		/// <summary>
		/// Gets or sets the associated title tile. The <see cref="TileDataItem"/>
		/// will be inserted into the title tile collection of items.
		/// </summary>
		/// <value>The title tile.</value>
		public TitleTile						TitleTile
		{
			get
			{
				return this.titleTile;
			}
			set
			{
				if (this.titleTile != value)
				{
					//	Manage insertion/removal from the title tile items collection.
					if (this.titleTile != null && this.Tile != null)
                    {
						this.titleTile.Items.Remove (this.Tile);
                    }

					this.titleTile = value;

					if (this.titleTile != null && this.Tile != null)
					{
						this.titleTile.Items.Add (this.Tile);
					}

					if (this.titleTile != null)
					{
						this.titleTile.Visibility = this.InitialVisibility;
					}
				}
			}
		}

		public GenericTile						Tile
		{
			get;
			set;
		}


		private System.Action addNewItem;

		public System.Action					AddNewItem
		{
			get
			{
				return this.addNewItem;
			}
			set
			{
				this.addNewItem = value;
			}
		}

		public System.Action					DeleteItem
		{
			get;
			set;
		}



		public Marshaler						EntityMarshaler
		{
			get;
			set;
		}

		public System.Func<AbstractEntity, AbstractEntity> EntityMarshalerConverter
		{
			get;
			private set;
		}

		public Accessor<FormattedText>			TitleAccessor
		{
			set
			{
				if (value != null)
				{
					this.bindings.Add (AccessorBinding.Create (value, () => this.DisplayedTitle, x => this.displayedTitle = x));
				}
			}
		}

		public Accessor<FormattedText>			TextAccessor
		{
			set
			{
				if (value != null)
				{
					this.bindings.Add (AccessorBinding.Create (value, () => this.DisplayedText, x => this.displayedText = x));
				}
			}
		}

		public Accessor<FormattedText>			CompactTitleAccessor
		{
			set
			{
				if (value != null)
				{
					this.bindings.Add (AccessorBinding.Create (value, () => this.DisplayedCompactTitle, x => this.displayedCompactTitle = x));
				}
			}
		}

		public Accessor<FormattedText>			CompactTextAccessor
		{
			set
			{
				if (value != null)
				{
					this.bindings.Add (AccessorBinding.Create (value, () => this.DisplayedCompactText, x => this.displayedCompactText = x));
				}
			}
		}

		public int								CreatedIndex
		{
			get;
			set;
		}


		public void ExecuteAccessors()
		{
			this.bindings.ForEach (x => x.Execute ());
		}

		public void SetEntityConverter<T>(System.Func<T, AbstractEntity> converter)
			where T : AbstractEntity
		{
			if (converter == null)
			{
				this.EntityMarshalerConverter = null;
			}
			else
			{
				//	We cannot use contravariance here, since there is no possible conversion from
				//	A = Func<T, X> to B = Func<AbstractEntity, X>; if we called B, passing it a T
				//	as parameter, it would end up calling A (which is OK), but if we called B with
				//	a type not compatible with T, the call to A would fail.
				//	See http://msdn.microsoft.com/en-us/library/dd465122%28VS.100%29.aspx
				
				//	We convert the function manually, assuming that the caller knows what he is
				//	doing and that the parameter will always be of the proper type:
				
				this.EntityMarshalerConverter = x => converter ((T) x);
			}
		}

		public void SetEntityConverter<T>(System.Delegate converter)
			where T : AbstractEntity
		{
			if (converter == null)
			{
				this.EntityMarshalerConverter = null;
			}
			else
			{
				this.EntityMarshalerConverter = x => (AbstractEntity) converter.DynamicInvoke (x);
			}
		}


		public static string BuildName(string prefix, int index)
		{
			return string.Concat (prefix, ".", index.ToString (System.Globalization.CultureInfo.InvariantCulture));
		}

		public static string GetNamePrefix(string name)
		{
			if (name == null)
            {
				return null;
            }

			int pos = name.LastIndexOf ('.');

			if (pos < 0)
			{
				return name;
			}
			else
			{
				return name.Substring (0, pos);
			}
		}

		public static int GetGroupingRank(int rank)
		{
			return rank / 1000;
		}

		public static int GetLocalRank(int rank)
		{
			return rank % 1000;
		}

		public static int CreateRank(int groupingRank, int localRank)
		{
			return groupingRank * 1000 + localRank;
		}

		
		#region ITileController Members

		EntityViewController ITileController.CreateSubViewController(Orchestrators.DataViewOrchestrator orchestrator, NavigationPathElement navigationPathElement)
		{
			var marshaler = this.EntityMarshaler;
			
			if (marshaler == null)
			{
				return null;
			}
			else
			{
				var entity = marshaler.GetValue<AbstractEntity> ();
				var mode   = this.DefaultMode;

				var converter = this.EntityMarshalerConverter;

				if (converter != null)
				{
					entity = converter (entity);
				}

				navigationPathElement = new TileNavigationPathElement (this.Name);
				
				var controller = EntityViewControllerFactory.Create ("ViewController", entity, mode, orchestrator, navigationPathElement: navigationPathElement);

				return controller;
			}
		}

		#endregion
		
		#region IComparable<TileDataItem> Members

		public int CompareTo(TileDataItem other)
		{
			int groupingRankA = TileDataItem.GetGroupingRank (this.Rank);
			int groupingRankB = TileDataItem.GetGroupingRank (other.Rank);

			if (groupingRankA < groupingRankB)
			{
				return -1;
			}
			else if (groupingRankA > groupingRankB)
			{
				return 1;
			}

			var options = System.Globalization.CompareOptions.StringSort | System.Globalization.CompareOptions.IgnoreCase;
			var culture = System.Globalization.CultureInfo.CurrentCulture;
			int result  = string.Compare (this.CompactTitle.ToSimpleText (), other.CompactTitle.ToSimpleText (), culture, options);

			if (result == 0)
            {
				if (this.Rank < other.Rank)
				{
					return -1;
				}
				else if (this.Rank > other.Rank)
                {
					return 1;
                }
            }

			return result;
		}

		#endregion

		#region IGroupController Members

		string IGroupedItem.GetGroupId()
		{
			if (this.GroupController == null)
			{
				return null;
			}
			else
			{
				return this.CompactTitle.ToString ();
			}
		}

		#endregion

		#region IGroupPositionController Members

		int IGroupedItemPosition.GroupedItemIndex
		{
			get
			{
				if (this.GroupController == null)
				{
					return 0;
				}

				return this.GroupController.GetItemIndex ();
			}
			set
			{
				if (this.GroupController == null)
				{
					throw new System.InvalidOperationException ();
				}

				this.GroupController.SetItemIndex (value);
			}
		}

		int IGroupedItemPosition.GroupedItemCount
		{
			get
			{
				if (this.GroupController == null)
				{
					return 0;
				}

				return this.GroupController.GetItemCount ();
			}
		}

		#endregion


		private readonly HashSet<AccessorBinding>	bindings;

		private TitleTile							titleTile;

		private FormattedText						displayedTitle;
		private FormattedText						displayedText;
		private FormattedText						displayedCompactTitle;
		private FormattedText						displayedCompactText;
		
		private bool								hideAddButton;
		private bool								hideRemoveButton;
	}
}