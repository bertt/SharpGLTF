using SharpGLTF.CodeGen;
using SharpGLTF.SchemaReflection;
using System.Collections.Generic;

namespace SharpGLTF
{
    internal class KHRSpzGaussianSplatsCompressionExtension : SchemaProcessor
    {
        public override string GetTargetProject() { return CesiumExtensions.CesiumProjectDirectory; }

        private static string NodeSchemaUri => CesiumExtensions.CustomExtensionsPath("KHR_spz_gaussian_splats_compression", "mesh.primitive.KHR_spz_gaussian_splats_compression.schema.json");

        public override void PrepareTypes(CSharpEmitter newEmitter, SchemaType.Context ctx)
        {
            newEmitter.SetRuntimeName("KHR_spz_gaussian_splats_compression glTF Mesh Primitive Extension", "SpzGaussianSplatsCompression", CesiumExtensions.CesiumNameSpace);
        }

        public override IEnumerable<(string TargetFileName, SchemaType.Context Schema)> ReadSchema()
        {
            yield return ("Ext.KHR_spz_gaussian_splats_compression.g", ProcessNode());
        }

        private static SchemaType.Context ProcessNode()
        {
            var ctx = SchemaProcessing.LoadExtensionSchemaContext(NodeSchemaUri);
            return ctx;
        }

    }
}
