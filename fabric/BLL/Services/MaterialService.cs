using System.Collections.Generic;
using System.Linq;
using fabric.DAL;
using fabric.DAL.Models;

namespace fabric.BLL.Services
{
    public class MaterialService
    {
        public List<Material> GetAll()
        {
            using (var db = new AppDbContext())
            {
                return db.Materials.OrderBy(m => m.Name).ToList();
            }
        }

        public Material GetById(int id)
        {
            using (var db = new AppDbContext())
            {
                return db.Materials.FirstOrDefault(m => m.Id == id);
            }
        }

        public bool AddMaterial(string name, decimal quantity, string unit)
        {
            if (string.IsNullOrWhiteSpace(name) || quantity < 0) return false;
            using (var db = new AppDbContext())
            {
                var m = new Material
                {
                    Name = name,
                    Quantity = quantity,
                    Unit = unit
                };
                db.Materials.Add(m);
                db.SaveChanges();
                return true;
            }
        }
    }
}
