using System;
using System.Collections.Generic;
using System.Numerics;
using WarehouseData.Models;

namespace WarehouseLibrary.Services
{
    public interface IOrganizationService
    {
        List<Organization> GetAll();
        Organization GetById(BigInteger id);
        Organization Create(string name);
        Organization Update(BigInteger id, string name);
        bool Delete(BigInteger id);
        void SeedTestData();
    }
}