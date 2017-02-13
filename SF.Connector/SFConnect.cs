using SF.Connector.com.salesforce.ap2;
using System;
using System.Configuration;
using SalesforceIntegrationWithSDLWeb8.DAL.Model;
using System.Collections.Generic;

namespace SF.Connector
{
    public class SFConnect
    {
        //Read data from SF
        public static List<SalesforceIntegrationWithSDLWeb8.DAL.Model.Lead> connectWithSFAndWriteDataInTridion()
        {
            SforceService SfdcBinding = null;
            LoginResult CurrentLoginResult = null;

            SfdcBinding = new SforceService();
            try
            {
                CurrentLoginResult = SfdcBinding.login(ConfigurationManager.AppSettings["uName"].ToString(), ConfigurationManager.AppSettings["pWD"].ToString());
                //Change the binding to the new endpoint
                SfdcBinding.Url = CurrentLoginResult.serverUrl;

                //Create a new session header object and set the session id to that returned by the login
                SfdcBinding.SessionHeaderValue = new SessionHeader();
                SfdcBinding.SessionHeaderValue.sessionId = CurrentLoginResult.sessionId;
                QueryResult queryResult = null;
                List<SalesforceIntegrationWithSDLWeb8.DAL.Model.Lead> cmsLeadData = new List<SalesforceIntegrationWithSDLWeb8.DAL.Model.Lead>();
                String SQL = "";

                SQL = "select FirstName, LastName, Phone from Lead";
                queryResult = SfdcBinding.query(SQL);
                if (queryResult.size > 0)
                {
                    //put some code in here to handle the records being returned
                    for (int i = 0; i < queryResult.size; i++)
                    {
                        com.salesforce.ap2.Lead lead = (com.salesforce.ap2.Lead)queryResult.records[i];
                        
                        SalesforceIntegrationWithSDLWeb8.DAL.Model.Lead _lead = new SalesforceIntegrationWithSDLWeb8.DAL.Model.Lead();
                        _lead.FName = new FName();
                        _lead.LName= new LName();
                        _lead.Company = new Company();
                        
                        _lead.FName.Text= lead.FirstName;
                        _lead.LName.Text = lead.LastName;
                        _lead.Company.Text = lead.Company;
                        _lead.SfID = lead.Id;

                        cmsLeadData.Add(_lead);
                    }
                }
                else
                {
                    //put some code in here to handle no records being returned
                    string message = "No records returned.";
                }
                return cmsLeadData;

            }
            catch (System.Web.Services.Protocols.SoapException e)
            {
                // This is likley to be caused by bad username or password
                SfdcBinding = null;
                throw (e);
            }
            catch (Exception e)
            {
                // This is something else, probably comminication
                SfdcBinding = null;
                throw (e);
            }
        }

        //Create a new record
        public static void connectWithTridionAndWriteDataInSF(List<SalesforceIntegrationWithSDLWeb8.DAL.Model.Lead> cmsLeadData)
        {
            SforceService SfdcBinding = null;
            LoginResult CurrentLoginResult = null;

            SfdcBinding = new SforceService();
            try
            {
                CurrentLoginResult = SfdcBinding.login(ConfigurationManager.AppSettings["uName"].ToString(), ConfigurationManager.AppSettings["pWD"].ToString());
                //Change the binding to the new endpoint
                SfdcBinding.Url = CurrentLoginResult.serverUrl;

                //Create a new session header object and set the session id to that returned by the login
                SfdcBinding.SessionHeaderValue = new SessionHeader();
                SfdcBinding.SessionHeaderValue.sessionId = CurrentLoginResult.sessionId;
                com.salesforce.ap2.Lead sfLead = new com.salesforce.ap2.Lead();

                //string firstName = "Hem";
                //string lastName = "Kant";
                //string email = "xyz@abc.com";
                //string company = "ABC Corp.";
                foreach (var item in cmsLeadData)
                {
                    sfLead.FirstName = item.FName.Text;
                    sfLead.LastName = item.LName.Text;
                    sfLead.Email = item.EmaiID.Text;
                    sfLead.Company = item.Company.Text;

                    SaveResult[] saveResults = SfdcBinding.create(new sObject[] { sfLead });

                    if (saveResults[0].success)
                    {
                        string Id = "";
                        Id = saveResults[0].id;
                    }
                    else
                    {
                        string result = "";
                        result = saveResults[0].errors[0].message;
                    }
                }
               
            }
            catch (System.Web.Services.Protocols.SoapException e)
            {
                // This is likley to be caused by bad username or password
                SfdcBinding = null;
                throw (e);
            }
            catch (Exception e)
            {
                // This is something else, probably comminication
                SfdcBinding = null;
                throw (e);
            }
        }
    }
}
