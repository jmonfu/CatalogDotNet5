using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CatalogDotnet5.API.Dtos;
using CatalogDotnet5.API.Entities;
using CatalogDotnet5.API.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CatalogDotnet5.API.Controllers
{
    [ApiController]
    [Route("items")]
    public class ItemsController : ControllerBase
    {
        private readonly IItemsRepository _repository;
        private readonly ILogger _logger;

        public ItemsController(IItemsRepository repository, ILogger logger)
        {
            _repository =  repository;
            _logger = logger;
        }

        //GET/Items/
        [HttpGet]
        public async Task<IEnumerable<ItemDto>> GetItemsAsync(string name = null)
        {
            //we use the Extension method here (item.AsDto())
            var items = (await _repository.GetItemsAsync()).Select(item => item.AsDto()); 
            if (!string.IsNullOrWhiteSpace(name))
            {
                items = items.Where(item => item.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
            }
            return items;
        }

        // GET/Items/{Id} 
        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDto>> GetItemAsync(Guid id)
        {
            var item = await _repository.GetItemAsync(id);

            if (item is null)
            {
                return NotFound();
            }

            //we use the Extension method here too (item.AsDto())
            return item.AsDto();
        }

        //POST/Items
        [HttpPost]
        public async Task<ActionResult<ItemDto>> CreateItemAsync(CreateItemDto itemDto)
        {
            Item item= new(){
                Id = Guid.NewGuid(),
                Name = itemDto.Name,
                Description = itemDto.Description,
                Price = itemDto.Price,
                CreatedDate = DateTimeOffset.UtcNow
            };

            await _repository.CreateItemAsync(item);

            return CreatedAtAction(nameof(GetItemAsync), new {id = item.Id}, item.AsDto());
        }

        //PUT/Items/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateItemAsync(Guid id, UpdateItemDto itemDto)
        {
            var existingItem = await _repository.GetItemAsync(id);

            if (existingItem is null)
            {
                return NotFound();
            }

            existingItem.Name = itemDto.Name;
            existingItem.Price = itemDto.Price;

            await _repository.UpdateItemAsync(existingItem);

            return NoContent();
        }

        //DELETE/Items/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteItemAsync(Guid id)
        {
            var existingItem = await _repository.GetItemAsync(id);

            if (existingItem is null)
            {
                return NotFound();
            }

            await _repository.DeleteItemAsync(existingItem.Id);

            return NoContent();
        }

    }
}