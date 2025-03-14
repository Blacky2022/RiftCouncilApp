﻿using Microsoft.Extensions.Caching.Memory;

namespace RiftCouncilAppLibrary.DataAccess;

public class MongoStatusData : IStatusData
{
    private readonly IMongoCollection<StatusModel> _statuses;
    private readonly IMemoryCache _cache;
    private const string cacheName = "StatusData";

    public MongoStatusData(IDbConnection db, IMemoryCache cache)
    {
        _cache = cache;
        _statuses = db.StatusCollection;
    }

    public async Task<List<StatusModel>> GetAllStatuses()
    {
        var output = _cache.Get<List<StatusModel>>(cacheName);
        if (output is null)
        {
            var results = await _statuses.FindAsync(_ => true);
            output = results.ToList();
            _cache.Set(cacheName, output, TimeSpan.FromDays(1));
        }
        return output;
    }

    public Task CreateStatus(StatusModel status)
    {
        return _statuses.InsertOneAsync(status);
    }
}