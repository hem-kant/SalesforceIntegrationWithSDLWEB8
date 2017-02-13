using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using CoreServiceClientFrameWork.CoreServiceFramework;
using Tridion.ContentManager.CoreService.Client;
using System.Xml;
using System.Collections;

namespace CoreServiceWithWeb8.TridionObjects
{
    #region Helper Class
    public class Helper
    {
        #region Module Level Variable
        public enum FieldType { Component, Metadata, None };

        public enum SchemaType { Component, Metadata, Multimedia, None };

        public const string SingleLineTextFieldDefinitionData = "SingleLineTextFieldDefinitionData";
        public const string EmbeddedSchemaFieldDefinitionData = "EmbeddedSchemaFieldDefinitionData";
        public const string DateFieldDefinitionData = "DateFieldDefinitionData";
        public const string NumberFieldDefinitionData = "NumberFieldDefinitionData";
        public const string ComponentLinkFieldDefinitionData = "ComponentLinkFieldDefinitionData";
        public const string MultimediaLinkFieldDefinitionData = "MultimediaLinkFieldDefinitionData";
        public const string KeywordFieldDefinitionData = "KeywordFieldDefinitionData";

        public const string CoreServiceUri = "/webservices/CoreService2011.svc/wsHttp";
        public const string CoreServiceUriForMutimedia = "/webservices/CoreService2011.svc//streamUpload_basicHttp";
        #endregion


        #region Get List data
        private static List<String> GetListData(string strValue)
        {
            try
            {
                List<string> str = new List<string>();
                string[] values = strValue.Split(',');
                for (int i = 0; i < values.Length; i++)
                {
                    if (values[i] != "")
                        str.Add(values[i].Trim());
                }

                return str.Distinct().ToList();
            }
            catch (Exception ex)
            {

                return null;
            }

        }
        #endregion
         

        #region Remove All Namespaces
        //Implemented based on interface, not part of algorithm
        public static string RemoveAllNamespaces(string xmlDocument)
        {
            XElement xmlDocumentWithoutNs = RemoveAllNamespaces(XElement.Parse(xmlDocument));

            return xmlDocumentWithoutNs.ToString();
        }


        //Core recursion function
        private static XElement RemoveAllNamespaces(XElement xmlDocument)
        {
            if (!xmlDocument.HasElements)
            {
                XElement xElement = new XElement(xmlDocument.Name.LocalName);
                xElement.Value = xmlDocument.Value;

                foreach (XAttribute attribute in xmlDocument.Attributes())
                    xElement.Add(attribute);

                return xElement;
            }
            return new XElement(xmlDocument.Name.LocalName, xmlDocument.Elements().Select(el => RemoveAllNamespaces(el)));
        }
        #endregion

        #region Get Tridion Object
        public static TridionObjectInfo GetTridionObject(ICoreServiceFrameworkContext coreService, ItemType itemtype, string parentTcmUri, string searchtext)
        {
            TridionObjectInfo tridionObjectInfo = new TridionObjectInfo();
            try
            {
                IdentifiableObjectData tridionObject = null;
                var filter = new OrganizationalItemItemsFilterData
                {
                    Recursive = true,
                    ItemTypes = new ItemType[] { itemtype }
                };
                var pageList = coreService.Client.GetListXml(parentTcmUri, filter);

                int objectCount = (from e in pageList.Elements()
                                   where e.Attribute("Title").Value.ToLower().Equals(searchtext.ToLower()) || e.Attribute("ID").Value.ToLower() == searchtext.ToLower()
                                   select e.Attribute("ID").Value).Count();

                tridionObjectInfo.ObjectCount = objectCount;
                if (objectCount > 0)
                {
                    var objectUri = (from e in pageList.Elements()
                                     where e.Attribute("Title").Value.ToLower().Equals(searchtext.ToLower()) || e.Attribute("ID").Value.ToLower() == searchtext.ToLower()
                                     select e.Attribute("ID").Value).First();

                    tridionObject = coreService.Client.Read(objectUri, new ReadOptions
                    {
                        LoadFlags = LoadFlags.None
                    });
                    tridionObjectInfo.TridionObject = tridionObject;
                    tridionObjectInfo.TcmUri = objectUri;
                }
            }
            catch (Exception ex)
            {


            }
            return tridionObjectInfo;
        }
        #endregion

