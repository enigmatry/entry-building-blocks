using Azure.Search.Documents.Indexes.Models;

namespace Enigmatry.Entry.AzureSearch;
public static class DefaultVectorSearch
{
    public const int Dimension = 3072;
    public const string ProfileName = "vector_search_profile";
    private const string AlgorithmConfigurationName = "algorithm_configuration";

    public static VectorSearch Create()
    {
        var algorithm = new HnswAlgorithmConfiguration(AlgorithmConfigurationName)
        {
            Parameters = new HnswParameters { M = 4, EfConstruction = 400, EfSearch = 500 }
        };

        var profile = new VectorSearchProfile(ProfileName, AlgorithmConfigurationName);

        return new VectorSearch
        {
            Algorithms = { algorithm },
            Profiles = { profile }
        };
    }
}

