using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;

using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.UserInterface
{

	
	internal sealed class WebTileDataItem
	{


		// This class is the webcore equivalent of Epsitec.Cresus.Core.Controllers.TileDataItem.


		public WebTileDataItem()
		{
			this.SubViewControllerMode = ViewControllerMode.Edition;
			this.InitialVisibility = true;
		}


		public string Name
		{
			get;
			set;
		}


		public string IconUri
		{
			get;
			set;
		}


		public bool AutoGroup
		{
			get;
			set;
		}


		public bool IsCompact
		{
			get;
			set;
		}


		public bool Frameless
		{
			get;
			set;
		}


		public bool HideAddButton
		{
			get;
			set;
		}


		public bool HideRemoveButton
		{
			get;
			set;
		}


		public bool InitialVisibility
		{
			get;
			set;
		}


		public bool FullHeightStretch
		{
			get;
			set;
		}


		public TileDataType DataType
		{
			get;
			set;
		}


		public FormattedText Title
		{
			get;
			set;
		}


		public FormattedText Text
		{
			get;
			set;
		}



		public FormattedText CompactTitle
		{
			get;
			set;
		}


		public FormattedText CompactText
		{
			get;
			set;
		}


		public FormattedText DisplayedTitle
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


		public FormattedText DisplayedText
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


		public FormattedText DisplayedCompactText
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


		public FormattedText DisplayedCompactTitle
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


		public ViewControllerMode SubViewControllerMode
		{
			get;
			set;
		}


		public int? SubViewControllerSubTypeId
		{
			get;
			set;
		}


		private FormattedText displayedTitle;
		private FormattedText displayedText;
		private FormattedText displayedCompactTitle;
		private FormattedText displayedCompactText;
	}


}
