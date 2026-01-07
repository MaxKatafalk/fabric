using System;

namespace fabric.DAL.Models
{
    public class ProductionTask
    {
        private int _id;
        private int _orderId;
        private string _description;
        private int _assignedToUserId;
        private int _assignedByUserId;
        private int _quantityAssigned;
        private int _quantityCompleted;
        private ProductionTaskStatus _status;
        private DateTime _startDate;
        private DateTime? _endDate;
        private int? _materialId;
        private decimal _quantityPerUnit;

        public int Id
        {
            get => _id;
            set => _id = value;
        }

        public int OrderId
        {
            get => _orderId;
            set => _orderId = value;
        }

        public string Description
        {
            get => _description;
            set => _description = value;
        }

        public int AssignedToUserId
        {
            get => _assignedToUserId;
            set => _assignedToUserId = value;
        }

        public int AssignedByUserId
        {
            get => _assignedByUserId;
            set => _assignedByUserId = value;
        }

        public int QuantityAssigned
        {
            get => _quantityAssigned;
            set => _quantityAssigned = value;
        }

        public int QuantityCompleted
        {
            get => _quantityCompleted;
            set => _quantityCompleted = value;
        }

        public ProductionTaskStatus Status
        {
            get => _status;
            set => _status = value;
        }

        public DateTime StartDate
        {
            get => _startDate;
            set => _startDate = value;
        }

        public DateTime? EndDate
        {
            get => _endDate;
            set => _endDate = value;
        }

        public int? MaterialId
        {
            get => _materialId;
            set => _materialId = value;
        }

        public decimal QuantityPerUnit
        {
            get => _quantityPerUnit;
            set => _quantityPerUnit = value;
        }
    }
}
