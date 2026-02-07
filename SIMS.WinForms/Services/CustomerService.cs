using System;
using System.Collections.Generic;
using SIMS.WinForms.Data.Repositories;
using SIMS.WinForms.Domain;

namespace SIMS.WinForms.Services
{
    public sealed class CustomerService
    {
        private readonly CustomerRepository _repo = new CustomerRepository();

        public List<Customer> GetAll() => _repo.GetAll();
        public List<Customer> Search(string keyword) => _repo.Search(keyword);

        public long Add(Customer c)
        {
            Validate(c);
            EnsureCodeUniqueForCreate(c.Code);
            return _repo.Add(c);
        }

        public void Update(Customer c)
        {
            if (c.Id <= 0) throw new Exception("Invalid Customer Id.");
            Validate(c);
            EnsureCodeUniqueForUpdate(c.Id, c.Code);
            _repo.Update(c);
        }

        public void Delete(long id)
        {
            if (id <= 0) return;
            _repo.Delete(id);
        }

        public Customer GetById(long id) => _repo.GetById(id);

        private static void Validate(Customer c)
        {
            if (c == null) throw new Exception("Customer is null.");
            if (string.IsNullOrWhiteSpace(c.Code)) throw new Exception("Customer Code is required.");
            if (string.IsNullOrWhiteSpace(c.FullName)) throw new Exception("Customer Name is required.");

            c.Code = c.Code.Trim();
            c.FullName = c.FullName.Trim();
            c.Phone = (c.Phone ?? "").Trim();
            c.Address = (c.Address ?? "").Trim();
        }

        private void EnsureCodeUniqueForCreate(string code)
        {
            var all = _repo.Search(code);
            for (int i = 0; i < all.Count; i++)
            {
                if (string.Equals(all[i].Code, code, StringComparison.OrdinalIgnoreCase))
                    throw new Exception("Customer Code already exists.");
            }
        }

        private void EnsureCodeUniqueForUpdate(long id, string code)
        {
            var all = _repo.Search(code);
            for (int i = 0; i < all.Count; i++)
            {
                if (all[i].Id != id &&
                    string.Equals(all[i].Code, code, StringComparison.OrdinalIgnoreCase))
                    throw new Exception("Customer Code already exists.");
            }
        }
        public int Add(Customer c) => Insert(c);
    }
}
