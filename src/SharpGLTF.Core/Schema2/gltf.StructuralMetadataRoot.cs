using System;
using System.Collections.Generic;

namespace SharpGLTF.Schema2
{

    public partial class EXTStructuralMetaData
    {
        private ModelRoot modelRoot;
        internal EXTStructuralMetaData(ModelRoot modelRoot)
        {
            this.modelRoot = modelRoot;
        }

        internal List<PropertyTable> PropertyTables
        {
            get { return _propertyTables; }
            set { _propertyTables = value; }
        }

        internal StructuralMetadataSchema Schema
        {
            get { return _schema; }
            set { _schema = value; }
        }
    }

    partial class PropertyTable
    {
        public string Class
        {
            get
            {
                return _class;
            }
            set
            {
                _class = value;
            }
        }

        public int Count
        {
            get
            {
                return _count;
            }
            set
            {
                _count = value;
            }
        }

        public Dictionary<String, PropertyTableProperty> Properties
        {
            get
            {
                return _properties;
            }
            set
            {
                _properties = value;
            }
        }


        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }
    }

    partial class PropertyTableProperty
    {
        public StringOffsets? StringOffsetType
        {
            get
            {
                return _stringOffsetType;
            }
            set
            {
                _stringOffsetType = value;
            }
        }

        public Int32? StringOffsets
        {
            get { return _stringOffsets; }
            set { _stringOffsets = value; }
        }

        public Int32 Values
        {
            get { return _values; }
            set { _values = value; }

        }
    }

    partial class ModelRoot
    {
        public void DoSomething()
        {
            var ext = UseExtension<EXTStructuralMetaData>();
            var propertyTable = new PropertyTable();
            propertyTable.Name = "propertyTable name";
            propertyTable.Count = 18;
            propertyTable.Class = "terrain";

            var properties = new Dictionary<String, PropertyTableProperty>();
            var bgtType = new PropertyTableProperty();
            bgtType.StringOffsetType = StringOffsets.UINT32;
            bgtType.StringOffsets = 3;
            bgtType.Values = 2;
            properties.Add("bgt_type", bgtType);

            var bronhouder = new PropertyTableProperty();
            bronhouder.StringOffsetType = StringOffsets.UINT32;
            bronhouder.StringOffsets = 5;
            bronhouder.Values = 4;
            properties.Add("bronhouder", bronhouder);

            var objectId = new PropertyTableProperty();
            objectId.Values = 6;
            properties.Add("objectid", objectId);

            propertyTable.Properties = properties;
            ext.PropertyTables = new List<PropertyTable>() { propertyTable };

        }
    }
}
