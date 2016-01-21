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

        // Add annotated types
        var json = new JObject();
        foreach (var assetTypeInfo in assetTypes)
        {
            var array = GetAssetArray(assetTypeInfo.Type, assetTypeInfo.Attr.Exports);
            json.Add(assetTypeInfo.Attr.Name, array);
        }

        // Add transform collections
        var collections = Object.FindObjectsOfType<Rig3DCollection>();
        foreach (var collection in collections)
        {
            var array = GetCollectionArray(collection, collection.Exports);
            json.Add(collection.CollectionName, array);
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

    private static JArray GetCollectionArray(Rig3DCollection collection, Rig3DExports defaultExports)
    {
        var objs = FlattenChildren(collection.transform);

        var array = new JArray();
        foreach (var behaviour in objs)
        {
            var jobj = CreateJsonObject(behaviour, defaultExports);

            array.Add(jobj);
        }
        return array;
    }

    private static JArray GetAssetArray(Type assetType, Rig3DExports defaultExports)
    {
        var objs = Object.FindObjectsOfType(assetType).Cast<MonoBehaviour>();

        var fields = GetExportFields(assetType);
        var properties = GetExportProperties(assetType);

        var array = new JArray();
        foreach (var behaviour in objs)
        {
            var jobj = CreateJsonObject(behaviour, defaultExports);

            foreach (var fieldPair in fields)
            {
                var name = fieldPair.Value.Name;
                if (string.IsNullOrEmpty(name))
                {
                    name = fieldPair.Key.Name;
                }

                var value = fieldPair.Key.GetValue(behaviour);
                jobj.Add(name, new JValue(value));
            }

            foreach (var propPair in properties)
            {
                var name = propPair.Value.Name;
                if (string.IsNullOrEmpty(name))
                {
                    name = propPair.Key.Name;
                }

                var value = propPair.Key.GetValue(behaviour, null);
                jobj.Add(name, new JValue(value));
            }

            array.Add(jobj);
        }
        return array;
    }

    private static IEnumerable<Transform> FlattenChildren(Transform transform)
    {
        if (transform.childCount == 0)
        {
            yield return transform;
        }

        var children = transform.Cast<Transform>();
        var flatten = (Func<Transform, IEnumerable<Transform>>) FlattenChildren;

        // select many receive 
        foreach (var child in children.SelectMany(flatten))
        {
            yield return child;
        }
    }


    private static JObject CreateJsonObject(Component component, Rig3DExports defaultExports)
    {
        var jobj = new JObject();

        var transform = component.transform;
        if (ExportContainsProperty(defaultExports, Rig3DExports.Position))
        {
            jobj.Add("position", ToJObject(transform.position));
        }

        if (ExportContainsProperty(defaultExports, Rig3DExports.Rotation))
        {
            jobj.Add("rotation", ToJObject(transform.rotation));
        }

        if (ExportContainsProperty(defaultExports, Rig3DExports.Scale))
        {
            jobj.Add("scale", ToJObject(transform.lossyScale));
        }

        return jobj;
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

    private static JToken ToJObject(Vector3 v)
    {
        // Using JRaw to aggregate all vector values in 1 line in the generated json string.
        return new JRaw(string.Format("[ {0:0.0}, {1:0.0}, {2:0.0} ]", v.x, v.y, v.z));
        // return new JArray { v.x, v.y, v.z };
    }

    private static JToken ToJObject(Quaternion q)
    {
        // Using JRaw to aggregate all vector values in 1 line in the generated json string.
        return new JRaw(string.Format("[ {0:0.0}, {1:0.0}, {2:0.0}, {3:0.0} ]", q.x, q.y, q.z, q.w));
        //return new JArray { q.x, q.y, q.z, q.w };
    }
    private static bool ExportContainsProperty(Rig3DExports defaultExports, Rig3DExports exports)
    {
        return (defaultExports & exports) == exports;
    }
}
