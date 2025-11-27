using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Enums
{
    public class Enum
    {
        public enum UserType
        {
            Admin = 1,
            SuperiorAdmin = 2,
            Client = 3
        }

        public enum StatusType
        {
            Failed = 0,
            Success = 1
        }

        public enum CarStatus
        {
            [Display(Name = "Bought New")]
            BoughtNew = 1,
            [Display(Name = "Loaded")]
            Loaded = 2,
            [Display(Name = "Arrived")]
            Arrived = 3
        }

        public enum DisplayStatus
        {
            General = 1,
            Private = 2
        }

        public enum PaperStatus
        {
            Ready = 1,
            Waiting = 2,
            Retrieval = 3
        }

        public enum CarType
        {
            Car = 1,
            Van = 2
        }

        public enum PaymentType
        {
            Credit = 1,
            Debit = 2
        }

        public enum BuyType
        {
            PurchasePrice = 1,
            SeaFreight = 2,
            InnerFreight = 3,
            Fees = 4,
            PurchaseOrder = 5,
            Commission = 6,
            CustomsClearance = 7,
            StorageFees = 8,
            Other = 9,
        }

        public enum CashType
        {
            Cash = 1,
            Transfer = 2
        }

        public enum ContainerStatus
        {
            AwaitingLoad = 1,
            Departured = 2,
            Arrived = 3
        }

        public enum UploadType
        {
            Auto = 1,
            Container = 2
        }
    }
}