        #region Get Component Schema Fields
        public static Fields GetComponentSchemaFields(ICoreServiceFrameworkContext coreService, string schemaID, EnumType.FieldType fieldType)
        {
            try
            {
                SchemaFieldsData schemaFieldData = coreService.Client.ReadSchemaFields(schemaID, true, new ReadOptions());
                return fieldType == EnumType.FieldType.Component ? Fields.ForContentOf(schemaFieldData)
                                                                     : Fields.ForMetadataOf(schemaFieldData);
            }
            catch (Exception ex)
            {

                return null;
            }

        }
        #endregion

        #region FieldEnumerator Class
        public class FieldEnumerator : IEnumerator<Field>
        {
            private Fields fields;
            private ItemFieldDefinitionData[] definitions;

            // Enumerators are positioned before the first element until the first MoveNext() call
            int position = -1;

            public FieldEnumerator(Fields _fields, ItemFieldDefinitionData[] _definitions)
            {
                fields = _fields;
                definitions = _definitions;
            }

            public bool MoveNext()
            {
                position++;
                return (position < definitions.Length);
            }

            public void Reset()
            {
                position = -1;
            }

            object IEnumerator.Current
            {
                get
                {
                    return Current;
                }
            }

            public Field Current
            {
                get
                {
                    try
                    {
                        return new Field(fields, definitions[position]);
                    }
                    catch (IndexOutOfRangeException)
                    {
                        throw new InvalidOperationException();
                    }
                }
            }

            public void Dispose()
            {
            }
        }
        #endregion

        #region Value Collection Class
        public class ValueCollection
        {
            private Fields fields;
            private ItemFieldDefinitionData definition;

            public ValueCollection(Fields _fields, ItemFieldDefinitionData _definition)
            {
                fields = _fields;
                definition = _definition;
            }

            public int Count
            {
                get { return fields.GetFieldElements(definition).Count(); }
            }

            public bool IsLinkField
            {
                get { return definition is ComponentLinkFieldDefinitionData || definition is ExternalLinkFieldDefinitionData || definition is MultimediaLinkFieldDefinitionData; }
            }
            public bool IsRichTextField
            {
                get { return definition is XhtmlFieldDefinitionData; }
            }

            public string this[int i]
            {
                get
                {
                    XmlElement[] elements = fields.GetFieldElements(definition).ToArray();
                    if (i >= elements.Length) throw new IndexOutOfRangeException();
                    if (IsLinkField)
                    {
                        return elements[i].Attributes["xlink:href"].Value;
                    }
                    else
                    {
                        return elements[i].InnerXml.ToString(); // used to be InnerText
                    }
                }
                set
                {
                    XmlElement[] elements = fields.GetFieldElements(definition).ToArray<XmlElement>();
                    if (i >= elements.Length) throw new IndexOutOfRangeException();
                    if (IsLinkField)
                    {
                        elements[i].SetAttribute("href", "http://www.w3.org/1999/xlink", value);
                        elements[i].SetAttribute("type", "http://www.w3.org/1999/xlink", "simple");
                    }
                    else
                    {
                        if (IsRichTextField)
                        {
                            elements[i].InnerXml = value;
                        }
                        else
                        {
                            elements[i].InnerText = value;
                        }
                    }
                }
            }

            public IEnumerator<string> GetEnumerator()
            {
                return fields.GetFieldElements(definition).Select<XmlElement, string>(elm => IsLinkField ? elm.Attributes["xlink:href"].Value : elm.InnerXml.ToString()
                ).GetEnumerator();
            }
        }
        #endregion
    }
    #endregion
}
