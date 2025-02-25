using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rg.integration.interfaces.qdrant;

public static class QdrantExtensions
{
    public static string ToOutputString(this List<float> vector)
    {
        return String.Join(", ", vector.ToArray());
    }

    public static ReadOnlyMemory<float> ToQdrantVector(this float[] vector, int vectorSize)
    {
        return new ReadOnlyMemory<float>(vector);
    }

    public static ReadOnlyMemory<float> ToQdrantVector(this List<float> vector, int vectorSize)
    {
        if (vector.Count < vectorSize)
        {
            vector.AddRange(Enumerable.Repeat<float>(0, vectorSize - vector.Count));
        }
        float[] paddedArray = vector.ToArray();
        ReadOnlyMemory<float> output = new ReadOnlyMemory<float>(paddedArray);
        return output;
    }
}