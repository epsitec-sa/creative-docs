//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

namespace Epsitec.Cresus.Core.Controllers
{
	public abstract class EntityViewController : CoreViewController
	{
		public EntityViewController(string name)
			: base (name)
		{
		}

		public AbstractEntity Entity
		{
			get;
			set;
		}

		public ViewControllerMode Mode
		{
			get;
			set;
		}

		public Orchestrators.DataViewOrchestrator Orchestrator
		{
			get;
			set;
		}


		public static EntityViewController CreateViewController(string name, AbstractEntity entity, ViewControllerMode mode, Orchestrators.DataViewOrchestrator orchestrator)
		{
			EntityViewController controller = EntityViewController.ResolveViewController (name, entity, mode);

			if (controller == null)
			{
				return null;
			}

			controller.Entity = entity;
			controller.Mode = mode;
			controller.Orchestrator = orchestrator;

			return controller;
		}
		
		private static EntityViewController ResolveViewController(string name, AbstractEntity entity, ViewControllerMode mode)
		{
			if (mode == ViewControllerMode.None)
			{
				return null;
			}

			if (entity is Entities.NaturalPersonEntity)
			{
				return new NaturalPersonViewController (name);
			}

			if (entity is Entities.LegalPersonEntity)
			{
				return new LegalPersonViewController (name);
			}

			if (entity is Entities.MailContactEntity)
			{
				return new MailContactViewController (name);
			}

			if (entity is Entities.TelecomContactEntity)
			{
				return new TelecomViewController (name);
			}

			if (entity is Entities.UriContactEntity)
			{
				return new UriViewController (name);
			}

			// TODO: Compléter ici au fur et à mesure des besoins...

			return null;
		}


		/// <summary>
		/// Crée une tuile résumée qui s'insère en bas de l'empilement (qui commence en haut).
		/// </summary>
		/// <param name="accessor">The accessor.</param>
		/// <param name="groupIndex">Index of the group.</param>
		/// <param name="compactFollower">if set to <c>true</c> [compact follower].</param>
		/// <param name="childrenMode">The children mode.</param>
		protected void CreateSummaryTile(EntitiesAccessors.AbstractAccessor accessor, int groupIndex, bool compactFollower, ViewControllerMode childrenMode)
		{
			System.Diagnostics.Debug.Assert (this.container != null);

			var tile = new Widgets.SummaryTile
			{
				Parent = this.container,
				Dock = DockStyle.Top,
				ArrowLocation = Direction.Right,
				EnteredSensitivity = childrenMode != ViewControllerMode.None,
				Entity = accessor.Entity,
				Mode = this.Mode,
				ChildrenMode = childrenMode,
				GroupIndex = groupIndex,
				CompactFollower = compactFollower,
				TopLeftIconUri = accessor.Icon,
				Title = accessor.Title,
				Summary = accessor.Summary,
			};

			tile.PreferredHeight = tile.ContentHeight;
			tile.Clicked += new EventHandler<MessageEventArgs> (this.HandleTileClicked);
		}

		/// <summary>
		/// Crée une tuile permettant l'édition. En principe, elle est seule dans son empilement.
		/// </summary>
		/// <param name="accessor">The accessor.</param>
		/// <param name="childrenMode">The children mode.</param>
		/// <returns></returns>
		protected FrameBox CreateEditionTile(EntitiesAccessors.AbstractAccessor accessor, ViewControllerMode childrenMode)
		{
			System.Diagnostics.Debug.Assert (this.container != null);

			var tile = new Widgets.EditionTile
			{
				Parent = this.container,
				Dock = DockStyle.Top,
				ArrowLocation = Direction.Right,
				EnteredSensitivity = childrenMode != ViewControllerMode.None,
				Entity = accessor.Entity,
				Mode = this.Mode,
				ChildrenMode = childrenMode,
				IsEditing = true,
				TopLeftIconUri = accessor.Icon,
				Title = accessor.Title,
			};

			tile.Clicked += new EventHandler<MessageEventArgs> (this.HandleTileClicked);

			return tile.Container;
		}


		/// <summary>
		/// Ajuste l'aspect visuel pour former des groupes bien distincts.
		/// Les traits horizontaux de séparation à l'intérieur des groupes sont supprimés.
		/// Les différents groupes sont espacés.
		/// </summary>
		protected void AdjustVisualForGroups()
		{
			System.Diagnostics.Debug.Assert (this.container != null);

			bool first = true;
			for (int i = 0; i < this.container.Children.Count; i++)
			{
				var currentTile = this.container.Children[i] as Widgets.AbstractTile;
				var nextTile    = (i+1 < this.container.Children.Count) ? this.container.Children[i+1] as Widgets.AbstractTile : null;
				System.Diagnostics.Debug.Assert (currentTile != null);

				if (nextTile != null && currentTile.GroupIndex == nextTile.GroupIndex && nextTile.CompactFollower)  // dans le même groupe ?
				{
					currentTile.Margins = new Margins (0, 0, 0, -1);  // léger chevauchement

					if (first)
					{
						currentTile.RectangleBordersShowed = Widgets.RectangleBordersShowedEnum.Left | Widgets.RectangleBordersShowedEnum.Right | Widgets.RectangleBordersShowedEnum.Up;
					}
					else
					{
						currentTile.RectangleBordersShowed = Widgets.RectangleBordersShowedEnum.Left | Widgets.RectangleBordersShowedEnum.Right;
					}

					first = false;
				}
				else  // dans deux groupes différents ?
				{
					currentTile.Margins = new Margins (0, 0, 0, 4);  // espacement

					if (first)
					{
						currentTile.RectangleBordersShowed = Widgets.RectangleBordersShowedEnum.All;
					}
					else
					{
						currentTile.RectangleBordersShowed = Widgets.RectangleBordersShowedEnum.Left | Widgets.RectangleBordersShowedEnum.Right | Widgets.RectangleBordersShowedEnum.Down;
					}

					first = true;
				}
			}
		}

