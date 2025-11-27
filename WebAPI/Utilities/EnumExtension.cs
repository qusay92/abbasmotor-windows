using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Utilities.Enums.Enum;

namespace Utilities
{
    public class EnumExtension
    {

        public string SwitchCarType(int value)
        {
            switch (value)
            {
                case (int)CarType.Car:
                    return "car";
                case (int)CarType.Van:
                    return "van";
                default:
                    return "car";
            }
        }

        public string SwitchCarStatus(int value)
        {
            switch (value)
            {
                case (int)CarStatus.BoughtNew:
                    return "Bought New";
                case (int)CarStatus.Loaded:
                    return "Loaded";
                case (int)CarStatus.Arrived:
                    return "Arrived";
                default:
                    return "Bought New";
            }
        }

        public string SwitchPaperStatus(int value)
        {
            switch (value)
            {
                case (int)PaperStatus.Ready:
                    return "ready";
                case (int)PaperStatus.Waiting:
                    return "waiting";
                case (int)PaperStatus.Retrieval:
                    return "retrieval";
                default:
                    return "ready";
            }
        }

        public string SwitchBuyType(int value)
        {
            switch (value)
            {
                case (int)BuyType.PurchasePrice:
                    return "purchaseprice";
                case (int)BuyType.SeaFreight:
                    return "seafreight";
                case (int)BuyType.InnerFreight:
                    return "innerfreight";
                case (int)BuyType.Fees:
                    return "fees";
                case (int)BuyType.PurchaseOrder:
                    return "purchaseorder";
                case (int)BuyType.Commission:
                    return "commission";
                case (int)BuyType.CustomsClearance:
                    return "customsclearance";
                case (int)BuyType.StorageFees:
                    return "storagefees";
                case (int)BuyType.Other:
                    return "other";
                default:
                    return "purchaseprice";
            }
        }

        public string SwitchContainerStatus(int value)
        {
            switch (value)
            {
                case (int)ContainerStatus.AwaitingLoad:
                    return "awaitingload";
                case (int)ContainerStatus.Departured:
                    return "departured";
                case (int)ContainerStatus.Arrived:
                    return "arrived";
                default:
                    return "awaitingload";
            }
        }

        public StatusHook<string, string> SwitchStatusByDate(DateTime? departureDate, DateTime? arrivalDate, bool isAuto = false)
        {
            StatusHook<string, string> result = new StatusHook<string, string>()
            {
                Status = !isAuto ? "Awaiting Load" : "Bought New",
                Color = ""
            };

            if (departureDate.HasValue)
            {
                if (departureDate.Value <= DateTime.Now)
                {
                    result.Status = !isAuto ? "Departured" : "Loaded";
                    result.Color = "#e3f5fc";
                }
            }

            if (arrivalDate.HasValue)
            {
                if (arrivalDate.Value <= DateTime.Now)
                {
                    result.Status = !isAuto ? "Arrived" : "Arrived";
                    result.Color = "#00e35526";
                }
            }

            return result;
        }

    }
}
