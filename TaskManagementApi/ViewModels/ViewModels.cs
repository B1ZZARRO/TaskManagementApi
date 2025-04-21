using System;
using System.Collections.Generic;

namespace TaskManagementApi.ViewModels
{
    public class UserAuthData
    {
        public string Login { get; set; }
        public string Password { get; set; }
    }
    
    public class TaskCreationData
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? Deadline { get; set; }
        public int? PriorityId { get; set; }
        public int? StatusId { get; set; }
        public int TaskCreatorId { get; set; }
    }
    
    public class DeviceData
    {
        public int DeviceId { get; set; }
        public string DeviceName { get; set; }
        public string DeviceModel { get; set; }
        public List<ComponentData> Components { get; set; }
    }

    public class ComponentData
    {
        public int ComponentId { get; set; }
        public string ComponentName { get; set; }
        public int DeviceId { get; set; }
        public List<ComponentItemData> Items { get; set; }
    }

    public class ItemsData
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public string ItemType { get; set; }
        public int Quantity { get; set; }
        public List<ComponentData> Components { get; set; }
    }

    public class ComponentItemData
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public string ItemType { get; set; }
        public int QuantityOnStorage { get; set; }
        public int QuantityNeeded { get; set; }
    }
    
    
    public class AddDeviceWithComponentIdsModel
    {
        public string DeviceName { get; set; }
        public string DeviceModel { get; set; }
        public List<int> ComponentIds { get; set; }
    }
    
    public class ComponentItemInputModel
    {
        public int ItemId { get; set; }
        public int QuantityNeeded { get; set; }
    }
    public class AddComponentModel
    {
        public int DeviceId { get; set; }
        public string ComponentName { get; set; }
        public List<ComponentItemInputModel> Items { get; set; }
    }
    
    public class AddStorageItemModel
    {
        public string ItemName { get; set; }
        public string ItemType { get; set; }
        public int Quantity { get; set; }
    }
    
    public class AddMovementModel
    {
        public int ItemId { get; set; }
        public string MovementType { get; set; }
        public int Quantity { get; set; }
        public int? RelatedTaskId { get; set; }
    }
}