namespace MarketplaceBack.Contracts.Exeptions
{
    public class DbInitializationException : Exception
    {
        public DbInitializationException(string? massege) : base(massege) { }

        public DbInitializationException(string? massege, Exception? innerException) : base(massege, innerException) { }
    }
}
