using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenFeasyo.Platform.Data
{
    public interface DataProvider
    {
        object MakeRequest(string requestUrl, Type t);
        bool MakePostRequest(string requestUrl, string data, string contentType);
        int PostObject(string requestUrl, string data, string contentType);
        bool UploadFile(string requestUrl, string pathToFile);
    }
}
