using System;

namespace fabric.DAL.Models
{
    public class MaterialTransaction
    {
        private int _id;
        private int _materialId;
        private decimal _quantity;
        private int _userId;
        private int? _productionTaskId;
        private DateTime _timestamp;

        public int Id
        {
            get => _id;
            set => _id = value;
        }

        public int MaterialId
        {
            get => _materialId;
            set => _materialId = value;
        }

        public decimal Quantity
        {
            get => _quantity;
            set => _quantity = value;
        }

        public int UserId
        {
            get => _userId;
            set => _userId = value;
        }

        public int? ProductionTaskId
        {
            get => _productionTaskId;
            set => _productionTaskId = value;
        }

        public DateTime Timestamp
        {
            get => _timestamp;
            set => _timestamp = value;
        }
    }
}
