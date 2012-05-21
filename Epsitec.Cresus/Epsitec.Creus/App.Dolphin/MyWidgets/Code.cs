//	Copyright © 2003-2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.App.Dolphin.MyWidgets
{
	/// <summary>
	/// Permet d'éditer une instruction codée.
	/// </summary>
	public class Code : AbstractGroup
	{
		public Code() : base()
		{
			this.PreferredHeight = 20;

			this.valueAddress = Misc.undefined;

			this.widgetAddress = new StaticText(this);
			this.widgetAddress.ContentAlignment = ContentAlignment.MiddleLeft;
			this.widgetAddress.PreferredHeight = 20;
			this.widgetAddress.PreferredWidth = 25;
			this.widgetAddress.Margins = new Margins(5, 3, 0, 0);
			this.widgetAddress.Dock = DockStyle.Left;

			MyWidgets.Panel groupCodes = new Panel(this);
			groupCodes.PreferredSize = new Size(20*Code.maxCodes, 20);
			groupCodes.Margins = new Margins(0, 0, 0, 0);
			groupCodes.Dock = DockStyle.Left;

			this.widgetCodes = new List<TextField>();
			for (int i=0; i<Code.maxCodes; i++)
			{
				TextField code = new TextField(groupCodes);
				code.IsReadOnly = true;
				code.PreferredSize = new Size(20, 20);
				code.Margins = new Margins(0, -1, 0, 0);
				code.Dock = DockStyle.Left;

				this.widgetCodes.Add(code);
			}

			this.widgetInstruction = new TextFieldEx(this);
			this.widgetInstruction.ButtonShowCondition = ButtonShowCondition.WhenModified;
			this.widgetInstruction.DefocusAction = DefocusAction.AutoAcceptOrRejectEdition;
			this.widgetInstruction.TextNavigator.AllowTabInsertion = true;
			this.widgetInstruction.PreferredHeight = 20;
			this.widgetInstruction.PreferredWidth = 150;
			this.widgetInstruction.Margins = new Margins(0, 0, 0, 0);
			this.widgetInstruction.Dock = DockStyle.Left;
			this.widgetInstruction.AcceptingEdition += new EventHandler<CancelEventArgs> (this.HandleInstructionAcceptingEdition);
			this.widgetInstruction.EditionAccepted += new EventHandler (this.HandleInstructionEditionAccepted);
			this.widgetInstruction.IsFocusedChanged += new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleFieldIsFocusedChanged);

			TextStyle.Tab tab = new TextStyle.Tab(50, TextTabType.Left, TextTabLine.None);
			this.widgetInstruction.TextLayout.TabInsert(tab);

			this.widgetCodeAddress = new MyWidgets.CodeAddress(this);
			this.widgetCodeAddress.PreferredHeight = 20;
			this.widgetCodeAddress.Dock = DockStyle.Fill;
			this.widgetCodeAddress.Entered += this.HandleCodeAddressEntered;
			this.widgetCodeAddress.Exited += this.HandleCodeAddressExited;
			this.widgetCodeAddress.Clicked += this.HandleCodeAddressClicked;
		}

		public Code(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.widgetAddress.Dispose();

				this.widgetInstruction.AcceptingEdition -= new EventHandler<CancelEventArgs> (this.HandleInstructionAcceptingEdition);
				this.widgetInstruction.EditionAccepted -= new EventHandler (this.HandleInstructionEditionAccepted);

				this.widgetCodeAddress.Entered -= this.HandleCodeAddressEntered;
				this.widgetCodeAddress.Exited -= this.HandleCodeAddressExited;
				this.widgetCodeAddress.Clicked -= this.HandleCodeAddressClicked;
			}

			base.Dispose(disposing);
		}


		public bool IsBackHilite
		{
			//	Détermine si le widget à un fond mis en évidence.
			get
			{
				return this.isBackHilite;
			}
			set
			{
				if (this.isBackHilite != value)
				{
					this.isBackHilite = value;
					this.Invalidate();
				}
			}
		}

		public void SetTabIndex(int index)
		{
			//	Spécifie l'ordre pour la navigation avec Tab.
			//	Attention, il ne doit pas y avoir 2x les mêmes numéros, même dans des widgets de parents différents !
			this.widgetInstruction.TabIndex = index;
			this.widgetInstruction.TabNavigationMode = TabNavigationMode.None;  // gestion maison, dans MainPanel
		}

		public Components.AbstractProcessor Processor
		{
			//	Processeur émulé affichée/modifée par ce widget.
			get
			{
				return this.processor;
			}
			set
			{
				this.processor = value;
			}
		}

		public CodeAccessor CodeAccessor
		{
			//	CodeAccessor associé au widget, facultatif.
			get
			{
				return this.codeAccessor;
			}
			set
			{
				this.codeAccessor = value;
			}
		}

		public CodeAddress CodeAddress
		{
			//	CodeAddress associé au widget.
			get
			{
				return this.widgetCodeAddress;
			}
		}

		public void SetCode(int address, List<int> codes, bool isTable, bool isRom)
		{
			//	Spécifie les codes de l'instruction représenté par ce widget.
			this.isTable = isTable;

			if (this.valueAddress != address)
			{
				this.valueAddress = address;
				this.widgetAddress.Text = this.valueAddress.ToString("X3");
			}

			this.valueCodes = new List<int>();
			for (int i=0; i<this.widgetCodes.Count; i++)
			{
				if (i < codes.Count)
				{
					this.valueCodes.Add(codes[i]);

					this.widgetCodes[i].Visibility = true;
					this.widgetCodes[i].Text = codes[i].ToString("X2");
				}
				else
				{
					this.widgetCodes[i].Visibility = false;
					this.widgetCodes[i].Text = "";
				}
			}

			this.arrowAddress = Misc.undefined;
			if (this.isTable)
			{
				this.widgetInstruction.Text = string.Concat("BYTE<tab/>#H'", this.valueCodes[0].ToString("X2"));
			}
			else
			{
				this.widgetInstruction.Text = this.processor.DessassemblyInstruction(this.valueCodes, address, out this.arrowAddress);
			}

			if (this.widgetInstruction.IsReadOnly != isRom)  // TODO: devrait être inutile (bug à corriger pour Pierre)
			{
				this.widgetInstruction.IsReadOnly = isRom;
				this.widgetInstruction.Invalidate();
			}
		}

		public void GetCode(out int address, List<int> codes)
		{
			//	Retourne les codes de l'instruction représenté par ce widget.
			address = this.valueAddress;

			codes.Clear();
			foreach (int code in this.valueCodes)
			{
				codes.Add(code);
			}
		}

		public int Address
		{
			//	Adresse source, là où part la flèche.
			get
			{
				return this.valueAddress;
			}
		}

		public int ArrowAddress
		{
			//	Adresse de destination, là où arrive la flèche.
			get
			{
				return this.arrowAddress;
			}
		}

		public bool IsErrorMet
		{
			// Indique si une erreur a été rencontrée lors du dernier EditionAccepted.
			get
			{
				return this.isErrorMet;
			}
		}


		protected MainPanel MainPanel
		{
			//	Retourne le premier parent de type MainPanel.
			get
			{
				Widget my = this;

				while (my.Parent != null)
				{
					my = my.Parent;
					if (my is MainPanel)
					{
						return my as MainPanel;
					}
				}

				return null;
			}
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			if (this.isBackHilite)
			{
				graphics.AddFilledRectangle(this.Client.Bounds);
				graphics.RenderSolid(DolphinApplication.ColorHilite);  // dessine un fond rouge si MarkPC
			}
		}


		private void HandleInstructionAcceptingEdition(object sender, CancelEventArgs e)
		{
			//	Avant de valider la saisie, la ligne éditable nous donne la
			//	possibilité de tout annuler en cas d'erreur de syntaxe.
			List<int> codes = new List<int>();
			string instruction = this.processor.AssemblyPreprocess(this.widgetInstruction.Text);
			string err = this.processor.AssemblyInstruction(instruction, codes);

			if (!string.IsNullOrEmpty(err))
			{
				Message message = Message.GetLastMessage();
				message.Consumer = this;
				message.Swallowed = true;

				this.isErrorMet = true;

				//	On ne peut pas ouvrir un dialogue depuis une méthode qui est appelée
				//	depuis l'intérieur d'une boucle d'événements, sinon on risque de se
				//	mélanger gaiement les pinceaux avec le dispatch des événements.
				//	C'est malheureusement fait dans un tas de code de "Epsitec.Cresus",
				//	où ça ne pose par chance aucun problème visible; par contre, ici, ce
				//	n'est vraiment pas possible.
				//	On va donc simplement demander à l'application d'afficher le dialogue
				//	plus tard, avant la prochaine entrée dans la boucle des événements :

				if (!this.isErrorDialogPending)
				{
					//	Evite que le dialogue ne s'affiche deux fois; dans certains cas,
					//	il se peut en effet que notre méthode soit appelée plusieurs fois
					//	de suite :

					this.isErrorDialogPending = true;

					Application.QueueAsyncCallback(
						delegate
						{
							string title = TextLayout.ConvertToSimpleText(Res.Strings.Window.Title);
							string icon = "manifest:Epsitec.Common.Dialogs.Images.Warning.icon";
							Common.Dialogs.IDialog dialog = Common.Dialogs.MessageDialog.CreateOk(title, icon, err, null, null);
							dialog.OwnerWindow = this.Window;
							dialog.OpenDialog();
							this.isErrorDialogPending = false;
						});
				}

				e.Cancel = true;
			}
		}

		private void HandleInstructionEditionAccepted(object sender)
		{
			//	L'édition de l'instruction a été acceptée.
			TextFieldEx field = sender as TextFieldEx;

			List<int> codes = new List<int>();
			string instruction = this.processor.AssemblyPreprocess(this.widgetInstruction.Text);
			string err = this.processor.AssemblyInstruction(instruction, codes);

			if (!string.IsNullOrEmpty(err))
			{
				//	Cela ne devrait jamais se produire, puisque l'on a déjà tout vérifié
				//	dans la méthode HandleInstructionAcceptingEdition :
				this.isErrorMet = true;
			}
			else
			{
				this.isErrorMet = false;

				this.valueCodes = codes;
				this.OnInstructionChanged();
			}
		}

		private void HandleFieldIsFocusedChanged(object sender, Common.Types.DependencyPropertyChangedEventArgs e)
		{
			//	La ligne éditable a pris ou perdu le focus.
			Widget widget = sender as Widget;
			bool focused = (bool) e.NewValue;

			if (focused)  // focus pris ?
			{
				this.isErrorMet = false;

				MainPanel mp = this.MainPanel;
				if (mp != null)
				{
					mp.SetDolphinFocusedWidget(widget);
				}

				this.OnInstructionSelected();
			}
			else  // focus perdu ?
			{
				this.OnInstructionDeselected();
			}
		}

		private void HandleCodeAddressEntered(object sender, MessageEventArgs e)
		{
			this.OnCodeAddressEntered();
		}

		private void HandleCodeAddressExited(object sender, MessageEventArgs e)
		{
			this.OnCodeAddressExited();
		}

		private void HandleCodeAddressClicked(object sender, MessageEventArgs e)
		{
			this.OnCodeAddressClicked();
		}


		#region EventHandler
		protected virtual void OnInstructionSelected()
		{
			//	Génère un événement pour dire qu'une cellule a été sélectionnée.
			EventHandler handler = (EventHandler) this.GetUserEventHandler("InstructionSelected");
			if (handler != null)
			{
				handler(this);
			}
		}

		public event Common.Support.EventHandler InstructionSelected
		{
			add
			{
				this.AddUserEventHandler("InstructionSelected", value);
			}
			remove
			{
				this.RemoveUserEventHandler("InstructionSelected", value);
			}
		}

		protected virtual void OnInstructionDeselected()
		{
			//	Génère un événement pour dire qu'une cellule a été sélectionnée.
			EventHandler handler = (EventHandler) this.GetUserEventHandler("InstructionDeselected");
			if (handler != null)
			{
				handler(this);
			}
		}

		public event Common.Support.EventHandler InstructionDeselected
		{
			add
			{
				this.AddUserEventHandler("InstructionDeselected", value);
			}
			remove
			{
				this.RemoveUserEventHandler("InstructionDeselected", value);
			}
		}

		protected virtual void OnInstructionChanged()
		{
			//	Génère un événement pour dire qu'une cellule a été sélectionnée.
			EventHandler handler = (EventHandler) this.GetUserEventHandler("InstructionChanged");
			if (handler != null)
			{
				handler(this);
			}
		}

		public event Common.Support.EventHandler InstructionChanged
		{
			add
			{
				this.AddUserEventHandler("InstructionChanged", value);
			}
			remove
			{
				this.RemoveUserEventHandler("InstructionChanged", value);
			}
		}

		protected virtual void OnCodeAddressEntered()
		{
			//	Génère un événement pour dire qu'une cellule a été sélectionnée.
			EventHandler handler = (EventHandler) this.GetUserEventHandler("CodeAddressEntered");
			if (handler != null)
			{
				handler(this);
			}
		}

		public event Common.Support.EventHandler CodeAddressEntered
		{
			add
			{
				this.AddUserEventHandler("CodeAddressEntered", value);
			}
			remove
			{
				this.RemoveUserEventHandler("CodeAddressEntered", value);
			}
		}

		protected virtual void OnCodeAddressExited()
		{
			//	Génère un événement pour dire qu'une cellule a été sélectionnée.
			EventHandler handler = (EventHandler) this.GetUserEventHandler("CodeAddressExited");
			if (handler != null)
			{
				handler(this);
			}
		}

		public event Common.Support.EventHandler CodeAddressExited
		{
			add
			{
				this.AddUserEventHandler("CodeAddressExited", value);
			}
			remove
			{
				this.RemoveUserEventHandler("CodeAddressExited", value);
			}
		}

		protected virtual void OnCodeAddressClicked()
		{
			//	Génère un événement pour dire qu'une cellule a été sélectionnée.
			EventHandler handler = (EventHandler) this.GetUserEventHandler("CodeAddressClicked");
			if (handler != null)
			{
				handler(this);
			}
		}

		public event Common.Support.EventHandler CodeAddressClicked
		{
			add
			{
				this.AddUserEventHandler("CodeAddressClicked", value);
			}
			remove
			{
				this.RemoveUserEventHandler("CodeAddressClicked", value);
			}
		}
		#endregion


		protected static readonly int			maxCodes = 4;

		protected Components.AbstractProcessor	processor;
		protected CodeAccessor					codeAccessor;
		protected int							valueAddress;
		protected List<int>						valueCodes;
		protected int							arrowAddress;
		protected bool							isTable;
		protected bool							isErrorMet;
		protected bool							isErrorDialogPending;
		protected bool							isBackHilite;

		protected StaticText					widgetAddress;
		protected List<TextField>				widgetCodes;
		protected TextFieldEx					widgetInstruction;
		protected MyWidgets.CodeAddress			widgetCodeAddress;
	}
}
