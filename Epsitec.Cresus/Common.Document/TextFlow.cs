using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Common.Text;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document
{
	/// <summary>
	/// Flux de texte.
	/// </summary>
	[System.Serializable()]
	public class TextFlow : ISerializable
	{
		// Crée un nouveau flux pour un seul pavé.
		public TextFlow(Document document)
		{
			this.document = document;

			this.Initialise();
			this.objectsChain = new UndoableList(this.document, UndoableListType.ObjectsChain);
		}

		protected void Initialise()
		{
			if ( this.document.Modifier == null )
			{
				this.textStory = new Text.TextStory(this.document.TextContext);
			}
			else
			{
				this.textStory = new Text.TextStory(this.document.Modifier.OpletQueue, this.document.TextContext);
			}

			this.textFitter    = new Text.TextFitter(this.textStory);
			this.textNavigator = new Text.TextNavigator(this.textFitter);
			
			this.textStory.TextContext.IsDegradedLayoutEnabled = true;
			this.textStory.DebugDisableOpletQueue = true;
			this.textNavigator.Insert(Text.Unicode.Code.EndOfText);
			this.textNavigator.MoveTo(Text.TextNavigator.Target.TextStart, 0);
			this.textStory.DebugDisableOpletQueue = false;
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


		// Ajoute un pavé de texte dans la chaîne de l'objet parent.
		// Si le pavé obj fait déjà partie d'une autre chaîne, toute sa chaîne est ajoutée
		// dans la chaîne de l'objet parent.
		public void Add(Objects.TextBox2 obj, Objects.TextBox2 parent, bool after)
		{
			if ( parent == null )
			{
				this.objectsChain.Add(obj);
				this.TextFitter.FrameList.Add(obj.TextFrame);
			}
			else
			{
				System.Diagnostics.Debug.Assert(parent.TextFlow == this);

				// Si l'objet à ajouter dans la chaîne y est déjà, mais ailleurs,
				// il faut d'abord l'enlever.
				if ( this.objectsChain.Contains(obj) )
				{
					obj.TextFlow.Remove(obj);
				}

				int index = this.objectsChain.IndexOf(parent);
				System.Diagnostics.Debug.Assert(index != -1);

				// Met dans la liste srcChain tous les objets faisant partie de la chaîne
				// de l'objet à ajouter (il peut faire partie d'une autre chaîne).
				TextFlow srcFlow = obj.TextFlow;
				System.Collections.ArrayList srcChain = new System.Collections.ArrayList();
				foreach ( Objects.TextBox2 listObj in srcFlow.Chain )
				{
					srcChain.Add(listObj);
				}

				foreach ( Objects.TextBox2 listObj in srcChain )
				{
					listObj.TextFlow.Remove(listObj);
				}

				this.MergeWith(srcFlow);  // chaîne parent <- texte source
				this.document.TextFlows.Remove(srcFlow);

				// Ajoute toute la chaîne initiale à la chaîne de l'objet parent.
				if ( after )  index ++;
				foreach ( Objects.TextBox2 listObj in srcChain )
				{
					this.objectsChain.Insert(index, listObj);
					this.TextFitter.FrameList.InsertAt(index, listObj.TextFrame);

					listObj.TextFlow = this;

					index ++;
				}
			}
		}

		// Fusionne le texte d'un flux source à la fin du flux courant.
		// TODO: gérer les tabulateurs, les styles, etc.
		public void MergeWith(TextFlow src)
		{
			src.textNavigator.MoveTo(Text.TextNavigator.Target.TextStart, 1);
			src.textNavigator.StartSelection();
			src.textNavigator.MoveTo(Text.TextNavigator.Target.TextEnd, 1);
			src.textNavigator.EndSelection();
			string[] texts = src.textNavigator.GetSelectedTexts();

			this.textNavigator.MoveTo(Text.TextNavigator.Target.TextEnd, 1);
			this.textNavigator.Insert(texts[0]);
		}

		// Supprime un pavé de texte de la chaîne. Le pavé sera alors solitaire.
		// Le texte lui-même reste dans la chaîne initiale.
		public void Remove(Objects.TextBox2 obj)
		{
			if ( this.objectsChain.Count == 1 )  return;

			this.objectsChain.Remove(obj);
			this.TextFitter.FrameList.Remove(obj.TextFrame);

			obj.UpdateTextLayout();
			obj.NotifyAreaFlow();

			obj.NewTextFlow();
		}

		// Nombre de pavés de texte dans la chaîne.
		public int Count
		{
			get
			{
				return this.objectsChain.Count;
			}
		}

		// Rang d'un pavé de texte dans la chaîne.
		public int Rank(Objects.Abstract obj)
		{
			return this.objectsChain.IndexOf(obj);
		}

		// Donne la chaîne des pavés de texte.
		public UndoableList Chain
		{
			get
			{
				return this.objectsChain;
			}
		}


		#region Serialization
		// Sérialise l'objet.
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("ObjectsChain", this.objectsChain);

			byte[] textStoryData = this.textStory.Serialize();
			info.AddValue("TextStoryData", textStoryData);
		}

		// Constructeur qui désérialise l'objet.
		protected TextFlow(SerializationInfo info, StreamingContext context)
		{
			this.document = Document.ReadDocument;

			this.objectsChain = (UndoableList) info.GetValue("ObjectsChain", typeof(UndoableList));
			this.textStoryData = (byte[]) info.GetValue("TextStoryData", typeof(byte[]));
		}

		// Adapte l'objet après une désérialisation.
		public void ReadFinalize()
		{
			this.Initialise();
			this.textStory.Deserialize(this.textStoryData);
			this.textStoryData = null;
			
			System.Diagnostics.Debug.Assert(this.TextFitter.FrameList.Count == 0);
			foreach ( Objects.TextBox2 obj in this.objectsChain )
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
		protected UndoableList					objectsChain;
	}
}
