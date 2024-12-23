/*
This file is part of CreativeDocs.

Copyright © 2003-2024, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland

CreativeDocs is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
any later version.

CreativeDocs is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

using System.Runtime.Serialization;
using System.Xml.Linq;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Properties
{
    //	ATTENTION: Ne jamais modifier les valeurs existantes de cette liste,
    //	sous peine de plantée lors de la désérialisation.
    public enum ArcType
    {
        Full = 0,
        Open = 1,
        Close = 2,
        Pie = 3,
    }

    /// <summary>
    /// La classe Arc représente une propriété d'un objet graphique.
    /// </summary>
    [System.Serializable()]
    public class Arc : Abstract, Support.IXMLSerializable<Arc>
    {
        public Arc(Document document, Type type)
            : base(document, type) { }

        protected override void Initialize()
        {
            base.Initialize();
            this.arcType = ArcType.Full;
            this.startingAngle = 90.0;
            this.endingAngle = 360.0;
        }

        public ArcType ArcType
        {
            get { return this.arcType; }
            set
            {
                if (this.arcType != value)
                {
                    this.NotifyBefore();
                    this.arcType = value;
                    this.NotifyAfter();
                }
            }
        }

        public double StartingAngle
        {
            get { return this.startingAngle; }
            set
            {
                if (this.startingAngle != value)
                {
                    this.NotifyBefore();
                    this.startingAngle = value;
                    this.NotifyAfter();
                }
            }
        }

        public double EndingAngle
        {
            get { return this.endingAngle; }
            set
            {
                if (this.endingAngle != value)
                {
                    this.NotifyBefore();
                    this.endingAngle = value;
                    this.NotifyAfter();
                }
            }
        }

#if false
		public override string SampleText
		{
			//	Donne le petit texte pour les échantillons.
			get
			{
				string a1 = this.document.Modifier.AngleToString(this.startingAngle);
				string a2 = this.document.Modifier.AngleToString(this.endingAngle);
				return string.Format("{0} - {1}", a1, a2);
			}
		}
#endif

        public override void PutStyleBrief(System.Text.StringBuilder builder)
        {
            //	Construit le texte résumé d'un style pour une propriété.
            this.PutStyleBriefPrefix(builder);

            builder.Append(Arc.GetName(this.arcType));

            if (this.ArcType != Properties.ArcType.Full)
            {
                builder.Append(", ");
                builder.Append(this.document.Modifier.AngleToString(this.startingAngle));
                builder.Append(", ");
                builder.Append(this.document.Modifier.AngleToString(this.endingAngle));
            }

            this.PutStyleBriefPostfix(builder);
        }

        public static string GetName(ArcType type)
        {
            //	Retourne le nom d'un type donné.
            string name = "";
            switch (type)
            {
                case ArcType.Full:
                    name = Res.Strings.Property.Arc.Full;
                    break;
                case ArcType.Open:
                    name = Res.Strings.Property.Arc.Open;
                    break;
                case ArcType.Close:
                    name = Res.Strings.Property.Arc.Close;
                    break;
                case ArcType.Pie:
                    name = Res.Strings.Property.Arc.Pie;
                    break;
            }
            return name;
        }

        public static string GetIconText(ArcType type)
        {
            //	Retourne l'icône pour un type donné.
            switch (type)
            {
                case ArcType.Full:
                    return "ArcFull";
                case ArcType.Open:
                    return "ArcOpen";
                case ArcType.Close:
                    return "ArcClose";
                case ArcType.Pie:
                    return "ArcPie";
            }
            return "";
        }

        public override bool AlterBoundingBox
        {
            //	Indique si un changement de cette propriété modifie la bbox de l'objet.
            get { return true; }
        }

        public override void MoveHandleStarting(
            Objects.Abstract obj,
            int rank,
            Point pos,
            DrawingContext drawingContext
        )
        {
            //	Début du déplacement d'une poignée.
            Point center = obj.Handle(0).Position;
            drawingContext.ConstrainAddCenter(center, false, 0);
        }

        public override int TotalHandle(Objects.Abstract obj)
        {
            //	Nombre de poignées.
            return 2;
        }

        public override bool IsHandleVisible(Objects.Abstract obj, int rank)
        {
            //	Indique si une poignée est visible.
            if (!this.document.Modifier.IsPropertiesExtended(this.type))
            {
                return false;
            }

            return (this.arcType != ArcType.Full);
        }

        public override Point GetHandlePosition(Objects.Abstract obj, int rank)
        {
            //	Retourne la position d'une poignée.
            Point pos = new Point();

            if (obj is Objects.Circle)
            {
                Objects.Circle circle = obj as Objects.Circle;

                if (rank == 0) // starting angle ?
                {
                    pos = circle.ComputeArcHandle(this.startingAngle);
                }

                if (rank == 1) // ending angle ?
                {
                    pos = circle.ComputeArcHandle(this.endingAngle);
                }
            }

            if (obj is Objects.Ellipse)
            {
                Objects.Ellipse ellipse = obj as Objects.Ellipse;

                if (rank == 0) // starting angle ?
                {
                    pos = ellipse.ComputeArcHandle(this.startingAngle);
                }

                if (rank == 1) // ending angle ?
                {
                    pos = ellipse.ComputeArcHandle(this.endingAngle);
                }
            }

            return pos;
        }

        public override void SetHandlePosition(Objects.Abstract obj, int rank, Point pos)
        {
            //	Modifie la position d'une poignée.
            if (obj is Objects.Circle)
            {
                Objects.Circle circle = obj as Objects.Circle;

                if (rank == 0) // starting angle ?
                {
                    this.StartingAngle = circle.ComputeArcHandle(pos);
                }

                if (rank == 1) // ending angle ?
                {
                    this.EndingAngle = circle.ComputeArcHandle(pos);
                }
            }

            if (obj is Objects.Ellipse)
            {
                Objects.Ellipse ellipse = obj as Objects.Ellipse;

                if (rank == 0) // starting angle ?
                {
                    this.StartingAngle = ellipse.ComputeArcHandle(pos);
                }

                if (rank == 1) // ending angle ?
                {
                    this.EndingAngle = ellipse.ComputeArcHandle(pos);
                }
            }

            base.SetHandlePosition(obj, rank, pos);
        }

        public Path PathEllipse(Rectangle rect)
        {
            //	Crée le chemin d'une ellipse inscrite dans un rectangle.
            Stretcher stretcher = new Stretcher();
            stretcher.InitialRectangle = rect;
            stretcher.FinalBottomLeft = rect.BottomLeft;
            stretcher.FinalBottomRight = rect.BottomRight;
            stretcher.FinalTopLeft = rect.TopLeft;
            stretcher.FinalTopRight = rect.TopRight;

            Point center = rect.Center;

            Path path = new Path();

            double a1,
                a2;
            if (this.ArcType == Properties.ArcType.Full)
            {
                a1 = 0.0;
                a2 = 360.0;
            }
            else
            {
                a1 = this.StartingAngle;
                a2 = this.EndingAngle;
            }
            if (a1 != a2)
            {
                Geometry.ArcBezierDeg(
                    path,
                    stretcher,
                    center,
                    rect.Width / 2.0,
                    rect.Height / 2.0,
                    a1,
                    a2,
                    true,
                    false
                );
            }

            if (this.ArcType == Properties.ArcType.Close)
            {
                path.Close();
            }
            if (this.ArcType == Properties.ArcType.Pie)
            {
                path.LineTo(stretcher.Transform(center));
                path.Close();
            }

            return path;
        }

        public override void CopyTo(Abstract property)
        {
            //	Effectue une copie de la propriété.
            base.CopyTo(property);
            Arc p = property as Arc;
            p.arcType = this.arcType;
            p.startingAngle = this.startingAngle;
            p.endingAngle = this.endingAngle;
        }

        public override bool Compare(Abstract property)
        {
            //	Compare deux propriétés.
            if (!base.Compare(property))
                return false;

            Arc p = property as Arc;
            if (p.arcType != this.arcType)
                return false;
            if (p.startingAngle != this.startingAngle)
                return false;
            if (p.endingAngle != this.endingAngle)
                return false;

            return true;
        }

        public override Panels.Abstract CreatePanel(Document document)
        {
            //	Crée le panneau permettant d'éditer la propriété.
            Panels.Abstract.StaticDocument = document;
            return new Panels.Arc(document);
        }

        public override void MoveGlobalStarting()
        {
            //	Début du déplacement global de la propriété.
            if (!this.document.Modifier.ActiveViewer.SelectorAdaptLine)
                return;

            this.InsertOpletProperty();

            this.initialStartingAngle = this.startingAngle;
            this.initialEndingAngle = this.endingAngle;
        }

        public override void MoveGlobalProcess(Selector selector)
        {
            //	Effectue le déplacement global de la propriété.
            if (!this.document.Modifier.ActiveViewer.SelectorAdaptLine)
                return;

            //	Ce traitement ne fonctionne pas avec l'objet ellipse. Avec cet objet, il
            //	faut simplement ne rien faire. Mais comme cette propriété est partagée par
            //	l'objet cercle et l'objet ellipse, ce n'est pas possible !
            if (selector.IsMirrorH || selector.IsMirrorV)
            {
                this.startingAngle = Math.ClipAngleDeg(360 - this.initialEndingAngle);
                this.endingAngle = Math.ClipAngleDeg(360 - this.initialStartingAngle);
            }

            this.document.Notifier.NotifyPropertyChanged(this);
        }

        #region Serialization
        public new bool HasEquivalentData(Support.IXMLWritable other)
        {
            Arc otherArc = (Arc)other;
            return base.HasEquivalentData(other)
                && this.arcType == otherArc.arcType
                && (
                    this.arcType == ArcType.Full
                    || (
                        this.startingAngle == otherArc.startingAngle
                        && this.endingAngle == otherArc.endingAngle
                    )
                );
        }

        public override XElement ToXML()
        {
            var root = new XElement(
                "Arc",
                base.IterXMLParts(),
                new XAttribute("ArcType", this.arcType)
            );
            if (this.arcType != ArcType.Full)
            {
                root.Add(
                    new XAttribute("StartingAngle", this.startingAngle),
                    new XAttribute("EndingAngle", this.endingAngle)
                );
            }
            return root;
        }

        public static Arc FromXML(XElement xml)
        {
            return new Arc(xml);
        }

        private Arc(XElement xml)
            : base(xml)
        {
            ArcType.TryParse(xml.Attribute("ArcType").Value, out this.arcType);
            if (this.arcType != ArcType.Full)
            {
                this.startingAngle = (double)xml.Attribute("StartingAngle");
                this.endingAngle = (double)xml.Attribute("EndingAngle");
            }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            //	Sérialise la propriété.
            base.GetObjectData(info, context);

            info.AddValue("ArcType", this.arcType);
            if (this.arcType != ArcType.Full)
            {
                info.AddValue("StartingAngle", this.startingAngle);
                info.AddValue("EndingAngle", this.endingAngle);
            }
        }

        protected Arc(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            //	Constructeur qui désérialise la propriété.
            this.arcType = (ArcType)info.GetValue("ArcType", typeof(ArcType));
            if (this.arcType != ArcType.Full)
            {
                this.startingAngle = info.GetDouble("StartingAngle");
                this.endingAngle = info.GetDouble("EndingAngle");
            }
        }
        #endregion


        protected ArcType arcType;
        protected double startingAngle;
        protected double endingAngle;
        protected double initialStartingAngle;
        protected double initialEndingAngle;
    }
}
