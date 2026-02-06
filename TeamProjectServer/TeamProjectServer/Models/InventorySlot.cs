namespace TeamProjectServer.Models
{
    public class InventorySlot
    {
        public int ID { get; set; }
        public int Stack { get; set; }
        public bool IsLocked { get; set; }
    }
}
