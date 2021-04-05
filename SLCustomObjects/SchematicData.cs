using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLCustomObjects
{
    public class SchematicWorkstationData : SchematicData
    {
    }

    public class SchematicItemData : SchematicData
    {
        public int ItemID { get; set; }
        public bool ExecuteEventOnPickup { get; set; }
        public string EventName { get; set; }
    }

    public class SchematicRagdollData : SchematicData
    {
        public int ClassID { get; set; }
    }

    public class SchematicAnimationData : SchematicData
    {
        public bool rotateAnimation { get; set; } = false;
        public float rotateAnimationSpeed { get; set; } = 3f;
        public bool blinkAnimation { get; set; } = false;
        public float blinkAnimationSpeed { get; set; } = 3f;
    }

    public class SchematicEmptyData : SchematicData
    {

    }

    public class SchematicColliderData : SchematicData
    {

    }


    public class SchematicData
    {
        public int DataID { get; set; }
        public int ParentID { get; set; }
        public JsonVector3 Position { get; set; }
        public JsonVector3 Rotation { get; set; }
        public JsonVector3 Scale { get; set; }
        public ObjectType ObjectType { get; set; }
    }

    public enum ObjectType
    {
        Workstation,
        Door,
        Ragdoll,
        Item,
        Animation,
        Empty,
        Collider
    }

    public class JsonVector3
    {
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
    }
}
