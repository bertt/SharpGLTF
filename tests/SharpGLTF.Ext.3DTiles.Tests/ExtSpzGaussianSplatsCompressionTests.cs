
using NUnit.Framework;
using Plotly;
using SharpGLTF.Geometry;
using SharpGLTF.Geometry.VertexTypes;
using SharpGLTF.Materials;
using Spz.NET;
using Spz.NET.Serialization;
using System.IO;
using System.Linq;
using System.Numerics;

namespace SharpGLTF.Schema2.Tiles3D
{
    [Category("Cesium")]
    public partial class ExtSpzGaussianSplatsCompressionTests
    {
        [SetUp]
        public void SetUp()
        {
            Tiles3DExtensions.RegisterExtensions();
        }

        [Test(Description = "Read tower.glb with SPZ Gaussian Splats Compression")]
        public void ReadTowerGlbWithSpzGaussianSplatsCompression()
        {
            var fileName = ResourceInfo.From($"spzgaussiansplatscompression/tower.glb");

            var model = ModelRoot.Load(fileName);
            Assert.That(model.LogicalMeshes.Count, Is.EqualTo(1));
            Assert.That(model.LogicalMeshes[0].Primitives.Count, Is.EqualTo(1));
            var primitive = model.LogicalMeshes[0].Primitives[0];
            var bytes = primitive.GetSpzGaussianSplatsCompression();
            Assert.That(bytes, Is.Not.Null);
            Assert.That(bytes.Length, Is.EqualTo(1512464));

            // query the points

            var vcounts = primitive.VertexAccessors.Values
                .Select(item => item.Count)
                .Distinct()
                .ToList();

            var verticesCount = vcounts[0];
            Assert.That(verticesCount, Is.EqualTo(73172), "Expected 73172 vertices in the primitive.");
        }

        [Test(Description = "Write tower.glb with SPZ Gaussian Splats Compression")]
        public void WriteTowerGlbWithSpzGaussianSplatsCompression()
        {
            Tiles3DExtensions.RegisterExtensions();

            var spz = ResourceInfo.From($"spzgaussiansplatscompression/tower.spz");
            var material = new MaterialBuilder("material1").WithUnlitShader();

            var mesh = VertexBuilder<VertexPosition, VertexSpz, VertexEmpty>.CreateCompatibleMesh("points");
            var pointCloud = mesh.UsePrimitive(material, 1);
            var scene = new Scenes.SceneBuilder();

            scene.AddRigidMesh(mesh, Matrix4x4.Identity);

            var splat = SplatSerializer.FromSpz(spz);
            foreach(var point in splat)
            {
                var position = new Vector3(point.Position.X, point.Position.Y, point.Position.Z);
                var color = new Vector4(point.Color.X, point.Color.Y, point.Color.Z, 0);
                var scale = new Vector3(point.Scale.X, point.Scale.Y, point.Scale.Z);

                var vertexPointSpz = new VertexSpz(color, scale);
                var vp0 = new VertexPosition(position);
                var vb0 = new VertexBuilder<VertexPosition, VertexSpz, VertexEmpty>(vp0, vertexPointSpz);
                pointCloud.AddPoint(vb0);
            }

            var model = scene.ToGltf2();
            var spzBytes = File.ReadAllBytes(spz);

            var primitives = model.LogicalMeshes[0].Primitives;
            foreach(var primitive in primitives)
            {
                primitive.SetSpzGaussianSplatsCompression(spzBytes);
            }

            model.SaveAsWavefront("mesh.obj");
            model.SaveGLB(@"mesh.glb");
            model.SaveGLTF("mesh.gltf");
        }
    }
}
