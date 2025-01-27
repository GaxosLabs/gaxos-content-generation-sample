using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

namespace ContentGeneration.Editor.MainWindow.Components.Meshy
{
    public class MeshyModelHelper
    {
        public static async Task Save(JToken result)
        {
            var path = EditorUtility.SaveFolderPanel(
                "Save model location",
                "Assets/",
                "");

            if (path.Length == 0) return;

            var fbx = await DownloadFileAsync(result["model_urls"]!["fbx"]!
                .ToObject<string>());
            await File.WriteAllBytesAsync(Path.Combine(path, "model.fbx"), fbx);

            List<Dictionary<string, string>> textureNames = new();
            var index = 0;
            foreach (var jToken in result["texture_urls"]!)
            {
                textureNames.Add(new Dictionary<string, string>());
                var texturesObject = (JObject)jToken;
                var textureUrls = texturesObject.ToObject<Dictionary<string, string>>();
                foreach (var textureDefinition in textureUrls)
                {
                    var bytes = await DownloadFileAsync(textureDefinition.Value);
                    var textureName = $"{index}_{textureDefinition.Key}.png";
                    textureNames[index].Add(textureDefinition.Key, textureName);
                    await File.WriteAllBytesAsync(Path.Combine(path, textureName), bytes);
                }

                index++;
            }

            if (path.StartsWith(Application.dataPath))
            {
                var localPath =
                    Application.dataPath == path ? "Assets" :
                    Path.Combine("Assets", path[(Application.dataPath.Length + 1)..]);

                AssetDatabase.Refresh();

                var model = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(
                    Path.Combine(localPath, "model.fbx")));
                var modelRenderer = model.GetComponent<Renderer>();
                var materials = new List<Material>();
                for (var i = 0; i < modelRenderer.sharedMaterials.Length; i++)
                {
                    var material = new Material(Shader.Find("Standard (Specular setup)"));

                    if(i< textureNames.Count)
                    {
                        if (textureNames[i].ContainsKey("base_color"))
                        {
                            material.SetTexture("_MainTex",
                                AssetDatabase.LoadAssetAtPath<Texture2D>(Path.Combine(localPath,
                                    textureNames[i]["base_color"])));
                        }

                        if (textureNames[i].ContainsKey("metallic"))
                        {
                            // material.SetTexture("_SpecColor", 
                            //     AssetDatabase.LoadAssetAtPath<Texture2D>(Path.Combine(localPath, textureNames[i]["metallic"])));
                        }

                        if (textureNames[i].ContainsKey("normal"))
                        {
                            var assetPath = Path.Combine(localPath, textureNames[i]["normal"]);
                            var importer = (TextureImporter)AssetImporter.GetAtPath(assetPath);
                            if (importer.textureType != TextureImporterType.NormalMap)
                            {
                                importer.SetTextureSettings(new TextureImporterSettings
                                {
                                    textureType = TextureImporterType.NormalMap
                                });

                                AssetDatabase.Refresh();
                            }

                            material.SetTexture("_BumpMap",
                                AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath));
                        }

                        if (textureNames[i].ContainsKey("roughness"))
                        {
                            material.SetTexture("_SpecGlossMap",
                                AssetDatabase.LoadAssetAtPath<Texture2D>(Path.Combine(localPath,
                                    textureNames[i]["roughness"])));
                        }
                    }

                    var materialPath = Path.Combine(localPath, $"material_{i}.mat");
                    AssetDatabase.CreateAsset(material, materialPath);
                    
                    materials.Add(AssetDatabase.LoadAssetAtPath<Material>(materialPath));
                }

                AssetDatabase.Refresh();
                modelRenderer.SetSharedMaterials(materials);
                
                model.transform.localPosition = Vector3.zero;
                model.transform.localEulerAngles = Vector3.zero;
                model.transform.localScale = Vector3.one;
                
                PrefabUtility.SaveAsPrefabAsset(model, Path.Combine(localPath, "prefab.prefab"));
                Object.DestroyImmediate(model, true);

                AssetDatabase.Refresh();
            }
        }

        public static Task<byte[]> DownloadFileAsync(string url)
        {
            var ret = new TaskCompletionSource<byte[]>();
            EditorCoroutineUtility.StartCoroutine(
                DownloadFileCo(url, ret), url);
            return ret.Task;
        }

        static IEnumerator DownloadFileCo(string url, TaskCompletionSource<byte[]> tcs)
        {
            var www = UnityWebRequest.Get(url);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                tcs.SetException(new Exception($"{www.error}: {www.downloadHandler?.text}"));
                yield break;
            }

            tcs.SetResult(www.downloadHandler.data);
        }
    }
}