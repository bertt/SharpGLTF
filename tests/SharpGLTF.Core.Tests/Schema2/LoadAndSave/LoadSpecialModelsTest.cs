﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

using NUnit.Framework;

namespace SharpGLTF.Schema2.LoadAndSave
{
    /// <summary>
    /// Test cases for models found in <see href="https://github.com/KhronosGroup/glTF-Blender-Exporter"/>
    /// </summary>
    [TestFixture]
    [AttachmentPathFormat("*/TestResults/LoadAndSave/?", true)]
    [Category("Model Load and Save")]
    public class LoadSpecialModelsTest
    {
        #region setup

        [OneTimeSetUp]
        public void Setup()
        {
            // TestFiles.DownloadReferenceModels();
        }

        #endregion

        [Test]
        public void LoadEscapedUriModel()
        {
            var model = ModelRoot.Load(ResourceInfo.From("white space.gltf"));
            Assert.That(model, Is.Not.Null);

            model.AttachToCurrentTest("white space.glb");
        }

        public void LoadWithCustomImageLoader()
        {
            // load Polly model
            var model = ModelRoot.Load(TestFiles.GetPollyFileModelPath());
        }

        [Test(Description = "Example of traversing the visual tree all the way to individual vertices and indices")]
        public void LoadPollyModel()
        {
            // load Polly model
            var model = ModelRoot.Load(TestFiles.GetPollyFileModelPath(), Validation.ValidationMode.TryFix);

            Assert.That(model, Is.Not.Null);

            var triangles = model.DefaultScene
                .EvaluateTriangles<Geometry.VertexTypes.VertexPosition, Geometry.VertexTypes.VertexTexture1>(null, model.LogicalAnimations[0], 0.5f)
                .ToList();

            // Save as GLB, and also evaluate all triangles and save as Wavefront OBJ            
            model.AttachToCurrentTest("polly_out.glb");
            model.AttachToCurrentTest("polly_out.obj");

            // hierarchically browse some elements of the model:

            var scene = model.DefaultScene;

            var pollyNode = scene.FindNode(n => n.Name == "Polly_Display");

            var pollyPrimitive = pollyNode.Mesh.Primitives[0];

            var pollyIndices = pollyPrimitive.GetIndices();
            var pollyPositions = pollyPrimitive.GetVertices<Vector3>("POSITION");
            var pollyNormals = pollyPrimitive.GetVertices<Vector3>("NORMAL");

            for (int i = 0; i < pollyIndices.Count; i += 3)
            {
                var a = (int)pollyIndices[i + 0];
                var b = (int)pollyIndices[i + 1];
                var c = (int)pollyIndices[i + 2];

                var ap = pollyPositions[a];
                var bp = pollyPositions[b];
                var cp = pollyPositions[c];

                var an = pollyNormals[a];
                var bn = pollyNormals[b];
                var cn = pollyNormals[c];

                TestContext.Out.WriteLine($"Triangle {ap} {an} {bp} {bn} {cp} {cn}");
            }

            // create a clone and apply a global axis transform.

            var clonedModel = model.DeepClone();

            var basisTransform
                = Matrix4x4.CreateScale(1, 2, 1)
                * Matrix4x4.CreateFromYawPitchRoll(1, 2, 3)                
                * Matrix4x4.CreateTranslation(10,5,2);

            clonedModel.ApplyBasisTransform(basisTransform);

            clonedModel.AttachToCurrentTest("polly_out_transformed.glb");

            var wsettings = new WriteSettings();
            wsettings.ImageWriting = ResourceWriteMode.BufferView;
            wsettings.MergeBuffers = true;
            wsettings.BuffersMaxSize = 1024 * 1024 * 10;
            clonedModel.AttachToCurrentTest("polly_out_merged_10mb.gltf", wsettings);
        }

        [Test]
        public void LoadUniVRM()
        {
            var path = TestFiles.GetUniVRMModelPath();
            
            var model = ModelRoot.Load(path);
            Assert.That(model, Is.Not.Null);

            var flattenExtensions = model.GatherUsedExtensions().ToArray();

            model.AttachToCurrentTest("AliceModel.glb");
        }

        // [Test]
        public void LoadShrekshaoModel()
        {
            var model = ModelRoot.Load(ResourceInfo.From("SpecialCases\\shrekshao.glb"));
            Assert.That(model, Is.Not.Null);
        }

