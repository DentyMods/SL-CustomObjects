using Assets;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Schematic : MonoBehaviour
{
    public string SchematicName;
    public string SchematicAuthor;

    private void Awake()
    {
        Traverse(this.gameObject);
        File.WriteAllText(Path.Combine("./Assets", "Schematics", "schematic-" + SchematicName + ".json"), JsonConvert.SerializeObject(susdata, Formatting.Indented));
    }

    public Dictionary<int, SchematicData> susdata = new Dictionary<int, SchematicData>();

    void Traverse(GameObject obj)
    {
        if (obj.TryGetComponent<ObjectRef>(out ObjectRef objref))
        {

            switch (objref.objectType)
            {
                case ObjectType.Door:
                    break;
                case ObjectType.Item:
                    susdata[obj.GetInstanceID()] = new SchematicItemData()
                    {
                        DataID = obj.GetInstanceID(),
                        ParentID = obj.transform.parent != null ? obj.transform.parent.gameObject.GetInstanceID() : 0,
                        ItemID = objref.ItemID,
                        ExecuteEventOnPickup = objref.executeEventOnPickup,
                        EventName = objref.eventName,
                        ObjectType = ObjectType.Item,
                        Position = obj.transform.localPosition.GetJsonVector(),
                        Rotation = obj.transform.localRotation.eulerAngles.GetJsonVector(),
                        Scale = obj.transform.localScale.GetJsonVector()
                    };
                    break;
                case ObjectType.Ragdoll:
                    susdata[obj.GetInstanceID()] = new SchematicRagdollData()
                    {
                        DataID = obj.GetInstanceID(),
                        ParentID = obj.transform.parent != null ? obj.transform.parent.GetInstanceID() : 0,
                        ClassID = objref.ClassID,
                        ObjectType = ObjectType.Ragdoll,
                        Position = obj.transform.localPosition.GetJsonVector(),
                        Rotation = obj.transform.localRotation.eulerAngles.GetJsonVector(),
                        Scale = obj.transform.localScale.GetJsonVector()
                    };
                    break;
                case ObjectType.Workstation:
                    susdata[obj.GetInstanceID()] = new SchematicWorkstationData()
                    {
                        DataID = obj.GetInstanceID(),
                        ParentID = obj.transform.parent != null ? obj.transform.parent.GetInstanceID() : 0,
                        ObjectType = ObjectType.Workstation,
                        Position = obj.transform.localPosition.GetJsonVector(),
                        Rotation = obj.transform.localRotation.eulerAngles.GetJsonVector(),
                        Scale = obj.transform.localScale.GetJsonVector()
                    };
                    break;
                case ObjectType.Collider:
                    susdata[obj.GetInstanceID()] = new SchematicColliderData()
                    {
                        DataID = obj.GetInstanceID(),
                        ParentID = obj.transform.parent != null ? obj.transform.parent.GetInstanceID() : 0,
                        ObjectType = ObjectType.Collider,
                        Position = obj.transform.localPosition.GetJsonVector(),
                        Rotation = obj.transform.localRotation.eulerAngles.GetJsonVector(),
                        Scale = obj.transform.localScale.GetJsonVector()
                    };
                    break;
            }

        }
        else if (obj.TryGetComponent<AnimationRef>(out AnimationRef anim))
        {
            susdata[obj.GetInstanceID()] = new SchematicAnimationData()
            {
                DataID = obj.GetInstanceID(),
                ParentID = obj.transform.parent != null ? obj.transform.parent.GetInstanceID() : 0,
                ObjectType = ObjectType.Animation,
                rotateAnimationSpeed = anim.rotateAnimationSpeed,
                rotateAnimation = anim.rotateAnimation,
                Position = obj.transform.localPosition.GetJsonVector(),
                Rotation = obj.transform.localRotation.eulerAngles.GetJsonVector(),
                Scale = obj.transform.localScale.GetJsonVector()
            };
        }
        else
        {
            susdata[obj.GetInstanceID()] = new SchematicEmptyData()
            {
                DataID = obj.GetInstanceID(),
                ParentID = obj.transform.parent != null ? obj.transform.parent.GetInstanceID() : 0,
                ObjectType = ObjectType.Empty,
                Position = obj.transform.localPosition.GetJsonVector(),
                Rotation = obj.transform.localRotation.eulerAngles.GetJsonVector(),
                Scale = obj.transform.localScale.GetJsonVector()
            };
        }
        foreach (Transform child in obj.transform)
        {
            Traverse(child.gameObject);
        }
    }
}
