using CoreServiceClientFrameWork.CoreServiceFramework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Xml.Linq;
using Tridion.ContentManager.CoreService.Client;


namespace CoreServiceWithWeb8.TridionObjects
{
    public class TridionComponent
    {
        #region Generate Component
        public static string GenerateComponent(ICoreServiceFrameworkContext coreService, string xml, string schemaID, EnumType.SchemaType schemaType, string folderUri, string title, string ext_Name)
        {
            try
            {

                string Title = string.Empty;
                string Tcmuri = string.Empty;
                SearchQueryData filter = new SearchQueryData();
                filter.FullTextQuery = "title";
                filter.ItemTypes = new ItemType[] { ItemType.Component };

                BasedOnSchemaData basedSchema = new BasedOnSchemaData();
                basedSchema.Schema = new LinkToSchemaData() { IdRef = schemaID };
                basedSchema.Field = "title";
                basedSchema.FieldValue = title;


                filter.BasedOnSchemas = new BasedOnSchemaData[] { basedSchema };
                XElement results = coreService.Client.GetSearchResultsXml(filter);
                for (IEnumerator<XElement> e = results.Descendants().GetEnumerator(); e.MoveNext();)
                {
                    Title = e.Current.Attribute(XName.Get("Title")).Value;
                    Tcmuri = e.Current.FirstAttribute.Value != null ? e.Current.FirstAttribute.Value : null;
                }
                Title = Title != string.Empty ? Title : title;
                ComponentData componentData = GetNewComponent(folderUri, schemaID, schemaType, Title);



                componentData.ComponentType = ComponentType.Normal;
                SchemaData sd = coreService.Client.Read(schemaID, null) as SchemaData;

                var content = XElement.Parse(xml);
                var xmlns = content;//UpdateNodesWithDefaultNamespace(content.ToString(), "xmlns=" + "\"" + @"" + sd.NamespaceUri.ToString() + @"""");

                componentData.Content = xmlns.ToString();


                TridionObjectInfo tridionObjectInfo = Helper.GetTridionObject(coreService, ItemType.Component, folderUri, Title);
                if (tridionObjectInfo.TcmUri != null)
                {
                    componentData.Id = tridionObjectInfo.TcmUri;
                    componentData = (ComponentData)(coreService.Client.SynchronizeWithSchema(
                                                                       componentData,
                                                                       new SynchronizeOptions
                                                                       {
                                                                           SynchronizeFlags = SynchronizeFlags.All
                                                                       }
                                                                       ).SynchronizedItem);
                    //var data = (ComponentData)coreService.Client.Read(tridionObjectInfo.TcmUri, new ReadOptions());
                   // var component = (ComponentData)coreService.Client.Read(tridionObjectInfo.TcmUri, new ReadOptions());
                    //SchemaFieldsData schemaFieldData = coreService.Client.ReadSchemaFields(schemaID, true, new ReadOptions());
                    //Fields fields = Fields.ForContentOf(schemaFieldData);
                    //fields = Fields.ForMetadataOf(schemaFieldData, data);
                    //fields["title"].Value = "Value of MetadataField";
                    //fields["para"]["subheading"].Value = "Value of MetadataField";
                    //componentData.Metadata = fields.ToString();
                    componentData = (ComponentData)coreService.Client.Update(componentData, new ReadOptions());
                }
                else
                {
                    componentData = (ComponentData)(coreService.Client.SynchronizeWithSchema(
                                                                        componentData,
                                                                        new SynchronizeOptions
                                                                        {
                                                                            SynchronizeFlags = SynchronizeFlags.All
                                                                        }
                                                                        ).SynchronizedItem);
                    componentData = (ComponentData)coreService.Client.Create(componentData, new ReadOptions());

                    
                }
                return componentData.Id.ToString();
            }
            catch (Exception ex)
            {
                
                return "";
            }
        }
        #endregion

