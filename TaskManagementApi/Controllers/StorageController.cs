using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagementApi.Data;
using TaskManagementApi.Data.Models;
using TaskManagementApi.ViewModels;

namespace TaskManagementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StorageController : ControllerBase
    {
        private readonly DatabaseContext _context;
        
        public StorageController(DatabaseContext context)
        {
            _context = context;
        }
        
        /// <summary>
        /// Получение всех складских позиций
        /// </summary>
        [Route("GetAllItems")]
        [HttpGet]
        public IActionResult GetAllItems()
        {
            try
            {
                var items = _context.StorageItems
                    .Select(i => new ItemsData()
                        {
                            ItemId = i.ItemId,
                            ItemName = i.ItemName,
                            ItemType = i.ItemType,
                            Quantity = i.Quantity
                        }
                        ).ToList();
                return Ok(new ApiResponse<List<ItemsData>>(items));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseMessage(ex.Message));
            }
        }

        /// <summary>
        /// Добавление движения товара (приход/расход)
        /// </summary>
        [Route("AddMovement")]
        [HttpPost]
        public IActionResult AddMovement([FromBody] AddMovementModel model)
        {
            try
            {
                var item = _context.StorageItems.FirstOrDefault(i => i.ItemId == model.ItemId);
                if (item == null)
                    return NotFound(new ApiResponseMessage("Деталь не найдена"));

                if (model.MovementType != "incoming" && model.MovementType != "outgoing")
                    return BadRequest(new ApiResponseMessage("Тип движения должен быть 'incoming' или 'outgoing'"));

                if (model.Quantity <= 0)
                    return BadRequest(new ApiResponseMessage("Количество должно быть больше 0"));

                if (model.MovementType == "outgoing" && item.Quantity < model.Quantity)
                    return BadRequest(new ApiResponseMessage("Недостаточно деталей на складе"));

                // Обновление количества
                item.Quantity += model.MovementType == "incoming" ? model.Quantity : -model.Quantity;

                // Создание записи движения
                var movement = new StorageMovement
                {
                    ItemId = model.ItemId,
                    MovementType = model.MovementType,
                    Quantity = model.Quantity,
                    MovementDate = DateTime.Now,
                    RelatedTaskId = model.RelatedTaskId
                };

                _context.StorageMovements.Add(movement);
                _context.SaveChanges();

                return Ok(new ApiResponseMessage("Движение успешно зарегистрировано"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseMessage(ex.Message));
            }
        }
        
        /// <summary>
        /// Получение всех движений на складе
        /// </summary>
        [HttpGet("GetAllStorageMovements")]
        public IActionResult GetAllStorageMovements()
        {
            try
            {
                var movements = _context.StorageMovements
                    .Include(m => m.Item)
                    .Include(m => m.Task)
                    .Select(m => new StorageMovementViewModel
                    {
                        MovementId = m.MovementId,
                        ItemId = m.ItemId,
                        ItemName = m.Item.ItemName,
                        MovementType = m.MovementType,
                        Quantity = m.Quantity,
                        MovementDate = m.MovementDate,
                        RelatedTaskId = m.RelatedTaskId,
                        RelatedTaskTitle = m.Task != null ? m.Task.Title : null
                    })
                    .OrderByDescending(m => m.MovementDate)
                    .ToList();

                return Ok(new ApiResponse<IEnumerable<StorageMovementViewModel>>(movements));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseMessage(ex.Message));
            }
        }
        
        /// <summary>
        /// Получение истории движения по товару
        /// </summary>
        [Route("GetItemHistory/{itemId}")]
        [HttpGet]
        public IActionResult GetItemHistory(int itemId)
        {
            try
            {
                var history = _context.StorageMovements
                    .Where(m => m.ItemId == itemId)
                    .Include(m => m.Task)
                    .OrderByDescending(m => m.MovementDate)
                    .Select(m => new MovementData
                    {
                        MovementId = m.MovementId,
                        ItemId = m.ItemId,
                        MovementType = m.MovementType,
                        Quantity = m.Quantity,
                        MovementDate = m.MovementDate,
                        RelatedTaskId = m.RelatedTaskId
                    })
                    .ToList();

                return Ok(new ApiResponse<IEnumerable<MovementData>>(history));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseMessage(ex.Message));
            }
        }
        
        /// <summary>
        /// Создание новой детали
        /// </summary>
        [Route("AddStorageItem")]
        [HttpPost]
        public IActionResult AddStorageItem([FromBody] AddStorageItemModel model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.ItemName))
                    return BadRequest(new ApiResponseMessage("Название детали не может быть пустым"));

                var item = new StorageItem
                {
                    ItemName = model.ItemName,
                    ItemType = model.ItemType,
                    Quantity = model.Quantity
                };

                _context.StorageItems.Add(item);
                _context.SaveChanges();

                return Ok(new
                {
                    message = "Деталь успешно добавлена",
                    itemId = item.ItemId
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseMessage(ex.Message));
            }
        }
        
        /// <summary>
        /// Обновление информации о детали
        /// </summary>
        [Route("update-item/{itemId}")]
        [HttpPut]
        public IActionResult UpdateStorageItem(int itemId, [FromBody] UpdateStorageItemModel model)
        {
            try
            {
                var item = _context.StorageItems.FirstOrDefault(i => i.ItemId == itemId);
                if (item == null)
                    return NotFound(new ApiResponseMessage("Деталь не найдена"));

                item.ItemName = model.ItemName ?? item.ItemName;
                item.ItemType = model.ItemType ?? item.ItemType;

                _context.SaveChanges();
                return Ok(new ApiResponseMessage("Информация о детали успешно обновлена"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseMessage(ex.Message));
            }
        }
        
        /// <summary>
        /// Удаление детали
        /// </summary>
        [Route("delete-item/{itemId}")]
        [HttpDelete]
        public IActionResult DeleteStorageItem(int itemId)
        {
            try
            {
                var item = _context.StorageItems.FirstOrDefault(i => i.ItemId == itemId);
                if (item == null)
                    return NotFound(new ApiResponseMessage("Деталь не найдена"));

                _context.StorageItems.Remove(item);
                _context.SaveChanges();
                return Ok(new ApiResponseMessage("Деталь успешно удалена"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseMessage(ex.Message));
            }
        }
    }
}