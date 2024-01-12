using Elasticsearch.Net;
using Nest;
using System;

class Program
{
    static void Main()
    {
        // Specify the Elasticsearch server connection settings
        var connectionSettings = new ConnectionSettings(new Uri("http://your-elasticsearch-server:9200"))
            .DefaultIndex("megacorp");

        // Create an ElasticClient instance
        var client = new ElasticClient(connectionSettings);

        // Define the search request
        var searchResponse = client.Search<Employee>(s => s
            .Query(q => q
                .Match(m => m
                    .Field(f => f.Last_name)
                    .Query("smith")
                )
            )
            .Aggregations(aggs => aggs
                .Terms("all_interests", terms => terms
                    .Field(f => f.Interests)
                )
            )
        );

        // Process the search response
        if (searchResponse.IsValid)
        {
            // Access search results
            var hits = searchResponse.Hits;

            // Access aggregation results
            var interestsAggregation = searchResponse.Aggregations.Terms("all_interests");
            foreach (var bucket in interestsAggregation.Buckets)
            {
                Console.WriteLine($"Interest: {bucket.Key}, Count: {bucket.DocCount}");
            }
        }
        else
        {
            Console.WriteLine($"Error: {searchResponse.OriginalException}");
        }
    }
}

// Define a class that represents your Elasticsearch document (employee in this case)
public class Employee
{
    public string Last_name { get; set; }
    public List<string> Interests { get; set; }
    // Add other properties as needed
}
