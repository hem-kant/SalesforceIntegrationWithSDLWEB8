using CoreServiceClientFrameWork.CoreServiceFramework;
using CoreServiceWithWeb8.DAL.Common;
using CoreServiceWithWeb8.TridionObjects;
using SalesforceIntegrationWithSDLWeb8.BAL;
using SalesforceIntegrationWithSDLWeb8.DAL.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDLWeb8.Connector
{
    public  class WriteDataInTridion
    {
        public static void createDataInTridion(ICoreServiceFrameworkContext coreService, string SchemaId, List<Lead> leadFromSF)
        {
            try
            {
                foreach (var item in leadFromSF)
                {
                    string serializeXml = "";
                    bool bln = DataTransformation.Serialize<Lead>(item, ref serializeXml);
                    string xml = serializeXml;
                    string tcmuri = TridionComponent.GenerateComponent(coreService, xml, SetPublication.Publication(ConfigurationManager.AppSettings["FolderLocation"].ToString(), SchemaId), EnumType.SchemaType.Component, ConfigurationManager.AppSettings["FolderLocation"].ToString(), item.FName.Text + " " + item.LName.Text, item.FName.Text + " " + item.LName.Text);
                }
               
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
