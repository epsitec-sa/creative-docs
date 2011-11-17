using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;

using System.Collections.Generic;


namespace Epsitec.Cresus.Core.Server.UserInterface
{

	
	public class WebDataItem
	{


		public WebDataItem()
		{
			this.bindings = new HashSet<AccessorBinding> ();
			this.DefaultMode = ViewControllerMode.Edition;
			this.InitialVisibility = true;
		}


		// Is this constructor used somewhere with reflexion? If not, remove it.

		public WebDataItem(WebDataItem template)
			: this ()
		{
			if (template != null)
			{
				this.Name                     = template.Name;
				this.IconUri                  = template.IconUri;
				this.AutoGroup                = template.AutoGroup;
				this.HideAddButton            = template.HideAddButton;
				this.HideRemoveButton         = template.HideRemoveButton;
				this.Frameless                = template.Frameless;
				this.InitialVisibility        = template.InitialVisibility;
				this.FullHeightStretch        = template.FullHeightStretch;
				this.DataType                 = template.DataType;
				this.Title                    = template.Title;
				this.CompactTitle             = template.CompactTitle;
				this.DefaultMode              = template.DefaultMode;
			}
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


		public ViewControllerMode DefaultMode
		{
			get;
			set;
		}


		public static string BuildName(string prefix, int index)
		{
			return string.Concat (prefix, ".", index.ToString (System.Globalization.CultureInfo.InvariantCulture));
		}


		private readonly HashSet<AccessorBinding>	bindings;


		private FormattedText displayedTitle;
		private FormattedText displayedText;
		private FormattedText displayedCompactTitle;
		private FormattedText displayedCompactText;
	}


}
