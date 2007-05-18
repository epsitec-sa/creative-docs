using System.Collections.Generic;
using System.Text.RegularExpressions;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Viewers
{
	/// <summary>
	/// Permet de représenter les ressources d'un module.
	/// </summary>
	public abstract class Abstract2 : Abstract
	{
		protected enum BandMode
		{
			CaptionSummary,
			CaptionView,
			Separator,
			SuiteSummary,
			SuiteView,
		}


		public Abstract2(Module module, PanelsContext context, ResourceAccess access, MainWindow mainWindow) : base(module, context, access, mainWindow)
		{
			//	Crée les deux volets séparés d'un splitter.
			this.firstPane = new Widget(this);
			this.firstPane.Name = "FirstPane";
			if (this.mainWindow.DisplayHorizontal)
			{
				this.firstPane.MinWidth = 80;
				this.firstPane.MaxWidth = 600;
				this.firstPane.PreferredWidth = Abstract.leftArrayWidth;
			}
			else
			{
				this.firstPane.MinHeight = 100;
				this.firstPane.MaxHeight = 600;
				this.firstPane.PreferredHeight = Abstract.topArrayHeight;
			}
			this.firstPane.Dock = this.mainWindow.DisplayHorizontal ? DockStyle.Left : DockStyle.Top;
			this.firstPane.Padding = new Margins(10, 10, 10, 10);
			this.firstPane.TabIndex = this.tabIndex++;
			this.firstPane.TabNavigationMode = TabNavigationMode.ForwardTabPassive;

			if (this.mainWindow.DisplayHorizontal)
			{
				this.splitter = new VSplitter(this);
			}
			else
			{
				this.splitter = new HSplitter(this);
			}
			this.splitter.Dock = this.mainWindow.DisplayHorizontal ? DockStyle.Left : DockStyle.Top;
			this.splitter.SplitterDragged += new EventHandler(this.HandleSplitterDragged);
			VSplitter.SetAutoCollapseEnable(this.firstPane, true);

			this.lastPane = new Widget(this);
			this.lastPane.Name = "LastPane";
			if (this.mainWindow.DisplayHorizontal)
			{
				this.lastPane.MinWidth = 200;
			}
			else
			{
				this.lastPane.MinHeight = 50;
			}
			this.lastPane.Dock = DockStyle.Fill;
			this.lastPane.Padding = new Margins(10, 10, 10, 10);
			this.lastPane.TabIndex = this.tabIndex++;
			this.lastPane.TabNavigationMode = TabNavigationMode.ForwardTabPassive;
			
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.splitter.SplitterDragged -= new EventHandler(this.HandleSplitterDragged);
			}

			base.Dispose(disposing);
		}


		protected override void UpdateModificationsCulture()
		{
			//	Met à jour les pastilles dans les boutons des cultures.
			if (this.secondaryButtonsCulture == null)  // pas de culture secondaire ?
			{
				return;
			}

			CultureMap item = this.access.CollectionView.CurrentItem as CultureMap;

			foreach (IconButtonMark button in this.secondaryButtonsCulture)
			{
				ResourceAccess.ModificationState state = this.access.GetModification(item, button.Name);

				if (state == ResourceAccess.ModificationState.Normal)
				{
					button.BulletColor = Color.Empty;
				}
				else
				{
					button.BulletColor = Abstract.GetBackgroundColor(state, 1.0);
				}
			}
		}

		protected override void UpdateSelectedCulture()
		{
			//	Sélectionne le bouton correspondant à la culture secondaire.
			this.table.ColumnHeader.SetColumnText(1, Misc.CultureName(this.access.GetBaseCultureName()));
			this.table.ColumnHeader.SetColumnText(2, Misc.CultureName(this.GetTwoLetters(1)));

			if (this.secondaryButtonsCulture == null)
			{
				return;
			}

			for (int i=0; i<this.secondaryButtonsCulture.Length; i++)
			{
				if (this.secondaryButtonsCulture[i].Name == this.GetTwoLetters(1))
				{
					this.secondaryButtonsCulture[i].ActiveState = ActiveState.Yes;
				}
				else
				{
					this.secondaryButtonsCulture[i].ActiveState = ActiveState.No;
				}
			}
		}

		protected override void UpdateCultures()
		{
			//	Met à jour les boutons des cultures en fonction des cultures existantes.
			if (this.secondaryButtonsCulture != null)
			{
				foreach (IconButtonMark button in this.secondaryButtonsCulture)
				{
					button.Clicked -= new MessageEventHandler(this.HandleButtonSecondaryCultureClicked);
					button.Dispose();
				}
				this.secondaryButtonsCulture = null;
			}

			this.primaryButtonCulture.Text = string.Format(Res.Strings.Viewers.Strings.Reference, Misc.CultureName(this.access.GetBaseCultureName()));

			List<string> list = this.access.GetSecondaryCultureNames();  // TODO:
			if (list.Count > 0)
			{
				this.secondaryButtonsCulture = new IconButtonMark[list.Count];
				for (int i=0; i<list.Count; i++)
				{
					this.secondaryButtonsCulture[i] = new IconButtonMark(this.secondaryButtonsCultureGroup);
					this.secondaryButtonsCulture[i].ButtonStyle = ButtonStyle.ActivableIcon;
					this.secondaryButtonsCulture[i].SiteMark = ButtonMarkDisposition.Below;
					this.secondaryButtonsCulture[i].MarkDimension = 5;
					this.secondaryButtonsCulture[i].Name = list[i];
					this.secondaryButtonsCulture[i].Text = Misc.CultureName(list[i]);
					this.secondaryButtonsCulture[i].AutoFocus = false;
					this.secondaryButtonsCulture[i].Dock = DockStyle.Fill;
					this.secondaryButtonsCulture[i].Margins = new Margins(0, (i==list.Count-1)?1:0, 0, 0);
					this.secondaryButtonsCulture[i].Clicked += new MessageEventHandler(this.HandleButtonSecondaryCultureClicked);
				}

				this.TwoLettersSecondaryCulture = list[0];
			}
			else
			{
				this.TwoLettersSecondaryCulture = null;
			}
		}


		protected string TwoLettersSecondaryCulture
		{
			//	Culture secondaire utilisée.
			get
			{
				return this.secondaryCulture;
			}
			set
			{
				this.secondaryCulture = value;

				this.UpdateSelectedCulture();
				this.UpdateArray();
			}
		}

		protected string GetTwoLetters(int row)
		{
			//	Retourne la culture primaire ou secondaire utilisée.
			System.Diagnostics.Debug.Assert(row == 0 || row == 1);
			return (row == 0) ? Resources.DefaultTwoLetterISOLanguageName : this.secondaryCulture;
		}


		protected void UpdateColor()
		{
			//	Met à jour les couleurs dans toutes les bandes.
			CultureMap item = this.access.CollectionView.CurrentItem as CultureMap;
			ResourceAccess.ModificationState state1 = this.access.GetModification(item, this.GetTwoLetters(0));
			ResourceAccess.ModificationState state2 = this.access.GetModification(item, this.GetTwoLetters(1));
			this.ColoriseBands(state1, state2);
		}

		
		#region Band
		protected GlyphButton CreateBand(out MyWidgets.StackedPanel leftContainer, out MyWidgets.StackedPanel rightContainer, string title, BandMode mode, GlyphShape extendShape, bool isNewSection, double backgroundIntensity)
		{
			//	Crée une bande horizontale avec deux containers gauche/droite pour les
			//	ressources primaire/secondaire.
			Widget band = new Widget(this.scrollable.Panel);
			band.Name = "BandForLeftAndRight";
			band.Dock = DockStyle.StackBegin;
			band.ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow;
			band.TabIndex = this.tabIndex++;
			band.TabNavigationMode = TabNavigationMode.ForwardTabPassive;

			leftContainer = new MyWidgets.StackedPanel(band);
			leftContainer.Name = "LeftContainer";
			leftContainer.Title = title;
			leftContainer.IsLeftPart = true;
			leftContainer.IsNewSection = isNewSection;
			leftContainer.ExtendShape = extendShape;
			leftContainer.MinWidth = 100;
			leftContainer.Dock = DockStyle.StackFill;
			leftContainer.TabIndex = this.tabIndex++;
			leftContainer.TabNavigationMode = TabNavigationMode.ForwardTabPassive;

			rightContainer = new MyWidgets.StackedPanel(band);
			rightContainer.Name = "RightContainer";
			rightContainer.Title = title;
			rightContainer.IsLeftPart = false;
			rightContainer.MinWidth = 100;
			rightContainer.Dock = DockStyle.StackFill;
			rightContainer.TabIndex = this.tabIndex++;
			rightContainer.TabNavigationMode = TabNavigationMode.ForwardTabPassive;

			this.bands.Add(new Band(band, leftContainer, rightContainer, mode, backgroundIntensity));

			return leftContainer.ExtendButton;
		}

		protected GlyphButton CreateBand(out MyWidgets.StackedPanel leftContainer, string title, BandMode mode, GlyphShape extendShape, bool isNewSection, double backgroundIntensity)
		{
			//	Crée une bande horizontale avec un seul container gauche pour la
			//	ressource primaire.
			Widget band = new Widget(this.scrollable.Panel);
			band.Name = "BandForLeft";
			band.Dock = DockStyle.StackBegin;
			band.ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow;
			band.TabIndex = this.tabIndex++;
			band.TabNavigationMode = TabNavigationMode.ForwardTabPassive;

			leftContainer = new MyWidgets.StackedPanel(band);
			leftContainer.Name = "LeftContainer";
			leftContainer.Title = title;
			leftContainer.IsLeftPart = true;
			leftContainer.IsNewSection = isNewSection;
			leftContainer.ExtendShape = extendShape;
			leftContainer.MinWidth = 100;
			leftContainer.Dock = DockStyle.StackFill;
			leftContainer.TabIndex = this.tabIndex++;
			leftContainer.TabNavigationMode = TabNavigationMode.ForwardTabPassive;

			this.bands.Add(new Band(band, leftContainer, null, mode, backgroundIntensity));

			return leftContainer.ExtendButton;
		}

		protected void ColoriseBands(ResourceAccess.ModificationState state1, ResourceAccess.ModificationState state2)
		{
			//	Colorise toutes les bandes horizontales.
			for (int i=0; i<this.bands.Count; i++)
			{
				MyWidgets.StackedPanel lc = this.bands[i].leftContainer;
				MyWidgets.StackedPanel rc = this.bands[i].rightContainer;

				lc.BackgroundColor = Abstract.GetBackgroundColor(state1, this.bands[i].intensityContainer);

				if (rc != null)
				{
					rc.BackgroundColor = Abstract.GetBackgroundColor(state2, this.bands[i].intensityContainer);
					rc.Visibility = (this.GetTwoLetters(1) != null);
				}
			}
		}

		protected struct Band
		{
			public Band(Widget band, MyWidgets.StackedPanel left, MyWidgets.StackedPanel right, BandMode mode, double intensity)
			{
				this.bandContainer = band;
				this.leftContainer = left;
				this.rightContainer = right;
				this.bandMode = mode;
				this.intensityContainer = intensity;
			}

			public Widget						bandContainer;
			public MyWidgets.StackedPanel		leftContainer;
			public MyWidgets.StackedPanel		rightContainer;
			public BandMode						bandMode;
			public double						intensityContainer;
		}
		#endregion


		private void HandleSplitterDragged(object sender)
		{
			//	Le splitter a été bougé.
			if (this.mainWindow.DisplayHorizontal)
			{
				Abstract.leftArrayWidth = this.firstPane.ActualWidth;
			}
			else
			{
				Abstract.topArrayHeight = this.firstPane.ActualHeight;
			}
		}

		private void HandleButtonSecondaryCultureClicked(object sender, MessageEventArgs e)
		{
			//	Un bouton pour changer de culture secondaire a été cliqué.
			IconButtonMark button = sender as IconButtonMark;
			this.TwoLettersSecondaryCulture = button.Name;

			this.UpdateEdit();
			this.UpdateColor();
			this.UpdateCommands();
		}


		
		protected Widget						firstPane;
		protected Widget						lastPane;
		protected AbstractSplitter				splitter;
		protected UI.ItemTable					table;
		protected MyWidgets.TextFieldExName		labelEdit;

		protected FrameBox						titleBox;
		protected StaticText					titleText;
		protected Scrollable					scrollable;
		protected List<Band>					bands;

		protected IconButtonMark				primaryButtonCulture;
		protected Widget						secondaryButtonsCultureGroup;
		protected IconButtonMark[]				secondaryButtonsCulture;
	}
}
