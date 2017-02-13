using CoreServiceClientFrameWork.CoreServiceFramework;
using SalesforceIntegrationWithSDLWeb8.BAL;
using SalesforceIntegrationWithSDLWeb8.DAL.Model;
using System.Collections.Generic;
using System.Configuration;
using System.Xml;
using Tridion.ContentManager.CoreService.Client;

namespace SDLWeb8.Connector
{
    public class ReadDataFromTridion
    {
        public static List<Lead> getLeadList(ICoreServiceFrameworkContext coreService)
        {
            SchemaFieldsData schemaFieldData = coreService.Client.ReadSchemaFields(ConfigurationManager.AppSettings["SchemaID"].ToString(), true, new ReadOptions());
            UsingItemsFilterData f = new UsingItemsFilterData { ItemTypes = new[] { ItemType.Component } };
            IdentifiableObjectData[] items = coreService.Client.GetList(ConfigurationManager.AppSettings["SchemaID"].ToString(), f);
            List<Lead> newLeadList = new List<Lead>();
            XmlDocument Xdoc = new XmlDocument();
            foreach (var item in items)
            {
                Lead _lead = new Lead();
                ComponentData component = (ComponentData)coreService.Client.Read(item.Id, new ReadOptions());
                Xdoc.LoadXml(component.Content);
                _lead = DataTransformation.Deserialize<Lead>(Xdoc);
                newLeadList.Add(_lead);
                //Do something
            }
            return newLeadList;
        }
    }
}
