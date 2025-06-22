using System.ComponentModel.DataAnnotations.Schema;

namespace Chill_Closet.Models
{
    public class ReturnRequestImage
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public int ReturnRequestId { get; set; }
        [ForeignKey("ReturnRequestId")]
        public ReturnRequest ReturnRequest { get; set; }
    }
}