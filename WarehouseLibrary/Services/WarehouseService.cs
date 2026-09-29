using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using WarehouseData.Models;

namespace WarehouseLibrary.Services
{
    public class WarehouseService : IWarehouseService
    {
        private List<Warehouse> _warehouses = new List<Warehouse>();

        public void SeedTestData(List<Organization> organizations)
        {
            if (!_warehouses.Any() && organizations.Any())
            {
                var org1 = organizations[0];
                var org2 = organizations[1];

                _warehouses.Add(new Warehouse(
                    "Склад №1 Рога и Копыта",
                    "г. Москва, ул. Складская, 1",
                    org1.OrgId));

                _warehouses.Add(new Warehouse(
                    "Склад №2 Рога и Копыта",
                    "г. Москва, ул. Заводская, 5",
                    org1.OrgId));

                _warehouses.Add(new Warehouse(
                    "Склад СтройДом",
                    "г. Санкт-Петербург, пр. Строителей, 10",
                    org2.OrgId));
            }
        }

        public List<Warehouse> GetByOrganizationId(BigInteger organizationId)
        {
            return _warehouses.Where(w => w.OrgId == organizationId).ToList();
        }

        public List<Warehouse> GetAll()
        {
            return _warehouses.ToList();
        }

        public Warehouse GetById(BigInteger id)
        {
            return _warehouses.FirstOrDefault(w => w.WhId == id);
        }

        public Warehouse Create(string name, string address, BigInteger organizationId)
        {
            var warehouse = new Warehouse(name, address, organizationId);
            _warehouses.Add(warehouse);
            return warehouse;
        }

        public Warehouse Update(BigInteger id, string name, string address)
        {
            var warehouse = GetById(id);
            if (warehouse != null)
            {
                warehouse.WhName = name;
                warehouse.WhAddress = address;
            }
            return warehouse;
        }

        public bool Delete(BigInteger id)
        {
            var warehouse = GetById(id);
            if (warehouse != null)
            {
                _warehouses.Remove(warehouse);
                return true;
            }
            return false;
        }
    }
}