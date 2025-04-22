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
    public class DeviceController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public DeviceController(DatabaseContext context)
        {
            _context = context;
        }
        
                /// <summary>
        /// Получение всех устройств
        /// </summary>
        [Route("GetAllDevices")]
        [HttpGet]
        public IActionResult GetAllDevices()
        {
            try
            {
                var devices = _context.Devices
                    .Select(d => new DeviceData()
                        {
                            DeviceId = d.DeviceId,
                            DeviceName = d.DeviceName,
                            DeviceModel = d.DeviceModel
                        }
                    ).ToList();
                return Ok(new ApiResponse<List<DeviceData>>(devices));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseMessage(ex.Message));
            }
        }

        /// <summary>
        /// Получение компонентов устройства
        /// </summary>
        [Route("GetDeviceComponents/{deviceId}")]
        [HttpGet]
        public IActionResult GetDeviceComponents(int deviceId)
        {
            try
            {
                var device = _context.Devices.FirstOrDefault(d => d.DeviceId == deviceId);
                if (device == null)
                    return NotFound(new ApiResponseMessage("Устройство не найдено"));

                var components = _context.DeviceComponents
                    .Where(c => c.DeviceId == deviceId)
                    .Select(c => new ComponentData
                    {
                        ComponentId = c.ComponentId,
                        ComponentName = c.ComponentName,
                        DeviceId = c.DeviceId,
                        Items = _context.ComponentItems
                            .Where(ci => ci.ComponentId == c.ComponentId)
                            .Include(ci => ci.Item)
                            .Select(ci => new ComponentItemData
                            {
                                ItemId = ci.Item.ItemId,
                                ItemName = ci.Item.ItemName,
                                ItemType = ci.Item.ItemType,
                                QuantityOnStorage = ci.Item.Quantity,
                                QuantityNeeded = ci.QuantityNeeded
                            }).ToList()
                    })
                    .ToList();

                return Ok(new
                {
                    body = components,
                    message = "Ok"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseMessage(ex.Message));
            }
        }

        /// <summary>
        ///  Добавление нового устройства
        /// </summary>
        [Route("AddDeviceWithComponents")]
        [HttpPost]
        public IActionResult AddDeviceWithComponents([FromBody] AddDeviceWithComponentIdsModel model)
        {
            try
            {
                // Создание устройства
                var device = new Device
                {
                    DeviceName = model.DeviceName,
                    DeviceModel = model.DeviceModel
                };
                _context.Devices.Add(device);
                _context.SaveChanges();

                // Обновляем компоненты: присваиваем device_id
                foreach (var compId in model.ComponentIds)
                {
                    var component = _context.DeviceComponents.FirstOrDefault(c => c.ComponentId == compId);
                    if (component != null)
                    {
                        component.DeviceId = device.DeviceId;
                    }
                }

                _context.SaveChanges();
                return Ok(new ApiResponseMessage("Устройство создано и компоненты привязаны"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseMessage(ex.Message));
            }
        }
        
        /// <summary>
        /// Добавление нового компонента
        /// </summary>
        [Route("AddComponentWithItems")]
        [HttpPost]
        public IActionResult AddComponentWithItems([FromBody] AddComponentModel model)
        {
            try
            {
                // Проверка существования устройства
                var device = _context.Devices.FirstOrDefault(d => d.DeviceId == model.DeviceId);
                if (device == null)
                    return NotFound(new ApiResponseMessage("Устройство не найдено"));

                // Создаём компонент
                var component = new DeviceComponent
                {
                    DeviceId = model.DeviceId,
                    ComponentName = model.ComponentName
                };
                _context.DeviceComponents.Add(component);
                _context.SaveChanges();

                // Привязываем детали
                foreach (var item in model.Items)
                {
                    _context.ComponentItems.Add(new ComponentItem
                    {
                        ComponentId = component.ComponentId,
                        ItemId = item.ItemId,
                        QuantityNeeded = item.QuantityNeeded
                    });
                }

                _context.SaveChanges();
                return Ok(new ApiResponseMessage("Компонент успешно добавлен и детали назначены"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseMessage(ex.Message));
            }
        }
        
        /// <summary>
        /// Обновление информации о приборе
        /// </summary>
        [Route("update-device/{deviceId}")]
        [HttpPut]
        public IActionResult UpdateDevice(int deviceId, [FromBody] UpdateDeviceModel model)
        {
            try
            {
                var device = _context.Devices.FirstOrDefault(d => d.DeviceId == deviceId);
                if (device == null)
                    return NotFound(new ApiResponseMessage("Устройство не найдено"));

                device.DeviceName = model.DeviceName ?? device.DeviceName;
                device.DeviceModel = model.DeviceModel ?? device.DeviceModel;

                _context.SaveChanges();
                return Ok(new ApiResponseMessage("Информация о приборе успешно обновлена"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseMessage(ex.Message));
            }
        }
        
        /// <summary>
        /// Обновление информации о компоненте
        /// </summary>
        [Route("update-component/{componentId}")]
        [HttpPut]
        public IActionResult UpdateComponent(int componentId, [FromBody] UpdateComponentModel model)
        {
            try
            {
                var component = _context.DeviceComponents.FirstOrDefault(c => c.ComponentId == componentId);
                if (component == null)
                    return NotFound(new ApiResponseMessage("Компонент не найден"));

                component.ComponentName = model.ComponentName ?? component.ComponentName;
                component.DeviceId = model.DeviceId ?? component.DeviceId;

                _context.SaveChanges();
                return Ok(new ApiResponseMessage("Информация о компоненте успешно обновлена"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseMessage(ex.Message));
            }
        }
        
        /// <summary>
        /// Удаление прибора
        /// </summary>
        [Route("delete-device/{deviceId}")]
        [HttpDelete]
        public IActionResult DeleteDevice(int deviceId)
        {
            try
            {
                var device = _context.Devices.FirstOrDefault(d => d.DeviceId == deviceId);
                if (device == null)
                    return NotFound(new ApiResponseMessage("Устройство не найдено"));

                _context.Devices.Remove(device);
                _context.SaveChanges();
                return Ok(new ApiResponseMessage("Устройство успешно удалено"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseMessage(ex.Message));
            }
        }
        
        /// <summary>
        /// Удаление компонента
        /// </summary>
        [Route("delete-component/{componentId}")]
        [HttpDelete]
        public IActionResult DeleteComponent(int componentId)
        {
            try
            {
                var component = _context.DeviceComponents.FirstOrDefault(c => c.ComponentId == componentId);
                if (component == null)
                    return NotFound(new ApiResponseMessage("Компонент не найден"));

                _context.DeviceComponents.Remove(component);
                _context.SaveChanges();
                return Ok(new ApiResponseMessage("Компонент успешно удалён"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseMessage(ex.Message));
            }
        }
    }
}