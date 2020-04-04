using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

using OpenFeasyo.Platform.Controls.Reports;

namespace OpenFeasyo.Platform.Controls.Constraints
{
    [DataContract]
    [Namespace(Prefix = "xna", Uri = "http://schemas.datacontract.org/2004/07/Microsoft.Xna.Framework")]
    [KnownType(typeof(SkeletonConstraintAngle))]
    [KnownType(typeof(SkeletonConstraintPosition))]
    public class SkeletonConstraintsCollection
    {
        private List<ISkeletonConstraint> constraints;
        [DataMember]
        public List<ISkeletonConstraint> Constraints
        {
            get { return constraints; }
            set { constraints = value; }
        }

        private TimeSpan timeToHold;
        [DataMember]
        public TimeSpan TimeToHold
        {
            get { return timeToHold; }
            set { timeToHold = value; }
        }

        public SkeletonConstraintsCollection()
        {
            constraints = new List<ISkeletonConstraint>();
            timeToHold = TimeSpan.FromSeconds(0);
        }

        public void CheckAll(ISkeleton skeleton, ISkeletonReport report)
        {
            foreach (ISkeletonConstraint constraint in constraints)
            {
                constraint.Check(skeleton, report);
            }
        }

        public void WriteConstraints(string fileName)
        {
            object[] attributes;
            attributes = this.GetType().GetCustomAttributes(typeof(NamespaceAttribute), true);

            var ds = new DataContractSerializer(typeof(SkeletonConstraintsCollection));

            var settings = new XmlWriterSettings { Indent = true, NewLineOnAttributes = true };

            using (var w = XmlWriter.Create(fileName, settings))
            {
                ds.WriteStartObject(w, this);
                foreach (NamespaceAttribute ns in attributes)
                    w.WriteAttributeString("xmlns", ns.Prefix, null, ns.Uri);

                // content
                ds.WriteObjectContent(w, this);
                ds.WriteEndObject(w);
            }
        }

        public void ReadConstraints(string fileName)
        {
            FileStream fs = new FileStream(fileName, FileMode.Open);
            XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas());
            DataContractSerializer ser = new DataContractSerializer(typeof(SkeletonConstraintsCollection));

            // Deserialize the data and read it from the instance.
            SkeletonConstraintsCollection deserializedCollection =
                (SkeletonConstraintsCollection)ser.ReadObject(reader, true);
            this.constraints = deserializedCollection.Constraints;
            this.TimeToHold = deserializedCollection.TimeToHold;
            reader.Close();
            fs.Close();
        }

        public void setDefaultConstraints()
        {

            foreach (PlayerJoint joint in Enum.GetValues(typeof(PlayerJoint)))
            {
                switch (joint)
                {
                    case PlayerJoint.HipCenter:
                        continue;
                    default:
                        constraints.Add(new SkeletonConstraintPosition(joint, Vector3.Zero, Vector3.Zero, 0));
                        break;
                }
            }

            foreach (PlayerJoint angle in Enum.GetValues(typeof(PlayerJoint)))
            {
                float wishedAngle = 0;
                float threshold = 15;
                int wishness = 0;

                switch (angle)
                {
                    case PlayerJoint.Head:
                    case PlayerJoint.ElbowLeft:
                    case PlayerJoint.ShoulderCenter:
                    case PlayerJoint.ElbowRight:
                    case PlayerJoint.HipRight:
                    case PlayerJoint.HipLeft:
                    case PlayerJoint.KneeRight:
                    case PlayerJoint.KneeLeft:
                        wishedAngle = 180;
                        break;
                    case PlayerJoint.Spine:
                    case PlayerJoint.HipCenter:
                    case PlayerJoint.Sternum:
                        wishedAngle = 0;
                        break;
                    case PlayerJoint.ShoulderLeft:
                    case PlayerJoint.ShoulderRight:
                        wishedAngle = 90;
                        break;
                    default:
                        continue;
                }

                constraints.Add(new SkeletonConstraintAngle(angle, wishedAngle, threshold, wishness, physioPlanes.NONE));
            }
        }

    }


    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public sealed class NamespaceAttribute : Attribute
    {

        public NamespaceAttribute()
        {
        }

        public NamespaceAttribute(string prefix, string uri)
        {
            Prefix = prefix;
            Uri = uri;
        }

        public string Prefix { get; set; }
        public string Uri { get; set; }
    }
}
