//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Helpers;
using Epsitec.Cresus.Compta.Search.Data;
using Epsitec.Cresus.Compta.Fields.Controllers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Search.Controllers
{
	public class SearchNodeController
	{
		public SearchNodeController(AbstractController controller, SearchNodeData nodeData, bool isFilter)
		{
			this.controller = controller;
			this.nodeData   = nodeData;
			this.isFilter   = isFilter;

			this.ignoreChanges = new SafeCounter ();
			this.compta        = this.controller.ComptaEntity;
			this.columnMappers = this.controller.ColumnMappers;

			this.tabControllers = new List<SearchTabController> ();
			this.orMode = false;
		}


		public FrameBox CreateUI(FrameBox parent, System.Action searchStartAction, System.Action<int> addRemoveAction, System.Action swapNodeAction)
		{
			this.searchStartAction = searchStartAction;
			this.addRemoveAction   = addRemoveAction;
			this.swapNodeAction    = swapNodeAction;

			var frame = new FrameBox
			{
				Parent          = parent,
				PreferredHeight = 20,
				DrawFullFrame   = true,
				Dock            = DockStyle.Top,
			};

			this.middleFrame = new FrameBox
			{
				Parent          = frame,
				PreferredHeight = 20,
				Dock            = DockStyle.Fill,
				Padding         = new Margins (0, 5, 5, 5),
			};

			var addRemoveFrame = new FrameBox
			{
				Parent          = frame,
				PreferredWidth  = 20,
				PreferredHeight = 20,
				Dock            = DockStyle.Right,
				Padding         = new Margins (0, 5, 5, 5),
			};

			this.CreateAddRemoveUI (addRemoveFrame);
			this.CreateMiddleUI ();

			this.UpdateButtons ();

			return frame;
		}


		public void UpdateContent()
		{
			this.CreateMiddleUI ();
		}


		public void UpdateColumns()
		{
			//	Met à jour les widgets en fonction de la liste des colonnes présentes.
			foreach (var controller in this.tabControllers)
			{
				controller.UpdateColumns ();
			}
		}

		public void SetAddAction(int index, bool enable)
		{
			bool add = (index == 0);

			if (this.index != index || this.addAction != add || this.addActionEnable != enable)
			{
				this.index           = index;
				this.addAction       = add;
				this.addActionEnable = enable;

				this.UpdateButtons ();

				foreach (var controller in this.tabControllers)
				{
					controller.ParentIndex = this.index;
				}
			}
		}


		private void CreateMiddleUI()
		{
			this.middleFrame.Children.Clear ();
			this.tabControllers.Clear ();

			this.middleFrame.ContainerLayoutMode = ContainerLayoutMode.VerticalFlow;

			int count = this.nodeData.TabsData.Count;
			for (int i = 0; i < count; i++)
			{
				var controller = new SearchTabController (this.controller, this.nodeData.TabsData[i], this.isFilter);

				var frame = controller.CreateUI (this.middleFrame, this.bigDataInterface, this.searchStartAction, this.AddRemoveAction, this.swapNodeAction, this.SwapTabAction);
				controller.ParentIndex = this.index;
				controller.SetAddAction (i, count < 10);

				frame.TabIndex = i+1;
				frame.Margins = new Margins (0, 0, 0, (count > 1 && i < count-1) ? -1 : 0);

				this.tabControllers.Add (controller);
			}
		}


		private void CreateAddRemoveUI(FrameBox parent)
		{
			this.addRemoveButton = new IconButton
			{
				Parent          = parent,
				PreferredWidth  = 20,
				PreferredHeight = 20,
				Dock            = DockStyle.Top,
				Margins         = new Margins (0, 0, 0, 0),
			};

			this.addRemoveButton.Clicked += delegate
			{
				this.addRemoveAction (this.index);
			};
		}


		public bool OrMode
		{
			get
			{
				return this.orMode;
			}
			set
			{
				this.orMode = value;

				foreach (var controller in this.tabControllers)
				{
					controller.ParentOrMode = this.orMode;
				}
				
				this.UpdateButtons ();
			}
		}

		private void UpdateOrMode()
		{
			foreach (var controller in this.tabControllers)
			{
				controller.OrMode = this.nodeData.OrMode;
			}
		}

		private void UpdateButtons()
		{
			this.addRemoveButton.IconUri = UIBuilder.GetResourceIconUri (this.addAction ? "Search.AddNode" : "Search.SubNode");
			this.addRemoveButton.Enable = !this.addAction || this.addActionEnable;

			if (this.addAction)
			{
				ToolTip.Default.SetToolTip (this.addRemoveButton, this.isFilter ? "Ajoute un nouveau critère de filtre" : "Ajoute un nouveau critère de recherche");
			}
			else
			{
				ToolTip.Default.SetToolTip (this.addRemoveButton, this.isFilter ? "Supprime le critère de filtre" : "Supprime le critère de recherche");
			}
		}


		public void SetFocus()
		{
			if (this.tabControllers.Count != 0)
			{
				this.tabControllers[0].SetFocus ();
			}
		}


		public void SearchClear()
		{
			while (this.nodeData.TabsData.Count > 1)
			{
				this.nodeData.TabsData.RemoveAt (1);
			}

			this.nodeData.TabsData[0].Clear ();

			this.CreateMiddleUI ();
			this.searchStartAction ();
		}


		private void AddRemoveAction(int index)
		{
			if (index == 0)
			{
				this.nodeData.TabsData.Add (new SearchTabData ());
			}
			else
			{
				this.nodeData.TabsData.RemoveAt (index);
			}

			this.CreateMiddleUI ();
			this.searchStartAction ();
		}

		private void SwapTabAction()
		{
			this.nodeData.OrMode = !this.nodeData.OrMode;
			this.UpdateOrMode ();
			this.searchStartAction ();
		}


		private bool BigDataInterface
		{
			get
			{
				return this.bigDataInterface;
			}
			set
			{
				if (this.bigDataInterface != value)
				{
					this.bigDataInterface = value;

					this.CreateMiddleUI ();
				}
			}
		}


		private readonly AbstractController				controller;
		private readonly ComptaEntity					compta;
		private readonly List<ColumnMapper>				columnMappers;
		private readonly SearchNodeData					nodeData;
		private readonly List<SearchTabController>		tabControllers;
		private readonly bool							isFilter;
		private readonly SafeCounter					ignoreChanges;

		private int										index;
		private bool									orMode;

		private bool									bigDataInterface;
		private System.Action							searchStartAction;
		private System.Action<int>						addRemoveAction;
		private System.Action							swapNodeAction;

		private FrameBox								middleFrame;
		private IconButton								addRemoveButton;

		private bool									addAction;
		private bool									addActionEnable;
	}
}
