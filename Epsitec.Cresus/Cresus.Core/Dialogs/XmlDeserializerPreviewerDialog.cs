//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Debug;
using Epsitec.Common.Dialogs;
using Epsitec.Common.Drawing;
using Epsitec.Common.IO;
using Epsitec.Common.Printing;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Printers;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Business.UserManagement;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Epsitec.Cresus.Core.Dialogs
{
	/// <summary>
	/// Dialogue pour choisir l'utilisateur (loggin).
	/// </summary>
	class XmlDeserializerPreviewerDialog : AbstractDialog
	{
		public XmlDeserializerPreviewerDialog(CoreApplication application, string xmlSource)
		{
			this.IsApplicationWindow = true;  // pour avoir les boutons Minimize/Maximize/Close !

			this.application = application;
			this.xmlSource   = xmlSource;

			this.jobs = Printers.PrintEngine.DeserializeJobs (this.xmlSource, zoom: 2);
			this.pages = this.Pages.ToList ();
		}


		protected override Window CreateWindow()
		{
			Window window = new Window ();

			this.SetupWindow  (window);
			this.SetupWidgets (window);
			this.SetupEvents  (window);

			window.AdjustWindowSize ();

			return window;
		}

		protected void SetupWindow(Window window)
		{
			this.OwnerWindow = this.application.Window;
			window.Icon = this.application.Window.Icon;
			window.Text = "Visualisation de la désérialisation d'un document";
			window.ClientSize = new Size (1024, 768);
			window.Root.WindowStyles = WindowStyles.DefaultDocumentWindow;  // pour avoir les boutons Minimize/Maximize/Close !

			window.WindowCloseClicked += delegate
			{
				this.OnDialogClosed ();
				this.CloseDialog ();
			};
		}

		protected void SetupWidgets(Window window)
		{
			int tabIndex = 1;

			var topPane = new FrameBox
			{
				Parent = window.Root,
				ContainerLayoutMode = Common.Widgets.ContainerLayoutMode.HorizontalFlow,
				Dock = DockStyle.Fill,
				Margins = new Margins (10, 10, 10, 0),
				TabIndex = tabIndex++,
			};

			var leftPane = new FrameBox
			{
				Parent = topPane,
				ContainerLayoutMode = Common.Widgets.ContainerLayoutMode.VerticalFlow,
				Dock = DockStyle.Fill,
				Margins = new Margins (0, 10, 0, 0),
				TabIndex = tabIndex++,
			};

			var footer = new FrameBox
			{
				Parent = window.Root,
				PreferredHeight = 20,
				Dock = DockStyle.Bottom,
				Margins = new Margins (10, 10, 10, 10),
				TabIndex = tabIndex++,
			};

			//	Crée la partie principale.
			this.textField = new TextFieldMulti
			{
				Parent = leftPane,
				MaxLength = 100000,
				Text = TextLayout.ConvertToTaggedText (this.xmlSource),
				IsReadOnly = true,
				Dock = DockStyle.Fill,
				Margins = new Margins (0, 0, 0, 0),
			};

			this.description = new TextFieldMulti
			{
				Parent = leftPane,
				IsReadOnly = true,
				PreferredHeight = 100,
				Dock = DockStyle.Bottom,
				Margins = new Margins (0, 0, 10, 0),
			};

			this.previewer = new XmlDeserializerPreviewer
			{
				Parent = topPane,
				Dock = DockStyle.Fill,
			};

			//	Crée le pied de page.
			{
				this.closeButton = new Button
				{
					Parent = footer,
					Text = "Fermer",
					ButtonStyle = Common.Widgets.ButtonStyle.DefaultCancel,
					Dock = DockStyle.Right,
					Margins = new Margins (20, 0, 0, 0),
					TabIndex = tabIndex++,
				};

				var next = new GlyphButton
				{
					Parent = footer,
					GlyphShape = GlyphShape.ArrowRight,
					PreferredWidth = 30,
					Dock = DockStyle.Right,
					Margins = new Margins (1, 0, 0, 0),
				};

				var prev = new GlyphButton
				{
					Parent = footer,
					GlyphShape = GlyphShape.ArrowLeft,
					PreferredWidth = 30,
					Dock = DockStyle.Right,
					Margins = new Margins (20, 0, 0, 0),
				};

				next.Clicked += delegate
				{
					this.ChangePage (1);
				};

				prev.Clicked += delegate
				{
					this.ChangePage (-1);
				};

				var b = new Button
				{
					Parent = footer,
					Text = "Re-print",
					Dock = DockStyle.Right,
					Margins = new Margins (0, 20, 0, 0),
				};

				b.Clicked += delegate
				{
					PrintEngine.DeserializeAndPrintJobs (this.xmlSource);
				};
			}

			this.UpdateWidgets ();
		}

		protected void SetupEvents(Window window)
		{
			this.closeButton.Clicked += delegate
			{
				this.CloseAction (cancel: true);
			};
		}

		private void CloseAction(bool cancel)
		{
			if (cancel)
			{
				this.Result = DialogResult.Cancel;
			}
			else
			{
				this.Result = DialogResult.Accept;
			}

			this.CloseDialog ();
		}

		private void ChangePage(int direction)
		{
			this.page += direction;

			this.page = System.Math.Max (this.page, 0);
			this.page = System.Math.Min (this.page, this.pages.Count-1);

			this.UpdateWidgets ();
		}

		private void UpdateWidgets()
		{
			if (this.page < 0 || this.page >= this.pages.Count)
			{
				this.description.Text = null;
				this.previewer.Bitmap = null;
			}
			else
			{
				this.description.Text = this.pages[this.page].FullDescription;
				this.previewer.Bitmap = this.pages[this.page].Miniature;
			}
		}


		protected override void OnDialogClosed()
		{
			base.OnDialogClosed ();
		}


		private IEnumerable<DeserializedPage> Pages
		{
			get
			{
				foreach (var job in this.jobs)
				{
					foreach (var section in job.Sections)
					{
						foreach (var page in section.Pages)
						{
							yield return page;
						}
					}
				}
			}
		}


		private readonly CoreApplication						application;
		private readonly string									xmlSource;

		private List<Printers.DeserializedJob>					jobs;
		private List<Printers.DeserializedPage>					pages;
		private int												page;

		private TextFieldMulti									textField;
		private TextFieldMulti									description;
		private XmlDeserializerPreviewer						previewer;
		private Button											closeButton;
	}
}
