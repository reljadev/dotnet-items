using Microsoft.AspNetCore.Mvc;
using Play.Common;
using Play.Inventory.Service.Dtos;
using Play.Inventory.Service.Entities;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Play.Inventory.Service.Controllers {
    [ApiController]
    [Route("items")]
    public class ItemsController : ControllerBase {
        private readonly IRepository<InventoryItem> inventoryItemsRepository;
        private readonly IRepository<CatalogItem> catalogItemsRepository;
        private IHttpContextAccessor _httpContextAccessor;

        public ItemsController(IRepository<InventoryItem> inventoryItemsRepository, 
                                IRepository<CatalogItem> catalogItemsRepository,
                                IHttpContextAccessor httpContextAccessor) {
            this.inventoryItemsRepository = inventoryItemsRepository;
            this.catalogItemsRepository = catalogItemsRepository;
            this._httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        [Authorize(Policy = "MatchingUserId")]
        public async Task<ActionResult<IEnumerable<InventoryItemDto>>> GetAsync(Guid userId) {
            if(userId == Guid.Empty) {
                return BadRequest();
            }

            var inventoryItemEntities = await inventoryItemsRepository.GetAllAsync(item => item.UserId == userId);
            var itemIds = inventoryItemEntities.Select(item => item.CatalogItemId);
            var catalogItemEntities = await catalogItemsRepository.GetAllAsync(item => itemIds.Contains(item.Id));

            var inventoryItemDtos = inventoryItemEntities.Select(inventoryItem => {
                var catalogItem = catalogItemEntities.Single(catalogItem => catalogItem.Id == inventoryItem.CatalogItemId);
                return inventoryItem.AsDto(catalogItem.Name, catalogItem.Description);
            });

            return Ok(inventoryItemDtos);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> PostAsync(GrantItemsDto grantItemsDto) {
            var userId = new Guid(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

            var inventoryItem = await inventoryItemsRepository.GetAsync(item => item.UserId == userId &&
                                                                        item.CatalogItemId == grantItemsDto.CatalogItemId);

            if(inventoryItem == null) {
                inventoryItem = new InventoryItem {
                    CatalogItemId = grantItemsDto.CatalogItemId,
                    UserId = userId,
                    Quantity = grantItemsDto.Quantity,
                    AcquiredDate = DateTimeOffset.UtcNow
                };

                await inventoryItemsRepository.CreateAsync(inventoryItem);
            } else {
                inventoryItem.Quantity += grantItemsDto.Quantity;
                await inventoryItemsRepository.UpdateAsync(inventoryItem);
            }

            return Ok();
        }
    }
}