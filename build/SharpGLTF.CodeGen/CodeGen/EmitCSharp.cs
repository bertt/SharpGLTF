﻿// #define USENEWTONSOFT

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using static System.FormattableString;

namespace SharpGLTF.CodeGen
{
    using SchemaReflection;

    /// <summary>
    /// Takes a <see cref="SchemaReflection.SchemaType.Context"/> and emits
    /// all its enums and classes as c# source code
    /// </summary>
    class CSharpEmitter
    {
        #region runtime names

        class _RuntimeType
        {
            internal _RuntimeType(SchemaType t) { _PersistentType = t; }

            private readonly SchemaType _PersistentType;

            public string RuntimeNamespace { get; set; }

            public string RuntimeName { get; set; }

            private readonly Dictionary<string, _RuntimeField> _Fields = new Dictionary<string, _RuntimeField>();
            private readonly Dictionary<string, _RuntimeEnum> _Enums = new Dictionary<string, _RuntimeEnum>();

            public _RuntimeField UseField(FieldInfo finfo)
            {
                var key = $"{finfo.PersistentName}";

                if (_Fields.TryGetValue(key, out _RuntimeField rfield)) return rfield;

                rfield = new _RuntimeField(finfo);

                _Fields[key] = rfield;

                return rfield;
            }

            public _RuntimeEnum UseEnum(string name)
            {
                var key = name;

                if (_Enums.TryGetValue(key, out _RuntimeEnum renum)) return renum;

                renum = new _RuntimeEnum(name);

                _Enums[key] = renum;

                return renum;
            }
        }

        class _RuntimeEnum
        {
            internal _RuntimeEnum(string name) { _Name = name; }

            private readonly string _Name;
        }

        class _RuntimeField
        {
            internal _RuntimeField(FieldInfo f) { _PersistentField = f; }

            private readonly FieldInfo _PersistentField;

            public string PrivateField { get; set; }
            public string PublicProperty { get; set; }

            public string CollectionContainer { get; set; }
            public string DictionaryContainer { get; set; }

            // MinVal, MaxVal, readonly, static

            // serialization sections
            // deserialization sections
            // validation sections
            // clone sections
        }        

        private readonly Dictionary<string, _RuntimeType> _Types = new Dictionary<string, _RuntimeType>();

        private string _DefaultCollectionContainer = "TItem[]";

        #endregion

        #region setup & declaration        

        private static string _SanitizeName(string name)
        {
            return name.Replace(" ", string.Empty, StringComparison.OrdinalIgnoreCase);
        }

        private _RuntimeType _UseType(SchemaType stype)
        {
            var key = $"{stype.PersistentName}";

            if (_Types.TryGetValue(key, out _RuntimeType rtype)) return rtype;

            rtype = new _RuntimeType(stype)
            {
                RuntimeName = _SanitizeName(stype.PersistentName)
            };

            _Types[key] = rtype;

            return rtype;
        }

        private _RuntimeField _UseField(FieldInfo finfo) { return _UseType(finfo.DeclaringClass).UseField(finfo); }

        public void SetRuntimeName(SchemaType stype, string newName, string runtimeNamespace = null)
        {
            var t = _UseType(stype);

            t.RuntimeNamespace = runtimeNamespace;
            t.RuntimeName = newName;
        }

        public void SetRuntimeName(string persistentName, string runtimeName, string runtimeNamespace = null)
        {
            if (!_Types.TryGetValue(persistentName, out _RuntimeType t)) return;

            t.RuntimeNamespace = runtimeNamespace;
            t.RuntimeName = runtimeName;
        }

        public string GetRuntimeName(string persistentName)
        {
            return _Types[persistentName].RuntimeName;
        }

        public string GetRuntimeNamespace(string persistentName)
        {
            return _Types[persistentName].RuntimeNamespace ?? Constants.OutputNamespace;
        }

        public void SetFieldName(FieldInfo finfo, string name) { _UseField(finfo).PrivateField = name; }

        public string GetFieldRuntimeName(FieldInfo finfo) { return _UseField(finfo).PrivateField; }

        public void SetPropertyName(FieldInfo finfo, string name) { _UseField(finfo).PublicProperty = name; }

        public string GetPropertyName(FieldInfo finfo) { return _UseField(finfo).PublicProperty; }



        public void SetCollectionContainer(string container) { _DefaultCollectionContainer = container; }

        public void SetCollectionContainer(FieldInfo finfo, string container) { _UseField(finfo).CollectionContainer = container; }        

