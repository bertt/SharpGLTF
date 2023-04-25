using SharpGLTF.CodeGen;
using SharpGLTF.SchemaReflection;
using System.Collections.Generic;

namespace SharpGLTF
{
    class ExtStructuralMetadataExtension : SchemaProcessor
    {
        private static string RootSchemaUri => Constants.VendorExtensionPath("EXT_structural_metadata", "glTF.EXT_structural_metadata.schema1.json");

        public override void PrepareTypes(CSharpEmitter newEmitter, SchemaType.Context ctx)
        {
            newEmitter.SetRuntimeName("EXT_structural_metadata glTF extension", "EXTStructuralMetaData");
            newEmitter.SetRuntimeName("Property Table in EXT_structural_metadata", "PropertyTable");
            newEmitter.SetRuntimeName("Schema in EXT_structural_metadata", "StructuralMetadataSchema");
            newEmitter.SetRuntimeName("Property Table Property in EXT_structural_metadata", "PropertyTableProperty");
            newEmitter.SetRuntimeName("Property Texture in EXT_structural_metadata", "PropertyTexture");
            newEmitter.SetRuntimeName("Property Texture Property in EXT_structural_metadata", "PropertyTextureProperty");
            newEmitter.SetRuntimeName("Property Attribute Property in EXT_structural_metadata", "PropertyAttributeProperty");
            newEmitter.SetRuntimeName("Class Property in EXT_structural_metadata", "ClassProperty");
            newEmitter.SetRuntimeName("Class in EXT_structural_metadata", "StructuralMetadataClass");
            newEmitter.SetRuntimeName("Enum Value in EXT_structural_metadata", "EnumValue");
            newEmitter.SetRuntimeName("Enum in EXT_structural_metadata", "StructuralMetadataEnum");

            newEmitter.SetRuntimeName("BOOLEAN-ENUM-MAT2-MAT3-MAT4-SCALAR-STRING-VEC2-VEC3-VEC4", "ElementType");
            newEmitter.SetRuntimeName("FLOAT32-FLOAT64-INT16-INT32-INT64-INT8-UINT16-UINT32-UINT64-UINT8", "DataType");
            newEmitter.SetRuntimeName("INT16-INT32-INT64-INT8-UINT16-UINT32-UINT64-UINT8", "IntegerType");
            newEmitter.SetRuntimeName("UINT16-UINT32-UINT64-UINT8", "StringOffsets");

        }

        public override IEnumerable<(string TargetFileName, SchemaType.Context Schema)> Process()
        {
            yield return ("ext.StructuralMetadataRoot.g", ProcessRoot());
        }

        private static SchemaType.Context ProcessRoot()
        {
            var ctx = SchemaProcessing.LoadSchemaContext(RootSchemaUri);
            ctx.IgnoredByCodeEmitter("glTF Property");
            ctx.IgnoredByCodeEmitter("glTF Child of Root Property");
            ctx.IgnoredByCodeEmitter("Texture Info");

    //        ctx.FindClass("Property Table Property in EXT_structural_metadata")
    //.GetField("_arrayOffsetTypeDefault")
    //.SetDataType(typeof(int), false)
    //.SetDefaultValue("2");


            return ctx;
        }
    }
}
