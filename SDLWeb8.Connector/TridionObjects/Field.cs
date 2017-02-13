using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Tridion.ContentManager.CoreService.Client;

namespace CoreServiceWithWeb8.TridionObjects
{
    public class Field
    {
        private Fields fields;
        private ItemFieldDefinitionData definition;

        public Field(Fields _fields, ItemFieldDefinitionData _definition)
        {
            fields = _fields;
            definition = _definition;
        }

        public string Name
        {
            get { return definition.Name; }
        }
        public int MinOccurs
        {
            get { return definition.MinOccurs; }
        }
        public bool Mandatory
        {
            get { return definition.MinOccurs > 0 ? true : false; }
        }
        public int MaxOccurs
        {
            get { return definition.MaxOccurs; }
        }
        public bool AllowMultiple
        {
            get { return definition.MaxOccurs == -1 ? true : false; }
        }
        public string FieldDataType
        {
            get { return definition.GetType().Name; }
        }
        public Type Type
        {
            get { return definition.GetType(); }
        }

        public List<string> ComponentLinkedSchema
        {
            get
            {
                if (definition.GetType().Name == Helper.ComponentLinkFieldDefinitionData)
                {
                    ComponentLinkFieldDefinitionData clf = (ComponentLinkFieldDefinitionData)definition;
                    return clf.AllowedTargetSchemas.ToList().Select(a => a.IdRef).ToList();
                }
                else
                {
                    return null;
                }
            }
        }

        public List<string> MutimediaLinkedSchema
        {
            get
            {
                if (definition.GetType().Name == Helper.MultimediaLinkFieldDefinitionData)
                {
                    MultimediaLinkFieldDefinitionData clf = (MultimediaLinkFieldDefinitionData)definition;
                    return clf.AllowedTargetSchemas.ToList().Select(a => a.IdRef).ToList();
                }
                else
                {
                    return null;
                }
            }
        }

        public string KeywrodLinkedSchema
        {
            get
            {
                if (definition.GetType().Name == Helper.KeywordFieldDefinitionData)
                {
                    KeywordFieldDefinitionData clf = (KeywordFieldDefinitionData)definition;
                    return clf.Category.IdRef.ToString();
                }
                else
                {
                    return null;
                }
            }
        }

        public List<string> ValuesFromSelectedList
        {
            get
            {
                if (definition.GetType().Name == Helper.SingleLineTextFieldDefinitionData)
                {
                    SingleLineTextFieldDefinitionData clf = (SingleLineTextFieldDefinitionData)definition;
                    return clf.List != null ? clf.List.Entries.ToList().Select(a => a).ToList() : null;
                }
                else
                    return null;
            }
        }

        public string Value
        {
            get
            {
                return Values.Count > 0 ? Values[0] : null;
            }
            set
            {
                if (Values.Count == 0) fields.AddFieldElement(definition);
                Values[0] = value;
            }
        }
        public Helper.ValueCollection Values
        {
            get
            {
                return new Helper.ValueCollection(fields, definition);
            }
        }

        public void AddValue(string value = null)
        {
            //XmlElement newElement = fields.AddFieldElement(definition);
            //if (value != null) newElement.InnerText = value;

            fields.AddFieldElement(definition);
            Values[Values.Count - 1] = value;
        }

        public void RemoveValue(string value)
        {
            var elements = fields.GetFieldElements(definition);
            foreach (var element in elements)
            {
                if (element.InnerText == value)
                {
                    element.ParentNode.RemoveChild(element);
                }
            }
        }

        public void RemoveValue(int i)
        {
            var elements = fields.GetFieldElements(definition).ToArray();
            elements[i].ParentNode.RemoveChild(elements[i]);
        }

        public IEnumerable<Fields> SubFields
        {
            get
            {
                var embeddedFieldDefinition = definition as EmbeddedSchemaFieldDefinitionData;
                if (embeddedFieldDefinition != null)
                {
                    var elements = fields.GetFieldElements(definition);
                    foreach (var element in elements)
                    {
                        yield return new Fields(fields, embeddedFieldDefinition.EmbeddedFields, (XmlElement)element);
                    }
                }
            }
        }

        public Fields GetSubFields(int i = 0)
        {
            var embeddedFieldDefinition = definition as EmbeddedSchemaFieldDefinitionData;
            if (embeddedFieldDefinition != null)
            {
                var elements = fields.GetFieldElements(definition);
                if (i == 0 && !elements.Any())
                {
                    // you can always set the first value of any field without calling AddValue, so same applies to embedded fields
                    AddValue();
                    elements = fields.GetFieldElements(definition);
                }
                return new Fields(fields, embeddedFieldDefinition.EmbeddedFields, elements.ToArray()[i]);
            }
            else
            {
                throw new InvalidOperationException("You can only GetSubField on an EmbeddedSchemaField");
            }
        }
        // The subfield with the given name of this field
        public Field this[string name]
        {
            get { return GetSubFields()[name]; }
        }
        // The subfields of the given value of this field
        public Fields this[int i]
        {
            get { return GetSubFields(i); }
        }

    }
}