        public void SetFieldToChildrenList(SchemaType.Context ctx, string persistentName, string fieldName)
        {
            var classType = ctx.FindClass(persistentName);
            if (classType == null) return;
            var field = classType.UseField(fieldName);
            var runtimeName = this.GetRuntimeName(persistentName);
            this.SetCollectionContainer(field, $"ChildrenList<TItem,{runtimeName}>");
        }

        public void SetFieldToChildrenDictionary(SchemaType.Context ctx, string persistentName, string fieldName)
        {
            var classType = ctx.FindClass(persistentName);
            if (classType == null) return;
            var field = classType.UseField(fieldName);
            var runtimeName = this.GetRuntimeName(persistentName);
            this.SetCollectionContainer(field, $"ChildrenDictionary<TItem,{runtimeName}>");
        }

        public void DeclareClass(ClassType type)
        {
            _UseType(type);

            foreach(var f in type.Fields)
            {
                var runtimeName = _SanitizeName(f.PersistentName).Replace("@","at", StringComparison.Ordinal);

                SetFieldName(f, $"_{runtimeName}");
                SetPropertyName(f, runtimeName);
            }
        }

        public void DeclareEnum(EnumType type)
        {
            _UseType(type);

            foreach (var f in type.Values)
            {
                // SetFieldName(f, $"_{runtimeName}");
                // SetPropertyName(f, runtimeName);
            }
        }

        public void DeclareContext(SchemaType.Context context)
        {
            foreach(var ctype in context.Classes)
            {
                DeclareClass(ctype);
            }

            foreach (var etype in context.Enumerations)
            {
                DeclareEnum(etype);
            }
        }

        internal string _GetRuntimeName(SchemaType type) { return _GetRuntimeName(type, null); }

        private string _GetRuntimeName(SchemaType type, _RuntimeField extra)
        {
            switch (type)
            {
                case null: throw new ArgumentNullException(nameof(type));

                case ObjectType anyType: return anyType.PersistentName;

                case StringType strType: return strType.PersistentName;

                case BlittableType blitType:
                    {
                        var tname = blitType.DataType.Name;

                        return blitType.IsNullable ? $"{tname}?" : tname;
                    }

                case ArrayType arrayType:
                    {
                        var container = extra?.CollectionContainer;
                        if (string.IsNullOrWhiteSpace(container)) container = _DefaultCollectionContainer;

                        return container.Replace("TItem", _GetRuntimeName(arrayType.ItemType), StringComparison.Ordinal);
                    }

                case DictionaryType dictType:
                    {
                        var key = _GetRuntimeName(dictType.KeyType);
                        var val = _GetRuntimeName(dictType.ValueType);

                        var container = extra?.CollectionContainer ?? string.Empty;

                        if (container.StartsWith("ChildrenDictionary<"))
                        {
                            if (key == "String") return container.Replace("TItem", val, StringComparison.Ordinal);
                        }

                        return $"Dictionary<{key},{val}>";
                    }

                case EnumType enumType: return _UseType(enumType).RuntimeName;

                case ClassType classType: return _UseType(classType).RuntimeName;

                default: throw new NotImplementedException(type.PersistentName);
            }
        }

        private string _GetConstantRuntimeName(SchemaType type)
        {
            switch (type)
            {
                case StringType strType: return $"const {typeof(string).Name}";

                case BlittableType blitType:
                    {
                        var tname = blitType.DataType.Name;

                        if (blitType.DataType == typeof(int)) return $"const {tname}";
                        if (blitType.DataType == typeof(float)) return $"const {tname}";
                        if (blitType.DataType == typeof(double)) return $"const {tname}";

                        return $"static readonly {tname}";
                    }

                case EnumType enumType: return $"const {_UseType(enumType).RuntimeName}";

                case ArrayType aType: return $"static readonly {_UseType(aType).RuntimeName}";

                default: throw new NotImplementedException();
            }
        }

