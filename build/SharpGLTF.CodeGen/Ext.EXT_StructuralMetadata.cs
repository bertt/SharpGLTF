using SharpGLTF.CodeGen;
using SharpGLTF.SchemaReflection;
using System.Collections.Generic;

namespace SharpGLTF
{
    class ExtStructuralMetadataExtension : SchemaProcessor
    {
        private static string RootSchemaUri => Constants.VendorExtensionPath("EXT_structural_metadata", "glTF.EXT_structural_metadata.schema.json");
        private static string MeshPrimitiveSchemaUri => Constants.VendorExtensionPath("EXT_structural_metadata", "mesh.primitive.EXT_structural_metadata.schema.json");

        public override void PrepareTypes(CSharpEmitter newEmitter, SchemaType.Context ctx)
        {
            newEmitter.SetRuntimeName("EXT_structural_metadata glTF extension", "EXTStructuralMetaData");
        }

        public override IEnumerable<(string TargetFileName, SchemaType.Context Schema)> Process()
        {
            yield return ("Ext.StructuralMetadataRoot.g", ProcessRoot());
            yield return ("Ext.StructuralMetadataMeshPrimitive.g", ProcessMeshPrimitive());
        }

        private static SchemaType.Context ProcessRoot()
        {
            var ctx = SchemaProcessing.LoadSchemaContext(RootSchemaUri);
            //ctx.IgnoredByCodeEmitter("glTF Property");
            //ctx.IgnoredByCodeEmitter("glTF Child of Root Property");

            return ctx;
        }
        private static SchemaType.Context ProcessMeshPrimitive()
        {
            var ctx = SchemaProcessing.LoadSchemaContext(MeshPrimitiveSchemaUri);
            ctx.IgnoredByCodeEmitter("glTF Property");
            ctx.IgnoredByCodeEmitter("glTF Child of Root Property");

            return ctx;
        }
    }
}
