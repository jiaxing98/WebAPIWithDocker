using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SimpleWebApplication.Models;

namespace SimpleWebApplication.Interface
{
    public interface IItemsRepository
    {
        Task CreateItemAsync(Item item);
        Task<Item> GetItemAsync(Guid id);
        Task<IEnumerable<Item>> GetItemsAsync();
        Task UpdateItemAsync(Item item);
        Task DeleteItemAsync(Guid id);
    }
}