        private Object _GetConstantRuntimeValue(SchemaType type, Object value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            switch (type)
            {
                case StringType _:

                    return value is string
                        ? value
                        : Convert.ChangeType(value, typeof(string), System.Globalization.CultureInfo.InvariantCulture);

                case BlittableType btype:
                    {
                        if (btype.DataType == typeof(bool).GetTypeInfo())
                        {
                            if (value is bool) return value;

                            var str = value as string;

                            if (str.ToUpperInvariant() == "FALSE") return false;
                            if (str.ToUpperInvariant() == "TRUE") return true;
                            throw new NotImplementedException();
                        }

                        return value is string
                            ? value
                            : Convert.ChangeType(value, btype.DataType.AsType(), System.Globalization.CultureInfo.InvariantCulture);
                    }

                case EnumType etype:
                    {
                        var etypeName = _GetRuntimeName(type);

                        if (value is string) return $"{etypeName}.{value}";
                        else return $"({etypeName}){value}";
                    }

                case ArrayType aType:
                    {
                        var atypeName = _GetRuntimeName(type);

                        return value.ToString();
                    }

                default: throw new NotImplementedException();
            }
        }        

        #endregion

        #region emit

        public string EmitContext(SchemaType.Context context)
        {
            var sb = new StringBuilder();

            sb.AppendLine("// <auto-generated/>");
            sb.AppendLine();

            sb.AppendLine("//------------------------------------------------------------------------------------------------");
            sb.AppendLine("//      This file has been programatically generated; DON´T EDIT!");
            sb.AppendLine("//------------------------------------------------------------------------------------------------");

            sb.AppendLine();            
            
            sb.AppendLine("#pragma warning disable SA1001");
            sb.AppendLine("#pragma warning disable SA1027");
            sb.AppendLine("#pragma warning disable SA1028");
            sb.AppendLine("#pragma warning disable SA1121");
            sb.AppendLine("#pragma warning disable SA1205");
            sb.AppendLine("#pragma warning disable SA1309");
            sb.AppendLine("#pragma warning disable SA1402");
            sb.AppendLine("#pragma warning disable SA1505");
            sb.AppendLine("#pragma warning disable SA1507");
            sb.AppendLine("#pragma warning disable SA1508");
            sb.AppendLine("#pragma warning disable SA1652");

            sb.AppendLine();           

            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Linq;");
            sb.AppendLine("using System.Text;");
            sb.AppendLine("using System.Numerics;");
            sb.AppendLine("using System.Text.Json;");            

            string currentNamespace = null;

            void _setCurrentNamespace(string ns)
            {
                if (currentNamespace == ns) return;
                if (currentNamespace != null) sb.AppendLine("}");
                currentNamespace = ns;
                if (currentNamespace != null)
                {
                    sb.AppendLine();
                    sb.AppendLine($"namespace {currentNamespace}");
                    sb.AppendLine("{");

                    sb.AppendLine("using Collections;".Indent(1));
                    sb.AppendLine();
                }
            }            

            foreach (var etype in context.Enumerations)
            {
                _setCurrentNamespace(GetRuntimeNamespace(etype.PersistentName));

                var cout = EmitEnum(etype);

                sb.AppendLine(cout);
                sb.AppendLine();
            }

            foreach (var ctype in context.Classes)
            {
                if (ctype.IgnoredByEmitter) continue;

                _setCurrentNamespace(GetRuntimeNamespace(ctype.PersistentName));

                var cout = EmitClass(ctype);

                sb.AppendLine(cout);
                sb.AppendLine();
            }

            _setCurrentNamespace(null);

            return sb.ToString();
        }

        public string EmitEnum(EnumType type)
        {
            var sb = new StringBuilder();

            foreach (var l in type.Description.EmitSummary(0)) sb.EmitLine(1, l);

            sb.EmitLine(1, $"public enum {_GetRuntimeName(type)}");
            sb.EmitLine(1, "{");

            if (type.UseIntegers)
            {
                foreach (var kvp in type.Values)
                {
                    var k =  kvp.Key;

                    sb.EmitLine(2, $"{k} = {kvp.Value},");
                }
            }
            else
            {
                foreach (var kvp in type.Values)
                {
                    var k = kvp.Key;

                    sb.EmitLine(2, $"{k},");
                }
            }            

            sb.EmitLine(1, "}");

            return sb.ToString();
        }

        public string EmitClass(ClassType type)
        {
            var xclass = new CSharpClassEmitter(this)
            {
                ClassSummary = type.Description,
                ClassDeclaration = _GetClassDeclaration(type),
                HasBaseClass = type.BaseClass != null
            };

            xclass.AddFields(type);            

            return String.Join("\r\n",xclass.EmitCode().Indent(1));            
        }

        private string _GetClassDeclaration(ClassType type)
        {
            var classDecl = string.Empty;            
            classDecl += "partial ";
            classDecl += "class ";
            classDecl += _GetRuntimeName(type);
            if (type.BaseClass != null) classDecl += $" : {_GetRuntimeName(type.BaseClass)}";
            return classDecl;
        }

