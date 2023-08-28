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

            var values = new List<uint>() { 1000, 1001, 1002 };

            var names = new List<string>() { "first", "second", "third" };

            // todo: add method to add list of string
            model.AddStructuralMetadata("objectid1", values);
            // model.AddStructuralMetadataStrings("objectid1", names);

            model.SaveGLB(@"d:\aaa\testhtml\test37.glb");

            var readSettings = new ReadSettings();
            readSettings.Validation = SharpGLTF.Validation.ValidationMode.TryFix;
            var glb = ModelRoot.Load(@"d:\aaa\testhtml\test37.glb");
        }
    }
}