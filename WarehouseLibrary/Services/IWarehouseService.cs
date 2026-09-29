using System;
using System.Collections.Generic;
using System.Numerics;
using WarehouseData.Models;

namespace WarehouseLibrary.Services
{
    public interface IWarehouseService
    {
        List<Warehouse> GetByOrganizationId(BigInteger organizationId);
        List<Warehouse> GetAll();
        Warehouse GetById(BigInteger id);
        Warehouse Create(string name, string address, BigInteger organizationId);
        Warehouse Update(BigInteger id, string name, string address);
        bool Delete(BigInteger id);
        void SeedTestData(List<Organization> organizations);
    }
}