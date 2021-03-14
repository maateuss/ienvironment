using iEnvironment.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iEnvironment.RestAPI.Services
{
    public class EquipmentService : BaseService<Equipment>
    {
        public EquipmentService() : base("equipments")
        {

        }
    }
}
