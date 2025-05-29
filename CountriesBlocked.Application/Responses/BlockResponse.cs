using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CountriesBlocked.Application.Responses
{
    public class BlockResponse<T>
    {
        public T Entity { get; set; } 

        public string Message {  get; set; }

        public HttpStatusCode Status {  get; set; }
        public BlockResponse()
        {
            
        }
        public BlockResponse(T entity,string message,HttpStatusCode status)
        {
            Entity = entity;
            Message = message;
            Status = status;
        }
    }
}
