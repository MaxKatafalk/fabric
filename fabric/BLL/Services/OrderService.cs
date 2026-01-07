using System;
using System.Collections.Generic;
using System.Linq;
using fabric.DAL;
using fabric.DAL.Models;

namespace fabric.BLL.Services
{
    public class OrderService
    {
        public bool CreateOrder(string orderNumber, string customerName, DateTime orderDate, DateTime? dueDate, string notes, int? managerId)
        {
            if (string.IsNullOrWhiteSpace(orderNumber) || string.IsNullOrWhiteSpace(customerName) || string.IsNullOrWhiteSpace(notes))
                return false;

            using (var db = new AppDbContext())
            {
                bool exists = db.Orders.Any(o => o.OrderNumber == orderNumber);
                if (exists) return false;

                db.Orders.Add(new Order
                {
                    OrderNumber = orderNumber,
                    CustomerName = customerName,
                    OrderDate = orderDate,
                    DueDate = dueDate,
                    Status = OrderStatus.InProgress,
                    Notes = notes,
                    ManagerId = managerId
                });
                db.SaveChanges();
                return true;
            }
        }

        public List<Order> GetAll()
        {
            using (var db = new AppDbContext())
            {
                return db.Orders.OrderByDescending(o => o.OrderDate).ToList();
            }
        }

        public Order GetById(int id)
        {
            using (var db = new AppDbContext())
            {
                return db.Orders.FirstOrDefault(o => o.Id == id);
            }
        }

        public bool UpdateStatus(int orderId, OrderStatus newStatus)
        {
            using (var db = new AppDbContext())
            {
                var order = db.Orders.FirstOrDefault(o => o.Id == orderId);
                if (order == null) return false;
                order.Status = newStatus;
                db.SaveChanges();
                return true;
            }
        }
    }
}
