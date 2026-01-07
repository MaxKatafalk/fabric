using System;
using System.Linq;
using fabric.DAL;
using fabric.DAL.Models;

namespace fabric.BLL.Services
{
    public class MaterialTransactionService
    {
        public bool RecordTransaction(int materialId, decimal quantity, int userId, int? productionTaskId)
        {
            if (materialId <= 0) return false;
            using (var db = new AppDbContext())
            {
                var mat = db.Materials.FirstOrDefault(m => m.Id == materialId);
                if (mat == null) return false;
                decimal newQty = mat.Quantity + quantity;
                if (newQty < 0) return false;
                mat.Quantity = newQty;
                var tr = new MaterialTransaction
                {
                    MaterialId = materialId,
                    Quantity = quantity,
                    UserId = userId,
                    ProductionTaskId = productionTaskId,
                    Timestamp = DateTime.Now
                };
                db.MaterialTransactions.Add(tr);
                db.SaveChanges();
                return true;
            }
        }

        public System.Collections.Generic.List<MaterialTransaction> GetByMaterial(int materialId)
        {
            using (var db = new AppDbContext())
            {
                return db.MaterialTransactions
                    .Where(t => t.MaterialId == materialId)
                    .OrderByDescending(t => t.Timestamp)
                    .ToList();
            }
        }
    }
}