        [Test]
        public void LoadMouseModel()
        {
            // this model has several nodes with curve animations containing a single animation key,
            // which is causing some problems to the interpolator.            

            var model = ModelRoot.Load(ResourceInfo.From("SpecialCases\\mouse.glb"));

            var boundingSphere = Runtime.MeshDecoder.EvaluateBoundingSphere(model.DefaultScene);

            var sampler = model
                .LogicalNodes[5]
                .GetCurveSamplers(model.LogicalAnimations[1])
                .Rotation
                .CreateCurveSampler(true);

            var node5_R_00 = sampler.GetPoint(0);
            var node5_R_01 = sampler.GetPoint(1);

            Assert.That(node5_R_01, Is.EqualTo(node5_R_00));

            model.AttachToCurrentTest("mouse_00.obj", model.LogicalAnimations[1], 0f);
            model.AttachToCurrentTest("mouse_01.obj", model.LogicalAnimations[1], 1f);
        }

        [TestCase("SketchfabExport-WhatIsPBR.glb")] // model has exported tangents in the form <0,0,0,1>
        public void LoadSketchfabModels(string path)
        {
            // this model has several nodes with curve animations containing a single animation key,
            // which is causing some problems to the interpolator.

            var model = ModelRoot.Load(ResourceInfo.From($"SpecialCases\\{path}"), Validation.ValidationMode.TryFix);

            model.AttachToCurrentTest("output.glb");            
        }

        // these models show normal mapping but lack tangents, which are expected to be
        // generated at runtime; These tests generate the tangents and check them against the baseline.
        [TestCase("NormalTangentTest.glb")]
        [TestCase("NormalTangentMirrorTest.glb")]
        public void LoadGeneratedTangetsTest(string fileName)
        {
            var path = TestFiles.GetSampleModelsPaths().FirstOrDefault(item => item.EndsWith(fileName));

            var model = ModelRoot.Load(path);

            var mesh = model.DefaultScene
                .EvaluateTriangles<Geometry.VertexTypes.VertexPositionNormalTangent, Geometry.VertexTypes.VertexTexture1>()
                .ToMeshBuilder();            

            var editableScene = new Scenes.SceneBuilder();
            editableScene.AddRigidMesh(mesh, Matrix4x4.Identity);

            model.AttachToCurrentTest("original.glb");
            editableScene.ToGltf2().AttachToCurrentTest("WithTangents.glb");
        }


        [Test]
        public void LoadAndSaveToMemory()
        {
            var path = TestFiles.GetSampleModelsPaths().FirstOrDefault(item => item.EndsWith("Avocado.glb"));

            var model = ModelRoot.Load(path);
            // model.LogicalImages[0].TransferToSatelliteFile(); // TODO

            // we will use this dictionary as our in-memory model container.
            var dictionary = new Dictionary<string, ArraySegment<Byte>>();

            // write to dictionary
            var wcontext = WriteContext.CreateFromDictionary(dictionary);
            model.Save("avocado.gltf", wcontext);
            Assert.That(dictionary.ContainsKey("avocado.gltf"), Is.True);
            Assert.That(dictionary.ContainsKey("avocado.bin"), Is.True);

            // read back from dictionary
            var rcontext = ReadContext.CreateFromDictionary(dictionary);
            var model2 = ModelRoot.Load("avocado.gltf", rcontext);
            
            // TODO: verify
            
        }

        [Test]
        public void LoadInvalidModelWithJsonFix()
        {
            // try to load an invalid gltf with an empty array            

            var xpath = ResourceInfo.From("SpecialCases\\Invalid_EmptyArray.gltf");

            Assert.Throws<Validation.SchemaException>(() => ModelRoot.Load(xpath));

            // try to load an invalid gltf with an empty array, using a hook to fix the json before running the parser.

            var rsettings = new ReadSettings();
            rsettings.JsonPreprocessor = _RemoveEmptyArrayJsonProcessor;

            var model = ModelRoot.Load(xpath, rsettings);
            Assert.That(model, Is.Not.Null);

            // save the model, using a hook to modify the json before writing it to the file.

            var wsettings = new WriteSettings();
            wsettings.JsonPostprocessor = json =>
            {
                json = json.Replace("glTF 2.0 Validator test suite", "postprocessed json"); return json;
            };

            var path = model.AttachToCurrentTest("modified.gltf", wsettings);

            model = ModelRoot.Load(path);

            Assert.That("postprocessed json", Is.EqualTo(model.Asset.Generator));
        }


        private string _RemoveEmptyArrayJsonProcessor(string json)
        {
            var obj = Newtonsoft.Json.Linq.JObject.Parse(json);

            var children = obj.Children().ToArray();

            children[1].Remove(); // remove the empty "meshes" array.

            json = obj.ToString();            

            return json;
        }
    }
}
