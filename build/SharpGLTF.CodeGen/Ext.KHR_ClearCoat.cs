﻿using System;
using System.Collections.Generic;
using System.Text;

using SharpGLTF.SchemaReflection;

namespace SharpGLTF
{
    class ClearCoatExtension : SchemaProcessor
    {
        private static string SchemaUri => Constants.KhronosExtensionPath("KHR_materials_clearcoat", "material.KHR_materials_clearcoat.schema.json");

        private const string ExtensionRootClassName = "KHR_materials_clearcoat glTF Material Extension";

        public override IEnumerable<(string, SchemaType.Context)> Process()
        {
            var ctx = SchemaProcessing.LoadSchemaContext(SchemaUri);
            ctx.IgnoredByCodeEmitter("glTF Property");
            ctx.IgnoredByCodeEmitter("glTF Child of Root Property");
            ctx.IgnoredByCodeEmitter("Texture Info");
            ctx.IgnoredByCodeEmitter("Material Normal Texture Info");

            yield return("ext.ClearCoat.g", ctx);
        }

        public override void PrepareTypes(CodeGen.CSharpEmitter newEmitter, SchemaType.Context ctx)
        {
            newEmitter.SetRuntimeName(ExtensionRootClassName, "MaterialClearCoat");
        }        
    }
}
