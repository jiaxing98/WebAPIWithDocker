using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SimpleWebApplication.Repositories;
using SimpleWebApplication.Models;
using SimpleWebApplication.Dtos;
using SimpleWebApplication.Interface;
using Microsoft.Extensions.Logging;

namespace SimpleWebApplication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ItemController : ControllerBase
    {
        private readonly IItemsRepository _repository;
        private readonly ILogger _logger;

        public ItemController(IItemsRepository repository, ILogger logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpPost("")]
        public async Task<ActionResult<Item>> CreateItemAsync(ClientItemDto clientItemDto)
        {
            var item = new Item()
            {
                Id = Guid.NewGuid(),
                Name = clientItemDto.Name,
                Price = clientItemDto.Price,
                CreatedDate = DateTimeOffset.UtcNow
            };

            await _repository.CreateItemAsync(item);

            return CreatedAtAction(nameof(GetItemAsync), new { id = item.Id }, item.ToDto());
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemDto>>> GetItemsAsync()
        {
            var items = (await _repository.GetItemsAsync()).Select(x => new ItemDto
            {
                Id = x.Id,
                Name = x.Name,
                Price = x.Price,
                CreatedDate = x.CreatedDate
            });

            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDto>> GetItemAsync(Guid id)
        {
            var item = await _repository.GetItemAsync(id);
            if (item == null) return NotFound();


            return item.ToDto();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateItemAsync(Guid id, ClientItemDto clientItemDto)
        {
            var existingItem = await _repository.GetItemAsync(id);
            if (existingItem == null) return NotFound();

            Item updatedItem = existingItem with
            {
                Name = clientItemDto.Name,
                Price = clientItemDto.Price
            };

            await _repository.UpdateItemAsync(updatedItem);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteItemAsync(Guid id)
        {
            var existingItem = _repository.GetItemAsync(id);
            if (existingItem == null) return NotFound();

            await _repository.DeleteItemAsync(id);

            return Ok();

        }
    }
}
