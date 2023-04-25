using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using static Microsoft.Azure.Cosmos.Container;
//using Bogus;
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
                string endPoint = "https://cosmosdb255.documents.azure.com:443/";
                string key = "123==";
                CosmosClient client = new CosmosClient(endPoint, key);
                AccountProperties account = await client.ReadAccountAsync();

                Console.WriteLine(account.Id);
                Console.WriteLine(account.ReadableRegions.FirstOrDefault()?.Name);

                //Database database = await client.CreateDatabaseIfNotExistsAsync("cosmicworks");
                //Console.WriteLine($"New Database:\tId: {database.Id}");
                Container sourceContainer =  client.GetContainer("cosmicworks", "products");
                Container leaseContainer = client.GetContainer("cosmicworks", "productslease");
                //Container container = await database.CreateContainerIfNotExistsAsync("products", "/categoryId", 400);
                //Console.WriteLine($"New Container:\tId: {container.Id}");

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


                //string id = "fhjfqwdhjfwedhjw123";
                //string categoryId = "jhagsdfhjgweyr2534";

                //PartitionKey partitionKey = new PartitionKey(categoryId);
                //Product saddle = await container.ReadItemAsync<Product>(id, partitionKey);

                //Console.WriteLine($"[{saddle.id}]\t{saddle.name} ({saddle.price:C})");
                //saddle.price = "99.99";
                //await container.UpsertItemAsync<Product>(saddle);

                //await container.DeleteItemAsync<Product>(id,partitionKey);

                //Product saddle = new("0120", "Roman Saddle", "jhvgahjdghjagfdjga56785");
                //Product handleBar = new("012A", "Rolex handleBar", "jhvgahjdghjagfdjga56785");
                //PartitionKey partitionKey = new("jhvgahjdghjagfdjga56785");

                //TransactionalBatch batch = container.CreateTransactionalBatch(partitionKey)
                //    .CreateItem<Product>(saddle)
                //    .CreateItem<Product>(handleBar);
                //using TransactionalBatchResponse response = await batch.ExecuteAsync();

                //Console.WriteLine($"Status:\t{response.StatusCode}");

                //List<Product> productsToInsert = new Faker<Product>()
                //  .StrictMode(true)
                //  .RuleFor(o => o.id, f => Guid.NewGuid().ToString())
                //  .RuleFor(o => o.name, f => f.Commerce.ProductName())
                //  .RuleFor(o => o.price, f => Convert.ToDouble(f.Commerce.Price(max: 1000, min: 10)))
                //  .RuleFor(o => o.categoryId, f => f.Commerce.Department(1))
                //  .Generate(1000);

                //List<Task> concurrentTask = new List<Task>();
                //foreach (Product product in productsToInsert)
                //{
                //    concurrentTask.Add(
                //        container.CreateItemAsync(product, new PartitionKey(product.categoryId))
                //        );
                //}

                //await Task.WhenAll(concurrentTask);



                //string sql = "Select * from products p";
                //QueryDefinition query = new QueryDefinition(sql);

                //using FeedIterator<Product> feedIterator = container.GetItemQueryIterator<Product>(query);
                //while (feedIterator.HasMoreResults)
                //{
                //    FeedResponse<Product> response = await feedIterator.ReadNextAsync();
                //    foreach(Product product in response)
                //    {
                //        Console.WriteLine("Id {0}", product.id);
                //        Console.WriteLine("CategoryId {0}", product.categoryId);
                //        Console.WriteLine("name {0}", product.name);
                //    }
                //}
                //Code to Detect any changes in the database
                ChangesHandler<Product> handleChanges = async (
                    IReadOnlyCollection<Product> changes,
                        CancellationToken cancellationToken
                ) =>
                {
                    //Console.WriteLine()
                    foreach (Product product in changes)
                    {
                        await Console.Out.WriteLineAsync($"Detected Operations:\t[{product.id}]\t[{product.name}]");
                    }
                };
                var builder = sourceContainer.GetChangeFeedProcessorBuilder<Product>(
                    processorName: "productsProcessor",
                    onChangesDelegate: handleChanges
                    );
                ChangeFeedProcessor processor = builder
                    .WithInstanceName("consoleApp")
                    .WithLeaseContainer(leaseContainer)
                    .Build();
                await processor.StartAsync();
                Console.WriteLine($"Run\tListening for changes");
                Console.WriteLine("Process any key to Stop");
                Console.ReadKey();

                await processor.StopAsync();
            }
            catch (Exception ex)
            {

            }
        }
    }
}
