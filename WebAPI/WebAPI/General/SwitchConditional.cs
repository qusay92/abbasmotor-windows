namespace WebAPI.General
{
    public static class SwitchConditional
    {
        public static bool SwitchConditionalCarStatus(Auto c, Container container ,int carStatus)
        {
            switch ((CarStatus)carStatus)
            {
                case CarStatus.BoughtNew:
                    {
                        return (c.ContainerId == null || (container.DepartureDate > DateTime.Now && container.ArrivalDate > DateTime.Now));
                    }
                case CarStatus.Loaded:
                    {
                        return (c.ContainerId != null && container.DepartureDate <= DateTime.Now && container.ArrivalDate > DateTime.Now);
                    }
                case CarStatus.Arrived:
                    {
                        return (c.ContainerId != null && container.ArrivalDate <= DateTime.Now);
                    }
                default:
                    return true;
            }
        }


        public static bool SwitchConditionalContainerStatus(Container c, int containerStatus)
        {
            switch ((ContainerStatus)containerStatus)
            {
                case ContainerStatus.AwaitingLoad:
                    {
                        return ((c.DepartureDate > DateTime.Now && c.ArrivalDate > DateTime.Now));
                    }
                case ContainerStatus.Departured:
                    {
                        return (c.DepartureDate <= DateTime.Now && c.ArrivalDate > DateTime.Now);
                    }
                case ContainerStatus.Arrived:
                    {
                        return (c.ArrivalDate <= DateTime.Now);
                    }
                default:
                    return true;
            }
        }
    }
}
