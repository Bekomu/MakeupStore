using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class CheckoutViewModel
    {
        public BasketViewModel Basket { get; set; }

        [Required(ErrorMessage = "Required")]
        [MaxLength(180, ErrorMessage = "Too long")]
        public string Street { get; set; }

        [Required(ErrorMessage = "Required")]
        [MaxLength(100, ErrorMessage = "Too long")]
        public string City { get; set; }

        [MaxLength(60, ErrorMessage = "Too long")]
        public string State { get; set; }

        [Required(ErrorMessage = "Required")]
        [MaxLength(180, ErrorMessage = "Too long")]
        public string Country { get; set; }

        [Required(ErrorMessage = "Required")]
        [MaxLength(18, ErrorMessage = "Too long")]
        public string ZipCode { get; set; }

        [Required(ErrorMessage = "Required")]
        public string CardHolder { get; set; }

        [Required(ErrorMessage = "Required")]
        [CreditCard(ErrorMessage = "Invalid")]
        public string CardNumber { get; set; }

        [Required(ErrorMessage = "Required")]
        [RegularExpression(@"^(0[1-9]|1[1-2])\/[0-9]{2}$", ErrorMessage = "Invalid")]
        public string CardExpire { get; set; }

        [Required(ErrorMessage = "Required")]
        [RegularExpression("^[0-9]{3,4}$", ErrorMessage = "Invalid")]
        public string CardCvv { get; set; }
    }
}
