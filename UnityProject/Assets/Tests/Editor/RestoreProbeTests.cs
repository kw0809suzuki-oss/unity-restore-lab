using System;
using System.IO;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace RestoreLab.Tests
{
    // Every test uses a disposable synthetic asset. No user assets are touched.
    public class RestoreProbeTests
    {
        private const string Root = "Assets/__RestoreLabTest";

        [SetUp]
        public void SetUp()
        {
            if (AssetDatabase.IsValidFolder(Root))
                AssetDatabase.DeleteAsset(Root);
            AssetDatabase.CreateFolder("Assets", "__RestoreLabTest");
        }

        [TearDown]
        public void TearDown()
        {
            AssetDatabase.DeleteAsset(Root);
            AssetDatabase.Refresh();
        }

        [Test]
        public void MaterialGuid_AndAssetExistence_AreVerifiable()
        {
            string path = Root + "/Mat.mat";
            var mat = NewMaterial();
            mat.color = Color.red;
            AssetDatabase.CreateAsset(mat, path);
            AssetDatabase.SaveAssets();

            string guid = AssetDatabase.AssetPathToGUID(path);
            Assert.IsFalse(String.IsNullOrEmpty(guid));
            Assert.AreEqual(path, AssetDatabase.GUIDToAssetPath(guid));
            Assert.IsNotNull(AssetDatabase.LoadAssetAtPath<Material>(path));
        }

        [Test]
        public void SameGuid_DoesNotGuaranteeSameMaterialValue()
        {
            string path = Root + "/Mat.mat";
            var mat = NewMaterial();
            mat.color = Color.red;
            AssetDatabase.CreateAsset(mat, path);
            AssetDatabase.SaveAssets();

            string guid = AssetDatabase.AssetPathToGUID(path);
            var original = AssetDatabase.LoadAssetAtPath<Material>(path).color;

            var saved = AssetDatabase.LoadAssetAtPath<Material>(path);
            saved.color = Color.blue;
            EditorUtility.SetDirty(saved);
            AssetDatabase.SaveAssets();

            Assert.AreEqual(guid, AssetDatabase.AssetPathToGUID(path));
            Assert.AreNotEqual(original, AssetDatabase.LoadAssetAtPath<Material>(path).color);
        }

        [Test]
        public void FullAssetBackup_RestoresChangedMaterialValue()
        {
            string path = Root + "/Mat.mat";
            var mat = NewMaterial();
            mat.color = Color.red;
            AssetDatabase.CreateAsset(mat, path);
            AssetDatabase.SaveAssets();
            string guid = AssetDatabase.AssetPathToGUID(path);
            byte[] original = File.ReadAllBytes(path);
            byte[] originalMeta = File.ReadAllBytes(path + ".meta");

            var changed = AssetDatabase.LoadAssetAtPath<Material>(path);
            changed.color = Color.blue;
            EditorUtility.SetDirty(changed);
            AssetDatabase.SaveAssets();
            Assert.IsFalse(BytesEqual(original, File.ReadAllBytes(path)));

            File.WriteAllBytes(path, original);
            File.WriteAllBytes(path + ".meta", originalMeta);
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);

            Assert.IsTrue(BytesEqual(original, File.ReadAllBytes(path)));
            Assert.AreEqual(guid, AssetDatabase.AssetPathToGUID(path));
            Assert.AreEqual(Color.red, AssetDatabase.LoadAssetAtPath<Material>(path).color);
        }

        [Test]
        public void PrefabVariant_PreservesOverriddenScale()
        {
            string basePath = Root + "/Base.prefab";
            string variantPath = Root + "/Variant.prefab";
            var source = new GameObject("SyntheticAvatar");
            var child = new GameObject("Clothing");
            child.transform.SetParent(source.transform, false);
            child.transform.localScale = Vector3.one;
            var basePrefab = PrefabUtility.SaveAsPrefabAsset(source, basePath);
            UnityEngine.Object.DestroyImmediate(source);

            var instance = (GameObject)PrefabUtility.InstantiatePrefab(basePrefab);
            var clothing = instance.transform.Find("Clothing");
            Assert.IsNotNull(clothing);
            clothing.localScale = new Vector3(0.7f, 0.8f, 0.9f);
            PrefabUtility.RecordPrefabInstancePropertyModifications(clothing);
            PrefabUtility.SaveAsPrefabAsset(instance, variantPath);
            UnityEngine.Object.DestroyImmediate(instance);
            AssetDatabase.SaveAssets();

            var variant = AssetDatabase.LoadAssetAtPath<GameObject>(variantPath);
            Assert.IsNotNull(variant);
            Assert.IsTrue(PrefabUtility.IsPartOfVariantPrefab(variant));
            Assert.AreEqual(new Vector3(0.7f, 0.8f, 0.9f), variant.transform.Find("Clothing").localScale);
        }

        private static Material NewMaterial()
        {
            var shader = Shader.Find("Standard");
            if (shader == null) shader = Shader.Find("Unlit/Color");
            Assert.IsNotNull(shader, "No compatible test shader installed.");
            return new Material(shader);
        }

        private static bool BytesEqual(byte[] a, byte[] b)
        {
            if (a.Length != b.Length) return false;
            for (int i = 0; i < a.Length; ++i)
                if (a[i] != b[i]) return false;
            return true;
        }
    }
}
