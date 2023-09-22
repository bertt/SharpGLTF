using System.Collections.Generic;
using System.Numerics;
using SharpGLTF.Geometry;
using SharpGLTF.Geometry.VertexTypes;
using SharpGLTF.Materials;

namespace SharpGLTF.Scenes
{
    public static class PrimitiveBuilderExtentionMethods
    {
        public static (int, int, int) AddTriangleWithFeatureId(this PrimitiveBuilder<MaterialBuilder, VertexPositionNormal, VertexWithFeatureId, VertexEmpty> prim, (Vector3, Vector3, Vector3) triangle, Vector3 normal, int featureId)
        {
            var vertices = GetVerticesWithFeatureId(triangle, normal, featureId);
            var res = prim.AddTriangle(vertices[0], vertices[1], vertices[2]);
            return res;
        }

        private static List<VertexBuilder<VertexPositionNormal, VertexWithFeatureId, VertexEmpty>> GetVerticesWithFeatureId((Vector3, Vector3, Vector3) triangle, Vector3 normal, int batchid)
        {
            var vb0 = GetVertexBuilderWithFeatureId(triangle.Item1, normal, batchid);
            var vb1 = GetVertexBuilderWithFeatureId(triangle.Item2, normal, batchid);
            var vb2 = GetVertexBuilderWithFeatureId(triangle.Item3, normal, batchid);
            return new List<VertexBuilder<VertexPositionNormal, VertexWithFeatureId, VertexEmpty>>() { vb0, vb1, vb2 };
        }

        private static VertexBuilder<VertexPositionNormal, VertexWithFeatureId, VertexEmpty> GetVertexBuilderWithFeatureId(Vector3 position, Vector3 normal, int featureid)
        {
            var vp0 = new VertexPositionNormal(position, normal);
            var vb0 = new VertexBuilder<VertexPositionNormal, VertexWithFeatureId, VertexEmpty>(vp0, featureid);
            return vb0;
        }
    }
}