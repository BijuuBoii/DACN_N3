using System.ComponentModel.DataAnnotations;

namespace DACN_N3.Models.Momo
{
	public class MomoInfoModel
	{
		[Key]
		public int Id { get; set; }
		public string OrderId { get; set; }
		public string OrderInfo { get; set; }
		public string FullName { get; set; }
		public decimal Amount { get; set; }
        public string PaymentType { get; set; }
        public DateTime DatePaid { get; set; }
	}
}
