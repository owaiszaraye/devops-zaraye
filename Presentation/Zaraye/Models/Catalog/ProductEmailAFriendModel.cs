using System.ComponentModel.DataAnnotations;
using Zaraye.Framework.Models;

namespace Zaraye.Models.Catalog
{
    public partial record ProductEmailAFriendModel : BaseZarayeModel
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public string ProductSeName { get; set; }

        [DataType(DataType.EmailAddress)]
        public string FriendEmail { get; set; }

        [DataType(DataType.EmailAddress)]
        public string YourEmailAddress { get; set; }

        public string PersonalMessage { get; set; }

        public bool SuccessfullySent { get; set; }
        public string Result { get; set; }

        public bool DisplayCaptcha { get; set; }
    }
}