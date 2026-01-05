using System;

namespace fabric.DAL.Models
{
    public class Material
    {
        private int _id;
        private string _name;
        private decimal _quantity;
        private string _unit;

        public int Id
        {
            get => _id;
            set => _id = value;
        }

        public string Name
        {
            get => _name;
            set => _name = value;
        }

        public decimal Quantity
        {
            get => _quantity;
            set => _quantity = value;
        }

        public string Unit
        {
            get => _unit;
            set => _unit = value;
        }
    }
}
