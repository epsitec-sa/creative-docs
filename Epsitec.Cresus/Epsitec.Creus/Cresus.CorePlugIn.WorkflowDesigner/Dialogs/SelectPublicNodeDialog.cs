//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Dialogs;
using Epsitec.Common.Drawing;
using Epsitec.Common.IO;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Epsitec.Cresus.CorePlugIn.WorkflowDesigner.Dialogs
{
	public class SelectPublicNodeDialog : AbstractDialog
	{
		public SelectPublicNodeDialog(Widget parent, Editor editor)
		{
			this.parent = parent;
			this.editor = editor;

			this.workflowNodeEntities = new List<WorkflowNodeEntity> ();
		}


		public WorkflowNodeEntity NodeEntity
		{
			get
			{
				int sel = this.listNodes.SelectedItemIndex;

				if (sel == -1)
				{
					return null;
				}
				else
				{
					return this.workflowNodeEntities[sel];
				}
			}
		}


		protected override Window CreateWindow()
		{
			this.CreateUserInterface ("SelectPublicNode", new Size (600, 400), "Choix d'un nœud public", this.parent.Window);
			this.UpdateLists ();
			this.UpdateButtons ();

			window.AdjustWindowSize ();

			return this.window;
		}

		protected Rectangle GetOwnerBounds()
		{
			//	Donne les frontières de l'application.
			var w = this.parent.Window;

			return new Rectangle (w.WindowLocation, w.WindowSize);
		}


		private void CreateUserInterface(string name, Size windowSize, string title, Window owner)
		{
			//	Crée la fenêtre et tous les widgets pour peupler le dialogue.
			this.window = new Window ();
			this.window.MakeSecondaryWindow ();
			this.window.PreventAutoClose = true;
			this.window.Name = name;
			this.window.Text = title;
			this.window.Owner = owner;
			this.window.Icon = owner == null ? null : this.window.Owner.Icon;
			this.window.ClientSize = windowSize;
			this.window.Root.Padding = new Margins (10);

			this.window.WindowCloseClicked += this.HandleWindowCloseClicked;

			var main = new FrameBox
			{
				Parent = this.window.Root,
				Dock = DockStyle.Fill,
			};

			var footer = new FrameBox
			{
				Parent = this.window.Root,
				Dock = DockStyle.Bottom,
				Margins = new Margins (0, 0, 10, 0),
			};

			this.CreateMain (main);
			this.CreateFooter (footer);
		}

		private void CreateMain(Widget parent)
		{
			var leftBox = new FrameBox
			{
				Parent = parent,
				PreferredWidth = 300,
				Dock = DockStyle.Left,
				Margins = new Margins (0, 0, 0, 0),
			};

			var rightBox = new FrameBox
			{
				Parent = parent,
				Dock = DockStyle.Fill,
				Margins = new Margins (10, 0, 0, 0),
			};

			//	Crée la partie de gauche.
			var leftLabel = new StaticText
			{
				Parent = leftBox,
				Text = "Workflows :",
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 0, 5),
			};

			this.listEntities = new ScrollList
			{
				Parent = leftBox,
				Dock = DockStyle.Fill,
			};

			this.listEntities.SelectedItemChanged += new EventHandler (this.HandlelistEnttiesSelectedItemChanged);

			//	Crée la partie de droite.
			var rightLabel = new StaticText
			{
				Parent = rightBox,
				Text = "Nœuds publics :",
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 0, 5),
			};

			this.listNodes = new ScrollList
			{
				Parent = rightBox,
				Dock = DockStyle.Fill,
			};

			this.listNodes.SelectedItemChanged += new EventHandler (this.HandlelistNodesSelectedItemChanged);
		}

		private void CreateFooter(Widget parent)
		{
			this.cancelButton = new Button
			{
				Parent = parent,
				Text = "Annuler",
				Dock = DockStyle.Right,
				Margins = new Margins (10, 0, 0, 0),
			};

			this.cancelButton.Clicked += new EventHandler<MessageEventArgs> (this.HandleCancelButtonClicked);

			this.acceptButton = new Button
			{
				Parent = parent,
				Text = "Choisir",
				Dock = DockStyle.Right,
				Margins = new Margins (10, 0, 0, 0),
			};

			this.acceptButton.Clicked += new EventHandler<MessageEventArgs> (this.HandleAcceptButtonClicked);
		}


		private void HandlelistEnttiesSelectedItemChanged(object sender)
		{
			this.UpdateListNodes ();
			this.UpdateButtons ();
		}

		private void HandlelistNodesSelectedItemChanged(object sender)
		{
			this.UpdateButtons ();
		}

		private void HandleCancelButtonClicked(object sender, MessageEventArgs e)
		{
			this.Result = DialogResult.Cancel;
			this.CloseDialog ();
		}

		private void HandleAcceptButtonClicked(object sender, MessageEventArgs e)
		{
			this.Result = DialogResult.Accept;
			this.CloseDialog ();
		}

		private void HandleWindowCloseClicked(object sender)
		{
			//	Fenêtre fermée.
			this.OnDialogClosed ();
			this.CloseDialog ();
		}


		private void UpdateLists()
		{
			this.UpdateListEntities ();
			this.UpdateListNodes ();
		}

		private void UpdateListEntities()
		{
			this.listEntities.Items.Clear ();

			this.workflowDefinitionEntities = this.editor.BusinessContext.Data.GetAllEntities<WorkflowDefinitionEntity> ().Where (x => x.Code != this.editor.WorkflowDefinitionEntity.Code).ToList ();
			foreach (var def in this.workflowDefinitionEntities)
			{
				this.listEntities.Items.Add (this.GetDefinitionDescription (def));
			}
		}

		private void UpdateListNodes()
		{
			this.workflowNodeEntities.Clear ();
			this.listNodes.Items.Clear ();

			int sel = this.listEntities.SelectedItemIndex;

			if (sel != -1)
			{
				var def = this.workflowDefinitionEntities[sel];

				foreach (var node in def.WorkflowNodes)
				{
					if (node.IsPublic && this.editor.IsUnusedCode (node.Code))
					{
						this.workflowNodeEntities.Add (node);
						this.listNodes.Items.Add (this.GetNodeDescription (node));
					}
				}
			}
		}

		private string GetDefinitionDescription(WorkflowDefinitionEntity def)
		{
			string text = def.WorkflowName.ToString ();

			if (!def.WorkflowDescription.IsNullOrWhiteSpace)
			{
				string desc = def.WorkflowDescription.ToString ().Replace ("<br/>", ", ");
				text += string.Concat (" (", desc, ")");
			}

			return text;
		}

		private string GetNodeDescription(WorkflowNodeEntity node)
		{
			string text = node.Name.ToString ();

			if (!node.Description.IsNullOrWhiteSpace)
			{
				string desc = node.Description.ToString ().Replace ("<br/>", ", ");
				text += string.Concat (" (", desc, ")");
			}

			return text;
		}

		private void UpdateButtons()
		{
			this.acceptButton.Enable = this.listEntities.SelectedItemIndex != -1 && this.listNodes.SelectedItemIndex != -1;
		}


		private readonly Widget					parent;
		private readonly Editor					editor;

		private Window							window;
		private ScrollList						listEntities;
		private ScrollList						listNodes;
		private Button							acceptButton;
		private Button							cancelButton;
		private List<WorkflowDefinitionEntity>	workflowDefinitionEntities;
		private List<WorkflowNodeEntity>		workflowNodeEntities;
	}
}
