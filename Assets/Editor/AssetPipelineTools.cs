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

    class MeshData
    {
        public string Path;
        public string Name;
        public Mesh Mesh;
        public JArray Instances;
    }

    [UnityEditor.MenuItem("Tools/Export Game Properties")]
    public static void ExportGameProperties()
    {
        var explorerGUIDs = new [] {
            "da50434930d53a547b16faf5a7e77d93",
            "e18db561f2541d04aa973e3ca745c5b4",
            "ddfaaa28f02042641864fa1a59641e7e",
        };

        var ghostGUID = "019c493b42c53a1468dbd861bbbd1d49";

        var json = new JObject();

        // --- GHOST

        var ghostJson = new JObject();

        var ghost = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(ghostGUID));
        var skillBar = ghost.GetComponent<SkillBar>();
        var ghostSkills = ghost.GetComponents<ISkill>();
        var strRegenEnergy = skillBar.restoreAmount.Select(val => val.ToString()).ToArray();
        ghostJson.Add("regenEnergy", new JRaw("[ " + string.Join(", ", strRegenEnergy) + " ]"));
        ghostJson.Add("regenEnergyTick", skillBar.waitTimeRestoreEnergy);
        ghostJson.Add("baseEnergy", skillBar.totalEnergy);
        ghostJson.Add("skills", ParseSkills(ghostSkills));

        json.Add("ghost", ghostJson);

        // --- EXPLORERS

        var explorerArray = new JArray();
        foreach (var guid in explorerGUIDs)
        {
            var explorerJson = new JObject();

            var explorer = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid));
            var controller = explorer.GetComponent<AvatarController>();
            var health = explorer.GetComponent<Health>();
            var skills = explorer.GetComponents<ISkill>();

            explorerJson.Add("name", controller.name);
            explorerJson.Add("moveSpeed", new JValue(controller.MoveSpeed));
            explorerJson.Add("baseHealth", new JValue(health.BaseHealth));
            explorerJson.Add("skills", ParseSkills(skills));

            explorerArray.Add(explorerJson);
        }
        json.Add("explorers", explorerArray);

        var defaultDir = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
        var initialDir = EditorPrefs.GetString("jsonSaveDir", defaultDir);
        var dialog = new SaveFileDialog
        {
            FileName = "config.json",
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

        File.WriteAllText(dialog.FileName, json.ToString());
        Debug.LogFormat("JSON asset saved at {0}.", dialog.FileName);
    }

    public static JArray ParseSkills(IEnumerable<ISkill> skills)
    {
        var skillArray = new JArray();
        foreach (var skill in skills)
        {
            var skillJson = new JObject();
            skillArray.Add(skillJson);

            skillJson.Add("name", skill.GetType().Name);
            skillJson.Add("description", skill.Name);
            skillJson.Add("cooldown", skill.Cooldown);
            skillJson.Add("cost", skill.cost);

            var lantern = skill as SetRegLantern;
            if (lantern)
            {
                skillJson.Add("charges", skill.UseCount);
                continue;
            }

            var grenadeToss = skill as GrenadeToss;
            if (grenadeToss)
            {
                skillJson.Add("damage", grenadeToss.Damage);
                skillJson.Add("aoe", grenadeToss.ExploRadius);
                skillJson.Add("maxRange", grenadeToss.ThrowDistance);
                skillJson.Add("knockback", grenadeToss.KnockBackMag);
                continue;
            }

            // review and expose trap params
            var glueTrap = skill as SetTrapGlue;
            if (glueTrap)
            {
                skillJson.Add("duration", 0);
                skillJson.Add("speedMultiplier", 0);
                continue;
            }

            var poisonTrap = skill as SetTrapPoison;
            if (poisonTrap)
            {
                skillJson.Add("duration", 0);
                skillJson.Add("damageOverTime", 0);
                continue;
            }

            var longAttack = skill as LongAttack;
            if (longAttack)
            {
                skillJson.Add("damage", longAttack.Damage);
                skillJson.Add("knockback", longAttack.KnockBackMag);
                continue;
            }

            var heal = skill as Heal;
            if (heal)
            {
                skillJson.Add("heal", heal.HealAmount);
                skillJson.Add("aoe", heal.AreaRadius);
                continue;
            }

            var coneAttack = skill as ConeAttack;
            if (coneAttack)
            {
                skillJson.Add("damage", coneAttack.Damage);
                skillJson.Add("knockback", coneAttack.KnockBackMag);
                continue;
            }

            var sprint = skill as Sprint;
            if (sprint)
            {
                skillJson.Add("duration", sprint.Duration);
                skillJson.Add("speedMultiplier", sprint.SpeedMultiplier);
                continue;
            }

            var spawnImp = skill as SpawnMinion;
            if (spawnImp)
            {
                skillJson.Add("spawnRadius", spawnImp.radius);
                skillJson.Add("spawnCount", spawnImp.spawnCount);
                continue;
            }

            //var spawnAbomination = skill as SpawnAOE;
            //if (spawnAbomination)
            //{
            //    continue;
            //}

            //var spawnFlytrap = skill as SpawnPlant;
            //if (spawnFlytrap)
            //{
            //    continue;
            //}
            
            var transmogrify = skill as Haunt_ExpToMinion;
            if (transmogrify)
            {
                skillJson.Add("duration", transmogrify.HauntDuration);
                continue;
            }
        }

        return skillArray;
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

        var assetMap = new Dictionary<Mesh, MeshData>();
        FindMeshes(ref assetMap);

        var json = new JObject();
        var counters = new JObject();

        // Add annotated types
        foreach (var assetTypeInfo in assetTypes)
        {
            var array = GetAssetArray(assetTypeInfo.Type, assetTypeInfo.Attr.Exports);
            json.Add(assetTypeInfo.Attr.Name, array);
            counters.Add(assetTypeInfo.Attr.Name, array.Count);    
        }

        // Add transform collections
        Bounds? bounds = null;
        var collections = Object.FindObjectsOfType<Rig3DCollection>();
        foreach (var collection in collections)
        {
            // special case for static meshes!!!
            if (collection.IsStaticMesh)
            {
                AddCollectionToMeshArray(collection, ref assetMap);
                continue;
            }

            var data = GetCollectionArray(collection);
            json.Add(collection.CollectionName, data.Collection);
            counters.Add(collection.CollectionName, data.Collection.Count);

            if (collection.CalculateBounds)
            {
                if (!bounds.HasValue)
                {
                    bounds = data.Bounds;
                }
                else
                {
                    var b = bounds.Value;
                    b.Encapsulate(data.Bounds);
                    bounds = b;
                }
            }
        }

        // add static mesh groups to the json
        var staticMeshes = new JObject();
        var staticMeshCounter = 0;
        foreach (var meshData in assetMap.Values)
        {
            if (!meshData.Instances.Any())
            {
                continue;
            }

            staticMeshes.Add(meshData.Name, meshData.Instances);
            staticMeshCounter += meshData.Instances.Count;
        }

        json.Add("staticMeshes", staticMeshes);
        counters.Add("staticMeshes", staticMeshCounter);
        
        // add level general metadata
        var metadata = new JObject();

        if (bounds.HasValue)
        {
            var center = bounds.Value.center;
            var size = bounds.Value.size;

            center.z = 0;
            size.z = 1;
            
            metadata.Add("bounds", ToJToken(new Bounds(center, size)));

            var dbg = GameObject.Find("__Debug") ?? new GameObject("__Debug");
            var debugBounds = dbg.GetComponent<DebugBounds>() ?? dbg.AddComponent<DebugBounds>();

            debugBounds.bounds.center = center;
            debugBounds.bounds.size = size;
        }

        metadata.Add("count", counters);


        json.AddFirst(new JProperty("metadata", metadata));

        return json.ToString();
    }

    private static void FindMeshes(ref Dictionary<Mesh, MeshData> map)
    {
        var guids = AssetDatabase.FindAssets("", new[] { "Assets/Prototyping/Models", "Assets/3D Models" });
        foreach (var t in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(t);
            var mesh = AssetDatabase.LoadAssetAtPath<MeshFilter>(path);

            if (mesh == null)
            {
                continue;
            }

            var meshData = new MeshData {
                Mesh = mesh.sharedMesh,
                Path = path,
                Name = Path.GetFileNameWithoutExtension(path),
                Instances = new JArray()
            };

            if (map.ContainsKey(mesh.sharedMesh))
            {
                var p = map[mesh.sharedMesh];
                Debug.Log("duplicate " + (p.Mesh == meshData.Mesh));
                continue;
            }

            Debug.LogFormat("Found mesh: {0} ({1})", path, mesh.sharedMesh);
            map.Add(mesh.sharedMesh, meshData);
        }
    }

    private static CollectionData GetCollectionArray(Rig3DCollection collection)
    {
        var objs = FlattenChildren(collection.transform);
        var bounds = new Bounds();

        var array = new JArray();
        foreach (var behaviour in objs)
        {
            var jobj = CreateJsonObject(behaviour, collection.Exports, collection.NormalizeDepth);

            if (collection.CalculateBounds)
            {
                foreach (var renderer in behaviour.GetComponentsInChildren<Renderer>())
                {
                    bounds.Encapsulate(renderer.bounds);
                }
            }

            if (collection.CullingLayer >= 0)
            {
                jobj.Add("cullingLayer", collection.CullingLayer);
            }

            array.Add(jobj);
        }
        return new CollectionData {
            Collection = array,
            Bounds = bounds,
        };
    }

    private static void AddCollectionToMeshArray(Rig3DCollection collection, ref Dictionary<Mesh, MeshData> map)
    {
        var objs = FlattenChildren(collection.transform);
        foreach (var behaviour in objs)
        {
            var jobj = CreateJsonObject(behaviour.parent, collection.Exports, collection.NormalizeDepth);
            
            var mesh = behaviour.GetComponentInChildren<MeshFilter>();
            if (mesh == null)
            {
                Debug.LogWarningFormat("Object {0} was marked as static mssh but did not contain a mesh filter component", behaviour);
                continue;
            }

            MeshData meshData;
            map.TryGetValue(mesh.sharedMesh, out meshData);
            if (meshData == null)
            {
                Debug.LogWarningFormat("Could not find mesh data for objcet {0} (mesh: {1})", behaviour, mesh.sharedMesh.name);
                continue;
            }

            if (collection.CullingLayer >= 0)
            {
                jobj.Add("cullingLayer", collection.CullingLayer);
            }

            if (collection.GetTexture)
            {
                var renderer = behaviour.GetComponentInChildren<MeshRenderer>();
                var texture = renderer.sharedMaterial.GetTexture("_MainTex");

                var path = AssetDatabase.GetAssetPath(texture);
                jobj.Add("textureName", Path.GetFileName(path));
            }

            meshData.Instances.Add(jobj);
        }
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
                if (value is Bounds)
                {
                    jobj.Add(name, ToJToken((Bounds)value));
                }
                else
                {
                    jobj.Add(name, new JValue(value));
                }
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


    private static JObject CreateJsonObject(Component component, Rig3DExports defaultExports, bool normalizeDepth = false)
    {
        var jobj = new JObject();

        var transform = component.transform;
        if (ExportContainsProperty(defaultExports, Rig3DExports.Position))
        {
            var pos = transform.position;
            if (normalizeDepth)
            {
                pos.z = 0;
            }
            jobj.Add("position", ToJToken(pos));
        }

        if (ExportContainsProperty(defaultExports, Rig3DExports.Rotation))
        {
            jobj.Add("rotation", ToJToken(transform.rotation));
        }

        if (ExportContainsProperty(defaultExports, Rig3DExports.Scale))
        {
            var skl = transform.lossyScale;
            if (normalizeDepth)
            {
                skl.z = 1;
            }
            jobj.Add("scale", ToJToken(skl));
        }

        if (ExportContainsProperty(defaultExports, Rig3DExports.Layer))
        {
            jobj.Add("layer", transform.gameObject.layer);
        }

        if (ExportContainsProperty(defaultExports, Rig3DExports.Tag))
        {
            jobj.Add("tag", transform.gameObject.tag);
        }

        if (ExportContainsProperty(defaultExports, Rig3DExports.Mesh))
        {
            var mesh = transform.GetComponentInChildren<MeshFilter>();
            if (mesh)
            { 
                var meshtrans = mesh.transform;
                Bounds bounds = new Bounds(meshtrans.TransformPoint(mesh.sharedMesh.bounds.min), Vector3.zero);
                bounds.Encapsulate(meshtrans.TransformPoint(mesh.sharedMesh.bounds.max));
                jobj.Add("bounds", ToJToken(bounds));
            }
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
