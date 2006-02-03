using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Text;

namespace Epsitec.Common.Document.TextPanels
{
	/// <summary>
	/// La classe Generator permet de choisir les puces et les numérotations.
	/// </summary>
	[SuppressBundleSupport]
	public class Generator : Abstract
	{
		public Generator(Document document, bool isStyle, StyleCategory styleCategory) : base(document, isStyle, styleCategory)
		{
			this.label.Text = Res.Strings.TextPanel.Generator.Title;

			this.fixIcon.Text = Misc.Image("TextGenerator");
			ToolTip.Default.SetToolTip(this.fixIcon, Res.Strings.TextPanel.Generator.Title);

			this.fieldType = new TextFieldCombo(this);
			this.fieldType.Text = "Aucun";
			this.fieldType.IsReadOnly = true;
			this.fieldType.AutoFocus = false;
			this.fieldType.TextChanged += new EventHandler(this.HandleTypeChanged);
			this.fieldType.TabIndex = this.tabIndex++;
			this.fieldType.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.fieldType.Items.Add("Aucun");
			this.fieldType.Items.Add("Petites puces");
			this.fieldType.Items.Add("Grosse puces");
			this.fieldType.Items.Add("Numérotation 1.1.1");
			this.fieldType.Items.Add("Numérotation 1.a.a");
			this.fieldType.Items.Add("Numérotation a.i.i");
			this.fieldType.Items.Add("Personnalisé");
			//?ToolTip.Default.SetToolTip(this.fieldType, Res.Strings.TextPanel.Generator.Tooltip.Generator);

			this.radioLevel = new RadioButton[Generator.maxLevel];
			for ( int i=0 ; i<Generator.maxLevel ; i++ )
			{
				this.radioLevel[i] = new RadioButton(this);
				this.radioLevel[i].Name = i.ToString(System.Globalization.CultureInfo.InvariantCulture);
				this.radioLevel[i].Text = i.ToString();
				this.radioLevel[i].Group = "Level";
				this.radioLevel[i].Clicked += new MessageEventHandler(this.HandleRadioLevelClicked);
			}
			this.radioLevel[0].Text = Res.Strings.TextPanel.Generator.Radio.Global;
			this.radioLevel[0].ActiveState = ActiveState.Yes;

			this.fieldPrefix = new TextFieldCombo(this);
			this.fieldPrefix.AutoFocus = false;
			this.fieldPrefix.TextChanged += new EventHandler(this.HandlePrefixChanged);
			this.fieldPrefix.TabIndex = this.tabIndex++;
			this.fieldPrefix.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.fieldPrefix.Items.Add(" ");
			this.fieldPrefix.Items.Add("o");
			this.fieldPrefix.Items.Add("-");
			this.fieldPrefix.Items.Add("(");
			ToolTip.Default.SetToolTip(this.fieldPrefix, Res.Strings.TextPanel.Generator.Tooltip.Prefix);

			this.fieldGenerator = new TextFieldCombo(this);
			this.fieldGenerator.IsReadOnly = true;
			this.fieldGenerator.AutoFocus = false;
			this.fieldGenerator.TextChanged += new EventHandler(this.HandleGeneratorChanged);
			this.fieldGenerator.TabIndex = this.tabIndex++;
			this.fieldGenerator.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.fieldGenerator.Items.Add(" ");
			this.fieldGenerator.Items.Add("1, 2...");
			this.fieldGenerator.Items.Add("A, B...");
			this.fieldGenerator.Items.Add("a, b...");
			this.fieldGenerator.Items.Add("i, ii...");
			this.fieldGenerator.Items.Add("I, II...");
			ToolTip.Default.SetToolTip(this.fieldGenerator, Res.Strings.TextPanel.Generator.Tooltip.Generator);

			this.fieldSuffix = new TextFieldCombo(this);
			this.fieldSuffix.AutoFocus = false;
			this.fieldSuffix.TextChanged += new EventHandler(this.HandleSuffixChanged);
			this.fieldSuffix.TabIndex = this.tabIndex++;
			this.fieldSuffix.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.fieldSuffix.Items.Add(" ");
			this.fieldSuffix.Items.Add(".");
			this.fieldSuffix.Items.Add("-");
			this.fieldSuffix.Items.Add(")");
			ToolTip.Default.SetToolTip(this.fieldSuffix, Res.Strings.TextPanel.Generator.Tooltip.Suffix);

			this.buttonClear = this.CreateClearButton(new MessageEventHandler(this.HandleClearClicked));

			this.ParagraphWrapper.Active.Changed  += new EventHandler(this.HandleWrapperChanged);
			this.ParagraphWrapper.Defined.Changed += new EventHandler(this.HandleWrapperChanged);

			this.Type = 0;
			this.Level = 0;

			this.isNormalAndExtended = true;
			this.UpdateAfterChanging();
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.ParagraphWrapper.Active.Changed  -= new EventHandler(this.HandleWrapperChanged);
				this.ParagraphWrapper.Defined.Changed -= new EventHandler(this.HandleWrapperChanged);
			}
			
			base.Dispose(disposing);
		}

		
		public override void UpdateAfterAttach()
		{
			//	Mise à jour après avoir attaché le wrappers.
			this.UpdateButtonClear();
		}


		public override double DefaultHeight
		{
			//	Retourne la hauteur standard.
			get
			{
				double h = this.LabelHeight;

				if ( this.isExtendedSize )  // panneau étendu ?
				{
					h += 105;
				}
				else	// panneau réduit ?
				{
					h += 30;
				}

				return h;
			}
		}


