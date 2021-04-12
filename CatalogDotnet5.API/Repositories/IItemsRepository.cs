using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CatalogDotnet5.API.Entities;

namespace CatalogDotnet5.API.Repositories
{
    public interface IItemsRepository
    {
        Task<Item> GetItemAsync(Guid id);
        Task<IEnumerable<Item>> GetItemsAsync();
        Task CreateItemAsync(Item item);
        Task UpdateItemAsync(Item item);
        Task DeleteItemAsync(Guid id);
    }
}