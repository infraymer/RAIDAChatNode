using System;
using System.ComponentModel.DataAnnotations;

namespace RAIDAChatNode.DTO
{
    public class GroupInfo: ITransactionInfo
    {
        public string name { get; set; }
        public Guid publicId { get; set; }
        public bool oneToOne { get; set; }
        public Guid transactionId { get; set; }
    }
}