        internal IEnumerable<string> _GetClassField(FieldInfo f)
        {            
            var tdecl = _GetRuntimeName(f.FieldType, _UseField(f));
            var fname = GetFieldRuntimeName(f);

            string defval = string.Empty;

            if (f.DefaultValue != null)
            {
                var tconst = _GetConstantRuntimeName(f.FieldType);
                var vconst = _GetConstantRuntimeValue(f.FieldType, f.DefaultValue);

                // fix boolean value
                if (vconst is Boolean bconst) vconst = bconst ? "true" : "false";

                defval = $"{fname}Default";

                yield return Invariant($"private {tconst} {defval} = {vconst};");
            }

            if (f.ExclusiveMinimumValue != null)
            {
                var tconst = _GetConstantRuntimeName(f.FieldType);
                var vconst = _GetConstantRuntimeValue(f.FieldType, f.ExclusiveMinimumValue);
                yield return Invariant($"private {tconst} {fname}ExclusiveMinimum = {vconst};");
            }

            if (f.InclusiveMinimumValue != null)
            {
                var tconst = _GetConstantRuntimeName(f.FieldType);
                var vconst = _GetConstantRuntimeValue(f.FieldType, f.InclusiveMinimumValue);
                yield return Invariant($"private {tconst} {fname}Minimum = {vconst};");
            }

            if (f.InclusiveMaximumValue != null)
            {
                var tconst = _GetConstantRuntimeName(f.FieldType);
                var vconst = _GetConstantRuntimeValue(f.FieldType, f.InclusiveMaximumValue);
                yield return Invariant($"private {tconst} {fname}Maximum = {vconst};");
            }

            if (f.ExclusiveMaximumValue != null)
            {
                var tconst = _GetConstantRuntimeName(f.FieldType);
                var vconst = _GetConstantRuntimeValue(f.FieldType, f.ExclusiveMaximumValue);
                yield return Invariant($"private {tconst} {fname}ExclusiveMaximum = {vconst};");
            }

            if (f.MinItems > 0)
            {                    
                yield return $"private const int {fname}MinItems = {f.MinItems};";
            }

            if (f.MaxItems > 0 && f.MaxItems < int.MaxValue)
            {                    
                yield return $"private const int {fname}MaxItems = {f.MaxItems};";
            }

            if (f.FieldType is EnumType etype && etype.IsNullable) tdecl = tdecl + "?";            

            yield return string.IsNullOrEmpty(defval) ? $"private {tdecl} {fname};" : $"private {tdecl} {fname} = {defval};";

            yield return string.Empty;
        }            

        #endregion
    }

    /// <summary>
    /// Utility class to emit a <see cref="SchemaReflection.ClassType"/>
    /// as c# source code
    /// </summary>
    class CSharpClassEmitter
    {
        #region constructor

        public CSharpClassEmitter(CSharpEmitter emitter)
        {
            _Emitter = emitter;            
        }

        #endregion

        #region data

        private readonly CSharpEmitter _Emitter;

        private readonly List<string> _Fields = new List<string>();
        private readonly List<string> _SerializerBody = new List<string>();
        private readonly List<string> _DeserializerSwitchBody = new List<string>();

        public string ClassSummary { get; set; }

        public string ClassDeclaration { get; set; }

        public bool HasBaseClass { get; set; }

        private const string _READERMODIFIER = "ref ";

        #endregion

        #region API

        public void AddFields(ClassType type)
        {
            foreach (var f in type.Fields)
            {
                var trname = _Emitter._GetRuntimeName(f.FieldType);
                var frname = _Emitter.GetFieldRuntimeName(f);

                _Fields.AddRange(_Emitter._GetClassField(f));

                if (f.FieldType is EnumType etype)
                {
                    // emit serializer
                    var smethod = etype.UseIntegers ? "SerializePropertyEnumValue" : "SerializePropertyEnumSymbol";
                    smethod = $"{smethod}<{trname}>(writer, \"{f.PersistentName}\", {frname}";
                    if (f.DefaultValue != null) smethod += $", {frname}Default";
                    smethod += ");";
                    this.AddFieldSerializerCase(smethod);

                    // emit deserializer
                    this.AddFieldDeserializerCase(f.PersistentName, $"{frname} = DeserializePropertyValue<{_Emitter._GetRuntimeName(etype)}>({_READERMODIFIER}reader);");

                    continue;
                }

                this.AddFieldSerializerCase(_GetJSonSerializerMethod(f));
                this.AddFieldDeserializerCase(f.PersistentName, _GetJSonDeserializerMethod(f));
            }
        }

