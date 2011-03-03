using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

namespace Epsitec.Common.Designer.Dialogs
{
	/// <summary>
	/// Dialogue en 3 volets permettant de créer une entité.
	/// </summary>
	public class EntityCreation : Abstract
	{
		public EntityCreation(DesignerApplication designerApplication)
			: base (designerApplication)
		{
			this.resourceName     = new ResourceName     (this.designerApplication);
			this.resourceSelector = new ResourceSelector (this.designerApplication);
			this.entityParameters = new EntityParameters (this.designerApplication);
		}


		public ResourceName ResourceName
		{
			get
			{
				return this.resourceName;
			}
		}

		public ResourceSelector ResourceSelector
		{
			get
			{
				return this.resourceSelector;
			}
		}

		public EntityParameters EntityParameters
		{
			get
			{
				return this.entityParameters;
			}
		}


		public override void Show()
		{
			//	Crée et montre la fenêtre du dialogue.
			this.isEditOk = false;

			if (this.window == null)
			{
				this.window = new Window();
				this.window.Icon = this.designerApplication.Icon;
				this.window.MakeSecondaryWindow ();
				this.window.PreventAutoClose = true;
				this.WindowInit ("EntityCreation", 500, 300, true);
				this.window.Text = "Créeation d'une nouvelle entité";  // Res.Strings.Dialog.EntityCreation.Title;
				this.window.Owner = this.parentWindow;
				this.window.WindowCloseClicked += this.HandleWindowCloseClicked;
				this.window.Root.MinSize = new Size(200, 150);
				this.window.Root.Padding = new Margins(8, 8, 8, 8);

				ResizeKnob resize = new ResizeKnob(this.window.Root);
				resize.Anchor = AnchorStyles.BottomRight;
				resize.Margins = new Margins(0, -8, 0, -8);
				ToolTip.Default.SetToolTip(resize, Res.Strings.Dialog.Tooltip.Resize);

				var mainPane = new FrameBox
				{
					Parent = this.window.Root,
					Dock = DockStyle.Fill,
				};

				//	Prépare les panneaux.
				this.selectedFrameBox = 0;
				this.frameBoxes = new List<FrameBox> ();

				for (int i = 0; i < 3; i++)
				{
					var frameBox = new FrameBox
					{
						Parent = mainPane,
						Anchor = AnchorStyles.All,
						Visibility = (i == this.selectedFrameBox),
					};

					this.frameBoxes.Add (frameBox);
				}

				//	Peuple les panneaux.
				this.resourceName    .CreateUI (this.frameBoxes[0]);
				this.resourceSelector.CreateUI (this.frameBoxes[1]);
				this.entityParameters.CreateUI (this.frameBoxes[2]);

				//	Boutons de fermeture.
				Widget footer = new Widget(this.window.Root);
				footer.PreferredHeight = 22;
				footer.Margins = new Margins(0, 0, 8, 0);
				footer.Dock = DockStyle.Bottom;

				this.levelLabel = new StaticText
				{
					Parent = footer,
					ContentAlignment = Drawing.ContentAlignment.MiddleRight,
					Dock = DockStyle.Fill,
					Margins = new Margins (0, 10, 0, 0),
				};

				this.buttonCancel = new Button(footer);
				this.buttonCancel.PreferredWidth = 75;
				this.buttonCancel.Text = Res.Strings.Dialog.Button.Cancel;
				this.buttonCancel.ButtonStyle = ButtonStyle.DefaultCancel;
				this.buttonCancel.Dock = DockStyle.Right;
				this.buttonCancel.Clicked += this.HandleButtonCloseClicked;
				this.buttonCancel.TabIndex = 11;
				this.buttonCancel.TabNavigationMode = TabNavigationMode.ActivateOnTab;

				this.buttonNext = new Button (footer);
				this.buttonNext.PreferredWidth = 75;
				this.buttonNext.ButtonStyle = ButtonStyle.DefaultAccept;
				this.buttonNext.Dock = DockStyle.Right;
				this.buttonNext.Margins = new Margins (0, 10, 0, 0);
				this.buttonNext.Clicked += this.HandleButtonNextClicked;
				this.buttonNext.TabIndex = 10;
				this.buttonNext.TabNavigationMode = TabNavigationMode.ActivateOnTab;

				this.buttonPrev = new Button (footer);
				this.buttonPrev.PreferredWidth = 75;
				this.buttonPrev.Text = "&lt; Précédent";
				this.buttonPrev.Dock = DockStyle.Right;
				this.buttonPrev.Margins = new Margins (0, 1, 0, 0);
				this.buttonPrev.Clicked += this.HandleButtonPrevClicked;
				this.buttonPrev.TabIndex = 10;
				this.buttonPrev.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			}

			this.resourceName.Update ();
			this.resourceSelector.Update ();
			this.entityParameters.Update ();

			this.selectedFrameBox = 0;  // commence toujours à la première "page"
			this.UpdateWidgets ();

			this.window.ShowDialog();
		}

		public bool IsEditOk
		{
			get
			{
				return this.isEditOk;
			}
		}


		private void UpdateWidgets()
		{
			for (int i = 0; i < this.frameBoxes.Count; i++)
			{
				this.frameBoxes[i].Visibility = (i == this.selectedFrameBox);
			}

			this.levelLabel.Text = string.Format ("{0} / {1}", (this.selectedFrameBox+1).ToString (), this.frameBoxes.Count.ToString ());
			this.buttonPrev.Enable = (this.selectedFrameBox != 0);
			this.buttonNext.Text = (this.selectedFrameBox < this.frameBoxes.Count-1) ? "Suivant &gt;" : "Créer";

			if (this.selectedFrameBox == 2)
			{
				this.EntityParameters.TitleEntity = this.resourceName.SelectedName;
			}
		}


		private void Close()
		{
			this.resourceName.Close ();
			this.resourceSelector.Close ();
			this.entityParameters.Close ();

			this.parentWindow.MakeActive ();
			this.window.Hide ();
			this.OnClosed ();
		}


		private void HandleWindowCloseClicked(object sender)
		{
			this.Close ();
		}

		private void HandleButtonCloseClicked(object sender, MessageEventArgs e)
		{
			this.Close ();
		}

		private void HandleButtonPrevClicked(object sender, MessageEventArgs e)
		{
			this.selectedFrameBox--;
			this.UpdateWidgets ();
		}

		private void HandleButtonNextClicked(object sender, MessageEventArgs e)
		{
			if (this.selectedFrameBox < this.frameBoxes.Count-1)
			{
				this.selectedFrameBox++;
				this.UpdateWidgets ();
			}
			else
			{
				this.Close ();
				this.isEditOk = true;
			}
		}


		private readonly ResourceName		resourceName;
		private readonly ResourceSelector	resourceSelector;
		private readonly EntityParameters	entityParameters;

		private bool						isEditOk;

		private int							selectedFrameBox;
		private List<FrameBox>				frameBoxes;

		private StaticText					levelLabel;
		private Button						buttonPrev;
		private Button						buttonNext;
		private Button						buttonCancel;
	}
}
