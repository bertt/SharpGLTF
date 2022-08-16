using SharpGLTF.Collections;

namespace SharpGLTF.Schema2
{
    // For ModelRoot
    public sealed partial class ExtMeshFeatures
    {
        internal ExtMeshFeatures(ModelRoot modelRoot)
        {
        }
    }

    // For Node
    public sealed partial class EXT_mesh_featuresextensionforEXT_mesh_gpu_instancing : IChildOf<ExtMeshFeatures>
    {
        internal EXT_mesh_featuresextensionforEXT_mesh_gpu_instancing(Node node)
        {
        }
        public int LogicalIndex { get; private set; } = -1;

        public ExtMeshFeatures LogicalParent { get; private set; }

        public void _SetLogicalParent(ExtMeshFeatures parent, int index)
        {
            LogicalParent = parent;
            LogicalIndex = index;
        }
    }

    // For Mesh
    public sealed partial class EXT_mesh_featuresglTFMeshPrimitiveextension
    {
        internal EXT_mesh_featuresglTFMeshPrimitiveextension(Mesh mesh)
        {
        }
    }

    // For Primitive
    public sealed partial class EXT_mesh_featuresglTFPrimitiveextension
    {
        internal EXT_mesh_featuresglTFPrimitiveextension(MeshPrimitive prim) { }

    }
}
