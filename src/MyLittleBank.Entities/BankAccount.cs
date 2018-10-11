namespace MyLittleBank.Entities
{
    public class BankAccount
    {
        #region Properties

        public int Id { get; set; }

        public string Number { get; set; }

        public decimal Balance { get; set; }

        public bool IsLocked { get; set; }

        #endregion
    }
}
