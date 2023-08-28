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
            PropertyTables = new List<PropertyTable>();
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

    partial class StructuralMetadataSchema
    {
        public StructuralMetadataSchema()
        {
            Classes = new Dictionary<string, StructuralMetadataClass>();
        }

        public Dictionary<String, StructuralMetadataClass> Classes
        {
            get { return _classes; }
            set { _classes = value; }
        }
    }

    partial class PropertyTable
    {
        public PropertyTable()
        {
            Properties = new Dictionary<string, PropertyTableProperty> ();
        }
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
        public PropertyTableProperty()
        {

        }
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

    partial class StructuralMetadataClass
    {
        public StructuralMetadataClass()
        {
            Properties = new Dictionary<string, ClassProperty>();
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public Dictionary<String, ClassProperty> Properties
        {
            get { return _properties; }
            set { _properties = value; }
        }
    }

    partial class ClassProperty
    {
        public ElementType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public Object NoData
        {
            get { return _noData; }
            set { _noData = value; }
        }

        public DataType? ComponentType
        {
            get { return _componentType; }
            set { _componentType = value; }
        }

        // Todo: add other properties...
    }

    partial class ModelRoot
    {
        private PropertyTable GetPropertyTable(string PropertyTableName, string Fieldname, int AccessorId, int numberOfFeatures)
        {
            var propertyTable = new PropertyTable();
            propertyTable.Count = numberOfFeatures;
            propertyTable.Class = PropertyTableName;

            var properties = new Dictionary<String, PropertyTableProperty>();
            var objectId = new PropertyTableProperty();
            //objectId.StringOffsetType= StringOffsets.UINT32;
            //objectId.StringOffsets = numberOfFeatures + 1;
            objectId.Values = AccessorId;
            // objectId.
            properties.Add(Fieldname, objectId);

            propertyTable.Properties = properties;
            return propertyTable;
        }

        public void AddStructuralMetadataStrings(string fieldname, List<string> values)
        {
            var bytes = BinaryTable.GetStringAsBytesWithOffsetBuffer(values);
            var componentType = DataType.INT32;
            // var componentType = DataType.INT32;

            AddStructuralMetadata(fieldname, bytes, componentType, values.Count);
        }

        public void AddStructuralMetadata(string fieldname, List<uint> values)
        {
            var bytes = BinaryTable.GetIntsAsBytes(values);
            var componentType = DataType.INT32;
            AddStructuralMetadata(fieldname, bytes, componentType, values.Count);
        }

        private void AddStructuralMetadata(string fieldname, byte[] bytes, DataType componentType, int numberOfFeatures)
        {
            var propertyTableName = "propertyTable";
            var featureId = new FeatureID(numberOfFeatures, 0, 0);
            var featureIds = new List<FeatureID>() { featureId };
            // todo fix when there are multiple MeshPrimitives
            LogicalMeshes[0].Primitives[0].SetFeatureIds(featureIds);

            var bview = UseBufferView(bytes);
            var index = bview.LogicalIndex;

            var ext = UseExtension<EXTStructuralMetaData>();

            var structuralMetadataSchema = new StructuralMetadataSchema();
            var structuralMetadataClass = new StructuralMetadataClass();
            var properties = new Dictionary<string, ClassProperty>();

            var p2 = new ClassProperty();
            p2.ComponentType = componentType;

            // p2.Type = ElementType.STRING;
            properties.Add(fieldname, p2);

            structuralMetadataClass.Properties = properties;
            ext.Schema = structuralMetadataSchema;

            structuralMetadataSchema.Classes = new Dictionary<string, StructuralMetadataClass>
            {
            { propertyTableName , structuralMetadataClass }
            };

            ext.PropertyTables = new List<PropertyTable>() { GetPropertyTable(propertyTableName, fieldname, index, numberOfFeatures) };
        }
    }
}
