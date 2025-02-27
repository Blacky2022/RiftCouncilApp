using MongoDB.Driver;

namespace RiftCouncilAppLibrary.DataAccess
{
    public interface IDbConnection
    {
        IMongoCollection<CategoryModel> CategoryCollection { get; }
        string CategoryCollectionName { get; }
        MongoClient Client { get; }
        string Dbname { get; }
        IMongoCollection<StatusModel> StatusCollection { get; }
        string StatusCollectionName { get; }
        IMongoCollection<SuggestionModel> SuggestionCollection { get; }
        string SuggestionCollectionName { get; }
        string UserCollectionName { get; }
        IMongoCollection<UserModel> UserCollection { get; }
    }
}