using SharpGLTF.Geometry;
using SharpGLTF.Geometry.VertexTypes;
using SharpGLTF.Materials;
using SharpGLTF.Scenes;
using SharpGLTF.Schema2;
using System.Numerics;

namespace CesiumMetadataSample
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            var material = MaterialBuilder.CreateDefault();

            var mesh = new MeshBuilder<VertexPositionNormal, VertexWithBatchId, VertexEmpty>("mesh");

            var prim = mesh.UsePrimitive(material);
            var vectors = (new Vector3(-10, 0, 0), new Vector3(10, 0, 0), new Vector3(0, 10, 0));
            prim.AddTriangleWithBatchId(vectors, Vector3.UnitX, 0);

            var vectors1 = (new Vector3(10, -10, 0), new Vector3(-10, 0, 0), new Vector3(0, -10, 0));
            prim.AddTriangleWithBatchId(vectors1, Vector3.UnitX, 1);

            var vectors2 = (new Vector3(5, 5, 0), new Vector3(5, 0, 0), new Vector3(10, 5, 0));
            prim.AddTriangleWithBatchId(vectors2, Vector3.UnitX, 2);

            var scene = new SceneBuilder();

            scene.AddRigidMesh(mesh, Matrix4x4.Identity);

            var model = scene.ToGltf2();

            var uints = new List<int>() { 1000, 1001, 1002 };
            var ints = new List<int>() { -1000, -1001, -1002 };
            var names = new List<string>() { "first777", "second777", "third777" };
            var floats = new List<float>() { 1.1000000f, 1.200000f, 1.300000f };

            var ext = model.InitializeMetadataExtension("propertyTable", names.Count);
            model.AddMetadata(ext, "objectid1", names);
            model.AddMetadata(ext, "ints", ints);
            model.AddMetadata(ext, "floats", floats);
            model.AddMetadata(ext, "uints", uints);


             model.SaveGLB(@"d:\aaa\testhtml\test37.glb");

            var readSettings = new ReadSettings();
            readSettings.Validation = SharpGLTF.Validation.ValidationMode.TryFix;
            var glb = ModelRoot.Load(@"d:\aaa\testhtml\test37.glb");
        }
    }
}