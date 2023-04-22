using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace CosmosDbApp
{
    class Program
    {
        static void Main(string[] args)
        {
            RunCosmosDb().Wait();
        }

        public static async Task RunCosmosDb()
        {
            try
            {
                string endPoint = "https://vision-archive-db.documents.azure.com:443/";
                string key = "tR3y1YkgTAUVhLaJbCHCnmKBCCHHKYH11iQeEETISeNeyro3tQicodZEV7R82EuEqOrBLvWhFsR2ACDbFwXPnQ==";
                CosmosClient client = new CosmosClient(endPoint, key);
                AccountProperties account = await client.ReadAccountAsync();

                Console.WriteLine(account.Id);
                Console.WriteLine(account.ReadableRegions.FirstOrDefault()?.Name);

                Database database = await client.CreateDatabaseIfNotExistsAsync("cosmicworks");
                Console.WriteLine($"New Database:\tId: {database.Id}");

                Container container = await database.CreateContainerIfNotExistsAsync("products", "/categoryId", 400);
                Console.WriteLine($"New Container:\tId: {container.Id}");

                //Product saddle = new()
                //{
                //    id = "fhjfqwdhjfwedhjw123",
                //    categoryId = "jhagsdfhjgweyr2534",
                //    name ="Road saddle",
                //    price = "49.99d",
                //    tags = new string[]
                //    {
                //        "tan",
                //        "new",
                //        "crisp"
                //    }
                //};

                //await container.CreateItemAsync<Product>(saddle);


                string id = "fhjfqwdhjfwedhjw123";
                string categoryId = "jhagsdfhjgweyr2534";

                PartitionKey partitionKey = new PartitionKey(categoryId);
                Product saddle = await container.ReadItemAsync<Product>(id, partitionKey);

                Console.WriteLine($"[{saddle.id}]\t{saddle.name} ({saddle.price:C})");
                saddle.price = "99.99";
                await container.UpsertItemAsync<Product>(saddle);

                await container.DeleteItemAsync<Product>(id,partitionKey);
            }
            catch (Exception ex)
            {

            }
        }
    }
}