        public static void Publish(string targetTcmId, string targets, ICoreServiceFrameworkContext coreService)
        {
            var pubData = new PublishInstructionData
            {
                ResolveInstruction = new ResolveInstructionData() { IncludeChildPublications = false },
                RenderInstruction = new RenderInstructionData()
            };


            //get the component info
            var component = coreService.Client.Read(targetTcmId, null) as ComponentData;
            var componentId = component.Id;

            //strip out the version number if it contains onercmd
            if (component.Id.Contains("v"))
            {
                componentId = component.Id.Substring(0, component.Id.LastIndexOf('-'));
            }

            //publish to the target supplied into this method on low priority
            try
            {
                string[] pubid = ConfigurationManager.AppSettings["PublicationID"].ToString().Split(',');
                string[] pubtargets = targets.ToString().Split(',');
                foreach (var item in pubid)
                {
                    string componentIdTcm = componentId.Substring(componentId.IndexOf("tcm:"), componentId.IndexOf("-"));
                    string publicationid = "tcm:" + item;
                    foreach (var target in pubtargets)
                    {
                        coreService.Client.Publish(new[] { componentId.Replace(componentIdTcm, publicationid) }, pubData, new[] { target.ToString() }, PublishPriority.High, null);

                    }

                }

            }
            catch (Exception ex)
            {

            }

        }
        private static String UpdateNodesWithDefaultNamespace(String xml, String defaultNamespace)
        {
            try
            {
                if (!String.IsNullOrEmpty(xml) && !String.IsNullOrEmpty(defaultNamespace))
                {
                    int currentIndex = 0;


                    //find index of tag opening character
                    int tagOpenIndex = "<CoreServiceSchema".Length;

                    //no more tag openings are found
                    if (tagOpenIndex == -1)
                    {
                        return "";
                    }

                    //if it's a closing tag
                    if (xml[tagOpenIndex + 1] == '/')
                    {
                        currentIndex = tagOpenIndex + 1;
                    }
                    else
                    {
                        currentIndex = tagOpenIndex;
                    }

                    //find corresponding tag closing character
                    int tagCloseIndex = xml.IndexOf('>', tagOpenIndex);
                    if (tagCloseIndex <= tagOpenIndex)
                    {
                        throw new Exception("Invalid XML file.");
                    }

                    //look for a colon within currently processed tag
                    String currentTagSubstring = xml.Substring(tagOpenIndex, tagCloseIndex - tagOpenIndex);
                    int firstSpaceIndex = currentTagSubstring.IndexOf(' ');
                    int nameSpaceColonIndex;
                    //if space was found
                    if (firstSpaceIndex != -1)
                    {
                        //look for namespace colon between tag open character and the first space character
                        nameSpaceColonIndex = currentTagSubstring.IndexOf(':', 0, firstSpaceIndex);
                    }
                    else
                    {
                        //look for namespace colon between tag open character and tag close character
                        nameSpaceColonIndex = currentTagSubstring.IndexOf(':');
                    }

                    //if there is no namespace
                    if (nameSpaceColonIndex == -1)
                    {
                        //insert namespace after tag opening characters '<' or '</'
                        xml = xml.Insert(currentIndex + 1, String.Format("{0} ", defaultNamespace));
                    }

                    //look for next tags after current tag closing character
                    currentIndex = tagCloseIndex;

                }

                return xml;
            }
            catch (Exception ex)
            {

                return "";
            }

        }
        #region Get New Blank Component Property
        public static ComponentData GetNewComponent(string folderUri, string schemaUri, EnumType.SchemaType schemaType, string title = null)
        {

            return new ComponentData
            {
                LocationInfo = new LocationInfo
                {
                    OrganizationalItem = new LinkToOrganizationalItemData
                    {
                        IdRef = folderUri
                        // WebDavUrl  = folderUri

                    },
                },

                ComponentType = schemaType == EnumType.SchemaType.Multimedia ? ComponentType.Multimedia : ComponentType.Normal,
                Title = title,

                Schema = new LinkToSchemaData
                {
                    IdRef = schemaUri //schemaData.IdRef
                    //WebDavUrl = schemaUri
                },

                IsBasedOnMandatorySchema = false,
                IsBasedOnTridionWebSchema = true,
                ApprovalStatus = new LinkToApprovalStatusData
                {
                    IdRef = "tcm:0-0-0"
                },
                Id = "tcm:0-0-0"
            };
        }
        #endregion



    }
}
