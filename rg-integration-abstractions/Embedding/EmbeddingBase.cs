using rg_chat_toolkit_cs.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rg_integration_abstractions.Embedding;

public abstract class EmbeddingBase
{
    protected IRGEmbeddingCache? cache;

    internal EmbeddingBase(IRGEmbeddingCache? cache)
    {
        this.cache = cache;
    }

    public abstract Task<float[]> GetEmbedding(string text);

    public string VectorName
    {
        get
        {
            const string VECTOR_NAME_PATTERN = "embedding_{0}_{1}";
            string vectorName = String.Format(VECTOR_NAME_PATTERN, this.EmbeddingModel, this.EmbeddingSize);
            return vectorName;
        }
    }

    public abstract int EmbeddingSize { get; }
    public abstract string EmbeddingModel { get; }

}

public class EmbeddingResponse
{
    public List<EmbeddingData>? Data { get; set; }
}

public class EmbeddingData
{
    public float[]? Embedding { get; set; }
}
