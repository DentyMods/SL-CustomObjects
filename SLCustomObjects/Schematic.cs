using Exiled.API.Features;
using Mirror;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SLCustomObjects
{
    public class Schematic
    {
        public static EventHandler<PickupArgs> PickupInteract;

        public class PickupArgs : EventArgs
        {
            public string SchematicName;
            public Pickup Pickup;
            public string EventName;
        }
        
        public static bool LoadSchematic(string path, Vector3 position)
        {
            if (File.Exists(path))
            {
                string schematicName = Path.GetFileNameWithoutExtension(path);
                UnloadSchematic(schematicName);
                JObject jobj = JObject.Parse(File.ReadAllText(path));
                IDictionary<string, JToken> dict = jobj;
                GameObject ob = new GameObject($"Schematic_{schematicName}");
                ob.transform.position = position;
                try
                {
                    foreach (var sm in dict)
                    {
                        if (Enum.TryParse<ObjectType>(sm.Value["ObjectType"].ToString(), out ObjectType objtype))
                        {
                            switch (objtype)
                            {
                                case ObjectType.Workstation:
                                    var wd = sm.Value.ToObject<SchematicWorkstationData>();
                                    CreateWorkstation(wd.Position.GetJsonVector(), wd.Rotation.GetJsonVector(), wd.Scale.GetJsonVector(), ob.transform, wd.DataID, wd.ParentID, schematicName);
                                    break;
                                case ObjectType.Item:
                                    var id = sm.Value.ToObject<SchematicItemData>();
                                    var obj5 = CreateItem((ItemType)id.ItemID, id.Position.GetJsonVector(), id.Rotation.GetJsonVector(), id.Scale.GetJsonVector(), ob.transform, id.DataID, id.ParentID, schematicName);
                                    if (id.ExecuteEventOnPickup)
                                    {
                                        var ev = obj5.AddComponent<PickupEvent>();
                                        ev.EventName = id.EventName;
                                        ev.SchematicName = schematicName;
                                        ev.pickup = obj5.GetComponent<Pickup>();
                                    }
                                    break;
                                case ObjectType.Empty:
                                    var oe = sm.Value.ToObject<SchematicEmptyData>();
                                    Transform parentObj = GetObjectParent(schematicName, oe.ParentID);
                                    if (parentObj == null) parentObj = ob.transform;
                                    GameObject obj = new GameObject($"Obj_{schematicName}_{oe.DataID}");
                                    obj.transform.parent = parentObj;
                                    break;
                                case ObjectType.Animation:
                                    var oe2 = sm.Value.ToObject<SchematicAnimationData>();
                                    Transform parentObj2 = GetObjectParent(schematicName, oe2.ParentID);
                                    if (parentObj2 == null) parentObj = ob.transform;
                                    GameObject obj2 = new GameObject($"Obj_{schematicName}_{oe2.DataID}");
                                    obj2.transform.parent = parentObj2;
                                    if (oe2.rotateAnimation)
                                    {
                                        obj2.AddComponent<AnimationRotate>().speed = oe2.rotateAnimationSpeed;
                                    }
                                    break;
                            }
                        }

                    }
                }catch(Exception ex)
                {
                    Log.Error(ex.ToString());
                    return false;
                }
                return true;
            }
            return false;
        }

        public static bool UnloadSchematic(string schematicName)
        {
            if (IfSchematicLoaded(schematicName))
            {
                GameObject go = GameObject.Find($"Schematic_{schematicName}");
                UnityEngine.Object.DestroyImmediate(go);
                return true;
            }
            return false;
        }

        public static bool BringSchematic(string schematicName, Player plr)
        {
            if (IfSchematicLoaded(schematicName))
            {
                GameObject go = GameObject.Find($"Schematic_{schematicName}");
                go.transform.position = plr.Position;
                return true;
            }
            return false;
        }

        public static bool SetSchematicPosition(string schematicName, Vector3 position)
        {
            if (IfSchematicLoaded(schematicName))
            {
                GameObject go = GameObject.Find($"Schematic_{schematicName}");
                go.transform.position = position;
                return true;
            }
            return false;
        }

        public static bool IfSchematicLoaded(string schematicName)
        {
            GameObject go = GameObject.Find($"Schematic_{schematicName}");
            if (go == null)
                return false;
            else
                return true;
        }

        public static GameObject CreateItem(ItemType item, Vector3 pos, Vector3 rot, Vector3 scale, Transform parent, int dataid, int parentid, string schematicName)
        {
            Transform parentObj = GetObjectParent(schematicName, parentid);
            if (parentObj == null) parentObj = parent;

            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(ReferenceHub.HostHub.inventory.pickupPrefab, parentObj);
            gameObject.transform.localScale = scale;
            gameObject.name = $"Obj_{schematicName}_{dataid}";
            gameObject.transform.localPosition = pos;
            gameObject.transform.localRotation = Quaternion.Euler(rot);
            gameObject.GetComponent<Rigidbody>().isKinematic = true;
            NetworkServer.Spawn(gameObject);
            gameObject.GetComponent<Pickup>().SetupPickup(item, -1f, ReferenceHub.HostHub.gameObject, new Pickup.WeaponModifiers(false, 0, 0, 0), gameObject.transform.position, gameObject.transform.rotation);
            return gameObject;
        }

        public static GameObject CreateWorkstation(Vector3 pos, Vector3 rot, Vector3 scale, Transform parent, int dataid, int parentid, string schematicName)
        {
            Transform parentObj = GetObjectParent(schematicName, parentid);
            if (parentObj == null) parentObj = parent;

            GameObject obj = UnityEngine.Object.Instantiate<GameObject>(CustomNetworkManager.singleton.spawnPrefabs.Where(tt => tt.name == "Work Station").FirstOrDefault(), parentObj);
            obj.transform.localScale = scale;
            obj.name = $"Obj_{schematicName}_{dataid}";
            obj.transform.localPosition = pos;
            obj.transform.localRotation = Quaternion.Euler(rot);
            NetworkServer.Spawn(obj);
            return obj;
        }

        public static Transform GetObjectParent(string schematicName, int parentID)
        {
            GameObject parObj = GameObject.Find($"Obj_{schematicName}_{parentID}");
            if (parObj == null)
                return null;
            else
                return parObj.transform;
        }
    }
}
