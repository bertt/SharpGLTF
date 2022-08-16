using SharpGLTF.CodeGen;
using SharpGLTF.SchemaReflection;
using System.Collections.Generic;

namespace SharpGLTF
{
    class ExtMeshFeaturesExtension : SchemaProcessor
    {
        // workaround: we use glTF.EXT_mesh_features1.schema.json, it does not have the OneOff construction
        private static string RootSchemaUri => Constants.VendorExtensionPath("EXT_mesh_features", "glTF.EXT_mesh_features1.schema.json");
        private static string NodeSchemaUri => Constants.VendorExtensionPath("EXT_mesh_features", "node.EXT_mesh_features.schema.json");
        private static string PrimitiveSchemaUri => Constants.VendorExtensionPath("EXT_mesh_features", "primitive.EXT_mesh_features.schema.json");
        private static string MeshSchemaUri => Constants.VendorExtensionPath("EXT_mesh_features", "mesh.primitive.EXT_mesh_features.schema.json");
        public override void PrepareTypes(CSharpEmitter newEmitter, SchemaType.Context ctx)
        {
            newEmitter.SetRuntimeName("EXT_mesh_features glTF extension", "ExtMeshFeatures");
        }

        public override IEnumerable<(string TargetFileName, SchemaType.Context Schema)> Process()
        {
            yield return ("ext.MeshFeaturesRoot.g", ProcessRoot());
            yield return ("ext.MeshFeaturesNode.g", ProcessNode());
            yield return ("ext.MeshFeaturesMesh.g", ProcessMesh());
            yield return ("ext.MeshFeaturesPrimitive.g", ProcessPrimitive());
        }

        private static SchemaType.Context ProcessMesh()
        {
            var ctx = SchemaProcessing.LoadSchemaContext(MeshSchemaUri);
            ctx.IgnoredByCodeEmitter("glTF Property");
            ctx.IgnoredByCodeEmitter("glTF Child of Root Property");
            ctx.IgnoredByCodeEmitter("Texture Info");

            // here something goes wrong with channels...
            var cls = ctx.FindClass("Feature ID Texture in EXT_mesh_features")
               .GetField("channels").SetDefaultValue("new Int32[1] {0 }");
            // manually edit the generated file because List<Int32> -> Int32[]
            return ctx;
        }

        private static SchemaType.Context ProcessPrimitive()
        {
            var ctx = SchemaProcessing.LoadSchemaContext(PrimitiveSchemaUri);
            ctx.IgnoredByCodeEmitter("glTF Property");
            ctx.IgnoredByCodeEmitter("glTF Child of Root Property");
            ctx.IgnoredByCodeEmitter("Texture Info");
            ctx.IgnoredByCodeEmitter("Feature ID Texture in EXT_mesh_features");
            return ctx;
        }

        private static SchemaType.Context ProcessRoot()
        {
            var ctx = SchemaProcessing.LoadSchemaContext(RootSchemaUri);
            ctx.IgnoredByCodeEmitter("glTF Property");
            ctx.IgnoredByCodeEmitter("glTF Child of Root Property");
            ctx.IgnoredByCodeEmitter("Texture Info");
            return ctx;
        }

        private static SchemaType.Context ProcessNode()
        {
            var ctx = SchemaProcessing.LoadSchemaContext(NodeSchemaUri);
            ctx.IgnoredByCodeEmitter("glTF Property");
            ctx.IgnoredByCodeEmitter("glTF Child of Root Property");
            return ctx;
        }
    }
}
