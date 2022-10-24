using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using SimpleWebApplication.Context;
using SimpleWebApplication.Interface;
using SimpleWebApplication.Models;

namespace SimpleWebApplication.Repositories
{
    public class ItemsRepository : IItemsRepository
    {
        private readonly IMongoDBContext _mongoContext;
        private readonly IMongoCollection<Item> _collection;

        private readonly FilterDefinitionBuilder<Item> _builder = Builders<Item>.Filter;

        public ItemsRepository(IMongoDBContext context)
        {
            _mongoContext = context;
            _collection = _mongoContext.GetCollection<Item>("Items");
        }

        public async Task CreateItemAsync(Item item)
        {
            await _collection.InsertOneAsync(item);
        }

        public async Task<IEnumerable<Item>> GetItemsAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task<Item> GetItemAsync(Guid id)
        {
            var filter = _builder.Eq(x => x.Id, id);
            return await _collection.Find(filter).SingleOrDefaultAsync();
        }

        public async Task UpdateItemAsync(Item item)
        {
            var filter = _builder.Eq(x => x.Id, item.Id);
            await _collection.ReplaceOneAsync(filter, item);
        }

        public async Task DeleteItemAsync(Guid id)
        {
            var filter = _builder.Eq(x => x.Id, id);
            await _collection.DeleteOneAsync(filter);
        }
    }
}
