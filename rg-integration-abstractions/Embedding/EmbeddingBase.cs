using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rg_integration_abstractions.Embedding;

public abstract class EmbeddingBase
{
    public abstract Task<float[]?> GetEmbedding(string text);

    public abstract int EmbeddingSize { get; }

}

public class EmbeddingResponse
{
    public List<EmbeddingData>? Data { get; set; }
}

public class EmbeddingData
{
    public float[]? Embedding { get; set; }
}
