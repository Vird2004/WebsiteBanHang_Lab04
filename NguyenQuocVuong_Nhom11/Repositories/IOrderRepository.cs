﻿using NguyenQuocVuong_Nhom11.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NguyenQuocVuong_Nhom11.Repositories
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetAllAsync();
        Task<Order> GetByIdAsync(int id);
        Task AddAsync(Order order);
        Task UpdateAsync(Order order);
        Task DeleteAsync(int id);
    }
}
