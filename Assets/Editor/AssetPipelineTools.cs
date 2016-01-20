using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

[InitializeOnLoad]
public class AssetPipelineTools
{
    [MenuItem("Tools/Export Active Scene")]
    public static void ExportActiveScene()
    {
       var scenePath = EditorApplication.currentScene;

        if (string.IsNullOrEmpty(scenePath))
        {
            Debug.LogWarning("No active scene! Please load a scene before exporting.");
            return;
        }

        var rig3DAssetAttr = typeof (Rig3DAssetAttribute);

        // Get a list of all classes annotated with AssetTypeAttribute
        var assetTypes =
            from t in rig3DAssetAttr.Assembly.GetTypes()
            let attr = t.GetCustomAttributes(rig3DAssetAttr, false)
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
                if (assetType.Attr.ExportPosition)
                {
                    jobj.Add("position", ToJObject(transform.position));
                }

                if (assetType.Attr.ExportRotation)
                {
                    jobj.Add("rotation", ToJObject(transform.rotation));
                }

                if (assetType.Attr.ExportScale)
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

        // extract scene name
        var startIndex = scenePath.LastIndexOfAny(new[] {'/', '\\'}) + 1;
        var endIndex = scenePath.LastIndexOf('.');
        var sceneName = scenePath.Substring(startIndex, endIndex - startIndex);

        var exportPath = Path.GetFullPath(Path.Combine(Application.dataPath, "../Export"));
        var jsonPath = Path.Combine(exportPath, sceneName + ".json");

        Directory.CreateDirectory(exportPath);
        File.WriteAllText(jsonPath, str);

        Debug.LogFormat("JSON asset saved at {0}.", jsonPath);
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
