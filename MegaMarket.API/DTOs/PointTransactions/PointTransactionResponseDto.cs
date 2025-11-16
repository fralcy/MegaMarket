namespace MegaMarket.API.DTOs.PointTransactions
{
    public class PointTransactionResponseDto
    {
        public int TransactionId { get; set; }         
        public int CustomerId { get; set; }            
        public string? CustomerName { get; set; }      
        public int? InvoiceId { get; set; }            
        public int PointChange { get; set; }           
        public string? TransactionType { get; set; }   
        public DateTime CreatedAt { get; set; }        
        public string? Description { get; set; }       
    }
}
