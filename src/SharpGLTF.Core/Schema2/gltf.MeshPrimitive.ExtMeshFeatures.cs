using System.Collections.Generic;

namespace SharpGLTF.Schema2
{
    partial class MeshPrimitive
    {

        public void SetFeatureIds(List<FeatureID> list)
        {
            var ext = UseExtension<MeshExtMeshFeatures>();
            ext.FeatureIds=list;
        }
    }

    public partial class MeshExtMeshFeatures
    {
        public MeshExtMeshFeatures()
        {
        }

        private MeshPrimitive meshPrimitive;
        internal MeshExtMeshFeatures(MeshPrimitive meshPrimitive)
        {
            this.meshPrimitive = meshPrimitive;
            _featureIds = new List<FeatureID>();
        }


        public List<FeatureID> FeatureIds
        {
            get
            {
                return _featureIds;
            }
            set
            {
                _featureIds = value;
            }
        }
    }

    public partial class FeatureID
    {
        public FeatureID()
        {

        }
        public FeatureID(int featureCount, int? attribute=null, int? propertyTable = null, string label=null)
        {
            _featureCount = featureCount;
            _attribute = attribute;
            _label = label;
            _propertyTable = propertyTable;
        }
    }
}
