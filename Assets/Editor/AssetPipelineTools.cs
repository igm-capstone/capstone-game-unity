using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

[InitializeOnLoad]
public class AssetPipelineTools
{
    [MenuItem("Tools/Export Active Scene")]
    public static void ExportActiveScene()
    {
        var rig3dAssetAttr = typeof (Rig3DAssetAttribute);

        // Get a list of all classes annotated with AssetTypeAttribute
        var assetTypes =
            from t in rig3dAssetAttr.Assembly.GetTypes()
            let attr = t.GetCustomAttributes(rig3dAssetAttr, false)
            where attr.Any()
            select new {
                Type = t,
                Attr = attr.First() as Rig3DAssetAttribute,
            };

        var json = new JObject();
        foreach (var assetType in assetTypes)
        {
            var objs = Object.FindObjectsOfType(assetType.Type).Cast<MonoBehaviour>();

            var fields = GetExportFields(assetType.Type);
            var properties = GetExportProperties(assetType.Type);

            var array = new JArray();
            foreach (var obj in objs)
            {
                var jobj = new JObject();

                var transform = obj.transform;
                if (!assetType.Attr.IgnorePosition)
                {
                    jobj.Add("position", ToJObject(transform.position));
                }

                if (!assetType.Attr.IgnoreRotation)
                {
                    jobj.Add("rotation", ToJObject(transform.rotation));
                }

                if (!assetType.Attr.IgnoreScale)
                {
                    jobj.Add("scale", ToJObject(transform.lossyScale));
                }
                
                foreach (var fieldPair in fields)
                {
                    var name = fieldPair.Value.Name;
                    if (string.IsNullOrEmpty(name))
                    {
                        name = fieldPair.Key.Name;
                    }

                    var value = fieldPair.Key.GetValue(obj);
                    jobj.Add(name, new JRaw(value));
                }

                foreach (var propPair in properties)
                {
                    var name = propPair.Value.Name;
                    if (string.IsNullOrEmpty(name))
                    {
                        name = propPair.Key.Name;
                    }

                    var value = propPair.Key.GetValue(obj, null);
                    jobj.Add(name, new JRaw(value));
                }

                array.Add(jobj);
            }

            json.Add(assetType.Attr.Name, array);
        }

        var str = json.ToString();
        Debug.Log(str);
    }

    private static IDictionary<PropertyInfo, ExportAttribute> GetExportProperties(Type type)
    {
        var prop =
            from m in type.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
            let attr = m.GetCustomAttributes(typeof(ExportAttribute), false)
            where attr.Any()
            select new {
                Property = m,
                Attr = attr.First() as ExportAttribute,
            };

        return prop.ToDictionary(t => t.Property, t => t.Attr);
    }

    private static IDictionary<FieldInfo, ExportAttribute> GetExportFields(Type type)
    {
        var fields =
            from m in type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
            let attr = m.GetCustomAttributes(typeof(ExportAttribute), false)
            where attr.Any()
            select new {
                Field = m,
                Attr = attr.First() as ExportAttribute,
            };

        return fields.ToDictionary(t => t.Field, t => t.Attr);
    }

    private static JArray ToJObject(Vector3 v)
    {
        return new JArray { v.x, v.y, v.z };
    }

    private static JArray ToJObject(Quaternion q)
    {
        return new JArray { q.x, q.y, q.z, q.w };
    }

}
