using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using protocol.vo;

namespace protocol
{
    public class ProtocolVOCreater
    {

        private Dictionary<int, Func<BaseVO>> dic_vo = new Dictionary<int, Func<BaseVO>>();

        public ProtocolVOCreater()
        {
            RegisterCreater(Protocol.sendString, sendStringVO.CreateInstance);
        }

        public void RegisterCreater(int protocolId, Func<BaseVO> packageCreateFun)
        {
            dic_vo[protocolId] = packageCreateFun;
        }

    }
}