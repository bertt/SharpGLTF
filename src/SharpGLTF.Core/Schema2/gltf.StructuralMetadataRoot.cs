using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpGLTF.Schema2
{
    public partial class EXTStructuralMetaData
    {
        internal EXTStructuralMetaData(ModelRoot modelRoot)
        {
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
            Properties = new Dictionary<string, PropertyTableProperty>();
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

        public DataType? ComponentType
        {
            get { return _componentType; }
            set { _componentType = value; }
        }

        // Todo: add other properties...
    }

    partial class ModelRoot
    {
        public void AddMetadata<T>(EXTStructuralMetaData ext, string fieldname, List<T> values)
        {
            var propertyTableName = ext.PropertyTables[0].Class;

            var type = typeof(T);
            if (type == typeof(float) || type == typeof(uint) || type == typeof(int))
            {
                var bytes = BinaryTable.GetBytes(values);
                AddStructuralMetadata<T>(this, ext, propertyTableName, fieldname, bytes);
            }
            else if (type == typeof(string))
            {
                var strings = values.Select(x => x.ToString()).ToList();
                var stringBytes = BinaryTable.GetStringsAsBytes(strings);
                if (stringBytes.Length > 0)
                {
                    var offSetbytes = BinaryTable.GetOffsetBuffer(strings);
                    AddStructuralMetadata<string>(this, ext, propertyTableName, fieldname, stringBytes, offSetbytes);
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private static void AddStructuralMetadata<T>(ModelRoot modelRoot, EXTStructuralMetaData ext, string propertyTableName, string fieldname, byte[] bytes, byte[] offsetBytes = null)
        {
            AddFieldToSchema<T>(ext, propertyTableName, fieldname);
            var propertyTableProperty = GetPropertyTableProperty(modelRoot, bytes, offsetBytes);
            ext.PropertyTables[0].Properties.Add(fieldname, propertyTableProperty);
        }

        private static PropertyTableProperty GetPropertyTableProperty(ModelRoot modelRoot, byte[] bytes, byte[] offsetBytes = null)
        {
            int? stringOffsets = null;
            var bview = modelRoot.UseBufferView(bytes);
            var values = bview.LogicalIndex;

            if (offsetBytes != null)
            {
                var bviewOffsets = modelRoot.UseBufferView(offsetBytes);
                stringOffsets = bviewOffsets.LogicalIndex;
            }
            var propertyTableProperty = GetPropertyTableProperty(values, stringOffsets);
            return propertyTableProperty;
        }

        private static void AddFieldToSchema<T>(EXTStructuralMetaData ext, string PropertyTableName, string FieldName)
        {
            var type = typeof(T);

            var classProperty = new ClassProperty();

            if (type == typeof(string))
            {
                classProperty.Type = ElementType.STRING;
            }
            else if (type == typeof(uint))
            {
                classProperty.Type = ElementType.SCALAR;
                classProperty.ComponentType = DataType.UINT32;
            }
            else if (type == typeof(int))
            {
                classProperty.Type = ElementType.SCALAR;
                classProperty.ComponentType = DataType.INT32;
            }
            else if (type == typeof(float))
            {
                classProperty.Type = ElementType.SCALAR;
                classProperty.ComponentType = DataType.FLOAT32;
            }
            else if (type == typeof(Boolean))
            {
                classProperty.Type = ElementType.BOOLEAN;
            }

            ext.Schema.Classes[PropertyTableName].Properties.Add(FieldName, classProperty);
        }

        // Creates EXTStructuralMetaData with Schema and 1 PropertyTable
        public EXTStructuralMetaData InitializeMetadataExtension(string propertyTableName, int numberOfFeatures)
        {
            var featureId = new FeatureID(numberOfFeatures);
            var featureIds = new List<FeatureID>() { featureId };
            // todo fix when there are multiple Meshes?
            foreach(var prim in LogicalMeshes[0].Primitives)
            {
                prim.SetFeatureIds(featureIds);
            }

            var ext = UseExtension<EXTStructuralMetaData>();

            var schema = GetInitialSchema(propertyTableName);
            ext.Schema = schema;
            var propertyTable = GetInitialPropertyTable(propertyTableName, numberOfFeatures);
            ext.PropertyTables = new List<PropertyTable>() { propertyTable };
            return ext;
        }

        // Create a propertyTableProperty with values and stringOffsets
        private static PropertyTableProperty GetPropertyTableProperty(int AccessorId, int? stringOffsets)
        {
            var objectId = new PropertyTableProperty();
            if (stringOffsets != null)
            {
                objectId.StringOffsetType = StringOffsets.UINT32;
                objectId.StringOffsets = stringOffsets;
            }
            objectId.Values = AccessorId;
            return objectId;
        }

        // Get empty PropertyTable
        private static PropertyTable GetInitialPropertyTable(string PropertyTableName, int numberOfFeatures)
        {
            var propertyTable = new PropertyTable();
            propertyTable.Count = numberOfFeatures;
            propertyTable.Class = PropertyTableName;
            var properties = new Dictionary<string, PropertyTableProperty>();
            propertyTable.Properties = properties;
            return propertyTable;
        }

        // Get MetadataSchame with 1 metadata class
        private static StructuralMetadataSchema GetInitialSchema(string schemaName)
        {
            var structuralMetadataSchema = new StructuralMetadataSchema();
            var structuralMetadataClass = new StructuralMetadataClass();
            var classProperties = new Dictionary<string, ClassProperty>();
            structuralMetadataClass.Properties = classProperties;
            structuralMetadataSchema.Classes = new Dictionary<string, StructuralMetadataClass>
            {
            { schemaName , structuralMetadataClass }
            };

            return structuralMetadataSchema;
        }
    }
}
