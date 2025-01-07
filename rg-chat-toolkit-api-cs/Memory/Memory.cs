using Amazon.Polly;
using Microsoft.AspNetCore.Mvc;
using rg_chat_toolkit_api_cs.Cache;
using rg_chat_toolkit_api_cs.Chat.Helpers;
using rg_chat_toolkit_api_cs.Data;
using rg_chat_toolkit_cs.Cache;
using rg_chat_toolkit_cs.Chat;
using rg_chat_toolkit_cs.Speech;
using rg_integration_abstractions.Tools.Memory;
using System.Diagnostics;
using System.Text;

namespace rg_chat_toolkit_api_cs.Chat;

public class MemoryItemUpdateRequest : RequestBase
{
    public string MemoryName { get; set; }
    public string MemoryItemID { get; set; }
    public string Value { get; set; }
    public string Json { get; set; }

    public MemoryItemUpdateRequest()
    {
        MemoryName = "";
        MemoryItemID = "";
        Value = "";
        Json = "";
    }

    public MemoryItemUpdateRequest(string memoryName, string memoryItemID, string value, string json)
    {
        MemoryName = memoryName;
        MemoryItemID = memoryItemID;
        Value = value;

        Json = json;
    }
}


public class MemoryItemSearchRequest : RequestBase
{
    public string MemoryName { get; set; }
    public string SearchText { get; set; }

    public MemoryItemSearchRequest()
    {
        MemoryName = "";
        SearchText = "";
    }

    public MemoryItemSearchRequest(string memoryName, string searchText)
    {
        MemoryName = memoryName;
        SearchText = searchText;
    }
}



[Route("[controller]")]
[ApiController]
public class MemoryController : ControllerBase
{
    protected readonly IRGEmbeddingCache EmbeddingCache;

    public MemoryController(IRGEmbeddingCache embeddingCache)
    {
        this.EmbeddingCache = embeddingCache;
    }

    [HttpGet]
    [Route("Item")]
    public async Task<IActionResult> MemoryItem_Search([FromQuery] MemoryItemSearchRequest request)
    {
        if (request.MemoryName == null)
        {
            throw new ApplicationException("Memory name is required.");
        }

        var memory = DataMethods.Memory_Get(request.TenantID, request.MemoryName);
        if (memory == null)
        {
            throw new ApplicationException($"Memory {request.MemoryName} not found.");
        }

        const string MEMORY_TYPE_VECTOR = "vector";
        if (memory.MemoryType?.ToLower()?.StartsWith(MEMORY_TYPE_VECTOR) ?? false)
        {
            VectorStoreMemory vectorStoreMemory = new GenericVectorStoreMemory(memory.Name, this.EmbeddingCache);
            var result = await vectorStoreMemory.Search(request.SearchText);

            return Ok(result);
        }

        return Ok();
    }

    [HttpPost]
    [Route("Item")]
    public async Task<IActionResult> MemoryItem_Update([FromBody] MemoryItemUpdateRequest request)
    {
        if (request.MemoryName == null)
        {
            throw new ApplicationException("Memory name is required.");
        }

        var memory = DataMethods.Memory_Get(request.TenantID, request.MemoryName);
        if (memory == null)
        {
            throw new ApplicationException($"Memory {request.MemoryName} not found.");
        }

        const string MEMORY_TYPE_VECTOR = "vector";
        if (memory.MemoryType?.ToLower()?.StartsWith(MEMORY_TYPE_VECTOR) ?? false)
        {
            VectorStoreMemory vectorStoreMemory = new GenericVectorStoreMemory(memory.Name, this.EmbeddingCache);
            await vectorStoreMemory.Add(request.MemoryItemID, request.Value, request.Json);
        }

        return Ok();
    }

}
