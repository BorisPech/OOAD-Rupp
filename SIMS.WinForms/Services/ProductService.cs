using System;
using System.Collections.Generic;
using SIMS.WinForms.Data.Repositories;
using SIMS.WinForms.Domain;

namespace SIMS.WinForms.Services
{
    public sealed class ProductService
    {
        private readonly ProductRepository _repo = new ProductRepository();

        public List<Product> GetAll() => _repo.GetAll();
        public List<Product> Search(string keyword) => _repo.Search(keyword);

        public long Add(Product p)
        {
            Validate(p);
            EnsureCodeUniqueForCreate(p.Code);
            return _repo.Add(p);
        }

        public void Update(Product p)
        {
            if (p.Id <= 0) throw new Exception("Invalid Product Id.");
            Validate(p);
            EnsureCodeUniqueForUpdate(p.Id, p.Code);
            _repo.Update(p);
        }

        public void Delete(long id)
        {
            if (id <= 0) return;
            _repo.Delete(id);
        }

        public Product GetById(long id) => _repo.GetById(id);

        private static void Validate(Product p)
        {
            if (p == null) throw new Exception("Product is null.");
            if (string.IsNullOrWhiteSpace(p.Code)) throw new Exception("Product Code is required.");
            if (string.IsNullOrWhiteSpace(p.Name)) throw new Exception("Product Name is required.");
            if (string.IsNullOrWhiteSpace(p.Category)) p.Category = "General";

            if (p.Cost < 0) throw new Exception("Cost must be >= 0.");
            if (p.Price < 0) throw new Exception("Price must be >= 0.");
            if (p.Stock < 0) throw new Exception("Stock must be >= 0.");
            if (p.ReorderLevel < 0) p.ReorderLevel = 0;

            p.Code = p.Code.Trim();
            p.Name = p.Name.Trim();
            p.Category = p.Category.Trim();
        }

        private void EnsureCodeUniqueForCreate(string code)
        {
            var all = _repo.Search(code);
            for (int i = 0; i < all.Count; i++)
            {
                if (string.Equals(all[i].Code, code, StringComparison.OrdinalIgnoreCase))
                    throw new Exception("Product Code already exists.");
            }
        }

        private void EnsureCodeUniqueForUpdate(long id, string code)
        {
            var all = _repo.Search(code);
            for (int i = 0; i < all.Count; i++)
            {
                if (all[i].Id != id &&
                    string.Equals(all[i].Code, code, StringComparison.OrdinalIgnoreCase))
                    throw new Exception("Product Code already exists.");
            }
        }
    }
}
