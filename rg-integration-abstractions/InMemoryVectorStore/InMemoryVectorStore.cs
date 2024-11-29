using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rg_integration_abstractions.InMemoryVector;

public class InMemoryVectorStore
{
    private List<KeyValueItem> vectorStore = new List<KeyValueItem>();

    public InMemoryVectorStore()
    {
    }

    public void Add(KeyValueItem item)
    {
        vectorStore.Add(item);
    }

    public void Add (string key, string value, float[] vector)
    {
        var keyValueItem = new KeyValueItem { Key = key, Value = value, ValueEmbedding = vector };
        vectorStore.Add(keyValueItem);
    }

    public class KeyValueItem
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public float[] ValueEmbedding { get; set; }
    }

    public class SearchResponse
    {
        public KeyValueItem Item{ get; set; }
        public double Distance { get; set; }
    }

    public SearchResponse[] Search(float[] query, int topN)
    {
        var comparison = new List<SearchResponse>();
        foreach (var currentSearchItem in this.vectorStore)
        {
            var distance = CosineSimilarity(query, currentSearchItem.ValueEmbedding);
            comparison.Add(new SearchResponse { Item = currentSearchItem, Distance = distance });
        }

        // Find the most similar vector
        var mostSimilar = comparison
            .OrderByDescending(x => x.Distance)
            .Take(topN)
            .ToArray();

        return mostSimilar.ToArray();
    }

    static double CosineSimilarity(float[] vectorA, float[] vectorB)
    {
        double dotProduct = vectorA.Zip(vectorB, (a, b) => a * b).Sum();
        double magnitudeA = Math.Sqrt(vectorA.Sum(a => a * a));
        double magnitudeB = Math.Sqrt(vectorB.Sum(b => b * b));
        return dotProduct / (magnitudeA * magnitudeB);
    }

}
