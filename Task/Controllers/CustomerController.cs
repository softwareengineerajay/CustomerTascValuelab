using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;


namespace Task.Controllers
{
    public class CustomerController : ApiController
    {
        // POST api/<controller>
        public HttpResponseMessage Post(string filename, int minAmount)
        {
            LogEvent("Attempting to Process file", "...");
            try
            {
                if (!string.IsNullOrEmpty(filename))
                {
                   
                    filename = "CustomerFile\\Customer.txt";

                    string textFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename);

                    IList<Customer> cusList = new List<Customer>();
                    Customer c = new Customer();
                    if (File.Exists(textFile))
                    {
                        LogEvent("Success", "Attempting to read  file");
                        // Read a text file line by line.  
                        string[] lines = File.ReadAllLines(textFile);
                      
                            foreach (string line in lines)
                            {
                            LogEvent("Attempting to read each message in  file", "...");
                            string[] cus = line.Split(',');

                                DateTime saleDate = Convert.ToDateTime(cus[4].Replace("[", "").Replace("]", "").Replace("(", "").Replace(")", ""));
                                decimal sales = Convert.ToDecimal(cus[3]);

                            //1. Total amount of sales is bigger than "minimum sales amount" - parameter 
                            //2. timestamp of the customer is earlier than the current date.
                            //3.  Lines that are empty or start with comment mark (#) are skipped.

                            if (!(line.StartsWith("#") || line.Count() == 0) && minAmount < Convert.ToDecimal(cus[3]) && saleDate < DateTime.Now)
                            {

                              
                                Customer customer = new Customer
                                {
                                    Id = Convert.ToInt32(cus[0]),
                                    Name = cus[2],
                                    saledate = saleDate,
                                    Sales = sales,
                                    Type = cus[1]
                                };
                                LogEvent("Success", "Those Customers whose meet given condtion" + customer.Id);
                                cusList.Add(customer);

                            }
                        }

                        //First all the companies sorted by name followed by private persons sorted by id.

                        IEnumerable<Customer> cusname = cusList.OrderBy(s => s.Name).Where(x => x.Type == "2");
                        IEnumerable<Customer> perid = cusList.OrderBy(y => y.Id).Where(y => y.Type == "1");
                        cusList = cusname.Union(perid).ToList();
                        //Insert data into database
                        LogEvent("Attempting to Inserting customer in Database", "...");
                        InsertCustomerInDB(cusList);
                        LogEvent("Successfully", "Customer are inserted in database");

                        LogEvent("Attempting to Writing  customer detail in output file", "...");
                        //Write output json into file 
                        WriteFile(cusList);
                        LogEvent("Successfully", "Writing process has been complted");
                        return Request.CreateResponse(HttpStatusCode.OK, cusList);

                    }
                    else
                    {
                        LogEvent("Error", "File Does not exist");
                        HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                        {
                            Content = new StringContent(string.Format("File Does not exist")),
                            ReasonPhrase = "File Not Found"
                        };

                        return Request.CreateResponse(resp);

                    }
                }
                else
                {
                    LogEvent("Error", "Please Provide fileName");
                    HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                    {
                        Content = new StringContent(string.Format("Please Provide fileName")),
                        ReasonPhrase = "Please Provide fileName"
                    };

                    return Request.CreateResponse(resp);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static void InsertCustomerInDB(IList<Customer> cusList)
        {
            using (Entities entities = new Entities())
            {
                foreach(var customer in cusList)
                {
                    Customer result = entities.Customers.SingleOrDefault(b => b.Id == customer.Id);
                    if (result == null)
                    {
                        entities.Customers.Add(customer);
                    }
                }
              
                entities.SaveChanges();
            }
        }

        private static void WriteFile(IList<Customer> cusList)
        {
            string outputfile = "customeroutfile.txt";
            try { 
          
            string outputFilepath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CustomerFile");
            System.IO.Directory.CreateDirectory(outputFilepath);
                outputFilepath = Path.Combine(outputFilepath,outputfile);
                // Check if file already exists. If yes, delete it.     
                if (File.Exists(outputFilepath))
                {
                    File.Delete(outputFilepath);
                }
                using (StreamWriter sw = File.CreateText(outputFilepath))
            {
                    sw.Close();
                string json = JsonConvert.SerializeObject(cusList.ToArray());
                System.IO.File.WriteAllText(outputFilepath, json);
            }
            }
            catch(Exception exe)
            {
                throw;
            }
        }

        private static void LogEvent(string eventName, string message)
        {
            string _loggerFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log", "Log.txt");
           
            

                using (StreamWriter sw = File.AppendText(_loggerFile))
            {
                sw.WriteLine(DateTime.Now + " " + eventName + ": " + message);
            }
        }

    }
}




