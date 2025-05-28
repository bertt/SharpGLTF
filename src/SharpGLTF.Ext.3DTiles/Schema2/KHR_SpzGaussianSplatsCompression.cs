using SharpGLTF.Schema2.Tiles3D;

namespace SharpGLTF.Schema2
{
    partial class Tiles3DExtensions
    {
        public static void SetSpzGaussianSplatsCompression(this MeshPrimitive primitive, byte[] spz)
        {
            // Todo: Guard that primitive type is point
            // var primitiveType = primitive.;

            Guard.NotNull(primitive, nameof(primitive));
            Guard.NotNull(spz, nameof(spz));

            var ext = primitive.UseExtension<SpzGaussianSplatsCompression>();
            var model = primitive.LogicalParent.LogicalParent;

            var bufferview = model.UseBufferView(spz);
            int logicalIndex = bufferview.LogicalIndex;
            ext.BufferViewIndex = logicalIndex;
        }

        public static byte[] GetSpzGaussianSplatsCompression(this MeshPrimitive primitive)
        {
            Guard.NotNull(primitive, nameof(primitive));
            var ext = primitive.GetExtension<SpzGaussianSplatsCompression>();
            if (ext == null) return null;
            var model = primitive.LogicalParent.LogicalParent;
            var bufferview = model.LogicalBuffers[ext.BufferViewIndex];
            if (bufferview == null) return null;
            return bufferview.Content;
        }
    }
}


namespace SharpGLTF.Schema2.Tiles3D
{
    partial class SpzGaussianSplatsCompression
    {
        private MeshPrimitive meshPrimitive;

        internal SpzGaussianSplatsCompression(MeshPrimitive meshPrimitive)
        {
            this.meshPrimitive = meshPrimitive;
        }

        public int BufferViewIndex
        {
            get => _bufferView;
            set
            {
                _bufferView = value;
            }
        }
    }
}