		/// <summary>
		/// Met le focus dans la tuile éditable sur le premier widget pertinant.
		/// </summary>
		protected void SetInitialFocus()
		{
			System.Diagnostics.Debug.Assert (this.container != null);

			this.SetInitialFocus(this.container);
		}

		private bool SetInitialFocus(Widget parent)
		{
			foreach (Widget widget in parent.Children)
			{
				if (widget is AbstractTextField)
				{
					var textField = widget as AbstractTextField;

					textField.SelectAll ();
					textField.Focus ();

					return true;
				}

				if (widget.Children != null && widget.Children.Count != 0)
				{
					if (this.SetInitialFocus (widget))
					{
						return true;
					}
				}
			}

			return false;
		}


		protected FrameBox CreateGroup(Widget embedder, string label)
		{
			var staticText = new StaticText
			{
				Parent = embedder,
				Text = string.Concat (label, " :"),
				Dock = DockStyle.Top,
				Margins = new Margins (0, 10, 0, 2),
			};

			var frameBox = new FrameBox
			{
				Parent = embedder,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 10, 0, 5),
				TabIndex = ++this.tabIndex,
			};

			return frameBox;
		}

		protected void CreateTextField(Widget embedder, int width, string initialValue, System.Action<string> callback, System.Func<string, bool> validator)
		{
			var textField = new TextField
			{
				Parent = embedder,
				Text = initialValue,
				Dock = (width == 0) ? DockStyle.Fill : DockStyle.Left,
				PreferredWidth = width,
				Margins = new Margins (0, (width == 0) ? 0:2, 0, 0),
				TabIndex = ++this.tabIndex,
			};

			this.CreateTextFieldHandler (textField, callback, validator);
		}

		protected void CreateTextField(Widget embedder, int width, string label, string initialValue, System.Action<string> callback, System.Func<string, bool> validator)
		{
			var staticText = new StaticText
			{
				Parent = embedder,
				Text = string.Concat (label, " :"),
				Dock = DockStyle.Top,
				Margins = new Margins (0, 10, 0, 2),
			};

			TextField textField;

			if (width == 0)  // occupe toute la largeur ?
			{
				textField = new TextField
				{
					Parent = embedder,
					Text = initialValue,
					Dock = DockStyle.Top,
					Margins = new Margins (0, 10, 0, 5),
					TabIndex = ++this.tabIndex,
				};
			}
			else  // largeur partielle fixe ?
			{
				var box = new FrameBox
				{
					Parent = embedder,
					Dock = DockStyle.Top,
				};

				textField = new TextField
				{
					Parent = box,
					Text = initialValue,
					Dock = DockStyle.Left,
					PreferredWidth = width,
					Margins = new Margins (0, 10, 0, 5),
					TabIndex = ++this.tabIndex,
				};
			}

			this.CreateTextFieldHandler (textField, callback, validator);
		}

		protected void CreateTextFieldMulti(Widget embedder, int height, string label, string initialValue, System.Action<string> callback, System.Func<string, bool> validator)
		{
			var staticText = new StaticText
			{
				Parent = embedder,
				Text = string.Concat (label, " :"),
				Dock = DockStyle.Top,
				Margins = new Margins (0, 10, 0, 2),
			};

			var textField = new TextFieldMulti
			{
				Parent = embedder,
				Text = initialValue,
				PreferredHeight = height,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 10, 0, 5),
				TabIndex = ++this.tabIndex,
			};

			this.CreateTextFieldHandler (textField, callback, validator);
		}

		private void CreateTextFieldHandler(AbstractTextField textField, System.Action<string> callback, System.Func<string, bool> validator)
		{
			textField.TextChanged +=
				delegate (object sender)
				{
					if (validator == null || validator (textField.Text))
					{
						callback (textField.Text);
						textField.SetError (false);
					}
					else
					{
						textField.SetError (true);
					}
				};
		}

		protected void CreateMargin(Widget embedder, bool horizontalSeparator)
		{
			if (horizontalSeparator)
			{
				var separator = new Separator
				{
					Parent = embedder,
					Dock = DockStyle.Top,
					Margins = new Margins (0, 10, 10, 10),
					PreferredHeight = 2,
				};
			}
			else
			{
				var frame = new FrameBox
				{
					Parent = embedder,
					Dock = DockStyle.Top,
					PreferredHeight = 10,
				};
			}
		}


		private void SelectTile(Widgets.AbstractTile tile)
		{
			this.DeselectAllTiles ();
			tile.SetSelected (true);
		}

		private void DeselectAllTiles()
		{
			System.Diagnostics.Debug.Assert (this.container != null);

			foreach (Widget widget in this.container.Children)
			{
				var tile = widget as Widgets.AbstractTile;

				if (tile != null)
				{
					tile.SetSelected (false);
				}
			}
		}


		private void HandleTileClicked(object sender, MessageEventArgs e)
		{
			//	Appelé lorsqu'une tuile quelconque est cliquée.
			var tile = sender as Widgets.AbstractTile;
			CoreViewController controller = EntityViewController.CreateViewController ("ViewController", tile.Entity, tile.ChildrenMode, this.Orchestrator);

			if (tile.IsSelected || controller == null)
			{
				this.DeselectAllTiles ();
				this.Orchestrator.CloseSubViews (this);
			}
			else
			{
				this.SelectTile (tile);
				this.Orchestrator.ShowSubView (this, controller);
			}
		}


		protected Widget container;
		private int tabIndex;
	}
}
