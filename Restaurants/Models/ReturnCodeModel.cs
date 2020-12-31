using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Restaurants.Models
{
    public class ReturnCodeModel
    {
        private Int32 _recordId;
        private String _returnCode;
        private String _message;

        public int RecordId { get => _recordId; set => _recordId = value; }
        public string ReturnCode { get => _returnCode; set => _returnCode = value; }
        public string Message { get => _message; set => _message = value; }
    }
}