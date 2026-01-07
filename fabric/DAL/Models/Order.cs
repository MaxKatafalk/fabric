using System;

namespace fabric.DAL.Models
{
    public enum OrderStatus
    {
        InProgress = 1,
        Completed = 2
    }

    public class Order
    {
        private int _id;
        private string _orderNumber;
        private string _customerName;
        private DateTime _orderDate;
        private DateTime? _dueDate;
        private OrderStatus _status;
        private string _notes;
        private int? _managerId;

        public int Id
        {
            get => _id;
            set => _id = value;
        }

        public string OrderNumber
        {
            get => _orderNumber;
            set => _orderNumber = value;
        }

        public string CustomerName
        {
            get => _customerName;
            set => _customerName = value;
        }

        public DateTime OrderDate
        {
            get => _orderDate;
            set => _orderDate = value;
        }

        public DateTime? DueDate
        {
            get => _dueDate;
            set => _dueDate = value;
        }

        public OrderStatus Status
        {
            get => _status;
            set => _status = value;
        }

        public string Notes
        {
            get => _notes;
            set => _notes = value;
        }

        public int? ManagerId
        {
            get => _managerId;
            set => _managerId = value;
        }
    }
}
