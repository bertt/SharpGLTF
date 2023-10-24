﻿using System;
using System.Collections.Generic;

using SharpGLTF.SchemaReflection;

namespace SharpGLTF
{
    class EmissiveStrengthExtension : SchemaProcessor
    {
        private static string SchemaUri => Constants.KhronosExtensionPath("KHR_materials_emissive_strength", "material.KHR_materials_emissive_strength.schema.json");

        private const string ExtensionRootClassName = "KHR_materials_emissive_strength glTF Material Extension";

        public override IEnumerable<(string, SchemaType.Context)> Process()
        {
            var ctx = SchemaProcessing.LoadSchemaContext(SchemaUri);
            ctx.IgnoredByCodeEmitter("glTF Property");
            ctx.IgnoredByCodeEmitter("glTF Child of Root Property");
            ctx.IgnoredByCodeEmitter("Texture Info");

            var cls = ctx.FindClass(ExtensionRootClassName);
            
            // straightforward extension, nothing to do

            yield return ("ext.MaterialsEmissiveStrength.g", ctx);
        }

        public override void PrepareTypes(CodeGen.CSharpEmitter newEmitter, SchemaType.Context ctx)
        {
            newEmitter.SetRuntimeName(ExtensionRootClassName, "MaterialEmissiveStrength");
        }
    }
}
