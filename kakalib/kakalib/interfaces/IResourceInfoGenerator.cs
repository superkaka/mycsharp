using System;
using System.Collections.Generic;
using System.Text;
using KLib.structs;

namespace KLib.interfaces
{
	public interface IResourceInfoGenerator
	{

        String resourceInfoFileName
        {
            get;
        }

        String buildVersionFileName
        {
            get;
        }

        Byte[] resourceInfoToBytes(ResourceInfo[] resInfoList);

        Byte[] buildVersionToBytes(String buildVersion);
        
           
	}
}
