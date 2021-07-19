using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Data;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System.Linq;

namespace Api {
    public static class TempDataFunction {
        [FunctionName("temp")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            [CosmosDB(
                databaseName: "kunsei-iot-db",
                collectionName: "kunsei-iot-container",
                ConnectionStringSetting = "CosmosDbConnectionString")]IAsyncCollector<TempData> documentsOut,
            [CosmosDB(
                databaseName: "kunsei-iot-db",
                collectionName: "kunsei-iot-container",
                ConnectionStringSetting = "CosmosDbConnectionString")]DocumentClient client,
            ILogger log) {
            log.LogInformation("C# HTTP trigger function processed a request.");

            if (req.Method == "GET") {
                //GET http://{{host}}/api/temp?name=[‚±‚ê‚ðŽæ“¾] 
                string name = req.Query["name"];
                if (string.IsNullOrWhiteSpace(name)) {
                    return new NotFoundResult();
                }

                // Name = name‚ð‘S•”“Ç‚ñ‚Å—ˆ‚¢
                Uri collectionUri = UriFactory.CreateDocumentCollectionUri("kunsei-iot-db", "kunsei-iot-container");

                IDocumentQuery<TempData> query = client.CreateDocumentQuery<TempData>(collectionUri, new FeedOptions { EnableCrossPartitionQuery = true })
                    .Where(p => p.Name.Contains(name))
                    .AsDocumentQuery();

                var results = new List<TempData>();
                while (query.HasMoreResults) {
                    foreach (TempData result in await query.ExecuteNextAsync()) {
                        results.Add(result);
                    }
                }
                return new OkObjectResult(results);
            }
            if (req.Method == "POST") {
                string requestBody = String.Empty;
                using (StreamReader streamReader = new StreamReader(req.Body)) {
                    requestBody = await streamReader.ReadToEndAsync();
                }
                var tempData = JsonConvert.DeserializeObject<TempData>(requestBody);
                tempData.DateTime = DateTime.Now;
                await documentsOut.AddAsync(tempData);
                return new OkObjectResult(tempData);
            }
            return null;
        }
    }
}
