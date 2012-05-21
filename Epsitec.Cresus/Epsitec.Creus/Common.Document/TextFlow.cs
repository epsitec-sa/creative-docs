using System.Collections.Generic;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Common.Text;
using Epsitec.Common.IO;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document
{
	/// <summary>
	/// Flux de texte.
	/// </summary>
	[System.Serializable()]
	public class TextFlow : ISerializable
	{
		private TextFlow()
		{
			//	Constructeur commun pour le constructeur standard et pour celui
			//	utilis� par la d�s�rialisation. Ce point d'entr�e commun est
			//	uniquement utilis� pour le debug.
		}
		
		public TextFlow(Document document) : this()
		{
			//	Cr�e un nouveau flux pour un seul pav�.
			this.document = document;

			this.InitializeNavigator ();
			this.InitializeEmptyTextStory ();
			
			this.objectsChain = new UndoableList(this.document, UndoableListType.ObjectsChain);
		}


		protected void InitializeNavigator()
		{
			Text.TextContext context = this.document.TextContext;
			
			if ( this.document.Modifier == null )
			{
				this.textStory = new Text.TextStory(context);
			}
			else
			{
				this.textStory = new Text.TextStory(this.document.Modifier.OpletQueue, context);
			}
			
			//	Il est important de ne pas g�n�rer d'oplets pour le undo/redo ici,
			//	car ils interf�reraient avec la gestion faite au plus haut niveau.
			this.textStory.DisableOpletQueue();
			
			this.textFitter    = new Text.TextFitter(this.textStory);
			this.textNavigator = new Text.TextNavigator(this.textFitter);
			this.metaNavigator = new TextNavigator2();
			
			this.metaNavigator.TextNavigator = this.textNavigator;
			
			this.textStory.OpletExecuted += this.HandleTextStoryOpletExecuted;
			this.textStory.RenamingOplet += new Text.TextStory.RenamingCallback(this.HandleTextStoryRenamingOplet);
			
			this.textNavigator.CursorMoved += this.HandleTextNavigatorCursorMoved;
			this.textNavigator.TextChanged += this.HandleTextNavigatorTextChanged;
			this.textNavigator.TabsChanged += this.HandleTextNavigatorTabsChanged;
			this.textNavigator.ActiveStyleChanged += this.HandleTextNavigatorActiveStyleChanged;
			
			this.textStory.EnableOpletQueue();
		}

		protected void InitializeEmptyTextStory()
		{
			System.Diagnostics.Debug.Assert(this.textStory.TextLength == 0);
			
			//	Il est important de ne pas g�n�rer d'oplets pour le undo/redo ici,
			//	car ils interf�reraient avec la gestion faite au plus haut niveau.
			this.textStory.DisableOpletQueue();
			
			this.textNavigator.Insert(Text.Unicode.Code.EndOfText);
			this.textNavigator.MoveTo(Text.TextNavigator.Target.TextStart, 0);
			
			this.textStory.EnableOpletQueue();
		}


		public Text.TextStory TextStory
		{
			get { return this.textStory; }
		}

		public Text.TextFitter TextFitter
		{
			get { return this.textFitter; }
		}

		public Text.TextNavigator TextNavigator
		{
			get { return this.textNavigator; }
		}

		public TextNavigator2 MetaNavigator
		{
			get { return this.metaNavigator; }
		}


		public void Add(Objects.AbstractText obj, Objects.AbstractText parent, bool after)
		{
			//	Ajoute un pav� de texte dans la cha�ne de l'objet parent.
			//	Si le pav� obj fait d�j� partie d'une autre cha�ne, toute sa cha�ne est ajout�e
			//	dans la cha�ne de l'objet parent.
			if ( parent == null )
			{
				this.objectsChain.Add(obj);
				this.TextFitter.FrameList.Add(obj.TextFrame);
			}
			else
			{
				System.Diagnostics.Debug.Assert(parent.TextFlow == this);

				//	Si l'objet � ajouter dans la cha�ne y est d�j�, mais ailleurs,
				//	il faut d'abord l'enlever.
				if ( this.objectsChain.Contains(obj) )
				{
					obj.TextFlow.Remove(obj);
				}

				int index = this.objectsChain.IndexOf(parent);
				System.Diagnostics.Debug.Assert(index != -1);

				//	Met dans la liste srcChain tous les objets faisant partie de la cha�ne
				//	de l'objet � ajouter (il peut faire partie d'une autre cha�ne).
				TextFlow srcFlow = obj.TextFlow;
				System.Collections.ArrayList srcChain = new System.Collections.ArrayList();
				foreach ( Objects.AbstractText listObj in srcFlow.Chain )
				{
					srcChain.Add(listObj);
				}

				foreach ( Objects.AbstractText listObj in srcChain )
				{
					listObj.TextFlow.Remove(listObj);
				}

				this.MergeWith(srcFlow);  // cha�ne parent <- texte source
				this.document.TextFlows.Remove(srcFlow);

				//	Ajoute toute la cha�ne initiale � la cha�ne de l'objet parent.
				if ( after )  index ++;
				foreach ( Objects.AbstractText listObj in srcChain )
				{
					this.objectsChain.Insert(index, listObj);
					this.TextFitter.FrameList.InsertAt(index, listObj.TextFrame);

					listObj.TextFlow = this;

					index ++;
				}
			}
			
			this.textStory.NotifyTextChanged();
		}

		public void MergeWith(TextFlow src)
		{
			//	Fusionne le texte d'un flux source � la fin du flux courant.
			//	TODO: g�rer les tabulateurs, les styles, etc.
			src.metaNavigator.ClearSelection();
			src.textNavigator.MoveTo(Text.TextNavigator.Target.TextStart, 1);
			src.textNavigator.StartSelection();
			src.textNavigator.MoveTo(Text.TextNavigator.Target.TextEnd, 1);
			src.textNavigator.EndSelection();
			
			if (src.textStory.TextContext == this.textStory.TextContext)
			{
				//	Si les TextContext sont compatibles, on travaille directement
				//	avec le texte de bas niveau (format 64-bit) :
				
				ulong[] text = src.textNavigator.GetSelectedLowLevelText(0);
				ulong marker = ~src.textNavigator.TextContext.Markers.Selected;

				for (int i = 0; i < text.Length; i++)
				{
					text[i] &= marker;
				}
				
				this.metaNavigator.MoveTo(Text.TextNavigator.Target.TextEnd, 1, 1, false);
				this.metaNavigator.DeleteSelection();

				if (text.Length == 0)
				{
					//	Cas particulier : la copie n'a rien copi�, mais on veut
					//	tout de m�me s'assurer que le caract�re de fin de texte
					//	a les bons attributs.

					TextStyle[] srcStyles = src.textNavigator.TextStyles;
					this.textNavigator.SetParagraphStyles (srcStyles);
					this.textNavigator.SetTextStyles (srcStyles);
					this.textNavigator.SetMetaProperties (Epsitec.Common.Text.Properties.ApplyMode.Set, srcStyles);
				}
				else
				{
					this.textStory.InsertText (this.textNavigator.ActiveCursor, text);
				}
			}
			else
			{
				//	Les TextContext sont diff�rents; il faudrait passer par une
				//	repr�sentation "portable"; comme elle n'existe pas pour le
				//	moment, on se contente de copier juste le texte :
				
				string[] texts = src.textNavigator.GetSelectedTexts();
				
				this.metaNavigator.MoveTo(Text.TextNavigator.Target.TextEnd, 1, 1, false);
				this.metaNavigator.Insert(texts[0]);
			}
		}

		public void Remove(Objects.AbstractText obj)
		{
			//	Supprime un pav� de texte de la cha�ne. Le pav� sera alors solitaire.
			//	Le texte lui-m�me reste dans la cha�ne initiale.
			if ( this.objectsChain.Count == 1 )  return;

			this.objectsChain.Remove(obj);
			this.textFitter.FrameList.Remove(obj.TextFrame);
			this.textStory.NotifyTextChanged();

			obj.UpdateTextLayout();
			
			this.NotifyAreaFlow();

			obj.NewTextFlow();
		}
		
		public void RebuildFrameList()
		{
			System.Collections.ArrayList list = new	System.Collections.ArrayList();
			
			foreach ( Objects.AbstractText obj in this.objectsChain )
			{
				list.Add(obj.TextFrame);
			}
			
			this.textFitter.FrameList.Reset(list);
			this.textStory.NotifyTextChanged();
		}
		
		public int Count
		{
			//	Nombre de pav�s de texte dans la cha�ne.
			get
			{
				return this.objectsChain.Count;
			}
		}

		public int Rank(Objects.AbstractText obj)
		{
			//	Rang d'un pav� de texte dans la cha�ne.
			return this.objectsChain.IndexOf(obj);
		}

		public UndoableList Chain
		{
			//	Donne la cha�ne des pav�s de texte.
			get
			{
				return this.objectsChain;
			}
		}
		
		public Objects.AbstractText ActiveTextBox
		{
			//	Pav� actuellement en �dition.
			get
			{
				return this.activeTextBox;
			}
			set
			{
				if ( this.activeTextBox != value )
				{
					this.activeTextBox = value;
				}
			}
		}
		
		public bool HasActiveTextBox
		{
			//	Y a-t-il un pav� en �dition ?
			get
			{
				return this.activeTextBox != null;
			}
		}

		public void UpdateTextRulers()
		{
			//	Met � jour les r�gles pour le texte en �dition.
			if ( this.document.HRuler == null )  return;

			if ( this.HasActiveTextBox )
			{
				Drawing.Rectangle bbox = this.activeTextBox.BoundingBoxThin;
				double width = this.activeTextBox.WidthForHRuler;
				this.document.HRuler.LimitLow  = bbox.Left;
				this.document.HRuler.LimitHigh = bbox.Left+width;
				this.document.VRuler.LimitLow  = bbox.Bottom;
				this.document.VRuler.LimitHigh = bbox.Top;

				Text.TextNavigator.TabInfo[] infos = this.TextNavigator.GetTabInfos();

				Widgets.Tab[] tabs = new Widgets.Tab[infos.Length];
				for ( int i=0 ; i<infos.Length ; i++ )
				{
					Text.TextNavigator.TabInfo info = infos[i] as Text.TextNavigator.TabInfo;

					double pos;
					Drawing.TextTabType type;
					Objects.AbstractText.GetTextTab(this.document, info.Tag, out pos, out type);

					tabs[i].Tag = info.Tag;
					tabs[i].Pos = bbox.Left+pos;
					tabs[i].Type = type;
					tabs[i].Shared = (info.Class == TabClass.Shared);
					tabs[i].Zombie = (info.Status == TabStatus.Zombie);
				}
				this.document.HRuler.Tabs = tabs;
			}
		}
		
		public void UpdateClipboardCommands()
		{
			//	Met � jour les commandes du clipboard.
			if ( this.document.CommandDispatcher == null )  return;

			bool sel = (this.TextNavigator.SelectionCount != 0);

			this.document.GetCommandState ("Cut").Enable = sel;
			this.document.GetCommandState ("Copy").Enable = sel;
		}

		public void NotifyAreaFlow()
		{
			//	Notifie "� repeindre" toute la cha�ne des pav�s.
			UndoableList chain = this.Chain;

			foreach ( Viewer viewer in this.document.Modifier.Viewers )
			{
				int currentPage = viewer.DrawingContext.CurrentPage;
				foreach ( Objects.AbstractText obj in chain )
				{
					if ( obj == null )  continue;
					if ( obj.PageNumber != currentPage )  continue;

					this.document.Notifier.NotifyArea(viewer, obj.BoundingBox);
				}
			}
		}

		public void NotifyAboutToExecuteCommand()
		{
			//	Signale que le document va ex�cuter une commande. Cette m�thode est
			//	par exemple appel�e avant que le UNDO ne soit ex�cut�.
			if ( this.textNavigator != null && this.textNavigator.IsSelectionActive )
			{
				this.textNavigator.EndSelection();
			}
		}

		private void ChangeObjectEdited()
		{
			//	Change �ventuellement le pav� �dit� en fonction de la position du curseur.
			//	Si un changement a lieu, des oplets seront cr��s. Cette m�thode ne peut donc
			//	pas �tre appel�e pendant une op�ration de undo/redo.
			System.Diagnostics.Debug.Assert(!this.textStory.OpletQueue.IsUndoRedoInProgress);
			
			Text.ITextFrame frame;
			
			if ( this.HasActiveTextBox &&
				 this.textNavigator.GetCursorGeometry(out frame) )
			{
				if ( frame != this.activeTextBox.TextFrame )
				{
					Objects.AbstractText obj = this.FindMatchingTextBox(frame);
					System.Diagnostics.Debug.Assert(obj != null);
					
					this.document.Modifier.SetEditObject(obj, false);
					System.Diagnostics.Debug.Assert(this.HasActiveTextBox);
					
					this.activeTextBox.SetAutoScroll();
				}
			}
		}
		
		private void SynchroniseObjectEdited()
		{
			if ( !this.textStory.OpletQueue.IsUndoRedoInProgress )
			{
				Text.ITextFrame frame;
				
				if ( this.HasActiveTextBox &&
					 this.textNavigator.IsSelectionActive &&
					 this.textNavigator.GetCursorGeometry(out frame) )
				{
					if ( frame != this.activeTextBox.TextFrame )
					{
						//	En provoquant une fin de s�lection provisoire ici, on force la
						//	g�n�ration d'un oplet et l'appel indirect de ChangeObjectEdited
						//	qui va mettre � jour le pav� en �dition :
						
						this.textNavigator.EndSelection();
					}
				}
			}
		}
		
		public Objects.AbstractText FindObject()
		{
			//	Cherche l'objet dans lequel est le curseur.
			Text.ITextFrame frame;
			this.textNavigator.GetCursorGeometry(out frame);
			if ( frame == null )  return null;

			return this.FindMatchingTextBox(frame);
		}

		private Objects.AbstractText FindMatchingTextBox(Text.ITextFrame frame)
		{
			//	Trouve le pav� correspondant au frame donn�.
			foreach ( Objects.AbstractText obj in this.objectsChain )
			{
				if ( frame == obj.TextFrame )
				{
					return obj;
				}
			}
			
			return null;
		}
		
		public Objects.AbstractText FindInTextFrame(Drawing.Point pos)
		{
			//	Trouve le TextFrame qui correspond � la coordonn�e souris.
			foreach ( Objects.AbstractText obj in this.objectsChain )
			{
				if ( obj.IsInTextFrame(pos) )
				{
					return obj;
				}
			}
			
			return null;
		}
		
		
		public void UpdateTextLayout()
		{
			this.textFitter.GenerateMarks();

			//	Indique qu'il faudra recalculer les bbox � toute la cha�ne des pav�s.
			foreach ( Objects.AbstractText obj in this.objectsChain )
			{
				if ( obj == null )  continue;
				obj.SetDirtyBbox();
			}
		}

		public void UpdateTabs()
		{
			this.HandleTextNavigatorTabsChanged(null);
		}


		public static void StatisticFonts(List<OpenType.FontName> list, UndoableList textFlows, TextStats.FontNaming fontNaming)
		{
			//	Remplit une liste avec tous les noms des polices utilis�es dans tous
			//	les TextFlows du document.
			foreach ( TextFlow flow in textFlows )
			{
				Text.TextStats stats = new TextStats(flow.textStory);
				OpenType.FontName[] fontNames = stats.GetFontUse(fontNaming);

				foreach ( OpenType.FontName fontName in fontNames )
				{
					if ( !list.Contains(fontName) )
					{
						list.Add(fontName);
					}
				}
			}
		}

		public static void ReadCheckWarnings(UndoableList textFlows, System.Collections.ArrayList warnings)
		{
			//	V�rifie que toutes les polices existent apr�s l'ouverture d'un document.
			if ( textFlows.Count == 0 )  return;

			List<OpenType.FontName> documentList = new List<OpenType.FontName>();
			TextFlow.StatisticFonts (documentList, textFlows, TextStats.FontNaming.Invariant);
			documentList.Sort();

			foreach (OpenType.FontName fontName in documentList)
			{
				if (!Misc.IsExistingFont(fontName))
				{
					string message = string.Format(Res.Strings.Object.Text.Error, fontName.FullName);
					if ( !warnings.Contains(message) )
					{
						warnings.Add(message);
					}
				}
			}
		}


		private void HandleTextNavigatorCursorMoved(object sender)
		{
			if ( this.HasActiveTextBox )
			{
				this.UpdateTextRulers();
				this.UpdateClipboardCommands();
				this.document.Notifier.NotifyTextChanged();
				this.NotifyAreaFlow();
				this.SynchroniseObjectEdited();
//				this.ChangeObjectEdited();
			}
		}
		
		private void HandleTextNavigatorTextChanged(object sender)
		{
			if ( this.HasActiveTextBox )
			{
				this.UpdateTextLayout();
				this.UpdateClipboardCommands();
				this.document.Notifier.NotifyTextChanged();
				this.NotifyAreaFlow();
			}
		}
		
		private void HandleTextNavigatorActiveStyleChanged(object sender)
		{
			if ( this.HasActiveTextBox )
			{
				this.UpdateTextLayout();
				this.document.Notifier.NotifyTextChanged();
				this.NotifyAreaFlow();
			}
		}

		private void HandleTextNavigatorTabsChanged(object sender)
		{
			if ( this.HasActiveTextBox )
			{
				this.UpdateTextRulers();
				this.UpdateTextLayout();
				this.document.Notifier.NotifyTextChanged();
				this.NotifyAreaFlow();
			}
		}

		private void HandleTextStoryOpletExecuted(object sender, OpletEventArgs e)
		{
			switch ( e.Event )
			{
				case Common.Support.OpletEvent.AddingOplet:
					this.ChangeObjectEdited();
					break;
			}
		}
		
		private void HandleTextStoryRenamingOplet(string name, ref string output)
		{
			//	Quand TextStory valide un ou plusieurs oplets et qu'il n'y a aucun
			//	nom d�fini, ce call-back est appel�, permettant de fournir un nom
			//	qui a un sens pour l'utilisateur.
			
			if ( name.IndexOf("TextDelete") >= 0 )
			{
				string format = Res.Strings.Action.Text.Edit.DeleteText;
				string insert;
				string delete;
				
				this.textStory.GetOpletsText(this.textStory.OpletQueue.LastActionOplets, out insert, out delete);
				output = string.Format(System.Globalization.CultureInfo.InvariantCulture, format, Misc.Resume(delete));
			}
			else if ( name.IndexOf("TextInsert") >= 0 )
			{
				string format = Res.Strings.Action.Text.Edit.InsertText;
				string insert;
				string delete;
				
				this.textStory.GetOpletsText(this.textStory.OpletQueue.LastActionOplets, out insert, out delete);
				output = string.Format(System.Globalization.CultureInfo.InvariantCulture, format, Misc.Resume(insert));
			}
			else if ( name.IndexOf("CursorMove") >= 0 )
			{
				output = Res.Strings.Action.Text.Edit.MoveCursor;
			}
			else if ( name.IndexOf("DefineSelection") >= 0 )
			{
				output = Res.Strings.Action.Text.Edit.DefineSelection;
			}
			else if ( name.IndexOf("ClearSelection") >= 0 )
			{
				output = Res.Strings.Action.Text.Edit.ClearSelection;
			}
			else if ( name.IndexOf("TextChange") >= 0 )
			{
				output = Res.Strings.Action.Text.Edit.ChangeText;
			}
		}
		
		
		#region Serialization
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			//	S�rialise l'objet.
			info.AddValue("ObjectsChain", this.objectsChain);

			byte[] textStoryData = this.textStory.Serialize();
			info.AddValue("TextStoryData", textStoryData);
		}

		protected TextFlow(SerializationInfo info, StreamingContext context) : this()
		{
			//	Constructeur qui d�s�rialise l'objet.
			this.document = Document.ReadDocument;

			this.objectsChain = (UndoableList) info.GetValue("ObjectsChain", typeof(UndoableList));
			this.textStoryData = (byte[]) info.GetValue("TextStoryData", typeof(byte[]));
		}

		public void ReadFinalizeTextStory()
		{
			//	Adapte l'objet apr�s une d�s�rialisation.
			this.InitializeNavigator ();
			
			System.Diagnostics.Debug.Assert(this.textStory != null);
			System.Diagnostics.Debug.Assert(this.textStory.OpletQueue != null);
			
			this.textStory.Deserialize(this.textStoryData);
			this.textStoryData = null;
		}
		
		public void ReadFinalizeTextObj()
		{
			System.Diagnostics.Debug.Assert(this.TextFitter.FrameList.Count == 0);
			
again:
			foreach ( Objects.AbstractText obj in this.objectsChain )
			{
				if ( obj.TextFrame == null )
				{
					//	Catastrophe: le fichier lu contient des objets texte qui n'ont
					//	pas de frame associ�; �a ne devrait jamais arriver, mais �a
					//	arrive quand-m�me...
					
					this.objectsChain.Remove(obj);
					goto again;
				}
			}
			
			foreach ( Objects.AbstractText obj in this.objectsChain )
			{
				System.Diagnostics.Debug.Assert(obj.TextFrame != null);
				this.textFitter.FrameList.Add(obj.TextFrame);
				obj.ReadFinalizeFlowReady(this);
			}

			this.textFitter.ClearAllMarks();
			this.textFitter.GenerateAllMarks();
		}
		#endregion

		
		protected Document						document;
		protected byte[]						textStoryData;
		protected Text.TextStory				textStory;
		protected Text.TextFitter				textFitter;
		protected Text.TextNavigator			textNavigator;
		protected TextNavigator2				metaNavigator;
		protected Objects.AbstractText			activeTextBox;
		protected UndoableList					objectsChain;
	}
}
