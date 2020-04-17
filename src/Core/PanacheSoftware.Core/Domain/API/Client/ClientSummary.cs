using System;
using System.Collections.Generic;
using System.Text;

namespace PanacheSoftware.Core.Domain.API.Client
{
    public class ClientSummary
    {
        public ClientSummary()
        {
            Id = Guid.Empty;
            ShortName = string.Empty;
            LongName = string.Empty;
            ContactId = Guid.Empty;
            MainContactName = string.Empty;
            Email = string.Empty;
            Phone = string.Empty;
        }

        public Guid Id { get; set; }
        public string ShortName { get; set; }
        public string LongName { get; set; }
        public Guid ContactId { get; set; }
        public string MainContactName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }
}
