﻿using MongoDB.Driver;
using TestCase.Models.Database;
using TestCase.Models.Filters;

namespace TestCase.Interfaces.Repositories
{
    public interface ICouponRepository
    {
        Task<CouponDto> CreateAsync(CouponDto coupon);
        Task<CouponDto> GetByIdAsync(Guid? clientId, Guid id);
        Task<IEnumerable<CouponDto>> GetAllAsync(Guid? clientId, CouponFilterModel filter = null);
        Task<bool> UpdateAsync(Guid id, UpdateDefinition<CouponDto> update);
    }
}