        private string _GetJSonSerializerMethod(FieldInfo f)
        {
            var pname = f.PersistentName;
            var fname = _Emitter.GetFieldRuntimeName(f);

            if (f.FieldType is ClassType ctype)
            {
                return $"SerializePropertyObject(writer, \"{pname}\", {fname});";
            }

            if (f.FieldType is ArrayType atype)
            {
                if (f.MinItems > 0) return $"SerializeProperty(writer, \"{pname}\", {fname}, {fname}MinItems);";

                return $"SerializeProperty(writer,\"{pname}\",{fname});";
            }

            if (f.DefaultValue != null) return $"SerializeProperty(writer, \"{pname}\", {fname}, {fname}Default);";

            return $"SerializeProperty(writer, \"{pname}\", {fname});";
        }

        private string _GetJSonDeserializerMethod(FieldInfo f)
        {
            var fname = _Emitter.GetFieldRuntimeName(f);

            var ownerTypeName = _Emitter._GetRuntimeName(f.DeclaringClass);

            if (f.FieldType is ArrayType atype)
            {
                var titem = _Emitter._GetRuntimeName(atype.ItemType);
                return $"DeserializePropertyList<{ownerTypeName}, {titem}>({_READERMODIFIER}reader, this, {fname});";
            }
            else if (f.FieldType is DictionaryType dtype)
            {
                var titem = _Emitter._GetRuntimeName(dtype.ValueType);
                return $"DeserializePropertyDictionary<{ownerTypeName}, {titem}>({_READERMODIFIER}reader, this, {fname});";
            }
            
            var fieldTypeName = _Emitter._GetRuntimeName(f.FieldType);            

            return $"DeserializePropertyValue<{ownerTypeName}, {fieldTypeName}>({_READERMODIFIER}reader, this, out {fname});";
        }        

        public void AddFieldSerializerCase(string line) { _SerializerBody.Add(line); }

        public void AddFieldDeserializerCase(string persistentName, string line)
        {
            _DeserializerSwitchBody.Add($"case \"{persistentName}\": {line} break;");            
        }

        public IEnumerable<string> EmitCode()
        {
            #if USENEWTONSOFT
            var readerType = "JsonReader";
            var writerType = "JsonWriter";
            #else
            var readerType = "ref Utf8JsonReader";
            var writerType = "Utf8JsonWriter";
            #endif


            foreach (var l in ClassSummary.EmitSummary(0)) yield return l;

            yield return "#if NET6_0_OR_GREATER";
            yield return "[System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembers(System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.NonPublicConstructors | System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.PublicConstructors)]";
            yield return "#endif";

            yield return "[global::System.CodeDom.Compiler.GeneratedCodeAttribute(\"SharpGLTF.CodeGen\", \"1.0.0.0\")]";

            yield return ClassDeclaration;
            yield return "{";

            yield return string.Empty;

            foreach (var l in _Fields.Indent(1)) yield return l;

            yield return string.Empty;

            // yield return "/// <inheritdoc />".Indent(1);
            yield return $"protected override void SerializeProperties({writerType} writer)".Indent(1);
            yield return "{".Indent(1);
            if (HasBaseClass) yield return "base.SerializeProperties(writer);".Indent(2);
            foreach (var l in _SerializerBody.Indent(2)) yield return l;
            yield return "}".Indent(1);

            yield return string.Empty;

            // yield return "/// <inheritdoc />".Indent(1);
            yield return $"protected override void DeserializeProperty(string jsonPropertyName, {readerType} reader)".Indent(1);
            yield return "{".Indent(1);
            yield return "switch (jsonPropertyName)".Indent(2);
            yield return "{".Indent(2);

            foreach (var l in _DeserializerSwitchBody.Indent(3)) yield return l;
            if (HasBaseClass) yield return $"default: base.DeserializeProperty(jsonPropertyName,{_READERMODIFIER}reader); break;".Indent(3);
            else yield return "default: throw new NotImplementedException();".Indent(3);

            yield return "}".Indent(2);
            yield return "}".Indent(1);

            yield return string.Empty;

            yield return "}";
        }

        #endregion
    }
}
