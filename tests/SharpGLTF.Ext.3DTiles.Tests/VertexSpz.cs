using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

using SharpGLTF.Geometry.VertexTypes;
using SharpGLTF.Memory;
using SharpGLTF.Schema2;

namespace SharpGLTF
{
    [System.Diagnostics.DebuggerDisplay("𝐂:{Color} 𝐔𝐕:{TexCoord}")]
    public struct VertexSpz : IVertexCustom
    {
        public VertexSpz(Vector4 color, Vector3 scale)
        {
            Color = color;
            Scale = scale;
        }

        public const string SCALEATTRIBUTENAME = "_SCALE";

        public Vector4 Color;
        public Vector3 Scale;

        IEnumerable<KeyValuePair<string, AttributeFormat>> IVertexReflection.GetEncodingAttributes()
        {
            yield return new KeyValuePair<string, AttributeFormat>("COLOR_0", new AttributeFormat(DimensionType.VEC4));
            yield return new KeyValuePair<string, AttributeFormat>(SCALEATTRIBUTENAME, new AttributeFormat(DimensionType.VEC3));
        }

        public int MaxColors => 1;

        public int MaxTextCoords => 0;

        public IEnumerable<string> CustomAttributes => throw new NotImplementedException();

        void IVertexMaterial.SetColor(int setIndex, Vector4 color)
        {
            if (setIndex == 0) Color = color;
        }

        public void SetTexCoord(int setIndex, Vector2 coord) { }

        public Vector4 GetColor(int index)
        {
            return Color;
        }

        public Vector2 GetTexCoord(int index) { throw new ArgumentOutOfRangeException(nameof(index)); }

        public void Validate() { }

        public object GetCustomAttribute(string attributeName)
        {
            throw new NotImplementedException();
        }

        public bool TryGetCustomAttribute(string attributeName, out object value)
        {
            if (attributeName == SCALEATTRIBUTENAME)
            {
                value = Scale; return true;
            }
            else
            {
                value = null; return false;
            }
        }

        public void SetCustomAttribute(string attributeName, object value)
        {
            throw new NotImplementedException();
        }

        public VertexMaterialDelta Subtract(IVertexMaterial baseValue)
        {
            throw new NotImplementedException();
        }

        public void Add(in VertexMaterialDelta delta)
        {
            throw new NotImplementedException();
        }
    }
}
