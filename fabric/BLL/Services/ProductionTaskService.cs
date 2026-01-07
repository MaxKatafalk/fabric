using System;
using System.Collections.Generic;
using System.Linq;
using fabric.DAL;
using fabric.DAL.Models;
using System.Windows.Forms;

namespace fabric.BLL.Services
{
    public class ProductionTaskService
    {
        public bool CreateTask(int orderId, string description, int quantityAssigned, int assignedToUserId, int assignedByUserId, int? materialId, decimal quantityPerUnit)
        {
            if (orderId <= 0 || quantityAssigned <= 0 || assignedToUserId <= 0) return false;

            using (var db = new AppDbContext())
            {
                var task = new ProductionTask
                {
                    OrderId = orderId,
                    Description = description,
                    QuantityAssigned = quantityAssigned,
                    QuantityCompleted = 0,
                    AssignedToUserId = assignedToUserId,
                    AssignedByUserId = assignedByUserId,
                    Status = ProductionTaskStatus.Assigned,
                    StartDate = DateTime.Now,
                    MaterialId = materialId,
                    QuantityPerUnit = quantityPerUnit
                };
                db.ProductionTasks.Add(task);

                var order = db.Orders.FirstOrDefault(o => o.Id == orderId);
                if (order != null && order.Status != OrderStatus.InProgress)
                {
                    order.Status = OrderStatus.InProgress;
                }

                db.SaveChanges();
                try { fabric.AppEvents.RaiseOrderStatusChanged(); } catch { }
                return true;
            }
        }

        public List<ProductionTask> GetTasksByAssignedUser(int userId)
        {
            using (var db = new AppDbContext())
            {
                return db.ProductionTasks.Where(t => t.AssignedToUserId == userId).OrderByDescending(t => t.StartDate).ToList();
            }
        }

        public List<ProductionTask> GetTasksByOrder(int orderId)
        {
            using (var db = new AppDbContext())
            {
                return db.ProductionTasks.Where(t => t.OrderId == orderId).ToList();
            }
        }

        public ProductionTask GetById(int id)
        {
            using (var db = new AppDbContext())
            {
                return db.ProductionTasks.FirstOrDefault(t => t.Id == id);
            }
        }

        public bool UpdateProgress(int taskId, int quantityCompleted, int performedByUserId)
        {
            if (quantityCompleted <= 0) return false;

            using (var db = new AppDbContext())
            {
                var task = db.ProductionTasks.FirstOrDefault(t => t.Id == taskId);
                if (task == null) return false;

                int remaining = task.QuantityAssigned - task.QuantityCompleted;
                int toAdd = Math.Min(quantityCompleted, remaining);
                if (toAdd <= 0) return false;

                task.QuantityCompleted += toAdd;

                if (task.QuantityCompleted >= task.QuantityAssigned)
                {
                    task.QuantityCompleted = task.QuantityAssigned;
                    task.Status = ProductionTaskStatus.Completed;
                    task.EndDate = DateTime.Now;
                }
                else
                {
                    task.Status = ProductionTaskStatus.InProgress;
                }

                db.SaveChanges();

                var order = db.Orders.FirstOrDefault(o => o.Id == task.OrderId);
                if (order != null)
                {
                    bool hasUnfinishedTasks = db.ProductionTasks
                        .Any(t => t.OrderId == order.Id && t.QuantityCompleted < t.QuantityAssigned);


                    if (!hasUnfinishedTasks)
                    {
                        order.Status = OrderStatus.Completed;
                        db.SaveChanges();
                    }
                }

                AppEvents.RaiseOrderStatusChanged();

                return true;
            }
        }



    }
}
