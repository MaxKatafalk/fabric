using System.Collections.Generic;
using System.Linq;
using fabric.DAL;
using fabric.DAL.Models;

namespace fabric.BLL.Services
{
    public class MaterialService
    {
        public bool AddMaterial(string name, decimal quantity, string unit)
        {
            if (string.IsNullOrWhiteSpace(name) || quantity < 0)
                return false;

            using (var db = new AppDbContext())
            {
                var exists = db.Materials.FirstOrDefault(m => m.Name == name && m.Unit == unit);
                if (exists != null)
                {
                    exists.Quantity += quantity;
                }
                else
                {
                    db.Materials.Add(new Material
                    {
                        Name = name,
                        Quantity = quantity,
                        Unit = unit
                    });
                }
                db.SaveChanges();
                return true;
            }
        }

        public List<Material> GetAll()
        {
            using (var db = new AppDbContext())
            {
                return db.Materials.ToList();
            }
        }

        public Material GetById(int id)
        {
            using (var db = new AppDbContext())
            {
                return db.Materials.FirstOrDefault(m => m.Id == id);
            }
        }

        public bool AdjustQuantity(int materialId, decimal delta)
        {
            using (var db = new AppDbContext())
            {
                var mat = db.Materials.FirstOrDefault(m => m.Id == materialId);
                if (mat == null) return false;
                decimal newQty = mat.Quantity + delta;
                if (newQty < 0) return false;
                mat.Quantity = newQty;
                db.SaveChanges();
                return true;
            }
        }
    }
}
