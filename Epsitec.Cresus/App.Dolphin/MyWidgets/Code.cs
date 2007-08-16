//	Copyright © 2003-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
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

			this.valueAddress = -1;

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
			this.widgetInstruction.ButtonShowCondition = ShowCondition.WhenModified;
			this.widgetInstruction.DefocusAction = DefocusAction.AutoAcceptOrRejectEdition;
			this.widgetInstruction.PreferredHeight = 20;
			this.widgetInstruction.PreferredWidth = 150;
			this.widgetInstruction.Margins = new Margins(0, 0, 0, 0);
			this.widgetInstruction.Dock = DockStyle.Left;
			this.widgetInstruction.EditionAccepted += new EventHandler(this.HandleInstructionEditionAccepted);
			this.widgetInstruction.EditionRejected += new EventHandler(this.HandleInstructionEditionRejected);
			this.widgetInstruction.IsFocusedChanged += new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleFieldIsFocusedChanged);
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

				this.widgetInstruction.EditionAccepted -= new EventHandler(this.HandleInstructionEditionAccepted);
				this.widgetInstruction.EditionRejected -= new EventHandler(this.HandleInstructionEditionRejected);
			}

			base.Dispose(disposing);
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

		public void SetCode(int address, List<int> codes, bool isRom)
		{
			//	Spécifie les codes de l'instruction représenté par ce widget.
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

			this.widgetInstruction.Text = this.processor.DessassemblyInstruction(this.valueCodes);

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
			Color color = this.BackColor;
			if (!color.IsEmpty)
			{
				graphics.AddFilledRectangle(this.Client.Bounds);
				graphics.RenderSolid(color);  // dessine un fond rouge si MarkPC
			}
		}


		private void HandleInstructionEditionAccepted(object sender)
		{
			//	L'édition de l'instruction a été acceptée.
			//?System.Diagnostics.Debug.WriteLine("HandleInstructionEditionAccepted");
			TextFieldEx field = sender as TextFieldEx;
			field.AcceptEdition();

			List<int> codes = new List<int>();
			string err = this.processor.AssemblyInstruction(this.widgetInstruction.Text, codes);

			if (codes == null || codes.Count == 0 || !string.IsNullOrEmpty(err))
			{
				this.isErrorMet = true;

				string title = "Dauphin";
				string icon = "manifest:Epsitec.Common.Dialogs.Images.Warning.icon";
				Common.Dialogs.IDialog dialog = Common.Dialogs.MessageDialog.CreateOk(title, icon, err, null, null);
				dialog.Owner = this.Window;
				dialog.OpenDialog();
			}
			else
			{
				this.isErrorMet = false;

				this.valueCodes = codes;
				this.OnInstructionChanged();
			}
		}

		private void HandleInstructionEditionRejected(object sender)
		{
			//	L'édition de l'instruction a été rejetée.
			//?System.Diagnostics.Debug.WriteLine("HandleInstructionEditionRejected");
			TextFieldEx field = sender as TextFieldEx;
			field.RejectEdition();
		}

		private void HandleFieldIsFocusedChanged(object sender, Common.Types.DependencyPropertyChangedEventArgs e)
		{
			//	La ligne éditable a pris ou perdu le focus.
			Widget widget = sender as Widget;
			bool focused = (bool) e.NewValue;
			//?System.Diagnostics.Debug.WriteLine(string.Format("HandleFieldIsFocusedChanged {0}", focused.ToString()));

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
		#endregion


		protected static readonly int			maxCodes = 4;

		protected Components.AbstractProcessor	processor;
		protected CodeAccessor					codeAccessor;
		protected int							valueAddress;
		protected List<int>						valueCodes;
		protected bool							isErrorMet;

		protected StaticText					widgetAddress;
		protected List<TextField>				widgetCodes;
		protected TextFieldEx					widgetInstruction;
	}
}