		protected void HandleWrapperChanged(object sender)
		{
			//	Le wrapper associé a changé.
			this.UpdateAfterChanging();
		}

		
		protected override void UpdateClientGeometry()
		{
			//	Met à jour la géométrie.
			base.UpdateClientGeometry();

			if ( this.buttonClear == null )  return;

			Rectangle rect = this.UsefulZone;
			Rectangle r;

			r = rect;
			r.Bottom = r.Top-20;
			r.Right = rect.Right-25;
			this.fieldType.Bounds = r;
			r.Left = rect.Right-20;
			r.Right = rect.Right;
			this.buttonClear.Bounds = r;

			if ( this.isExtendedSize )  // panneau étendu ?
			{
				r.Offset(0, -25);
				r.Left = rect.Left;
				r.Width = 31;
				r.Bottom = r.Top-20;
				for ( int i=0 ; i<Generator.maxLevel ; i++ )
				{
					this.radioLevel[i].Bounds = r;
					this.radioLevel[i].Visibility = true;
					r.Offset(r.Width, 0);
				}

				r.Offset(0, -25);
				r.Left = rect.Left;
				r.Width = 56;
				this.fieldPrefix.Bounds = r;
				this.fieldPrefix.Visibility = true;
				r.Offset(r.Width+5, 0);
				this.fieldGenerator.Bounds = r;
				this.fieldGenerator.Visibility = true;
				r.Offset(r.Width+5, 0);
				this.fieldSuffix.Bounds = r;
				this.fieldSuffix.Visibility = true;
			}
			else
			{
				for ( int i=0 ; i<Generator.maxLevel ; i++ )
				{
					this.radioLevel[i].Visibility = false;
				}

				this.fieldPrefix.Visibility = false;
				this.fieldGenerator.Visibility = false;
				this.fieldSuffix.Visibility = false;
			}

			this.UpdateButtonClear();
		}

		protected void UpdateButtonClear()
		{
			this.buttonClear.Visibility = !this.ParagraphWrapper.IsAttachedToDefaultParagraphStyle;
		}

		protected override void UpdateAfterChanging()
		{
			//	Met à jour après un changement du wrapper.
			base.UpdateAfterChanging();
			
			if ( this.ParagraphWrapper.IsAttached == false )  return;

			double leftFirst = this.ParagraphWrapper.Active.LeftMarginFirst;
			double leftBody  = this.ParagraphWrapper.Active.LeftMarginBody;
			double right     = this.ParagraphWrapper.Active.RightMarginBody;
			bool isLeftFirst = this.ParagraphWrapper.Defined.IsLeftMarginFirstDefined;
			bool isLeftBody  = this.ParagraphWrapper.Defined.IsLeftMarginBodyDefined;
			bool isRight     = this.ParagraphWrapper.Defined.IsRightMarginBodyDefined;

			this.ignoreChanged = true;


			this.ignoreChanged = false;
		}


		protected void UpdateWidgets()
		{
			bool custom = (this.type == 6);

			for ( int i=0 ; i<Generator.maxLevel ; i++ )
			{
				this.radioLevel[i].Enable = custom;
			}

			this.fieldPrefix.Enable = custom;
			this.fieldGenerator.Enable = (custom && this.level != 0);
			this.fieldSuffix.Enable = custom;
		}

		protected int Type
		{
			get
			{
				return this.type;
			}

			set
			{
				if ( this.type != value )
				{
					this.type = value;
					this.UpdateWidgets();
				}
			}
		}

		protected int Level
		{
			get
			{
				return this.level;
			}

			set
			{
				if ( this.level != value )
				{
					this.level = value;
					this.UpdateWidgets();
				}
			}
		}


		private void HandleTypeChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.ParagraphWrapper.IsAttached )  return;

			TextFieldCombo combo = sender as TextFieldCombo;
			this.Type = combo.SelectedIndex;
		}

		private void HandleRadioLevelClicked(object sender, MessageEventArgs e)
		{
			RadioButton button = sender as RadioButton;
			int level = int.Parse(button.Name);
			this.Level = level;
		}

		private void HandlePrefixChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.ParagraphWrapper.IsAttached )  return;

		}

		private void HandleGeneratorChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.ParagraphWrapper.IsAttached )  return;

		}

		private void HandleSuffixChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.ParagraphWrapper.IsAttached )  return;

		}

		private void HandleClearClicked(object sender, MessageEventArgs e)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.ParagraphWrapper.IsAttached )  return;

			this.ParagraphWrapper.SuspendSynchronizations();
			this.ParagraphWrapper.Defined.ClearLeftMarginFirst();
			this.ParagraphWrapper.Defined.ClearLeftMarginBody();
			this.ParagraphWrapper.Defined.ClearRightMarginFirst();
			this.ParagraphWrapper.Defined.ClearRightMarginBody();
			this.ParagraphWrapper.Defined.ClearMarginUnits();
			this.ParagraphWrapper.Defined.ClearIndentationLevel();
			this.ParagraphWrapper.Defined.ClearIndentationLevelAttribute();
			this.ParagraphWrapper.DefineOperationName("ParagraphMarginsClear", Res.Strings.TextPanel.Clear);
			this.ParagraphWrapper.ResumeSynchronizations();
			this.document.IsDirtySerialize = true;
		}


		protected static readonly int		maxLevel = 6;

		protected RadioButton[]				radioLevel;
		protected TextFieldCombo			fieldType;
		protected TextFieldCombo			fieldPrefix;
		protected TextFieldCombo			fieldGenerator;
		protected TextFieldCombo			fieldSuffix;
		protected IconButton				buttonClear;

		protected int						type = -1;
		protected int						level = -1;
	}
}
