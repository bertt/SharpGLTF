﻿namespace SharpGLTF.Schema2
{
    using Tiles3D;

    /// <summary>
    /// Extension methods for 3DTiles glTF Extensions
    /// </summary>
    public static partial class Tiles3DExtensions
    {
        private static bool _3DTilesRegistered;

        /// <summary>
        /// This method most be called once at application's startup to register the extensions.
        /// </summary>
        public static void RegisterExtensions()
        {
            if (_3DTilesRegistered) return;

            _3DTilesRegistered = true;

            ExtensionsFactory.RegisterExtension<MeshPrimitive, CesiumPrimitiveOutline>("CESIUM_primitive_outline", p=> new CesiumPrimitiveOutline(p));
            ExtensionsFactory.RegisterExtension<Node, MeshExtInstanceFeatures>("EXT_instance_features", p => new MeshExtInstanceFeatures(p));
            ExtensionsFactory.RegisterExtension<MeshPrimitive, MeshExtMeshFeatures>("EXT_mesh_features", p => new MeshExtMeshFeatures(p));
            ExtensionsFactory.RegisterExtension<ModelRoot, EXTStructuralMetadataRoot>("EXT_structural_metadata", p => new EXTStructuralMetadataRoot(p));
            ExtensionsFactory.RegisterExtension<MeshPrimitive, ExtStructuralMetadataMeshPrimitive>("EXT_structural_metadata", p => new ExtStructuralMetadataMeshPrimitive(p));
            ExtensionsFactory.RegisterExtension<MeshPrimitive, SpzGaussianSplatsCompression>("KHR_spz_gaussian_splats_compression", p => new SpzGaussianSplatsCompression(p));
        }
    }
}
