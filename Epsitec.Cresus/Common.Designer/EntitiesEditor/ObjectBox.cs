using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.EntitiesEditor
{
	/// <summary>
	/// Boîte pour représenter une entité.
	/// </summary>
	public class ObjectBox : AbstractObject
	{
		public enum ConnectionAnchor
		{
			Left,
			Right,
			Bottom,
			Top,
		}


		public ObjectBox(Editor editor) : base(editor)
		{
			this.title = new TextLayout();
			this.title.DefaultFontSize = 12;
			this.title.Alignment = ContentAlignment.MiddleCenter;
			this.title.BreakMode = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine;

			this.fields = new List<Field>();

			this.columnsSeparatorRelative = 0.5;
			this.isRoot = false;
			this.isExtended = false;
		}


		public CultureMap CultureMap
		{
			get
			{
				return this.cultureMap;
			}
		}

		public string Title
		{
			//	Titre au sommet de la boîte.
			get
			{
				return this.titleString;
			}
			set
			{
				if (this.titleString != value)
				{
					this.titleString = value;

					this.title.Text = string.Concat("<b>", this.titleString, "</b>");
				}
			}
		}

		public void SetContent(CultureMap cultureMap)
		{
			//	Initialise le contenu de la boîte.
			this.cultureMap = cultureMap;

			StructuredData data = this.cultureMap.GetCultureData(Resources.DefaultTwoLetterISOLanguageName);
			IList<StructuredData> dataFields = data.GetValue(Support.Res.Fields.ResourceStructuredType.Fields) as IList<StructuredData>;

			this.fields.Clear();
			if (dataFields != null)
			{
				for (int i=0; i<dataFields.Count; i++)
				{
					Field field = new Field(this.editor);
					this.UpdateField(dataFields[i], field);
					this.fields.Add(field);
				}
			}

			this.UpdateFields();
			this.UpdateParents();
		}

		public ObjectComment Comment
		{
			//	Commentaire lié.
			get
			{
				return this.comment;
			}
			set
			{
				this.comment = value;
			}
		}

		public override Rectangle Bounds
		{
			//	Retourne la boîte de l'objet.
			//	Attention: le dessin peut déborder, par exemple pour l'ombre.
			get
			{
				return this.bounds;
			}
		}

		public void SetBounds(Rectangle bounds)
		{
			//	Modifie la boîte de l'objet.
			Point p1 = this.bounds.TopLeft;
			this.bounds = bounds;
			Point p2 = this.bounds.TopLeft;

			//	S'il existe un commentaire associé, il doit aussi être déplacé.
			if (this.comment != null)
			{
				Rectangle rect = this.comment.InternalBounds;
				rect.Offset(p2-p1);
				this.comment.SetBounds(rect);
			}
		}

		public List<Field> Fields
		{
			get
			{
				return this.fields;
			}
		}

		public Field ParentField
		{
			get
			{
				return this.parentField;
			}
			set
			{
				this.parentField = value;
			}
		}

		public bool IsRoot
		{
			//	Indique s'il s'agit de la boîte racine, c'est-à-dire de la boîte sélectionnée
			//	dans la liste de gauche.
			get
			{
				return this.isRoot;
			}
			set
			{
				if (this.isRoot != value)
				{
					this.isRoot = value;

					this.editor.Invalidate();
				}
			}
		}

		public bool IsExtended
		{
			//	Etat de la boîte (compact ou étendu).
			//	En mode compact, seul le titre est visible.
			//	En mode étendu, les champs sont visibles.
			get
			{
				return this.isExtended;
			}
			set
			{
				if (this.isExtended != value)
				{
					this.isExtended = value;

					this.UpdateFields();
					this.editor.Invalidate();
				}
			}
		}


		public double GetBestHeight()
		{
			//	Retourne la hauteur requise selon le nombre de champs définis.
			if (this.isExtended)
			{
				return AbstractObject.headerHeight + ObjectBox.fieldHeight*this.fields.Count + AbstractObject.footerHeight + 20;
			}
			else
			{
				return AbstractObject.headerHeight;
			}
		}

		public double GetConnectionSrcVerticalPosition(int rank)
		{
			//	Retourne la position verticale pour un trait de liaison.
			//	Il s'agit toujours de la position de départ d'une liaison.
			if (this.isExtended && rank < this.fields.Count)
			{
				Rectangle rect = this.GetFieldBounds(rank);
				return rect.Center.Y;
			}
			else
			{
				return this.bounds.Center.Y;
			}
		}

		public Point GetConnectionDstPosition(double posv, ConnectionAnchor anchor)
		{
			//	Retourne la position où accrocher la destination.
			//	Il s'agit toujours de la position d'arrivée d'une liaison.
			switch (anchor)
			{
				case ConnectionAnchor.Left:
					if (posv >= this.bounds.Bottom+ObjectBox.roundFrameRadius &&
						posv <= this.bounds.Top-ObjectBox.roundFrameRadius &&
						this.IsVerticalPositionFree(posv, false))
					{
						return new Point(this.bounds.Left, posv);
					}

					return new Point(this.bounds.Left, this.bounds.Top-AbstractObject.headerHeight/2);


				case ConnectionAnchor.Right:
					if (posv >= this.bounds.Bottom+ObjectBox.roundFrameRadius &&
						posv <= this.bounds.Top-ObjectBox.roundFrameRadius &&
						this.IsVerticalPositionFree(posv, true))
					{
						return new Point(this.bounds.Right, posv);
					}

					return new Point(this.bounds.Right, this.bounds.Top-AbstractObject.headerHeight/2);

				case ConnectionAnchor.Bottom:
					return new Point(this.bounds.Center.X, this.bounds.Bottom);

				case ConnectionAnchor.Top:
					return new Point(this.bounds.Center.X, this.bounds.Top);
			}

			return Point.Zero;
		}

		protected bool IsVerticalPositionFree(double posv, bool right)
		{
			//	Cherche si une position verticale n'est occupée par aucun départ de liaison.
			for (int i=0; i<this.fields.Count; i++)
			{
				Field field = this.fields[i];
				ObjectConnection connection = field.Connection;

				if (field.Relation != FieldRelation.None && connection != null)
				{
					Rectangle rect = this.GetFieldBounds(i);
					if (posv >= rect.Bottom && posv <= rect.Top)
					{
						if (field.IsExplored)
						{
							if (field.IsAttachToRight)
							{
								if (right)  return false;
							}
							else
							{
								if (!right)  return false;
							}
						}
						else
						{
							if (right)  return false;
						}
					}
				}
			}

			return true;
		}


		public override bool MouseMove(Point pos)
		{
			//	Met en évidence la boîte selon la position de la souris.
			//	Si la souris est dans cette boîte, retourne true.
			if (this.isDragging)
			{
				Rectangle bounds = this.Bounds;

				bounds.Offset(pos-this.draggingPos);
				this.draggingPos = pos;

				this.SetBounds(bounds);
				this.editor.UpdateConnections();
				return true;
			}
			else if (this.isFieldMoving)
			{
				return base.MouseMove(pos);
			}
			else if (this.isChangeWidth)
			{
				Rectangle bounds = this.Bounds;
				bounds.Width = System.Math.Max(pos.X-this.changeWidthPos+this.changeWidthInitial, 100);
				this.SetBounds(bounds);
				this.editor.UpdateConnections();
				return true;
			}
			else if (this.isMoveColumnsSeparator)
			{
				Rectangle rect = this.Bounds;
				rect.Deflate(ObjectBox.textMargin, 0);
				this.columnsSeparatorRelative = (pos.X-rect.Left)/rect.Width;
				this.columnsSeparatorRelative = System.Math.Max(this.columnsSeparatorRelative, 0.2);
				this.columnsSeparatorRelative = System.Math.Min(this.columnsSeparatorRelative, 1.0);
				this.editor.Invalidate();
				return true;
			}
			else if (this.isParentsMenu)
			{
				Rectangle rect = this.RectangleParentsMenu;
				int sel = -1;
				if (rect.Contains(pos))
				{
					sel = (int) ((pos.Y-rect.Bottom)/ObjectBox.parentsMenuHeight);
					sel = this.parentsClosedCount-sel-1;
				}

				if (this.parentsMenuSelected != sel)
				{
					this.parentsMenuSelected = sel;
					this.editor.Invalidate();
				}

				return true;
			}
			else
			{
				return base.MouseMove(pos);
			}
		}

		public override void MouseDown(Point pos)
		{
			//	Le bouton de la souris est pressé.
			if (this.isParentsMenu)  // menu resté du clic précédent ?
			{
				if (this.parentsMenuSelected == -1)  // clic en dehors ?
				{
					this.isParentsMenu = false;  // ferme le menu
					this.editor.LockObject(null);
					this.editor.Invalidate();
				}
				return;
			}

			if (this.hilitedElement == ActiveElement.HeaderDragging && this.editor.BoxCount > 1)
			{
				this.isDragging = true;
				this.draggingPos = pos;
				this.editor.Invalidate();
				this.editor.LockObject(this);
			}

			if (this.hilitedElement == ActiveElement.FieldMovable)
			{
				this.isFieldMoving = true;
				this.fieldInitialRank = this.hilitedFieldRank;
				this.editor.LockObject(this);
			}

			if (this.hilitedElement == ActiveElement.ChangeWidth)
			{
				this.isChangeWidth = true;
				this.changeWidthPos = pos.X;
				this.changeWidthInitial = this.bounds.Width;
				this.editor.LockObject(this);
			}

			if (this.hilitedElement == ActiveElement.MoveColumnsSeparator)
			{
				this.isMoveColumnsSeparator = true;
				this.editor.LockObject(this);
			}

			if (this.hilitedElement == ActiveElement.ParentsButton)
			{
				this.isParentsMenu = true;
				this.parentsMenuSelected = -1;
				this.editor.Invalidate();
				this.editor.LockObject(this);
			}
		}

		public override void MouseUp(Point pos)
		{
			//	Le bouton de la souris est relâché.
			if (this.isDragging)
			{
				this.editor.UpdateAfterMoving(this);
				this.isDragging = false;
				this.editor.LockObject(null);
			}
			else if (this.isFieldMoving)
			{
				if (this.hilitedElement == ActiveElement.FieldMoving)
				{
					this.MoveField(this.fieldInitialRank, this.hilitedFieldRank);
				}
				this.isFieldMoving = false;
				this.editor.LockObject(null);
			}
			else if (this.isChangeWidth)
			{
				this.editor.UpdateAfterMoving(this);
				this.isChangeWidth = false;
				this.editor.LockObject(null);
			}
			else if (this.isMoveColumnsSeparator)
			{
				this.isMoveColumnsSeparator = false;
				this.editor.LockObject(null);
			}
			else if (this.isParentsMenu)
			{
				if (this.parentsMenuSelected != -1)
				{
					ParentInfo info = this.GetParentInfo(this.parentsMenuSelected);
					this.OpenParent(info.CultureMap, info.Rank);

					this.isParentsMenu = false;  // ferme le menu
					this.editor.LockObject(null);
					this.editor.Invalidate();
				}
				//	Si on est pas dans une case valide (par exemple si on a cliqué sans bouger
				//	dans l'en-tête), le menu reste.
			}
			else
			{
				if (this.hilitedElement == ActiveElement.ExtendButton)
				{
					this.IsExtended = !this.IsExtended;
					this.editor.UpdateAfterGeometryChanged(this);
				}

				if (this.hilitedElement == ActiveElement.CloseButton)
				{
					if (!this.isRoot)
					{
						this.editor.CloseBox(this);
						this.editor.CreateConnections();
						this.editor.UpdateAfterMoving(null);
					}
				}

				if (this.hilitedElement == ActiveElement.FieldRemove)
				{
					this.RemoveField(this.hilitedFieldRank);
				}

				if (this.hilitedElement == ActiveElement.FieldAdd)
				{
					this.AddField(this.hilitedFieldRank);
				}

				if (this.hilitedElement == ActiveElement.FieldNameSelect)
				{
					this.ChangeFieldName(this.hilitedFieldRank);
				}

				if (this.hilitedElement == ActiveElement.FieldTypeSelect)
				{
					this.ChangeFieldType(this.hilitedFieldRank);
				}

				if (this.hilitedElement == ActiveElement.CommentButton)
				{
					this.AddComment();
				}
			}
		}

		protected override bool MouseDetect(Point pos, out ActiveElement element, out int fieldRank)
		{
			//	Détecte l'élément actif visé par la souris.
			element = ActiveElement.None;
			fieldRank = -1;
			this.SetConnectionsHilited(false);

			if (pos.IsZero)
			{
				//	Si l'une des connection est dans l'état ConnectionOpen*, il faut afficher
				//	aussi les petits cercles de gauche.
				if (this.IsConnectionReadyForOpen())
				{
					this.SetConnectionsHilited(true);
				}
				return false;
			}

			Rectangle rect;

			if (this.isFieldMoving)
			{
				//	Souris entre deux champs ?
				for (int i=-1; i<this.fields.Count; i++)
				{
					rect = this.GetFieldMovingBounds(i);
					if (rect.Contains(pos))
					{
						element = ActiveElement.FieldMoving;
						fieldRank = i;
						return true;
					}
				}
			}
			else
			{
				//	Souris dans le bouton compact/étendu ?
				if (this.DetectRoundButton(this.PositionExtendButton, pos))
				{
					element = ActiveElement.ExtendButton;
					return true;
				}

				//	Souris dans le bouton de fermeture ?
				if (this.DetectRoundButton(this.PositionCloseButton, pos))
				{
					element = ActiveElement.CloseButton;
					return true;
				}

				//	Souris dans le bouton des parents ?
				if (this.parentsClosedCount > 0)
				{
					if (this.DetectRoundButton(this.PositionParentsButton, pos))
					{
						element = ActiveElement.ParentsButton;
						return true;
					}
				}

				//	Souris dans le bouton des commentaires ?
				if (this.DetectRoundButton(this.PositionCommentButton, pos))
				{
					element = ActiveElement.CommentButton;
					return true;
				}

				if (this.isExtended)
				{
					//	Souris dans le bouton pour changer la largeur ?
					//	Souris dans le bouton pour déplacer le séparateur des colonnes ?
					double d1 = Point.Distance(this.PositionChangeWidthButton, pos);
					double d2 = Point.Distance(this.PositionMoveColumnsButton, pos);

					if (d1 < d2)
					{
						if (d1 <= AbstractObject.buttonRadius+1)
						{
							element = ActiveElement.ChangeWidth;
							return true;
						}
					}
					else
					{
						if (d2 <= AbstractObject.buttonRadius+1)
						{
							element = ActiveElement.MoveColumnsSeparator;
							return true;
						}
					}

					//	Souris dans l'en-tête ?
					if (this.bounds.Contains(pos) && 
						(pos.Y >= this.bounds.Top-AbstractObject.headerHeight ||
						 pos.Y <= this.bounds.Bottom+AbstractObject.footerHeight))
					{
						element = ActiveElement.HeaderDragging;
						this.SetConnectionsHilited(true);
						return true;
					}

					//	Souris entre deux champs ?
					for (int i=-1; i<this.fields.Count; i++)
					{
						rect = this.GetFieldAddBounds(i);
						if (rect.Contains(pos))
						{
							element = ActiveElement.FieldAdd;
							fieldRank = i;
							this.SetConnectionsHilited(true);
							return true;
						}
					}

					//	Souris sur le séparateur des colonnes ?
					double sep = this.ColumnsSeparatorAbsolute;
					if (this.columnsSeparatorRelative < 1.0 && pos.X >= sep-4 && pos.X <= sep+4 &&
						pos.Y >= this.bounds.Bottom+AbstractObject.footerHeight &&
						pos.Y <= this.bounds.Top-AbstractObject.headerHeight)
					{
						element = ActiveElement.MoveColumnsSeparator;
						this.SetConnectionsHilited(true);
						return true;
					}

					//	Souris dans un champ ?
					for (int i=0; i<this.fields.Count; i++)
					{
						rect = this.GetFieldRemoveBounds(i);
						if (rect.Contains(pos))
						{
							element = ActiveElement.FieldRemove;
							fieldRank = i;
							this.SetConnectionsHilited(true);
							return true;
						}

						rect = this.GetFieldMovableBounds(i);
						if (rect.Contains(pos))
						{
							element = ActiveElement.FieldMovable;
							fieldRank = i;
							this.SetConnectionsHilited(true);
							return true;
						}

						rect = this.GetFieldNameBounds(i);
						if (rect.Contains(pos))
						{
							element = ActiveElement.FieldNameSelect;
							fieldRank = i;
							this.SetConnectionsHilited(true);
							return true;
						}

						rect = this.GetFieldTypeBounds(i);
						if (rect.Contains(pos))
						{
							element = ActiveElement.FieldTypeSelect;
							fieldRank = i;
							this.SetConnectionsHilited(true);
							return true;
						}
					}
				}
				else  // boîte compactée ?
				{
					if (this.bounds.Contains(pos))
					{
						element = ActiveElement.HeaderDragging;
						return true;
					}
				}
			}

			if (!this.bounds.Contains(pos))
			{
				return false;
			}

			element = ActiveElement.Inside;
			this.SetConnectionsHilited(true);
			return true;
		}

		protected void SetConnectionsHilited(bool isHilited)
		{
			//	Modifie l'état 'hilited' de toutes les connections qui partent de l'objet.
			//	Avec false, les petits cercles des liaisons fermées ne sont affichés qu'à droite.
			foreach (Field field in this.fields)
			{
				if (field.Connection != null)
				{
					field.Connection.IsSrcHilited = isHilited;
				}
			}
		}

		protected bool IsConnectionReadyForOpen()
		{
			//	Indique si l'une des connections qui partent de l'objet est en mode ConnectionOpen*.
			foreach (Field field in this.fields)
			{
				if (field.Connection != null)
				{
					ActiveElement ae = field.Connection.HilitedElement;
					if (ae == ActiveElement.ConnectionOpenLeft ||
						ae == ActiveElement.ConnectionOpenRight)
					{
						return true;
					}
				}
			}

			return false;
		}

		protected Rectangle GetFieldRemoveBounds(int rank)
		{
			//	Retourne le rectangle occupé par le bouton (-) d'un champ.
			Rectangle rect = this.GetFieldBounds(rank);

			rect.Width = rect.Height;
			
			return rect;
		}

		protected Rectangle GetFieldAddBounds(int rank)
		{
			//	Retourne le rectangle occupé par le bouton (+) d'un champ.
			Rectangle rect = this.GetFieldBounds(rank);
			
			rect.Width = rect.Height;
			rect.Bottom -= 6;
			rect.Height = 6*2;

			return rect;
		}

		protected Rectangle GetFieldMovableBounds(int rank)
		{
			//	Retourne le rectangle occupé par le bouton (|) d'un champ.
			Rectangle rect = this.GetFieldBounds(rank);

			rect.Left = rect.Right-rect.Height;
			
			return rect;
		}

		protected Rectangle GetFieldMovingBounds(int rank)
		{
			//	Retourne le rectangle occupé par la destination d'un déplacement de champ.
			Rectangle rect = this.GetFieldBounds(rank);
			
			rect.Bottom -= ObjectBox.fieldHeight/2;
			rect.Height = ObjectBox.fieldHeight;

			return rect;
		}

		protected Rectangle GetFieldNameBounds(int rank)
		{
			//	Retourne le rectangle occupé par le nom d'un champ.
			Rectangle rect = this.GetFieldBounds(rank);

			rect.Deflate(ObjectBox.textMargin, 0);
			rect.Right = this.ColumnsSeparatorAbsolute;

			return rect;
		}

		protected Rectangle GetFieldTypeBounds(int rank)
		{
			//	Retourne le rectangle occupé par le type d'un champ.
			Rectangle rect = this.GetFieldBounds(rank);
			
			rect.Deflate(ObjectBox.textMargin, 0);
			rect.Left = this.ColumnsSeparatorAbsolute+1;

			return rect;
		}

		protected Rectangle GetFieldBounds(int rank)
		{
			//	Retourne le rectangle occupé par un champ.
			Rectangle rect = this.bounds;

			rect.Deflate(2, 0);
			rect.Bottom = rect.Top - AbstractObject.headerHeight - ObjectBox.fieldHeight*(rank+1) - 12;
			rect.Height = ObjectBox.fieldHeight;

			return rect;
		}


		protected void MoveField(int srcRank, int dstRank)
		{
			//	Déplace un champ.
			if (dstRank != srcRank && dstRank != srcRank-1)
			{
				StructuredData data = this.cultureMap.GetCultureData(Resources.DefaultTwoLetterISOLanguageName);
				IList<StructuredData> dataFields = data.GetValue(Support.Res.Fields.ResourceStructuredType.Fields) as IList<StructuredData>;

				if (dstRank < srcRank)
				{
					dstRank++;
				}

				StructuredData movingfield = dataFields[srcRank];
				dataFields.RemoveAt(srcRank);
				dataFields.Insert(dstRank, movingfield);

				Field movingFld = this.fields[srcRank];
				this.fields.RemoveAt(srcRank);
				this.fields.Insert(dstRank, movingFld);

				this.UpdateFields();
				this.editor.UpdateAfterAddOrRemoveConnection(this);
				this.SetDirty();
			}

			this.hilitedElement = ActiveElement.None;
		}

		protected void RemoveField(int rank)
		{
			//	Supprime un champ.
			string question = string.Format("Voulez-vous supprimer le champ <b>{0}</b> ?", this.fields[rank].FieldName);
			if (this.editor.Module.MainWindow.DialogQuestion(question) == Epsitec.Common.Dialogs.DialogResult.Yes)
			{
				this.fields[rank].IsExplored = false;
				this.fields[rank].DstBox = null;
				this.editor.CloseBox(null);

				StructuredData data = this.cultureMap.GetCultureData(Resources.DefaultTwoLetterISOLanguageName);
				IList<StructuredData> dataFields = data.GetValue(Support.Res.Fields.ResourceStructuredType.Fields) as IList<StructuredData>;
				dataFields.RemoveAt(rank);

				this.fields.RemoveAt(rank);

				this.UpdateFields();
				this.editor.UpdateAfterAddOrRemoveConnection(this);
				this.SetDirty();
			}

			this.hilitedElement = ActiveElement.None;
		}

		protected void AddField(int rank)
		{
			//	Ajoute un nouveau champ.
			Module module = this.editor.Module;
			string name = this.GetNewName();
			name = module.MainWindow.DlgFieldName(name);
			if (string.IsNullOrEmpty(name))
			{
				this.hilitedElement = ActiveElement.None;
				return;
			}
			
			StructuredData data = this.cultureMap.GetCultureData(Resources.DefaultTwoLetterISOLanguageName);
			IList<StructuredData> dataFields = data.GetValue(Support.Res.Fields.ResourceStructuredType.Fields) as IList<StructuredData>;

			IResourceAccessor accessor = this.editor.Module.AccessEntities.Accessor;
			IDataBroker broker = accessor.GetDataBroker(data, Support.Res.Fields.ResourceStructuredType.Fields.ToString());
			StructuredData newField = broker.CreateData(this.cultureMap);

			Druid druid = this.CreateFieldCaption(name);
			newField.SetValue(Support.Res.Fields.Field.CaptionId, druid);

			dataFields.Insert(rank+1, newField);

			Field field = new Field(this.editor);
			this.UpdateField(newField, field);
			this.fields.Insert(rank+1, field);

			this.UpdateFields();
			this.editor.UpdateAfterAddOrRemoveConnection(this);
			this.SetDirty();
			this.hilitedElement = ActiveElement.None;
		}

		protected void ChangeFieldName(int rank)
		{
			//	Choix du nom pour un champ.
#if true
			StructuredData data = this.cultureMap.GetCultureData(Resources.DefaultTwoLetterISOLanguageName);
			IList<StructuredData> dataFields = data.GetValue(Support.Res.Fields.ResourceStructuredType.Fields) as IList<StructuredData>;

			StructuredData dataField = dataFields[rank];
			Druid druid = (Druid) dataField.GetValue(Support.Res.Fields.Field.CaptionId);

			Module module = this.editor.Module;
			druid = module.MainWindow.DlgResourceSelector(module, ResourceAccess.Type.Fields, ResourceAccess.TypeType.None, druid, null, null);
			if (druid.IsEmpty)
			{
				return;
			}

			dataField.SetValue(Support.Res.Fields.Field.CaptionId, druid);
			this.UpdateField(dataField, this.fields[rank]);
			this.SetDirty();
			this.editor.Invalidate();
#else
			Module module = this.editor.Module;
			string name = this.fields[rank].FieldName;
			name = module.MainWindow.DlgFieldName(name);
			if (string.IsNullOrEmpty(name))
			{
				this.hilitedElement = ActiveElement.None;
				return;
			}
			
			// TODO: modifier le nom du type...
#endif
		}

		protected void ChangeFieldType(int rank)
		{
			//	Choix du type pour un champ.
			StructuredData data = this.cultureMap.GetCultureData(Resources.DefaultTwoLetterISOLanguageName);
			IList<StructuredData> dataFields = data.GetValue(Support.Res.Fields.ResourceStructuredType.Fields) as IList<StructuredData>;

			StructuredData dataField = dataFields[rank];
			Druid druid = (Druid) dataField.GetValue(Support.Res.Fields.Field.TypeId);

			Module module = this.editor.Module;
			druid = module.MainWindow.DlgResourceSelector(module, ResourceAccess.Type.Types, ResourceAccess.TypeType.None, druid, null, null);
			if (druid.IsEmpty)
			{
				return;
			}

			if (this.fields[rank].Relation != FieldRelation.None)
			{
				ObjectBox dst = this.fields[rank].DstBox;
				this.fields[rank].IsExplored = false;
				this.fields[rank].DstBox = null;
				this.editor.CloseBox(dst);
			}

			dataField.SetValue(Support.Res.Fields.Field.TypeId, druid);

			AbstractType type = TypeRosetta.GetTypeObject(module.ResourceManager.GetCaption(druid));
			if (type is StructuredType)
			{
				dataField.SetValue(Support.Res.Fields.Field.Relation, FieldRelation.Reference);
			}
			else
			{
				dataField.SetValue(Support.Res.Fields.Field.Relation, FieldRelation.None);
			}
			
			this.UpdateField(dataField, this.fields[rank]);
			this.SetDirty();
			this.editor.UpdateAfterAddOrRemoveConnection(this);
		}

		protected void UpdateField(StructuredData dataField, Field field)
		{
			//	Met à jour une instance de la classe Field, selon le StructuredData d'un champ.
			System.Diagnostics.Debug.Assert(!dataField.IsEmpty);

			Druid fieldCaptionId = (Druid) dataField.GetValue(Support.Res.Fields.Field.CaptionId);
			FieldMembership membership = (FieldMembership) dataField.GetValue(Support.Res.Fields.Field.Membership);
			FieldRelation rel = (FieldRelation) dataField.GetValue(Support.Res.Fields.Field.Relation);
			Druid sourceId = (Druid) dataField.GetValue(Support.Res.Fields.Field.SourceFieldId);
			Druid typeId = (Druid) dataField.GetValue(Support.Res.Fields.Field.TypeId);
			
			Module dstModule = this.editor.Module.MainWindow.SearchModule(typeId);
			CultureMap dstItem = (dstModule == null) ? null : dstModule.AccessEntities.Accessor.Collection[typeId];
			if (dstItem == null)
			{
				rel = FieldRelation.None;  // ce n'est pas une vraie relation !
			}

			Caption fieldCaption = this.editor.Module.AccessEntities.DirectGetCaption(fieldCaptionId);
			Caption typeCaption = typeId.IsEmpty ? null : this.editor.Module.AccessEntities.DirectGetCaption(typeId);

			field.FieldName = (fieldCaption == null) ? "" : fieldCaption.Name;
			field.TypeName = (typeCaption == null) ? "" : typeCaption.Name;
			field.Relation = rel;
			field.Destination = typeId;
			field.SrcBox = this;
		}

		protected void UpdateFields()
		{
			//	Met à jour toutes les liaisons des champs.
			for (int i=0; i<this.fields.Count; i++)
			{
				this.fields[i].IsSourceExpanded = this.isExtended;
				this.fields[i].Rank = i;
			}
		}

		protected Druid CreateFieldCaption(string text)
		{
			//	Crée un nouveau Caption de type Field (dont le nom commence par "Fld.").
			//	TODO: remplacer cette cuisine par les nouveaux mécanismes...
			string name = string.Concat(this.Title, ".", text);

			//	Crée un Caption de type Field (dont le nom commence par "Fld.").
			ResourceAccess access = this.editor.Module.AccessCaptions;

			access.BypassFilterOpenAccess(ResourceAccess.Type.Fields, ResourceAccess.TypeType.None, null, null);
			Druid druid = access.BypassFilterCreate(access.GetCultureBundle(null), name, text);
			access.BypassFilterCloseAccess();

			return druid;
		}

		protected string GetNewName()
		{
			//	Cherche un nouveau nom jamais utilisé.
			for (int i=1; i<10000; i++)
			{
				string name = string.Format(Res.Strings.Viewers.Types.Structured.NewName, i.ToString(System.Globalization.CultureInfo.InvariantCulture));
				if (!this.IsExistingName(name))
				{
					return name;
				}
			}
			return null;
		}

		protected bool IsExistingName(string name)
		{
			//	Indique si un nom existe.
			for (int i=0; i<this.fields.Count; i++)
			{
				if (name == this.fields[i].FieldName)
				{
					return true;
				}
			}

			return false;
		}


		protected void AddComment()
		{
			//	Ajoute un commentaire à la boîte.
			if (this.comment == null)
			{
				this.comment = new ObjectComment(this.editor);
				this.comment.Box = this;

				Rectangle rect = this.bounds;
				rect.Width = System.Math.Max(rect.Width, AbstractObject.commentMinWidth);
				rect.Bottom = rect.Top+20;
				rect.Height = 50;
				this.comment.SetBounds(rect);

				this.editor.AddComment(comment);
				this.editor.UpdateAfterCommentChanged();
			}
			else
			{
				this.comment.IsVisible = !this.comment.IsVisible;
			}
		}


		/// <summary>
		/// Informations sur une entité parente, ouverte ou fermée.
		/// </summary>
		protected class ParentInfo
		{
			public string ModuleName;
			public string FieldName;
			public CultureMap CultureMap;
			public int Rank;
			public bool Opened;
		}

		public void UpdateAfterOpenOrCloseBox()
		{
			//	Appelé après avoir ajouté ou supprimé une boîte.
			this.parentsClosedCount = 0;
			for (int i=0; i<this.parentsList.Count; i++)
			{
				ParentInfo info = this.parentsList[i];

				info.Opened = false;
				foreach (ObjectBox box in this.editor.Boxes)
				{
					if (box.cultureMap == info.CultureMap)
					{
						info.Opened = true;
						break;
					}
				}

				if (!info.Opened)
				{
					this.parentsClosedCount++;  // màj le nombre de parents fermés
				}
			}
		}

		protected void UpdateParents()
		{
			//	Met à jour la liste de tous les parents potentiels de l'entité courante.
			this.parentsList = new List<ParentInfo>();

			List<Module> modules = this.editor.Module.MainWindow.Modules;
			foreach (Module module in modules)
			{
				foreach (CultureMap cultureMap in module.AccessEntities.Accessor.Collection)
				{
					StructuredData data = cultureMap.GetCultureData(Resources.DefaultTwoLetterISOLanguageName);
					IList<StructuredData> dataFields = data.GetValue(Support.Res.Fields.ResourceStructuredType.Fields) as IList<StructuredData>;

					if (dataFields != null)
					{
						int i = 0;
						foreach (StructuredData dataField in dataFields)
						{
							Druid typeId = (Druid) dataField.GetValue(Support.Res.Fields.Field.TypeId);
							Module dstModule = this.editor.Module.MainWindow.SearchModule(typeId);
							CultureMap fieldCultureMap = (dstModule == null) ? null : dstModule.AccessEntities.Accessor.Collection[typeId];
							
							if (fieldCultureMap == this.cultureMap && !this.IsExistingParentInfo(cultureMap))
							{
								ParentInfo info = new ParentInfo();
								
								info.CultureMap = cultureMap;
								info.ModuleName = module.ModuleInfo.Name;
								info.FieldName = cultureMap.Name;
								info.Rank = i;
								info.Opened = false;

								this.parentsList.Add(info);
							}

							i++;
						}
					}
				}
			}

			this.parentsClosedCount = this.parentsList.Count;
		}

		protected bool IsExistingParentInfo(CultureMap cultureMap)
		{
			foreach (ParentInfo info in this.parentsList)
			{
				if (info.CultureMap == cultureMap)
				{
					return true;
				}
			}
			return false;
		}

		protected ParentInfo GetParentInfo(int sel)
		{
			//	Retourne une information sur un parent fermé.
			foreach (ParentInfo info in this.parentsList)
			{
				if (!info.Opened && sel-- == 0)
				{
					return info;
				}
			}
			throw new System.Exception("Selection out of range.");
		}

		protected void OpenParent(CultureMap cultureMap, int rank)
		{
			//	Ouvre une entité parent.
			ObjectBox box = this.editor.SearchBox(cultureMap.Name);
			if (box == null)
			{
				//	Ouvre la connection sur une nouvelle boîte.
				box = new ObjectBox(this.editor);
				box.Title = cultureMap.Name;
				box.SetContent(cultureMap);

				Field field = box.Fields[rank];
				field.DstBox = this;
				field.IsAttachToRight = true;
				field.IsExplored = true;

				this.editor.AddBox(box);
				this.editor.UpdateGeometry();

				//	Essaie de trouver une place libre, pour déplacer le moins possible d'éléments.
				double oy = box.bounds.Top - box.GetConnectionSrcVerticalPosition(rank) - AbstractObject.headerHeight/2;

				Rectangle bounds = new Rectangle(this.bounds.Left-50-box.Bounds.Width, this.bounds.Top+oy-box.bounds.Height, box.Bounds.Width, box.Bounds.Height);
				bounds.Inflate(50, Editor.pushMargin);

				for (int i=0; i<1000; i++)
				{
					if (this.editor.IsEmptyArea(bounds))
					{
						break;
					}
					bounds.Offset(-1, 0);
				}

				bounds.Deflate(50, Editor.pushMargin);
				box.SetBounds(bounds);
			}

			this.editor.CreateConnections();
			this.editor.UpdateAfterMoving(box);
		}


		public override void DrawBackground(Graphics graphics)
		{
			//	Dessine le fond de l'objet.
			Rectangle rect;

			bool dragging = (this.hilitedElement == ActiveElement.HeaderDragging);

			//	Dessine l'ombre.
			rect = this.bounds;
			if (this.isRoot)
			{
				rect.Inflate(2);
			}
			rect.Offset(ObjectBox.shadowOffset, -(ObjectBox.shadowOffset));
			this.DrawShadow(graphics, rect, ObjectBox.roundFrameRadius+ObjectBox.shadowOffset, (int)ObjectBox.shadowOffset, 0.2);

			//	Construit le chemin du cadre arrondi.
			rect = this.bounds;
			rect.Deflate(1);
			Path path = this.PathRoundRectangle(rect, ObjectBox.roundFrameRadius);

			//	Dessine l'intérieur en blanc.
			graphics.Rasterizer.AddSurface(path);
			graphics.RenderSolid(Color.FromBrightness(1));

			//	Dessine l'intérieur en dégradé.
			graphics.Rasterizer.AddSurface(path);
			Color c1 = this.GetColorCaption(dragging ? 0.6 : 0.4);
			Color c2 = this.GetColorCaption(dragging ? 0.2 : 0.1);
			this.RenderHorizontalGradient(graphics, this.bounds, c1, c2);

			Color colorLine = Color.FromBrightness(0.9);
			if (dragging)
			{
				colorLine = this.GetColorCaption(0.3);
			}

			Color colorFrame = dragging ? this.GetColorCaption() : Color.FromBrightness(0);

			//	Dessine en blanc la zone pour les champs.
			if (this.isExtended)
			{
				Rectangle inside = new Rectangle(this.bounds.Left+1, this.bounds.Bottom+AbstractObject.footerHeight, this.bounds.Width-2, this.bounds.Height-AbstractObject.footerHeight-AbstractObject.headerHeight);
				graphics.AddFilledRectangle(inside);
				graphics.RenderSolid(Color.FromBrightness(1));
				graphics.AddFilledRectangle(inside);
				Color ci1 = this.GetColorCaption(dragging ? 0.2 : 0.1);
				Color ci2 = this.GetColorCaption(0.0);
				this.RenderHorizontalGradient(graphics, inside, ci1, ci2);

				//	Trait vertical de séparation.
				if (this.columnsSeparatorRelative < 1.0)
				{
					double posx = System.Math.Floor(this.ColumnsSeparatorAbsolute)+0.5;
					graphics.AddLine(posx, this.bounds.Bottom+AbstractObject.footerHeight+0.5, posx, this.bounds.Top-AbstractObject.headerHeight-0.5);
					graphics.RenderSolid(colorLine);
				}

				//	Ombre supérieure.
				Rectangle shadow = new Rectangle(this.bounds.Left+1, this.bounds.Top-AbstractObject.headerHeight-8, this.bounds.Width-2, 8);
				graphics.AddFilledRectangle(shadow);
				this.RenderVerticalGradient(graphics, shadow, Color.FromAlphaRgb(0.0, 0, 0, 0), Color.FromAlphaRgb(0.3, 0, 0, 0));
			}

			//	Dessine le titre.
			rect = new Rectangle(this.bounds.Left, this.bounds.Top-AbstractObject.headerHeight, this.bounds.Width, AbstractObject.headerHeight);
			rect.Deflate(4, 2);
			this.title.LayoutSize = rect.Size;
			this.title.Paint(rect.BottomLeft, graphics, Rectangle.MaxValue, Color.FromBrightness(0), GlyphPaintStyle.Normal);

			//	Dessine le bouton compact/étendu.
			GlyphShape shape = this.isExtended ? GlyphShape.ArrowUp : GlyphShape.ArrowDown;
			if (this.hilitedElement == ActiveElement.ExtendButton)
			{
				this.DrawRoundButton(graphics, this.PositionExtendButton, AbstractObject.buttonRadius, shape, true, false);
			}
			else if (this.IsHeaderHilite && !this.isDragging)
			{
				this.DrawRoundButton(graphics, this.PositionExtendButton, AbstractObject.buttonRadius, shape, false, false);
			}

			//	Dessine le bouton de fermeture.
			if (this.hilitedElement == ActiveElement.CloseButton)
			{
				this.DrawRoundButton(graphics, this.PositionCloseButton, AbstractObject.buttonRadius, GlyphShape.Close, true, false, !this.isRoot);
			}
			else if (this.IsHeaderHilite && !this.isDragging)
			{
				this.DrawRoundButton(graphics, this.PositionCloseButton, AbstractObject.buttonRadius, GlyphShape.Close, false, false, !this.isRoot);
			}

			//	Dessine le moignon pour les parents à gauche.
			if (this.parentsClosedCount > 0)
			{
				Point p1 = this.PositionParentsButton;
				Point p2 = p1;
				p1.X = this.bounds.Left-1-AbstractObject.lengthClose;
				p2.X = this.bounds.Left-1;
				graphics.LineWidth = 2;
				graphics.AddLine(p1, p2);
				this.DrawEndingArrow(graphics, p1, p2, FieldRelation.Reference);
				graphics.LineWidth = 1;
				graphics.RenderSolid(colorFrame);
			}

			//	Dessine le bouton des parents.
			if (this.hilitedElement == ActiveElement.ParentsButton)
			{
				this.DrawRoundButton(graphics, this.PositionParentsButton, AbstractObject.buttonRadius, GlyphShape.TriangleDown, true, false, this.parentsClosedCount > 0);
			}
			else if (this.IsHeaderHilite && !this.isDragging)
			{
				this.DrawRoundButton(graphics, this.PositionParentsButton, AbstractObject.buttonRadius, GlyphShape.TriangleDown, false, false, this.parentsClosedCount > 0);
			}

			//	Dessine le bouton des commentaires.
			if (this.hilitedElement == ActiveElement.CommentButton)
			{
				this.DrawRoundButton(graphics, this.PositionCommentButton, AbstractObject.buttonRadius, "C", true, false);
			}
			else if (this.IsHeaderHilite && !this.isDragging)
			{
				this.DrawRoundButton(graphics, this.PositionCommentButton, AbstractObject.buttonRadius, "C", false, false);
			}

			//	Dessine les noms des champs.
			if (this.isExtended)
			{
				graphics.AddLine(this.bounds.Left+2, this.bounds.Top-AbstractObject.headerHeight-0.5, this.bounds.Right-2, this.bounds.Top-AbstractObject.headerHeight-0.5);
				graphics.AddLine(this.bounds.Left+2, this.bounds.Bottom+AbstractObject.footerHeight+0.5, this.bounds.Right-2, this.bounds.Bottom+AbstractObject.footerHeight+0.5);
				graphics.RenderSolid(colorFrame);

				for (int i=0; i<this.fields.Count; i++)
				{
					Color colorName = Color.FromBrightness(0);
					Color colorType = Color.FromBrightness(0);

					if (this.hilitedElement == ActiveElement.FieldNameSelect && this.hilitedFieldRank == i)
					{
						rect = this.GetFieldNameBounds(i);

						graphics.AddFilledRectangle(rect);
						graphics.RenderSolid(this.GetColorCaption());

						colorName = Color.FromBrightness(1);
					}

					if (this.hilitedElement == ActiveElement.FieldTypeSelect && this.hilitedFieldRank == i)
					{
						rect = this.GetFieldTypeBounds(i);

						graphics.AddFilledRectangle(rect);
						graphics.RenderSolid(this.GetColorCaption());

						colorType = Color.FromBrightness(1);
					}

					if ((this.hilitedElement == ActiveElement.FieldRemove || this.hilitedElement == ActiveElement.FieldMovable) && this.hilitedFieldRank == i)
					{
						rect = this.GetFieldBounds(i);

						graphics.AddFilledRectangle(rect);
						graphics.RenderSolid(this.GetColorCaption(0.3));
					}

					if (this.isFieldMoving && this.fieldInitialRank == i)
					{
						rect = this.GetFieldBounds(i);

						graphics.AddFilledRectangle(rect);
						graphics.RenderSolid(this.GetColorCaption(0.3));
					}

					//	Affiche le nom du champ.
					rect = this.GetFieldNameBounds(i);
					rect.Right -= 2;
					this.fields[i].TextLayoutField.LayoutSize = rect.Size;
					this.fields[i].TextLayoutField.Paint(rect.BottomLeft, graphics, Rectangle.MaxValue, colorName, GlyphPaintStyle.Normal);

					//	Affiche le type du champ.
					rect = this.GetFieldTypeBounds(i);
					rect.Left += 1;
					if (rect.Width > 10)
					{
						this.fields[i].TextLayoutType.LayoutSize = rect.Size;
						this.fields[i].TextLayoutType.Paint(rect.BottomLeft, graphics, Rectangle.MaxValue, colorType, GlyphPaintStyle.Normal);
					}

					rect = this.GetFieldBounds(i);
					graphics.AddLine(rect.Left, rect.Bottom+0.5, rect.Right, rect.Bottom+0.5);
					graphics.RenderSolid(colorLine);
				}

				if (this.hilitedElement == ActiveElement.FieldMoving)
				{
					Point p1 = this.GetFieldBounds(this.fieldInitialRank).Center;
					Point p2 = this.GetFieldMovingBounds(this.hilitedFieldRank).Center;
					p1.X = p2.X = this.GetFieldMovableBounds(0).Center.X;
					this.DrawMovingArrow(graphics, p1, p2);
				}

				if (this.hilitedElement != ActiveElement.None &&
					this.hilitedElement != ActiveElement.HeaderDragging &&
					this.hilitedElement != ActiveElement.ParentsButton &&
					this.hilitedElement != ActiveElement.CommentButton &&
					this.hilitedElement != ActiveElement.ExtendButton &&
					this.hilitedElement != ActiveElement.CloseButton &&
					this.hilitedElement != ActiveElement.ChangeWidth &&
					this.hilitedElement != ActiveElement.MoveColumnsSeparator &&
					!this.isFieldMoving && !this.isChangeWidth && !this.isMoveColumnsSeparator)
				{
					//	Dessine la glissière à gauche pour suggérer les boutons Add/Remove des champs.
					Point p1 = this.GetFieldAddBounds(-1).Center;
					Point p2 = this.GetFieldAddBounds(this.fields.Count-1).Center;
					bool hilited = this.hilitedElement == ActiveElement.FieldAdd || this.hilitedElement == ActiveElement.FieldRemove;
					this.DrawEmptySlider(graphics, p1, p2, hilited);

					//	Dessine la glissière à droite pour suggérer les boutons Movable des champs.
					if (this.fields.Count != 0)
					{
						p1.X = p2.X = this.GetFieldMovableBounds(0).Center.X;
						hilited = this.hilitedElement == ActiveElement.FieldMovable;
						this.DrawEmptySlider(graphics, p1, p2, hilited);
					}
				}
			}

			//	Dessine le cadre en noir.
			graphics.Rasterizer.AddOutline(path, this.isRoot ? 6 : 2);
			graphics.RenderSolid(colorFrame);

			//	Dessine les boutons sur les glissières.
			if (this.isExtended)
			{
				if (this.hilitedElement == ActiveElement.FieldRemove)
				{
					rect = this.GetFieldRemoveBounds(this.hilitedFieldRank);
					this.DrawRoundButton(graphics, rect.Center, AbstractObject.buttonRadius, GlyphShape.Minus, true, true);
				}

				if (this.hilitedElement == ActiveElement.FieldAdd)
				{
					rect = this.GetFieldBounds(this.hilitedFieldRank);
					rect.Deflate(this.isRoot ? 3.5 : 1.5, 0.5);
					this.DrawDashLine(graphics, rect.BottomRight, rect.BottomLeft, this.GetColorCaption());

					rect = this.GetFieldAddBounds(this.hilitedFieldRank);
					this.DrawRoundButton(graphics, rect.Center, AbstractObject.buttonRadius, GlyphShape.Plus, true, true);
				}

				if (this.hilitedElement == ActiveElement.FieldMovable)
				{
					rect = this.GetFieldMovableBounds(this.hilitedFieldRank);
					this.DrawRoundButton(graphics, rect.Center, AbstractObject.buttonRadius, GlyphShape.VerticalMove, true, true);
				}

				if (this.hilitedElement == ActiveElement.FieldMoving)
				{
					rect = this.GetFieldBounds(this.hilitedFieldRank);
					rect.Deflate(this.isRoot ? 3.5 : 1.5, 0.5);
					this.DrawDashLine(graphics, rect.BottomRight, rect.BottomLeft, this.GetColorCaption());
				}
			}

			if (this.isExtended)
			{
				//	Dessine le bouton pour déplacer le séparateur des colonnes.
				if (this.hilitedElement == ActiveElement.MoveColumnsSeparator)
				{
					double sep = this.ColumnsSeparatorAbsolute;
					graphics.LineWidth = 4;
					graphics.AddLine(sep, this.bounds.Bottom+AbstractObject.footerHeight+3, sep, this.bounds.Top-AbstractObject.headerHeight-3);
					graphics.LineWidth = 1;
					graphics.RenderSolid(this.GetColorCaption());

					this.DrawRoundButton(graphics, this.PositionMoveColumnsButton, AbstractObject.buttonRadius, GlyphShape.HorizontalMove, true, false);
				}
				if (this.hilitedElement == ActiveElement.HeaderDragging && !this.isDragging)
				{
					this.DrawRoundButton(graphics, this.PositionMoveColumnsButton, AbstractObject.buttonRadius, GlyphShape.HorizontalMove, false, false);
				}

				//	Dessine le bouton pour changer la largeur.
				if (this.hilitedElement == ActiveElement.ChangeWidth)
				{
					this.DrawRoundButton(graphics, this.PositionChangeWidthButton, AbstractObject.buttonRadius, GlyphShape.HorizontalMove, true, false);
				}
				if (this.hilitedElement == ActiveElement.HeaderDragging && !this.isDragging)
				{
					this.DrawRoundButton(graphics, this.PositionChangeWidthButton, AbstractObject.buttonRadius, GlyphShape.HorizontalMove, false, false);
				}
			}
		}

		protected bool IsHeaderHilite
		{
			//	Indique si la souris est dans l'en-tête.
			get
			{
				return (this.hilitedElement == ActiveElement.HeaderDragging ||
						this.hilitedElement == ActiveElement.ParentsButton ||
						this.hilitedElement == ActiveElement.CommentButton ||
						this.hilitedElement == ActiveElement.ExtendButton ||
						this.hilitedElement == ActiveElement.CloseButton);
			}
		}

		public override void DrawForeground(Graphics graphics)
		{
			//	Dessine le dessus de l'objet.
			if (this.isParentsMenu)
			{
				this.DrawParentsMenu(graphics, this.PositionParentsMenu);
			}
		}

		protected void DrawParentsMenu(Graphics graphics, Point pos)
		{
			//	Dessine le menu pour choisir un parent.
			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;
			double h = ObjectBox.parentsMenuHeight;
			Rectangle box = this.RectangleParentsMenu;

			//	Dessine la boîte vide ombrée.
			Rectangle big = box;
			big.Inflate(7);

			Rectangle rs = big;
			rs.Inflate(2);
			rs.Offset(8, -8);
			this.DrawShadow(graphics, rs, 18, 10, 0.6);

			Path path = this.PathRoundRectangle(big, 10);
			graphics.Rasterizer.AddSurface(path);
			graphics.RenderSolid(Color.FromBrightness(1));
			graphics.Rasterizer.AddSurface(path);
			this.RenderHorizontalGradient(graphics, big, this.GetColorCaption(0.6), this.GetColorCaption(0.2));

			graphics.Rasterizer.AddOutline(path);
			graphics.RenderSolid(Color.FromBrightness(0));

			graphics.AddFilledRectangle(box);
			graphics.RenderSolid(Color.FromBrightness(1));

			//	Dessine l'en-tête.
			Rectangle rect = box;
			rect.Bottom = rect.Top-h-1;

			graphics.AddFilledRectangle(rect);
			graphics.RenderSolid(this.GetColorCaption());

			Rectangle gr = new Rectangle(this.PositionParentsButton.X-AbstractObject.buttonRadius, this.PositionParentsButton.Y-AbstractObject.buttonRadius, AbstractObject.buttonRadius*2, AbstractObject.buttonRadius*2);
			adorner.PaintGlyph(graphics, gr, WidgetPaintState.Enabled, Color.FromBrightness(1), GlyphShape.TriangleDown, PaintTextStyle.Button);
			
			graphics.AddText(rect.Left+AbstractObject.buttonRadius*2+5, rect.Bottom+1, rect.Width-(AbstractObject.buttonRadius*2+10), rect.Height, "Entités parentes", Font.GetFont(Font.DefaultFontFamily, "Bold"), 14, ContentAlignment.MiddleLeft);
			graphics.RenderSolid(Color.FromBrightness(1));
			
			rect = box;
			rect.Top = rect.Bottom+h;
			rect.Offset(0, h*(this.parentsClosedCount-1));

			//	Dessine les lignes du menu.
			for (int i=0; i<this.parentsClosedCount; i++)
			{
				ParentInfo info = this.GetParentInfo(i);

				if (i == this.parentsMenuSelected)
				{
					graphics.AddFilledRectangle(rect);
					graphics.RenderSolid(this.GetColorCaption(0.2));
				}

				string text = string.Concat(info.ModuleName, ": ", info.FieldName);
				graphics.AddText(rect.Left+5, rect.Bottom, rect.Width-10, rect.Height, text, Font.DefaultFont, 10, ContentAlignment.MiddleLeft);
				graphics.RenderSolid(Color.FromBrightness(0));

				graphics.AddLine(rect.TopLeft, rect.TopRight);
				graphics.RenderSolid(Color.FromBrightness(0));

				rect.Offset(0, -h);
			}

			//	Dessine le cadre du menu.
			graphics.AddRectangle(box);
			graphics.RenderSolid(Color.FromBrightness(0));
		}

		protected void DrawDashLine(Graphics graphics, Point p1, Point p2, Color color)
		{
			//	Dessine un large traitillé.
			Path path = new Path();
			path.MoveTo(p1);
			path.LineTo(p2);
			Misc.DrawPathDash(graphics, path, 3, 5, 5, color);
		}

		protected void DrawEmptySlider(Graphics graphics, Point p1, Point p2, bool hilited)
		{
			//	Dessine une glissère vide, pour suggérer les boutons qui peuvent y prendre place.
			Rectangle rect = new Rectangle(p1, p2);
			rect.Inflate(2.5+6);
			this.DrawShadow(graphics, rect, rect.Width/2, 6, 0.2);
			rect.Deflate(6);
			Path path = this.PathRoundRectangle(rect, rect.Width/2);

			Color hiliteColor = Color.FromBrightness(1);
			if (hilited)
			{
				hiliteColor = this.GetColorCaption();
				hiliteColor.R = 1-(1-hiliteColor.R)*0.2;
				hiliteColor.G = 1-(1-hiliteColor.G)*0.2;
				hiliteColor.B = 1-(1-hiliteColor.B)*0.2;
			}

			graphics.Rasterizer.AddSurface(path);
			graphics.RenderSolid(hiliteColor);

			graphics.Rasterizer.AddOutline(path);
			graphics.RenderSolid(Color.FromBrightness(0));
		}

		protected void DrawMovingArrow(Graphics graphics, Point p1, Point p2)
		{
			//	Dessine une flèche pendant le déplacement d'un champ.
			if (System.Math.Abs(p1.Y-p2.Y) < ObjectBox.fieldHeight)
			{
				return;
			}

			p2 = Point.Move(p2, p1, 1);
			double d = (p1.Y > p2.Y) ? -6 : 6;
			double sx = 3;

			Path path = new Path();
			path.MoveTo(p2);
			path.LineTo(p2.X-d*3/sx, p2.Y-d*2);
			path.LineTo(p2.X-d/sx, p2.Y-d*2);
			path.LineTo(p1.X-d/sx, p1.Y);
			path.LineTo(p1.X+d/sx, p1.Y);
			path.LineTo(p2.X+d/sx, p2.Y-d*2);
			path.LineTo(p2.X+d*3/sx, p2.Y-d*2);
			path.Close();

			graphics.Rasterizer.AddSurface(path);
			graphics.RenderSolid(this.GetColorCaption());
		}

		protected Point PositionCloseButton
		{
			//	Retourne la position du bouton pour fermer.
			get
			{
				return new Point(this.bounds.Right-AbstractObject.buttonRadius-6, this.bounds.Top-AbstractObject.headerHeight/2);
			}
		}

		protected Point PositionExtendButton
		{
			//	Retourne la position du bouton pour étendre.
			get
			{
				return new Point(this.bounds.Right-AbstractObject.buttonRadius*3-8, this.bounds.Top-AbstractObject.headerHeight/2);
			}
		}

		protected Point PositionChangeWidthButton
		{
			//	Retourne la position du bouton pour changer la largeur.
			get
			{
				return new Point(this.bounds.Right-1, this.bounds.Bottom+AbstractObject.footerHeight/2+1);
			}
		}

		protected Point PositionMoveColumnsButton
		{
			//	Retourne la position du bouton pour déplacer le séparateur des colonnes.
			get
			{
				return new Point(this.ColumnsSeparatorAbsolute, this.bounds.Bottom+AbstractObject.footerHeight/2+1);
			}
		}

		protected Point PositionParentsButton
		{
			//	Retourne la position du bouton pour montrer les parents.
			get
			{
				return new Point(this.bounds.Left+AbstractObject.buttonRadius+6, this.bounds.Top-AbstractObject.headerHeight/2);
			}
		}

		protected Point PositionCommentButton
		{
			//	Retourne la position du bouton pour montrer les parents.
			get
			{
				return new Point(this.bounds.Left+AbstractObject.buttonRadius*3+8, this.bounds.Top-AbstractObject.headerHeight/2);
			}
		}

		protected Point PositionParentsMenu
		{
			//	Retourne la position du menu pour montrer les parents.
			get
			{
				Point pos = this.PositionParentsButton;
				pos.X -= AbstractObject.buttonRadius;
				pos.Y += AbstractObject.buttonRadius;
				return pos;
			}
		}

		protected Rectangle RectangleParentsMenu
		{
			//	Retourne le rectangle du menu pour montrer les parents.
			get
			{
				Point pos = this.PositionParentsMenu;
				double h = ObjectBox.parentsMenuHeight*(this.parentsClosedCount+1);
				Rectangle rect = new Rectangle(pos.X, pos.Y-h, 200, h);
				rect.Inflate(0.5);
				return rect;
			}
		}

		protected double ColumnsSeparatorAbsolute
		{
			//	Retourne la position absolue du séparateur des colonnes.
			get
			{
				Rectangle rect = this.bounds;
				rect.Deflate(ObjectBox.textMargin, 0);
				return rect.Left + rect.Width*this.columnsSeparatorRelative;
			}
		}



		protected static readonly double roundFrameRadius = 12;
		protected static readonly double shadowOffset = 6;
		protected static readonly double textMargin = 13;
		protected static readonly double fieldHeight = 20;
		protected static readonly double parentsMenuHeight = 20;

		protected CultureMap cultureMap;
		protected ObjectComment comment;
		protected Rectangle bounds;
		protected double columnsSeparatorRelative;
		protected bool isRoot;
		protected bool isExtended;
		protected string titleString;
		protected TextLayout title;
		protected List<Field> fields;
		protected Field parentField;
		protected List<ParentInfo> parentsList;
		protected int parentsClosedCount;

		protected bool isDragging;
		protected Point draggingPos;

		protected bool isFieldMoving;
		protected int fieldInitialRank;

		protected bool isChangeWidth;
		protected double changeWidthPos;
		protected double changeWidthInitial;

		protected bool isMoveColumnsSeparator;

		protected bool isParentsMenu;
		protected int parentsMenuSelected;
	}
}
