using System.Collections.Generic;

namespace SIMS.WinForms.Data.Repositories
{
    public interface IRepository<T>
    {
        List<T> GetAll();
        List<T> Search(string keyword);
        T GetById(int id);
        int Insert(T entity);
        void Update(T entity);
        void Delete(int id);
    }
}
