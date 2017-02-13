
using CoreServiceClientFrameWork.CoreServiceFramework;
using SalesforceIntegrationWithSDLWeb8.DAL.Model;
using SDLWeb8.Connector;
using SF.Connector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;

namespace SalesforceIntegrationwithSDLWEB8.Client
{
  public  class Program
    {
        public static ICoreServiceFrameworkContext coreService = null;
        static void Main(string[] args)
        {
            //Read data from SF
            Console.WriteLine("Please enter 1 to connect With SF And Write Data In Tridion");
            int input = int.Parse(Console.ReadLine());
            coreService = CoreServiceFactory.GetCoreServiceContext(new Uri(ConfigurationManager.AppSettings["CoreServiceURL"].ToString()), new NetworkCredential(ConfigurationManager.AppSettings["cmsUserName"].ToString(), ConfigurationManager.AppSettings["cmsPassword"].ToString(), ConfigurationManager.AppSettings["cmsDomain"].ToString()));
           
            if (input ==1)
            {
                List<Lead> leadList = SFConnect.connectWithSFAndWriteDataInTridion();
                WriteDataInTridion.createDataInTridion(coreService, ConfigurationManager.AppSettings["SchemaID"].ToString(), leadList);
            }
            else
            {
                
                List<Lead> leadList = ReadDataFromTridion.getLeadList(coreService);
                SFConnect.connectWithTridionAndWriteDataInSF(leadList);
            }



            //Create a new record
            //if (true)
            //{
            //    coreService = CoreServiceFactory.GetCoreServiceContext(new Uri(ConfigurationManager.AppSettings["CoreServiceURL"].ToString()), new NetworkCredential(ConfigurationManager.AppSettings["cmsUserName"].ToString(), ConfigurationManager.AppSettings["cmsPassword"].ToString(), ConfigurationManager.AppSettings["cmsDomain"].ToString()));
            //    List<Lead> leadList = ReadDataFromTridion.getLeadList(coreService);
            //    SFConnect.connectWithSFAndWriteData(leadList);
            //}
        }
    }
}
