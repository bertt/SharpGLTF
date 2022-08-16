# EXT_Mesh_Features branch

Specs: https://github.com/CesiumGS/glTF/blob/proposal-EXT_mesh_features/extensions/2.0/Vendor/EXT_mesh_features/README.md

--------------------------------------------------------------------------------------------------------------------------

Code generation

In this step the JSON schema's are converted to C# code

1] Copy schema

Copy from: 

https://github.com/CesiumGS/glTF/tree/proposal-EXT_mesh_features/extensions/2.0/Vendor/EXT_mesh_features/schema

To:

\build\SharpGLTF.CodeGen\bin\Debug\net6.0\glTF\extensions\2.0\Vendor\EXT_mesh_features\schema\

WorkAround in file glTF.EXT_mesh_features1.schema.json: Construction 'OneOff' is removed because problems when 
generating code.

2] Program.cs

Add a processor

```
    processors.Add(new ExtMeshFeaturesExtension());
```

3] Add SchemaProcessor

see Ext.EXT_MeshFeatures.cs

4] MainSchemaProcessor -> add Enum types

```
// EXT_mesh_features types
newEmitter.SetRuntimeName("ARRAY-MAT2-MAT3-MAT4-SINGLE-VEC2-VEC3-VEC4", "ClassPropertyType");
newEmitter.SetRuntimeName("BOOLEAN-ENUM-FLOAT32-FLOAT64-INT16-INT32-INT64-INT8-STRING-UINT16-UINT32-UINT64-UINT8", "ElementComponentType");
newEmitter.SetRuntimeName("INT16-INT32-INT64-INT8-UINT16-UINT32-UINT64-UINT8", "IntegerType");
newEmitter.SetRuntimeName("UINT16-UINT32-UINT64-UINT8", "OffsetBufferViewType");
```

5] Generate code

Code will be generated in folder src\SharpGLTF.Core\Schema2\Generated

There is 1 manual item after generating code: 

File: ext.MeshFeaturesMesh.g.csFile:  

From:

```
private List<Int32> _channels = _channelsDefault;
```

To:

```
private Int32[] _channels = _channelsDefault;
```

--------------------------------------------------------------------------------------------------------------------------

Using Extensions

1] Register

Register extension in gltf.ExtensionsFactory.cs:

```
// EXT_Mesh_Features
RegisterExtension<ModelRoot, ExtMeshFeatures>("EXT_MeshFeatures");
RegisterExtension<Node, EXT_mesh_featuresextensionforEXT_mesh_gpu_instancing>("EXT_MeshFeatures");
RegisterExtension<Mesh, EXT_mesh_featuresglTFMeshPrimitiveextension>("EXT_MeshFeatures");
RegisterExtension<MeshPrimitive, EXT_mesh_featuresglTFPrimitiveextension>("EXT_MeshFeatures");
```

2] ExtMeshFeatures.cs

Add ExtMeshFeatures.cs in Schema2 folder

3] Use extension in Example1 code

```
// EXT_Mesh_Features
model.UseExtension<ExtMeshFeatures>();
model.UseExtension<EXT_mesh_featuresextensionforEXT_mesh_gpu_instancing>();
model.UseExtension<EXT_mesh_featuresglTFMeshPrimitiveextension>();
model.UseExtension<EXT_mesh_featuresglTFPrimitiveextension>();
```

Current issue: Runtime exception on UseExtension second extension
