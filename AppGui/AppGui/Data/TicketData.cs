namespace AppGui.Data
{
    public class TicketData
    {

        private string letter;
        private string description;
        private int latest;
        private int averageAtendingTime; 
        private int averageWaitingTime;
        private int clientsWaiting;
        private bool enabled;

        public TicketData(string letter, string description, bool enabled) {
            this.letter = letter;
            this.description = description;
            this.enabled = enabled;
        }
        public string Letter
        {
            get
            {
                return letter;
            }

            set
            {
                letter = value;
            }
        }

        public string Description
        {
            get
            {
                return description;
            }

            set
            {
                description = value;
            }
        }

        public int Latest
        {
            get
            {
                return latest;
            }

            set
            {
                latest = value;
            }
        }

        public int AverageAtendingTime
        {
            get
            {
                return averageAtendingTime;
            }

            set
            {
                averageAtendingTime = value;
            }
        }

        public bool Enabled
        {
            get
            {
                return enabled;
            }

            set
            {
                enabled = value;
            }
        }

        public int AverageWaitingTime
        {
            get
            {
                return averageWaitingTime;
            }

            set
            {
                averageWaitingTime = value;
            }
        }

        public int ClientsWaiting
        {
            get
            {
                return clientsWaiting;
            }

            set
            {
                clientsWaiting = value;
            }
        }
    }
}