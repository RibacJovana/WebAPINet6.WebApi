using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPINet6.WebApi;

namespace WebAPINet6.BusinessLogic.Services.Interfaces
{
    public interface IDataTaker
    {
        public Task<List<SymbolInfo>> GetSymbolsInfo(string[] ids);
    }
}
