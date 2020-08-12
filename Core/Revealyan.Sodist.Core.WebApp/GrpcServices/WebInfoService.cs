using Grpc.Core;
using Revealyan.Sodist.Core.Protos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Revealyan.Sodist.Core.WebApp.GrpcServices
{
    public class WebAppInfoGrpcService : WebAppInfo.WebAppInfoBase
    {
        #region data

        #endregion

        #region ctors
        public WebAppInfoGrpcService()
        {

        }
        #endregion

        #region WebAppInfoBase
        public async override Task<InfoResponse> GetInfo(InfoRequest request, ServerCallContext context)
        {
            var result = new InfoResponse();
            //TODO: Реализовать получение информации о приложении
            await Task.Delay(1);
            return result;
        }
        #endregion
    }
}