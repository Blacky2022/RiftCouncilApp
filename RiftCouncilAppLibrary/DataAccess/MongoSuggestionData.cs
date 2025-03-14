﻿using Microsoft.Extensions.Caching.Memory;

namespace RiftCouncilAppLibrary.DataAccess
{
    public class MongoSuggestionData : ISuggestionData
    {
        private readonly IDbConnection db;
        private readonly IUserData userData;
        private readonly IMemoryCache cache;
        private readonly IMongoCollection<SuggestionModel> suggestions;
        private const string CacheName = "SuggestionData";

        public MongoSuggestionData(IDbConnection db, IUserData userData, IMemoryCache cache)
        {
            this.db = db;
            this.userData = userData;
            this.cache = cache;
            this.suggestions = db.SuggestionCollection;
        }

        public async Task<List<SuggestionModel>> GetAllSuggestions()
        {
            var output = cache.Get<List<SuggestionModel>>(CacheName);
            if (output is null)
            {
                var results = await suggestions.FindAsync(s => s.Archived == false);
                output = results.ToList();
                cache.Set(CacheName, output, TimeSpan.FromMinutes(1));
            }
            return output;
        }

        public async Task<List<SuggestionModel>> GetUserSuggestions(string userId)
        {
            var output = this.cache.Get<List<SuggestionModel>>(userId);
            if (output is null)
            {
                var results = await this.suggestions.FindAsync(s => s.Author.Id == userId);
                output = results.ToList();
                this.cache.Set(userId, output, TimeSpan.FromMinutes(1));
            }
            return output;
        }

        public async Task<List<SuggestionModel>> GetAllApprovedSuggestions()
        {
            var output = await GetAllSuggestions();
            return output.Where(x => x.ApprovedForRelease).ToList();
        }

        public async Task<SuggestionModel> GetSuggestion(string id)
        {
            var results = await this.suggestions.FindAsync(s => s.Id == id);
            return results.FirstOrDefault();
        }
        public async Task<List<SuggestionModel>> GetAllSuggestionsWaitingForApproval()
        {
            var output = await GetAllSuggestions();
            return output.Where(x =>
                x.ApprovedForRelease == false
                && x.Rejected == false).ToList();
        }

        public async Task UpdateSuggestion(SuggestionModel suggestion)
        {
            await this.suggestions.ReplaceOneAsync(s => s.Id == suggestion.Id, suggestion);
            this.cache.Remove(CacheName);
        }

        public async Task UpVoteSuggestion(string suggestionId, string userId)
        {
            var client = this.db.Client;

            using var session = await client.StartSessionAsync();
            session.StartTransaction();

            try
            {
                var db = client.GetDatabase(this.db.Dbname);
                var suggestionsInTransaction = db.GetCollection<SuggestionModel>(this.db.SuggestionCollectionName);
                var suggestion = (await suggestionsInTransaction.FindAsync(S => S.Id == suggestionId)).First();

                bool isUpvote = suggestion.UserVotes.Add(userId);

                if (isUpvote == false)
                {
                    suggestion.UserVotes.Remove(userId);
                }

                await suggestionsInTransaction.ReplaceOneAsync(s => s.Id == suggestionId, suggestion);

                var usersInTransaction = db.GetCollection<UserModel>(this.db.UserCollectionName);
                var user = await this.userData.GetUser(userId);

                if (isUpvote)
                {
                    user.VotedonSuggestions.Add(new BasicSuggestionModel(suggestion));
                }
                else
                {
                    var suggestionToRemove = user.VotedonSuggestions.Where(s => s.Id == suggestionId).First();
                    user.VotedonSuggestions.Remove(suggestionToRemove);
                }
                await usersInTransaction.ReplaceOneAsync(u => u.Id == user.Id, user);

                await session.CommitTransactionAsync();

                this.cache.Remove(CacheName);
            }
            catch (Exception ex)
            {
                await session.AbortTransactionAsync();
                throw;
            }
        }

        public async Task CreateSuggestion(SuggestionModel suggestion)
        {
            var client = this.db.Client;

            using var session = await client.StartSessionAsync();

            session.StartTransaction();

            try
            {
                var db = client.GetDatabase(this.db.Dbname);
                var suggestionsInTransaction = db.GetCollection<SuggestionModel>(this.db.SuggestionCollectionName);
                await suggestionsInTransaction.InsertOneAsync(suggestion);

                var usersInTransaction = db.GetCollection<UserModel>(this.db.UserCollectionName);
                var user = await this.userData.GetUser(suggestion.Author.Id);
                user.AuthoredSuggestions.Add(new BasicSuggestionModel(suggestion));
                await usersInTransaction.ReplaceOneAsync(u => u.Id == user.Id, user);

                await session.CommitTransactionAsync();

            }
            catch (Exception ex)
            {
                await session.AbortTransactionAsync();
                throw;
            }
        }
    }
}
