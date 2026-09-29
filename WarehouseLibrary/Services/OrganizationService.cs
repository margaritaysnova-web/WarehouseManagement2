using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using WarehouseData.Models;

namespace WarehouseLibrary.Services
{
    public class OrganizationService : IOrganizationService
    {
        private List<Organization> _organizations = new List<Organization>();

        public void SeedTestData()
        {
            if (!_organizations.Any())
            {
                var org1 = new Organization("ООО Рога и Копыта");
                var org2 = new Organization("ЗАО СтройДом");
                var org3 = new Organization("НА Холодец");

                _organizations.Add(org1);
                _organizations.Add(org2);
                _organizations.Add(org3);
            }
        }

        public List<Organization> GetAll()
        {
            return _organizations.ToList();
        }

        public Organization GetById(BigInteger id)
        {
            return _organizations.FirstOrDefault(o => o.OrgId == id);
        }

        public Organization Create(string name)
        {
            var org = new Organization(name);
            _organizations.Add(org);
            return org;
        }

        public Organization Update(BigInteger id, string name)
        {
            var org = GetById(id);
            if (org != null)
            {
                org.OrgName = name;
            }
            return org;
        }

        public bool Delete(BigInteger id)
        {
            var org = GetById(id);
            if (org != null)
            {
                _organizations.Remove(org);
                return true;
            }
            return false;
        }
    }
}