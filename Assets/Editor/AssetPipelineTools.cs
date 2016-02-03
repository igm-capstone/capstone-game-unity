using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;
using Application = UnityEngine.Application;
using Component = UnityEngine.Component;
using Object = UnityEngine.Object;

[InitializeOnLoad]
public class AssetPipelineTools
{
    class CollectionData
    {
        public JArray Collection;
        public Bounds Bounds;
    }

    [UnityEditor.MenuItem("Tools/Export Active Scene")]
    public static void ExportActiveScene()
    {
       var scenePath = EditorApplication.currentScene;

        if (string.IsNullOrEmpty(scenePath))
        {
            Debug.LogWarning("No active scene! Please load a scene before exporting.");
            return;
        }

        var str = ExportScene(scenePath);

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

    [UnityEditor.MenuItem("Tools/Export Active Scene As...")]
    public static void ExportActiveSceneAs()
    {
        var scenePath = EditorApplication.currentScene;

        if (string.IsNullOrEmpty(scenePath))
        {
            Debug.LogWarning("No active scene! Please load a scene before exporting.");
            return;
        }
        
        var str = ExportScene(scenePath);
        
        // extract scene name
        var startIndex = scenePath.LastIndexOfAny(new[] { '/', '\\' }) + 1;
        var endIndex = scenePath.LastIndexOf('.');
        var sceneName = scenePath.Substring(startIndex, endIndex - startIndex);

        var defaultDir = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
        var initialDir = EditorPrefs.GetString("jsonSaveDir", defaultDir);
        var dialog = new SaveFileDialog {
            FileName = sceneName + ".json",
            InitialDirectory = initialDir,
            Filter = "JSON Asset (*.json)|*.json",
        };

        var result = dialog.ShowDialog();

        if (result != DialogResult.OK)
        {
            return;
        }

        var dir = Path.GetDirectoryName(dialog.FileName) ?? defaultDir;
        EditorPrefs.SetString("jsonSaveDir", dir);

        Directory.CreateDirectory(dir);

        File.WriteAllText(dialog.FileName, str);
        Debug.LogFormat("JSON asset saved at {0}.", dialog.FileName);
    }

    private static string ExportScene(string scenePath)
    {
        var rig3DAssetAttr = typeof(Rig3DAssetAttribute);

        // Get a list of all classes annotated with AssetTypeAttribute
        var assetTypes =
            from t in rig3DAssetAttr.Assembly.GetTypes()
            let attr = t.GetCustomAttributes(rig3DAssetAttr, false)
            where attr.Any()
            select new
            {
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
        Bounds? bounds = null;
        var collections = Object.FindObjectsOfType<Rig3DCollection>();
        foreach (var collection in collections)
        {
            var data = GetCollectionArray(collection);

            json.Add(collection.CollectionName, data.Collection);

            if (collection.CalculateBounds)
            {
                if (!bounds.HasValue)
                {
                    bounds = data.Bounds;
                }
                else
                {
                    bounds.Value.Encapsulate(data.Bounds);
                }
            }
        }
        // add level general metadata

        var metadata = new JObject();

        if (bounds.HasValue)
        {
            metadata.Add("bounds", ToJToken(bounds.Value));
        }

        json.AddFirst(new JProperty("metadata", metadata));

        return json.ToString();
    }

    private static CollectionData GetCollectionArray(Rig3DCollection collection)
    {
        var objs = FlattenChildren(collection.transform);
        var bounds = new Bounds();

        var array = new JArray();
        foreach (var behaviour in objs)
        {
            var jobj = CreateJsonObject(behaviour, collection.Exports);

            if (collection.CalculateBounds)
            {
                foreach (var renderer in behaviour.GetComponentsInChildren<Renderer>())
                {
                    bounds.Encapsulate(renderer.bounds);
                }
            }

            array.Add(jobj);
        }
        return new CollectionData {
            Collection = array,
            Bounds = bounds,
        };
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
            jobj.Add("position", ToJToken(transform.position));
        }

        if (ExportContainsProperty(defaultExports, Rig3DExports.Rotation))
        {
            jobj.Add("rotation", ToJToken(transform.rotation));
        }

        if (ExportContainsProperty(defaultExports, Rig3DExports.Scale))
        {
            jobj.Add("scale", ToJToken(transform.lossyScale));
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



    private static JToken ToJToken(Bounds b)
    {
        return new JObject {
            new JProperty("center", ToJToken(b.center)),
            new JProperty("extents", ToJToken(b.extents)),
        };
    }

    private static JToken ToJToken(Vector3 v)
    {
        // Using JRaw to aggregate all vector values in 1 line in the generated json string.
        return new JRaw(string.Format("[ {0:F4}, {1:F4}, {2:F4} ]", v.x, v.y, v.z));
        // return new JArray { v.x, v.y, v.z };
    }

    private static JToken ToJToken(Quaternion q)
    {
        // Using JRaw to aggregate all vector values in 1 line in the generated json string.
        return new JRaw(string.Format("[ {0:F4}, {1:F4}, {2:F4}, {3:F4} ]", q.x, q.y, q.z, q.w));
        //return new JArray { q.x, q.y, q.z, q.w };
    }
    private static bool ExportContainsProperty(Rig3DExports defaultExports, Rig3DExports exports)
    {
        return (defaultExports & exports) == exports;
    }
}
