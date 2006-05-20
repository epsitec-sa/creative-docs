using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer
{
	/// <summary>
	/// Contient le contexte commun à tous les Viewers.
	/// </summary>
	public class PanelsContext
	{
		public PanelsContext()
		{
		}


		public string Tool
		{
			get
			{
				return this.tool;
			}
			set
			{
				this.tool = value;
			}
		}

		public bool ShowGrid
		{
			get
			{
				return this.showGrid;
			}
			set
			{
				this.showGrid = value;
			}
		}

		public bool ShowConstrain
		{
			get
			{
				return this.showConstrain;
			}
			set
			{
				this.showConstrain = value;
			}
		}

		public bool ShowAnchor
		{
			get
			{
				return this.showAnchor;
			}
			set
			{
				this.showAnchor = value;
			}
		}

		public bool ShowExpand
		{
			get
			{
				return this.showExpand;
			}
			set
			{
				this.showExpand = value;
			}
		}

		public bool ShowZOrder
		{
			get
			{
				return this.showZOrder;
			}
			set
			{
				this.showZOrder = value;
			}
		}

		public bool ShowTabIndex
		{
			get
			{
				return this.showTabIndex;
			}
			set
			{
				this.showTabIndex = value;
			}
		}


		protected string				tool = "ToolSelect";
		protected bool					showGrid = false;
		protected bool					showConstrain = true;
		protected bool					showAnchor = true;
		protected bool					showExpand = false;
		protected bool					showZOrder = false;
		protected bool					showTabIndex = false;
	}
}
