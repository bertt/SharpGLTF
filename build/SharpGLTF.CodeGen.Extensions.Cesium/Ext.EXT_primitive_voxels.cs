using SharpGLTF.CodeGen;
using SharpGLTF.SchemaReflection;
using System.Collections.Generic;

namespace SharpGLTF
{
    class ExtPrimitiveVoxelsExtension : SchemaProcessor
    {
        public override string GetTargetProject() { return CesiumExtensions.CesiumProjectDirectory; }

        private static string RootSchemaUri => CesiumExtensions.CustomExtensionsPath("EXT_primitive_voxels", "mesh.primitive.EXT_primitive_voxels.schema.json");

        public override void PrepareTypes(CSharpEmitter newEmitter, SchemaType.Context ctx)
        {
            newEmitter.SetRuntimeName("EXT_primitive_voxels glTF extension", "EXT_primitive_voxels", CesiumExtensions.CesiumNameSpace);
        }

        public override IEnumerable<(string TargetFileName, SchemaType.Context Schema)> ReadSchema()
        {
            yield return ("Ext.CESIUM_ext_primitive_voxels.g", ProcessRoot());
        }

        private static SchemaType.Context ProcessRoot()
        {
            var ctx = SchemaProcessing.LoadExtensionSchemaContext(RootSchemaUri);

            return ctx;
        }
    }
}