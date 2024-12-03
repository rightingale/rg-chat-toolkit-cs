using rg_chat_toolkit_api_cs.Data;
using rg_chat_toolkit_api_cs.Data.Models;

namespace rg_chat_toolkit_api_cs.Chat.Helpers;

public static class PromptChooser
{
    public static async Task<string?> ChoosePrompt (Guid tenantID, string searchQuery)
    {
        var memoryStore = await DataMethods.Prompt_EnsureEmbedding(tenantID);


        var searchEmbedding = await RG.Instance.EmbeddingModel.GetEmbedding(searchQuery);
        var searchResponse = memoryStore.Search(searchEmbedding, 10);

        //// Get the first bigram, splitting on word boundaries:
        //var searchQueryHalf = searchQuery.Split(" ").Take(2).Aggregate((a, b) => a + " " + b);
        //var searchEmbeddingHalf = await RG.Instance.EmbeddingModel.GetEmbedding(searchQueryHalf);
        //var searchResponseHalf = memoryStore.Search(searchEmbeddingHalf, 10);

        //foreach (var currentResult in searchResponse)
        //{
        //    // Find the bigram distance and add:
        //    var currentHalfResult = searchResponseHalf.Where(halfKey => halfKey.Item.ID == currentResult.Item.ID)
        //        .SingleOrDefault();

        //    Console.WriteLine("Combining " + currentResult.Item.Key + " and " + currentHalfResult.Item.Key + " for " + currentResult.Distance + " + " + currentHalfResult.Distance + " = " + (currentResult.Distance + currentHalfResult.Distance));
        
        //    if (currentHalfResult != null)
        //    {
        //        currentResult.Distance += currentHalfResult.Distance;
        //    }
        //}

        //// Print all results
        //foreach (var currentResult in searchResponse.OrderByDescending(x => x.Distance))
        //{
        //    Console.WriteLine(currentResult.Item.Key + "\t" + currentResult.Distance + "\t" + currentResult.Item.Value.Substring(0, currentResult.Item.Value.Length > 20 ? 20 : currentResult.Item.Value.Length));
        //}

        // Take top 1 by distance desc
        var topResult = searchResponse.OrderByDescending(x => x.Distance).Take(1).SingleOrDefault();
        string? promptName = null;
        if (topResult != null)
        {
            promptName = topResult.Item.Key;
        }

        Console.WriteLine("Top result: " + promptName);

        return promptName;
    }
}
