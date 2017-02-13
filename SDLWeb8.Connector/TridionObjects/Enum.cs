using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreServiceWithWeb8.TridionObjects
{
    public class EnumType
    {
        public enum SchemaType { Component, Metadata, Multimedia, None };
        public enum FieldType { Component, Metadata, None };

        public const string SingleLineTextFieldDefinitionData = "SingleLineTextFieldDefinitionData";
        public const string EmbeddedSchemaFieldDefinitionData = "EmbeddedSchemaFieldDefinitionData";
        public const string DateFieldDefinitionData = "DateFieldDefinitionData";
        public const string NumberFieldDefinitionData = "NumberFieldDefinitionData";
        public const string ComponentLinkFieldDefinitionData = "ComponentLinkFieldDefinitionData";
        public const string MultimediaLinkFieldDefinitionData = "MultimediaLinkFieldDefinitionData";
        public const string KeywordFieldDefinitionData = "KeywordFieldDefinitionData";

    }
}
