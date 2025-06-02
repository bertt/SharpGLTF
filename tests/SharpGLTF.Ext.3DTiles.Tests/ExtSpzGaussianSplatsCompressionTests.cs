
using NUnit.Framework;

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

            var settings = new ReadSettings();
            // todo: check validation mode
            settings.Validation = Validation.ValidationMode.Skip;
            var model = ModelRoot.Load(fileName, settings);
            Assert.That(model.LogicalMeshes.Count, Is.EqualTo(1));
            Assert.That(model.LogicalMeshes[0].Primitives.Count, Is.EqualTo(1));
            var primitive = model.LogicalMeshes[0].Primitives[0];
            var bytes = primitive.GetSpzGaussianSplatsCompression();
            Assert.That(bytes, Is.Not.Null);
            Assert.That(bytes.Length, Is.EqualTo(1512464));
        }
    }
}
